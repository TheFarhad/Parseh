namespace Framework;

public abstract class DesignModel<Owner, Viewmodel>
    where Owner : DesignModel<Owner, Viewmodel>, new()
    where Viewmodel : ViewModel, new()
{
    public static readonly Owner Self = new();
    public readonly Viewmodel Model = new();
}