using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Models;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class BorrowEquipmentService
{
    private readonly IStudentRepository _studentRepo;
    private readonly IEquipmentRepository _equipmentRepo;
    private readonly IBorrowingRepository _borrowingRepo;

    public BorrowEquipmentService(
        IStudentRepository studentRepo,
        IEquipmentRepository equipmentRepo,
        IBorrowingRepository borrowingRepo)
    {
        _studentRepo = studentRepo;
        _equipmentRepo = equipmentRepo;
        _borrowingRepo = borrowingRepo;
    }

    public async Task<BorrowResult> ExecuteAsync(int studentId, int equipmentId, CancellationToken ct = default)
    {
        // 1. Does the student exist?
        var student = await _studentRepo.GetByIdAsync(studentId, ct);
        if (student == null)
        {
            return BorrowResult.Fail($"Student with ID {studentId} does not exist.");
        }

        // 2. Is the student allowed to borrow?
        if (!student.IsEligibleToBorrow)
        {
            return BorrowResult.Fail($"Student '{student.Name}' (ID: {student.Id}) is not eligible to borrow equipment.");
        }

        // 3. Does the equipment exist?
        var equipment = await _equipmentRepo.GetByIdAsync(equipmentId, ct);
        if (equipment == null)
        {
            return BorrowResult.Fail($"Equipment with ID {equipmentId} does not exist.");
        }

        // 4. Is the equipment currently available?
        if (!equipment.IsAvailable)
        {
            return BorrowResult.Fail($"Equipment '{equipment.Name}' (ID: {equipment.Id}) is currently unavailable.");
        }

        // 5. Has the student reached the maximum allowed active borrowings?
        int activeCount = await _borrowingRepo.GetActiveBorrowingCountAsync(studentId, ct);
        if (activeCount >= student.MaxBorrowLimit)
        {
            return BorrowResult.Fail($"Student '{student.Name}' has reached the maximum borrowing limit of {student.MaxBorrowLimit} item(s).");
        }

        // 6. Create borrowing, update equipment state, and persist changes
        var borrowing = new Borrowing(studentId, equipmentId, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        await _borrowingRepo.AddAsync(borrowing, ct);

        equipment.MarkAsBorrowed();
        await _equipmentRepo.UpdateAsync(equipment, ct);

        return BorrowResult.Ok(borrowing, $"Successfully borrowed '{equipment.Name}' for {student.Name}. Due on {borrowing.DueDate:yyyy-MM-dd}.");
    }
}