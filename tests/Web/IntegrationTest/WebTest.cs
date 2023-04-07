using System;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Xunit;

namespace IntegrationTest;

[CollectionDefinition("WebContext")]
public class WebTest : IClassFixture<WebFixture>
{
    WebFixture Web { get; }
    public WebTest(WebFixture web)
    {
        Web = web;
    } 
    [Theory]
    [InlineData("/films", "/f1")]
    [InlineData("/persons", "/p1")]
    [InlineData("/censors", "/c1")]
    [InlineData("/selections", "/s1")]
    [InlineData("/media")]
    [InlineData("/searchcontent")]
    public async Task GetActions(string url, string id = "")
    {
        var finalUrl = url + id;

        var response = await Web.Client.GetAsync(finalUrl);

        response.EnsureSuccessStatusCode();
    }
    [Theory]
    [InlineData("/films")]
    [InlineData("/persons")]
    [InlineData("/censors")]
    [InlineData("/selections")]
    public async Task PostActions(string url)
    {
        var finalUrl = url;

        var response = await Web.Client.PostAsJsonAsync<Object>(finalUrl, new {});

        response.EnsureSuccessStatusCode();
    }
}
