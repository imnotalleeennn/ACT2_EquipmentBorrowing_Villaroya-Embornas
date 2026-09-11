using System;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Desktop.ViewModels;
using EquipmentBorrowing.Desktop.Views;
using EquipmentBorrowing.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EquipmentBorrowing.Desktop;

public partial class App : Avalonia.Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // 1. Configure Dependency Injection Composition Root
        var services = new ServiceCollection();

        // Repositories registered as Singletons so in-memory state is preserved across navigation
        services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
        services.AddSingleton<IEquipmentRepository, InMemoryEquipmentRepository>();
        services.AddSingleton<IBorrowingRepository, InMemoryBorrowingRepository>();

        // Application Services
        services.AddTransient<BorrowEquipmentService>();
        services.AddTransient<ReturnEquipmentService>();
        services.AddTransient<GetAllEquipmentService>();
        services.AddTransient<GetAllStudentsService>();
        services.AddTransient<GetActiveBorrowingsService>();

        // ViewModels
        services.AddTransient<EquipmentViewModel>();
        services.AddTransient<BorrowingsViewModel>();
        services.AddTransient<MainWindowViewModel>();

        Services = services.BuildServiceProvider();

        // 2. Resolve Main Window and DataContext
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}