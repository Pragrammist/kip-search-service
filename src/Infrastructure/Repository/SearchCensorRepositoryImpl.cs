using Core;
using Core.Repositories;
using static Infrastructure.Repositories.DescriptorHelpers;
using static Infrastructure.Repositories.CensorFieldHelpers;

using Core.Dtos.Search;
using Nest;

namespace Infrastructure.Repositories;

public class SearchCensorRepositoryImpl<TCensorType> : RepositoryBase, SearchRepository<TCensorType> where TCensorType : class, IDable
{
    
    public SearchCensorRepositoryImpl(IElasticClient elasticClient) : base(elasticClient, "censors") 
    {
        
    }
    public async Task<IEnumerable<TCensorType>> Search(SearchDto settings)
    {
        var shouldDesc = await ShouldDesc(settings);
        var res = await _elasticClient.SearchAsync<TCensorType>(s => s
            .Index(index)
            .Take((int)settings.Take)
            .Skip((int)(settings.Take * (settings.Page - 1)))
            .Query(q => q
                .Bool(b => b
                    .Should(shouldDesc)
                    .MinimumShouldMatch(1)
                )
            )
        );

        return res.SelectHitsWithId();
    }
    
    async Task <IEnumerable<Func<QueryContainerDescriptor<TCensorType>, QueryContainer>>> ShouldDesc(SearchDto settings) =>
        QueryContainerList<TCensorType>()
        .CensorNameQuery(settings)
        .ValuesFilter(CensorFilmsField(), await _elasticClient.SearchRelatedFilms(settings))
        .ValuesFilter(CensorFilmsField(), await _elasticClient.FilmFromPersons(settings));
    
    
    

}
