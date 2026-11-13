using HRR.Aplication.Interfaces;
using HRR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRR.Infrastructure.Repositories;

public class DepartmentRepository(AppDbContext context) : IDepartmentRepository
{
    public async Task AddAsync(Department department)
    {
        await context.Departments.AddAsync(department);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Department department)
    {
        context.Departments.Remove(department);
        await context.SaveChangesAsync();
    }

    public async Task<Department> GetByIdAsync(int id) => await context.Departments.FindAsync(id);
   

    public async Task<List<Department>> GetAllAsync()=>await context.Departments.ToListAsync();

    public async Task<bool> HasEmployee(int DepartmentId)
    {
        return await context.Employees.AnyAsync(x => x.DepartmentId == DepartmentId);
    }

    public async Task UpdateAsnc(Department department)
    {
       context.Departments.Update(department);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        return await context.Departments.AnyAsync(x => x.Name == name
            && (excludeId == null || x.Id != excludeId));
    }

    public async Task<List<Department>> GetPagedAsync(int skip, int take, string? search, string sortBy, bool descending)
    {
        var query = ApplySearch(context.Departments.AsQueryable(), search);
        query = ApplySort(query, sortBy, descending);

        return await query
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> CountAsync(string? search)
    {
        return await ApplySearch(context.Departments.AsQueryable(), search).CountAsync();
    }

    private static IQueryable<Department> ApplySearch(IQueryable<Department> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;

        return query.Where(x => x.Name.Contains(search));
    }

    private static IQueryable<Department> ApplySort(IQueryable<Department> query, string sortBy, bool descending)
    {
        return sortBy switch
        {
            "name" => descending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            "description" => descending ? query.OrderByDescending(x => x.Description) : query.OrderBy(x => x.Description),
            _ => descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
        };
    }
}
