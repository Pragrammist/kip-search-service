using Core;
using Nest;

namespace Infrastructure.Repositories;

public static class ExpressionHelpers
{
    public static IEnumerable<TObj> SelectHitsWithId<TObj>(this ISearchResponse<TObj> response) where TObj: class, IDable
    {
        if(response.Hits.Count == 0)
            return Enumerable.Empty<TObj>();

        var select = response.Hits.Select(s => {
            var source = s.Source;
            source.Id = s.Id;
            return source;
        });
        return select ?? Enumerable.Empty<TObj>();;
    }
}
