using System.Linq.Expressions;
using Core.Dtos;
using Core.Dtos.Search;
using Nest;
using Core.Repositories;
using Core;

namespace Infrastructure.Repositories;



public class SearchFilmRepositoryImpl<TFilmType> : RepositoryBase, SearchRepository<TFilmType> where TFilmType : class, Idable
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
        if(res.Hits.Count < 1)
            return Enumerable.Empty<TFilmType>();
        var sources = res.Hits.Select(s => 
        {
            var res = s.Source;
            res.Id = s.Id;
            return res;
        });
        return sources ?? Enumerable.Empty<TFilmType>();
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
        selector.Script(s => s.Source($"(doc['{nameof(FilmDto.Score)}'].value * doc['{nameof(FilmDto.ScoreCount)}'].value + 1) / (doc['{nameof(FilmDto.ScoreCount)}'].value+1)")); //(a.Score * a.ScoreCount + 1) / (++a.ScoreCount)
        return selector;
    }


    void SetDefaultDateTimeRange(SearchDto settings)
    {
        if(settings.From is null)
            settings.From = new DateTime(1887, 1, 1, 0, 0, 0);
        if(settings.To is null) 
            settings.To = new DateTime(2023, 01, 28, 0, 0, 0); 
    }


    // IEnumerable<TFilmType> SortDescriptor(IEnumerable<TFilmType> toSort, SearchDto settings)
    // {
        //return toSort;
        // if(settings.Sort is null || toSort.Count() < 1)
        //     return toSort;

        // if(settings.Sort == Core.SortBy.POPULARIY)
        //     return toSort.OrderByDescending(f => f.ViewCount);
        
        // if(settings.Sort == Core.SortBy.RATING)
        //     return toSort.OrderByDescending(a => (a.Score * a.ScoreCount + 1) / (++a.ScoreCount));

        // else
        //     return toSort.OrderByDescending(f => GiveNotNullFieldOrDefault(f, DateTime.MinValue));
    //}
    // DateTime? GiveNotNullFieldOrDefault(FilmDto film, DateTime? defValue = null)
    // {
        
    //     if(film.EndScreening is not null)
    //         return film.EndScreening;
        
    //     if(film.StartScreening is not null)
    //         return film.StartScreening;

    //     if(film.Release is not null)
    //         return film.Release;
        
    //     return defValue ?? default;
    // }

    
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

    IEnumerable<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>> FilterDescriptor(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>>();

        if(settings.AgeLimit is not null)
            qResult.Add(f => f.Range(r => r.LessThanOrEquals(settings.AgeLimit).Field(AgeLimitField())));
        
        SetDefaultDateTimeRange(settings);

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

        
    
    
    async Task <IEnumerable<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>>> MustDescriptor(SearchDto settings) 
    {
        var qResult = new List<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>>();
        var shouldDescr = (await ShuldDescriptorInMustElement(settings));
        
        qResult.Add(q => q.Bool(b => b.Should(shouldDescr)));

        if(settings.KindOfFilm is not null)
            qResult.Add(q => q
                .Term(m => m
                    .Value(settings.KindOfFilm)
                    .Field(KindOfFilmField())
                )
            );

        
        if(settings.ReleaseType is not null)
            qResult.Add(q => q
                .Term(m => m
                    .Value(settings.ReleaseType)
                    .Field(ReleaseTypeField())
                )
            );

        if(settings.Genres is not null)
            qResult.Add(q => q
                .Terms(t => t
                    .Terms(settings.Genres)
                    .Field(GenresField())
                )
            );

        if(settings.Countries is not null)
            qResult.Add(q => SearcgByCountry(q, settings.Countries));

        
        return qResult;
    }
    QueryContainer SearcgByCountry(QueryContainerDescriptor<TFilmType> desc, string[] countries)
    {
        desc.Bool(b => b.Should(sh => {
            foreach(var country in countries)
                sh.Match(m => m.Query(country).Field(CountryField()));
                
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

    async Task<IEnumerable<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>>> ShuldDescriptorInMustElement(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>>();
        
        if(settings.Query is null)
            return qResult;

        
        qResult.Add(q => 
            q.MultiMatch(m => m
                .Query(settings.Query)
                .MinimumShouldMatch("2<50%")
                .Fields(fs => fs
                    .Field(NameField(3))
                    .Field(DescriptionField(2))
                )
            )
        );
        qResult.Add(q => q.Term(NominationsField(), settings.Query));

        var relatedFilmsFromActors = await PersonSelector(settings);

        if(relatedFilmsFromActors.Count() < 1)
            return qResult;

        qResult.Add(q => q.Ids(ids => ids.Values(relatedFilmsFromActors)));
        
        return qResult;
    }
    Field DescriptionField(double? boost = null)
    {
        Expression<Func<FilmDto, string>> fieldExpr = f => f.Description;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    Field NominationsField(double? boost = null)
    {
        Expression<Func<FilmDto, string[]>> fieldExpr = f => f.Nominations;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    Field CountryField(double? boost = null)
    {
        Expression<Func<FilmDto, string>> fieldExpr = f => f.Country;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    Field KindOfFilmField(double? boost = null)
    {
        Expression<Func<FilmDto, FilmType>> fieldExpr = f => f.KindOfFilm;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    Field ReleaseTypeField(double? boost = null)
    {
        Expression<Func<FilmDto, FilmReleaseType>> fieldExpr = f => f.ReleaseType;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    Field ReleaseField(double? boost = null)
    {
        Expression<Func<FilmDto, DateTime?>> fieldExpr = f => f.Release;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    Field StartSreeningField(double? boost = null)
    {
        Expression<Func<FilmDto, DateTime?>> fieldExpr = f => f.StartScreening;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    Field EndSreeningField(double? boost = null)
    {
        Expression<Func<FilmDto, DateTime?>> fieldExpr = f => f.StartScreening;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    Field AgeLimitField(double? boost = null)
    {
        Expression<Func<FilmDto, uint>> fieldExpr = f => f.AgeLimit;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    Field GenresField(double? boost = null)
    {
        Expression<Func<FilmDto, string[]>> fieldExpr = f => f.Genres;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    Field NameField(double? boost = null)
    {
        Expression<Func<FilmDto, string>> fieldExpr = f => f.Name;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    Field WatchedCountField(double? boost = null)
    {
        Expression<Func<FilmDto, uint>> fieldExpr = f => f.WatchedCount;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    Field ScoreField(double? boost = null)
    {
        Expression<Func<FilmDto, double>> fieldExpr = f => f.Score;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

}

