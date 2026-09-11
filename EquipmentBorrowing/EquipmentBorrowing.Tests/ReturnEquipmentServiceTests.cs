using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Repositories;
using Xunit;

namespace EquipmentBorrowing.Tests;

public class ReturnEquipmentServiceTests
{
    private readonly InMemoryEquipmentRepository _equipmentRepo;
    private readonly InMemoryBorrowingRepository _borrowingRepo;
    private readonly ReturnEquipmentService _service;

    public ReturnEquipmentServiceTests()
    {
        _equipmentRepo = new InMemoryEquipmentRepository();
        _borrowingRepo = new InMemoryBorrowingRepository();
        _service = new ReturnEquipmentService(_equipmentRepo, _borrowingRepo);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEquipmentDoesNotExist_ReturnsFailure()
    {
        // Act
        var result = await _service.ExecuteAsync(equipmentId: 999);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("does not exist", result.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEquipmentIsNotCurrentlyBorrowed_ReturnsFailure()
    {
        // Equipment 1 is available and not actively borrowed
        // Act
        var result = await _service.ExecuteAsync(equipmentId: 1);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("No active borrowing found", result.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEquipmentIsBorrowed_ReturnsSuccessAndRestoresAvailability()
    {
        // Equipment 2 (Dell Latitude Laptop) has an active borrowing initially
        // Act
        var result = await _service.ExecuteAsync(equipmentId: 2);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("now available", result.Message);

        // Verify equipment is now available in repository
        var equipment = await _equipmentRepo.GetByIdAsync(2);
        Assert.NotNull(equipment);
        Assert.True(equipment.IsAvailable);

        // Verify borrowing is marked returned
        var activeBorrowing = await _borrowingRepo.GetActiveBorrowingByEquipmentIdAsync(2);
        Assert.Null(activeBorrowing);
    }
}
