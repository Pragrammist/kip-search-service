using Microsoft.AspNetCore.Mvc;
using Core.Repositories;
using Core.Interactors;
using Core.Dtos.Search;
using Core.Dtos;
namespace Web.Controllers;

[ApiController]
public class SearchController : ControllerBase
{
    readonly SearchRepository<FilmDto> _films;
    readonly SearchRepository<PersonDto> _persons;
    readonly SearchRepository<CensorDto> _censors;
    readonly SearchInteractor _searchInteractor;
    readonly SearchRepository<FilmSelectionDto> _selections;
    public SearchController(
        SearchRepository<FilmDto> films, 
        SearchRepository<PersonDto> persons, 
        SearchRepository<CensorDto> censors, 
        SearchRepository<FilmSelectionDto> selections,
        SearchInteractor searchInteractor
    )
    {
        _films = films;
        _persons = persons;
        _censors = censors;
        _selections = selections;
        _searchInteractor = searchInteractor;
    }
    [HttpPost("films")]
    
    public async Task<IActionResult> SearchFilms(SearchDto search)
    {
        return new ObjectResult(await _films.Search(search));
    }

    [HttpPost("persons")]
    public async Task<IActionResult> SearchPersons(SearchDto search)
    {
        return new ObjectResult(await _persons.Search(search));
    }


    [HttpPost("selections")]
    public async Task<IActionResult> SearchSelections(SearchDto search)
    {
        return new ObjectResult(await _selections.Search(search));
    }
    
    [HttpPost("censors")]
    public async Task<IActionResult> SearchCensors(SearchDto search)
    {
        return new ObjectResult(await _censors.Search(search));
    }

    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
    [HttpGet("media")]
    public async Task<IActionResult> GetMediaContent()
    {
        return new ObjectResult(
            value: await _searchInteractor.GenerateContent()
        );
    }
}

