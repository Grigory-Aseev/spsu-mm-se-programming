using System.Diagnostics;
using System.Threading.Channels;

namespace Fibers.ProcessManagerFramework;

using FibersLib;

public class CycleScheduler : IScheduler
{
    private readonly Queue<FiberStorage> _fibers;
    private uint _deprecatedFiber;

    public CycleScheduler()
    {
        _fibers = new Queue<FiberStorage>();
    }

    public void Schedule()
    {
        // if there are no running fibers, then we have to switch to the main fibers
        if (_fibers.Count == 0)
        {
            if (Fiber.PrimaryId != 0)
                Fiber.Switch(Fiber.PrimaryId);
            else
                throw new InvalidOperationException("There is no process at the moment.");
        }
        else
        {
            // take the fiber we want to connect to, if it's free switch to
            var currentFiber = _fibers.Peek();
            if (currentFiber.State is not (FiberState.Ready or FiberState.Paused)) return;
            currentFiber.State = FiberState.Working;
            Fiber.Switch(currentFiber.Id);
        }
    }

    public void StopFiber(bool isFinished)
    {

        // all the fibers have been deleted, you can't stop something that doesn't exist
        if (_fibers.Count == 0) throw new InvalidOperationException("There is no fiber to stop at the moment.");

        if (isFinished)
        {
            // remove the previous fiber
            if (_deprecatedFiber != 0)
            {
                Fiber.Delete(_deprecatedFiber);
            }
            // remove the fiber from the queue and leave it for future deletion
            var fiber = _fibers.Dequeue();
            _deprecatedFiber = fiber.Id;
        }
        else
        {
            // pause the fiber and return it to the end of the queue
            var fiber = _fibers.Dequeue();
            if (_fibers.Count != 0)
            {
                fiber.State = FiberState.Paused;
            }
            _fibers.Enqueue(fiber);
        }
    }

    public void AddProcess(Process process)
    {
        // starting up the fibers
        var fiberTime = DateTime.Now;
        var fiber = new Fiber(process.Run);
        _fibers.Enqueue(new FiberStorage(fiber.Id, 0, fiberTime));
    }


    public void Dispose()
    {
        // remove the remaining fiber, if there is any
        if (_deprecatedFiber == 0)
        {
            return;
        }
        Fiber.Delete(_deprecatedFiber);
        _deprecatedFiber = 0;
    }

}