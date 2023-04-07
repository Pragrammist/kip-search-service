using System.Linq;
using Nest;
using Core.Repositories;
using Infrastructure.Repositories;
using Core.Dtos;

using System;
using System.Collections.Generic;
using Infrastructure.Configuration;

namespace IntegrationTests;

public class ElasticFixture : IDisposable
{
    private readonly DataFiller _elkDataFiller;
    private IElasticClient _elkClient; 

    public SearchRepository<CensorDto> Censors { get; }

    public SearchRepository<FilmShortDto> Films { get; }

    public FilmRepository<FilmShortDto> FilmRepo { get; }

    public ByIdRepository<FilmShortDto> ByIdFilmRepository { get; }

    public SearchRepository<PersonDto> Persons { get; }

    public SearchRepository<FilmSelectionDto> Selections { get; }


    public ElasticFixture()
    {
        
        var settings = new ConnectionSettings();
        settings.DefaultFieldNameInferrer(p => p);
        settings.ThrowExceptions();
        
        _elkClient = new ElasticClient(settings);
        _elkDataFiller= new DataFiller(_elkClient);
        _elkDataFiller.FillFilmsData();
        Censors = new SearchCensorRepositoryImpl<CensorDto>(_elkClient);
        Films = new SearchFilmRepositoryImpl<FilmShortDto>(_elkClient);
        Persons = new SearchPersonRepositoryImpl<PersonDto>(_elkClient);
        Selections = new SelectionRepositoryImpl<FilmSelectionDto>(_elkClient);
        FilmRepo = new ReadFilmRepositoryImpl<FilmShortDto>(_elkClient);
        
        ByIdFilmRepository = new ReadByIdRepoGeneric<FilmShortDto>(_elkClient, "films");
        
    }

    
    public void Dispose()
    {
        //_elkDataFiller.ClearFromData();
    }
   

}
