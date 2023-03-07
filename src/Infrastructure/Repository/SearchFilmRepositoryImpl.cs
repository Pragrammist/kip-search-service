using Core.Dtos.Search;
using Nest;
using Core.Repositories;
using Core;
using static Infrastructure.Repositories.FilmFieldHelpers;
using static Infrastructure.Repositories.DescriptorHelpers;

namespace Infrastructure.Repositories;



public class SearchFilmRepositoryImpl<TFilmType> : RepositoryBase, SearchRepository<TFilmType> where TFilmType : class, IDable
{
    
    public SearchFilmRepositoryImpl(IElasticClient elasticClient) : base(elasticClient, "films") { }
    

    public async Task<IEnumerable<TFilmType>> Search(SearchDto settings)
    {
        SetDefaultDateTimeRange(settings);
        var mustDesc = await MustDescriptor(settings);
        var res = await _elasticClient.SearchAsync<TFilmType>(s => s
            .Index(index)
            .Sort(s => SortDescriptor(s, settings))
            .From((int)(settings.Take * (settings.Page - 1)))
            .Size((int)settings.Take)
            .Query(q => 
                q.Bool(b => b
                    .Filter(FilterDescriptor(settings)) 
                    .Must(queries:mustDesc)
                )
            )
        );

        return res.SelectHitsWithId();
    }

    private IPromise<IList<ISort>> SortDescriptor(SortDescriptor<TFilmType> selector, SearchDto settings)
    {
        if(settings.Sort is null)
            return selector;

        if(settings.Sort == SortBy.POPULARIY)
            selector.Descending(WatchedCountField());
        else if (settings.Sort == SortBy.RATING)
            selector.Script(RatingCalculator);
        else if(settings.Sort == SortBy.RATING)
            selector.Descending(ReleaseField())
                    .Descending(StartSreeningField())
                    .Descending(EndSreeningField());

        return selector;
    }
    
    IScriptSort RatingCalculator(ScriptSortDescriptor<TFilmType> selector)
    {
        selector.Type("number");
        selector.Script(s => s.Source($"(doc['{nameof(FilmSearchModel.Score)}'].value * doc['{nameof(FilmSearchModel.ScoreCount)}'].value + 1) / (doc['{nameof(FilmSearchModel.ScoreCount)}'].value+1)")); //(a.Score * a.ScoreCount + 1) / (++a.ScoreCount)
        return selector;
    }


    void SetDefaultDateTimeRange(SearchDto settings)
    {
        if(settings.From is null)
            settings.From = new DateTime(1887, 1, 1, 0, 0, 0);
        if(settings.To is null) 
            settings.To = new DateTime(2023, 01, 28, 0, 0, 0); 
    }

    IEnumerable<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>> FilterDescriptor(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>>();

        if(settings.AgeLimit is not null)
            qResult.AgeFilter(settings);

        qResult.Add(f => DateTimeFilter(f, settings));
        
        return qResult;
    }

    QueryContainer DateTimeFilter(QueryContainerDescriptor<TFilmType> f, SearchDto settings) => f.Bool(b2 => b2
        .Should(
            sh => sh
                .DateRange(d => d
                    .Field(ReleaseField())
                    .GreaterThan(settings.From)
                    .LessThan(settings.To)
                ),
            sh => sh
                .DateRange(d => d
                    .Field(EndSreeningField())
                    .GreaterThan(settings.From)
                    .LessThan(settings.To)
                    
                ),
            sh => sh
                .DateRange(d => d
                    .Field(StartSreeningField())
                    .GreaterThan(settings.From)                    
                    .LessThan(settings.To)
                )
            )
        .MinimumShouldMatch(1)
        );

        
    
    
    async Task <IEnumerable<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>>> MustDescriptor(SearchDto settings) => 
        QueryContainerList<TFilmType>()
        .AddShouldDesc(await ShuldDescriptorInMustElement(settings))
        .KindOfFilmFilter(settings)
        .ReleaseTypeFilter(settings)
        .GenresFilter(settings)
        .CountriesFilter(settings);
    


    async Task<IEnumerable<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>>> ShuldDescriptorInMustElement(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>>();
        
        if(settings.Query is null)
            return qResult;

        
        qResult.FilmQueryFilter(settings, "2<50%");

        qResult.Add(q => q.Term(FilmNominationsField(), settings.Query));

        var relatedFilmsFromActors = await _elasticClient.RelatedPersons(settings);

        if(relatedFilmsFromActors.Count() < 1)
            return qResult;

        qResult.Add(q => q.Ids(ids => ids.Values(relatedFilmsFromActors)));
        
        return qResult;
    }
    
}

