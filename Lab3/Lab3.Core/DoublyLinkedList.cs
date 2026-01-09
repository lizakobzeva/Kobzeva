using System.Collections;

namespace Lab3.Core;

public class DoublyLinkedList : IList
{
    private class Node
    {
        public object Value { get; set; }
        public Node Previous { get; set; }
        public Node Next { get; set; }
    }

    private Node _head;
    private Node _tail;
    private int _count;
    
    public object this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException();
            return GetNodeAt(index).Value;
        }
        set
        {
            if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException();
            GetNodeAt(index).Value = value;
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
        var newNode = new Node { Value = value };
        if (_count == 0)
        {
            _head = _tail = newNode;
        }
        else
        {
            _tail.Next = newNode;
            newNode.Previous = _tail;
            _tail = newNode;
        }
        return _count++;
    }

    public void Clear()
    {
        _head = _tail = null;
        _count = 0;
    }

    public bool Contains(object? value)
    {
        return IndexOf(value) != -1;
    }
    
    public void CopyTo(Array array, int index)
    {
        Node current = _head;
        int i = index;
        while (current != null)
        {
            array.SetValue(current.Value, i++);
            current = current.Next;
        }
    }

    public int IndexOf(object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        int index = 0;
        Node current = _head;
        while (current != null)
        {
            if (Equals(current.Value, value)) return index;
            current = current.Next;
            index++;
        }
        return -1;
    }

    public void Insert(int index, object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (index < 0 || index > _count) throw new ArgumentOutOfRangeException(index.ToString());
        var newNode = new Node { Value = value };
        if (index == 0)
        {
            if (_count == 0)
            {
                _head = _tail = newNode;
            }
            else
            {
                newNode.Next = _head;
                _head.Previous = newNode;
                _head = newNode;
            }
        }
        else if (index == _count)
        {
            _tail.Next = newNode;
            newNode.Previous = _tail;
            _tail = newNode;
        }
        else
        {
            Node current = GetNodeAt(index);
            newNode.Previous = current.Previous;
            newNode.Next = current;
            current.Previous.Next = newNode;
            current.Previous = newNode;
        }
        _count++;
    }

    public void Remove(object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        int index = IndexOf(value);
        RemoveAt(index);
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(index.ToString());
        Node node = GetNodeAt(index);

        if (node.Previous != null && node.Next != null)
        {
            node.Previous.Next = node.Next;
            node.Next.Previous = node.Previous;
        }
        if (node == _head) _head = node.Next;
        if (node == _tail) _tail = node.Previous;
        _count--;
    }

    private Node GetNodeAt(int index)
    {
        Node current = _head;
        for (int i = 0; i < index; i++)
        {
            current = current.Next;
        }
        return current;
    }

    public IEnumerator GetEnumerator()
    {
        return new DoublyLinkedListEnumerator(_head);
    }

    private class DoublyLinkedListEnumerator : IEnumerator
    {
        private Node _currentNode;
        private Node _start;

        public DoublyLinkedListEnumerator(Node head)
        {
            _start = head;
            _currentNode = null;
        }

        public object Current => _currentNode?.Value;

        public bool MoveNext()
        {
            if (_currentNode == null)
            {
                _currentNode = _start;
            }
            else
            {
                _currentNode = _currentNode.Next;
            }
            return _currentNode != null;
        }

        public void Reset()
        {
            _currentNode = null;
        }
    }
}
