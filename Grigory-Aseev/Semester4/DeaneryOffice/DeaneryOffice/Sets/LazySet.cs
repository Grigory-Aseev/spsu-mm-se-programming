namespace DeaneryOffice.Sets;

public class LazySet<T>
{
    private Node<T> _tail = new Node<T>(int.MaxValue);
    private Node<T> _head = new Node<T>(int.MinValue);

    public int Count { get; private set; }

    public LazySet()
    {
        _head.Next = _tail;
    }

    /*
        Now we check that both nodes are not logically remote
        And we check that the first refers to the second.
        This approach allows you to very advantageously increase the speed.
    */

    private bool Validate(Node<T> pred, Node<T> curr)
    {
        return !pred.Marked && !curr.Marked && pred.Next == curr;
    }

    public bool Add(T item)
    {
        int key = item.GetHashCode();
        while (true)
        {
            Node<T> pred = _head;
            Node<T> curr = _head.Next;
            while (curr.Key < key)
            {
                pred = curr;
                curr = curr.Next;
            }

            pred.Lock.Lock();
            try
            {
                curr.Lock.Lock();
                try
                {
                    if (Validate(pred, curr))
                    {
                        if (curr.Key == key)
                        {
                            return false;
                        }
                        else
                        {
                            Node<T> node = new Node<T>(item) {Next = curr};
                            pred.Next = node;
                            Count++;
                            return true;
                        }
                    }
                }
                finally
                {
                    curr.Lock.Unlock();
                }
            }
            finally
            {
                pred.Lock.Unlock();
            }
        }
    }

    public bool Remove(T item)
    {
        int key = item.GetHashCode();
        while (true)
        {
            Node<T> pred = _head;
            Node<T> curr = _head.Next;
            while (curr.Key < key)
            {
                pred = curr;
                curr = curr.Next;
            }

            pred.Lock.Lock();
            try
            {
                curr.Lock.Lock();
                try
                {
                    if (Validate(pred, curr))
                    {
                        if (curr.Key != key)
                        {
                            return false;
                        }
                        else
                        {
                            // initially curr.Marked = false
                            // deletion does not happen immediately. First there is a logical deletion (marked becomes)
                            // and physical removal occurs later
                            curr.Marked = true;
                            pred.Next = curr.Next;
                            Count--;
                            return true;
                        }
                    }
                }
                finally
                {
                    curr.Lock.Unlock();
                }
            }
            finally
            {
                pred.Lock.Unlock();
            }
        }
    }

    /*There are no blocks at all*/
    public bool Сontains(T item)
    {
        int key = item.GetHashCode();
        Node<T> curr = _head;
        while (curr.Key < key)
            curr = curr.Next;

        // be sure to check that the node is not deleted
        return curr.Key == key && !curr.Marked;
    }
}