namespace GooNet.Application.DTOs.Requests;

public class SearchSitePagenedRequest
{
    public string Title { get; set; } = string.Empty;

    public int TotalItems { get; set; }
    
    public int Page { get; set; }
}