using Core;
using Core.Dtos;
using Core.Dtos.Search;
using Nest;
using Core.Repositories;
using static Infrastructure.Repositories.PersonFieldHelpers;
using static Infrastructure.Repositories.FilmFieldHelpers;


namespace Infrastructure.Repositories;

public class SearchPersonRepositoryImpl<TPerson> : RepositoryBase, SearchRepository<TPerson> where TPerson : class, Idable
{
    
    public SearchPersonRepositoryImpl(IElasticClient elasticClient) : base(elasticClient, "persons")
    {
        
    }

    public async Task<IEnumerable<TPerson>> Search(SearchDto settings)
    {
        var mustDesc = await MustDescriptor(settings);
        var persons = await _elasticClient.SearchAsync<TPerson>(s => s
            .Index(index)
            .Sort(s => SortDescriptor(s, settings))
            .Take((int)settings.Take)
            .Skip((int)(settings.Take * (settings.Page - 1)))
            .Query(q => q
                .Bool(b => b
                    .Must(mustDesc)
                )    
            )
        );

        if(persons.Hits.Count == 0)
            return Enumerable.Empty<TPerson>();

        
        var res = persons.Hits.Select(s => {
            var source = s.Source;
            source.Id = s.Id;
            return source;
        });
        
        return res;
    }
    
    private IPromise<IList<ISort>> SortDescriptor(SortDescriptor<TPerson> selector, SearchDto settings)
    {
        if(settings.Sort == SortBy.DATE)
            selector.Descending(BirdayField());

        // if(settings.Sort == SortBy.POPULARIY)
        //     selector.Descending(WatchedCountField());
        // else if (settings.Sort == SortBy.RATING)
        //     selector.Script(RatingCalculator);

            return selector;
    }
    
    async Task<IEnumerable<Func<QueryContainerDescriptor<TPerson>, QueryContainer>>> MustDescriptor(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<TPerson>, QueryContainer>>();
        var shouldDescr = await ShouldDescriptorInMust(settings);

        qResult.Add(m => m
                    .Bool(b => b.Should(shouldDescr))
                );
        
        if(settings.KindOfPerson is not null)
            qResult.Add(s => s.Term(KindOfPersonField(), settings.KindOfPerson));

        if(settings.From is not null)
            qResult.Add(s => s.DateRange(d => d.Field(BirdayField()).GreaterThanOrEquals(settings.From)));
        
        if(settings.To is not null)
            qResult.Add(s => s.DateRange(d => d.Field(BirdayField()).LessThanOrEquals(settings.To)));
        return qResult;
    }
    
    async Task <IEnumerable<Func<QueryContainerDescriptor<TPerson>, QueryContainer>>> ShouldDescriptorInMust(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<TPerson>, QueryContainer>>();

        if(settings.Query is not null)
            qResult.Add(s => s
                        .MultiMatch(match => match
                            .Query(settings.Query)
                            .Fields(fs => fs
                                .Field(PersonNameField(3))
                                .Field(PersonNominationsField(2))
                                .Field(CareerField(1))
                            )
                            .MinimumShouldMatch(1)
                        )
                    );
        
        var filmIds = await SearchRelatedFilms(settings);


        if(filmIds.Count() > 0)
            qResult.Add(s => s.Terms(t => t
                .Terms(filmIds)
                .Field(PersonFilmsField())
            )
        );

        return qResult;
    }
    
    async Task <IEnumerable<string>> SearchRelatedFilms(SearchDto settings)
    {
        var res = await _elasticClient.SearchAsync<FilmSearchModel>(s => s
            .Index("films")
            .Query(q => q
                .Bool(b => b
                    .Must(MustDescriptorForFilm<FilmSearchModel>(settings))
                )
            )
        );

        if(res.Hits.Count == 0)
            return Enumerable.Empty<string>();
        
        return res.Hits.Select(s => s.Id);
    }
    
    IEnumerable<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>> MustDescriptorForFilm<TFilmType>(SearchDto settings) where TFilmType : class
    {
        var qResult = new List<Func<QueryContainerDescriptor<TFilmType>, QueryContainer>>();
        
        if(settings.Query is not null)
            qResult.Add(s => s
                .MultiMatch(m => m
                    .Query(settings.Query)
                    .Fields(fs => fs
                        .Field(FilmNameField())
                        .Field(DescriptionField())
                    )
                )
            );
        if(settings.Genres is not null)
                qResult.Add(s => s.Terms(t => t
                    .Field(GenresField())
                    .Terms(settings.Genres)
                )
            );
        
        if(settings.KindOfFilm is not null)
                qResult.Add(s => s.Term(KindOfFilmField(),settings.KindOfFilm));

        if(settings.ReleaseType is not null)
                qResult.Add(s => s.Term(ReleaseTypeField(), settings.ReleaseType));

        if(settings.AgeLimit is not null)
            qResult.Add(s => s
                .Range(r => r
                    .Field(AgeLimitField())
                    .LessThanOrEquals(settings.AgeLimit)
                )
            );
        return qResult;
    }
    
}