using Core;
using Core.Dtos;
using Core.Dtos.Search;
using Nest;
using Core.Repositories;
using static Infrastructure.Repositories.PersonFieldHelpers;
using static Infrastructure.Repositories.FilmFieldHelpers;
using static Infrastructure.Repositories.DescriptorHelpers;


namespace Infrastructure.Repositories;

public class SearchPersonRepositoryImpl<TPerson> : RepositoryBase, SearchRepository<TPerson> where TPerson : class, IDable
{
    
    public SearchPersonRepositoryImpl(IElasticClient elasticClient) : base(elasticClient, "persons")
    {
        
    }

    public async Task<IEnumerable<TPerson>> Search(SearchDto settings)
    {
        var mustDesc = await MustDescriptor(settings);
        var persons = await _elasticClient.SearchAsync<TPerson>(s => s
            .Index(index)
            .Sort(s => SortDescriptor(s, settings))
            .Take((int)settings.Take)
            .Skip((int)(settings.Take * (settings.Page - 1)))
            .Query(q => q
                .Bool(b => b
                    .Must(mustDesc)
                )    
            )
        );
        
        return persons.SelectHitsWithId();
    }
    
    private IPromise<IList<ISort>> SortDescriptor(SortDescriptor<TPerson> selector, SearchDto settings)
    {
        if(settings.Sort == SortBy.DATE)
            selector.Descending(BirdayField());

        return selector;
    }
    
    async Task<IEnumerable<Func<QueryContainerDescriptor<TPerson>, QueryContainer>>> MustDescriptor(SearchDto settings) => 
    QueryContainerList<TPerson>()
    .AddShouldDesc(await ShouldDescriptorInMust(settings))
    .KindOfPersonFilter(settings)
    .BirdayFromFilter(settings)
    .BirdayToFilter(settings);
    
    
    async Task <IEnumerable<Func<QueryContainerDescriptor<TPerson>, QueryContainer>>> ShouldDescriptorInMust(SearchDto settings) =>
        QueryContainerList<TPerson>()
        .PersonQueryWithNominationFilter(settings)
        .ValuesFilter(PersonFilmsField(), await _elasticClient.SearchRelatedFilmsForPerson(settings));
   
    
    
}