using Core.Dtos;
namespace Core.Repositories;

public interface CensorRepository
{
    public Task <IEnumerable<FilmDto>> GetCensors(params string[] ids);
}