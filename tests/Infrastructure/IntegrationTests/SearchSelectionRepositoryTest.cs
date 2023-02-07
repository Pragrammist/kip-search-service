using Xunit;
using System.Linq;
using Core.Dtos.Search;
using System.Threading.Tasks;
using FluentAssertions;
using Core.Dtos;
using Core;

namespace IntegrationTests;

[Collection("Elastic")]
public class SearchSelectionRepositoryTest
{
    readonly ElasticFixture _elasticFixture;
    public SearchSelectionRepositoryTest(ElasticFixture elasticFixture)
    {
        _elasticFixture = elasticFixture;
    }
    [Theory]
    [InlineData("лучши фильмы по версии кого-то другого", false)]
    [InlineData("Супер герои", false)]
    [InlineData("Букин Алексей Петрович", false)]
    [InlineData("Бря... скрряяя па па па па", true)]
    public async Task SearchByQuery(string query, bool isZeroShould)
    {
        SearchDto settings = new SearchDto {
            Query = query
        };
        var res = await _elasticFixture.Selections.Search(settings);
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
        var res = await _elasticFixture.Selections.Search(settings);
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
        var res = await _elasticFixture.Selections.Search(settings);
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
        var res = await _elasticFixture.Selections.Search(settings);
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
        var res = await _elasticFixture.Selections.Search(settings);
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
        var res = await _elasticFixture.Selections.Search(settings);
        var isZero = res.Count() == 0;
        isZero.Should().Be(isZeroShould);
    }
    [Theory]
    [InlineData(SortBy.DATE, false)]
    [InlineData(SortBy.POPULARIY, false)]
    [InlineData(SortBy.RATING, false)]
    public async Task SearchWithSortType(SortBy sort, bool isZeroShould)
    {
        SearchDto settings = new SearchDto {
            Sort = sort
        };
        var res = await _elasticFixture.Selections.Search(settings);
        var isZero = res.Count() == 0;
        isZero.Should().Be(isZeroShould);
    }
}
