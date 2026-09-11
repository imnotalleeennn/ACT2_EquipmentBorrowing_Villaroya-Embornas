using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryEquipmentRepository : IEquipmentRepository
{
    private readonly List<Equipment> _equipment = new()
    {
        new Equipment(1, "Epson LCD Projector", isAvailable: true),
        new Equipment(2, "Dell Latitude Laptop", isAvailable: false),
        new Equipment(3, "HDMI to USB-C Adapter", isAvailable: true),
        new Equipment(4, "Canon DSLR Camera", isAvailable: true),
        new Equipment(5, "Wireless Microphone Set", isAvailable: true)
    };

    public Task<Equipment?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var item = _equipment.FirstOrDefault(e => e.Id == id);
        return Task.FromResult(item);
    }

    public Task UpdateAsync(Equipment equipment, CancellationToken ct = default)
    {
        var index = _equipment.FindIndex(e => e.Id == equipment.Id);
        if (index != -1)
        {
            _equipment[index] = equipment;
        }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Equipment>> GetAvailableAsync(CancellationToken ct = default)
    {
        IReadOnlyList<Equipment> available = _equipment.Where(e => e.IsAvailable).ToList();
        return Task.FromResult(available);
    }

    public Task<IReadOnlyList<Equipment>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<Equipment> all = _equipment.ToList();
        return Task.FromResult(all);
    }
}