using Core.Dtos;
namespace Core.Repositories;

public interface FilmRepository
{
    public Task <IEnumerable<FilmDto>> GetFilms(params string[] ids);
}
