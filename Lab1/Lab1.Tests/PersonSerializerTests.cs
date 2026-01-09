using System.Text;
using System.Text.Json;
using Lab1.Core;

public class PersonSerializerTests : IDisposable
{
    private readonly PersonSerializer _serializer;
    private readonly string _tempDir;

    public PersonSerializerTests()
    {
        _serializer = new PersonSerializer();
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        Directory.Delete(_tempDir, true);
    }
    
    private string GetTempFilePath() => Path.Combine(_tempDir, Guid.NewGuid().ToString() + ".json");

    private Person CreateTestPerson()
    {
        return new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 30,
            Password = "secret",
            Id = "123",
            BirthDate = new DateTime(1990, 1, 1),
            Email = "john@example.com",
            PhoneNumber = "123-456"
        };
    }

    [Fact]
    public void SerializeToJsonTest()
    {
        var person = CreateTestPerson();
        var json = _serializer.SerializeToJson(person);

        Assert.Contains("\"personId\": \"123\"", json);
        Assert.Contains("\"FirstName\": \"John\"", json);
        Assert.Contains("\"LastName\": \"Doe\"", json);
        Assert.Contains("\"Age\": 30", json);
        Assert.Contains("\"_birthDate\": \"1990-01-01T00:00:00\"", json);
        Assert.Contains("\"Email\": \"john@example.com\"", json);
        Assert.Contains("\"phone\": \"123-456\"", json);
        Assert.Contains("\"FullName\": \"John Doe\"", json);
        Assert.Contains("\"IsAdult\": true", json);
        Assert.DoesNotContain("Password", json);
    }

    [Fact]
    public void CorrectDeserializeFromJsonTest()
    {
        var person = CreateTestPerson();
        var json = _serializer.SerializeToJson(person);
        var deserialized = _serializer.DeserializeFromJson(json);

        Assert.Equal(person.FirstName, deserialized.FirstName);
        Assert.Equal(person.LastName, deserialized.LastName);
        Assert.Equal(person.Age, deserialized.Age);
        Assert.Null(deserialized.Password);
        Assert.Equal(person.Id, deserialized.Id);
        Assert.Equal(person.BirthDate, deserialized.BirthDate);
        Assert.Equal(person.Email, deserialized.Email);
        Assert.Equal(person.PhoneNumber, deserialized.PhoneNumber);
        Assert.Equal(person.FullName, deserialized.FullName);
        Assert.True(deserialized.IsAdult);
    }

    [Fact]
    public void IncorrectDeserializeFromJsonTest()
    {
        var invalidJson = "{ invalid }";
        Assert.Throws<JsonException>(() => _serializer.DeserializeFromJson(invalidJson));
    }

    [Fact]
    public void CorrectSaveToFileTest()
    {
        var person = CreateTestPerson();
        var filePath = GetTempFilePath();
        _serializer.SaveToFile(person, filePath);

        var jsonFromFile = File.ReadAllText(filePath, Encoding.UTF8);
        Assert.Contains("\"personId\": \"123\"", jsonFromFile);
    }
    
    [Fact]
    public void IncorrectSaveToFileTest()
    {
        var person = CreateTestPerson();
        Assert.Throws<DirectoryNotFoundException>(() => _serializer.SaveToFile(person, "invalid:/path.json"));
    }

    [Fact]
    public void CorrectLoadFromFileTest()
    {
        var person = CreateTestPerson();
        var filePath = GetTempFilePath();
        _serializer.SaveToFile(person, filePath);

        var loaded = _serializer.LoadFromFile(filePath);
        Assert.Equal(person.Id, loaded.Id);
    }

    [Fact]
    public void IncorrectLoadFromFileTest()
    {
        var filePath = GetTempFilePath();
        Assert.Throws<FileNotFoundException>(() => _serializer.LoadFromFile(filePath));
    }

    [Fact]
    public async Task SaveToFileAsyncTest()
    {
        var person = CreateTestPerson();
        var filePath = GetTempFilePath();
        await _serializer.SaveToFileAsync(person, filePath);

        var jsonFromFile = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
        Assert.Contains("\"personId\": \"123\"", jsonFromFile);
    }

    [Fact]
    public async Task CorrectLoadFromFileAsyncTest()
    {
        var person = CreateTestPerson();
        var filePath = GetTempFilePath();
        await _serializer.SaveToFileAsync(person, filePath);

        var loaded = await _serializer.LoadFromFileAsync(filePath);
        Assert.Equal(person.Id, loaded.Id);
    }

    [Fact]
    public async Task IncorrectLoadFromFileAsyncTest()
    {
        var filePath = GetTempFilePath();
        await Assert.ThrowsAsync<FileNotFoundException>(() => _serializer.LoadFromFileAsync(filePath));
    }

    [Fact]
    public void SaveListToFileTest()
    {
        var people = new List<Person> { CreateTestPerson(), CreateTestPerson() };
        var filePath = GetTempFilePath();
        _serializer.SaveListToFile(people, filePath);

        var jsonFromFile = File.ReadAllText(filePath, Encoding.UTF8);
        Assert.StartsWith("[", jsonFromFile);
        Assert.Contains("\"personId\": \"123\"", jsonFromFile);
    }

    [Fact]
    public void LoadListFromFileTest()
    {
        var people = new List<Person> { CreateTestPerson() };
        var filePath = GetTempFilePath();
        _serializer.SaveListToFile(people, filePath);

        var loaded = _serializer.LoadListFromFile(filePath);
        Assert.Single(loaded);
        Assert.Equal(people[0].Id, loaded[0].Id);
    }

    [Fact]
    public void IncorrectLoadListFromFileTest()
    {
        var filePath = GetTempFilePath();
        Assert.Throws<FileNotFoundException>(() => _serializer.LoadListFromFile(filePath));
    }

    [Fact]
    public void IncorrectEmailSerializingTest()
    {
        var json = "{\"Email\": \"invalid\"}";
        Assert.Throws<ArgumentException>(() => _serializer.DeserializeFromJson(json));
    }
}
