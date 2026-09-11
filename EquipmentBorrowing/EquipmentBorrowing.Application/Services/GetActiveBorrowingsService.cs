using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Models;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class GetActiveBorrowingsService
{
    private readonly IBorrowingRepository _borrowingRepo;
    private readonly IStudentRepository _studentRepo;
    private readonly IEquipmentRepository _equipmentRepo;

    public GetActiveBorrowingsService(
        IBorrowingRepository borrowingRepo,
        IStudentRepository studentRepo,
        IEquipmentRepository equipmentRepo)
    {
        _borrowingRepo = borrowingRepo;
        _studentRepo = studentRepo;
        _equipmentRepo = equipmentRepo;
    }

    public async Task<IReadOnlyList<ActiveBorrowingDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var allBorrowings = await _borrowingRepo.GetAllAsync(ct);
        var active = allBorrowings.Where(b => b.Status == BorrowingStatus.Active).ToList();

        var dtos = new List<ActiveBorrowingDto>();
        foreach (var b in active)
        {
            var student = await _studentRepo.GetByIdAsync(b.StudentId, ct);
            var equipment = await _equipmentRepo.GetByIdAsync(b.EquipmentId, ct);

            dtos.Add(new ActiveBorrowingDto
            {
                BorrowingId = b.Id,
                StudentId = b.StudentId,
                StudentName = student?.Name ?? $"Student #{b.StudentId}",
                EquipmentId = b.EquipmentId,
                EquipmentName = equipment?.Name ?? $"Equipment #{b.EquipmentId}",
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate,
                Status = b.Status
            });
        }

        return dtos;
    }
}
