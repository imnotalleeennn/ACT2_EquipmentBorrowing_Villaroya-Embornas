using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Models;

namespace EquipmentBorrowing.Application.Services;

public class ReturnEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepo;
    private readonly IBorrowingRepository _borrowingRepo;

    public ReturnEquipmentService(
        IEquipmentRepository equipmentRepo,
        IBorrowingRepository borrowingRepo)
    {
        _equipmentRepo = equipmentRepo;
        _borrowingRepo = borrowingRepo;
    }

    public async Task<ReturnResult> ExecuteAsync(int equipmentId, CancellationToken ct = default)
    {
        // 1. Does the equipment exist?
        var equipment = await _equipmentRepo.GetByIdAsync(equipmentId, ct);
        if (equipment == null)
        {
            return ReturnResult.Fail($"Equipment with ID {equipmentId} does not exist.");
        }

        // 2. Is there an active borrowing record for this equipment?
        var borrowing = await _borrowingRepo.GetActiveBorrowingByEquipmentIdAsync(equipmentId, ct);
        if (borrowing == null)
        {
            return ReturnResult.Fail($"No active borrowing found for equipment '{equipment.Name}'. It may already be returned.");
        }

        // 3. Mark borrowing as returned and persist
        borrowing.MarkAsReturned(DateTime.UtcNow);
        await _borrowingRepo.UpdateAsync(borrowing, ct);

        // 4. Mark equipment as available again and persist
        equipment.MarkAsReturned();
        await _equipmentRepo.UpdateAsync(equipment, ct);

        return ReturnResult.Ok($"Successfully returned '{equipment.Name}'. The equipment is now available.");
    }
}
