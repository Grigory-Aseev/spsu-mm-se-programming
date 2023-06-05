namespace DeaneryOffice.Sets;
using System.Security.Cryptography;

public abstract class PhasedCuckooHashSet<T>
{
	// list is semi-full
	protected const int THRESHOLD = 2;

	// list is full
	protected const int LIST_SIZE = 4;

	// steps to relocate
	protected const int LIMIT = 5;

	volatile protected int capacity;
	volatile protected List<T>[,] table;

	public PhasedCuckooHashSet(int size)
	{
		capacity = size;
		table = new List<T>[2, capacity];

		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < capacity; j++)
			{
				table[i, j] = new List<T>(LIST_SIZE);
			}
		}
	}



	// SHA256 hash method
	protected int Hash0(T i)
    {
        var input = i.GetHashCode();
        var bytes = SHA256.HashData(BitConverter.GetBytes(input));

        var hashValue = BitConverter.ToInt32(bytes, 0);
        return Math.Abs(hashValue);
    }

	// MD5 hash method

    protected int Hash1(T i)
    {
        var input = i.GetHashCode();
        var bytes = MD5.HashData(BitConverter.GetBytes(input));

        var hashValue = BitConverter.ToInt32(bytes, 0);
        return Math.Abs(hashValue);
    }

    public bool Contains(T x)
	{
		Acquire(x);
		try
		{
			// check both arrays to see if it exists in at least one of them
			List<T> set0 = table[0, Hash0(x) % capacity];
			if (set0.Contains(x))
			{
				return true;
			}
			else
			{
				List<T> set1 = table[1, Hash1(x) % capacity];
				if (set1.Contains(x))
				{
					return true;
				}
			}

			return false;
		}
		finally
		{
			Release(x);
		}
	}

	protected abstract void Acquire(T x);
	protected abstract void Release(T x);
	protected abstract void Resize();

	public bool Remove(T x)
	{
		Acquire(x);
		try
		{
			// look for an element in one of the arrays and remove
			List<T> set0 = table[0, Hash0(x) % capacity];
			if (set0.Contains(x))
			{
				set0.Remove(x);
				return true;
			}
			else
			{
				List<T> set1 = table[1, Hash1(x) % capacity];
				if (set1.Contains(x))
				{
					set1.Remove(x);
					return true;
				}
			}

			return false;
		}
		finally
		{
			Release(x);
		}
	}

	public bool Add(T x)
	{
		Acquire(x);
		int h0 = Hash0(x) % capacity, h1 = Hash1(x) % capacity;
		int i = -1, h = -1;
		bool mustResize = false;
		try
		{
			// first we try to add an element to the zero array
			// if present, do not add
			if (Contains(x)) return false;
			List<T> set0 = table[0, h0];
			List<T> set1 = table[1, h1];
			// trying to add where there are fewer elements
			if (set0.Count < THRESHOLD)
			{
				set0.Add(x);
				return true;
			}
			else if (set1.Count < THRESHOLD)
			{
				set1.Add(x);
				return true;
			}
			// if someone is almost full, then we will try to throw the elements
			else if (set0.Count < LIST_SIZE)
			{
				set0.Add(x);
				i = 0;
				h = h0;
			}
			else if (set1.Count < LIST_SIZE)
			{
				set1.Add(x);
				i = 1;
				h = h1;
			}
			// can't throw it away
			else
			{
				mustResize = true;
			}
		}
		finally
		{
			Release(x);
		}

		if (mustResize)
		{
			Resize();
			Add(x);
		}
		else if (!Relocate(i, h))
		{
			// if not, then you need to completely resize
			Resize();
		}

		return true; // x must have been present
	}


	//  we are trying to unload one array in this method
	protected bool Relocate(int i, int hi)
	{
		int hj = 0;
		int j = 1 - i;
		// limiting transfers by the number of operations
		for (int round = 0; round < LIMIT; round++)
		{
			List<T> iSet = table[i, hi];
			T y = iSet[0];
			switch (i)
			{
				case 0:
					hj = Hash1(y) % capacity;
					break;
				case 1:
					hj = Hash0(y) % capacity;
					break;
			}

			Acquire(y);
			List<T> jSet = table[j, hj];
			// determine from where and where we will transfer
			try
			{
				// succeeded in deleting the element
				if (iSet.Remove(y))
				{
					// if there are few elements, then we throw the element
					if (jSet.Count < THRESHOLD)
					{
						jSet.Add(y);
						return true;
					}
					//  if we unloaded our array but loaded another one, then try to unload another vector
					else if (jSet.Count < LIST_SIZE)
					{
						jSet.Add(y);
						i = 1 - i;
						hi = hj;
						j = 1 - j;
					}
					// can't transfer, return back
					else
					{
						iSet.Add(y);
						return false;
					}
				}
				//  if it has more elements, then go with a new element
				else if (iSet.Count >= THRESHOLD)
				{
					continue;
				}
				// if unloaded, then everything is cool
				else
				{
					return true;
				}
			}
			finally
			{
				Release(y);
			}
		}

		return false;
	}
}