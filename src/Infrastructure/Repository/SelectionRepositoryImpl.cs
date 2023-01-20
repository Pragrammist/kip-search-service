
using Core;
using Core.Repositories;

namespace Infrastructure.Repositories;
public class SelectionRepositoryImpl : SelectionRepository
{
    public Task<FilmSelection> CreateSelection(List<string> films, string name)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(string id)
    {
        throw new NotImplementedException();
    }

    public Task<FilmSelection> Get(string id)
    {
        throw new NotImplementedException();
    }

    public Task<FilmSelection> Get(uint page = 1, uint limit = 20)
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