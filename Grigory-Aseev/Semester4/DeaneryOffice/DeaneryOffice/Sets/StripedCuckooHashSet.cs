namespace DeaneryOffice.Sets;

public class StripedCuckooHashSet<T> : PhasedCuckooHashSet<T>
{
    Mutex[,] locks;

    public StripedCuckooHashSet(int capacity) : base(capacity)
    {
        locks = new Mutex[2, capacity];
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < capacity; j++)
            {
                locks[i, j] = new Mutex();
            }
        }
    }


    protected override void Acquire(T x)
    {
        // lock the appropriate mutex
        locks[0, Hash0(x) % locks.GetLength(1)].WaitOne();
        locks[1, Hash1(x) % locks.GetLength(1)].WaitOne();
    }

    protected override void Release(T x)
    {
        // release the appropriate mutex
        locks[0, Hash0(x) % locks.GetLength(1)].ReleaseMutex();
        locks[1, Hash1(x) % locks.GetLength(1)].ReleaseMutex();
    }

    protected override void Resize()
    {
        int oldCapacity = capacity;
        // all mutexes are locked at index 0
        // there is no sense more than zero, since we take and release at the same time
        for (int i = 0; i < locks.GetLength(1); i++)
        {
            locks[0, i].WaitOne();
        }

        try
        {
            // if we were thwarted
            if (capacity != oldCapacity)
            {
                return;
            }

            // resize the table

            List<T>[,] oldTable = table;
            capacity = 2 * capacity;
            table = new List<T>[2, capacity];

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < capacity; j++)
                {
                    table[i, j] = new List<T>(LIST_SIZE);
                }
            }

            // For each element, honestly call the add method
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < oldCapacity; j++)
                {
                    foreach (T z in oldTable[i, j])
                    {
                        Add(z);
                    }
                }
            }
        }
        finally
        {
            // release mutexes
            for (int i = 0; i < locks.GetLength(1); i++)
            {
                locks[0, i].ReleaseMutex();
            }
        }
    }
}