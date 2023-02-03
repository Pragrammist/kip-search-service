using System.Security.Cryptography.X509Certificates;
namespace Core;
public class FilmSelection
{
    public string Id  {get; set; } = null!;

    public FilmSelection(string[] films, string name)
    {
        Films = films;
        Name = name;
    }
    public string[] Films { get; set; } = new string[0];

    public string Name { get; set; } = null!;
}
