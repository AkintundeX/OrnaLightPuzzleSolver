using Database.Models;

namespace Database.Abstractions;

internal interface ILogRepository
{
    Task CreateLogAsync(LogModel logModel, CancellationToken cancellationToken);
}