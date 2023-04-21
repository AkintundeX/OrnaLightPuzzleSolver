namespace Database.Configuration;

public class CosmosDatabaseConfiguration
{
    public string Endpoint { get; init; }

    public string Key { get; init; }

    public CosmosDatabaseConfiguration(string endpoint, string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(endpoint, nameof(endpoint));
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        Endpoint = endpoint;
        Key = key;
    }
}
