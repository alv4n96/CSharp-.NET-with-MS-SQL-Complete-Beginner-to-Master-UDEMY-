using System.Data;
using Dapper;
using HelloWorld.Models;

namespace HelloWorld.Service;

public class CreateDataComputer
{
    public static void Execute(IDbConnection parameterConnection, Computer parameterComputer)
    {
        string insertCommand = @"
                    INSERT INTO TutorialAppSchema.Computer (Motherboard, CPUCores, HasWifi, HasLTE, ReleaseDate, Price, VideoCard)
                    VALUES (@Motherboard, @CPUCores, @HasWifi, @HasLTE, @ReleaseDate, @Price, @VideoCard)";

        parameterConnection.Execute(insertCommand, parameterComputer);

        Console.WriteLine("Computer data inserted successfully.");
    }
}