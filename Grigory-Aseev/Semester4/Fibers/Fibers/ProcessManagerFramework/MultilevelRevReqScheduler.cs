using System.ComponentModel.Design;
using System.Diagnostics;

namespace Fibers.ProcessManagerFramework;

using FibersLib;

public sealed class MultilevelRevReqScheduler : IScheduler
{
    // quantum of time to move to a lower priority level
    private const byte QuantumTimeMSeconds = 5;
    // number of levels
    private const byte NumberLayers = 5;
    // priority levels
    private readonly PriorityQueue<FiberStorage, long>[] _fibers;
    // lowest priority level
    private readonly Queue<FiberStorage> _fibersLowest;
    private uint _deprecatedFiber;
    private bool _isLast;
    private int _deleteFibers = 0;
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
        // find the current fiber by priority
        for (var i = 0; i < NumberLayers - 1; i++)
            if (_fibers[i].Count > 0)
                return (_fibers[i].Peek(), false);

        return _fibersLowest.TryPeek(out var result) ? (result, true) : (null, false);
    }

    public void Schedule()
    {
        Console.WriteLine(_deleteFibers);
        var (nextFiber, isLowest) = FindFiber();

        if (nextFiber != null && Count() > 1)
        {
            // If all are on the lower level, then return some to the upper level
            if (isLowest)
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
        else if (nextFiber == null)
        {
            // there are no fibers left - switch to the main fiber

            if (Fiber.PrimaryId != 0)
                Fiber.Switch(Fiber.PrimaryId);
            else
                throw new InvalidOperationException("There is no process at the moment.");
        }
        else if (!_isLast)
        {
            // switch once to the last fiber
            _isLast = true;
            Fiber.Switch(nextFiber.Id);
        }
    }

    public void StopFiber(bool isFinished)
    {
        Console.WriteLine( _deleteFibers);
        var time = DateTime.Now;
        for (var i = 0; i < NumberLayers - 1; i++)
            if (_fibers[i].Count > 0)
            {
                if (isFinished)
                {
                    // delete the previous fiber, and set the next fiber as obsolete
                    if (_deprecatedFiber != 0)
                    {
                        Fiber.Delete(_deprecatedFiber);
                        _deleteFibers++;
                        Console.WriteLine(_deleteFibers);
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
                        // if the limit in the current level has not yet been exceeded, update its priority and return
                        _fibers[i].Enqueue(fiber,
                            fiber.Priority + fiber.OccupiedTime / (_fibers[i].Count > 0 ? _fibers[i].Count : 1));
                    }
                    else
                    {
                        //  otherwise go to the next level
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

        // the current fiber is at the lower level, so we also work but do not go lower

        if (_fibersLowest.Count == 0) throw new InvalidOperationException("There is no fiber to stop at the moment.");
        if (isFinished)
        {
            if (_deprecatedFiber != 0)
            {
                Fiber.Delete(_deprecatedFiber);
                _deleteFibers++;
                Console.WriteLine(_deleteFibers);
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
        Console.WriteLine(_deleteFibers);
        if (_deprecatedFiber == 0)
        {
            return;
        }
        Fiber.Delete(_deprecatedFiber);
        _deleteFibers++;
        Console.WriteLine(_deleteFibers);
        _deprecatedFiber = 0;
    }

}