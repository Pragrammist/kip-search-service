using Core.Repositories;
using Nest;
using Core;
namespace Infrastructure.Repositories;

public class ReadByIdRepoGeneric<TObj> : RepositoryBase, ByIdRepository<TObj> where TObj : class, Idable
{
    public ReadByIdRepoGeneric(IElasticClient elasticClient, string index) : base(elasticClient, index)
    {

    }

    public async Task<IEnumerable<TObj>> GetByIds(params string[] ids)
    {
        var res = await _elasticClient.SearchAsync<TObj>(s => s.Index(index)
            .Query(q => 
                    q.Ids(id => 
                        id.Values(ids)
                    )
            )
        );
        var selected = res.Hits.Select(s => {
            var source = s.Source;
            source.Id = s.Id;
            return source;
        }) ?? Enumerable.Empty<TObj>();
        return selected;
    }
}



