using System;
using System.Net.Http;
using Core.Dtos;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace IntegrationTest;

public class WebFixture : IDisposable
{
    readonly WebApplicationFactory<Program> _factory;
    public HttpClient Client { get; }
    public WebFixture()
    {
        _factory = new WebApplicationFactory<Program>();
        Client = _factory.CreateClient();
        var elkClient = _factory.Services.GetRequiredService<IElasticClient>();
        DataFiller filler = new DataFiller(elkClient);
        filler.FillFilmsData();
    }

    class DataFiller
    {
        const string FILM_INDEX = "films";
        const string PERSON_INDEX = "persons";
        const string CENSOR_INDEX = "censors";

        const string SELECTION_INDEX = "selections";
        IElasticClient _elkClient;
        public DataFiller(IElasticClient elkClient)
        {
            _elkClient = elkClient;
        }
        
        FilmSearchModel[] GetFilms() => new FilmSearchModel[]
        {
            new FilmSearchModel {
                Id = "f1",
                AgeLimit = 12,
                Articles = new string[] { "a1", "a2", "a3" },
                Banner = "ref",
                Content = "ref",
                Country = "Vegas",
                Description = "росский фильм про какое-то гавно",
                Duration = TimeSpan.Parse("01:30:11"),
                EndScreening = DateTime.Parse("2003-12-12"),
                Fees = 20000,
                Images = new string[] { "ref" },
                KindOfFilm = default,
                Name = "Супер герои",
                NotInterestingCount = 10,
                ReleaseType = FilmReleaseType.SCREENING,
                Score = 4.47,
                ScoreCount = 20,
                ShareCount = 20,
                StartScreening = DateTime.Parse("2000-12-03"),
                Stuff = new string[] { "p1", "p2" },
                Trailers = new string[] { "ref" },
                ViewCount = 10,
                WatchedCount = 200,
                WillWatchCount = 210,
                Release = null,
                Genres = new string[] {
                    "g1 g2",
                    "g3",
                    "g4"
                },
                Nominations = new string[] { "Фильм года", "Он классный", "ГРАФИКА ВЕКА" },
                RelatedFilms = new string[] { "ref" },
                Tizers = new string[] { "ref" }
            },
            new FilmSearchModel {
                Id = "f2",
                AgeLimit = 18,
                Articles = new string[] { "a3", "a4", "a5" },
                Banner = "ref",
                Content = null,
                Country = "Vegas 2",
                Description = "индийский боевик",
                Duration = TimeSpan.Parse("01:30:11"),
                EndScreening = DateTime.Parse("2013-09-03"),
                Fees = 20000000,
                Images = new string[] { "ref" },
                KindOfFilm = FilmType.SERIAL,
                Name = "Возращение на родину",
                NotInterestingCount = 0,
                ReleaseType = default,
                Score = 5,
                ScoreCount = 2000,
                ShareCount = 2000,
                StartScreening = DateTime.Parse("2012-11-11"),
                Stuff = new string[] { "p3", "p2" },
                Trailers = new string[] { "ref" },
                ViewCount = 20,
                WatchedCount = 300,
                WillWatchCount = 410,
                Release = DateTime.Parse("2014-03-02"),
                Seasons = new SeasonSearchModel[] { 
                    new SeasonSearchModel {
                        Num = 1,
                        Banner = "ref",
                        Serias = new SeriaSearchModel[] {
                            new SeriaSearchModel { 
                                IdFile = "ref",
                                Num = 1
                            }
                        }
                    }   
                },
                Genres = new string[] {
                    "g1 g2",
                    "g3",
                    "g4"
                },
                Nominations = new string[] { "Логика года" },
                RelatedFilms = new string[] { "ref" },
                Tizers = new string[] { "ref" }
            },
            new FilmSearchModel {
                Id = "f3",
                AgeLimit = 14,
                Articles = new string[] { "a7", "a8", "a9" },
                Banner = "ref",
                Content = "ref",
                Country = "vegas",
                Description = "Жили были не тужили но вот началось",
                Duration = TimeSpan.Parse("01:30:11"),
                EndScreening = DateTime.Parse("1999-09-01"),
                Fees = 3000,
                Images = new string[] { "ref" },
                KindOfFilm = FilmType.SERIAL,
                Name = "Что-то там про прошельцев 2",
                NotInterestingCount = 300000,
                ReleaseType = default,
                Score = 4,
                ScoreCount = 5000,
                ShareCount = 20,
                StartScreening = DateTime.Parse("1998-11-11"),
                Stuff = new string[] { "p2"},
                Trailers = new string[] { "ref" },
                ViewCount = 200000,
                WatchedCount = 2000,
                WillWatchCount = 2000,
                Release = DateTime.Parse("1999-12-01"),
                Genres = new string[] {
                    "g6"
                },
                Nominations = new string[] { "Лучшее продолжение года" },
                RelatedFilms = new string[] { "ref" },
                Tizers = new string[] { "ref" },
                
            },
            new FilmSearchModel {
                Id = "f4",
                AgeLimit = 6,
                Articles = new string[] { "a7", "a8", "a9" },
                Banner = "ref",
                Content = null,
                Country = "Vegas 3",
                Description = "фильм от США про какие русские плохие",
                Duration = TimeSpan.Parse("01:30:11"),
                EndScreening = null,
                Fees = 30000,
                Images = new string[] { "ref" },
                KindOfFilm = FilmType.SERIAL,
                Name = "Что-то там про прошельцев",
                NotInterestingCount = 20000,
                ReleaseType = FilmReleaseType.PREMIERA,
                Score = 0,
                ScoreCount = 0,
                ShareCount = 1000,
                StartScreening = null,
                Stuff = new string[] { "p1"},
                Trailers = new string[] { "ref" },
                ViewCount = 10000,
                WatchedCount = 0,
                WillWatchCount = 20,
                Release = DateTime.Parse("2018-01-01"),
                Genres = new string[] {
                    "g6"
                },
                Nominations = new string[] { "Русский злодей года" },
                RelatedFilms = new string[] { "ref" },
                Tizers = new string[] { "ref" },
                
            },
            new FilmSearchModel {
                Id = "f5",
                AgeLimit = 6,
                Articles = new string[] { "a1", "a2", "a3" },
                Banner = "ref",
                Content = null,
                Country = "Mexico",
                Description = "Жизнь",
                Duration = null,
                EndScreening = DateTime.Parse("2005-09-01"),
                Fees = 40000,
                Images = new string[] { "ref" },
                KindOfFilm = FilmType.SERIAL,
                Name = "Как просрать жизнь",
                NotInterestingCount = 40000,
                ReleaseType = FilmReleaseType.SCREENING,
                Score = 5,
                ScoreCount = 7000,
                ShareCount = 30,
                StartScreening = DateTime.Parse("2005-10-01"),
                Stuff = new string[] { "p1"},
                Trailers = new string[] { "ref" },
                ViewCount = 20,
                WatchedCount = 20,
                WillWatchCount = 20,
                Release = null,
                Genres = new string[] {
                    "g1",
                    "g2"
                },
                Nominations = new string[] { "Учитель года" },
                RelatedFilms = new string[] { "ref" },
                Tizers = new string[] { "ref" }
            },
            new FilmSearchModel {
                Id = "f6",
                AgeLimit = 18,
                Articles = new string[] { "a7", "a8", "a9" },
                Banner = "ref",
                Content = "ref",
                Country = "Nano",
                Description = "Приоритет какой-то запах",
                Duration = null,
                EndScreening = DateTime.Parse("2006-09-01"),
                Fees = 600,
                Images = new string[] { "ref" },
                KindOfFilm = FilmType.SERIAL,
                Name = "Как просрать жизнь 2",
                NotInterestingCount = 400,
                ReleaseType = default,
                Score = 5,
                ScoreCount = 7000,
                ShareCount = 30,
                StartScreening = DateTime.Parse("2006-10-01"),
                Stuff = new string[] { "p1", "p2"},
                Trailers = new string[] { "ref" },
                ViewCount = 20,
                WatchedCount = 20,
                WillWatchCount = 20,
                Release = DateTime.Parse("2006-11-02"),
                Genres = new string[] {
                    "g4",
                    "g6"
                },
                Nominations = new string[] { "Философ года" },
                RelatedFilms = new string[] { "ref" },
                Tizers = new string[] { "ref" }
            },
        };
        FilmSelectionSearchModel[] GetSelections() => new FilmSelectionSearchModel[]
        {
            
            new FilmSelectionSearchModel{
                Id = "s1",
                Films = new[] {"f1", "f3"},
                Name = "Отдохните вечером"
            },
            new FilmSelectionSearchModel{
                Id = "s2",
                Films = new[] {"f2", "f6", "f4"},
                Name = "Приятно проведите время"
            },
            new FilmSelectionSearchModel{
                Id = "s3",
                Films = new[] {"f5"},
                Name = "Секретный фильм (:"
            }
        };
        CensorSearchModel[] GetCensors() => new CensorSearchModel[] 
        {
            new CensorSearchModel
            {
                Id = "c1",
                Films = new[]{"f1", "f2", "f3"},
                Name = "лучши фильмы по версии кого-то другого"
            },
            new CensorSearchModel
            {
                Id = "c2",
                Films = new[]{"f1", "f3", "f4"},
                Name = "меня роняли"
            },
            new CensorSearchModel
            {
                Id = "c2",
                Films = new[]{"f1", "f2", "f4"},
                Name = "секс это конечно хорошо а вы ездили в тюмень"
            }


        };
        PersonSearchModel[] GetPersons() => new PersonSearchModel[] {
            new PersonSearchModel { 
                Id = "p1",
                Birthday = DateTime.Parse("2004-12-29"),
                BirthPlace = "В канаве",
                Career = "По жизни этому человеку не повезло очень сильно",
                Films = new string[] { "f1", "f2", "f4", "f5", "f6" },
                Height = 189,
                KindOfPerson = default,
                Name = "Букин Алексей Петрович",
                Nominations = new string[] {"Почти гена букин"},
                Photo = "ref",
            },
            new PersonSearchModel { 
                Id = "p2",
                Birthday = DateTime.Parse("1999-12-29"),
                BirthPlace = "Таганрог",
                Career = "По жизни этому человеку не повезло очень сильно",
                Films = new string[] { "f1", "f2", "f3", "f6" },
                Height = 169,
                KindOfPerson = PersonType.PRODUCER,
                Name = "Папич Алексей Юрьевич",
                Nominations = new string[] {"Выигрывает в казино", "стример года"},
                Photo = "ref",
            },
            new PersonSearchModel { 
                Id = "p3",
                Birthday = DateTime.Parse("1989-03-13"),
                BirthPlace = "Владивосток",
                Career = "Карьера супер просто",
                Films = new string[] { "f2" },
                Height = 159,
                KindOfPerson = PersonType.ACTOR,
                Name = "Имененко Имя Имьянич",
                Nominations = new string[] {"Выигрывает в казино", "стример года"},
                Photo = "ref",
            },
        };
        public void FillFilmsData()
        {
            var filmsResponse =_elkClient.IndexMany(GetFilms(), FILM_INDEX);
            

            var personsResponse =_elkClient.IndexMany(GetPersons(), PERSON_INDEX);

            var selectionsResponse = _elkClient.IndexMany(GetSelections(), SELECTION_INDEX);
        

            var censrorsResponse =_elkClient.IndexMany(GetCensors(), CENSOR_INDEX);
        }
        
    }


    public void Dispose()
    {
        _factory.Dispose();
    }
}
