using EyeCareSearch.API.Configurations;
using EyeCareSearch.API.Interfaces;
using EyeCareSearch.API.Models;
using Microsoft.Extensions.Options;
using OpenSearch.Client;
using System.Text.Json;

namespace EyeCareSearch.API.Services;

public class SearchService : ISearchService
{
    private readonly IOpenSearchClient _client;
    private readonly OpenSearchSettings _settings;

    public SearchService(
        IOpenSearchService openSearchService,
        IOptions<OpenSearchSettings> options)
    {
        _client = openSearchService.Client;
        _settings = options.Value;
    }

    public async Task<bool> CreateIndexAsync()
    {
        var existsResponse = await _client.Indices.ExistsAsync(_settings.IndexName);

        if (existsResponse.Exists)
        {
            return true;
        }

        var response = await _client.Indices.CreateAsync(
            _settings.IndexName,
            c => c.Map<Patient>(m => m.AutoMap()));

        return response.IsValid;

    }

    public async Task<bool> SeedPatientsAsync()
    {
        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "SampleData",
            "patients.json");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"patients.json not found at {filePath}");
        }

        var json = await File.ReadAllTextAsync(filePath);

        var patients = JsonSerializer.Deserialize<List<Patient>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (patients == null || patients.Count == 0)
        {
            throw new Exception("No patients found in JSON.");
        }

        // Delete existing documents so you don't get duplicates
        await _client.DeleteByQueryAsync<Patient>(d => d
            .Index(_settings.IndexName)
            .Query(q => q.MatchAll()));

        await _client.Indices.RefreshAsync(_settings.IndexName);

        var bulkDescriptor = new BulkDescriptor();

        foreach (var patient in patients)
        {
            bulkDescriptor.Index<Patient>(op => op
                .Index(_settings.IndexName)
                .Id(patient.PatientId)
                .Document(patient));
        }

        var bulkResponse = await _client.BulkAsync(bulkDescriptor);

        await _client.Indices.RefreshAsync(_settings.IndexName);

        Console.WriteLine($"JSON Count: {patients.Count}");
        Console.WriteLine($"Bulk Items: {bulkResponse.Items.Count}");
        Console.WriteLine($"Errors: {bulkResponse.Errors}");

        foreach (var item in bulkResponse.ItemsWithErrors)
        {
            Console.WriteLine(item.Error?.Reason);
        }

        if (!bulkResponse.IsValid)
        {
            Console.WriteLine(bulkResponse.DebugInformation);
        }

        return bulkResponse.IsValid;
    }

    public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchText)
    {
        var response = await _client.SearchAsync<Patient>(s => s
            .Index(_settings.IndexName)
            .Size(100)
            .Query(q => q
                .MultiMatch(m => m
                    .Query(searchText)
                    .Fields(f => f
                        .Field(p => p.Name)
                        .Field(p => p.Doctor)
                        .Field(p => p.Diagnosis)
                    )
                )
            )
            .Highlight(h => h
                .PreTags("<mark>")
                .PostTags("</mark>")
                .Fields(
                    hf => hf.Field(p => p.Name),
                    hf => hf.Field(p => p.Doctor),
                    hf => hf.Field(p => p.Diagnosis)
                )
            )
        );

        foreach (var hit in response.Hits)
        {
            var patient = hit.Source;

            if (hit.Highlight.TryGetValue("name", out var nameHighlight))
            {
                patient.HighlightName = nameHighlight.FirstOrDefault();
            }

            if (hit.Highlight.TryGetValue("doctor", out var doctorHighlight))
            {
                patient.HighlightDoctor = doctorHighlight.FirstOrDefault();
            }

            if (hit.Highlight.TryGetValue("diagnosis", out var diagnosisHighlight))
            {
                patient.HighlightDiagnosis = diagnosisHighlight.FirstOrDefault();
            }
        }

        return response.Hits.Select(h => h.Source);
    }

    public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
    {
        var response = await _client.SearchAsync<Patient>(s => s
            .Index(_settings.IndexName)
            .Size(100)
            .Query(q => q.MatchAll()));

        return response.Documents;
    }
}