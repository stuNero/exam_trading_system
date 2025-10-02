namespace App;

class Item
{
    public string? Name;
    public string? Description;
    public User? Owner;
    public bool Available;

    public Item(string name, string description, User owner)
    {
        Name = name;
        Description = description;
        Owner = owner;
        Available = true;
    }
    public string Info()
    {
        return $"[Name]: '{Name}'\n[Description]:\n'{Description}'\n[Owner]: '{Owner.Name}'";
    }
}