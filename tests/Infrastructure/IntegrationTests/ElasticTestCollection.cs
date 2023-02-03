using Xunit;

namespace IntegrationTests;

[CollectionDefinition("Elastic")]
public class ElasticTestCollection : ICollectionFixture<ElasticFixture>
{

}