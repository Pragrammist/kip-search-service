using Core.Dtos;
using Core.Dtos.Search;
using Nest;

namespace Infrastructure.Repositories;

public class SearchCensorRepositoryImpl : SearchRepositoryBase<CensorDto>
{
    
    public SearchCensorRepositoryImpl(IElasticClient elasticClient) : base(elasticClient) 
    {
        
    }
    public override Task<IEnumerable<CensorDto>> Search(SearchDto settings)
    {
        throw new NotImplementedException();
    }
}
