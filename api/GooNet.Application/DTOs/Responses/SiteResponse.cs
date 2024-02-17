namespace GooNet.Application.DTOs.Responses;

public class SiteResponse
{
    public SiteResponse(string title, string link)
    {
        Title = title;
        
        Link = link;
    }
    
    public string Title { get; set; }
    
    public string Link { get; set; }
}