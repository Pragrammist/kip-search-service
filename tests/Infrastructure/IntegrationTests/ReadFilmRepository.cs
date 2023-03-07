using Xunit;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;

namespace IntegrationTests;

[Collection("Elastic")]
public class ReadFilmRepository
{
    readonly ElasticFixture _elasticFixture;
    public ReadFilmRepository(ElasticFixture elasticFixture)
    {
        _elasticFixture = elasticFixture;
    }


    [Fact]
    public async Task GetGenres()
    {
        var res = await _elasticFixture.FilmRepo.GetGenres();
        var isZero = res.Count() == 0;
        isZero.Should().Be(false);
    }
}
