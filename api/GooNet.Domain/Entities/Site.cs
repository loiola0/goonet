namespace GooNet.Domain.Entities;

public class Site
{
    public Site()
    {
        Title = string.Empty;
        
        Link = string.Empty;
    }
    
    public string Title { get; set; }
    
    public string Link { get; set; }
}