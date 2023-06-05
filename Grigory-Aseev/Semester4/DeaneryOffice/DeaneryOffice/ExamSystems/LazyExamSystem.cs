namespace DeaneryOffice.ExamSystems;

using DeaneryOffice.Sets;

public class LazyExamSystem : IExamSystem
{
    // (long studentId, long courseId)
    private readonly LazySet<(long, long)> _lazySet = new();


    public void Add(long studentId, long courseId)
    {
        _lazySet.Add((studentId, courseId));
    }

    public void Remove(long studentId, long courseId)
    {
        _lazySet.Remove((studentId, courseId));
    }

    public bool Contains(long studentId, long courseId) => _lazySet.Сontains((studentId, courseId));

    public int Count => _lazySet.Count;
}