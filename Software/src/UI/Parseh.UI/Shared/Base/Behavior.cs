namespace Parseh.UI;

public abstract class Behavior<TOwner, TProperty>
    where TOwner : Behavior<TOwner, TProperty>, new()
{
    static readonly TOwner _self = new();
    //event Action<DependencyObject, DependencyPropertyChangedEventArgs> ValueChanged = (sender, e) => { };

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.RegisterAttached("Value", typeof(TProperty), typeof(Behavior<TOwner, TProperty>), new PropertyMetadata(default(TProperty), new PropertyChangedCallback(OnValuePeopertyChanged)));

    public static TProperty GetValue(DependencyObject uielement) => uielement.GetValue(ValueProperty).As<TProperty>();
    public static void SetValue(DependencyObject uielement, TProperty value) => uielement.SetValue(ValueProperty, value);

    protected abstract void OnValueChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e);
    private static void OnValuePeopertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        _self.OnValueChanged(uielement, e);
        //_self.ValueChanged(uielement, e);
    }
}