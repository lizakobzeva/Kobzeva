using System.Collections.Concurrent;

namespace Lab4.Core;

public class SleepingBarber
{
    private static int WaitingRoomSeats = 3;
    private static Mutex queueMutex = new Mutex();
    private static int clientsCounter = 0;
    
    public static ConcurrentQueue<int> waitingRoom = new ConcurrentQueue<int>();
    public static SemaphoreSlim barberSemaphore = new SemaphoreSlim(0);
    private static Random random = new Random();
    public static int HaircutsDone = 0;
    public static int CustomersRejected = 0;

    public static void Barber()
    {
        while (true)
        {
            barberSemaphore.Wait();
            
            queueMutex.WaitOne();
            if (waitingRoom.TryDequeue(out int clientId))
            {
                Console.WriteLine($"парикмахер стрижет клиента {clientId}");
                HaircutsDone++;
            }
            queueMutex.ReleaseMutex();
            
            Thread.Sleep(1000);
            Console.WriteLine($"парикмахер подстриг клиента {clientId}");
        }
    }

    public static void Customer(int id)
    {
        Thread.Sleep(random.Next(100, 500));
        
        queueMutex.WaitOne();
        if (waitingRoom.Count < WaitingRoomSeats)
        {
            waitingRoom.Enqueue(id);
            Console.WriteLine($"клиент {id} сел в очередь");
            barberSemaphore.Release();
            queueMutex.ReleaseMutex();
        }
        else
        {
            queueMutex.ReleaseMutex();
            Console.WriteLine($"клиент {id} ушел из-за отсутствия стульев");
            CustomersRejected++;
        }
    }
}