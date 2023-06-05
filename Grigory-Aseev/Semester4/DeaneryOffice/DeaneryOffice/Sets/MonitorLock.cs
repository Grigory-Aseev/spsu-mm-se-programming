namespace DeaneryOffice.Sets;

public class MonitorLock
{
    private readonly object _locker = new object();

    public void Lock() => Monitor.Enter(_locker);

    public void Unlock() => Monitor.Exit(_locker);
}