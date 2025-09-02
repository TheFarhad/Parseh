namespace Framework;

using System.Collections.ObjectModel;

public interface ISet { }

public class MvvmSet<T> : ObservableCollection<T>, ISet
{
    public MvvmSet() { }
    public MvvmSet(IEnumerable<T> source) : base(source) { }
    public MvvmSet(List<T> source) : base(source) { }
}
