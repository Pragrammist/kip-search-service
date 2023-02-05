using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Infrastructure.Configuration;

public static class ElasticSearchConfiguration
{
    public static IServiceCollection AddElastic(this IServiceCollection services, string elasticUser, string elasticPassword, string cloudId)
    {
        services.AddSingleton<IElasticClient>(sp =>
            {
                var credentials = new BasicAuthenticationCredentials(elasticUser, elasticPassword);
                var settings = new ConnectionSettings(cloudId, credentials);
                settings.DefaultFieldNameInferrer(p => p);
                return new ElasticClient(settings);
            }); 
        return services;
    }

    public static IServiceCollection AddElastic(this IServiceCollection services)
    {
        services.AddSingleton<IElasticClient>(sp =>
            {
                var settings = new ConnectionSettings();
                settings.DefaultFieldNameInferrer(p => p);
                return new ElasticClient(settings);
            }); 
        return services;
    }



    public static IElasticClient CreateElasticClient(string filmsIndexName = "films", string personsIndexName = "persons", string censorsIndexName = "censors", string selectionsIndexName ="filmselections")
    {
        var settings = new ConnectionSettings();
        settings.DefaultFieldNameInferrer(p => p);
        
        var client = new ElasticClient(settings);
        return client;
    }
}

