using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public EquipmentViewModel EquipmentViewModel { get; }
    public BorrowingsViewModel BorrowingsViewModel { get; }

    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private bool _isEquipmentActive = true;

    [ObservableProperty]
    private bool _isBorrowingsActive = false;

    public MainWindowViewModel(
        EquipmentViewModel equipmentViewModel,
        BorrowingsViewModel borrowingsViewModel)
    {
        EquipmentViewModel = equipmentViewModel;
        BorrowingsViewModel = borrowingsViewModel;

        _currentPage = equipmentViewModel;
        _ = equipmentViewModel.LoadDataAsync();
    }

    [RelayCommand]
    public async Task NavigateToEquipmentAsync()
    {
        CurrentPage = EquipmentViewModel;
        IsEquipmentActive = true;
        IsBorrowingsActive = false;
        await EquipmentViewModel.LoadDataAsync();
    }

    [RelayCommand]
    public async Task NavigateToBorrowingsAsync()
    {
        CurrentPage = BorrowingsViewModel;
        IsEquipmentActive = false;
        IsBorrowingsActive = true;
        await BorrowingsViewModel.LoadDataAsync();
    }
}
