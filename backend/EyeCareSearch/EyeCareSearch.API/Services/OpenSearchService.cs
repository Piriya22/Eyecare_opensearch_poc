using EyeCareSearch.API.Configurations;
using EyeCareSearch.API.Interfaces;
using Microsoft.Extensions.Options;
using OpenSearch.Client;

namespace EyeCareSearch.API.Services;

public class OpenSearchService : IOpenSearchService
{
    public IOpenSearchClient Client { get; }

    public OpenSearchService(IOptions<OpenSearchSettings> options)
    {
        var settings = options.Value;

        var connectionSettings = new ConnectionSettings(new Uri(settings.Url))
            .DefaultIndex(settings.IndexName)
            .PrettyJson()
            .DisableDirectStreaming();

        Client = new OpenSearchClient(connectionSettings);
    }
}