using Core;


namespace Core.Repositories;

public interface SelectionRepository
{
    public Task<FilmSelection> CreateSelection(List<string> films, string name);

    public Task<bool> UpdateSelectionName(string id, string name);

    public Task<bool> UpdateSelectionFilmCollection(string id, List<string> films);

    public Task<bool> Delete(string id);

    public Task<FilmSelection> Get(string id);

    public Task<FilmSelection> Get(uint page = 1, uint limit = 20);
}
