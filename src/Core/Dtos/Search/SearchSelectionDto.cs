using System.Security.Cryptography.X509Certificates;

namespace Core.Dtos.Search;

public class SearchSelectionDto
{
    public string? Query { get; set; }

    public List<string>? Genres { get; set; }

    public List<string>? Countries { get; set; }

    public DateTime From { get; set; } = new DateTime(1887, 1, 1); // по фильмам в которых снимались

    public DateTime To { get; set; } = DateTime.Now; // по фильмам в которых снимались

    public SortBy Sort { get; set; }
}