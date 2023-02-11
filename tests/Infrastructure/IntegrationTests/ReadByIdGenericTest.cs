using Xunit;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;

namespace IntegrationTests;

[Collection("Elastic")]
public class ReadByIdGenericTest
{
    readonly ElasticFixture _elasticFixture;
    public ReadByIdGenericTest(ElasticFixture elasticFixture)
    {
        _elasticFixture = elasticFixture;
    }

    [Fact]
    public async Task GetByIds()
    {
        var res = await _elasticFixture.ByIdFilmRepository.GetByIds("f1", "f2");
        var isZero = res.Count() == 0;
        isZero.Should().Be(false);
    }
}
