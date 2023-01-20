using Core.Dtos;
using Core.Dtos.Search;
using Nest;

namespace Infrastructure.Repositories;

public class SearchFilmRepositoryImpl : SearchRepositoryBase<FilmDto>
{
    
    public SearchFilmRepositoryImpl(IElasticClient elasticClient) : base(elasticClient) { }
    

    public async override Task<IEnumerable<FilmDto>> Search(Search settings)
    {
        var shouldExpr = await ShuldDescriptor(settings);

        var res = await _elasticClient.SearchAsync<FilmDto>(s => 
            s.Index("films")
            .Sort(s => SortDescriptor(s, settings))
            .From((int)(settings.Take * settings.Page))
            .Size((int)settings.Take)
            .Query(q => 
                q.Bool(b => b
                    .Filter(FilterDescriptor(settings))
                    .Must(MustDescriptor(settings))
                    .Should(shouldExpr)
                )
            )
        );
        
        return res.Hits.Select(s => s.Source) ?? Enumerable.Empty<FilmDto>();
    }

    IPromise<IList<ISort>> SortDescriptor(SortDescriptor<FilmDto> selector, Search settings)
    {
        if(settings.Sort is Core.SortBy.POPULARIY)
            selector.Descending(p => p.ViewCount);
        if(settings.Sort is Core.SortBy.RATING)
            selector.Descending(p => p.Score);
        else
            selector.Descending(f => DescByDateFilm(f));
        return selector;
    }
    object? DescByDateFilm(FilmDto f)
    {
        if (f.Release is not null)
            return f.Release;
        if (f.EndScreening is not null)
            return f.EndScreening;
        else
            return f.StartScreening;
    }
    async Task<IEnumerable<string>> PersonSelector(Search settings)
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
        
        var res = maybeRalatedFilmsFromActors.Hits
        .Select(h => h.Source.Films)
        .Aggregate((s,s2) => {
            s.AddRange(s2);
            return s;
        })
        .Take((int)settings.Take);
        return res;
    }
    IEnumerable<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>> FilterDescriptor(Search settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>>();

        if(settings.AgeLimit is not null)
            qResult.Add(f => f.Range(r => r.LessThanOrEquals(settings.AgeLimit).Field(f => f.AgeLimit)));
        
        if(settings.From is not null) //?????
            qResult.Add(f => f.DateRange(r => r.GreaterThanOrEquals(settings.From).Field(f => f.Release)) | 
                             f.DateRange(r => r.GreaterThanOrEquals(settings.From).Field(f => f.StartScreening)) |
                             f.DateRange(r => r.GreaterThanOrEquals(settings.From).Field(f => f.EndScreening)));

        if(settings.To is not null) //?????
            qResult.Add(f => f.DateRange(r => r.LessThanOrEquals(settings.To).Field(f => f.Release)) | 
                             f.DateRange(r => r.LessThanOrEquals(settings.To).Field(f => f.StartScreening)) |
                             f.DateRange(r => r.LessThanOrEquals(settings.To).Field(f => f.EndScreening)));
        
        return qResult;
    }   
    IEnumerable<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>> MustDescriptor(Search settings) 
    {
        var qResult = new List<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>>();
        
        if(settings.KindOfFilm is not null)
            qResult.Add(q => q
                .Match(m => m
                    .Query(settings.KindOfFilm.Value.ToString().ToLower())
                    .Field(f => f.KindOfFilm)
                )
            );
        if(settings.ReleaseType is not null)
            qResult.Add(q => q
                .Match(m => m
                    .Query(settings.ReleaseType.Value.ToString().ToLower())
                    .Field(f => f.ReleaseType)
                )
            );

        if(settings.Query is not null)
            qResult.Add(q => 
                q.MultiMatch(m => 
                    m.Query(settings.Query)
                        .MinimumShouldMatch("2<50%")
                        .Fields(FilmQuerySearchFields())
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
            qResult.Add(q => q.Terms(t => t.Terms(settings.Countries).Field(f => f.Country)));

        
        return qResult;
    }
    
    IEnumerable<Func<QueryContainerDescriptor<PersonDto>, QueryContainer>> MustPersonDecriptor (Search settings)
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
                .Match(m => m
                    .Query(settings.KindOfPerson.Value.ToString().ToLower())
                    .Field(f => f.KindOfPerson)
                )
            );
        return qRes;
    }
    async Task<IEnumerable<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>>> ShuldDescriptor(Search settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>>();

        if(settings.Query is null)
            return qResult;

        qResult.Add(q => q.Term(f => f.Nominations, settings.Query));

        var relatedFilmsFromActors = await PersonSelector(settings);

        qResult.Add(q => q.Terms(t => t.Terms(relatedFilmsFromActors).Field(f => f.Id)));
        
        return qResult;
    }
    Fields FilmQuerySearchFields()
    {
        string[] resExpr = new string[] 
        {
            $"{nameof(FilmDto.Name).ToLower()}^5",
            $"{nameof(FilmDto.Description).ToLower()}^4", 
            
            //$"{nameof(FilmDto.Articles)}", !! искать по тексту статью по нужной сткроке(query)
        };
        return resExpr;
    }
}
