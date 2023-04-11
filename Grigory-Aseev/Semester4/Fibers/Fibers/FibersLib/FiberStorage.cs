namespace Fibers.FibersLib;

public class FiberStorage
{
    public uint Id { get; }
    public int Priority { get; }
    public int OccupiedTime { get; set; }
    public FiberState State { get; set; } = FiberState.Ready;
    public DateTime StartTime;

    public FiberStorage(uint id, int priority, DateTime time)
    {
        Id = id;
        Priority = priority;
        StartTime = time;
        OccupiedTime = 0;
    }
}