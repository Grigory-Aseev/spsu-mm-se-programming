using System.ComponentModel.Design;
using System.Diagnostics;

namespace Fibers.ProcessManagerFramework;

using FibersLib;

public sealed class MultilevelRevReqScheduler : IScheduler
{
    private const byte QuantumTimeMSeconds = 5;
    private const byte NumberLayers = 5;
    private readonly PriorityQueue<FiberStorage, long>[] _fibers;
    private readonly Queue<FiberStorage> _fibersLowest;
    private uint _deprecatedFiber;

    public MultilevelRevReqScheduler()
    {
        _fibers = new PriorityQueue<FiberStorage, long> [NumberLayers - 1];
        for (var i = 0; i < NumberLayers - 1; i++) _fibers[i] = new PriorityQueue<FiberStorage, long>();
        _fibersLowest = new Queue<FiberStorage>();
    }

    private int Count()
    {
        var count = 0;
        for (var i = 0; i < NumberLayers - 1; i++) count += _fibers[i].Count;
        count += _fibersLowest.Count;
        return count;
    }

    private (FiberStorage?, bool) FindFiber()
    {
        for (var i = 0; i < NumberLayers - 1; i++)
            if (_fibers[i].Count > 0)
                return (_fibers[i].Peek(), false);

        return _fibersLowest.TryPeek(out var result) ? (result, true) : (null, false);
    }

    public void Schedule()
    {
        
        var (nextFiber, isLowest) = FindFiber();

        if (nextFiber != null)
        {
            if (isLowest && _fibersLowest.Count / NumberLayers > 0)
                for (var i = 0; i < _fibersLowest.Count / NumberLayers; i++)
                {
                    var upgradedFiber = _fibersLowest.Dequeue();
                    _fibers[0].Enqueue(upgradedFiber,
                        upgradedFiber.Priority +
                        upgradedFiber.OccupiedTime / (_fibers[0].Count > 0 ? _fibers[0].Count : 1));
                }

            var time = DateTime.Now;
            nextFiber.StartTime = time;
            Fiber.Switch(nextFiber.Id);
        }
        else
        {
            if (Fiber.PrimaryId != 0)
                Fiber.Switch(Fiber.PrimaryId);
            else
                throw new InvalidOperationException("There is no process at the moment.");
        }

    }

    public void StopFiber(bool isFinished)
    {
        var time = DateTime.Now;
        for (var i = 0; i < NumberLayers - 1; i++)
            if (_fibers[i].Count > 0)
            {
                if (isFinished)
                {
                    if (_deprecatedFiber != 0)
                    {
                        Fiber.Delete(_deprecatedFiber);
                    }
                    var fiber = _fibers[i].Dequeue();
                    _deprecatedFiber = fiber.Id;
                }
                else
                {
                    var fiber = _fibers[i].Dequeue();
                    fiber.OccupiedTime += (time - fiber.StartTime).Milliseconds;
                    if (fiber.OccupiedTime < 8 * QuantumTimeMSeconds * Math.Pow(2, i))
                    {
                        _fibers[i].Enqueue(fiber,
                            fiber.Priority + fiber.OccupiedTime / (_fibers[i].Count > 0 ? _fibers[i].Count : 1));
                    }
                    else
                    {
                        if (i == NumberLayers - 2)
                            _fibersLowest.Enqueue(fiber);
                        else
                            _fibers[i + 1].Enqueue(fiber,
                                fiber.Priority + fiber.OccupiedTime /
                                (_fibers[i + 1].Count > 0 ? _fibers[i + 1].Count : 1));
                    }
                }

                return;
            }

        if (_fibersLowest.Count == 0) throw new InvalidOperationException("There is no fiber to stop at the moment.");
        if (isFinished)
        {
            if (_deprecatedFiber != 0)
            {
                Fiber.Delete(_deprecatedFiber);
            }
            var fiber = _fibersLowest.Dequeue();
            _deprecatedFiber = fiber.Id;
        }
        else
        {
            var fiber = _fibersLowest.Dequeue();
            _fibersLowest.Enqueue(fiber);
        }
    }

    public void AddProcess(Process process)
    {
        var fiberTime = DateTime.Now;
        var fiber = new Fiber(process.Run);
        var fiberStorage = new FiberStorage(fiber.Id, process.Priority, fiberTime);
        _fibers[0].Enqueue(fiberStorage, fiberStorage.Priority);
    }
    
    public void Dispose()
    {
        if (_deprecatedFiber == 0)
        {
            return;
        }
        Fiber.Delete(_deprecatedFiber);
        _deprecatedFiber = 0;
    }

}