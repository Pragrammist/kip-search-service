namespace Core.Dtos;

public class CensorDto : Idable
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string[] Films { get; set; } = new string[0];

}
