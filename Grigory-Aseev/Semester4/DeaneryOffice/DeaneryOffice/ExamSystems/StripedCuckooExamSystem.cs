namespace DeaneryOffice.ExamSystems;

using DeaneryOffice.Sets;

public class StripedCuckooExamSystem : IExamSystem
{
    private readonly StripedCuckooHashSet<(long, long)> _cuckooHashSet = new(64);
    private readonly object _lockerObject = new();


    public void Add(long studentId, long courseId)
    {
        if (_cuckooHashSet.Add((studentId, courseId)))
        {
            lock (_lockerObject) Count++;
        }
    }

    public void Remove(long studentId, long courseId)
    {
        if (_cuckooHashSet.Remove((studentId, courseId)))
        {
            lock (_lockerObject) Count--;
        }
    }

    public bool Contains(long studentId, long courseId) => _cuckooHashSet.Contains((studentId, courseId));

    public int Count { get; private set; }
}