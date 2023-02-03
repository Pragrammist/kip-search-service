using Core;
using Core.Dtos;

namespace Core.Repositories;

public interface SelectionRepository : SearchRepository<FilmSelectionDto>
{
    public Task<FilmSelectionDto> CreateSelection(string[] films, string name);

    public Task<bool> UpdateSelectionName(string id, string name);

    public Task<bool> UpdateSelectionFilmCollection(string id, List<string> films);

    public Task<bool> Delete(string id);

    public Task<FilmSelectionDto> Get(string id);

    public Task<FilmSelectionDto> Get(uint page = 1, uint limit = 20);
}
