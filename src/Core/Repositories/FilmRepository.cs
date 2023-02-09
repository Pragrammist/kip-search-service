namespace Core.Repositories;

public interface FilmRepository<SearchType>
{
    public Task <IEnumerable<string>> GetGenres();
}