using System.Linq.Expressions;
using Core.Dtos;
using Nest;

namespace Infrastructure.Repositories;

public static class FilmFieldHelpers
{
    public static Field DescriptionField(double? boost = null)
    {
        Expression<Func<FilmDto, string>> fieldExpr = f => f.Description;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    public static Field FilmNominationsField(double? boost = null)
    {
        Expression<Func<FilmDto, string[]>> fieldExpr = f => f.Nominations;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    public static Field FilmCountryField(double? boost = null)
    {
        Expression<Func<FilmDto, string>> fieldExpr = f => f.Country;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    public static Field KindOfFilmField(double? boost = null)
    {
        Expression<Func<FilmDto, FilmType>> fieldExpr = f => f.KindOfFilm;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    public static Field ReleaseTypeField(double? boost = null)
    {
        Expression<Func<FilmDto, FilmReleaseType>> fieldExpr = f => f.ReleaseType;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    public static Field ReleaseField(double? boost = null)
    {
        Expression<Func<FilmDto, DateTime?>> fieldExpr = f => f.Release;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    public static Field StartSreeningField(double? boost = null)
    {
        Expression<Func<FilmDto, DateTime?>> fieldExpr = f => f.StartScreening;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    public static Field EndSreeningField(double? boost = null)
    {
        Expression<Func<FilmDto, DateTime?>> fieldExpr = f => f.StartScreening;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    public static Field AgeLimitField(double? boost = null)
    {
        Expression<Func<FilmDto, uint>> fieldExpr = f => f.AgeLimit;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    public static Field GenresField(double? boost = null)
    {
        Expression<Func<FilmDto, string[]>> fieldExpr = f => f.Genres;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    public static Field FilmFactsField(double? boost = null)
    {
        Expression<Func<FilmDto, string[]>> fieldExpr = f => f.Facts;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    public static Field FilmNameField(double? boost = null)
    {
        Expression<Func<FilmDto, string>> fieldExpr = f => f.Name;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    public static Field WatchedCountField(double? boost = null)
    {
        Expression<Func<FilmDto, uint>> fieldExpr = f => f.WatchedCount;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }
    public static Field ScoreField(double? boost = null)
    {
        Expression<Func<FilmDto, double>> fieldExpr = f => f.Score;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

    public static Field ViewCountField(double? boost = null)
    {
        Expression<Func<FilmDto, uint>> fieldExpr = f => f.ViewCount;
        Field res =  fieldExpr;
        res.Boost = boost;
        return res;
    }

}
