using Core;

namespace Core.Dtos.Search;

public class SearchFilmDto
{
    public string? Query { get; set; }

    public List<string>? Genres { get; set; }

    public List<string>? Countries { get; set; }

    public DateTime From { get; set; } = new DateTime(1887, 1, 1);

    public DateTime To { get; set; } = DateTime.Now;

    public SortBy Sort { get; set; } 
}
