using Lab3.Core;

namespace Lab3.Tests;

public class DoublyLinkedListTests
{
    [Fact]
    public void CorrectGetSetValueTest()
    {
        var list = new DoublyLinkedList();
        list.Add(5);
        list[0] = 1;
        var value = list[0];
        Assert.Equal(1, list.Count);
        Assert.Equal(1, value);
    }
    
    [Fact]
    public void IncorrectGetSetValueTest()
    {
        var list = new DoublyLinkedList();
        Assert.Throws<ArgumentOutOfRangeException>(() => list[0]);
        Assert.Throws<ArgumentOutOfRangeException>(() => list[0] = 1);
    }
    
    [Fact]
    public void AddTest()
    {
        var list = new DoublyLinkedList();
        list.Add(1);
        list.Add(2);
        Assert.Equal(2, list.Count);
        Assert.Throws<ArgumentNullException>(() => list.Add(null));
    }
    
    [Fact]
    public void ClearTest()
    {
        var list = new DoublyLinkedList();
        list.Add(1);
        list.Clear();
        Assert.Empty(list);
    }
    
    [Fact]
    public void ContainsTest()
    {
        var list = new DoublyLinkedList();
        list.Add("test");
        Assert.True(list.Contains("test"));
        Assert.False(list.Contains("missing"));
    }
    
    [Fact]
    public void CopyToTest()
    {
        var list = new DoublyLinkedList();
        var copyList = new string[2]{"", ""};
        list.Add("test");
        list.Add("test2");
        list.CopyTo(copyList, 0);
        Assert.Equal(2, copyList.Length);
        Assert.Equal("test", copyList[0]);
        Assert.Equal("test2", copyList[1]);
    }

    [Fact]
    public void IndexOfTest()
    {
        var list = new DoublyLinkedList();
        list.Add(10);
        list.Add(20);
        Assert.Equal(1, list.IndexOf(20));
        Assert.Equal(-1, list.IndexOf(30));
    }
    
    [Fact]
    public void InsertTest()
    {
        var list = new DoublyLinkedList();
        list.Add(1);
        list.Add(3);
        list.Insert(1, 2);
        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void RemoveTest()
    {
        var list = new DoublyLinkedList();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.Remove(2);
        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
    }
    
    [Fact]
    public void RemoveAtTest()
    {
        var list = new DoublyLinkedList();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.RemoveAt(1);
        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
    }

    [Fact]
    public void EnumeratorTest()
    {
        var list = new DoublyLinkedList();
        list.Add(1);
        list.Add(2);
        int sum = 0;
        foreach (int item in list) sum += item;
        Assert.Equal(3, sum);
    }
}
