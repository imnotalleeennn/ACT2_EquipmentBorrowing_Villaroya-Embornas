namespace EquipmentBorrowing.Domain;

public class Borrowing
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int EquipmentId { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BorrowingStatus Status { get; set; } = BorrowingStatus.Active;

    public Borrowing() { }

    public Borrowing(int studentId, int equipmentId, DateTime borrowDate, DateTime dueDate)
    {
        StudentId = studentId;
        EquipmentId = equipmentId;
        BorrowDate = borrowDate;
        DueDate = dueDate;
        Status = BorrowingStatus.Active;
    }

    public void MarkAsReturned(DateTime returnDate)
    {
        Status = BorrowingStatus.Returned;
        ReturnDate = returnDate;
    }
}