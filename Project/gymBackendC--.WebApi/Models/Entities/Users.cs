namespace gymBackendC__.WebApi.Models.Entities;

public class Users : IEquatable<Users>
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Trainings> Trainings { get; set; } = new List<Trainings>();
    
    public bool Equals(Users? other)
    {
        if (other is null) return false;
        return Id == other.Id && Username == other.Username && Email == other.Email
            && Password == other.Password && Role == other.Role;
    }

    public override bool Equals(object? obj) => Equals(obj as Users);

    public override int GetHashCode() => HashCode.Combine(Id, Username);
}
