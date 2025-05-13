namespace Parseh.UI;

public abstract class AttachedProperty<TOwner, TProperty>
    where TOwner : AttachedProperty<TOwner, TProperty>, new()
{
    static readonly TOwner _default = new();
    //event Action<DependencyObject, DependencyPropertyChangedEventArgs> _defaultPropertyChanged = (sender, e) => { };
    //event Func<DependencyObject, object, object> _defaultCoerceValue = (sender, value) => { return value; };

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty
        .RegisterAttached("Value",
            typeof(TProperty),
            typeof(AttachedProperty<TOwner, TProperty>),
            new UIPropertyMetadata(default(TProperty), new PropertyChangedCallback(OnPeopertyChangedCallback), new CoerceValueCallback(OnCoerceValueCallback)));

    public static TProperty GetValue(DependencyObject uielement) => uielement.GetValue(ValueProperty).As<TProperty>();
    public static void SetValue(DependencyObject uielement, TProperty value) => uielement.SetValue(ValueProperty, value);

    public virtual void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e) { }
    public virtual void OnCoerceValue(DependencyObject uielement, object value) { }

    #region Private Functionality

    static void OnPeopertyChangedCallback(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        _default.OnPropertyChanged(uielement, e);
    }
    static object OnCoerceValueCallback(DependencyObject uielement, object value)
    {
        _default.OnCoerceValue(uielement, value);
        return value;
    }

    #endregion

}

public abstract class SimpleAnimatedProperty<TOwner> : AttachedProperty<TOwner, bool>
    where TOwner : AttachedProperty<TOwner, bool>, new()
{
    public override void OnCoerceValue(DependencyObject uielement, object value)
    {
        // if the propert isn't FrameworkElement
        if (!(uielement is FrameworkElement element)) return;

        // if the property not changed
        if (element.GetValue(ValueProperty) == value) return;

        DoAnimate(element, value.As<bool>());
    }

    protected abstract void DoAnimate(FrameworkElement element, bool value);
}

public abstract class AnimatedAttachedProperty<TOwner> : AttachedProperty<TOwner, bool>
    where TOwner : AttachedProperty<TOwner, bool>, new()
{
    public bool IsFirstLoad { get; set; } = true;

    public override void OnCoerceValue(DependencyObject uielement, object value)
    {
        // if the propert isn't FrameworkElement
        if (!(uielement is FrameworkElement element)) return;

        // if the property not changed
        if (element.GetValue(ValueProperty) == value && !IsFirstLoad) return;

        var val = value.As<bool>();
        if (IsFirstLoad)
        {
            // TODO: Best Technique
            // تکنیک جالب برای اینکه یک اکشن فقط یکبار اتفاق بیافتد
            RoutedEventHandler onloaded = default!;
            onloaded = (sender, e) =>
            {
                DoAnimate(element, val);
                element.Loaded -= onloaded;
                IsFirstLoad = false;
            };
            element.Loaded += onloaded;
        }
        else DoAnimate(element, val);
    }

    protected abstract void DoAnimate(FrameworkElement element, bool value);
}



