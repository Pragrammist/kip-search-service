using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests;

[Collection("Elastic")]
public class SearchInteractorIntegrationTest
{
    readonly ElasticFixture _elasticFixture;
    public SearchInteractorIntegrationTest(ElasticFixture elasticFixture)
    {
        _elasticFixture = elasticFixture;
        
    }
    
}