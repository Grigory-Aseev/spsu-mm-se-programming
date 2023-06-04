namespace ThreadPool;

public static class Program
{
    private static volatile int _tasksCompleted = 1;

    public static void Main(string[] args)
    {
        Console.WriteLine("The program demonstrates the work of the thread pool\n" +
                          "It counts prime numbers from 1 to 1000");
        do
        {

            Console.Write("Number of threads for the pool: ");
            var threads = Console.ReadLine();
            Console.Write("Number of tasks for the thread pool: ");
            var tasks = Console.ReadLine();

            if (!int.TryParse(threads, out var capacity) || !int.TryParse(tasks, out var tasksNumber))
            {
                Console.WriteLine("You have to write the numbers. Try again.");
                continue;
            }

            var pool = new ThreadPool(capacity);

            for (var i = 0; i < tasksNumber; i++)
                pool.Enqueue(TaskAction);

            pool.Dispose();
            Console.WriteLine("Successful finished");

            Console.WriteLine("Press Q to exit or any other key to continue");
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Q)
            {
                break;
            }

        } while (true);
        Console.WriteLine("Goodbye!");
    }

    private static void TaskAction()
    {
        var count = CountPrimes();
        Thread.Sleep(new Random().Next(1, 1000));
        var taskNumber = Interlocked.Increment(ref _tasksCompleted);
        Console.WriteLine($"The {taskNumber - 1} task was completed with a score of {count}");
    }

    public static int CountPrimes()
    {
        var count = 0;

        for (var i = 1; i <= 1000; i++)
        {
            if (IsPrime(i))
                count++;
        }

        return count;
    }



    public static bool IsPrime(int n) // The method that determines whether a number is prime
    {
        if (n < 2)
            return false;
        for (var i = 2; i <= Math.Sqrt(n); i++)
        {
            if (n % i == 0)
                return false;
        }

        return true;
    }
}