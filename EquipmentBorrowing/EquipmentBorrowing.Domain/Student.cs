namespace EquipmentBorrowing.Domain;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsEligibleToBorrow { get; set; }
    public int MaxBorrowLimit { get; set; }

    public Student() { }

    public Student(int id, string name, bool isEligibleToBorrow = true, int maxBorrowLimit = 3)
    {
        Id = id;
        Name = name;
        IsEligibleToBorrow = isEligibleToBorrow;
        MaxBorrowLimit = maxBorrowLimit;
    }

    public bool CanBorrow(int currentActiveBorrowings)
    {
        return IsEligibleToBorrow && currentActiveBorrowings < MaxBorrowLimit;
    }
}