namespace Lab4.Core;

public class DinningProfessorsWithoutDeadlock
{
    private readonly int id;
    private readonly object leftFork;
    private readonly object rightFork;
    public int EatCount = 0;

    public DinningProfessorsWithoutDeadlock(int id, object leftFork, object rightFork)
    {
        this.id = id;
        this.leftFork = leftFork;
        this.rightFork = rightFork;
    }

    public void Start()
    {
        while (true)
        {
            Think();
            object first = leftFork.GetHashCode() < rightFork.GetHashCode() ? leftFork : rightFork;
            object second = leftFork.GetHashCode() > rightFork.GetHashCode() ? leftFork : rightFork;
            lock (first)
            {
                lock (second)
                {
                    Eat();
                }
            }
        }
    }

    private void Think()
    {
        Console.WriteLine($"философ {id} думает");
    }

    private void Eat()
    {
        Console.WriteLine($"философ {id} ест");
        EatCount++;
    }
}
