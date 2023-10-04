namespace Donace_BE_Project.Shared.Pagination;

public class PaginationOutput<T>
{
    public virtual int TotalCount { get; set; }
    public virtual List<T>  Items { get; set; } = new();
}
