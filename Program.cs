using System.Globalization;
using System.Text.Json;
using AutoMapper;
using HelloWorld.Data;
using HelloWorld.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        DataContextDapper dapper = new DataContextDapper(config);
        DataContextEF entityFramework = new DataContextEF(config);

        string sqlCommand = "SELECT GETDATE()";

        DateTime rightNow = dapper.LoadDataSingle<DateTime>(sqlCommand);

        Console.WriteLine($"Current Date and Time: {rightNow}");

        Computer myComputer = new Computer()
        {
            Motherboard = "ASUS ROG Strix X570-E",
            CPUCores = 16,
            HasWifi = false,
            HasLTE = true,
            ReleaseDate = new DateTime(2020, 7, 7),
            Price = 299.99m,
            VideoCard = "AMD Radeon RX 6800 XT"
        };


        // entityFramework.Add(myComputer);
        // entityFramework.SaveChanges();



        // string insertCommand = @"
        //             INSERT INTO TutorialAppSchema.Computer (Motherboard, CPUCores, HasWifi, HasLTE, ReleaseDate, Price, VideoCard)
        //             VALUES (@Motherboard, @CPUCores, @HasWifi, @HasLTE, @ReleaseDate, @Price, @VideoCard)";

        // bool result = dapper.ExecuteSql<Computer>(insertCommand, myComputer);

        // CreateDataComputer.Execute(dbConnection, myComputer);

        // string selectCommand = "SELECT * FROM TutorialAppSchema.Computer";

        // File.WriteAllText("log.txt", "\n" + selectCommand + "\n");

        // using StreamWriter writer = new StreamWriter("log.txt", append: true);

        // writer.WriteLine("\n" + selectCommand + "\n");

        // writer.Close();

        // System.Console.WriteLine(File.ReadAllText("log.txt"));


        // IEnumerable<Computer> computers = dapper.LoadData<Computer>(selectCommand);

        // foreach (var computer in computers)
        // {
        //     // Console.WriteLine($"\nMotherboard: {computer.Motherboard} \nCPU Cores: {computer.CPUCores} \nHas WiFi: {computer.HasWifi} \nHas LTE: {computer.HasLTE} \nRelease Date: {computer.ReleaseDate} \nPrice: {computer.Price} \nVideo Card: {computer.VideoCard}");
        // }

        // IEnumerable<Computer>? computersEF = entityFramework.Computer?.ToList<Computer>();

        // if (computersEF != null)
        // {
        //     foreach (var computer in computersEF)
        //     {
        //         Console.WriteLine($"\nComputer Id : {computer.ComputerId}\nMotherboard: {computer.Motherboard} \nCPU Cores: {computer.CPUCores} \nHas WiFi: {computer.HasWifi} \nHas LTE: {computer.HasLTE} \nRelease Date: {computer.ReleaseDate} \nPrice: {computer.Price} \nVideo Card: {computer.VideoCard}");
        //     }
        // }

        string ComputerJson = File.ReadAllText("ComputersSnake.json");

        // Mapper mapper = new Mapper(new MapperConfiguration(cfg =>
        // {
        //     cfg.CreateMap<ComputerSnake, Computer>()
        //         .ForMember(dest => dest.ComputerId, opt =>
        //             opt.MapFrom(src => src.computer_id))
        //         .ForMember(dest => dest.CPUCores, opt =>
        //             opt.MapFrom(src => src.cpu_cores))
        //         .ForMember(dest => dest.HasLTE, opt =>
        //             opt.MapFrom(src => src.has_lte))
        //         .ForMember(dest => dest.HasWifi, opt =>
        //             opt.MapFrom(src => src.has_wifi))
        //         .ForMember(dest => dest.Motherboard, opt =>
        //             opt.MapFrom(src => src.motherboard))
        //         .ForMember(dest => dest.VideoCard, opt =>
        //             opt.MapFrom(src => src.video_card))
        //         .ForMember(dest => dest.ReleaseDate, opt =>
        //             opt.MapFrom(src => src.release_date))
        //         .ForMember(dest => dest.Price, opt =>
        //             opt.MapFrom(src => src.price));
        // }));

        IEnumerable<Computer>? computers = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Computer>>(ComputerJson);

        if (computers != null)
        {
            // IEnumerable<Computer> mappedComputers = mapper.Map<IEnumerable<Computer>>(computers);

            foreach (Computer computer in computers)
            {
                Console.WriteLine($"Motherboard: {computer.Motherboard}");
            }
        }

        // System.Console.WriteLine($"{ComputerJson}");

        // JsonSerializerOptions options = new JsonSerializerOptions
        // {
        //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // };

        // IEnumerable<Computer>? computers = JsonSerializer.Deserialize<IEnumerable<Computer>>(ComputerJson, options);
        // IEnumerable<Computer>? computers = JsonConvert.DeserializeObject<IEnumerable<Computer>>(ComputerJson);

        // if (computers != null)
        // {
        //     foreach (Computer computer in computers)
        //     {
        //         // Console.WriteLine(computer.Motherboard);
        //         string sql = @"INSERT INTO TutorialAppSchema.Computer (
        //                 Motherboard,
        //                 HasWifi,
        //                 HasLTE,
        //                 ReleaseDate,
        //                 Price,
        //                 VideoCard
        //             ) VALUES ('" + EscapeSingleQuote(computer.Motherboard)
        //                 + "','" + computer.HasWifi
        //                 + "','" + computer.HasLTE
        //                 + "','" + computer.ReleaseDate?.ToString("yyyy-MM-dd")
        //                 + "','" + computer.Price.ToString("0.00", CultureInfo.InvariantCulture)
        //                 + "','" + EscapeSingleQuote(computer.VideoCard)
        //         + "')";

        //         bool result = dapper.ExecuteSql(sql);
        //     }
        // }

        // string computersNewtonCopy = JsonConvert.SerializeObject(computers, new JsonSerializerSettings()
        // {
        //     ContractResolver = new CamelCasePropertyNamesContractResolver(),
        // });

        // File.WriteAllText("ComputersCopyNewtonsoft.json", computersNewtonCopy);

        // string computersSystemCopy = System.Text.Json.JsonSerializer.Serialize(computers, new JsonSerializerOptions
        // {
        //     WriteIndented = true,
        //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        // });
        // File.WriteAllText("ComputersCopySystem.json", computersSystemCopy);
    }

    static string EscapeSingleQuote(string input)
    {
        string output = input.Replace("'", "''");

        return output;
    }

}