namespace ProducerConsumer
{
    public class Manager<T>
    {
        private readonly List<Agent<T>> _agents;

        public Manager(int countOfConsumers, int countOfProducers, int timePauseLimit, Func<T> creationObjectFunc)
        {
            var random = new Random();
            var semaphore = new Semaphore(1, 1);
            List<T> objects = new();
            _agents = new List<Agent<T>>();

            // adding agents

            this._agents.AddRange(Enumerable.Range(1, countOfConsumers)
                .Select(_ => new Consumer<T>(objects, random.Next(1, timePauseLimit), semaphore)).ToList());
            this._agents.AddRange(Enumerable.Range(1, countOfProducers).Select(_ =>
                new Producer<T>(objects, random.Next(1, timePauseLimit), semaphore, creationObjectFunc)).ToList());

            Console.WriteLine("Manager added producers and consumers.");
        }

        public void Start()
        {
            // launch agents till the key is not entered
            LaunchAgents();
            Console.ReadKey(true);
            StopAgents();
        }

        private void LaunchAgents()
        {
            Console.WriteLine("The agents were launched.");

            foreach (var agent in _agents)
                agent.Start();
        }

        private void StopAgents()
        {
            foreach (var agent in _agents)
                agent.Stop();

            Console.WriteLine("The agents were stopped.");
        }
    }
}