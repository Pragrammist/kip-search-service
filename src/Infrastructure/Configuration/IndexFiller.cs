using Infrastructure.Repositories;
using Nest;
namespace Infrastructure.Configuration;

public class IndexFiller
{
    readonly IElasticClient _elasticClient;
    public IndexFiller(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public void FillIndexes()
    {
        // _elasticClient.Map<FilmSearchModel>( m => m
        //     .Index("films")
        //     .Properties(p => p
        //         .Scalar(f => (int)f.AgeLimit)
        //         .Keyword(f => f.Name(n => n.Articles).Fields(fs => fs.Keyword(k => k.Name(n => n.Articles))))
        //         .Keyword(f => f.Name(n => n.Banner).Index(false))
        //         .Keyword(f => f.Name(n => n.Content).Index(false))
        //         .Text(f => f.Name(n => n.Country))
        //         .Text(f => f.Name(n => n.Description))
        //         .Keyword(f => f.Name(n => n.Duration).Index(false))
        //         .Date(f => f.Index(false).Name(f => f.EndScreening))
        //         .Scalar(f => (int?)f.Fees)
        //         .Keyword(f => f.Name(n => n.Genres))
        //         .Keyword(f => f.Name(n => n.Images).Index(false))
        //         .Scalar(f => (int)f.KindOfFilm)
        //         .Text(f => f.Name(n => n.Name))
        //         .Keyword(f => f.Name(n => n.Nominations))
        //         .Scalar(f => (int)f.NotInterestingCount)
        //         .Keyword(f => f.Name(n => n.RelatedFilms))
        //         .Date(f => f.Index(false).Name(f => f.Release))
        //         .Scalar(f => f.Score)
        //         .Scalar(f => (int)f.ScoreCount)
        //         .Object<SeasonSearchModel>(f => f.Name(n => n.Seasons))
        //         .Scalar(f => (int)f.ShareCount)
        //         .Keyword(f => f.Name(n => n.Stuff))
        //         .Keyword(f => f.Name(n => n.Tizers).Index(false))
        //         .Keyword(f => f.Name(n => n.Trailers).Index(false))
        //         .Scalar(f => (int)f.ViewCount)
        //         .Scalar(f => (int)f.WatchedCount)
        //         .Scalar(f => (int)f.WillWatchCount)
        //     )
        // );

        // _elasticClient.Map<PersonSearchModel>( m => m
        //     .Index("persons")
        //     .Properties(p => p
        //         .Text(f => f.Name(n => n.BirthPlace))
        //         .Date(f => f.Index(false).Name(f => f.Birthday))
        //         .Keyword(f => f.Name(n => n.Nominations))
        //         .Scalar(f => (int)f.Height)
        //         .Text(f => f.Name(n => n.Career))
        //         .Text(f => f.Name(n => n.Name))
        //         .Keyword(f => f.Name(n => n.Films))
        //         .Scalar(f => (int)f.KindOfPerson)
        //         .Keyword(f => f.Name(n => n.Photo))   
        //     )
        // );

        // _elasticClient.Map<FilmSelectionSearchModel>( m => m
        //     .Index("selections")
        //     .Properties(p => p
        //         .Text(f => f.Name(n => n.Name))
        //         .Keyword(f => f.Name(n => n.Films))
        //     )
        // );

        //  _elasticClient.Map<CensorSearchModel>( m => m
        //     .Index("censors")
        //     .Properties(p => p
        //         .Text(f => f.Name(n => n.Name))
        //         .Keyword(f => f.Name(n => n.Films))
        //     )
        // );  
    }
    
}

