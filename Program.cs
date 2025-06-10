using System.Threading.Tasks;
using HelloWorld.Models;
using HelloWorld.Service;
using Microsoft.Extensions.Configuration;

internal class Program
{
    private static async Task Main(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(@"Properties\appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // ConnectionToDatabaseDapper.Run(config);
        // ConnectionToDatabaseEF.Run(config);

        // ReadFile.Run();
        // ReadFileJSON.Run(config);

        // MappingData.Run();

        await AsyncAwaitImplement.Run();

    }

}