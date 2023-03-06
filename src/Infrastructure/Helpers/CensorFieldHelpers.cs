using System.Linq.Expressions;
using Nest;

namespace Infrastructure.Repositories;

public static class CensorFieldHelpers
{
    public static Field CensorNameField(double? boost = null)
    {
        Expression<Func<CensorSearchModel, string>> fieldExpr = f => f.Name;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    public static Field CensorFilmsField(double? boost = null)
    {
        Expression<Func<CensorSearchModel, string[]>> fieldExpr = f => f.Films;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
}
