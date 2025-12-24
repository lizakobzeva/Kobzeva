using System.Collections;

namespace Lab3.Core;

public class SimpleList : IList
{
    private object[] _array;
    private int _count;
    private int _capacity;
    private const int InitialCapacity = 4;

    public SimpleList()
    {
        _capacity = InitialCapacity;
        _array = new object[_capacity];
        _count = 0;
    }
    
    public object this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException();
            return _array[index];
        }
        set
        {
            if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException();
            _array[index] = value;
        }
    }
    
    public int Count => _count;
    public bool IsFixedSize => false;
    public bool IsReadOnly => false;
    public bool IsSynchronized => false;
    public object SyncRoot => this;

    public int Add(object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (_count == _capacity) Resize(_capacity * 2);
        _array[_count] = value;
        return _count++;
    }

    public void Clear()
    {
        Array.Clear(_array, 0, _count);
        _count = 0;
    }

    public bool Contains(object? value)
    {
        return IndexOf(value) != -1;
    }
    
    public void CopyTo(Array array, int index)
    {
        Array.Copy(_array, 0, array, index, _count);
    }

    public int IndexOf(object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        for (int i = 0; i < _count; i++)
        {
            if (Equals(_array[i], value)) return i;
        }
        return -1;
    }

    public void Insert(int index, object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (index < 0 || index > _count) throw new ArgumentOutOfRangeException(index.ToString());
        if (_count == _capacity) Resize(_capacity * 2);
        Array.Copy(_array, index, _array, index + 1, _count - index);
        _array[index] = value;
        _count++;
    }

    public void Remove(object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        int index = IndexOf(value);
        if (index == -1) throw new KeyNotFoundException(value?.ToString());
        RemoveAt(index);
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(index.ToString());
        _count--;
        Array.Copy(_array, index + 1, _array, index, _count - index);
        _array[_count] = new object();
    }

    private void Resize(int newCapacity)
    {
        object[] newArray = new object[newCapacity];
        Array.Copy(_array, newArray, _count);
        _array = newArray;
        _capacity = newCapacity;
    }

    public IEnumerator GetEnumerator()
    {
        return new SimpleListEnumerator(this);
    }

    private class SimpleListEnumerator : IEnumerator
    {
        private SimpleList _list;
        private int _index;
        private object _current;

        public SimpleListEnumerator(SimpleList list)
        {
            _list = list;
            _index = -1;
        }

        public object Current => _current;

        public bool MoveNext()
        {
            if (++_index >= _list._count) return false;
            _current = _list._array[_index];
            return true;
        }

        public void Reset()
        {
            _index = -1;
        }
    }
}
