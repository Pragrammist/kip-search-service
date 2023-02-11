using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;


namespace IntegrationTest;

public class WebFixture : IDisposable
{
    readonly WebApplicationFactory<Program> _factory;
    public HttpClient Client { get; }
    public WebFixture()
    {
        _factory = new WebApplicationFactory<Program>();
        Client = _factory.CreateClient();
    }

    // class ElasticDataFiller
    // {
    //     class IdObj
    //     {
    //         public string Id { get; set; } = null!;
    //     }
    //     readonly WebApplicationFactory<Program> _factory;
    //     public ElasticDataFiller(WebApplicationFactory<Program> factory)
    //     {
    //         _factory = factory;
    //     }
    //     public void FillData()
    //     {
    //         var elkClient = _factory.Services.GetRequiredService<IElasticClient>();
    //         elkClient.Index<IdObj>(new IdObj {Id = "personTest"}, s => s.Index("persons"));
    //         elkClient.Index<IdObj>(new IdObj {Id = "filmTest"}, s => s.Index("films"));
    //         elkClient.Index<IdObj>(new IdObj {Id = "censorTest"}, s => s.Index("censors"));
    //         elkClient.Index<IdObj>(new IdObj {Id = "selectionTest"}, s => s.Index("selections"));
    //     }
    // }


    public void Dispose()
    {
        Client.Dispose();
        _factory.Dispose();
    }
}
