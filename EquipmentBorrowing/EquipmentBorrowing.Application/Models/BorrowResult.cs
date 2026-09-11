using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Models;

public class BorrowResult
{
    public bool Success { get; }
    public string Message { get; }
    public Borrowing? Borrowing { get; }

    private BorrowResult(bool success, string message, Borrowing? borrowing = null)
    {
        Success = success;
        Message = message;
        Borrowing = borrowing;
    }

    public static BorrowResult Ok(Borrowing borrowing, string message = "Equipment borrowed successfully.")
    {
        return new BorrowResult(true, message, borrowing);
    }

    public static BorrowResult Fail(string message)
    {
        return new BorrowResult(false, message);
    }

    // Allows implicit conversion to bool so legacy code like 'bool success = await service.ExecuteAsync(...)' works seamlessly
    public static implicit operator bool(BorrowResult result) => result.Success;
}
