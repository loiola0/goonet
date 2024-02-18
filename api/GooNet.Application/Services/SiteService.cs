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

        var indexSite = _configuration["INDEX_SITES"];

        var result = await _elasticClient.SearchAsync<Site>(s => s
            .Index(indexSite)
            .Size(request.TotalItems) // Total de items a serem retornados.
            .From((request.Page) * request.TotalItems) // Página a ser buscada.
            .Query(q => q
                .Match(m => m // É usado para pesquisar por uma ou mais palavras em um campo de texto.
                    .Field(f => f.Title)
                    .Query(request.Title)
                    .Fuzziness(Fuzziness.Auto) // Tolerância a erros de digitação
                    .PrefixLength(1) // Prefixo mínimo -  por exemplo, se fosse 3, a palavra "elephant" será tratada como "ele" para fins de pesquisa.
                    .MaxExpansions(10) // Número máximo de expansões. Controlar o número de variações de um termo que são consideradas na pesquisa
                    .Operator(Operator.Or) // Apenas um dos termos de busca precisa estar presente no campo para que um documento seja considerado correspondente.
                )
            )
            .Sort(sort => sort
                .Descending("_score")
            )
        );

        if (!result.IsValid)
            throw new Exception("Error during search");

        var sitesMapped = _mapper.Map<List<SiteResponse>>(result.Documents);

        return sitesMapped;
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