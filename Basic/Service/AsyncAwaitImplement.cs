using System.Threading.Tasks;

namespace HelloWorld.Service;

public class AsyncAwaitImplement
{
    public static async Task Run()
    {
        // Task firstTask = Task.Run(() =>
        Task firstTask = new Task(() =>
        {
            Console.WriteLine("First task started.");
            Thread.Sleep(1000); // Simulate work
            Console.WriteLine("First task completed.");
        });

        firstTask.Start();

        Task secondTask = consoleAFterDelayAsync("Second task running.", 1500);

        consoleAFterDelay("First void running.", 3000);

        Task thirdTask = consoleAFterDelayAsync("Third task running.", 500);

        await secondTask;
        await thirdTask;

        await firstTask;
        Console.WriteLine("End Of Task Execution");

    }

    private static void consoleAFterDelay(string text, int delay)
    {
        Thread.Sleep(delay);
        Console.WriteLine(text);
    }

    private static async Task consoleAFterDelayAsync(string text, int delay)
    {
        await Task.Delay(delay);
        Console.WriteLine(text);
    }

}