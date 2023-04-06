namespace Core.Dtos;

public class SearchMenuContentDot
{
    public IEnumerable<FilmSelectionDto> Selections { get; set;} = Enumerable.Empty<FilmSelectionDto>();

    public IEnumerable<string> Genres { get; set;} = Enumerable.Empty<string>();

    public IEnumerable<PersonShortDto> Today { get; set; } = Enumerable.Empty<PersonShortDto>();

    public IEnumerable<PersonShortDto> MostPopular { get; set; } = Enumerable.Empty<PersonShortDto>();
}
/*
    при поиске(не поиск)
    Подборки.
    Жанры.
    Персоны(родились сегодня, популярные*)
*/