using Core.Dtos.Search;
using Nest;
using static Infrastructure.Repositories.SelectionFieldHelpers;

namespace Infrastructure.Repositories;

public static class SelectionDescriptorHelpers
{
    public static List<Func<QueryContainerDescriptor<TSelectionType>, QueryContainer>> SelectionNameQuery<TSelectionType>(this List<Func<QueryContainerDescriptor<TSelectionType>, QueryContainer>> selsDesc, SearchDto settings) 
    where TSelectionType : class
    {
        if(settings.Query is not null)
            selsDesc.Add(q => q
                    .Match(m => m
                        .Field(SelectionNameField())
                            .Query(settings.Query)
                    )
                );
        return selsDesc;
    }
}