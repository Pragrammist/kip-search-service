using Core;
using Core.Repositories;
using static Infrastructure.Repositories.FilmFieldHelpers;
using static Infrastructure.Repositories.PersonFieldHelpers;
using static Infrastructure.Repositories.CensorFieldHelpers;
using Core.Dtos.Search;
using Nest;

namespace Infrastructure.Repositories;

public class SearchCensorRepositoryImpl<TCensorType> : RepositoryBase, SearchRepository<TCensorType> where TCensorType : class, IDable
{
    
    public SearchCensorRepositoryImpl(IElasticClient elasticClient) : base(elasticClient, "censors") 
    {
        
    }
    public async Task<IEnumerable<TCensorType>> Search(SearchDto settings)
    {
        var shouldDesc = await ShouldDesc(settings);
        var res = await _elasticClient.SearchAsync<TCensorType>(s => s
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

        return res.SelectHitsWithId();
    }
    
    async Task <IEnumerable<Func<QueryContainerDescriptor<TCensorType>, QueryContainer>>> ShouldDesc(SearchDto settings)
    {
        var qResult = new List<Func<QueryContainerDescriptor<TCensorType>, QueryContainer>>();
        if(settings.Query is not null)
            qResult.Add(q => q
                .Match(m => m
                    .Field(CensorNameField())
                        .Query(settings.Query)
                )
            );
        
        var films = await _elasticClient.SearchRelatedFilms(settings);

        if(films.Count() > 0)
            qResult.Add(q => q
                .Terms(t => t.Terms(films).Field(CensorFilmsField()))
            );


        var filmsFromPersons = await _elasticClient.RelatedPersons(settings);

        if(filmsFromPersons.Count() > 0)
            qResult.Add(q => q
                .Terms(t => t.Terms(filmsFromPersons).Field(CensorFilmsField()))
            );
        return qResult;
    }
    
    

}
