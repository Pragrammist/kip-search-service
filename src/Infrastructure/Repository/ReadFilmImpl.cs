using System.Linq.Expressions;
using Core.Repositories;
using Core.Dtos;
using Nest;

namespace Infrastructure.Repositories;

public class ReadFilmRepositoryImpl : RepositoryBase, ByIdRepository<ShortFilmDto>, FilmRepository<ShortFilmDto>, GetManyRepository<FilmTrailer>
{
    
    public ReadFilmRepositoryImpl(IElasticClient elasticClient) : base(elasticClient,"films")
    {
        
    }
    public async Task<IEnumerable<ShortFilmDto>> GetByIds(params string[] ids)
    {
        var res = await _elasticClient.SearchAsync<ShortFilmDto>(s => s.Index(index)
            .Query(q => 
                    q.Ids(id => 
                        id.Values(ids)
                    )
            )
        );

        var selected = res.Hits.Select(s => {
            var source = s.Source;
            source.Id = s.Id;
            return source;
        }) ?? Enumerable.Empty<ShortFilmDto>();
        return selected;
    }
    public async Task<IEnumerable<string>> GetGenres()
    {
        const string AGG_NAME = "genres";
        var res = await _elasticClient.SearchAsync<FilmDto>(s => s
            .Index(index)
            .Aggregations(a => a
                .Terms(
                    AGG_NAME, 
                    sg => sg
                        .Field(f => f.Genres)
                )
            )

        );
        var genres = res.Aggregations.Terms(AGG_NAME).Buckets.Select(b => b.Key) ?? Enumerable.Empty<string>();
        return genres;
    }
    public async Task <IEnumerable<FilmTrailer>> GetMany()
    {
        Expression<Func<FilmDto, FilmReleaseType>> fieldExpr = f => f.ReleaseType;
        Expression<Func<FilmDto, uint>> fieldPremieraExpr = f => f.ViewCount;
        Field releaseTypeField = fieldExpr;
        Field premieraViewsField = fieldPremieraExpr;


        var trailers = await _elasticClient.SearchAsync<FilmTrailer>(s => s
            .Index(index)
            .Sort(s => s.Descending(premieraViewsField))
            .Query(q => q
                .Bool(b => b
                    .Must(m => m
                        .Terms(ts => ts.Field(releaseTypeField).Terms(FilmReleaseType.SCREENING))
                    )
                )
            )
        );
        return trailers.Hits.Count < 0 
        ? Enumerable.Empty<FilmTrailer>()
        : trailers.Hits.Select(s => {
            var source = s.Source;
            source.Id = s.Id;
            return source;
        });
    }
}


