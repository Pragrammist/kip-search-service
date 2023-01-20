using Core.Repositories;
using Core.Dtos;
using Core.Dtos.Search;
using Nest;

namespace Infrastructure.Repositories;

public class SearchFilmSelectionRepositoryImpl : SearchRepositoryBase<FilmSelectionDto>
{
    public SearchFilmSelectionRepositoryImpl(IElasticClient elasticClient) : base(elasticClient) { }

    public override Task<IEnumerable<FilmSelectionDto>> Search(Search settings)
    {
        throw new NotImplementedException();
    }
}