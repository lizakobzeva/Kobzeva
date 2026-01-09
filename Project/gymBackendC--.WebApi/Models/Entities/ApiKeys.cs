namespace gymBackendC__.WebApi.Models.Entities;

public class ApiKeys
{
    public int Id { get; set; }
    public string Key { get; set; }
    public DateTime Expiration { get; set; }
    public bool IsActive { get; set; }
}