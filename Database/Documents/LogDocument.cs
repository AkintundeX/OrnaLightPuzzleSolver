namespace Database.Documents;

public class LogDocument : DocumentBase
{
    public override DocumentType Type => DocumentType.Log;

    public string Message { get; init; }

    public Exception? Exception { get; init; }

    public byte[]? ImageData { get; init; }

    public string RequestId { get; init; }

    public override string PartitionKey => Id;

    public LogDocument(LogModel model)
    {
        Message = model.Message;
        Exception = model.Exception;
        ImageData = model.ImageData;
        RequestId = model.RequestId;
    }
}
