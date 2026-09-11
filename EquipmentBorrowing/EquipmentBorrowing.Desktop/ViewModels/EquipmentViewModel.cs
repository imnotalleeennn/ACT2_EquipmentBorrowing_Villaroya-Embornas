using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Models;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ViewModelBase
{
    private readonly GetAllEquipmentService _getAllEquipmentService;
    private readonly GetAllStudentsService _getAllStudentsService;
    private readonly BorrowEquipmentService _borrowEquipmentService;

    [ObservableProperty]
    private ObservableCollection<Equipment> _equipmentList = new();

    [ObservableProperty]
    private ObservableCollection<Student> _studentsList = new();

    [ObservableProperty]
    private Equipment? _selectedEquipment;

    [ObservableProperty]
    private Student? _selectedStudent;

    [ObservableProperty]
    private DateTimeOffset? _expectedReturnDate = DateTimeOffset.Now.AddDays(7);

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private bool _isSuccess;

    [ObservableProperty]
    private bool _hasStatus;

    [ObservableProperty]
    private bool _isLoading;

    public EquipmentViewModel(
        GetAllEquipmentService getAllEquipmentService,
        GetAllStudentsService getAllStudentsService,
        BorrowEquipmentService borrowEquipmentService)
    {
        _getAllEquipmentService = getAllEquipmentService;
        _getAllStudentsService = getAllStudentsService;
        _borrowEquipmentService = borrowEquipmentService;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var equipment = await _getAllEquipmentService.ExecuteAsync();
            EquipmentList.Clear();
            foreach (var item in equipment)
            {
                EquipmentList.Add(item);
            }

            var students = await _getAllStudentsService.ExecuteAsync();
            StudentsList.Clear();
            foreach (var student in students)
            {
                StudentsList.Add(student);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task BorrowAsync()
    {
        // --- 1. PRESENTATION-LEVEL VALIDATION ---
        if (SelectedStudent == null)
        {
            SetStatus("Please select a student from the dropdown list.", false);
            return;
        }

        if (SelectedEquipment == null)
        {
            SetStatus("Please select an equipment item to borrow.", false);
            return;
        }

        if (ExpectedReturnDate == null || ExpectedReturnDate.Value.Date <= DateTimeOffset.Now.Date)
        {
            SetStatus("Expected return date must be in the future.", false);
            return;
        }

        // --- 2. DELEGATE TO APPLICATION SERVICE ---
        IsLoading = true;
        try
        {
            BorrowResult result = await _borrowEquipmentService.ExecuteAsync(
                studentId: SelectedStudent.Id,
                equipmentId: SelectedEquipment.Id);

            SetStatus(result.Message, result.Success);

            if (result.Success)
            {
                // Refresh equipment list to reflect updated availability
                await LoadDataAsync();
                SelectedEquipment = null;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void ClearStatus()
    {
        HasStatus = false;
        StatusMessage = null;
    }

    private void SetStatus(string message, bool isSuccess)
    {
        StatusMessage = message;
        IsSuccess = isSuccess;
        HasStatus = true;
    }
}
