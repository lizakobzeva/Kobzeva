using System.Collections.Concurrent;
using Lab4.Core;

namespace Lab4.Tests;

public class SleepingBarberTests
{
    [Fact]
    public void BarberShouldCutHairWhenCustomerArrives()
    {
        SleepingBarber.HaircutsDone = 0;
        SleepingBarber.CustomersRejected = 0;
        SleepingBarber.waitingRoom = new ConcurrentQueue<int>();
        SleepingBarber.barberSemaphore = new SemaphoreSlim(0);
        
        var barberThread = new Thread(SleepingBarber.Barber);
        barberThread.Start();
    
        SleepingBarber.Customer(1);
        Thread.Sleep(2000);
    
        Assert.Equal(1, SleepingBarber.HaircutsDone);
    }

    [Fact]
    public void CustomersShouldLeaveWhenNoChairs()
    {
        SleepingBarber.HaircutsDone = 0;
        SleepingBarber.CustomersRejected = 0;
        SleepingBarber.waitingRoom = new ConcurrentQueue<int>();
        SleepingBarber.barberSemaphore = new SemaphoreSlim(0);
        
        var barberThread = new Thread(SleepingBarber.Barber);
        barberThread.Start();
        
        for (int i = 1; i <= 5; i++)
        {
            int id = i;
            new Thread(() => SleepingBarber.Customer(id)).Start();
        }
        Thread.Sleep(5000);

        Assert.Equal(4, SleepingBarber.HaircutsDone);
        Assert.Equal(1, SleepingBarber.CustomersRejected);
    }
}