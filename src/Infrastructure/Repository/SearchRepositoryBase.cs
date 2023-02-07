using Core.Repositories;
using Core.Dtos.Search;
using Nest;

namespace Infrastructure.Repositories;

public abstract class SearchRepositoryBase<TDocument> : SearchRepository<TDocument> where TDocument : class
{
    protected readonly IElasticClient _elasticClient;
    public SearchRepositoryBase(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async virtual Task<IEnumerable<TDocument>> GetByIds(params string[] ids)
    {
        var res = await _elasticClient.SearchAsync<TDocument>(s => s.Index("censors").Query(q => 
                q.Ids(id => 
                    id.Values(ids)
                )
            )
        );

        var selectedCensors = res.Hits.Select(s => s.Source) ?? Enumerable.Empty<TDocument>();
        return selectedCensors;
    }

    public abstract Task<IEnumerable<TDocument>> Search(SearchDto settings);
    
}
