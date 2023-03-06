using System.Linq.Expressions;
using Nest;

namespace Infrastructure.Repositories;

public static class SelectionFieldHelpers
{
    public static Field SelectionNameField(double? boost = null)
    {
        Expression<Func<FilmSelectionSearchModel, string>> fieldExpr = f => f.Name;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    public static Field SelectionFilmsField(double? boost = null)
    {
        Expression<Func<FilmSelectionSearchModel, string[]>> fieldExpr = f => f.Films;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
}