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
   

    public override Task<IEnumerable<PersonDto>> Search(SearchDto settings)
    {
        
        _elasticClient.SearchAsync<PersonDto>(s => s
            .Index("persons")
            .Query(q => q
                .Bool(b => b
                    .Must(m => m
                        .MultiMatch(match => match
                            .Query(settings.Query)
                            .Fields(fs => fs
                                .Field(f => f.Name)
                                .Field(f => f.Nominations)
                                .Field(f => f.Career)
                            )
                        )
                    )
                )    
            )
        );
        throw new NotImplementedException();
    }

    IEnumerable<Func<QueryContainerDescriptor<PersonDto>, QueryContainer>> MustDescriptor(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<PersonDto>, QueryContainer>>();
        
        //qResult.Add();
        return qResult;
    }

}
