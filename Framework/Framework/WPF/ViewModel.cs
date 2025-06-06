namespace Framework;

using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public abstract class Atom : Object
{
    public const string Empty = "";
}

public interface INotify : INotifyPropertyChanged
{
    void Notify([CallerMemberName] string property = "");
}

public class Reference : Dictionary<string, object>
{
    public Reference() : base() { }
    public Reference(IDictionary<string, object> source) : base(source) { }
    public Reference(IEnumerable<KeyValuePair<string, object>> source) : base(source) { }
}

public abstract class ViewModel : Atom, INotify
{
    readonly Reference _reference;
    protected Type OwnerType => GetType();
    public event PropertyChangedEventHandler? PropertyChanged = (seder, e) => { };

    protected ViewModel()
    {
        _reference = new();
        OnCreate();
        InitDefaults();
    }

    protected virtual void OnCreate() { }

    protected T Get<T>([CallerMemberName] string property = Empty)
        => _reference.TryGetValue(property, out object? value) ? (T)value : default!;

    protected dynamic Get([CallerMemberName] string property = Empty)
        => Get<dynamic>(property);

    protected void Set(object value, [CallerMemberName] string property = Empty)
    {
        if (HasProp(property))
        {
            // TODO: اگر برای اتچ پراپرتی ها و برای متود مربوط به کوئرس آنها مشکل ایجاد میکند،این خط حذف شود
            if (ProprtyNotChanged(value, property))
                return;

            Set(property, value);
            Notify(property);
            return;
        }

        Set(property, value);
    }

    public void Notify([CallerMemberName] string property = Empty)
      => PropertyChanged!.Invoke(this, new PropertyChangedEventArgs(property));

    protected void Notify(IRelayCommand command) => command?.Notify();

    protected async Task RunAsync(Func<Task> command, Action<Exception> exeptionHandler = default!, Action final = default!)
    {
        try
        {
            await command();
        }
        catch (Exception e)
        {
            exeptionHandler?.Invoke(e);
        }
        finally
        {
            final?.Invoke();
        }
    }

    protected async Task<Output> RunAsync<Output>(Func<Task<Output>> command, Action<Exception> errorhandler = default!, Action final = default!)
    {
        Output result = default!;
        try
        {
            result = await command();
        }
        catch (Exception e)
        {
            errorhandler?.Invoke(e);
            //throw;
        }
        finally
        {
            final?.Invoke();
        }
        return result;
    }

    #region Private Functionality

    bool HasProp(string property) => _reference.ContainsKey(property);

    void Set(string property, object value) => _reference[property] = Verify(property, value);

    bool ProprtyNotChanged(object newvalue, string property)
        // its ok...
        => EqualityComparer<object>.Default.Equals(newvalue, _reference[property]);

    void InitDefaults()
    {
        var ownerType = OwnerType;
        var membersInfo = ownerType.GetProperties().Where(_ => _.ShoudSet());

        foreach (var memberInfo in membersInfo)
        {
            var value = DefaultValue(memberInfo, ownerType);
            // اگر نال بود چه؟
            // برای خودشو کلاس های مشتق شده اش نال برمیگرداند
            if (value is not null)
            {
                var name = memberInfo.Name;
                var type = memberInfo.PropertyType;
                var info = ownerType.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
                info.SetValue(this, value);
            }
        }
    }

    object? DefaultValue(PropertyInfo info, Type owner)
    {
        object? result = default!;
        var type = info.PropertyType;

        if (type.IsNumeric()) result = 0;
        else if (type.Is<bool>()) result = false;
        else if (type.Is<String>()) result = Empty;
        else if (type.IsEnum) result = type.Create();
        else if (type.IsSubclassOf(typeof(Enumer))) result = type.Create();
        else if (type.Is<DateTime>()) result = DateTime.MinValue;
        else if (type.Is<TimeSpan>()) result = TimeSpan.MinValue;
        else if (type.IsAssignableTo(typeof(ISet))) result = DefaultSet(type);
        else if (type == owner || type.IsSubclassOf(owner)) result = null;

        return result;
    }

    object Verify(string property, object? value)
    {
        var result = value;
        var ownerType = OwnerType;
        if (value is null)
        {
            var propInfo = ownerType.GetPropertyInfo(property)!;
            result = DefaultValue(propInfo, ownerType);
        }
        return result!;
    }

    object? DefaultSet(Type type) => typeof(ObservableSet<>).Generic(type.FirstGenericType());

    #endregion
}
