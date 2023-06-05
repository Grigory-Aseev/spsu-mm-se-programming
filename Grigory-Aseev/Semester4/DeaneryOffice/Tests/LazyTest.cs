using DeaneryOffice.ExamSystems;
using NUnit.Framework;

namespace Tests;


public class LazyTests
{
    private LazyExamSystem _lazySystem;

    [SetUp]
    public void Setup()
    {
        _lazySystem = new LazyExamSystem();
    }

    [Test]
    public void AddTest()
    {

        for (var i = 0; i < 10; i++)
            _lazySystem.Add(i, i);

        Assert.That(_lazySystem.Count, Is.EqualTo(10));

        for (var i = 0; i < 10; i++)
            Assert.That(_lazySystem.Contains(i, i), Is.True);
    }

    [Test]
    public void RemoveTest()
    {
        for (var i = 0; i < 10; i++)
            _lazySystem.Add(i, i);

        Assert.That(_lazySystem.Contains(5, 5), Is.True);

        _lazySystem.Remove(5, 5);

        Assert.That(_lazySystem.Contains(5, 5), Is.False);
    }

    [Test]
    public void CountTest()
    {

        for (var i = 0; i < 10; i++)
            _lazySystem.Add(i,  i);

        Assert.That(_lazySystem.Count, Is.EqualTo(10));

        _lazySystem.Remove(5, 5);

        Assert.That(_lazySystem.Count, Is.EqualTo(9));
    }
}