namespace Core.Dtos;

public class FilmTrailer
{
    public string Id { get; set; } = null!;

    public string Trailer { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string[] Genres { get; set; } = new string[0];

    public FilmReleaseType ReleaseType { get; set; }
}