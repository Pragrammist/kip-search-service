
namespace Core.Dtos;

public class FilmSelectionDto : IDable
{
    public string Id  { get; set; } = null!;

    public string[] Films { get; set; } = new string[0];

    public IEnumerable<FilmShortDto> FilmObjects { get; set; } = Enumerable.Empty<FilmShortDto>();

    public string Name { get; set; } = null!;
}

