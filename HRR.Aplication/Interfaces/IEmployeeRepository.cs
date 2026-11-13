using HRR.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRR.Aplication.Interfaces;

public interface IEmployeeRepository
{
    Task<List<Employee>> GettAllAsync();
    Task<Employee> GetById(int id);
    Task AddAsync(Employee department);
    Task UpdateAsnc(Employee department);
    Task DeleteAsync(Employee department);
    Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);

    Task<List<Employee>> GetPagedAsync(int skip, int take);
    Task<int> CountAsync();
}
