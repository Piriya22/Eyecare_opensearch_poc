using EyeCareSearch.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EyeCareSearch.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpPost("create-index")]
    public async Task<IActionResult> CreateIndex()
    {
        var created = await _searchService.CreateIndexAsync();

        if (!created)
            return BadRequest("Failed to create index.");

        return Ok("Index created successfully.");
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedPatients()
    {
        var success = await _searchService.SeedPatientsAsync();

        if (!success)
        {
            return BadRequest("Failed to seed patient data.");
        }

        return Ok(new
        {
            Message = "Patient data seeded successfully."
        });
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest("Search text is required.");
        }

        var patients = await _searchService.SearchPatientsAsync(q);

        return Ok(patients);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _searchService.GetAllPatientsAsync();
        return Ok(patients);
    }
}