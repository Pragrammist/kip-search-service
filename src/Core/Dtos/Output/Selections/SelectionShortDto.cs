namespace Core.Dtos;

public class SelectionShortDto : IDable
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Image { get; set; } = null!;
}
