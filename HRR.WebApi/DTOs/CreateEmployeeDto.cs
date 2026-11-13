namespace HRR.WebApi.DTOs;

public class CreateEmployeeDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Position { get; set; }
    public int DepartmentId { get; set; }
}