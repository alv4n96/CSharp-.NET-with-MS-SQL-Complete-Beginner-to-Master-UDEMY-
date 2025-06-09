using System.Data;
using Dapper;
using HelloWorld.Data;
using HelloWorld.Models;
using HelloWorld.Service;
using Microsoft.Data.SqlClient;

internal class Program
{
    private static void Main(string[] args)
    {
        DataContextDapper dapper = new DataContextDapper();

        string sqlCommand = "SELECT GETDATE()";

        DateTime rightNow = dapper.LoadDataSingle<DateTime>(sqlCommand);

        Console.WriteLine($"Current Date and Time: {rightNow}");

        Computer myComputer = new Computer()
        {
            Motherboard = "VIVOBOOK Pro 15",
            CPUCores = 5,
            HasWifi = true,
            HasLTE = true,
            ReleaseDate = new DateTime(2023, 2, 23),
            Price = 1599.99m,
            VideoCard = "AMD Radeon RX 6800M"
        };

        // string insertCommand = @"
        //             INSERT INTO TutorialAppSchema.Computer (Motherboard, CPUCores, HasWifi, HasLTE, ReleaseDate, Price, VideoCard)
        //             VALUES (@Motherboard, @CPUCores, @HasWifi, @HasLTE, @ReleaseDate, @Price, @VideoCard)";
                    
        // bool result = dapper.ExecuteSql<Computer>(insertCommand, myComputer);

        // CreateDataComputer.Execute(dbConnection, myComputer);

        string selectCommand = "SELECT * FROM TutorialAppSchema.Computer";

        IEnumerable<Computer> computers = dapper.LoadData<Computer>(selectCommand);

        foreach (var computer in computers)
        {
            Console.WriteLine($"\nMotherboard: {computer.Motherboard} \nCPU Cores: {computer.CPUCores} \nHas WiFi: {computer.HasWifi} \nHas LTE: {computer.HasLTE} \nRelease Date: {computer.ReleaseDate} \nPrice: {computer.Price} \nVideo Card: {computer.VideoCard}");
        }


        // ReadDataComputer.Execute(dbConnection);



        // UpdateDataComputer.Execute(dbConnection, myComputer);

    }
}