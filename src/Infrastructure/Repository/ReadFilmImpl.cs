
using Core.Repositories;
using Nest;
using Core;
using static Infrastructure.Repositories.FilmFieldHelpers;
using Core.Dtos;

namespace Infrastructure.Repositories;

public class ReadFilmRepositoryImpl<TFilmType> : RepositoryBase, FilmRepository<TFilmType> where  TFilmType : class, IDable
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
   
}



