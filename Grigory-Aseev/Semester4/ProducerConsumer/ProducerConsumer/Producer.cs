namespace ProducerConsumer
{
    public class Producer<T> : Agent<T>
    {
        private readonly Func<T> _creationObjectFunc;

        public Producer(List<T> objects, int timePause, Semaphore semaphore, Func<T> creationObjectFunc) : base(objects,
            timePause, semaphore)
        {
            this._creationObjectFunc = creationObjectFunc;
        }

        protected override void Act()
        {
            // creating an object
            var someObject = _creationObjectFunc();
            Objects.Add(someObject);

            Console.WriteLine(
                $"Object {someObject} was added by producer with {Environment.CurrentManagedThreadId} thread.");
        }
    }
}