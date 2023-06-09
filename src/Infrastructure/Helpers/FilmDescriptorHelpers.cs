using Core.Dtos.Search;
using Nest;
using static Infrastructure.Repositories.FilmFieldHelpers;
using static Infrastructure.Repositories.DescriptorHelpers;

namespace Infrastructure.Repositories;

public static class FilmDescriptorHelpers
{
    public static List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> GenresFilter<TFilmSearchModel>(this List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> filmDesc, SearchDto settings) where TFilmSearchModel: class
    {
        if(settings.Genres is not null)
            filmDesc.Add(q => q
                    .Terms(t => t
                        .Terms(settings.Genres)
                        .Field(GenresField())
                    )
                );
        return filmDesc;
    }

    public static List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> ReleaseTypeFilter<TFilmSearchModel>(this List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> filmDesc, SearchDto settings) where TFilmSearchModel: class
    {
        if(settings.ReleaseType is not null)
            filmDesc.Add(q => q
                    .Term(m => m
                        .Value(settings.ReleaseType)
                        .Field(ReleaseTypeField())
                    )
                );
        return filmDesc;
    }

    public static List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> CountriesFilter<TFilmSearchModel>(this List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> filmDesc, SearchDto settings) where TFilmSearchModel: class
    {
        if(settings.Countries is not null)
            filmDesc.Add(q => SearchByCountry(q, settings.Countries ?? new string[] {}));

        return filmDesc;
    }
    
    static QueryContainer SearchByCountry<TFilmSearchModel>(QueryContainerDescriptor<TFilmSearchModel> desc, string[] countries) where TFilmSearchModel : class
    {
        desc.Bool(b => b.Should(sh => {
            foreach(var country in countries)
                sh.Match(m => m.Query(country).Field(FilmCountryField()));
                
            return sh;
        }));
        

        return desc;
    }

    public static List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> KindOfFilmFilter<TFilmSearchModel>(this List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> filmDesc, SearchDto settings) where TFilmSearchModel: class
    {
        if(settings.KindOfFilm is not null)
            filmDesc.Add(q => q
                    .Term(m => m
                        .Value(settings.KindOfFilm)
                        .Field(KindOfFilmField())
                    )
                );
        return filmDesc;
    }

    public static List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> AgeFilter<TFilmSearchModel>(this List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> filmDesc, SearchDto settings) where TFilmSearchModel: class
    {
        if(settings.AgeLimit is not null)
            filmDesc.Add(f => f.Range(r => r.LessThanOrEquals(settings.AgeLimit).Field(AgeLimitField())));

        return filmDesc;
    }

    public static List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> FilmQueryByNameFilter<TFilmSearchModel>(this List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> filmDesc, SearchDto settings) where TFilmSearchModel: class
    {
        if(settings.Query is not null)
            filmDesc.Add(q => q
                    .Match(m => m
                        .Field(FilmNameField())
                            .Query(settings.Query)
                    )
            );
        return filmDesc;
    }

    public static List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> FilmQueryFilter<TFilmSearchModel>(this List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> filmDesc, SearchDto settings, string minShouldMatch = "0") where TFilmSearchModel: class
    {
        if(settings.Query is not null)
            filmDesc.Add(q => 
                q.MultiMatch(m => m
                    .Query(settings.Query)
                    .MinimumShouldMatch(minShouldMatch)
                    .Fields(fs => fs
                        .Field(FilmNameField(3))
                        .Field(FilmFactsField(2))
                        .Field(DescriptionField(1))
                    )
                )
            );
        return filmDesc;
    }          

    public static List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> FilmNominationFilter<TFilmSearchModel>(this List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> filmDesc, SearchDto settings, string minShouldMatch = "0") where TFilmSearchModel: class
    {
        if(settings.Query is not null)
            filmDesc.Add(q => q.Term(FilmNominationsField(), settings.Query));
        return filmDesc;
    }          
   
    public static List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> FilmDateFilter<TFilmSearchModel>(this List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> filmDesc, SearchDto settings) where TFilmSearchModel : class
    {
        SetDefaultDateTimeRange(settings);
        filmDesc.Add(f => DateTimeFilter(f, settings));
        return filmDesc;
    }
    
    static void SetDefaultDateTimeRange(SearchDto settings)
    {
        if(settings.From is null)
            settings.From = new DateTime(1887, 1, 1, 0, 0, 0);
        if(settings.To is null) 
            settings.To = DateTime.Now; 
    }
    static QueryContainer DateTimeFilter<TFilmSearchModel>(QueryContainerDescriptor<TFilmSearchModel> f, SearchDto settings) where TFilmSearchModel : class
        => f.Bool(b2 => b2
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

    static async Task<IEnumerable<string>> SearchRelatedFilms(
        this IElasticClient elasticClient,
        SearchDto settings, 
        Func<SearchDto, IEnumerable<Func<QueryContainerDescriptor<FilmSearchModel>, QueryContainer>>> mustFilmDesc
    ) 
    {
        var mustDescToPass = mustFilmDesc(settings);
        
        if(mustDescToPass.Count() == 0)
            return Enumerable.Empty<string>();

        var res = await elasticClient.SearchAsync<FilmSearchModel>(s => s
            .Index("films")
                .Query(q => q
                    .Bool(b => b
                        .Must(mustDescToPass)
                    )
                )
            );
        return res.Hits.Count == 0 
        ? Enumerable.Empty<string>()
        : res.Hits.Select(h => h.Id);
    }
    public static async Task<IEnumerable<string>> SearchRelatedFilms(
        this IElasticClient elasticClient,
        SearchDto settings
    ) => await SearchRelatedFilms(elasticClient, settings, MustFilmDesc<FilmSearchModel>);

    public static async Task<IEnumerable<string>> SearchRelatedFilmsForPerson(
        this IElasticClient elasticClient,
        SearchDto settings
    ) => await SearchRelatedFilms(elasticClient, settings, MustFilmDescriptorForPerson<FilmSearchModel>);
    
    static IEnumerable<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> MustFilmDesc<TFilmSearchModel>(SearchDto settings) 
    where TFilmSearchModel : class =>
        QueryContainerList<TFilmSearchModel>()
        .FilmQueryByNameFilter(settings)
        .CountriesFilter(settings)
        .AddDefaultFilmFilters(settings);
    
    static List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> AddDefaultFilmFilters<TFilmSearchModel>(this List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> filmDesc, SearchDto settings) 
    where TFilmSearchModel : class => filmDesc
        .GenresFilter(settings)
        .KindOfFilmFilter(settings)
        .ReleaseTypeFilter(settings)
        .AgeFilter(settings);

    
    static IEnumerable<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>> MustFilmDescriptorForPerson<TFilmType>(SearchDto settings) 
    where TFilmType : class => QueryContainerList<TFilmType>()
        .FilmQueryFilter(settings)
        .AddDefaultFilmFilters(settings);
    
}
