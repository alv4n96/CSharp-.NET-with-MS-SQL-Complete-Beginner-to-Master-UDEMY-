using System.Globalization;
using System.Linq;
using HelloWorld.Data;
using HelloWorld.Models;
using HelloWorld.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HelloWorld.Service;

public class ConnectionToDatabaseEF
{
    public static void Run(IConfiguration config)
    {

        DataContextDapper dapper = new DataContextDapper(config);
        DataContextEF entityFramework = new DataContextEF(config);


        // Jalankan SELECT GETDATE() langsung dari EF
        using var command = entityFramework.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT GETDATE()";
        entityFramework.Database.OpenConnection();

        var result = command.ExecuteScalar();
        DateTime rightNow = Convert.ToDateTime(result);

        Console.WriteLine($"Current Date and Time from EF: {rightNow}");

        // entityFramework.Add(ComputerDefaults.MyComputer);
        // entityFramework.SaveChanges();

        IEnumerable<Computer>? computersEf = entityFramework.Computer?.ToList<Computer>();

        if (computersEf != null)
        {
            Console.WriteLine("'ComputerId','Motherboard','HasWifi','HasLTE','ReleaseDate'"
                + ",'Price','VideoCard'");
            foreach (Computer singleComputer in computersEf)
            {
                Console.WriteLine("'" + singleComputer.ComputerId
                    + "','" + singleComputer.Motherboard
                    + "','" + singleComputer.HasWifi
                    + "','" + singleComputer.HasLTE
                    + "','" + singleComputer.ReleaseDate!.Value.ToString("yyyy-MM-dd")
                    + "','" + singleComputer.Price.ToString("0.00", CultureInfo.InvariantCulture)
                    + "','" + singleComputer.VideoCard + "'");
            }
        }

    }
}