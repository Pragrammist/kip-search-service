using System.Security.Cryptography.X509Certificates;

namespace Core.Dtos.Search;

public class Search
{
    public string? Query { get; set; }

    public List<string>? Genres { get; set; }

    public List<string>? Countries { get; set; }

    public DateTime From { get; set; } = new DateTime(1887, 1, 1);

    public DateTime To { get; set; } = DateTime.Now;

    public SortBy Sort { get; set; } 

    public bool ShowWatched { get; set; }
}