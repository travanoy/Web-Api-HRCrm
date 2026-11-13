using HRR.Aplication.Interfaces;
using HRR.Aplication.Results;
using HRR.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRR.Aplication.Services;

public class DepartmentService(
    IDepartmentRepository departmentrepos,
    ILogger<DepartmentService> logger) : IDepartmentService
{
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 50;
    private static readonly string[] AllowedSortFields = { "id", "name", "description" };

    public async Task<Result<List<Department>>> GetAllAsync()
    {

        var res = await departmentrepos.GetAllAsync();
        return Result<List<Department>>.Success(res);
    }

    public async Task<Result<PagedResult<Department>>> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, bool descending)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = DefaultPageSize;

        if (pageSize > MaxPageSize)
            pageSize = MaxPageSize;

        var sortField = string.IsNullOrWhiteSpace(sortBy) ? "id" : sortBy.Trim().ToLowerInvariant();

        if (!AllowedSortFields.Contains(sortField))
        {
            logger.LogWarning("Unknown sort field {SortBy} requested", sortBy);
            return Result<PagedResult<Department>>.Failure(
                $"Unknown sort field '{sortBy}'. Allowed fields: {string.Join(", ", AllowedSortFields)}.",
                ErrorType.Validation);
        }

        var skip = (page - 1) * pageSize;

        var departments = await departmentrepos.GetPagedAsync(skip, pageSize, search, sortField, descending);
        var totalCount = await departmentrepos.CountAsync(search);

        var result = new PagedResult<Department>()
        {
            Items = departments,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return Result<PagedResult<Department>>.Success(result);
    }

    public async Task<Result<Department?>> GetByIdAsync(int id)
    {
        var res =await departmentrepos.GetByIdAsync(id);
        if (res == null)
        {
            logger.LogWarning("Department with {Id} does not exist", id);
            return Result<Department>.Failure("Not Found BY Id", ErrorType.NotFound);
        }
        return Result<Department?>.Success(res);
    }

    public async Task<Result<Department>> CreateAsync(string name, string description)
    {
        if (await departmentrepos.ExistsByNameAsync(name))
        {
            logger.LogWarning("Cant create the department with name {Name} due to duplicate", name);
            return Result<Department>.Failure("Can not insert dublicate departments", ErrorType.Conflict);
        }

        var dep = new Department() { Name = name, Description = description };

        await departmentrepos.AddAsync(dep);

        return Result<Department>.Success(dep);
    }

    public async Task<Result> UpdateAsync(int id, string name, string description)
    {
        var dep = await departmentrepos.GetByIdAsync(id);
        if (dep == null)
        {
            logger.LogWarning("Department with {Id} does not exist", id);
            return Result.Failure("not found by id.", ErrorType.NotFound);
        }

        if (await departmentrepos.ExistsByNameAsync(name, id))
        {
            logger.LogWarning("Cant update the department with name {Name} due to duplicate", name);
            return Result.Failure("Can not insert dublicate departments", ErrorType.Conflict);
        }

        dep.Name = name;
        dep.Description = description;

        await departmentrepos.UpdateAsnc(dep);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var dep = await departmentrepos.GetByIdAsync(id);


        if (dep is null)
        {
            logger.LogWarning("Department with {Id} does not exist", id);
            return Result.Failure("not found by id", ErrorType.NotFound);
        }

        
        var hasEmployees =
            await departmentrepos.HasEmployee(id);

        if (hasEmployees == true)
        {
            logger.LogWarning("Cant delete department {Name} since it contains employees", dep.Name);
            return Result.Failure("HAs emloyees",ErrorType.Conflict);
        }

        
        await departmentrepos.DeleteAsync(dep);
        return Result.Success();


    }


}
