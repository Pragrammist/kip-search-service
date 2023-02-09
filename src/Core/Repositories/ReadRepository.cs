namespace Core.Repositories;

public interface ByIdRepository<SearchType>
{
    public Task <IEnumerable<SearchType>> GetByIds(params string[] ids);
}

public interface GetManyRepository<SearchType>
{
    public Task <IEnumerable<SearchType>> GetMany();
}