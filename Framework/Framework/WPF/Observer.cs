namespace Framework;

using System.Collections.ObjectModel;

public interface ISet { }

public class Observer<T> : ObservableCollection<T>, ISet
{
    public Observer() { }
    public Observer(IEnumerable<T> source) : base(source) { }
    public Observer(List<T> source) : base(source) { }
}
