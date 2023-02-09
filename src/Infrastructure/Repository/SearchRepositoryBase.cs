using Core.Dtos.Search;
using Nest;

namespace Infrastructure.Repositories;

public abstract class RepositoryBase
{
    protected readonly IElasticClient _elasticClient;
    protected readonly string index;
    public RepositoryBase(IElasticClient elasticClient, string index)
    {
        _elasticClient = elasticClient;
        this.index = index;
    }


    
}
