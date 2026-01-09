using gymBackendC__.WebApi.Models.Entities;

namespace gymBackendC__.WebApi.Models.DTO;

public class TrainingsResponseModel
{
    public int Id { get; set; }
    public string Date { get; set; }
    public string? Notes { get; set; }
    public List<int> Users { get; set; }
}
