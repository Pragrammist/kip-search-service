using Core;

namespace Core.Dtos;

public class FilmShortDto : Idable
{
    public string Id { get; set; } = null!;

    public string Banner { get; set; } = null!;
    
    public string[] Genres { get; set; } = new string[0];

    public string Name { get; set; } = null!;

    public double Score { get; set; } 
}