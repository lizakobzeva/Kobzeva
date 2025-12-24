using Lab4.Core;

namespace Lab4.Tests;

public class DinningProfessorsTests
{
    [Fact]
    public void WithDeadlock()
    {
        object[] forks = new object[5];
        for (int i = 0; i < 5; i++) forks[i] = new object();
    
        DinningProfessorsWithDeadlock[] philosophers = new DinningProfessorsWithDeadlock[5];
        var threads = new Thread[5];
        for (int i = 0; i < 5; i++)
        {
            philosophers[i] = new DinningProfessorsWithDeadlock(i, forks[i], forks[(i + 1) % 5]);
            threads[i] = new Thread(philosophers[i].Start);
            threads[i].Start();
        }
        
        Thread.Sleep(5000);
        bool atLeastOneAte = false;
        int totalEatsBefore = 0;
        int totalEatsAfter =  0;
        foreach (var p in philosophers)
        {
            if (p.EatCount > 0) atLeastOneAte = true;
            totalEatsBefore += p.EatCount;
        }
        Thread.Sleep(5000);
        foreach (var p in philosophers)
        {
            totalEatsAfter += p.EatCount;
        }
        // хотя бы кто-то поел
        Assert.True(atLeastOneAte);
        // случился deadlock
        Assert.True(totalEatsBefore == totalEatsAfter);
    }
    
    [Fact]
    public void WithoutDeadlock()
    {
        object[] forks = new object[5];
        for (int i = 0; i < 5; i++) forks[i] = new object();
    
        DinningProfessorsWithoutDeadlock[] philosophers = new DinningProfessorsWithoutDeadlock[5];
        var threads = new Thread[5];
        for (int i = 0; i < 5; i++)
        {
            philosophers[i] = new DinningProfessorsWithoutDeadlock(i, forks[i], forks[(i + 1) % 5]);
            threads[i] = new Thread(philosophers[i].Start);
            threads[i].Start();
        }
        
        Thread.Sleep(5000);
        bool atLeastOneAte = false;
        int totalEatsBefore = 0;
        int totalEatsAfter =  0;
        foreach (var p in philosophers)
        {
            if (p.EatCount > 0) atLeastOneAte = true;
            totalEatsBefore += p.EatCount;
        }
        Thread.Sleep(5000);
        foreach (var p in philosophers)
        {
            totalEatsAfter += p.EatCount;
        }
        
        // хотя бы кто-то поел
        Assert.True(atLeastOneAte);
        // не случился deadlock
        Assert.True(totalEatsBefore != totalEatsAfter);
    }
}