namespace Framework;

using System.Collections.ObjectModel;

public interface ISet { }

public class Set<T> : ObservableCollection<T>, ISet
{
    public Set() { }
    public Set(IEnumerable<T> source) : base(source) { }
    public Set(List<T> source) : base(source) { }
}
