using Microsoft.AspNetCore.Mvc;
using Core.Repositories;
using Core.Interactors;
using Core.Dtos.Search;
using Core.Dtos;
namespace Web.Controllers;

[ApiController]
public class SearchController : ControllerBase
{
    readonly SearchRepository<FilmShortDto> _films;
    readonly SearchRepository<PersonShortDto> _persons;
    readonly SearchRepository<CensorShortDto> _censors;
    readonly SearchInteractor _searchInteractor;
    readonly SearchRepository<SelectionShortDto> _selections;
    public SearchController(
        SearchRepository<FilmShortDto> films, 
        SearchRepository<PersonShortDto> persons, 
        SearchRepository<CensorShortDto> censors, 
        SearchRepository<SelectionShortDto> selections,
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
            value: await _searchInteractor.GenerateMediaContent()
        );
    }

    [HttpGet("films/{id}")]
    public async Task<IActionResult> GetFilmById(string id)
    {
        var searchObject = await _searchInteractor.FindFilmById(id);
         
        return searchObject is null ? BadRequest() : new ObjectResult(searchObject);
    }

    [HttpGet("persons/{id}")]
    public async Task<IActionResult> GetPersonById(string id)
    {
        var searchObject = await _searchInteractor.FindPersonById(id);
         
        return searchObject is null ? BadRequest() : new ObjectResult(searchObject);
    }

    [HttpGet("censors/{id}")]
    public async Task<IActionResult> GetCensorById(string id)
    {
        var searchObject = await _searchInteractor.FindCensorById(id);
         
        return searchObject is null ? BadRequest() : new ObjectResult(searchObject);
    }

    [HttpGet("selections/{id}")]
    public async Task<IActionResult> GetSelectionById(string id)
    {
        var searchObject = await _searchInteractor.FindSelectionById(id);
         
        return searchObject is null ? BadRequest() : new ObjectResult(searchObject);
    }
}

