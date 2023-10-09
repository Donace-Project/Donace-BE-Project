using Donace_BE_Project.Shared.Pagination;

namespace Donace_BE_Project.Models.Event.Input;

public class PaginationEventInput : PaginationInput
{
    public DateTime FromDate { get; set; } = DateTime.MinValue;
    public DateTime ToDate { get; set; } = DateTime.MaxValue;
}
