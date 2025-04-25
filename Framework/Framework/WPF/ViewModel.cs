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
    private readonly Reference _reference;
    protected Type Type => GetType();
    public event PropertyChangedEventHandler? PropertyChanged = (seder, e) => { };

    protected ViewModel()
    {
        _reference = new Reference();
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
        if (ProprtyNotChanged(value, property))
            return;

        _reference[property] = Verify(property, value);
        Notify(property);
    }

    //protected bool Set<T>(ref T field, T value, [CallerMemberName] string property = "")
    //{
    //    if (EqualityComparer<T>.Default.Equals(value, field))
    //        return false;

    //    field = value;
    //    Notify(property);
    //    return true;
    //}

    public void Notify([CallerMemberName] string property = Empty)
        => PropertyChanged!.Invoke(this, new PropertyChangedEventArgs(property));

    protected async Task RunAsync(Func<Task> command, Action<Exception>? exeptionHandler = null)
    {
        try
        {
            await command();
        }
        catch (Exception e)
        {
            exeptionHandler?.Invoke(e);
        }
    }

    protected async Task<Output> RunAsync<Output>(Func<Task<Output>> command, Action<Exception>? errorhandler = null)
    {
        Output result = default!;
        try
        {
            return await command();
        }
        catch (Exception e)
        {
            errorhandler?.Invoke(e);
            //throw;
        }
        return result;
    }

    #region private methods

    bool ProprtyNotChanged(object newvalue, string property)
        // its ok...
        => EqualityComparer<object>.Default.Equals(newvalue, _reference[property]);

    void InitDefaults()
    {
        var ownerType = Type;
        var membersInfo = ownerType.GetProperties().Where(_ => _.ShoudSet());

        foreach (var memberInfo in membersInfo)
        {
            var propValue = DefaultValue(memberInfo, ownerType);
            // اگر نال بود چه؟
            // برای خودشو کلاس های مشتق شده اش نال برمیگرداند
            if (propValue is not null)
            {
                var name = memberInfo.Name;
                var type = memberInfo.PropertyType;
                var propertyInfo = ownerType.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
                propertyInfo.SetValue(this, propValue);
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
        var ownerType = Type;
        if (value is null)
        {
            var propInfo = ownerType.GetPropertyInfo(property)!;
            result = DefaultValue(propInfo, ownerType);
        }
        return result!;
    }

    object? DefaultSet(Type type) => typeof(Set<>).Generic(type.FirstGenericType());

    #endregion
}
