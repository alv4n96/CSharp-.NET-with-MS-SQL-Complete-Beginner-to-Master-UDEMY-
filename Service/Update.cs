using System.Data;
using Dapper;
using HelloWorld.Models;

namespace HelloWorld.Service;

public class UpdateDataComputer
{
    public static void Execute(IDbConnection parameterConnection, Computer parameterComputer)
    {
        string updateCommand = @"
            UPDATE TutorialAppSchema.Computer
            SET CPUCores = @CPUCores,
                HasWifi = @HasWifi,
                HasLTE = @HasLTE,
                ReleaseDate = @ReleaseDate,
                Price = @Price,
                VideoCard = @VideoCard
            WHERE Motherboard = @Motherboard";
        

        parameterConnection.Execute(updateCommand, parameterComputer);

        Console.WriteLine("Computer data updated successfully.");
    }
}