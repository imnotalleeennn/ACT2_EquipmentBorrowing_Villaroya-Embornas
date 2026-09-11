using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken ct = default);
}