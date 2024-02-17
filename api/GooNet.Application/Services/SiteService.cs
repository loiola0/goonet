using AutoMapper;
using GooNet.Application.DTOs.Requests;
using GooNet.Application.DTOs.Responses;
using GooNet.Application.Interfaces;
using GooNet.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Nest;

namespace GooNet.Application.Services;

public class SiteService : ISiteService
{
    private readonly IElasticClient _elasticClient;

    private readonly IMapper _mapper;

    private readonly IConfiguration _configuration;

    public SiteService(IElasticClient elasticClient, IMapper mapper, IConfiguration configuration)
    {
        _elasticClient = elasticClient;

        _mapper = mapper;

        _configuration = configuration;
    }

    public async Task<List<SiteResponse>> SearchPagenedBy(SearchSitePagenedRequest request)
    {
        ValidateSearchRequest(request);

        var indexSite = _configuration["IndexSites"];

        var result = await _elasticClient.SearchAsync<Site>(s =>
            s.Index(indexSite)
                .Size(request.TotalItems)
                .From((request.Page - 1) * request.TotalItems)
                .Query(q =>
                    q.Bool(b =>
                        b.Should(
                            // Procura por termos que correspondam parcialmente ao título
                            should => should.Match(m => m.Field(f => f.Title)
                                .Query(request.Title).Fuzziness(Fuzziness.Auto)),
                            // Procura por frases exatas no título
                            should => should.MatchPhrase(m => m.Field(f => f.Title)
                                .Query(request.Title))
                        )
                    )
                )
        );


        if (!result.IsValid)
            throw new Exception("Error during search");

        var sites = _mapper.Map<List<SiteResponse>>(result.Documents);

        return sites;
    }

    private static void ValidateSearchRequest(SearchSitePagenedRequest request)
    {
        if (string.IsNullOrEmpty(request.Title))
            throw new Exception("Title is null or empty");

        if (request.Page < 0)
            throw new Exception("The number of pages is negative");

        if (request.TotalItems < 0)
            throw new Exception("The total items is negative");
    }
}