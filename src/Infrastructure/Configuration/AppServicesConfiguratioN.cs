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
        services.AddSingleton<SearchRepository<CensorShortDto>, SearchCensorRepositoryImpl<CensorShortDto>>();
        services.AddSingleton<SearchRepository<FilmShortDto>, SearchFilmRepositoryImpl<FilmShortDto>>();
        services.AddSingleton<SearchRepository<PersonShortDto>, SearchPersonRepositoryImpl<PersonShortDto>>();
        services.AddSingleton<SearchRepository<SelectionShortDto>, SelectionRepositoryImpl<SelectionShortDto>>();
        services.AddSingleton<SearchRepository<FilmSelectionDto>, SelectionRepositoryImpl<FilmSelectionDto>>();
        services.AddSingleton<SearchRepository<FilmTrailer>, SearchFilmRepositoryImpl<FilmTrailer>>();
        services.AddSingleton<FilmRepository<FilmShortDto>, ReadFilmRepositoryImpl<FilmShortDto>>();
        services.AddSingleton<SearchRepository<FilmTrailer>, SearchFilmRepositoryImpl<FilmTrailer>>();
        services.AddSingleton<SearchInteractor>();
        services.AddByIdRepositories();
        return services;
    }
    public static IServiceCollection AddByIdRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ByIdRepository<FilmShortDto>, ReadByIdRepoGeneric<FilmShortDto>>(services => {
            var elastic = services.GetRequiredService<IElasticClient>();
            return new ReadByIdRepoGeneric<FilmShortDto>(elastic, "films");
        });
        services.AddSingleton<ByIdRepository<FilmDto>, ReadByIdRepoGeneric<FilmDto>>(services => {
            var elastic = services.GetRequiredService<IElasticClient>();
            return new ReadByIdRepoGeneric<FilmDto>(elastic, "films");
        });
        services.AddSingleton<ByIdRepository<PersonDto>, ReadByIdRepoGeneric<PersonDto>>(services => {
            var elastic = services.GetRequiredService<IElasticClient>();
            return new ReadByIdRepoGeneric<PersonDto>(elastic, "persons");
        });
        services.AddSingleton<ByIdRepository<PersonShortDto>, ReadByIdRepoGeneric<PersonShortDto>>(services => {
            var elastic = services.GetRequiredService<IElasticClient>();
            return new ReadByIdRepoGeneric<PersonShortDto>(elastic, "persons");
        });
        services.AddSingleton<ByIdRepository<CensorDto>, ReadByIdRepoGeneric<CensorDto>>(services => {
            var elastic = services.GetRequiredService<IElasticClient>();
            return new ReadByIdRepoGeneric<CensorDto>(elastic, "censors");
        });
        services.AddSingleton<ByIdRepository<CensorShortDto>, ReadByIdRepoGeneric<CensorShortDto>>(services => {
            var elastic = services.GetRequiredService<IElasticClient>();
            return new ReadByIdRepoGeneric<CensorShortDto>(elastic, "censors");
        });
        services.AddSingleton<ByIdRepository<FilmSelectionDto>, ReadByIdRepoGeneric<FilmSelectionDto>>(services => {
            var elastic = services.GetRequiredService<IElasticClient>();
            return new ReadByIdRepoGeneric<FilmSelectionDto>(elastic, "selections");
        });
        services.AddSingleton<ByIdRepository<SelectionShortDto>, ReadByIdRepoGeneric<SelectionShortDto>>(services => {
            var elastic = services.GetRequiredService<IElasticClient>();
            return new ReadByIdRepoGeneric<SelectionShortDto>(elastic, "selections");
        });
        return services;
    }

    
}