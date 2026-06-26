using OpenSearch.Client;

namespace EyeCareSearch.API.Interfaces;

public interface IOpenSearchService
{
    IOpenSearchClient Client { get; }
}