namespace ProducerConsumer;

using static Thread;

public abstract class Agent<T>
{
    private protected readonly List<T> Objects;

    private readonly Thread _thread;
    private readonly int _timePause;

    private readonly Semaphore _semaphore;
    private volatile bool _stop;

    protected Agent(List<T> objects, int timePause, Semaphore semaphore)
    {
        Objects = objects;
        _thread = new Thread(ThreadAction);
        _timePause = timePause;
        _semaphore = semaphore;
    }


    protected abstract void Act();

    private void ThreadAction()
    {
        while (!_stop)
        {
            _semaphore.WaitOne();

            Act();

            _semaphore.Release();

            Sleep(_timePause);
        }
    }

    public void Start()
    {
        _stop = false;
        _thread.Start();
    }

    public void Stop()
    {
        _stop = true;
        _thread.Join();
    }
}