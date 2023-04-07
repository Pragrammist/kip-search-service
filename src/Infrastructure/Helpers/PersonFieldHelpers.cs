using System.Linq.Expressions;
using Core.Dtos;
using Nest;

namespace Infrastructure.Repositories;

public static class PersonFieldHelpers
{
    public static Field BirdayField(double? boost = null)
    {
        Expression<Func<PersonDto, DateTime>> fieldExpr = f => f.Birthday;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    public static Field KindOfPersonField(double? boost = null)
    {
        Expression<Func<PersonDto, PersonType>> fieldExpr = f => f.KindOfPerson;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    public static Field PersonFactsField(double? boost = null)
    {
        Expression<Func<PersonDto, string[]>> fieldExpr = f => f.Facts;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    public static Field PersonNameField(double? boost = null)
    {
        Expression<Func<PersonDto, string>> fieldExpr = f => f.Name;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    public static Field PersonFilmsField(double? boost = null)
    {
        Expression<Func<PersonDto, string[]>> fieldExpr = f => f.Films;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    public static Field PersonNominationsField(double? boost = null)
    {
        Expression<Func<PersonDto, string[]>> fieldExpr = f => f.Nominations;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    public static Field CareerField(double? boost = null)
    {
        Expression<Func<PersonDto, string>> fieldExpr = f => f.Career;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

}
