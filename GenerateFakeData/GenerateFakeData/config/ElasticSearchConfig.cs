using GenerateFakeData.Entities;
using Nest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GenerateFakeData.config
{
    public static class ElasticSearchConfig
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var user = configuration["UserName"];

            var password = configuration["Password"];

            var uri = configuration["Uri"];

            var indexDefault = configuration["IndexSites"];
            
            var settings = new ConnectionSettings(new Uri(uri))
                .BasicAuthentication(user, password)
                .DefaultIndex(indexDefault);
            
            settings.EnableApiVersioningHeader();

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);

            CreateIndex(client, indexDefault);
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            if (client.Indices.Exists(indexName).Exists) return;
            
            var response = client.Indices.Create(indexName, c => c
                .Map<Site>(m => m
                    .AutoMap()
                    .Properties(ps => ps
                        .Number(n => n
                            .Name(p => p.Id)
                            .Type(NumberType.Integer)
                        )
                        .Text(t => t
                            .Name(n => n.Title)
                            .Fields(f => f
                                .Keyword(k => k.Name("keyword"))
                            )
                        ).Text(t => t
                            .Name(n => n.Link)
                            .Fields(f => f
                                .Keyword(k => k.Name("keyword"))
                            )
                        )
                    )
                )
            );
                
            if (response.IsValid)
                Console.WriteLine("Index created");

        }
    }
}