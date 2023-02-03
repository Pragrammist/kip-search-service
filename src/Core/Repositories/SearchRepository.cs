using Core.Dtos.Search;
namespace Core.Repositories;

public interface SearchRepository<SearchType>
{
    public Task <IEnumerable<SearchType>> Search(SearchDto settings);

    public Task <IEnumerable<SearchType>> GetByIds(params string[] ids);
}