using Database.Configuration;
using Logger;
using Microsoft.Azure.Cosmos;

namespace Database.Internal;

internal class LogRepository : RepositoryBase, ILogRepository
{
    protected override string ContainerId => "Logs";

    private readonly ILogWriter<LogRepository> _logger;

    public LogRepository(CosmosDatabaseConfiguration cosmosDatabaseConfiguration,
                         ILogWriter<LogRepository> logger)
        : base(cosmosDatabaseConfiguration)
    {
        _logger = logger;
    }

    public async Task CreateLogAsync(LogModel logModel, CancellationToken cancellationToken)
    {
        try
        {
            await CreateAsync(new LogDocument(logModel), cancellationToken);
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
