namespace Parseh.UI;

public abstract class Behavior<TOwner, TProperty>
    where TOwner : Behavior<TOwner, TProperty>, new()
{
    static readonly TOwner _self = new();
    //event Action<DependencyObject, DependencyPropertyChangedEventArgs> ValueChanged = (sender, e) => { };
    //event Action<DependencyObject, object> ValueUpdated = (sender, value) => { };

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty
        .RegisterAttached("Value",
            typeof(TProperty),
            typeof(Behavior<TOwner, TProperty>),
            new UIPropertyMetadata(default(TProperty), new PropertyChangedCallback(OnValuePeopertyChanged), new CoerceValueCallback(OnValuePropertyUpdated)));

    public static TProperty GetValue(DependencyObject uielement) => uielement.GetValue(ValueProperty).As<TProperty>();
    public static void SetValue(DependencyObject uielement, TProperty value) => uielement.SetValue(ValueProperty, value);

    protected abstract void OnValueChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e);
    private static void OnValuePeopertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        _self.OnValueChanged(uielement, e);
        //_self.ValueChanged(uielement, e);
    }

    public virtual object OnValueUpdated(DependencyObject uielement, object value)
    {
        return value;
    }
    static object OnValuePropertyUpdated(DependencyObject uielement, object value)
    {
        //return _self.ValueUpdated(uielement, value);
        return _self.OnValueUpdated(uielement, value);
    }
}

public abstract class AnimatedBehavior<TOwner> : Behavior<TOwner, bool>
    where TOwner : Behavior<TOwner, bool>, new()
{
    public override object OnValueUpdated(DependencyObject uielement, object value)
    {
        return base.OnValueUpdated(uielement, value);
    }
}