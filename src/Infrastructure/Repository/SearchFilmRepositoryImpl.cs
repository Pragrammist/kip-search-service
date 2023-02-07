using System.Linq.Expressions;
using Core.Dtos;
using Core.Dtos.Search;
using Nest;

namespace Infrastructure.Repositories;

public class SearchFilmRepositoryImpl : SearchRepositoryBase<FilmDto>
{
    
    public SearchFilmRepositoryImpl(IElasticClient elasticClient) : base(elasticClient) { }
    

    public async override Task<IEnumerable<FilmDto>> Search(SearchDto settings)
    {
        var mustDesc = await MustDescriptor(settings);
        var res = await _elasticClient.SearchAsync<FilmDto>(s => 
            s.Index("films")
            .From((int)(settings.Take * (settings.Page - 1)))
            .Size((int)settings.Take)
            .Query(q => 
                q.Bool(b => b
                    .Filter(FilterDescriptor(settings)) 
                    .Must(queries:mustDesc)
                )
            )
        );
        if(res.Hits.Count < 1)
            return Enumerable.Empty<FilmDto>();
        var sources = res.Hits.Select(s => 
        {
            var res = s.Source;
            res.Id = s.Id;
            return res;
        });
        return SortDescriptor(sources, settings) ?? Enumerable.Empty<FilmDto>();
    }

    void SetDefaultDateTimeRange(SearchDto settings)
    {
        if(settings.From is null)
            settings.From = new DateTime(1887, 1, 1, 0, 0, 0);
        if(settings.To is null) 
            settings.To = new DateTime(2023, 01, 28, 0, 0, 0); 
    }


    IEnumerable<FilmDto> SortDescriptor(IEnumerable<FilmDto> toSort, SearchDto settings)
    {
        if(settings.Sort is null || toSort.Count() < 1)
            return toSort;

        if(settings.Sort == Core.SortBy.POPULARIY)
            return toSort.OrderByDescending(f => f.ViewCount);
        
        if(settings.Sort == Core.SortBy.RATING)
            return toSort.OrderByDescending(a => (a.Score * a.ScoreCount + 1) / (++a.ScoreCount));

        else
            return toSort.OrderByDescending(f => GiveNotNullFieldOrDefault(f, DateTime.MinValue));
    }
    DateTime? GiveNotNullFieldOrDefault(FilmDto film, DateTime? defValue = null)
    {
        
        if(film.EndScreening is not null)
            return film.EndScreening;
        
        if(film.StartScreening is not null)
            return film.StartScreening;

        if(film.Release is not null)
            return film.Release;
        
        return defValue ?? default;
    }

    
    async Task<IEnumerable<string>> PersonSelector(SearchDto settings)
    {    
        if(settings.Query is null)
            return Enumerable.Empty<string>();

        var maybeRalatedFilmsFromActors = await _elasticClient
            .SearchAsync<PersonDto>(s => s
                .Index("persons")
                .Query(q => q
                    .Bool(b => b
                        .Must(MustPersonDecriptor(settings))
                    )
                )
            );

        
        if(maybeRalatedFilmsFromActors.Hits.Count < 1)
            return Enumerable.Empty<string>();

        var res = maybeRalatedFilmsFromActors.Hits
        .Take((int)settings.Take)
        .Select(h => h.Source.Films)
        .Aggregate((s,s2) => {
            var arrInList = s.ToList();
            arrInList.AddRange(s2);
            return arrInList.ToArray();
        })
        .Distinct();
        return res;
    }

    IEnumerable<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>> FilterDescriptor(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>>();

        if(settings.AgeLimit is not null)
            qResult.Add(f => f.Range(r => r.LessThanOrEquals(settings.AgeLimit).Field(f => f.AgeLimit)));
        
        SetDefaultDateTimeRange(settings);

        qResult.Add(f => DateTimeFilter(f, settings));
        
        return qResult;
    }

    QueryContainer DateTimeFilter(QueryContainerDescriptor<FilmDto> f, SearchDto settings) => f.Bool(b2 => b2
        .Should(
            sh => sh
                .DateRange(d => d
                    .Field(f => f.Release)
                    .GreaterThan(settings.From)
                    .LessThan(settings.To)
                ),
            sh => sh
                .DateRange(d => d
                    .Field(f => f.EndScreening)
                    .GreaterThan(settings.From)
                    .LessThan(settings.To)
                    
                ),
            sh => sh
                .DateRange(d => d
                    .Field(f => f.StartScreening)
                    .GreaterThan(settings.From)                    
                    .LessThan(settings.To)
                )
            )
        .MinimumShouldMatch(1)
        );

        
    
    
    async Task <IEnumerable<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>>> MustDescriptor(SearchDto settings) 
    {
        var qResult = new List<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>>();
        var shouldDescr = (await ShuldDescriptorInMustElement(settings));
        
        qResult.Add(q => q.Bool(b => b.Should(shouldDescr)));

        if(settings.KindOfFilm is not null)
            qResult.Add(q => q
                .Term(m => m
                    .Value(settings.KindOfFilm)
                    .Field(f => f.KindOfFilm)
                )
            );

        
        if(settings.ReleaseType is not null)
            qResult.Add(q => q
                .Term(m => m
                    .Value(settings.ReleaseType)
                    .Field(f => f.ReleaseType)
                )
            );

        if(settings.Genres is not null)
            qResult.Add(q => q
                .Terms(t => t
                    .Terms(settings.Genres)
                    .Field(f => f.Genres)
                )
            );

        if(settings.Countries is not null)
            qResult.Add(q => SearcgByCountry(q, settings.Countries));

        
        return qResult;
    }
    QueryContainer SearcgByCountry(QueryContainerDescriptor<FilmDto> desc, string[] countries)
    {
        desc.Bool(b => b.Should(sh => {
            foreach(var country in countries)
                sh.Match(m => m.Query(country).Field(f => f.Country));
                
            return sh;
        }));
        

        return desc;
    }


    IEnumerable<Func<QueryContainerDescriptor<PersonDto>, QueryContainer>> MustPersonDecriptor(SearchDto settings)
    {
        var qRes = new List<Func<QueryContainerDescriptor<PersonDto>, QueryContainer>>();

        if(settings.Query is not null)
            qRes.Add(q => q
                .MultiMatch(m => m
                    .Query(settings.Query)
                    .Fields(p => p
                        .Fields(f => f.Name, f => f.Career)
                    )
                )
            );
        
        if(settings.KindOfPerson is not null)
            qRes.Add(q => q
                .Term(m => m
                    .Value(settings.KindOfPerson)
                    .Field(f => f.KindOfPerson)
                )
            );
        return qRes;
    }

    async Task<IEnumerable<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>>> ShuldDescriptorInMustElement(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>>();
        
        if(settings.Query is null)
            return qResult;

        
        qResult.Add(q => 
            q.MultiMatch(m => m
                .Query(settings.Query)
                .MinimumShouldMatch("2<50%")
                .Fields(fs => fs
                    .Field(f => f.Name, 3)
                    .Field(f => f.Description, 2)
                )
            )
        );
        qResult.Add(q => q.Term(f => f.Nominations, settings.Query));

        var relatedFilmsFromActors = await PersonSelector(settings);

        if(relatedFilmsFromActors.Count() < 1)
            return qResult;

        qResult.Add(q => q.Ids(ids => ids.Values(relatedFilmsFromActors)));
        
        return qResult;
    }
}
