using Lab4.Core;

namespace Lab4.Tests;

public class ProducerConsumerTests
{
    [Fact]
    public void Test()
    {
        var producers = new Thread[5];
        var consumers = new Thread[5];
        for (var i = 0; i < 5; i++)
        {
            producers[i] = new Thread(ProducerConsumer.Producer);
            producers[i].Start();
        }
        for (var i = 0; i < 5; i++)
        {
            consumers[i] = new Thread(ProducerConsumer.Consumer);
            consumers[i].Start();
        }
        for (var i = 0; i < 5; i++) producers[i].Join();
        for (var i = 0; i < 5; i++) consumers[i].Join();

        Assert.Equal(0, ProducerConsumer.queue.Count);
    }
}
