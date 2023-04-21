using Database.Configuration;
using Database.Documents;
using Microsoft.Azure.Cosmos;

namespace Database.Internal;

internal abstract class RepositoryBase
{
    private const string DATABASE_ID = "OrnaLights";
    protected Container Container { get; init; }
    protected abstract string ContainerId { get; }

    protected RepositoryBase(CosmosDatabaseConfiguration configuration)
    {
        var client = new CosmosClient(configuration.Endpoint, configuration.Key);
        Container = client.GetContainer(DATABASE_ID, ContainerId);
    }

    protected async Task CreateAsync<T>(T document, CancellationToken cancellationToken)
        where T : DocumentBase
    {
        _ = await Container.CreateItemAsync(document, new PartitionKey(document.PartitionKey), new ItemRequestOptions { EnableContentResponseOnWrite = false }, cancellationToken);
    }
}
