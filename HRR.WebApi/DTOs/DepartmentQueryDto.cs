namespace HRR.WebApi.DTOs;

public class DepartmentQueryDto : PaginationDto
{
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}
