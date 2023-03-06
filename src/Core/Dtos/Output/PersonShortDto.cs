namespace Core.Dtos;

public class PersonShortDto
{
    public string Name { get; set; }  = null!;

    public PersonType KindOfPerson { get; set; }

    public string Photo { get; set; } = null!;  
}