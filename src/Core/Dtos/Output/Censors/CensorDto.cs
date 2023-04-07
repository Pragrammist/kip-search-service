namespace Core.Dtos;

public class CensorDto : IDable
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public IEnumerable<FilmShortDto> FilmObjects { get; set; } = Enumerable.Empty<FilmShortDto>();

    public string[] Films { get; set; } = new string[0];

    public string Image { get; set; } = null!;
}
