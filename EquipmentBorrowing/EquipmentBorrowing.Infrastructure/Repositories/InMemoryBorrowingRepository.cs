using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryBorrowingRepository : IBorrowingRepository
{
    private readonly List<Borrowing> _borrowings = new()
    {
        // Pre-existing active borrowing for Equipment 2 (Dell Latitude Laptop) borrowed by Student 1
        new Borrowing(studentId: 1, equipmentId: 2, borrowDate: DateTime.UtcNow.AddDays(-2), dueDate: DateTime.UtcNow.AddDays(5))
        {
            Id = 1,
            Status = BorrowingStatus.Active
        }
    };

    public Task<int> GetActiveBorrowingCountAsync(int studentId, CancellationToken ct = default)
    {
        int count = _borrowings.Count(b => b.StudentId == studentId && b.Status == BorrowingStatus.Active);
        return Task.FromResult(count);
    }

    public Task AddAsync(Borrowing borrowing, CancellationToken ct = default)
    {
        if (borrowing.Id == 0)
        {
            borrowing.Id = _borrowings.Count > 0 ? _borrowings.Max(b => b.Id) + 1 : 1;
        }
        _borrowings.Add(borrowing);
        return Task.CompletedTask;
    }

    public Task<Borrowing?> GetActiveBorrowingByEquipmentIdAsync(int equipmentId, CancellationToken ct = default)
    {
        var record = _borrowings.FirstOrDefault(b => b.EquipmentId == equipmentId && b.Status == BorrowingStatus.Active);
        return Task.FromResult(record);
    }

    public Task UpdateAsync(Borrowing borrowing, CancellationToken ct = default)
    {
        var index = _borrowings.FindIndex(b => b.Id == borrowing.Id);
        if (index != -1)
        {
            _borrowings[index] = borrowing;
        }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Borrowing>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<Borrowing> all = _borrowings.ToList();
        return Task.FromResult(all);
    }
}