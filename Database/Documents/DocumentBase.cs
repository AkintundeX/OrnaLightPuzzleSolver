using System.Text.Json.Serialization;

namespace Database.Documents;

public abstract class DocumentBase
{
    public abstract DocumentType Type { get; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    public DateTime CreatedDateTime { get; set; }

    [JsonIgnore]
    public abstract string PartitionKey { get; }

    public string? Etag { get; set; }
}
