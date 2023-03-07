using Nest;
using Core;
using Core.Repositories;
using Core.Dtos;
using Core.Dtos.Search;
using Elasticsearch;
using static Infrastructure.Repositories.DescriptorHelpers;
using static Infrastructure.Repositories.SelectionFieldHelpers;
using static Infrastructure.Repositories.PersonFieldHelpers;
using static Infrastructure.Repositories.FilmFieldHelpers;

namespace Infrastructure.Repositories;
public class SelectionRepositoryImpl<TSelectionType> : RepositoryBase, SearchRepository<TSelectionType> where TSelectionType : class, IDable
{
    
    public SelectionRepositoryImpl(IElasticClient elasticClient) : base(elasticClient, "selections")
    {

    }
    public async Task<IEnumerable<TSelectionType>> Search(SearchDto settings)
    {
        var shouldDesc = await ShouldDesc(settings);
        var res = await _elasticClient.SearchAsync<TSelectionType>(s => s
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
    
    async Task <IEnumerable<Func<QueryContainerDescriptor<TSelectionType>, QueryContainer>>> ShouldDesc(SearchDto settings) => 
        QueryContainerList<TSelectionType>()
        .SelectionNameQuery(settings)
        .ValuesFilter(SelectionFilmsField(), await _elasticClient.SearchRelatedFilms(settings))
        .ValuesFilter(SelectionFilmsField(), await _elasticClient.FilmFromPersons(settings));

   
    

   
    
}