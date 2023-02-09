
namespace Core.Dtos;

public class FilmSelectionDto
{
    public string Id  { get; set; } = null!;

    public string[] Films { get; set; } = new string[0];

    public IEnumerable<ShortFilmDto> FilmObjects { get; set; } = Enumerable.Empty<ShortFilmDto>();

    public string Name { get; set; } = null!;
}

