using HelloWorld.Data;
using HelloWorld.Models;
using HelloWorld.Utilities;
using Microsoft.Extensions.Configuration;

namespace HelloWorld.Service;

public class ConnectionToDatabaseDapper
{
    public static void Run(IConfiguration config)
    {

        DataContextDapper dapper = new DataContextDapper(config);

        string sqlCommand = "SELECT GETDATE()";
        DateTime rightNow = dapper.LoadDataSingle<DateTime>(sqlCommand);
        Console.WriteLine($"Current Date and Time from Dapper: {rightNow}");

        // string insertCommand = @"
        //             INSERT INTO TutorialAppSchema.Computer (Motherboard, CPUCores, HasWifi, HasLTE, ReleaseDate, Price, VideoCard)
        //             VALUES (@Motherboard, @CPUCores, @HasWifi, @HasLTE, @ReleaseDate, @Price, @VideoCard)";

        // bool result = dapper.ExecuteSql<Computer>(insertCommand, ComputerDefaults.MyComputer);


        string selectCommand = "SELECT * FROM TutorialAppSchema.Computer";

        IEnumerable<Computer> computers = dapper.LoadData<Computer>(selectCommand);

        foreach (var computer in computers)
        {
            Console.WriteLine($"\nMotherboard: {computer.Motherboard} \nCPU Cores: {computer.CPUCores} \nHas WiFi: {computer.HasWifi} \nHas LTE: {computer.HasLTE} \nRelease Date: {computer.ReleaseDate} \nPrice: {computer.Price} \nVideo Card: {computer.VideoCard}");
        }


    }
}