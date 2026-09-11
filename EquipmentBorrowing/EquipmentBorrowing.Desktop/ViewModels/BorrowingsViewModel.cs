using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Models;
using EquipmentBorrowing.Application.Services;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class BorrowingsViewModel : ViewModelBase
{
    private readonly GetActiveBorrowingsService _getActiveBorrowingsService;
    private readonly ReturnEquipmentService _returnEquipmentService;

    [ObservableProperty]
    private ObservableCollection<ActiveBorrowingDto> _activeBorrowings = new();

    [ObservableProperty]
    private ActiveBorrowingDto? _selectedBorrowing;

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private bool _isSuccess;

    [ObservableProperty]
    private bool _hasStatus;

    [ObservableProperty]
    private bool _isLoading;

    public BorrowingsViewModel(
        GetActiveBorrowingsService getActiveBorrowingsService,
        ReturnEquipmentService returnEquipmentService)
    {
        _getActiveBorrowingsService = getActiveBorrowingsService;
        _returnEquipmentService = returnEquipmentService;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var active = await _getActiveBorrowingsService.ExecuteAsync();
            ActiveBorrowings.Clear();
            foreach (var item in active)
            {
                ActiveBorrowings.Add(item);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task ReturnAsync()
    {
        // --- 1. PRESENTATION-LEVEL VALIDATION ---
        if (SelectedBorrowing == null)
        {
            SetStatus("Please select an active borrowing record from the list to return.", false);
            return;
        }

        // --- 2. DELEGATE TO APPLICATION SERVICE ---
        IsLoading = true;
        try
        {
            ReturnResult result = await _returnEquipmentService.ExecuteAsync(SelectedBorrowing.EquipmentId);

            SetStatus(result.Message, result.Success);

            if (result.Success)
            {
                SelectedBorrowing = null;
                await LoadDataAsync();
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
