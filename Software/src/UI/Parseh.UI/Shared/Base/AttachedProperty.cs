namespace Parseh.UI;

public abstract class AttachedProperty<Owner, Property>
    where Owner : AttachedProperty<Owner, Property>, new()
{
    public static readonly Owner _default = /*_default ??*/ new();
    protected event Action<DependencyObject, DependencyPropertyChangedEventArgs> PropertyChanged = (sender, e) => { };
    protected event Func<DependencyObject, object, object> CoerceValue = (sender, value) => { return value; };

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty
        .RegisterAttached("Value",
            typeof(Property),
            typeof(AttachedProperty<Owner, Property>),
            new UIPropertyMetadata(default(Property), new PropertyChangedCallback(OnPropertyChangedCallback), new CoerceValueCallback(OnCoerceValueCallback)));

    public static Property GetValue(DependencyObject uielement) => uielement.GetValue(ValueProperty).As<Property>();
    public static void SetValue(DependencyObject uielement, Property value) => uielement.SetValue(ValueProperty, value);

    public virtual void OnCoerceValue(DependencyObject uielement, object value) { }
    public virtual void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e) { }

    #region Private Functionality

    /*
     کوئرس دقیقا یک مرحله قبل از ولیوچنج اتفاق می افتد
     بیشتر برای اعتبارسنجی ولیو ارسال شده صورت میگیرد
     این متود موقعی اتفاق می افتد که اتچ پراپرتی تغییر کند، حتی اگر مقدار جاری با قبلی یکی باشد
     مثلا اگر ولیو نامعتبر بود خودمان در متود کوئرس، آن را به مقدار مورد نطر تغییر میدهیم
     و این مقدار تغییر یافته به متود ولیوچنجد ارسال می شود
     سپس آنجا تصمیم گرفته می شود که در ازای این مقدار، چه واکنشی داشته باشیم
     */
    static object OnCoerceValueCallback(DependencyObject uielement, object value)
    {
        _default?.OnCoerceValue(uielement, value);
        _default?.CoerceValue(uielement, value);
        return value;
    }

    static void OnPropertyChangedCallback(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        _default?.PropertyChanged(uielement, e);
        _default?.OnPropertyChanged(uielement, e);
    }

    #endregion
}

public abstract class SimpleAnimatedProperty<TOwner> : AttachedProperty<TOwner, bool>
    where TOwner : AttachedProperty<TOwner, bool>, new()
{
    public override void OnCoerceValue(DependencyObject uielement, object value)
    {
        uielement?.Is<FrameworkElement>(element =>
        {
            if (element.GetValue(ValueProperty) == value)
                return;

            DoAnimate(element, value.As<bool>());
        });
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



