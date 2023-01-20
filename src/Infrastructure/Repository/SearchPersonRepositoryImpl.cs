using Core.Repositories;
using Core.Dtos;
using Core.Dtos.Search;
using Nest;

namespace Infrastructure.Repositories;

public class SearchPersonRepositoryImpl : SearchRepositoryBase<PersonDto>
{
    public SearchPersonRepositoryImpl(IElasticClient elasticClient) : base(elasticClient)
    {

    }
   

    public override Task<IEnumerable<PersonDto>> Search(Search settings)
    {
        throw new NotImplementedException();
    }
}
