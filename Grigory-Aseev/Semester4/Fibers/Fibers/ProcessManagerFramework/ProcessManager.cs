namespace Fibers.ProcessManagerFramework;

using FibersLib;

public static class ProcessManager
{
    private static Strategy Algorithm { get; set; } = Strategy.NonPriority;
    private static IScheduler scheduler = new CycleScheduler();

    public static void Switch(bool fiberFinished)
    {
        // stop the fiber and continue planning the next fiber
        scheduler.StopFiber(fiberFinished);
        scheduler.Schedule();
    }

    public static void Run()
    {
        scheduler.Schedule();
        scheduler.Dispose();
    }

    public static void AddProcess(Process process)
    {
        scheduler.AddProcess(process);
    }

    public static void ChangeAlgorithm(Strategy algorithm)
    {
        if (algorithm == Algorithm) return;
        Algorithm = algorithm;
        scheduler = algorithm switch
        {
            Strategy.NonPriority => new CycleScheduler(),
            Strategy.Priority => new MultilevelRevReqScheduler(),
            _ => throw new ArgumentException("There is no algorithm with this name.")
        };
    }
}