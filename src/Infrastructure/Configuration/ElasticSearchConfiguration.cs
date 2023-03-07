using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Serilog;
namespace Infrastructure.Configuration;

public static class ElasticSearchConfiguration
{
    // public static IServiceCollection AddElastic(this IServiceCollection services, string elasticUser, string elasticPassword, string cloudId)
    // {
    //     services.AddSingleton<IElasticClient>(sp =>
    //         {
    //             try{
    //                 var credentials = new BasicAuthenticationCredentials(elasticUser, elasticPassword);
    //                 var settings = new ConnectionSettings(cloudId, credentials);
    //                 settings.DefaultFieldNameInferrer(p => p);
    //                 return new ElasticClient(settings);
    //             }
    //             catch(Exception ex){
    //                 Log.Logger.Error(ex.Message);
    //                 throw;
    //             }
    //         });
        
    //     return services;
    // }

    public static IServiceCollection AddElastic(this IServiceCollection services, string? elkUrl = null)
    {
        services.AddSingleton<IElasticClient>(sp =>
            {
                Uri? uri = null;
                
                if(elkUrl is not null)
                    uri = new Uri(elkUrl);

                var settings = new ConnectionSettings(uri);
                
                settings.ThrowExceptions();
                settings.EnableApiVersioningHeader();

                settings.DefaultFieldNameInferrer(p => p);
                var client = new ElasticClient(settings);
                var indexFiller = new IndexFiller(client);
                indexFiller.FillIndexes();
                return client;
            });
         
        return services;
    }

}

