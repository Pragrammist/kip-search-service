namespace Core.Dtos;

public class PersonDto : IDable
{
    public string Id { get; set; }  = null!;

    public PersonType[] KindOfPerson { get; set; } = null!; 

    public DateTime Birthday { get; set; }

    public string Name { get; set; }  = null!; 

    public string[] Photos { get; set; } = new string[0];

    public uint Height { get; set; }

    public string Career { get; set; } = null!; 

    public string BirthPlace { get; set; } = null!;

    public string[] Facts { get; set; } = new string[0];

    public string[] Films { get; set; } = new string[0];

    public IEnumerable<FilmShortDto>? FilmObjects { get; set; }

    public string[] Nominations { get; set; } = new string[0];

}