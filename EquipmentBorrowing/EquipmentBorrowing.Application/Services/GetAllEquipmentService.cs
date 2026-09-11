using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class GetAllEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepo;

    public GetAllEquipmentService(IEquipmentRepository equipmentRepo)
    {
        _equipmentRepo = equipmentRepo;
    }

    public async Task<IReadOnlyList<Equipment>> ExecuteAsync(CancellationToken ct = default)
    {
        return await _equipmentRepo.GetAllAsync(ct);
    }
}
