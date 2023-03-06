using Core.Dtos.Search;
using Nest;
using Core.Repositories;
using Core;
using static Infrastructure.Repositories.FilmFieldHelpers;
using static Infrastructure.Repositories.PersonFieldHelpers;

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
    
    async Task<IEnumerable<string>> PersonSelector(SearchDto settings)
    {    
        if(settings.Query is null)
            return Enumerable.Empty<string>();

        var maybeRalatedFilmsFromActors = await _elasticClient
            .SearchAsync<PersonSearchModel>(s => s
                .Index("persons")
                .Query(q => q
                    .Bool(b => b
                        .Must(MustPersonDecriptor<PersonSearchModel>(settings))
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
                sh.Match(m => m.Query(country).Field(FilmCountryField()));
                
            return sh;
        }));
        

        return desc;
    }


    IEnumerable<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> MustPersonDecriptor<TPersonSearchModel>(SearchDto settings) where TPersonSearchModel : class
    {
        var qRes = new List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>>();

        if(settings.Query is not null)
            qRes.Add(q => q
                .MultiMatch(m => m
                    .Query(settings.Query)
                    .Fields(p => p
                        .Fields(new List<Field> {PersonNameField(), CareerField()})
                    )
                )
            );
        
        if(settings.KindOfPerson is not null)
            qRes.Add(q => q
                .Term(m => m
                    .Value(settings.KindOfPerson)
                    .Field(KindOfPersonField())
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
                    .Field(FilmNameField(3))
                    .Field(DescriptionField(2))
                )
            )
        );
        qResult.Add(q => q.Term(FilmNominationsField(), settings.Query));

        var relatedFilmsFromActors = await PersonSelector(settings);

        if(relatedFilmsFromActors.Count() < 1)
            return qResult;

        qResult.Add(q => q.Ids(ids => ids.Values(relatedFilmsFromActors)));
        
        return qResult;
    }
    
}

