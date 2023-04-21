using Database.Models;
using Infrastructure;
using Solver;

namespace Database.Documents;

public class RequestDocument : DocumentBase
{
    public DiscordUser User { get; init; }

    public byte[]? ImageData { get; init; }

    public Solution? Solution { get; init; }

    public LightsOutBoard? Board { get; init; }

    public override DocumentType Type => DocumentType.Request;

    public override string PartitionKey => Id;

    public RequestDocument(RequestModel model)
    {
        User = model.User;
        ImageData = model.ImageData;
        Solution = model.Solution;
        Board = model.Board;

    }
}
