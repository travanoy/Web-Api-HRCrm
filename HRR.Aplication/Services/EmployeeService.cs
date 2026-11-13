using HRR.Aplication.Interfaces;
using HRR.Aplication.Results;
using HRR.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HRR.Aplication.Services;

public class EmployeeService(
    IEmployeeRepository employeeRepository,
    IDepartmentRepository departmentRepository,
    ILogger<EmployeeService> logger) : IEmployeeService
{
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 50;

    public async Task<Result<List<Employee>>> GetAllAsync()
    {
        var res = await employeeRepository.GettAllAsync();
        return Result<List<Employee>>.Success(res);
    }

    public async Task<Result<PagedResult<Employee>>> GetPagedAsync(int page, int pageSize)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = DefaultPageSize;

        if (pageSize > MaxPageSize)
            pageSize = MaxPageSize;

        var skip = (page - 1) * pageSize;

        var employees = await employeeRepository.GetPagedAsync(skip, pageSize);
        var totalCount = await employeeRepository.CountAsync();

        return Result<PagedResult<Employee>>.Success(new PagedResult<Employee>
        {
            Items = employees,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    public async Task<Result<Employee?>> GetByIdAsync(int id)
    {
        var res = await employeeRepository.GetById(id);
        if (res == null)
        {
            logger.LogWarning("Employee with {Id} does not exist", id);
            return Result<Employee>.Failure("not found", ErrorType.NotFound);
        }
        return Result<Employee?>.Success(res);
    }


    public async Task<Result<Employee?>> CreateAsync(string fullName, string email, string position, int departmentId)
    {
        if (await departmentRepository.GetByIdAsync(departmentId) is null)
        {
            logger.LogWarning("Department with {Id} does not exist", departmentId);
            return Result<Employee>.Failure($"Department with id {departmentId} was not found.", ErrorType.Validation);
        }

        if (await employeeRepository.ExistsByEmailAsync(email))
        {
            logger.LogWarning("Cant create the employee with email {Email} due to duplicate", email);
            return Result<Employee>.Failure("Can not insert dublicate employees", ErrorType.Conflict);
        }

        var employee = new Employee
        {
            FullName = fullName,
            Email = email,
            Position = position,
            DepartmentId = departmentId
        };

        await employeeRepository.AddAsync(employee);
        return Result<Employee>.Success(employee);
    }

    public async Task<Result<Employee?>> UpdateAsync(int id, string fullName, string email, string position, int departmentId)
    {
        var employee = await employeeRepository.GetById(id);
        if (employee is null)
        {
            logger.LogWarning("Employee with {Id} does not exist", id);
            return Result<Employee>.Failure("Not Found by id", ErrorType.NotFound);
        }

        if (await departmentRepository.GetByIdAsync(departmentId) is null)
        {
            logger.LogWarning("Department with {Id} does not exist", departmentId);
            return Result<Employee>.Failure("Department not found", ErrorType.NotFound);
        }

        if (await employeeRepository.ExistsByEmailAsync(email, id))
        {
            logger.LogWarning("Cant update the employee with email {Email} due to duplicate", email);
            return Result<Employee>.Failure("Can not insert dublicate employees", ErrorType.Conflict);
        }

        employee.FullName = fullName;
        employee.Email = email;
        employee.Position = position;
        employee.DepartmentId = departmentId;

        await employeeRepository.UpdateAsnc(employee);
        return Result<Employee>.Success(employee);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var employee = await employeeRepository.GetById(id);
        if (employee is null)
        {
            logger.LogWarning("Employee with {Id} does not exist", id);
            return Result.Failure("Employee not found",ErrorType.NotFound);
        }

        await employeeRepository.DeleteAsync(employee);
        return Result.Success();
    }
}