using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class GetAllStudentsService
{
    private readonly IStudentRepository _studentRepo;

    public GetAllStudentsService(IStudentRepository studentRepo)
    {
        _studentRepo = studentRepo;
    }

    public async Task<IReadOnlyList<Student>> ExecuteAsync(CancellationToken ct = default)
    {
        return await _studentRepo.GetAllAsync(ct);
    }
}
