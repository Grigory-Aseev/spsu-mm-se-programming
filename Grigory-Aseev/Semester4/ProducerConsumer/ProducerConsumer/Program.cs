namespace ProducerConsumer;

public static class Program
{
    private static readonly Random Random = new Random();

    private static readonly Func<int> CreateInt = () => Random.Next(100000);

    public static int Main(string[] args)
    {
        const int countOfConsumers = 5;
        const int countOfProducers = 5;

        const int timePauseLimit = 16;

        var manager = new Manager<int>(countOfConsumers, countOfProducers, timePauseLimit, CreateInt);

        manager.Start();

        return 0;
    }
}