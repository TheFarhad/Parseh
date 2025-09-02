namespace Framework;

public interface IQueryBase { }
public interface IQuery<out TData> : IRequest<TData>, IQueryBase { }

public abstract class PagingQuery<TData> : IQuery<TData>
{
    public int Page { get; set; } = 1;
    public int Take { get; set; } = 8;
    public int Skip => (Page - 1) * Take;
    public string SortBy { get; set; } = "Id";
    public bool AscSort { get; set; } = true;
    public bool FetchTotalCount { get; set; } = false;
}

public abstract record QueryData<TItem>
{
    public IReadOnlyCollection<TItem> Items { get; init; } = [];
    public int? TotalCount { get; init; } = default!;

    public QueryData() { }
}

