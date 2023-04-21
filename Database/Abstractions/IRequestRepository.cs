using Database.Models;

namespace Database.Abstractions;

public interface IRequestRepository
{
    Task CreateRequestAsync(RequestModel request, CancellationToken cancellationToken);
}