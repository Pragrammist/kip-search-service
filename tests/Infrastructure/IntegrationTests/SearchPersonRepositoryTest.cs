using System;
using Xunit;
using System.Linq;
using Core.Dtos.Search;
using System.Threading.Tasks;
using FluentAssertions;
using Core.Dtos;
using Core;

namespace IntegrationTests;


[Collection("Elastic")]
public class SearchPersonRepositoryTest
{
    readonly ElasticFixture _elasticFixture;
    public SearchPersonRepositoryTest(ElasticFixture elasticFixture)
    {
        _elasticFixture = elasticFixture;
    }

    [Theory]
    [InlineData("Папич Алексей Юрьевич", false)]
    [InlineData("Почти гена букин", false)]
    [InlineData("Карьера супер просто", false)]
    [InlineData("перчатки", false)]
    [InlineData("носки", false)]
    [InlineData("бывший программист", false)]
    [InlineData("Бря... скрряяя па па па па", true)]
    public async Task SearchByQuery(string query, bool isZeroShould)
    {
        SearchDto settings = new SearchDto {
            Query = query
        };
        var res = await _elasticFixture.Persons.Search(settings);
        var isZero = res.Count() == 0;
        isZero.Should().Be(isZeroShould);
    }
    [Theory]
    [InlineData("росский фильм про какое-то гавно", false)]
    public async Task SearchByQueryToRelatedFilms(string query, bool isZeroShould)
    {
        SearchDto settings = new SearchDto {
            Query = query
        };
        var res = await _elasticFixture.Persons.Search(settings);
        var isZero = res.Count() == 0;
        isZero.Should().Be(isZeroShould);
    }
    [Theory]
    [InlineData(new[] {"g6", "g1", "g2"}, false)]
    public async Task SearchByGenresToRelatedFilms(string[] genres, bool isZeroShould)
    {
        SearchDto settings = new SearchDto {
            Genres = genres
        };
        var res = await _elasticFixture.Persons.Search(settings);
        var isZero = res.Count() == 0;
        isZero.Should().Be(isZeroShould);
    }
    [Theory]
    [InlineData(FilmType.SERIAL, false)]
    [InlineData(FilmType.FILM, false)]
    public async Task SearchByKindOfFilmToRelatedFilms(FilmType filmType, bool isZeroShould)
    {
        SearchDto settings = new SearchDto {
            KindOfFilm = filmType
        };
        var res = await _elasticFixture.Persons.Search(settings);
        var isZero = res.Count() == 0;
        isZero.Should().Be(isZeroShould);
    }
    [Theory]
    [InlineData(FilmReleaseType.PREMIERA, false)]
    [InlineData(FilmReleaseType.RELEASE, false)]
    [InlineData(FilmReleaseType.SCREENING, false)]
    public async Task SearchByReleaseTypeToRelatedFilms(FilmReleaseType releaseType, bool isZeroShould)
    {
        SearchDto settings = new SearchDto {
            ReleaseType = releaseType
        };
        var res = await _elasticFixture.Persons.Search(settings);
        var isZero = res.Count() == 0;
        isZero.Should().Be(isZeroShould);
    }
    [Theory]
    [InlineData(6, false)]
    [InlineData(12, false)]
    [InlineData(18, false)]
    public async Task SearchAgeLimitTypeToRelatedFilms(int ageLimit, bool isZeroShould)
    {
        SearchDto settings = new SearchDto {
            AgeLimit = (uint)ageLimit
        };
        var res = await _elasticFixture.Persons.Search(settings);
        var isZero = res.Count() == 0;
        isZero.Should().Be(isZeroShould);
    }
    [Theory]
    [InlineData("2004-12-29", false)]
    [InlineData("1999-12-29", false)]
    [InlineData("1989-03-13", false)]
    [InlineData("2023-01-01", true)]
    public async Task SearchWithDateTimeRangeFrom(string from, bool isZeroShould)
    {
        SearchDto settings = new SearchDto {
            From = DateTime.Parse(from)
        };
        var res = await _elasticFixture.Persons.Search(settings);
        var isZero = res.Count() == 0;
        isZero.Should().Be(isZeroShould);
    }
    [Theory]
    [InlineData("2004-12-29", false)]
    [InlineData("1999-12-29", false)]
    [InlineData("1989-03-13", false)]
    [InlineData("1800-01-01", true)]
    public async Task SearchWithDateTimeRangeTo(string to, bool isZeroShould)
    {
        SearchDto settings = new SearchDto {
            To = DateTime.Parse(to)
        };
        var res = await _elasticFixture.Persons.Search(settings);
        var isZero = res.Count() == 0;
        isZero.Should().Be(isZeroShould);
    }
    [Theory]
    [InlineData(PersonType.ACTOR, false)]
    [InlineData(PersonType.PRODUCER, false)]
    public async Task SearchWithPersonType(PersonType personType, bool isZeroShould)
    {
        SearchDto settings = new SearchDto {
            KindOfPerson = personType
        };
        var res = await _elasticFixture.Persons.Search(settings);
        var isZero = res.Count() == 0;
        isZero.Should().Be(isZeroShould);
    }
    // [Theory]
    // [InlineData(SortBy.DATE, false)]
    // [InlineData(SortBy.POPULARIY, false)]
    // [InlineData(SortBy.RATING, false)]
    // public async Task SearchWithSortType(SortBy sort, bool isZeroShould)
    // {
    //     SearchDto settings = new SearchDto {
    //         Sort = sort
    //     };
    //     var res = await _elasticFixture.Persons.Search(settings);
    //     var isZero = res.Count() == 0;
    //     isZero.Should().Be(isZeroShould);
    // }
}
