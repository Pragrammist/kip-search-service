using Nest;
using Core;
using Core.Repositories;
using Core.Dtos;
using Core.Dtos.Search;
using Mapster;
using Elasticsearch;

namespace Infrastructure.Repositories;
public class SelectionRepositoryImpl : SelectionRepository
{
    readonly IElasticClient _elasticClient;
    public SelectionRepositoryImpl(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }
    public async Task<FilmSelectionDto> CreateSelection(string[] films, string name)
    {
        FilmSelection _doc = new FilmSelection(films, name);

        var res = await _elasticClient.IndexAsync(_doc, d => d.Index("selections"));
        _doc.Id = res.Id;

        return _doc.Adapt<FilmSelectionDto>();
    }

    public Task<bool> Delete(string id)
    {
        throw new NotImplementedException();
    }

    public Task<FilmSelectionDto> Get(string id)
    {
        throw new NotImplementedException();
    }

    public Task<FilmSelectionDto> Get(uint page = 1, uint limit = 20)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<FilmSelectionDto>> GetByIds(params string[] ids)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<FilmSelectionDto>> Search(SearchDto settings)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateSelectionFilmCollection(string id, List<string> films)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateSelectionName(string id, string name)
    {
        throw new NotImplementedException();
    }
}