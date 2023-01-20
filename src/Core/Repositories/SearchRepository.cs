using Core.Dtos.Search;
namespace Core.Repositories;

public interface SearchRepository<SearchType>
{
    public Task <IEnumerable<SearchType>> Search(Search settings);

    public Task <IEnumerable<SearchType>> GetByIds(params string[] ids);
}