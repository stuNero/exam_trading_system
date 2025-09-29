namespace App;

class Item
{
    public string? Name;
    public string? Description;

    public User? Owner;

    public Item(string name, string description)
    {
        Name = name;
        Description = description;
    }
    public string Info()
    {
        return $"Name:\n{Name}\nDescription:\n{Description}";
    }
}