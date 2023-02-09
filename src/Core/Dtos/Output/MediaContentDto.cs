using System.Security.Cryptography.X509Certificates;
namespace Core.Dtos;

public class MediaContentDto
{
    public IEnumerable<FilmSelectionDto> Selections { get; set;} = Enumerable.Empty<FilmSelectionDto>();

    public IEnumerable<string> Genres { get; set;} = Enumerable.Empty<string>();

    public IEnumerable<FilmTrailer> Trailers { get; set; } = Enumerable.Empty<FilmTrailer>();
}
