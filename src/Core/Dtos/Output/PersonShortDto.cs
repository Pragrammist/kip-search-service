namespace Core.Dtos;

public class PersonShortDto : Idable
{
    public string Id { get; set; } = null!;
    
    public string Name { get; set; }  = null!;

    public PersonType KindOfPerson { get; set; }

    public string Photo { get; set; } = null!;  
}