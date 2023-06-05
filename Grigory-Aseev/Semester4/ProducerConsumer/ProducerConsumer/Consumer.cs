namespace ProducerConsumer
{
    public class Consumer<T> : Agent<T>
    {
        public Consumer(List<T> objects, int timePause, Semaphore semaphore) :
            base(objects, timePause, semaphore)
        {
        }

        protected override void Act()
        {
            // taking an object
            if (Objects.Count == 0) return;
            Console.WriteLine(
                $"Consumer with {Environment.CurrentManagedThreadId} thread flow extracted object {Objects[0]}.");
            Objects.RemoveAt(0);
        }
    }
}