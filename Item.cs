namespace App;

/// <summary>
/// Represents an item object with name, description and an owner.
/// </summary>
class Item
{
    public string Name;
    public string Description;
    public User Owner;

    public Item(string name, string description, User owner)
    {
        Name = name;
        Description = description;
        Owner = owner;
    }
    /// <summary>
    /// Formats Item variables into readable user friendly text
    /// </summary>
    /// <returns>returns string formatted text with Item information</returns>
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