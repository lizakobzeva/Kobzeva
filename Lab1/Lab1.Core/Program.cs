namespace Lab1.Core;

class Program
{
    static void Main(string[] args)
    {
        // Тест Person и Serializer
        Person person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 30,
            Password = "secret",
            Id = "123",
            BirthDate = DateTime.Now,
            Email = "john@example.com",
            PhoneNumber = "123-456-7890"
        };

        PersonSerializer serializer = new PersonSerializer();

        string json = serializer.SerializeToJson(person);
        Console.WriteLine("Сериализованный JSON:");
        Console.WriteLine(json);

        string filePath = "person.json";
        serializer.SaveToFile(person, filePath);
        Console.WriteLine("\nPerson сохранен в файл.");

        Person loadedPerson = serializer.LoadFromFile(filePath);
        Console.WriteLine("\nЗагруженный Person:");
        Console.WriteLine($"FullName: {loadedPerson.FullName}, Age: {loadedPerson.Age}, IsAdult: {loadedPerson.IsAdult}");

        // Тест списка
        List<Person> people = new List<Person> { person, new Person { FirstName = "Jane", LastName = "Smith", Age = 25, Id = "456", BirthDate = DateTime.Now.AddYears(-25), Email = "jane@example.com", PhoneNumber = "987-654-3210" } };
        string listFilePath = "people.json";
        serializer.SaveListToFile(people, listFilePath);
        List<Person> loadedPeople = serializer.LoadListFromFile(listFilePath);
        Console.WriteLine("\nКоличество загруженных людей: " + loadedPeople.Count);

        // Тест асинхронных методов (пример в синхронном Main)
        // serializer.SaveToFileAsync(person, "async_person.json").Wait();

        // Тест FileResourceManager
        string testFile = "test.txt";
        using (FileResourceManager manager = new FileResourceManager(testFile, FileMode.Create))
        {
            manager.OpenForWriting();
            manager.WriteLine("Привет, мир!");
            manager.AppendText(" Дополнительный текст.");
            
            manager.OpenForReading();
            Console.WriteLine("\nИнформация о файле:");
            Console.WriteLine(manager.GetFileInfo());
            Console.WriteLine("\nСодержимое файла:");
            Console.WriteLine(manager.ReadAllText());
        }
    }
}
