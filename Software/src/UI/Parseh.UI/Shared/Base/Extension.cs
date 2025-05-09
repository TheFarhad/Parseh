namespace Parseh.UI;

using System.Security;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows.Media;

internal static partial class Extension
{
    public static string Unsecure(this SecureString source)
    {
        if (source is null)
            return String.Empty;


        var ptr = IntPtr.Zero;
        try
        {
            ptr = Marshal.SecureStringToGlobalAllocUnicode(source);
            return Marshal.PtrToStringUni(ptr)!;
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(ptr);
        }
    }

    public static Brush ToBrush(this string hex)
    {
        if (hex.IsEmpty())
            return Brushes.Transparent;

        if (!hex.StartsWith("#"))
            hex = $"#{hex}";

        return new BrushConverter().ConvertFromString(hex)!.As<SolidColorBrush>();
    }

    public static Task WaitDo(this Action<Task> ondo, double seconds)
        => Task.Delay(TimeSpan.FromSeconds(seconds)).ContinueWith((t) => ondo);
}

internal static partial class Extension
{
    public static T GetPropertyValue<T>(this Expression<Func<T>> source) => source.Compile().Invoke();

    public static void SetPropertyValue<T>(this Expression<Func<T>> source, T value)
    {
        var expression = source.As<LambdaExpression>().Body.As<MemberExpression>();
        var propertyinfo = expression.Member.As<PropertyInfo>();
        var target = Expression.Lambda(expression.Expression!).Compile().DynamicInvoke();
        propertyinfo.SetValue(target, value);
    }
}