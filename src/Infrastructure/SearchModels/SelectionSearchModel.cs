namespace Infrastructure.Repositories;

public class SelectionSearchModel
{
    public string Id  { get; set; } = null!;

    public string[] Films { get; set; } = new string[0];

    public string Name { get; set; } = null!;
}
