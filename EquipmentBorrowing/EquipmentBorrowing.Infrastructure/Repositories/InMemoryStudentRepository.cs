using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _students = new()
    {
        new Student(1, "Mharc Allen Villaroya", isEligibleToBorrow: true, maxBorrowLimit: 3),
        new Student(2, "Prince Lawrence Embornas", isEligibleToBorrow: false, maxBorrowLimit: 2),
        new Student(3, "Aliya Raymundo", isEligibleToBorrow: true, maxBorrowLimit: 1)
    };

    public Task<Student?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var student = _students.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(student);
    }

    public Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<Student> all = _students.ToList();
        return Task.FromResult(all);
    }
}