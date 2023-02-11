using Xunit;

namespace IntegrationTest;

[CollectionDefinition("WebContext")]
public class WebTestCollection : ICollectionFixture<WebFixture>
{

}