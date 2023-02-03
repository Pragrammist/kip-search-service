using Nest;
using Core.Repositories;
using Infrastructure.Repositories;
using Core.Dtos;

using System;

namespace IntegrationTests;

public class ElasticFixture : IDisposable
{
    private IElasticClient _elkClient; 

    public SearchRepository<CensorDto> Censors { get; }

    public SearchRepository<FilmDto> Films { get; }

    public SearchRepository<PersonDto> Persons { get; }

    public SelectionRepository Selections { get; }

    public ElasticFixture()
    {
        var settings = new ConnectionSettings();
        settings.DefaultFieldNameInferrer(p => p);
        settings.ThrowExceptions();
        
        _elkClient = new ElasticClient(settings);
        Censors = new SearchCensorRepositoryImpl(_elkClient);
        Films = new SearchFilmRepositoryImpl(_elkClient);
        Persons = new SearchPersonRepositoryImpl(_elkClient);
        Selections = new SelectionRepositoryImpl(_elkClient);
    }

    
    public void Dispose()
    {
        
    }
}
