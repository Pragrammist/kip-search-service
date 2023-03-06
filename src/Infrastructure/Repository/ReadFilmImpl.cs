
using Core.Repositories;
using Nest;
using Core;
using static Infrastructure.Repositories.FilmFieldHelpers;
using Core.Dtos;

namespace Infrastructure.Repositories;

public class ReadFilmRepositoryImpl<TFilmType> : RepositoryBase, FilmRepository<TFilmType>, GetManyRepository<TFilmType> where  TFilmType : class, Idable
{
    
    public ReadFilmRepositoryImpl(IElasticClient elasticClient) : base(elasticClient,"films")
    {
        
    }
    public async Task<IEnumerable<string>> GetGenres()
    {
        const string AGG_NAME = "genres";
        var res = await _elasticClient.SearchAsync<FilmSearchModel>(s => s
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
    public async Task <IEnumerable<TFilmType>> GetSreeningFilms()
    {

        var trailers = await _elasticClient.SearchAsync<TFilmType>(s => s
            .Index(index)
            .Sort(s => s.Descending(ViewCountField()))
            .Query(q => q
                .Bool(b => b
                    .Must(m => m
                        .Terms(ts => ts.Field(ReleaseTypeField()).Terms<FilmReleaseType>(FilmReleaseType.SCREENING))
                    )
                )
            )
        );
        return trailers.Hits.Count < 0 
        ? Enumerable.Empty<TFilmType>()
        : trailers.Hits.Select(s => {
            var source = s.Source;
            source.Id = s.Id;
            return source;
        });
    }
}



