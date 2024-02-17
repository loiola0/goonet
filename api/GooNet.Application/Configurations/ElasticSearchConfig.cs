using Nest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GooNet.Application.Configurations
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
        }
    }
}