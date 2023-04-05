namespace Core.Dtos;

public class TrailerDto : IDable
{
    public string Id { get; set; } = null!;

    public string[] Trailers { get; set; } = new string[0];

    public string Name { get; set; } = null!;

    public string[] Genres { get; set; } = new string[0];

    public DateTime? StartScreening { get; set; }

    public FilmReleaseType ReleaseType { get; set; }
}