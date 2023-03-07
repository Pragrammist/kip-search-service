using Core;
using Core.Dtos;
using Core.Dtos.Search;
using Nest;
using Core.Repositories;
using static Infrastructure.Repositories.PersonFieldHelpers;
using static Infrastructure.Repositories.FilmFieldHelpers;


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

        // if(settings.Sort == SortBy.POPULARIY)
        //     selector.Descending(WatchedCountField());
        // else if (settings.Sort == SortBy.RATING)
        //     selector.Script(RatingCalculator);

            return selector;
    }
    
    async Task<IEnumerable<Func<QueryContainerDescriptor<TPerson>, QueryContainer>>> MustDescriptor(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<TPerson>, QueryContainer>>();
        var shouldDescr = await ShouldDescriptorInMust(settings);

        qResult.Add(m => m
                    .Bool(b => b.Should(shouldDescr))
                );
        
        if(settings.KindOfPerson is not null)
            qResult.KindOfPersonFilter(settings);

        if(settings.From is not null)
            qResult.Add(s => s.DateRange(d => d.Field(BirdayField()).GreaterThanOrEquals(settings.From)));
        
        if(settings.To is not null)
            qResult.Add(s => s.DateRange(d => d.Field(BirdayField()).LessThanOrEquals(settings.To)));
        return qResult;
    }
    
    async Task <IEnumerable<Func<QueryContainerDescriptor<TPerson>, QueryContainer>>> ShouldDescriptorInMust(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<TPerson>, QueryContainer>>();

        if(settings.Query is not null)
            qResult.PersonQueryWithNominationFilter(settings);
        
        var filmIds = await _elasticClient.SearchRelatedFilmsForPerson(settings);


        if(filmIds.Count() > 0)
            qResult.Add(s => s.Terms(t => t
                .Terms(filmIds)
                .Field(PersonFilmsField())
            )
        );

        return qResult;
    }
    
    
}