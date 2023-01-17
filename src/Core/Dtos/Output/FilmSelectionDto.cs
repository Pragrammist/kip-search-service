
namespace Core.Dtos;

public class FilmSelectionDto
{
    public string Id  { get; set; } = null!;

    public List<string> Films { get; set; } = new List<string>();

    public string Name { get; set; } = null!;
}

