

namespace Core.Dtos.Search;

public class SearchDto
{
    public string? Query { get; set; }

    public string[]? Genres { get; set; }

    public uint? AgeLimit { get; set; }

    public string[]? Countries { get; set; }

    public DateTime From { get; set; } = new DateTime(1887, 1, 1, 0, 0, 0);

    public DateTime To { get; set; } = new DateTime(2023, 01, 28, 0, 0, 0);

    public SortBy? Sort { get; set; }  

    public uint Page { get; set; } = 1;

    public uint Take { get; set; } = 20;

    public FilmType? KindOfFilm { get; set; }

    public FilmReleaseType? ReleaseType { get; set; }

    public PersonType? KindOfPerson {get; set; }

    //public bool ShowWatched { get; set; }
}