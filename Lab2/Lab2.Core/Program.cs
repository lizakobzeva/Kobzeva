using System.Collections.Immutable;
using System.Diagnostics;

namespace Lab2.Core;

class Program
{
    const int Size = 100001;
    const int Runs = 7;

    static void Main(string[] args)
    {
        Console.WriteLine("-- Замер производительности --");
        MeasureList();
        MeasureLinkedList();
        MeasureQueue();
        MeasureStack();
        MeasureImmutableList();
    }

    static void MeasureList()
    {
        Console.WriteLine("\nList<int>");
        var times = new Dictionary<string, List<long>>();
        for (int run = 0; run < Runs; run++)
        {
            var list = Enumerable.Range(0, Size).ToList();
            var sw = Stopwatch.StartNew();
            list.Add(Size);
            sw.Stop();
            AddTime(times, "Add в конец", sw.ElapsedTicks);

            list = Enumerable.Range(0, Size).ToList();
            sw = Stopwatch.StartNew();
            list.Insert(0, -1);
            sw.Stop();
            AddTime(times, "Add в начало", sw.ElapsedTicks);

            list = Enumerable.Range(0, Size).ToList();
            sw = Stopwatch.StartNew();
            list.Insert(Size / 2, -1);
            sw.Stop();
            AddTime(times, "Add в середину", sw.ElapsedTicks);

            list = Enumerable.Range(0, Size).ToList();
            sw = Stopwatch.StartNew();
            list.RemoveAt(0);
            sw.Stop();
            AddTime(times, "Remove из начала", sw.ElapsedTicks);

            list = Enumerable.Range(0, Size).ToList();
            sw = Stopwatch.StartNew();
            list.RemoveAt(list.Count - 1);
            sw.Stop();
            AddTime(times, "Remove из конца", sw.ElapsedTicks);

            list = Enumerable.Range(0, Size).ToList();
            sw = Stopwatch.StartNew();
            list.RemoveAt(list.Count / 2);
            sw.Stop();
            AddTime(times, "Remove из середины", sw.ElapsedTicks);

            list = Enumerable.Range(0, Size).ToList();
            sw = Stopwatch.StartNew();
            list.IndexOf(Size / 2);
            sw.Stop();
            AddTime(times, "Поиск", sw.ElapsedTicks);

            list = Enumerable.Range(0, Size).ToList();
            sw = Stopwatch.StartNew();
            var x = list[Size / 2];
            sw.Stop();
            AddTime(times, "Получение по индексу", sw.ElapsedTicks);
        }
        PrintAverage(times);
    }

    static void MeasureLinkedList()
    {
        Console.WriteLine("\nLinkedList<int>");
        var times = new Dictionary<string, List<long>>();
        for (int run = 0; run < Runs; run++)
        {
            var list = new LinkedList<int>(Enumerable.Range(0, Size));
            var sw = Stopwatch.StartNew();
            list.AddLast(Size);
            sw.Stop();
            AddTime(times, "Add в конец", sw.ElapsedTicks);

            list = new LinkedList<int>(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            list.AddFirst(-1);
            sw.Stop();
            AddTime(times, "Add в начало", sw.ElapsedTicks);

            list = new LinkedList<int>(Enumerable.Range(0, Size));
            var middleNode = GetNodeAt(list, Size / 2);
            sw = Stopwatch.StartNew();
            list.AddBefore(middleNode, -1);
            sw.Stop();
            AddTime(times, "Add в середину", sw.ElapsedTicks);

            list = new LinkedList<int>(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            list.RemoveFirst();
            sw.Stop();
            AddTime(times, "Remove из начала", sw.ElapsedTicks);

            list = new LinkedList<int>(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            list.RemoveLast();
            sw.Stop();
            AddTime(times, "Remove из конца", sw.ElapsedTicks);

            list = new LinkedList<int>(Enumerable.Range(0, Size));
            middleNode = GetNodeAt(list, Size / 2);
            sw = Stopwatch.StartNew();
            list.Remove(middleNode);
            sw.Stop();
            AddTime(times, "Remove из середины", sw.ElapsedTicks);

            list = new LinkedList<int>(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            list.Find(Size / 2);
            sw.Stop();
            AddTime(times, "Поиск", sw.ElapsedTicks);

            list = new LinkedList<int>(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            GetNodeAt(list, Size / 2);
            sw.Stop();
            AddTime(times, "Получение по индексу (симулировано)", sw.ElapsedTicks);
        }
        PrintAverage(times);
    }

    static LinkedListNode<int> GetNodeAt(LinkedList<int> list, int index)
    {
        var node = list.First;
        for (int i = 0; i < index; i++)
        {
            node = node.Next;
        }
        return node;
    }

    static void MeasureQueue()
    {
        Console.WriteLine("\nQueue<int>");
        var times = new Dictionary<string, List<long>>();
        for (int run = 0; run < Runs; run++)
        {
            var queue = new Queue<int>(Enumerable.Range(0, Size));
            var sw = Stopwatch.StartNew();
            queue.Enqueue(Size);
            sw.Stop();
            AddTime(times, "Add в конец", sw.ElapsedTicks);

            queue = new Queue<int>(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            queue.Dequeue();
            sw.Stop();
            AddTime(times, "Remove из начала", sw.ElapsedTicks);

            queue = new Queue<int>(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            queue.Contains(Size / 2);
            sw.Stop();
            AddTime(times, "Поиск", sw.ElapsedTicks);
        }
        PrintAverage(times);
    }

    static void MeasureStack()
    {
        Console.WriteLine("\nStack<int>");
        var times = new Dictionary<string, List<long>>();
        for (int run = 0; run < Runs; run++)
        {
            var stack = new Stack<int>(Enumerable.Range(0, Size).Reverse());
            var sw = Stopwatch.StartNew();
            stack.Push(Size);
            sw.Stop();
            AddTime(times, "Add в начало", sw.ElapsedTicks);

            stack = new Stack<int>(Enumerable.Range(0, Size).Reverse());
            sw = Stopwatch.StartNew();
            stack.Pop();
            sw.Stop();
            AddTime(times, "Remove из начала", sw.ElapsedTicks);

            stack = new Stack<int>(Enumerable.Range(0, Size).Reverse());
            sw = Stopwatch.StartNew();
            stack.Contains(Size / 2);
            sw.Stop();
            AddTime(times, "Поиск", sw.ElapsedTicks);
        }
        PrintAverage(times);
    }

    static void MeasureImmutableList()
    {
        Console.WriteLine("\nImmutableList<int>");
        var times = new Dictionary<string, List<long>>();
        for (int run = 0; run < Runs; run++)
        {
            var list = ImmutableList.CreateRange(Enumerable.Range(0, Size));
            var sw = Stopwatch.StartNew();
            list.Add(Size);
            sw.Stop();
            AddTime(times, "Add в конец", sw.ElapsedTicks);

            list = ImmutableList.CreateRange(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            list.Insert(0, -1);
            sw.Stop();
            AddTime(times, "Add в начало", sw.ElapsedTicks);

            list = ImmutableList.CreateRange(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            list.Insert(Size / 2, -1);
            sw.Stop();
            AddTime(times, "Add в середину", sw.ElapsedTicks);

            list = ImmutableList.CreateRange(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            list.RemoveAt(0);
            sw.Stop();
            AddTime(times, "Remove из начала", sw.ElapsedTicks);

            list = ImmutableList.CreateRange(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            list.RemoveAt(Size - 1);
            sw.Stop();
            AddTime(times, "Remove из конца", sw.ElapsedTicks);

            list = ImmutableList.CreateRange(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            list.RemoveAt(Size / 2);
            sw.Stop();
            AddTime(times, "Remove из середины", sw.ElapsedTicks);

            list = ImmutableList.CreateRange(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            list.IndexOf(Size / 2);
            sw.Stop();
            AddTime(times, "Поиск", sw.ElapsedTicks);

            list = ImmutableList.CreateRange(Enumerable.Range(0, Size));
            sw = Stopwatch.StartNew();
            var x = list[Size / 2];
            sw.Stop();
            AddTime(times, "Получение по индексу", sw.ElapsedTicks);
        }
        PrintAverage(times);
    }

    static void AddTime(Dictionary<string, List<long>> times, string key, long time)
    {
        if (!times.ContainsKey(key))
            times[key] = new List<long>();
        times[key].Add(time);
    }

    static void PrintAverage(Dictionary<string, List<long>> times)
    {
        foreach (var kv in times)
        {
            var avg = kv.Value.Average();
            Console.WriteLine($"{kv.Key}: {avg:F2} тиков");
        }
    }
}
