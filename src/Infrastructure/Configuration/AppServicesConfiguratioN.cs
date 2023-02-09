using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Repositories;
using Core.Repositories;
using Core.Interactors;
using Core.Dtos;

namespace Infrastructure.Configuration;


public static class AppServicesConfiguration
{
    public static IServiceCollection AddSearchers(this IServiceCollection services)
    {
        services.AddTransient<SearchRepository<CensorDto>, SearchCensorRepositoryImpl>();
        services.AddTransient<SearchRepository<FilmDto>, SearchFilmRepositoryImpl>();
        services.AddTransient<SearchRepository<PersonDto>, SearchPersonRepositoryImpl>();
        services.AddTransient<SearchRepository<FilmSelectionDto>, SelectionRepositoryImpl>();
        services.AddTransient<ByIdRepository<ShortFilmDto>, ReadFilmRepositoryImpl>();
        services.AddTransient<FilmRepository<ShortFilmDto>, ReadFilmRepositoryImpl>();
        services.AddTransient<GetManyRepository<FilmTrailer>, ReadFilmRepositoryImpl>();
        services.AddTransient<SearchInteractor>();
        return services;
    }

    
}