namespace UseMiddleware;

public class Item
{
    public string Name { get; }

    public Item(string name)
    {
        if (name.Contains("Test"))
            throw new ArgumentException("Name can't contain the string 'Test' in it!");

        Name = name;
    }
}

