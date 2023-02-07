using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Repositories;
using Core.Repositories;
using Core.Dtos;

namespace Infrastructure.Configuration;


public static class AppServicesConfiguration
{
    public static IServiceCollection AddSearchers(this IServiceCollection services)
    {
        services.AddScoped<SearchRepository<CensorDto>, SearchCensorRepositoryImpl>();
        services.AddScoped<SearchRepository<FilmDto>, SearchFilmRepositoryImpl>();
        services.AddScoped<SearchRepository<PersonDto>, SearchPersonRepositoryImpl>();
        services.AddScoped<SearchRepository<FilmSelectionDto>, SelectionRepositoryImpl>();
        return services;
    }

    
}