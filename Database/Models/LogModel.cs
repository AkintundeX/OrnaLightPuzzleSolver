namespace Database.Models;

public class LogModel
{
    public string Message { get; set; }

    public Exception? Exception { get; set; }

    public byte[]? ImageData { get; set; }

    public string RequestId { get; set; }

    public LogModel(string message, string requestId)
    {
        Message = message;
        RequestId = requestId;
    }
}
