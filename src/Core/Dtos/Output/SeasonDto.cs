namespace Core.Dtos;

public class SeasonDto
{
    public uint Num { get; set; }

    public SeriaDto[] Serias { get; set; } = new SeriaDto[0];

    public string Banner { get; set; } = null!;
}
