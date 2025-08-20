// SYSTEM
global using System;
global using System.IO;
global using System.Windows;
global using System.Reflection;
global using System.Windows.Media;
global using System.Windows.Data;
global using System.Windows.Input;
global using System.Globalization;
global using System.ComponentModel;
global using System.Windows.Markup;
global using System.Threading.Tasks;
global using System.Windows.Controls;
global using System.Windows.Threading;

// .NET

// EXTERNAL LIBRARIES
global using Microsoft.Extensions.DependencyInjection;

//LOCAL LIBRARIES
global using Framework;
global using Parseh.UI.Views;
global using Parseh.UI.ViewModels;
global using ViewModel = Framework.ViewModel;


public interface IDialogService
{
    TViewModel ShowDialog<TView, TViewModel>(Window ownerWindow = default!)
        where TView : ContentLayout
        where TViewModel : ViewModel;

    void ShowMessageBox(string message, string title);
}

public class DialogService : IDialogService
{
    private readonly IServiceProvider _serviceProvider;

    public DialogService(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public TViewModel ShowDialog<TView, TViewModel>(Window ownerWindow = default!)
        where TView : ContentLayout
        where TViewModel : ViewModel
    {
        // ایجاد یک Scope جدید برای دیالوگ برای مدیریت چرخه حیات و وابستگی‌های آن [24]
        using (var scope = _serviceProvider.CreateScope())
        {
            var dialogView = scope.ServiceProvider.GetRequiredService<TView>();
            var dialogViewModel = scope.ServiceProvider.GetRequiredService<TViewModel>();

            dialogView.DataContext = dialogViewModel;

            if (ownerWindow is { }) // تنظیم Owner برای رفتار صحیح پنجره Modal [26, 28]
            {
                dialogView.Owner = ownerWindow;
                dialogView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                dialogView.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            dialogView.ShowDialog(); // بلاک می‌کند تا دیالوگ بسته شود [26]
            return dialogViewModel; // ViewModel را برمی‌گرداند تا به نتایج دسترسی پیدا شود [26, 27]
        }
    }

    public void ShowMessageBox(string message, string title)
    {
        // TODO: بر اساس دیالوگ های خودم تکمیل شود
    }
}

