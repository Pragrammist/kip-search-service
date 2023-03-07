using Core.Dtos.Search;
using Nest;
using static Infrastructure.Repositories.CensorFieldHelpers;

namespace Infrastructure.Repositories;

public static class CensorDescriptorHelpers
{
    public static List<Func<QueryContainerDescriptor<TCensorType>, QueryContainer>> CensorNameQuery<TCensorType>(this List<Func<QueryContainerDescriptor<TCensorType>, QueryContainer>> censDesc, SearchDto settings) where TCensorType : class
    {
        if(settings.Query is not null)
            censDesc.Add(q => q
                    .Match(m => m
                        .Field(CensorNameField())
                            .Query(settings.Query)
                    )
                );
        return censDesc;
    }
}
