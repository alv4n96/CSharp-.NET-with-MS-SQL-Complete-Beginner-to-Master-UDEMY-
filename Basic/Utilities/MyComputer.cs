using HelloWorld.Models;

namespace HelloWorld.Utilities;
public static class ComputerDefaults
{
    public static readonly Computer MyComputer = new Computer
    {
        Motherboard = "ASUS ROG Strix X570-E",
        CPUCores = 16,
        HasWifi = false,
        HasLTE = true,
        ReleaseDate = new DateTime(2020, 7, 7),
        Price = 299.99m,
        VideoCard = "AMD Radeon RX 6800 XT"
    };
}