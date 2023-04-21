using Logger;
using Microsoft.Azure.Cosmos;

namespace Database.Internal;

internal class RequestRepository : RepositoryBase, IRequestRepository
{
    protected override string ContainerId => "Requests";
    private readonly ILogWriter<RequestRepository> _logger;

    public RequestRepository(CosmosDatabaseConfiguration configuration,
                             ILogWriter<RequestRepository> logger)
        : base(configuration)
    {
        _logger = logger;
    }

    public async Task CreateRequestAsync(RequestModel request, CancellationToken cancellationToken)
    {
        try
        {
            await CreateAsync(new RequestDocument(request), cancellationToken);
        }
        catch (CosmosException cEx)
        {
            _logger.Error(cEx, cEx.Message);
        }
        catch (TaskCanceledException tcEx)
        {
            _logger.Error(tcEx, "Write to database was canceled");
        }
    }
}
