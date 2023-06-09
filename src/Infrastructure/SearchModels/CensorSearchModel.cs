namespace Infrastructure.Repositories;

public class CensorSearchModel
{
    public string Id  { get; set; } = null!;

    public string[] Films { get; set; } = new string[0];

    public string Name { get; set; } = null!;

    public string Image { get; set; } = null!;
}