using System.Data;
using Dapper;
using HelloWorld.Models;

namespace HelloWorld.Service;

public class ReadDataComputer
{
    public static void Execute(IDbConnection poConnection)
    {
        string selectCommand = "SELECT * FROM TutorialAppSchema.Computer";

        IEnumerable<Computer> computers = poConnection.Query<Computer>(selectCommand);

        foreach (var computer in computers)
        {
            Console.WriteLine($"\nMotherboard: {computer.Motherboard} \nCPU Cores: {computer.CPUCores} \nHas WiFi: {computer.HasWifi} \nHas LTE: {computer.HasLTE} \nRelease Date: {computer.ReleaseDate} \nPrice: {computer.Price} \nVideo Card: {computer.VideoCard}");
        }

    }
}