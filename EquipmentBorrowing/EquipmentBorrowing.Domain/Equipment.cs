namespace EquipmentBorrowing.Domain;

public class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;

    public Equipment() { }

    public Equipment(int id, string name, bool isAvailable = true)
    {
        Id = id;
        Name = name;
        IsAvailable = isAvailable;
    }

    public void MarkAsBorrowed()
    {
        IsAvailable = false;
    }

    public void MarkAsReturned()
    {
        IsAvailable = true;
    }
}