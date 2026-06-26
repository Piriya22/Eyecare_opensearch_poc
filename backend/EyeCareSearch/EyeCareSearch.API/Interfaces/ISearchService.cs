using EyeCareSearch.API.Models;

namespace EyeCareSearch.API.Interfaces;

public interface ISearchService
{
    Task<bool> CreateIndexAsync();

    Task<bool> SeedPatientsAsync();

    Task<IEnumerable<Patient>> GetAllPatientsAsync();

    Task<IEnumerable<Patient>> SearchPatientsAsync(string searchText);
}