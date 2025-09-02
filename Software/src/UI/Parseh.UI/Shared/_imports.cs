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
global using Parseh.UI.Resources;
global using Parseh.UI.ViewModels;
global using ViewModel = Framework.ViewModel;
using System.Runtime.CompilerServices;


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




public class MultiDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
    where TKey : notnull
{
    public void Add(TKey key, TValue item)
    {
        if (!TryGetValue(key, out List<TValue> items))
        {
            items = [];
            Add(key, items);
        }
        items.Add(item);
    }

    public void RemoveValue(TKey key, TValue value)
    {
        if (TryGetValue(key, out List<TValue> items))
        {
            items.Remove(value);

            if (items.Count == 0)
                Remove(key);
        }
    }

    public void RemoveKey(TKey key)
    {
        if (ContainsKey(key))
            Remove(key);
    }

    public IEnumerable<TValue> Get(TKey key)
        => TryGetValue(key, out List<TValue> items) ? items : [];
}


public enum ViewModelMessages
{
    UserLoggedIn,
    ProductSelected,
    OrderPlaced
}

public sealed class Mediator
{
    private static readonly Mediator _instance = new Mediator();
    private readonly MultiDictionary<ViewModelMessages, Action<object>> _internalList = new MultiDictionary<ViewModelMessages, Action<object>>();

    private Mediator() { }

    public static Mediator Instance => _instance ?? new();


    public void Register(Action<object> callback, ViewModelMessages message)
        => _internalList.Add(message, callback);

    public void Unregister(Action<object> callback, ViewModelMessages message)
       => _internalList.RemoveValue(message, callback);

    /// <summary>
    /// یک پیام را به تمام همکاران ثبت شده اطلاع می‌دهد.
    /// </summary>
    /// <param name="message">نوع پیامی که باید اطلاع داده شود.</param>
    /// <param name="args">آرگومان‌های پیام.</param>
    public void NotifyColleagues(ViewModelMessages message, object args)
    {
        if (_internalList.ContainsKey(message))
        {
            // از یک کپی برای جلوگیری از خطاهای همزمانی در حین تکرار استفاده کنید
            foreach (var callback in _internalList.Get(message).ToList())
            {
                callback.Invoke(args);
            }
        }
    }
}

// ViewModel فرستنده
public class LoginViewModel : ViewModel
{
    public string Username { get => Get(); private set => Set(value); }

    public IRelayCommand LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new Command<Object>(ExecuteLogin);
    }

    private void ExecuteLogin(object parameter)
    {
        // منطق احراز هویت
        if (!string.IsNullOrEmpty(Username))
        {
            // پس از ورود موفق، پیام را از طریق Mediator ارسال کنید
            Mediator.Instance.NotifyColleagues(ViewModelMessages.UserLoggedIn, Username);
        }
    }
}

// ViewModel گیرنده
public class DashboardViewModel : ViewModel, IDisposable
{
    public string WelcomeMessage { get => Get(); private set => Set(value); }

    public DashboardViewModel()
    {
        var xx = new ConditionalWeakTable<Mediator, string>();

        // ثبت نام برای پیام UserLoggedIn
        Mediator.Instance.Register(HandleUserLoggedIn, ViewModelMessages.UserLoggedIn);
    }

    private void HandleUserLoggedIn(object username)
    {
        WelcomeMessage = $"Welcome, {username}!";
    }

    // پیاده‌سازی IDisposable برای لغو ثبت نام و جلوگیری از نشت حافظه
    public void Dispose()
    {
        Mediator.Instance.Unregister(HandleUserLoggedIn, ViewModelMessages.UserLoggedIn);
    }
}

