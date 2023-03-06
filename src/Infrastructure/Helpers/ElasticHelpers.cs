using System.Security.Cryptography.X509Certificates;
using Nest;

namespace Core.Repositories;

public static class ElasticHelpers
{
    // the type must contain Id field
    static public async Task<IEnumerable<TDocument>> GetByIds<TDocument>(this IElasticClient elasticClient, params string[] ids) where TDocument : class 
    {
        var res = await elasticClient.SearchAsync<TDocument>(s => s.Index("censors").Query(q => 
                q.Terms(t => 
                    t.Terms(ids).Field("Id")
                )
            )
        );

        var selectedCensors = res.Hits.Select(s => s.Source) ?? Enumerable.Empty<TDocument>();
        return selectedCensors;
    }
}
