using Lab3.Core;

namespace Lab3.Tests;

public class SimpleDictionaryTests
{
    [Fact]
    public void CorrectGetSetValueTest()
    {
        var dict = new SimpleDictionary<string, int>();
        dict["key1"] = 10;
        var value = dict["key1"];
        Assert.True(dict.ContainsKey("key1"));
        Assert.Equal(10, value);
    }
    
    [Fact]
    public void IncorrectGetSetValueTest()
    {
        var dict = new SimpleDictionary<string, int>();
        Assert.Throws<KeyNotFoundException>(() => dict["key1"]);
    }
    
    [Fact]
    public void AddTest()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("key1", 10);
        dict.Add("key2", 20);
        Assert.Equal(2, dict.Count);
        Assert.Throws<ArgumentNullException>(() => dict.Add(null, 15));
    }
    
    [Fact]
    public void ClearTest()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("key1", 10);
        dict.Add("key2", 20);
        dict.Clear();
        Assert.Empty(dict);
    }
    
    [Fact]
    public void ContainsTest()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("key1", 10);
        dict.Add("key2", 20);
        Assert.True(dict.Contains(new KeyValuePair<string, int>("key1", 10)));
        Assert.False(dict.Contains(new KeyValuePair<string, int>("key1", 20)));
    }
    
    [Fact]
    public void ContainsKeyTest()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("key1", 10);
        dict.Add("key2", 20);
        Assert.True(dict.ContainsKey("key1"));
        Assert.False(dict.ContainsKey("key3"));
    }
    
    [Fact]
    public void CopyToTest()
    {
        var dict = new SimpleDictionary<string, int>();
        var copyDict = new KeyValuePair<string, int>[2];
        dict.Add("key1", 10);
        dict.Add("key2", 20);
        dict.CopyTo(copyDict, 0);
        Assert.Equal(2, copyDict.Length);
        Assert.Contains(new KeyValuePair<string, int>("key1", 10), copyDict);
        Assert.Contains(new KeyValuePair<string, int>("key2", 20), copyDict);
    }

    [Fact]
    public void RemoveTest()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("key", 5);
        dict.Add("key2", 10);
        Assert.True(dict.Remove("key"));
        Assert.False(dict.ContainsKey("key"));
        Assert.True(dict.ContainsKey("key2"));
    }

    [Fact]
    public void TryGetValueTest()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("key", 100);
        Assert.True(dict.TryGetValue("key", out int value));
        Assert.Equal(100, value);
        Assert.False(dict.TryGetValue("missing", out _));
    }

    [Fact]
    public void EnumeratorTest()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("a", 1);
        dict.Add("b", 2);
        int sum = 0;
        foreach (var pair in dict) sum += pair.Value;
        Assert.Equal(3, sum);
    }

    [Fact]
    public void DuplicateKeyThrowsTest()
    {
        var dict = new SimpleDictionary<string, int>();
        dict.Add("key", 1);
        Assert.Throws<ArgumentException>(() => dict.Add("key", 2));
    }
}
