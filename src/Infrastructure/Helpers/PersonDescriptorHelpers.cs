using Core.Dtos.Search;
using Nest;
using static Infrastructure.Repositories.PersonFieldHelpers;

namespace Infrastructure.Repositories;

public static class PersonDescriptorHelpers
{
    public static void PersonQueryFilter<TPersonSearchModel>(this List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> persDesc, SearchDto settings) where TPersonSearchModel: class
    {
        persDesc.Add(q => q
                .MultiMatch(m => m
                    .Query(settings.Query)
                    .Fields(p => p
                        .Fields(new List<Field> {PersonNameField(3), CareerField(1)})
                    )
                )
            );
    }

    public static void PersonQueryWithNominationFilter<TPersonSearchModel>(this List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> persDesc, SearchDto settings) where TPersonSearchModel: class
    {
        persDesc.Add(q => q
                .MultiMatch(m => m
                    .Query(settings.Query)
                    .Fields(p => p
                        .Fields(new List<Field> {PersonNameField(3), PersonNominationsField(2), CareerField(1)})
                    )
                )
            );
    }

    public static void KindOfPersonFilter<TPersonSearchModel>(this List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> persDesc, SearchDto settings) where TPersonSearchModel: class
    {
        persDesc.Add(q => q
                .Term(m => m
                    .Value(settings.KindOfPerson)
                    .Field(KindOfPersonField())
                ));
    }

    public static async Task<IEnumerable<string>> RelatedPersons(this IElasticClient elasticClient, SearchDto settings)
    {
        var res = await elasticClient.SearchAsync<PersonSearchModel>(s => s
            .Index("persons")
                .Query(q => q
                    .Bool(b => b
                        .Must(MustPersonDesc<PersonSearchModel>(settings))
                    )
                )
            );
        return res.Hits.Count == 0 
        ? Enumerable.Empty<string>()
        : res.Hits.Select(h => h.Source.Films)

        .Aggregate((s,s2) => {
            var arrInList = s.ToList();
            arrInList.AddRange(s2);
            return arrInList.ToArray();
        }).Distinct();
    }
    
    static IEnumerable<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> MustPersonDesc<TPersonSearchModel>(SearchDto settings) where TPersonSearchModel : class
    {
        var qResult = new List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>>();
        if(settings.Query is not null)
            qResult.PersonQueryFilter(settings);
        if(settings.KindOfPerson is not null)
            qResult.KindOfPersonFilter(settings);
        
        return qResult;
    }
}