namespace Framework;

public abstract class DesignModel<Owner, Viewmodel>
    where Owner : DesignModel<Owner, Viewmodel>, new()
    where Viewmodel : ViewModel, new()
{
    public static readonly Owner Defualt = new();
    public Viewmodel Model { get; set; } = default!;

    protected DesignModel() => Model = new();
}