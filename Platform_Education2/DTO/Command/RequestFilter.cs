namespace PlatformEduPro.DTO.Command
{
    public class RequestFilter
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 5;
        public string? SortColumn { get; init; }
        public string? SortDirection { get; init; }
    }
}
