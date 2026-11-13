

using HRR.Domain.Entities;

namespace HRR.Aplication.Interfaces;

public interface IDepartmentRepository
{
    Task<List<Department>> GetAllAsync();
    Task<Department> GetByIdAsync(int id);
    Task AddAsync(Department department);
    Task UpdateAsnc(Department department);
    Task DeleteAsync(Department department);
    Task<bool> HasEmployee(int id);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

    Task<List<Department>> GetPagedAsync(int skip, int take, string? search, string sortBy, bool descending);
    Task<int> CountAsync(string? search);

}
