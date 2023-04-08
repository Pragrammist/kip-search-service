
using Core.Dtos;

namespace Infrastructure.Repositories;

public class PersonSearchModel
{
    public string Id { get; set; }  = null!;

    public PersonType[] KindOfPerson { get; set; } = new PersonType[] { default };

    public DateTime Birthday { get; set; }

    public string Name { get; set; }  = null!; 

    public string[] Photos { get; set; } = new string[0];

    public uint Height { get; set; }

    public string Career { get; set; } = null!; 

    public string BirthPlace { get; set; } = null!;

    public string[] Films { get; set; } = new string[0];

    public string[] Nominations { get; set; } = new string[0];

    public string[] Facts { get; set; } = new string[0];

    public string Banner { get; set; } = null!;
}