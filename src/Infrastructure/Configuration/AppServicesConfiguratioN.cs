using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Repositories;
using Core.Repositories;
using Core.Interactors;
using Core.Dtos;
using Nest;

namespace Infrastructure.Configuration;


public static class AppServicesConfiguration
{
    public static IServiceCollection AddSearchers(this IServiceCollection services)
    {
        services.AddTransient<SearchRepository<CensorDto>, SearchCensorRepositoryImpl>();
        services.AddTransient<SearchRepository<ShortFilmDto>, SearchFilmRepositoryImpl<ShortFilmDto>>();
        services.AddTransient<SearchRepository<PersonDto>, SearchPersonRepositoryImpl>();
        services.AddTransient<SearchRepository<FilmSelectionDto>, SelectionRepositoryImpl>();
        services.AddTransient<ByIdRepository<ShortFilmDto>, ReadByIdRepoGeneric<ShortFilmDto>>(services => {
            var elastic = services.GetRequiredService<IElasticClient>();
            return new ReadByIdRepoGeneric<ShortFilmDto>(elastic, "films");
        });
        services.AddTransient<SearchRepository<FilmTrailer>, SearchFilmRepositoryImpl<FilmTrailer>>();
        services.AddTransient<FilmRepository<ShortFilmDto>, ReadFilmRepositoryImpl>();
        services.AddTransient<GetManyRepository<FilmTrailer>, ReadFilmRepositoryImpl>();
        services.AddTransient<SearchInteractor>();
        return services;
    }

    
}