using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class GetAvailableEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepo;

    public GetAvailableEquipmentService(IEquipmentRepository equipmentRepo)
    {
        _equipmentRepo = equipmentRepo;
    }

    public async Task<IReadOnlyList<Equipment>> ExecuteAsync(CancellationToken ct = default)
    {
        return await _equipmentRepo.GetAvailableAsync(ct);
    }
}
