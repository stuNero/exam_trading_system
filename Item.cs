namespace App;

class Item
{
    public string? Name;
    public string? Description;
    public User? Owner;

    public Item(string name, string description, User owner)
    {
        Name = name;
        Description = description;
        Owner = owner;
    }
    public string Info()
    {
        if (Owner != null)
        {
            return $"[Name]: '{Name}'\n[Description]:\n'{Description}'\n[Owner]: '{Owner.Name}'";
        }
        else
        {
            return $"NO OWNER FOUND FOR ITEM {Name}";
        }
    }
}