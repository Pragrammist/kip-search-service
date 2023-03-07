using Nest;

namespace Infrastructure.Repositories;

public static class DescriptorHelpers
{
    public static List<Func<QueryContainerDescriptor<Model>, QueryContainer>> QueryContainerList<Model>() where Model : class
         => new List<Func<QueryContainerDescriptor<Model>, QueryContainer>>();

    public static List<Func<QueryContainerDescriptor<TModel>, QueryContainer>> AddShouldDesc<TModel>(this List<Func<QueryContainerDescriptor<TModel>, QueryContainer>> filmDesc, IEnumerable<Func<QueryContainerDescriptor<TModel>, QueryContainer>> shouldDescr) where TModel: class
    {
        filmDesc.Add(q => q.Bool(b => b.Should(shouldDescr)));
        return filmDesc;
    }

    public static List<Func<QueryContainerDescriptor<TModel>, QueryContainer>> ValuesFilter<TModel>(this List<Func<QueryContainerDescriptor<TModel>, QueryContainer>> filmDesc, Field field, IEnumerable<string> values) where TModel : class
    {
        if(values.Count() > 0)
            filmDesc.Add(q => q
                .Terms(t => t.Terms(values).Field(field))
            );
        return filmDesc;
    }

    public static List<Func<QueryContainerDescriptor<TModel>, QueryContainer>> IdsFilter<TModel>(this List<Func<QueryContainerDescriptor<TModel>, QueryContainer>> filmDesc, IEnumerable<string> valIds) where TModel : class
    {
        if(valIds.Count() > 0)
            filmDesc.Add(q => q.Ids(ids => ids.Values(valIds)));
        
        return filmDesc;
    }
}
