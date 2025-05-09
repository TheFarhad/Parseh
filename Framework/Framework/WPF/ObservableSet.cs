namespace Framework;

using System.Collections.ObjectModel;

public interface ISet { }

public class ObservableSet<T> : ObservableCollection<T>, ISet
{
    public ObservableSet() { }
    public ObservableSet(IEnumerable<T> source) : base(source) { }
    public ObservableSet(List<T> source) : base(source) { }
}
