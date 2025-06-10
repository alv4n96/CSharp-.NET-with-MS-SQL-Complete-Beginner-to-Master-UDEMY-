using System.Globalization;
using System.Text.Json;
using HelloWorld.Data;
using HelloWorld.Models;
using HelloWorld.Utilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HelloWorld.Service;

public class ReadFileJSON
{
    public static void Run(IConfiguration config)
    {
        DataContextDapper dapper = new DataContextDapper(config);

        string computersJson = File.ReadAllText(@"Files\Input\Computers.json");

        // Console.WriteLine(computersJson);

        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        IEnumerable<Computer>? computersNewtonSoft = JsonConvert.DeserializeObject<IEnumerable<Computer>>(computersJson);

        IEnumerable<Computer>? computersSystem = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Computer>>(computersJson, options);


        if (computersNewtonSoft != null)
        {
            foreach (Computer computer in computersNewtonSoft)
            {
                // Console.WriteLine(computer.Motherboard);
                string sql = @"INSERT INTO TutorialAppSchema.Computer (
                        Motherboard,
                        HasWifi,
                        HasLTE,
                        ReleaseDate,
                        Price,
                        VideoCard
                    ) VALUES ('" + EscapeSingleQuote.Escape(computer.Motherboard)
                        + "','" + computer.HasWifi
                        + "','" + computer.HasLTE
                        + "','" + computer.ReleaseDate?.ToString("yyyy-MM-dd")
                        + "','" + computer.Price.ToString("0.00", CultureInfo.InvariantCulture)
                        + "','" + EscapeSingleQuote.Escape(computer.VideoCard)
                + "')";

                // bool result = dapper.ExecuteSql(sql);
            }
        }

        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        string computersCopyNewtonsoft = JsonConvert.SerializeObject(computersNewtonSoft, settings);

        // string folderPath = Path.Combine("Files", "Output");
        // string fullFilePath = Path.Combine(folderPath, "computersCopyNewtonsoft.json");
        // Directory.CreateDirectory(folderPath);
        // File.WriteAllText(fullFilePath, computersCopyNewtonsoft);

        File.WriteAllText(@"Files/Output/computersCopyNewtonsoft.json", computersCopyNewtonsoft);

        string computersCopySystem = System.Text.Json.JsonSerializer.Serialize(computersSystem, options);

        File.WriteAllText(@"Files/Output/computersCopySystem.json", computersCopySystem);
    }
}