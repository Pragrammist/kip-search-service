using Core.Repositories;
using Nest;
using Core;
namespace Infrastructure.Repositories;

public class ReadByIdRepoGeneric<TObj> : RepositoryBase, ByIdRepository<TObj> where TObj : class, IDable
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

        return res.SelectHitsWithId();
    }
}



