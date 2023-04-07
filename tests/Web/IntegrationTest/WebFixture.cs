using System;
using System.Net.Http;
using Core.Dtos;
using Infrastructure.Configuration;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace IntegrationTest;

public class WebFixture : IDisposable
{
    readonly WebApplicationFactory<Program> _factory;
    public HttpClient Client { get; }
    public WebFixture()
    {
        _factory = new WebApplicationFactory<Program>();
        Client = _factory.CreateClient();
        var elkClient = _factory.Services.GetRequiredService<IElasticClient>();
        DataFiller filler = new DataFiller(elkClient);
        filler.FillFilmsData();
    }


    public void Dispose()
    {
        _factory.Dispose();
    }
}
