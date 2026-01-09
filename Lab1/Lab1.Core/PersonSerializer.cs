using System.Text;
using System.Text.Json;

namespace Lab1.Core;

public class PersonSerializer
{
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
    private readonly object _lock = new();

    public string SerializeToJson(Person person)
    {
        return JsonSerializer.Serialize(person, _options);
    }

    public Person DeserializeFromJson(string json)
    {
        return JsonSerializer.Deserialize<Person>(json, _options);
    }

    public void SaveToFile(Person person, string filePath)
    {
        lock (_lock)
        {
            try
            {
                string json = SerializeToJson(person);
                File.WriteAllText(filePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }
    }

    public Person LoadFromFile(string filePath)
    {
        lock (_lock)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"файл не найден: {filePath}");
                string json = File.ReadAllText(filePath, Encoding.UTF8);
                return DeserializeFromJson(json);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }
    }

    public async Task SaveToFileAsync(Person person, string filePath)
    {
        try
        {
            string json = SerializeToJson(person);
            await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    public async Task<Person> LoadFromFileAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");
            string json = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
            return DeserializeFromJson(json);
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    public void SaveListToFile(List<Person> people, string filePath)
    {
        lock (_lock)
        {
            try
            {
                string json = JsonSerializer.Serialize(people, _options);
                File.WriteAllText(filePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }
    }

    public List<Person> LoadListFromFile(string filePath)
    {
        lock (_lock)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"файл не найден: {filePath}");
                string json = File.ReadAllText(filePath, Encoding.UTF8);
                return JsonSerializer.Deserialize<List<Person>>(json, _options);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }
    }

    private void LogError(Exception ex)
    {
        try
        {
            File.AppendAllText("error.log", $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n");
        }
        catch { /* пропуск ошибок при логировании */ }
    }
}
