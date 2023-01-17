using System.Collections.Generic;
namespace Core;
public class FilmSelection
{
    public string Id  {get; set; } = null!;

    public List<string> Films { get; set; } = new List<string>();

    public string Name { get; set; } = null!;
}
