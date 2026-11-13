using HRR.Aplication.Results;
using HRR.Domain.Entities;

namespace HRR.Aplication.Services;

public interface IEmployeeService
{
    Task<Result<List<Employee>>> GetAllAsync();
    Task<Result<PagedResult<Employee>>> GetPagedAsync(int page, int pageSize);
    Task<Result<Employee?>> GetByIdAsync(int id);
    Task<Result<Employee?>> CreateAsync(string fullName, string email, string position, int departmentId);
    Task<Result<Employee?>> UpdateAsync(int id, string fullName, string email, string position, int departmentId);
    Task<Result> DeleteAsync(int id);
}