namespace Framework;

public interface IQueryRequest<out TData> : IRequest<TData> { }

public abstract class QueryRequest<TData> : IQueryRequest<TData>
{
    public int Page { get; set; } = 1;
    public int Take { get; set; } = 8;
    public int Skip => (Page - 1) * Take;
    public string SortBy { get; set; } = "Id";
    public bool AscSort { get; set; } = true;
    public bool FetchTotalCount { get; set; } = false;
}

public abstract record QueryData<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = [];
    public int? TotalCount { get; init; } = default!;

    public QueryData() { }
}

