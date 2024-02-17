using Bogus;
using GenerateFakeData.Entities;
using Microsoft.Extensions.Configuration;
using Nest;

namespace GenerateFakeData.Services;

public class GenerateFakeDataService
{
    private readonly IElasticClient _elasticClient;

    private readonly IConfiguration _configuration;
    
    public GenerateFakeDataService(IElasticClient elasticClient, IConfiguration configuration)
    {
        _elasticClient = elasticClient;
        
        _configuration = configuration;
    }

    public async Task InsertDataInIndexElasticSearch()
    {
        var sitesFake = new Faker<Site>("pt_BR")
            .RuleFor(c => c.Id, f => f.IndexFaker)
            .RuleFor(c => c.Title, f => f.Company.CompanyName().ToString())
            .RuleFor(c => c.Link, f => f.Internet.Url().ToLower());

        const int totalDataFake = 10000000;
        
        var sites = sitesFake.Generate(totalDataFake);
        
        const int totalInsertionsPerIteration = 100000;

        const int  totalIterations = (totalDataFake / totalInsertionsPerIteration) + 1;
        
        var indexDefault = _configuration["IndexSites"];
        
        for (var i = 0; i < totalIterations; i++)
        {
            var descriptor = new BulkDescriptor();
            
            descriptor.IndexMany(sites.Skip((i - 1) * totalInsertionsPerIteration)
                                      .Take(totalInsertionsPerIteration),
                                        (idx, obj) =>
                                        idx.Index(indexDefault).Document(obj));
                                
            var response = await _elasticClient.BulkAsync(descriptor);
            
            if (!response.IsValid)
                throw new Exception(message: "An error occurred while entering data");
        }
        
        Console.WriteLine("All data entered.");
        
    }
}