using Core.Dtos.Search;
using Nest;
using static Infrastructure.Repositories.PersonFieldHelpers;
using static Infrastructure.Repositories.DescriptorHelpers;

namespace Infrastructure.Repositories;

public static class PersonDescriptorHelpers
{
    public static List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> PersonQueryFilter<TPersonSearchModel>(this List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> persDesc, SearchDto settings) where TPersonSearchModel: class
    {
        if(settings.Query is not null)
            persDesc.Add(q => q
                    .MultiMatch(m => m
                        .Query(settings.Query)
                        .Fields(p => p
                            .Fields(new List<Field> {PersonNameField(3), CareerField(1)})
                        )
                    )
                );
        return persDesc;
    }

    public static List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> PersonQueryWithNominationFilter<TPersonSearchModel>(this List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> persDesc, SearchDto settings) where TPersonSearchModel: class
    {
        if(settings.Query is not null)
            persDesc.Add(q => q
                    .MultiMatch(m => m
                        .Query(settings.Query)
                        .Fields(p => p
                            .Fields(new List<Field> {PersonNameField(3), PersonNominationsField(2), CareerField(1)})
                        )
                    )
                );
        return persDesc;
    }

    public static List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> KindOfPersonFilter<TPersonSearchModel>(this List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> persDesc, SearchDto settings) where TPersonSearchModel: class
    {
        if(settings.KindOfPerson is not null)
            persDesc.Add(q => q
                    .Term(m => m
                        .Value(settings.KindOfPerson)
                        .Field(KindOfPersonField())
                    ));
        return persDesc;
    }

    public static List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> BirdayFromFilter<TPersonSearchModel>(this List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> persDesc, SearchDto settings) where TPersonSearchModel: class
    {
        if(settings.From is not null)
            persDesc.Add(s => s.DateRange(d => d.Field(BirdayField()).GreaterThanOrEquals(settings.From)));

        return persDesc;
    }

    public static List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> BirdayToFilter<TPersonSearchModel>(this List<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> persDesc, SearchDto settings) where TPersonSearchModel: class
    {
        if(settings.To is not null)
            persDesc.Add(s => s.DateRange(d => d.Field(BirdayField()).LessThanOrEquals(settings.To)));

        return persDesc;
    }
    public static async Task<IEnumerable<string>> FilmFromPersons(this IElasticClient elasticClient, SearchDto settings)
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
    
    static IEnumerable<Func<QueryContainerDescriptor<TPersonSearchModel>, QueryContainer>> MustPersonDesc<TPersonSearchModel>(SearchDto settings) where TPersonSearchModel : class =>
        QueryContainerList<TPersonSearchModel>()
        .PersonQueryFilter(settings)
        .KindOfPersonFilter(settings);

}