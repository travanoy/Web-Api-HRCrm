using HRR.Aplication.Results;
using HRR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRR.Aplication.Services;

public interface IDepartmentService
{
    Task<Result<List<Department>>> GetAllAsync();
    Task<Result<PagedResult<Department>>> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, bool descending);
    Task<Result<Department>> GetByIdAsync(int id);

    Task<Result<Department>> CreateAsync(string name, string description);

    Task<Result>  UpdateAsync(int id, string name, string description);

    Task<Result>DeleteAsync(int id);
}
