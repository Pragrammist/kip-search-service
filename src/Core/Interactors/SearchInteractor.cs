using Core.Repositories;
using Core.Dtos;
using Core.Dtos.Search;
using System.Linq;


namespace Core.Interactors;

public class SearchInteractor
{
    readonly SearchRepository<FilmSelectionDto> _selections;
    readonly SearchRepository<PersonShortDto> _searchPerson;
    readonly ByIdRepository<FilmShortDto> _readRepoShortFilms;
    readonly ByIdRepository<FilmDto> _readRepoFilms;
    readonly ByIdRepository<PersonShortDto> _readRepoShortPersons;
    readonly ByIdRepository<PersonDto> _readRepoPersons;
    readonly ByIdRepository<FilmSelectionDto> _readRepoSelections;
    readonly ByIdRepository<CensorDto> _readRepoCensors;
    readonly FilmRepository<FilmShortDto> _filmRepo;
    readonly SearchRepository<TrailerDto> _serchRepoTrailer;
    public SearchInteractor(
        SearchRepository<FilmSelectionDto> selections, 
        FilmRepository<FilmShortDto> filmRepo,
        ByIdRepository<FilmShortDto> readRepoShortFilms,
        SearchRepository<TrailerDto> serchRepoTrailer,
        ByIdRepository<FilmDto> readRepoFilms,
        ByIdRepository<PersonShortDto> readRepoShortPersons,
        ByIdRepository<PersonDto> readRepoPersons,
        ByIdRepository<FilmSelectionDto> readRepoSelections,
        ByIdRepository<CensorDto> readRepoCensors,
        SearchRepository<PersonShortDto> searchPerson
    )
    {
        _serchRepoTrailer = serchRepoTrailer;
        _filmRepo = filmRepo;
        _readRepoShortFilms = readRepoShortFilms;
        _selections = selections;
        _readRepoFilms = readRepoFilms;
        _readRepoShortPersons = readRepoShortPersons;
        _readRepoPersons = readRepoPersons;
        _readRepoSelections = readRepoSelections;
        _readRepoCensors = readRepoCensors;
        _searchPerson = searchPerson;
    }


    public async Task<SearchMenuContentDto> GenerateSearchContent(SearchDto? settings = null)
    {
        settings = settings ?? new SearchDto {};

        
        return new SearchMenuContentDto
        {
            Genres = await _filmRepo.GetGenres(),
            Selections = await FillFilmObjects(
                selections: await _selections.Search(settings)
            ).ToListAsync(),
            Today = await _searchPerson.Search(new SearchDto {
                From = GetFromDateTime()
            }),
            MostPopular = await _searchPerson.Search(new SearchDto {
                Sort = SortBy.POPULARIY
            })
        };
    }
    DateTime GetFromDateTime() => new DateTime(
        year: DateTime.Today.Year - 30,
        month: 1,
        day: 1
    );
    public async Task<MediaContentDto> GenerateMediaContent(SearchDto? settings = null)
    {
        settings = settings ?? new SearchDto {};


        return new MediaContentDto{
            Selections = await FillFilmObjects(
                selections: await _selections.Search(settings)
            ).ToListAsync(),
            Genres = await _filmRepo.GetGenres(),
            Trailers = await _serchRepoTrailer.Search(new SearchDto{ ReleaseType = FilmReleaseType.SCREENING})
        };
    }
    async IAsyncEnumerable<FilmSelectionDto> FillFilmObjects(IEnumerable<FilmSelectionDto> selections)
    {
        foreach(var selection in selections)
        {   
            selection.FilmObjects = await _readRepoShortFilms.GetByIds(selection.Films);
            yield return selection;
        }
    }


    public async Task<FilmDto?> FindFilmById(string id)
    {
        var films = await _readRepoFilms.GetByIds(id);
        var film = films.FirstOrDefault();
        if(film is null)
            return film;
        await FillStuffByStuffObjects(film);
        film.RelatedFilmObjects = await _readRepoShortFilms.GetByIds(film.RelatedFilms);
        return film;
    }

    async Task FillStuffByStuffObjects(FilmDto film)
    {
        foreach(var person in film.Stuff)
            person.StuffObjects = await _readRepoShortPersons.GetByIds(person.Stuff);
    }

    string[] GetAllStuffIdsFromFilm(FilmDto film) => film.Stuff
    .Select(s => s.Stuff)
    .Aggregate((s1, s2) => 
    {
        var res = s1.ToList();
        res.AddRange(s2);
        return res.ToArray();
    });

    public async Task<PersonDto?> FindPersonById(string id)
    {
        var persons = await _readRepoPersons.GetByIds(id);
        var person = persons.FirstOrDefault();
        if(person is null)
            return person;
        
        person.FilmObjects = await _readRepoShortFilms.GetByIds(person.Films);
        return person;
    }

    public async Task<FilmSelectionDto?> FindSelectionById(string id)
    {
        var selections = await _readRepoSelections.GetByIds(id);
        var selection = selections.FirstOrDefault();
        if(selection is null)
            return selection;
        
        selection.FilmObjects = await _readRepoShortFilms.GetByIds(selection.Films);
        return selection;
    }

    public async Task<CensorDto?> FindCensorById(string id)
    {
        var censors = await _readRepoCensors.GetByIds(id);
        var censor = censors.FirstOrDefault();
        if(censor is null)
            return censor;
        
        censor.FilmObjects = await _readRepoShortFilms.GetByIds(censor.Films);
        return censor;
    }
}