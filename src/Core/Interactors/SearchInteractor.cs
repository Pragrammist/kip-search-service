using Core.Repositories;
using Core.Dtos;
using Core.Dtos.Search;
using System.Linq;


namespace Core.Interactors;

public class SearchInteractor
{
    readonly SearchRepository<FilmSelectionDto> _selections;
    readonly ByIdRepository<ShortFilmDto> _readRepoFilms;
    readonly FilmRepository<ShortFilmDto> _filmRepo;
    readonly GetManyRepository<FilmTrailer> _trailersRepo;
    public SearchInteractor(
        SearchRepository<FilmSelectionDto> selections, 
        FilmRepository<ShortFilmDto> filmRepo,
        ByIdRepository<ShortFilmDto> readRepoFilms,
        GetManyRepository<FilmTrailer> trailersRepo
    )
    {
        _trailersRepo = trailersRepo;
        _filmRepo = filmRepo;
        _readRepoFilms = readRepoFilms;
        _selections = selections;
    }
    public async Task<MediaContentDto> GenerateContent(SearchDto? settings = null)
    {
        settings = settings ?? new SearchDto {};


        return new MediaContentDto{
            Selections = await FillFilmObjects(
                selections: await _selections.Search(settings)
            ).ToListAsync(),
            Genres = await _filmRepo.GetGenres(),
            Trailers = await _trailersRepo.GetMany()
        };
    }
    async IAsyncEnumerable<FilmSelectionDto> FillFilmObjects(IEnumerable<FilmSelectionDto> selections)
    {
        foreach(var selection in selections)
        {   
            selection.FilmObjects = await _readRepoFilms.GetByIds(selection.Films);
            yield return selection;
        }
    }
}