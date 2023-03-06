using Elasticsearch.Net;
using Infrastructure.Repositories;
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
                return new ElasticClient(settings);
            });
         
        return services;
    }

}

public class IndexFiller
{
    readonly IElasticClient _elasticClient;
    public IndexFiller(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public void FillIndexes(string filmIndex = "films")
    {
        
    }
    void FilmIndex(string filmIndex = "films")
    {
        _elasticClient.Map<FilmSearchModel>( m => m
            .Index(filmIndex)
            .Properties(p => p
                .IntegerRange(f => f.Name(n => n.AgeLimit))
            )
        );
    }
}

