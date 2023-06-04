using System.Diagnostics;
using System.Reflection;
using ProducerConsumer;

namespace Tests;

using NUnit.Framework;

public class Tests
{
    private Random? _random;

    [SetUp]
    public void Setup()
    {
        _random = new Random();
    }

    [Test]
    public void ManagerTest()
    {
        var manager = new Manager<int>(5, 5, 10, () =>
        {
            Debug.Assert(_random != null, nameof(_random) + " != null");
            return _random.Next(1, 100000);
        });
        var t = typeof(Manager<int>);
        t.InvokeMember("LaunchAgents",
            BindingFlags.InvokeMethod | BindingFlags.NonPublic |
            BindingFlags.Public | BindingFlags.Instance,
            null, manager, null);
        Thread.Sleep(3000);
        t.InvokeMember("StopAgents",
            BindingFlags.InvokeMethod | BindingFlags.NonPublic |
            BindingFlags.Public | BindingFlags.Instance,
            null, manager, null);

        Assert.Pass();
    }

    [Test]
    public void ProducerTest()
    {
        var semaphore = new Semaphore(1, 1);
        var producer = new Producer<int>(new List<int>(), 4, semaphore, () => 10);
        var t = typeof(Producer<int>);
        t.InvokeMember("Act",
            BindingFlags.InvokeMethod | BindingFlags.NonPublic |
            BindingFlags.Public | BindingFlags.Instance,
            null, producer, null);

        var f = t.GetField("Objects", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (f?.GetValue(producer) is List<int> objects)
        {
            Assert.Multiple(() =>
            {
                Assert.That(objects[0], Is.EqualTo(10));
                Assert.That(objects, Has.Count.EqualTo(1));
            });
        }
        else
        {
            Assert.Fail();
        }
    }


    [Test]
    public void ConsumerTest()
    {
        var semaphore = new Semaphore(1, 1);
        var consumer = new Consumer<int>(new List<int>(), 4, semaphore);
        var t = typeof(Consumer<int>);


        var f = t.GetField("Objects", BindingFlags.NonPublic | BindingFlags.Instance);

        f?.SetValue(consumer, new List<int>() { 10 });

        if (f?.GetValue(consumer) is List<int> objects)
        {
            Assert.Multiple(() =>
            {
                Assert.That(objects[0], Is.EqualTo(10));
                Assert.That(objects, Has.Count.EqualTo(1));
            });
        }
        else
        {
            Assert.Fail();
        }

        t.InvokeMember("Act",
            BindingFlags.InvokeMethod | BindingFlags.NonPublic |
            BindingFlags.Public | BindingFlags.Instance,
            null, consumer, null);

        if (f?.GetValue(consumer) is List<int> objectsAfter)
        {
            Assert.Multiple(() => { Assert.That(objectsAfter, Has.Count.EqualTo(0)); });
        }
        else
        {
            Assert.Fail();
        }
    }
}