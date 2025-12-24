using System.Collections;

namespace Lab3.Core;

public class SimpleDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
{
    private List<KeyValuePair<TKey, TValue>>[] _buckets;
    private int _count;
    private const int InitialCapacity = 16;
    private const float LoadFactor = 0.75f;

    public SimpleDictionary()
    {
        _buckets = new List<KeyValuePair<TKey, TValue>>[InitialCapacity];
        for (int i = 0; i < InitialCapacity; i++)
        {
            _buckets[i] = new List<KeyValuePair<TKey, TValue>>();
        }
    }
    
    public TValue this[TKey key]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(key);
            if (TryGetValue(key, out TValue value)) return value;
            throw new KeyNotFoundException(key.ToString());
        }
        set
        {
            int index = GetBucketIndex(key);
            for (int i = 0; i < _buckets[index].Count; i++)
            {
                if (EqualityComparer<TKey>.Default.Equals(_buckets[index][i].Key, key))
                {
                    _buckets[index][i] = new KeyValuePair<TKey, TValue>(key, value);
                    return;
                }
            }
            Add(key, value);
        }
    }
    
    public int Count => _count;
    public bool IsReadOnly => false;
    
    public ICollection<TKey> Keys => GetKeys();
    public ICollection<TValue> Values => GetValues();
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => GetKeys();
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => GetValues();

    private List<TKey> GetKeys()
    {
        var keys = new List<TKey>(_count);
        foreach (var bucket in _buckets)
        {
            foreach (var pair in bucket) keys.Add(pair.Key);
        }
        return keys;
    }

    private List<TValue> GetValues()
    {
        var values = new List<TValue>(_count);
        foreach (var bucket in _buckets)
        {
            foreach (var pair in bucket) values.Add(pair.Value);
        }
        return values;
    }
    
    private int GetBucketIndex(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);
        // & 0x7FFFFFFF - убираем знак, т.к. индекс не может быть отрицательный
        return (key.GetHashCode() & 0x7FFFFFFF) % _buckets.Length;
    }

    public void Add(TKey key, TValue value)
    {
        if (ContainsKey(key)) throw new ArgumentException("key already exists");
        int index = GetBucketIndex(key);
        _buckets[index].Add(new KeyValuePair<TKey, TValue>(key, value));
        _count++;
        if (_count > _buckets.Length * LoadFactor) Resize();
    }
    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    public void Clear()
    {
        foreach (var bucket in _buckets) bucket.Clear();
        _count = 0;
    }
    
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (TryGetValue(item.Key, out TValue value))
            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        return false;
    }
    
    public bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out TValue value);
    }
    
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        int i = arrayIndex;
        foreach (var bucket in _buckets)
        {
            foreach (var pair in bucket) array[i++] = pair;
        }
    }

    public bool Remove(TKey key)
    {
        int index = GetBucketIndex(key);
        for (int i = 0; i < _buckets[index].Count; i++)
        {
            if (EqualityComparer<TKey>.Default.Equals(_buckets[index][i].Key, key))
            {
                _buckets[index].RemoveAt(i);
                _count--;
                return true;
            }
        }
        return false;
    }
    
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (TryGetValue(item.Key, out TValue value) && EqualityComparer<TValue>.Default.Equals(value, item.Value))
            return Remove(item.Key);
        return false;
    }
    
    private void Resize()
    {
        int newCapacity = _buckets.Length * 2;
        var newBuckets = new List<KeyValuePair<TKey, TValue>>[newCapacity];
        for (int i = 0; i < newCapacity; i++)
        {
            newBuckets[i] = new List<KeyValuePair<TKey, TValue>>();
        }

        foreach (var bucket in _buckets)
        {
            foreach (var pair in bucket)
            {
                int newIndex = (pair.Key.GetHashCode() & 0x7FFFFFFF) % newCapacity;
                newBuckets[newIndex].Add(pair);
            }
        }

        _buckets = newBuckets;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int index = GetBucketIndex(key);
        foreach (var pair in _buckets[index])
        {
            if (EqualityComparer<TKey>.Default.Equals(pair.Key, key))
            {
                value = pair.Value;
                return true;
            }
        }
        value = default;
        return false;
    }
    
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var bucket in _buckets)
        {
            foreach (var pair in bucket) yield return pair;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
