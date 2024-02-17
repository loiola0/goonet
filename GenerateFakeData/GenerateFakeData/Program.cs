using GenerateFakeData.config;
using GenerateFakeData.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GenerateFakeData
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            
            ConfigureServices(serviceCollection, configuration);
           
            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            var generateFakeDataService = serviceProvider.GetRequiredService<GenerateFakeDataService>();
            
            await generateFakeDataService.InsertDataInIndexElasticSearch();
        }
        
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddElasticsearch(configuration);

            services.AddSingleton(configuration);
            
            services.AddSingleton<GenerateFakeDataService>();
        }
    }
}