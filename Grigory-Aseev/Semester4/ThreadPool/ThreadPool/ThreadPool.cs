namespace ThreadPool;
public class ThreadPool : IDisposable
{
    private readonly object _tasksSync = new();
    private readonly List<Thread> _threads;
    private readonly Queue<Action> _tasks = new();
    private volatile bool _isDisposed;

    public ThreadPool(int capacity)
    {
        _threads = Enumerable.Range(1, capacity).Select(_ => {
             var thread = new Thread(RunThread);
             thread.Start();
             return thread;
         }).ToList(); // Initializing threading
    }

    public void Enqueue(Action a)
    {

        if (_isDisposed)
            throw new InvalidOperationException("Thread pool object was disposed");


        Monitor.Enter(_tasksSync); // Critical section for adopting the task
        _tasks.Enqueue(a);

        Monitor.Pulse(_tasksSync);
        Monitor.Exit(_tasksSync);
    }

    public void Dispose()
    {
        _isDisposed = true;

        Monitor.Enter(_tasksSync);
        Monitor.PulseAll(_tasksSync); // Notify threads that there are no more tasks
        Monitor.Exit(_tasksSync);

        _threads.ForEach(thread => thread.Join()); // Waiting for each thread to complete
        _threads.Clear();
    }

   private void RunThread()
   {
       while (true)
       {
            Action action;
            bool hasAction;
            try
            {
                Monitor.Enter(_tasksSync); // Critical section for taking the task
                while (_tasks.Count == 0)
                {
                    if (_isDisposed)
                    {
                        return;
                    }

                    Monitor.Wait(_tasksSync); // Waiting for the task to be added
                }

                action = _tasks.Dequeue();
                hasAction = true;
            }
            finally
            {
                Monitor.Exit(_tasksSync);
            }

            if (hasAction)
            {
                action();
            }
       }
   }
}