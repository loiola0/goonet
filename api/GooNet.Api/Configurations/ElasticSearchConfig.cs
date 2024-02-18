using Nest;

namespace GooNet.Api.Configurations
{
    public static class ElasticSearchConfig
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var user = configuration["USER_NAME"];

            var password = configuration["PASSWORD"];

            var uri = configuration["URI"];

            var indexDefault = configuration["INDEX_SITES"];

            var settings = new ConnectionSettings(new Uri(uri))
                .BasicAuthentication(user, password)
                .DefaultIndex(indexDefault);

            settings.EnableApiVersioningHeader();

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
        }
    }
}