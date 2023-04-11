namespace Fibers.ProcessManagerFramework;

public interface IScheduler : IDisposable
{
    public void Schedule();
    public void StopFiber(bool isFinished);
    public void AddProcess(Process process);
}