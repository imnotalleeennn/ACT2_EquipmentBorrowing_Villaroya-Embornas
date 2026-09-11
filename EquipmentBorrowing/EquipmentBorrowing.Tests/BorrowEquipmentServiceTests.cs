using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Repositories;
using Xunit;

namespace EquipmentBorrowing.Tests;

public class BorrowEquipmentServiceTests
{
    private readonly InMemoryStudentRepository _studentRepo;
    private readonly InMemoryEquipmentRepository _equipmentRepo;
    private readonly InMemoryBorrowingRepository _borrowingRepo;
    private readonly BorrowEquipmentService _service;

    public BorrowEquipmentServiceTests()
    {
        _studentRepo = new InMemoryStudentRepository();
        _equipmentRepo = new InMemoryEquipmentRepository();
        _borrowingRepo = new InMemoryBorrowingRepository();
        _service = new BorrowEquipmentService(_studentRepo, _equipmentRepo, _borrowingRepo);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudentDoesNotExist_ReturnsFailure()
    {
        // Act
        var result = await _service.ExecuteAsync(studentId: 999, equipmentId: 1);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("does not exist", result.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudentIsNotEligible_ReturnsFailure()
    {
        // Student 2 is Maria Santos (IsEligibleToBorrow = false)
        // Act
        var result = await _service.ExecuteAsync(studentId: 2, equipmentId: 1);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not eligible to borrow", result.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEquipmentDoesNotExist_ReturnsFailure()
    {
        // Act
        var result = await _service.ExecuteAsync(studentId: 1, equipmentId: 888);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("does not exist", result.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEquipmentIsAlreadyUnavailable_ReturnsFailure()
    {
        // Equipment 2 (Dell Latitude Laptop) is initially unavailable
        // Act
        var result = await _service.ExecuteAsync(studentId: 1, equipmentId: 2);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("currently unavailable", result.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudentReachesMaxLimit_ReturnsFailure()
    {
        // Student 3 (Pedro Penduko) has MaxBorrowLimit = 1
        // First borrow: should succeed
        var firstResult = await _service.ExecuteAsync(studentId: 3, equipmentId: 3);
        Assert.True(firstResult.Success);

        // Second borrow: should fail because limit of 1 is reached
        var secondResult = await _service.ExecuteAsync(studentId: 3, equipmentId: 4);

        // Assert
        Assert.False(secondResult.Success);
        Assert.Contains("maximum borrowing limit", secondResult.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRulesAreSatisfied_SucceedsAndUpdatesAvailability()
    {
        // Student 1 (Juan Dela Cruz, limit 3) borrows Equipment 1 (Epson LCD Projector, available)
        // Act
        var result = await _service.ExecuteAsync(studentId: 1, equipmentId: 1);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Borrowing);
        Assert.Equal(1, result.Borrowing.StudentId);
        Assert.Equal(1, result.Borrowing.EquipmentId);
        Assert.Equal(BorrowingStatus.Active, result.Borrowing.Status);

        // Verify equipment is marked unavailable in repository
        var updatedEquipment = await _equipmentRepo.GetByIdAsync(1);
        Assert.NotNull(updatedEquipment);
        Assert.False(updatedEquipment.IsAvailable);
    }
}
