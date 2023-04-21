using Infrastructure;
using Solver;

namespace Database.Models;

public class RequestModel
{
    public Guid Id { get; set; }

    public DiscordUser User { get; init; }

    public byte[]? ImageData { get; set; }

    public Solution? Solution { get; set; }

    public LightsOutBoard? Board { get; set; }

    public RequestModel(DiscordUser user, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        User = user;
    }
}
