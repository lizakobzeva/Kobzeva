namespace gymBackendC__.WebApi.Models.DTO;

public class ExercisesRequestModel
{
    public int TrainingId { get; set; }
    public string Name { get; set; }
    public int Repeats { get; set; }
    public float Weight { get; set; }
    public int? TimeSec { get; set; }
}