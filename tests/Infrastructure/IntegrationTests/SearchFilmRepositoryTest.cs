using System;
using Xunit;
using System.Linq;
using Core.Dtos.Search;
using System.Threading.Tasks;
using FluentAssertions;
using System.Collections.Generic;
using Core.Dtos;

namespace IntegrationTests;

[Collection("Elastic")]
public class SearchFilmRepositoryTest
{
    readonly ElasticFixture _elasticFixture;
    public SearchFilmRepositoryTest(ElasticFixture elasticFixture)
    {
        _elasticFixture = elasticFixture;
        
    }
    [Theory]
    [InlineData("Супер герои", false)]
    [InlineData("Бря... скрряяя па па па па", true)]
    public async Task SearchQuery(string query, bool isCountZero)
    {
        SearchDto inpSearch = new SearchDto{
            Query = query,
        };
        var res = await _elasticFixture.Films.Search(inpSearch);
        var isCountZeroRes = res.Count() == 0;
        isCountZeroRes.Should().Be(isCountZero);
        if(!isCountZeroRes)
            res.First().Name.Should().NotBeNull();
    }
    [Theory]
    [InlineData(1, false)]
    [InlineData(5, true)]
    public async Task SearchPage(int page, bool isCountZero)
    {
        SearchDto inpSearch = new SearchDto{
            Page = (uint)page
        };

        var res = await _elasticFixture.Films.Search(inpSearch);

        var isCountZeroRes = res.Count() == 0;
        isCountZeroRes.Should().Be(isCountZero);
    }

    [Theory]
    [InlineData(12,0)]
    public async Task SearchAgeLimit(int limit, int limit2)
    {
        SearchDto inpSearch = new SearchDto{
            AgeLimit = (uint)limit
        };

        SearchDto inpSearch2 = new SearchDto{
            AgeLimit = (uint)limit2
        };
        var res = await _elasticFixture.Films.Search(inpSearch);
        var res2 = await _elasticFixture.Films.Search(inpSearch2);

        res.Count().Should().BeGreaterThan(0);
        res.First().Name.Should().NotBeNull();

        res2.Count().Should().Be(0);
    }

    [Theory]
    [MemberData(nameof(SearchDateTimeRangeData))]
    public async Task SearchDateTimeRange(DateTime from, DateTime to, bool isCountZero)
    {
        SearchDto inpSearch = new SearchDto{
            From = from,
            To = to
        };
        
        var res = await _elasticFixture.Films.Search(inpSearch);
        
        var isCountZeroRes = res.Count() == 0;
        isCountZeroRes.Should().Be(isCountZero);
    }
    static IEnumerable<object[]> SearchDateTimeRangeData()
    {
        yield return new object[] { new DateTime(2011, 01, 01), new DateTime(2020, 01, 01), false};
        yield return new object[] { DateTime.Now, DateTime.MaxValue , true};
    }
    
    [Theory]
    [InlineData(FilmType.FILM, false)]
    [InlineData(FilmType.SERIAL, false)]
    public async Task SearchKindOfFilm(FilmType filmType, bool isCountZero)
    {
        SearchDto inpSearch = new SearchDto{
            KindOfFilm = filmType
        };
        
        var res = await _elasticFixture.Films.Search(inpSearch);
        
        var isCountZeroRes = res.Count() == 0;
        isCountZeroRes.Should().Be(isCountZero);
    }

    [Theory]
    [InlineData(FilmReleaseType.PREMIERA, false)]
    [InlineData(FilmReleaseType.RELEASE, false)]
    [InlineData(FilmReleaseType.SCREENING, false)]
    public async Task SearchReleaseType(FilmReleaseType releaseType, bool isCountZero)
    {
        SearchDto inpSearch = new SearchDto{
            ReleaseType = releaseType
        };
        
        var res = await _elasticFixture.Films.Search(inpSearch);
        
        var isCountZeroRes = res.Count() == 0;
        isCountZeroRes.Should().Be(isCountZero);
    }
    [Theory]
    [InlineData(new string[] {"g1", "g2"}, false)]
    [InlineData(new string[] {"g3", "g6"}, false)]
    [InlineData(new string[] {"g1", "g2", "g3", "g4", "g5", "g6"}, false)]
    [InlineData(new string[] {"этого жаннра нет"}, true)]
    public async Task SearchGenres(string[] genres, bool isCountZero)
    {
        SearchDto inpSearch = new SearchDto{
            Genres = genres
        };
        
        var res = await _elasticFixture.Films.Search(inpSearch);
        
        var isCountZeroRes = res.Count() == 0;
        isCountZeroRes.Should().Be(isCountZero);
    }

    [Theory]
    [InlineData(new string[] {"Vegas", "Vegas 3", "Mexico", "Nano", "3", "nan"}, false)]
    [InlineData(new string[] {"Vegas", "Vegas 3"}, false)]
    [InlineData(new string[] {"Vegas 3", "этой страны нет"}, false)]
    [InlineData(new string[] {"nano"}, false)]
    [InlineData(new string[] {"этой страны нет"}, true)]
    public async Task SearchCountries(string[] countries, bool isCountZero)
    {
        SearchDto inpSearch = new SearchDto{
            Countries = countries
        };
        
        var res = await _elasticFixture.Films.Search(inpSearch);
        
        var isCountZeroRes = res.Count() == 0;
        isCountZeroRes.Should().Be(isCountZero);
    }


    [Theory]
    [InlineData("Букин Алексей Петрович", false)]
    public async Task SearchPersons(string query, bool isCountZero)
    {
        SearchDto inpSearch = new SearchDto{
            Query = query
        };
        
        var res = await _elasticFixture.Films.Search(inpSearch);

        var isCountZeroRes = res.Count() == 0;
        isCountZeroRes.Should().Be(isCountZero);
    }

    [Theory]
    [InlineData("Имененко Имя Имьянич", true)]
    public async Task SearchPersonsWithPersonKind(string query, bool isCountZero)
    {
        SearchDto inpSearch = new SearchDto{
            Query = query,
            KindOfPerson = PersonType.PRODUCER
        };
        
        var res = await _elasticFixture.Films.Search(inpSearch);

        var isCountZeroRes = res.Count() == 0;
        isCountZeroRes.Should().Be(isCountZero);
    }

    [Theory]
    [InlineData("ГРАФИКА ВЕКА", false)]
    [InlineData("Логика года", false)]
    public async Task SearchByNomination(string query, bool isCountZero)
    {
        SearchDto inpSearch = new SearchDto{
            Query = query,
        };
        
        var res = await _elasticFixture.Films.Search(inpSearch);

        var isCountZeroRes = res.Count() == 0;
        isCountZeroRes.Should().Be(isCountZero);
    }
}
