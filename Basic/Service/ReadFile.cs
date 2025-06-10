using System.Globalization;
using HelloWorld.Models;
using HelloWorld.Utilities;

namespace HelloWorld.Service;

public class ReadFile
{
    public static void Run()
    {
        Computer myComputer = ComputerDefaults.MyComputer;

        string sql = @"INSERT INTO TutorialAppSchema.Computer (
                Motherboard,
                HasWifi,
                HasLTE,
                ReleaseDate,
                Price,
                VideoCard
            ) VALUES ('" + myComputer.Motherboard
                + "','" + myComputer.HasWifi
                + "','" + myComputer.HasLTE
                + "','" + myComputer.ReleaseDate!.Value.ToString("yyyy-MM-dd")
                + "','" + myComputer.Price.ToString("0.00", CultureInfo.InvariantCulture)
                + "','" + myComputer.VideoCard
        + "')";

        File.WriteAllText(@"Files/Output/log.txt", "\n" + sql + "\n");

        using StreamWriter openFile = new(@"Files/Output/log.txt", append: true);

        openFile.WriteLine("\n" + sql + "\n");

        openFile.Close();

        string fileText = File.ReadAllText(@"Files/Output/log.txt");

        Console.WriteLine(fileText);
    }
}