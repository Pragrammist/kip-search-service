using Core;
using Core.Dtos;
using Core.Dtos.Search;
using Nest;

namespace Infrastructure.Repositories;

public class SearchCensorRepositoryImpl : SearchRepositoryBase<CensorDto>
{
    
    public SearchCensorRepositoryImpl(IElasticClient elasticClient) : base(elasticClient) 
    {
        
    }
    public override async Task<IEnumerable<CensorDto>> Search(SearchDto settings)
    {
        var shouldDesc = await ShouldDesc(settings);
        var res = await _elasticClient.SearchAsync<CensorDto>(s => s
            .Index("censors")
            .Take((int)settings.Take)
            .Skip((int)(settings.Take * (settings.Page - 1)))
            .Query(q => q
                .Bool(b => b
                    .Should(shouldDesc)
                    .MinimumShouldMatch(1)
                )
            )
        );
        if(res.Hits.Count == 0)
            return Enumerable.Empty<CensorDto>();
        

        var toSort = res.Hits.Select(s => {
            var source = s.Source;
            source.Id = s.Id;
            return source;
        });

        return await SortCensors(settings, toSort);
    }
    async Task<IEnumerable<CensorDto>> SortCensors(SearchDto settings, IEnumerable<CensorDto> toSearch)
    {
        if(settings.Sort is null || settings.Sort is SortBy.DATE)
            return toSearch;

        
        if(settings.Sort == SortBy.POPULARIY){
            return await toSearch.ToAsyncEnumerable().OrderByDescendingAwait(async c => 
            {
                var films = await PersonFilms(c);
                var avgWatched = films.Average(a => a.WatchedCount);
                return avgWatched;
            }
            ).ToListAsync();
        }
        else
            return await toSearch.ToAsyncEnumerable().OrderByDescendingAwait(async c => 
            {
                var films = await PersonFilms(c);
                var avgScore = films.Average(a => (a.Score * a.ScoreCount + 1) / (++a.ScoreCount));
                return avgScore;
            }
            ).ToListAsync();
    }
    async Task<IEnumerable<FilmDto>> PersonFilms(CensorDto censor)
    {
        var films = await _elasticClient.SearchAsync<FilmDto>(s => s
                .Index("films")
                .Query(q => q
                    .Ids(s => s
                        .Values(censor.Films)
                )
            )
        );
        return films.Hits.Count == 0 
            ? Enumerable.Empty<FilmDto>() 
            : films.Hits.Select(s => s.Source);
    }
    async Task <IEnumerable<Func<QueryContainerDescriptor<CensorDto>, QueryContainer>>> ShouldDesc(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<CensorDto>, QueryContainer>>();
        if(settings.Query is not null)
            qResult.Add(q => q
                .Match(m => m
                    .Field(f => f.Name)
                        .Query(settings.Query)
                )
            );
        
        var films = await RelatedFilms(settings);

        if(films.Count() > 0)
            qResult.Add(q => q
                .Terms(t => t.Terms(films).Field(f => f.Films))
            );


        var filmsFromPersons = await RelatedPersonsFilms(settings);

        if(filmsFromPersons.Count() > 0)
            qResult.Add(q => q
                .Terms(t => t.Terms(filmsFromPersons).Field(f => f.Films))
            );
        return qResult;
    }
    async Task<IEnumerable<string>> RelatedFilms(SearchDto settings)
    {
        var res = await _elasticClient.SearchAsync<FilmDto>(s => s
            .Index("films")
                .Query(q => q
                    .Bool(b => b
                        .Must(MustFilmDesc(settings))
                    )
                )
            );
        return res.Hits.Count == 0 
        ? Enumerable.Empty<string>()
        : res.Hits.Select(h => h.Id);
    }
    async Task<IEnumerable<string>> RelatedPersonsFilms(SearchDto settings)
    {
        var res = await _elasticClient.SearchAsync<PersonDto>(s => s
            .Index("films")
                .Query(q => q
                    .Bool(b => b
                        .Must(MustPersonDesc(settings))
                    )
                )
            );
        return res.Hits.Count == 0 
        ? Enumerable.Empty<string>()
        : res.Hits.Select(h => h.Source.Films)

        .Aggregate((s,s2) => {
            var arrInList = s.ToList();
            arrInList.AddRange(s2);
            return arrInList.ToArray();
        });
    }
    IEnumerable<Func<QueryContainerDescriptor<PersonDto>, QueryContainer>> MustPersonDesc(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<PersonDto>, QueryContainer>>();
        if(settings.Query is not null)
            qResult.Add(q => q
                .Match(m => m
                    .Query(settings.Query)
                    .Field(f => f.Name)
                )
            );
        if(settings.KindOfPerson is not null)
            qResult.Add(q => q
                .Term(t => t.KindOfPerson, settings.KindOfPerson));
        return qResult;
    }
    IEnumerable<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>> MustFilmDesc(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>>();

        if(settings.Query is not null)
            qResult.Add(q => q
                .Match(m => m
                    .Field(f => f.Name)
                        .Query(settings.Query
                    )
                )
            );
        
        if(settings.Genres is not null)
            qResult.Add(q => q
                .Terms(t => t
                    .Field(f => f.Genres)
                        .Terms(settings.Genres)
                )
            );
        if(settings.Countries is not null)
            qResult.Add(q => q
                .Bool(b => b
                    .Should(ShouldCountriesDesc(settings))
                )
            );
        if(settings.KindOfFilm is not null)
            qResult.Add(q => q.Term(t => t.KindOfFilm, settings.KindOfFilm));

        if(settings.ReleaseType is not null)
            qResult.Add(q => q.Term(t => t.ReleaseType, settings.ReleaseType));

        if(settings.AgeLimit is not null)
            qResult.Add(q => q
                .Range(r => r
                    .Field(f => f.AgeLimit)
                    .LessThanOrEquals(settings.AgeLimit)
                )
            );
        return qResult;
    }
    IEnumerable<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>> ShouldCountriesDesc(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>>();
        if(settings.Countries is null)
            return qResult;
        foreach(var count in settings.Countries)
        {
            qResult.Add(q => q
                .Match(m => m
                    .Query(count)
                    .Field(f => f.Country)
                )
            );
        }
        return qResult;
    }
}
