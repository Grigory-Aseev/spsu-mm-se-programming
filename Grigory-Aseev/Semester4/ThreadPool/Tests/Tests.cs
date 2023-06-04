using Microsoft.VisualStudio.TestPlatform.TestHost;
using ThreadPool;
using Program = ThreadPool.Program;

namespace Tests;

public class ThreadPoolTests
{
    private static readonly Random Random = new Random();

    private static readonly Action TestTask = () =>
    {
        var task = Random.Next(100);
        Thread.Sleep(Random.Next(10));
    };

    [Test]
    public void ThreadPoolTest()
    {
        var testVar = 0;

        using (var threadPool = new ThreadPool.ThreadPool(1))
        {
            threadPool.Enqueue(() => testVar += 10);
            Thread.Sleep(1);
        }

        Assert.That(testVar, Is.EqualTo(10));
    }

    [Test]
    public void NegativeThreadPoolTest()
    {
        try
        {
            using (var pool = new ThreadPool.ThreadPool(-1))
            {
                for (var i = 0; i < 10; i++)
                    pool.Enqueue(TestTask);
            }
            Assert.Fail();
        }
        catch (ArgumentOutOfRangeException e)
        {
            Assert.Pass();
        }
        catch (Exception e)
        {
            Assert.Fail();
        }
    }
    [Test]
    public void MultiThreadPoolTest()
    {
        var testVar = 0;

        using (var threadPool = new ThreadPool.ThreadPool(10))
        {
            for (var i = 0; i < 100; i++)
            {
                threadPool.Enqueue(() => testVar += 10);
                Thread.Sleep(1);
            }
        }

        Assert.That(testVar, Is.EqualTo(1000));
    }

    [Test]
    public void DisposeThreadPoolTest()
    {
        try
        {
            var pool = new ThreadPool.ThreadPool(10);

            for (var i = 0; i < 100; i++)
                pool.Enqueue(TestTask);

            pool.Dispose();
            pool.Enqueue(TestTask);
            Assert.Fail();
        }
        catch (InvalidOperationException e)
        {
            Assert.Pass();
        }
        catch (Exception e)
        {
            Assert.Fail();
        }
    }

    [Test]
    public void IsPrimeTest()
    {
        Assert.That(Program.IsPrime(1), Is.EqualTo(false));
        Assert.That(Program.IsPrime(4), Is.EqualTo(false));
        Assert.That(Program.IsPrime(7), Is.EqualTo(true));
        Assert.That(Program.IsPrime(11), Is.EqualTo(true));

    }

    [Test]
    public void CountPrimeTest()
    {
        Assert.That(Program.CountPrimes(), Is.EqualTo(168));
    }



}