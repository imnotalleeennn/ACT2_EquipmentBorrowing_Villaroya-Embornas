using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Interfaces;

public interface IBorrowingRepository
{
    Task<int> GetActiveBorrowingCountAsync(int studentId, CancellationToken ct = default);
    Task AddAsync(Borrowing borrowing, CancellationToken ct = default);
    Task<Borrowing?> GetActiveBorrowingByEquipmentIdAsync(int equipmentId, CancellationToken ct = default);
    Task UpdateAsync(Borrowing borrowing, CancellationToken ct = default);
    Task<IReadOnlyList<Borrowing>> GetAllAsync(CancellationToken ct = default);
}