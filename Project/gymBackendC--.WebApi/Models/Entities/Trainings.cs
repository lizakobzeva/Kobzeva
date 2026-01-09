namespace gymBackendC__.WebApi.Models.Entities;

public class Trainings
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public ICollection<Exercises> Exercises { get; set; } = new List<Exercises>();
    public ICollection<Users> Users { get; set; } = new List<Users>();
}