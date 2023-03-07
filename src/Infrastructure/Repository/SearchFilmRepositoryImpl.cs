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


        return selector;
    }
    
    IScriptSort RatingCalculator(ScriptSortDescriptor<TFilmType> selector)
    {
        selector.Type("number");
        selector.Script(s => s.Source($"(doc['{nameof(FilmSearchModel.Score)}'].value * doc['{nameof(FilmSearchModel.ScoreCount)}'].value + 1) / (doc['{nameof(FilmSearchModel.ScoreCount)}'].value+1)")); //(a.Score * a.ScoreCount + 1) / (++a.ScoreCount)
        return selector;
    }


    

    IEnumerable<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>> FilterDescriptor(SearchDto settings) => 
        QueryContainerList<TFilmType>()
        .AgeFilter(settings)
        .FilmDateFilter(settings);
    
     async Task <IEnumerable<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>>> MustDescriptor(SearchDto settings) => 
        QueryContainerList<TFilmType>()
        .AddShouldDesc(await ShuldDescriptorInMustElement(settings))
        .KindOfFilmFilter(settings)
        .ReleaseTypeFilter(settings)
        .GenresFilter(settings)
        .CountriesFilter(settings);
    


    async Task<IEnumerable<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>>> ShuldDescriptorInMustElement(SearchDto settings) =>
        settings.Query is null ? QueryContainerList<TFilmType>() : QueryContainerList<TFilmType>()
            .FilmQueryFilter(settings, "2<50%")
            .FilmNominationFilter(settings)
            .IdsFilter(await _elasticClient.FilmFromPersons(settings));

    

    

        
    
    
   
    
}

