using Database.Abstractions;
using Database.Configuration;
using Database.Internal;
using Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Database;

public static class RepositoryServiceBuilder
{
    public static IServiceCollection RegisterRepositoryServices(this IServiceCollection services, IConfiguration configuration)
    {
        var dbCofigSection = configuration.GetRequiredSection("DatabaseConfiguration");
        var sectionKeys = dbCofigSection.GetChildren();
        var dbKey = sectionKeys.First(x => x.Key == "Key").Value;
        var dbEndpoint = sectionKeys.First(x => x.Key == "Endpoint").Value;

        ArgumentNullException.ThrowIfNull(dbEndpoint, nameof(dbEndpoint));
        ArgumentNullException.ThrowIfNull(dbKey, nameof(dbKey));

        var dbConfig = new CosmosDatabaseConfiguration(dbEndpoint, dbKey);

        services.AddSingleton(dbConfig);
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<ILogWriter<RequestRepository>, LogWriter<RequestRepository>>();
        return services;
    }
}