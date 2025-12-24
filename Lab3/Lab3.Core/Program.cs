namespace Lab3.Core;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("-- SimpleList tests --");
        var list = new SimpleList();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.Add(4);
        list.RemoveAt(1);
        list.Insert(1, 5);
        Console.WriteLine("index 0 element: " + list[0]);
        Console.WriteLine("index of '5' element: " + list.IndexOf(5));
        Console.WriteLine("is contains '4' element: " + list.Contains(4));
        Console.Write("list elems: ");
        foreach (int item in list) Console.Write(item + " ");
        Console.WriteLine("\n");
        
        Console.WriteLine("-- SimpleDictionary tests --");
        var dict = new SimpleDictionary<string, int>();
        dict.Add("key1", 10);
        dict.Add("key2", 20);
        dict.Add("key3", 30);
        dict.Add("key4", 40);
        dict.Remove("key2");
        Console.WriteLine("'key1' element: " + dict["key1"]);
        Console.WriteLine("is contains 'key4' element: " + dict.ContainsKey("key4"));
        Console.Write("dict elems: ");
        foreach (var item in dict) Console.Write(item.Key + ":" + item.Value + " ");
        Console.WriteLine("\n");
        
        Console.WriteLine("-- DoublyLinkedList tests --");
        var doublyLinkedList = new DoublyLinkedList();
        doublyLinkedList.Add(1);
        doublyLinkedList.Add(2);
        doublyLinkedList.Add(3);
        doublyLinkedList.Add(4);
        doublyLinkedList.RemoveAt(1);
        doublyLinkedList.Insert(1, 5);
        Console.WriteLine("index 0 element: " + doublyLinkedList[0]);
        Console.WriteLine("index of '5' element: " + doublyLinkedList.IndexOf(5));
        Console.WriteLine("is contains '4' element: " + doublyLinkedList.Contains(4));
        Console.Write("list elems: ");
        foreach (int item in doublyLinkedList) Console.Write(item + " ");
        Console.WriteLine("\n");
    }
}