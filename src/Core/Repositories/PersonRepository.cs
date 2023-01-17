using Core.Dtos;
namespace Core.Repositories;

public interface PersonRepository
{
    public Task <IEnumerable<PersonDto>> GetPeople(params string[] ids);
}
