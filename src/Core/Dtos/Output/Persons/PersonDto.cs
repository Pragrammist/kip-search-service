namespace Core.Dtos;

public class PersonDto : IDable
{
    public string Id { get; set; }  = null!;

    public PersonType KindOfPerson { get; set; } 

    public DateTime Birthday { get; set; }

    public string Name { get; set; }  = null!; 

    public string Photo { get; set; } = null!;

    public uint Height { get; set; }

    public string Career { get; set; } = null!; 

    public string BirthPlace { get; set; } = null!;

    public string[] Films { get; set; } = new string[0];

    public IEnumerable<FilmShortDto>? FilmObjects { get; set; }

    public string[] Nominations { get; set; } = new string[0];

}