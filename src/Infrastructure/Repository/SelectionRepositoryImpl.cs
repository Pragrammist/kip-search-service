using Nest;
using Core;
using Core.Repositories;
using Core.Dtos;
using Core.Dtos.Search;
using Mapster;
using Elasticsearch;
using static Infrastructure.Repositories.SelectionFieldHelpers;
using static Infrastructure.Repositories.PersonFieldHelpers;
using static Infrastructure.Repositories.FilmFieldHelpers;

namespace Infrastructure.Repositories;
public class SelectionRepositoryImpl<TSelectionType> : RepositoryBase, SearchRepository<TSelectionType> where TSelectionType : class, Idable
{
    
    public SelectionRepositoryImpl(IElasticClient elasticClient) : base(elasticClient, "selections")
    {

    }
    public async Task<IEnumerable<TSelectionType>> Search(SearchDto settings)
    {
        var shouldDesc = await ShouldDesc(settings);
        var res = await _elasticClient.SearchAsync<TSelectionType>(s => s
            .Index(index)
            .Take((int)settings.Take)
            .Skip((int)(settings.Take * (settings.Page - 1)))
            .Query(q => q
                .Bool(b => b
                    .Should(shouldDesc)
                    .MinimumShouldMatch(1)
                )
            )
        );
        if(res.Hits.Count == 0)
            return Enumerable.Empty<TSelectionType>();
        

        var toSort = res.Hits.Select(s => {
            var source = s.Source;
            source.Id = s.Id;
            return source;
        });

        return toSort;
    }
    
    async Task <IEnumerable<Func<QueryContainerDescriptor<TSelectionType>, QueryContainer>>> ShouldDesc(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<TSelectionType>, QueryContainer>>();
        if(settings.Query is not null)
            qResult.Add(q => q
                .Match(m => m
                    .Field(SelectionNameField())
                        .Query(settings.Query)
                )
            );
        
        var films = await RelatedFilms(settings);

        if(films.Count() > 0)
            qResult.Add(q => q
                .Terms(t => t.Terms(films).Field(SelectionFilmsField()))
            );


        var filmsFromPersons = await RelatedPersonsFilms(settings);

        if(filmsFromPersons.Count() > 0)
            qResult.Add(q => q
                .Terms(t => t.Terms(filmsFromPersons).Field(SelectionFilmsField()))
            );
        return qResult;
    }
    
    async Task<IEnumerable<string>> RelatedFilms(SearchDto settings)
    {
        var res = await _elasticClient.SearchAsync<FilmSearchModel>(s => s
            .Index("films")
                .Query(q => q
                    .Bool(b => b
                        .Must(MustFilmDesc<FilmSearchModel>(settings))
                    )
                )
            );
        return res.Hits.Count == 0 
        ? Enumerable.Empty<string>()
        : res.Hits.Select(h => h.Id);
    }
    
    async Task<IEnumerable<string>> RelatedPersonsFilms(SearchDto settings)
    {
        var res = await _elasticClient.SearchAsync<PersonSearchModel>(s => s
            .Index("persons")
                .Query(q => q
                    .Bool(b => b
                        .Must(MustPersonDesc<PersonSearchModel>(settings))
                    )
                )
            );
        return res.Hits.Count == 0 
        ? Enumerable.Empty<string>()
        : res.Hits.Select(h => h.Source.Films)

        .Aggregate((s,s2) => {
            var arrInList = s.ToList();
            arrInList.AddRange(s2);
            return arrInList.ToArray();
        }).Distinct();
    }
    
    IEnumerable<Func<QueryContainerDescriptor<TPersonModel>, QueryContainer>> MustPersonDesc<TPersonModel>(SearchDto settings) where TPersonModel : class
    {
        var qResult = new List<Func<QueryContainerDescriptor<TPersonModel>, QueryContainer>>();
        if(settings.Query is not null)
            qResult.Add(q => q
                .Match(m => m
                    .Query(settings.Query)
                    .Field(PersonNameField())
                )
            );
        if(settings.KindOfPerson is not null)
            qResult.Add(q => q
                .Term(KindOfPersonField(), settings.KindOfPerson));
        return qResult;
    }
    
    IEnumerable<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> MustFilmDesc<TFilmSearchModel>(SearchDto settings) where TFilmSearchModel : class
    {
        var qResult = new List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>>();

        if(settings.Query is not null)
            qResult.Add(q => q
                .Match(m => m
                    .Field(FilmNameField())
                        .Query(settings.Query)
                )
            );
        
        if(settings.Genres is not null)
            qResult.Add(q => q
                .Terms(t => t
                    .Field(GenresField())
                        .Terms(settings.Genres)
                )
            );
        if(settings.Countries is not null)
            qResult.Add(q => q
                .Bool(b => b
                    .Should(ShouldCountriesDesc<TFilmSearchModel>(settings))
                )
            );
        if(settings.KindOfFilm is not null)
            qResult.Add(q => q.Term(KindOfFilmField(), settings.KindOfFilm));

        if(settings.ReleaseType is not null)
            qResult.Add(q => q.Term(ReleaseTypeField(), settings.ReleaseType));

        if(settings.AgeLimit is not null)
            qResult.Add(q => q
                .Range(r => r
                    .Field(AgeLimitField())
                    .LessThanOrEquals(settings.AgeLimit)
                )
            );
        return qResult;
    }
    
    IEnumerable<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>> ShouldCountriesDesc<TFilmSearchModel>(SearchDto settings) where TFilmSearchModel : class
    {
        var qResult = new List<Func<QueryContainerDescriptor<TFilmSearchModel>, QueryContainer>>();
        if(settings.Countries is null)
            return qResult;
        foreach(var count in settings.Countries)
        {
            qResult.Add(q => q
                .Match(m => m
                    .Query(count)
                    .Field(FilmCountryField())
                )
            );
        }
        return qResult;
    }
}