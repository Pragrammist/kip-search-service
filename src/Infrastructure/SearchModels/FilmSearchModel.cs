
using Core.Dtos;

namespace Infrastructure.Repositories;

public class FilmSearchModel
{
    public uint AgeLimit { get; set; }


    public string Id { get; set; } = null!;
    
    public string Banner { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Country { get; set; } = null!;

    

    public FilmType KindOfFilm { get; set; } 
    

    public FilmReleaseType ReleaseType { get; set; } 


    public TimeSpan? Duration { get; set; } 

    public DateTime? Release { get; set; }

    public DateTime? StartScreening { get; set; }

    public DateTime? EndScreening { get; set; }

    public string? Content { get; set; } 

    public int? Fees { get; set; } // сборы



    public double Score { get; set; }

    public uint ScoreCount { get; set; }

    public uint WillWatchCount { get; set; }

    public uint ShareCount { get; set; }

    public uint WatchedCount { get; set; }

    public uint ViewCount { get; set; }

    public uint NotInterestingCount { get; set; }


    public string[] Facts { get; set; } = new string[0];
    public string[] Images { get; set; } = new string[0];

    public string[] Stuff { get; set; } = new string[0];

    public string[] Articles { get; set; }  = new string[0];

    public string[] Trailers { get; set; } = new string[0];

    public string[] Tizers { get; set; }  = new string[0];

    public string[] RelatedFilms { get; set; }  = new string[0];

    public string[] Genres { get; set; } = new string[0];

    public string[] Nominations { get; set; } = new string[0];

    public SeasonSearchModel[] Seasons { get; set; } = new SeasonSearchModel[0];
}

public class SeasonSearchModel
{
    public uint Num { get; set; }

    public SeriaSearchModel[] Serias { get; set; } = new SeriaSearchModel[0];

    public string Banner { get; set; } = null!;
}
public class SeriaSearchModel
{
    public uint Num { get; set; }

    public string IdFile {  get; set; } = null!;
}
