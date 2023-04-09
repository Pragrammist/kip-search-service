using System.Text.Json.Serialization;

namespace Core.Dtos;


public class FilmDto : IDable
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

    public FilmFeesDto[] Fees { get; set; } = new FilmFeesDto[] { };


    

    public double Score { get; set; }

    public uint ScoreCount { get; set; }

    public uint WillWatchCount { get; set; }

    public uint ShareCount { get; set; }

    public uint WatchedCount { get; set; }

    public uint ViewCount { get; set; }

    public uint NotInterestingCount { get; set; }


    public string[] Images { get; set; } = new string[0];

    public StuffInFilmByProfessionDto[] Stuff { get; set; } = new StuffInFilmByProfessionDto[0];

    public string[] Facts { get; set; } = new string[0];

    public string[] Articles { get; set; }  = new string[0];

    public string[] Trailers { get; set; } = new string[0];

    public string[] Tizers { get; set; }  = new string[0];

    [JsonIgnore]
    public string[] RelatedFilms { get; set; }  = new string[0];

    public IEnumerable<FilmShortDto>? RelatedFilmObjects { get; set; }  

    public string[] Genres { get; set; } = new string[0];

    public string[] Nominations { get; set; } = new string[0];

    public SeasonDto[] Seasons { get; set; } = new SeasonDto[0];
}

public class StuffInFilmByProfessionDto
{
    public PersonType Profession { get; set; }

    [JsonIgnore]
    public string[] Stuff { get; set; } = new string[0];

    public IEnumerable<PersonShortDto>? StuffObjects { get; set; }
}