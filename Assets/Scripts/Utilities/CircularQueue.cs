using System.Collections.Generic;
using System.Linq;

/*
 * Circular queue for when you want to iterate through a set of type T
 */
public class CircularQueue<T>
{
    public int Length => _queue.Count;

    List<T> _queue;
    int _index;

    public CircularQueue() {
        _queue = new List<T>();
        _index = 0;
    }

    public CircularQueue(IEnumerable<T> items)
    {
        _queue = new List<T>();
        _index = 0;
        Add(items);
    }

    /*
     * Returns next item in the queue
     */
    public T Next() {
        T result = Peek();
        IncrementIndex();
        return result;
    }

    /*
     * Peeks next item in the queue without iterating it
     */
    public T Peek() {
        return _queue[_index];
    }

    /*
     * Adds item to the queue
     */
    public void Add(T item) {
        _queue.Add(item);
    }

    /*
     * Adds multiple items to the queue
     */
    public void Add(IEnumerable<T> items) {
        _queue.AddRange(items);
    }

    /*
     * Removes an item from the queue
     */
    public void Remove(T item) {
        if (!_queue.Contains(item))
            return;
        int index = _queue.IndexOf(item);
        if (index >= _index) {
            DecrementIndex();
        }
        _queue.RemoveAt(index);
    }

    /*
     * Empties the queue
     */
    public void Clear() {
        _queue.Clear();
        _index = 0;
    }

    /*
     * Shuffles the queue
     */
    public void Shuffle() {
        System.Random r = new System.Random();
        _queue = _queue.OrderBy(x => r.Next()).ToList();
        _index = 0;
    }

    void IncrementIndex() {
        _index = (_index + 1) % Length;
    }

    void DecrementIndex()
    {
        _index = (_index - 1) % Length;
    }
}
