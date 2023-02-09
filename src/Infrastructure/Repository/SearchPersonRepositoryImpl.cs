using System.Linq;
using Core;
using Core.Dtos;
using Core.Dtos.Search;
using Nest;
using Core.Repositories;

namespace Infrastructure.Repositories;

public class SearchPersonRepositoryImpl : RepositoryBase, SearchRepository<PersonDto>
{
    
    public SearchPersonRepositoryImpl(IElasticClient elasticClient) : base(elasticClient, "persons")
    {
        
    }
   

    public async Task<IEnumerable<PersonDto>> Search(SearchDto settings)
    {
        var mustDesc = await MustDescriptor(settings);
        var persons = await _elasticClient.SearchAsync<PersonDto>(s => s
            .Index(index)
            .Take((int)settings.Take)
            .Skip((int)(settings.Take * (settings.Page - 1)))
            .Query(q => q
                .Bool(b => b
                    .Must(mustDesc)
                )    
            )
        );

        if(persons.Hits.Count == 0)
            return Enumerable.Empty<PersonDto>();

        
        var res = persons.Hits.Select(s => {
            var source = s.Source;
            source.Id = s.Id;
            return source;
        });
        
        return await SortPersons(settings, res);
    }
    async Task<IEnumerable<PersonDto>> SortPersons(SearchDto settings, IEnumerable<PersonDto> toSearch)
    {
        if(settings.Sort is null)
            return toSearch;

        if(settings.Sort == SortBy.DATE)
            return toSearch.OrderByDescending(p => p.Birthday);

        if(settings.Sort == SortBy.POPULARIY){
            return await toSearch.ToAsyncEnumerable().OrderByDescendingAwait(async p => 
            {
                var films = await PersonFilms(p);
                var avgWatched = films.Average(a => a.WatchedCount);
                return avgWatched;
            }
            ).ToListAsync();
        }
        else
            return await toSearch.ToAsyncEnumerable().OrderByDescendingAwait(async p => 
            {
                var films = await PersonFilms(p);
                var avgScored = films.Average(a => (a.Score * a.ScoreCount + 1) / (++a.ScoreCount));
                return avgScored;
            }
            ).ToListAsync();
    }

    async Task<IEnumerable<FilmDto>> PersonFilms(PersonDto person)
    {
        var films = await _elasticClient.SearchAsync<FilmDto>(s => s
                .Index("films")
                .Query(q => q
                    .Ids(s => s
                        .Values(person.Films)
                )
            )
        );
        return films.Hits.Count == 0 
            ? Enumerable.Empty<FilmDto>() 
            : films.Hits.Select(s => s.Source);
    }

    async Task<IEnumerable<Func<QueryContainerDescriptor<PersonDto>, QueryContainer>>> MustDescriptor(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<PersonDto>, QueryContainer>>();
        var shouldDescr = await ShouldDescriptorInMust(settings);

        qResult.Add(m => m
                    .Bool(b => b.Should(shouldDescr))
                );
        
        if(settings.KindOfPerson is not null)
            qResult.Add(s => s.Term(t => t.KindOfPerson, settings.KindOfPerson));

        if(settings.From is not null)
            qResult.Add(s => s.DateRange(d => d.Field(f => f.Birthday).GreaterThanOrEquals(settings.From)));
        
        if(settings.To is not null)
            qResult.Add(s => s.DateRange(d => d.Field(f => f.Birthday).LessThanOrEquals(settings.To)));
        return qResult;
    }
    async Task <IEnumerable<Func<QueryContainerDescriptor<PersonDto>, QueryContainer>>> ShouldDescriptorInMust(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<PersonDto>, QueryContainer>>();

        if(settings.Query is not null)
            qResult.Add(s => s
                        .MultiMatch(match => match
                            .Query(settings.Query)
                            .Fields(fs => fs
                                .Field(f => f.Name, 3)
                                .Field(f => f.Nominations, 2)
                                .Field(f => f.Career, 1)
                            )
                            .MinimumShouldMatch(1)
                        )
                    );
        
        var filmIds = await SearchRelatedFilms(settings);


        if(filmIds.Count() > 0)
            qResult.Add(s => s.Terms(t => t
                .Terms(filmIds)
                .Field(f => f.Films)
            )
        );

        return qResult;
    }
    
    async Task <IEnumerable<string>> SearchRelatedFilms(SearchDto settings)
    {
        var res = await _elasticClient.SearchAsync<FilmDto>(s => s
            .Index("films")
            .Query(q => q
                .Bool(b => b
                    .Must(MustDescriptorForFilm(settings))
                )
            )
        );

        if(res.Hits.Count == 0)
            return Enumerable.Empty<string>();
        
        return res.Hits.Select(s => s.Id);
    }
    IEnumerable<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>> MustDescriptorForFilm(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<FilmDto>, QueryContainer>>();
        
        if(settings.Query is not null)
            qResult.Add(s => s
                .MultiMatch(m => m
                    .Query(settings.Query)
                    .Fields(fs => fs
                        .Field(f => f.Name)
                        .Field(f => f.Description)
                    )
                )
            );
        if(settings.Genres is not null)
                qResult.Add(s => s.Terms(t => t
                    .Field(f => f.Genres)
                    .Terms(settings.Genres)
                )
            );
        
        if(settings.KindOfFilm is not null)
                qResult.Add(s => s.Term(t => t.KindOfFilm,settings.KindOfFilm));

        if(settings.ReleaseType is not null)
                qResult.Add(s => s.Term(t => t.ReleaseType, settings.ReleaseType));

        if(settings.AgeLimit is not null)
            qResult.Add(s => s
                .Range(r => r
                    .Field(f => f.AgeLimit)
                    .LessThanOrEquals(settings.AgeLimit)
                )
            );
        return qResult;
    }
}
