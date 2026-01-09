using System.Collections.Immutable;

namespace Lab2.Tests
{
    public class CollectionTests
    {
        [Fact]
        public void ListTests()
        {
            var list = new List<int>();
            list.Add(1);
            Assert.Equal(1, list[0]);
            list.Insert(0, 0);
            Assert.Equal(0, list[0]);
            list.Insert(1, 2);
            Assert.Equal(new[] { 0, 2, 1 }, list);

            list.RemoveAt(0);
            Assert.Equal(new[] { 2, 1 }, list);
            list.RemoveAt(list.Count - 1);
            Assert.Equal(new[] { 2 }, list);
            list = new List<int> { 0, 2, 1 };
            list.RemoveAt(1);
            Assert.Equal(new[] { 0, 1 }, list);

            Assert.True(list.Contains(1)); 
            Assert.Equal(1, list[1]); 
        }

        [Fact]
        public void LinkedListTests()
        {
            var list = new LinkedList<int>();
            list.AddLast(1); 
            Assert.Equal(1, list.Last.Value);
            list.AddFirst(0); 
            Assert.Equal(0, list.First.Value);
            var node = list.Find(0);
            list.AddAfter(node, 2); 
            Assert.Equal(0, list.First.Value);
            Assert.Equal(2, list.First.Next.Value);
            Assert.Equal(1, list.Last.Value);

            list.RemoveFirst(); 
            Assert.Equal(2, list.First.Value);
            list.RemoveLast(); 
            Assert.Equal(2, list.First.Value);
            list = new LinkedList<int>(new[] { 0, 2, 1 });
            list.Remove(list.First.Next); 
            Assert.Equal(new[] { 0, 1 }, list.ToArray());

            Assert.NotNull(list.Find(1)); 
        }

        [Fact]
        public void QueueTests()
        {
            var queue = new Queue<int>();
            queue.Enqueue(0); 
            queue.Enqueue(1);
            queue.Enqueue(2);
            Assert.Equal(0, queue.Dequeue()); 
            Assert.Equal(1, queue.Peek());
            Assert.True(queue.Contains(2)); 
        }

        [Fact]
        public void StackTests()
        {
            var stack = new Stack<int>();
            stack.Push(0); 
            stack.Push(1);
            stack.Push(2);
            Assert.Equal(2, stack.Pop()); 
            Assert.Equal(1, stack.Peek());
            Assert.True(stack.Contains(0)); 
        }

        [Fact]
        public void ImmutableListTests()
        {
            var list = ImmutableList<int>.Empty;
            list = list.Add(1); 
            Assert.Equal(1, list[0]);
            list = list.Insert(0, 0); 
            Assert.Equal(0, list[0]);
            list = list.Insert(1, 2); 
            Assert.Equal(0, list[0]);
            Assert.Equal(2, list[1]);
            Assert.Equal(1, list[2]);

            list = list.RemoveAt(0); 
            Assert.Equal(2, list[0]);
            list = list.RemoveAt(list.Count - 1); 
            Assert.Equal(2, list[0]);
            list = ImmutableList.Create(0, 2, 1);
            list = list.RemoveAt(1); 
            Assert.Equal(0, list[0]);
            Assert.Equal(1, list[1]);

            Assert.True(list.IndexOf(1) >= 0); 
            Assert.Equal(1, list[1]); 
        }
    }
}
