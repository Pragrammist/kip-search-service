namespace Core.Dtos;

public class PersonShortDto : IDable
{
    public string Id { get; set; } = null!;
    
    public string Name { get; set; }  = null!;

    public PersonType[] KindOfPerson { get; set; } = null!; 

    public string Banner { get; set; } = null!;
    
    public string[] Photos { get; set; } = null!;  
}