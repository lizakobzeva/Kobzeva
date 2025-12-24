using System.Collections.Concurrent;

namespace Lab4.Core;

public class ProducerConsumer
{
    public static BlockingCollection<int> queue = new BlockingCollection<int>(boundedCapacity: 5);

    public static void Producer()
    {
        for (int i = 1; i <= 10; i++)
        {
            queue.Add(i);
            Console.WriteLine($"произведено: {i}");
            Thread.Sleep(200);
        }
        queue.CompleteAdding();
    }

    public static void Consumer()
    {
        foreach (var item in queue.GetConsumingEnumerable())
        {
            Console.WriteLine($"потреблено: {item}");
            Thread.Sleep(300);
        }
    }
}