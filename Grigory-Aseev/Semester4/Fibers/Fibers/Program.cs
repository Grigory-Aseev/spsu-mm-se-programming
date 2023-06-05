using System.Net;
using System.Threading.Channels;

namespace Fibers;

using ProcessManagerFramework;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to my process scheduler!\n" +
                          "Type through space and specialize process manager startup settings!");
        Console.WriteLine("Specify in the first number the number of processes (greater than zero)\n" +
                              "In the second number specify the number of the scheduler\n");
        Console.WriteLine(
            "Available schedulers: \n\t Cycle - 0 \n\t Multi-level queues with reverse request - 1\n");
        Console.Write("[number of processes, type of scheduler]: ");
        var s = Console.ReadLine();
        if (string.IsNullOrEmpty(s))
        {
            Console.WriteLine("Goodbye!!!");
            return;
        }

        s = s.Trim();
        try
        {
            var input = s.Split(' ').Select(int.Parse).ToArray();
            if (input.Length != 2 || input[0] <= 0 || !(input[1] == 0 || input[1] == 1))
                throw new ArgumentException("Incorrect input");

            ProcessManager.ChangeAlgorithm((Strategy)input[1]);
            for (var i = 0; i < input[0]; i++) ProcessManager.AddProcess(new Process());
            ProcessManager.Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.WriteLine("Successful finished!");
    }
}