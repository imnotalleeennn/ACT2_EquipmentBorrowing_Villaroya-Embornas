namespace EquipmentBorrowing.Application.Models;

public class ReturnResult
{
    public bool Success { get; }
    public string Message { get; }

    private ReturnResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public static ReturnResult Ok(string message = "Equipment returned successfully.")
    {
        return new ReturnResult(true, message);
    }

    public static ReturnResult Fail(string message)
    {
        return new ReturnResult(false, message);
    }

    public static implicit operator bool(ReturnResult result) => result.Success;
}
