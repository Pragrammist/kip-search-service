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
        var objs = await _films.Search(search);

        return objs.FirstOrDefault() is null ? NotFound() : new ObjectResult(objs); 
    }

    [HttpPost("persons")]
    public async Task<IActionResult> SearchPersons(SearchDto search)
    {
        var objs = await _persons.Search(search);
        
        return objs.FirstOrDefault() is null ? NotFound() : new ObjectResult(objs); 
    }


    [HttpPost("selections")]
    public async Task<IActionResult> SearchSelections(SearchDto search)
    {
        var objs = await _selections.Search(search);
        
        return objs.FirstOrDefault() is null ? NotFound() : new ObjectResult(objs); 
    }
    
    [HttpPost("censors")]
    public async Task<IActionResult> SearchCensors(SearchDto search)
    {
        var objs = await _censors.Search(search);
        
        return objs.FirstOrDefault() is null ? NotFound() : new ObjectResult(objs);
    }

    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
    [HttpGet("media")]
    public async Task<IActionResult> GetMediaContent()
    {
        var objs = await _searchInteractor.GenerateMediaContent();
        
        return 
            objs.Genres.FirstOrDefault() is null && 
            objs.Selections.FirstOrDefault() is null && 
            objs.Trailers.FirstOrDefault() is null 
            ? NotFound()  : new ObjectResult(objs);
        
    }
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
    [HttpGet("searchcontent")]
    public async Task<IActionResult> GetSearchContent()
    {
        var objs = await _searchInteractor.GenerateSearchContent();
        
        return 
            objs.Genres.FirstOrDefault() is null && 
            objs.Selections.FirstOrDefault() is null && 
            objs.Today.FirstOrDefault() is null && 
            objs.MostPopular.FirstOrDefault() is null
            ? NotFound()  : new ObjectResult(objs);
        
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

