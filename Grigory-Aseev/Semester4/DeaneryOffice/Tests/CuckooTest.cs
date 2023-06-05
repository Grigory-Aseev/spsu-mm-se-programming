using DeaneryOffice.ExamSystems;
using NUnit.Framework;

namespace Tests;


public class CuckooTests
{
    private StripedCuckooExamSystem _cuckooExamSystem;

    [SetUp]
    public void Setup()
    {
        _cuckooExamSystem = new StripedCuckooExamSystem();
    }

    [Test]
    public void AddTest()
    {

        for (var i = 0; i < 10; i++)
            _cuckooExamSystem.Add(i, i);

        Assert.That(_cuckooExamSystem.Count, Is.EqualTo(10));

        for (var i = 0; i < 10; i++)
            Assert.That(_cuckooExamSystem.Contains(i,  i), Is.True);
    }

    [Test]
    public void RemoveTest()
    {

        for (var i = 0; i < 10; i++)
            _cuckooExamSystem.Add(i, i);

        Assert.That(_cuckooExamSystem.Contains(5, 5), Is.True);

        _cuckooExamSystem.Remove(5, 5);

        Assert.That(_cuckooExamSystem.Contains(5, 5), Is.False);
    }

    [Test]
    public void CountTest()
    {

        for (var i = 0; i < 10; i++)
            _cuckooExamSystem.Add(i,  i);

        Assert.That(_cuckooExamSystem.Count, Is.EqualTo(10));

        _cuckooExamSystem.Remove(5, 5);

        Assert.That(_cuckooExamSystem.Count, Is.EqualTo(9));
    }

    [Test]
    public void ResizeTest()
    {
        for (var i = 0; i < 500; i++)
            _cuckooExamSystem.Add(i, i);

        Assert.That(_cuckooExamSystem.Count, Is.EqualTo(500));
    }
}