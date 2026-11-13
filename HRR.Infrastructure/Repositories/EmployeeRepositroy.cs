using HRR.Aplication.Interfaces;
using HRR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRR.Infrastructure.Repositories;

public class EmployeeRepositroy(AppDbContext context) : IEmployeeRepository
{
    public async Task<List<Employee>> GettAllAsync() => await context.Employees.ToListAsync();

    public async Task<Employee> GetById(int id) => await context.Employees.FindAsync(id);

    public async Task AddAsync(Employee employee)
    {
        await context.Employees.AddAsync(employee);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsnc(Employee employee)
    {
        context.Employees.Update(employee);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Employee employee)
    {
        context.Employees.Remove(employee);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
    {
        return await context.Employees.AnyAsync(e => e.Email == email
            && (excludeId == null || e.Id != excludeId));
    }

    public async Task<List<Employee>> GetPagedAsync(int skip, int take)
    {
        return await context.Employees
            .OrderBy(e => e.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> CountAsync() => await context.Employees.CountAsync();
}