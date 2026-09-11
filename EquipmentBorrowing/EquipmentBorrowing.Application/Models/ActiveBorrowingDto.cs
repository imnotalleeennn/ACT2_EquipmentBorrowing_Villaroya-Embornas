using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Models;

public class ActiveBorrowingDto
{
    public int BorrowingId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public BorrowingStatus Status { get; set; }
}
