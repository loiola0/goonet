using GooNet.Application.DTOs.Requests;
using GooNet.Application.DTOs.Responses;

namespace GooNet.Application.Interfaces;

public interface ISiteService
{
    public Task<List<SiteResponse>> SearchPagenedBy(SearchSitePagenedRequest request);
}