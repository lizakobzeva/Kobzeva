namespace gymBackendC__.WebApi.Models.DTO;

public class TrainingsRequestModel
{
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public List<int> Users { get; set;  }
}