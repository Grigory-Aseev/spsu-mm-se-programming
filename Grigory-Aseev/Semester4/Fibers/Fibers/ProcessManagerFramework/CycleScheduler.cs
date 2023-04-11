using System.Diagnostics;

namespace Fibers.ProcessManagerFramework;

using FibersLib;

public class CycleScheduler : IScheduler
{
    private readonly Queue<FiberStorage> _fibers;
    private uint _deprecatedFiber;
    private bool _disposed;

    public CycleScheduler()
    {
        _fibers = new Queue<FiberStorage>();
    }

    public void Schedule()
    {
        if (_fibers.Count == 0)
        {
            if (Fiber.PrimaryId != 0)
                Fiber.Switch(Fiber.PrimaryId);
            else
                throw new InvalidOperationException("There is no process at the moment.");
        }
        else
        {
            var currentFiber = _fibers.Peek();
            if (currentFiber.State is not (FiberState.Ready or FiberState.Paused)) return;
            currentFiber.State = FiberState.Working;
            Fiber.Switch(currentFiber.Id);
        }

        if (_deprecatedFiber == 0) return;
        Fiber.Delete(_deprecatedFiber);
        _deprecatedFiber = 0;
    }

    public void StopFiber(bool isFinished)
    {
        if (_fibers.Count == 0) throw new InvalidOperationException("There is no fiber to stop at the moment.");

        if (isFinished)
        {
            var fiber = _fibers.Dequeue();
            _deprecatedFiber = fiber.Id;
        }
        else
        {
            var fiber = _fibers.Dequeue();
            fiber.State = FiberState.Paused;
            _fibers.Enqueue(fiber);
        }
    }

    public void AddProcess(Process process)
    {
        var fiberTime = DateTime.Now;
        var fiber = new Fiber(process.Run);
        _fibers.Enqueue(new FiberStorage(fiber.Id, 0, fiberTime));
    }

    private protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
            if (_deprecatedFiber != 0 || _fibers.Count != 0)
                throw new UnreachableException("There should be no active fibers and no fiber to be removed.");

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CycleScheduler()
    {
        Dispose(false);
    }
}