using HRR.Aplication.Interfaces;
using HRR.Aplication.Results;
using HRR.Aplication.Services;
using HRR.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace HRR.AplicationTests.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _employees = new();
    private readonly Mock<IDepartmentRepository> _departments = new();
    private readonly Mock<ILogger<EmployeeService>> _logger = new();

    private EmployeeService CreateService() => new(_employees.Object, _departments.Object, _logger.Object);


    [Fact]
    public async Task CreateAsync_WhenDepartmentDoesNotExist_ReturnsValidationAndDoesNotSave()
    {
        _departments.Setup(r => r.GetByIdAsync(7)).ReturnsAsync((Department)null!);

        var result = await CreateService().CreateAsync("Bob", "bob@x.com", "dev", 7);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        _employees.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailIsTaken_ReturnsConflictAndDoesNotSave()
    {
        _departments.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Department { Id = 1, Name = "IT" });
        _employees.Setup(r => r.ExistsByEmailAsync("bob@x.com", null)).ReturnsAsync(true);

        var result = await CreateService().CreateAsync("Bob", "bob@x.com", "dev", 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);
        _employees.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenDepartmentExistsAndEmailIsFree_CreatesEmployee()
    {
        _departments.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Department { Id = 1, Name = "IT" });
        _employees.Setup(r => r.ExistsByEmailAsync("bob@x.com", null)).ReturnsAsync(false);
        Employee? saved = null;
        _employees.Setup(r => r.AddAsync(It.IsAny<Employee>()))
                  .Callback<Employee>(e => saved = e)
                  .Returns(Task.CompletedTask);

        var result = await CreateService().CreateAsync("Bob", "bob@x.com", "dev", 1);

        Assert.True(result.IsSuccess);
        Assert.NotNull(saved);
        Assert.Equal("Bob", saved!.FullName);
        Assert.Equal("bob@x.com", saved.Email);
        Assert.Equal("dev", saved.Position);
        Assert.Equal(1, saved.DepartmentId);
        _employees.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
    }


    [Fact]
    public async Task UpdateAsync_WhenEmailIsTakenByAnotherEmployee_ReturnsConflictAndDoesNotSave()
    {
        _employees.Setup(r => r.GetById(5))
                  .ReturnsAsync(new Employee { Id = 5, FullName = "Bob", Email = "bob@x.com", Position = "dev", DepartmentId = 1 });
        _departments.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Department { Id = 1, Name = "IT" });
        _employees.Setup(r => r.ExistsByEmailAsync("taken@x.com", 5)).ReturnsAsync(true);

        var result = await CreateService().UpdateAsync(5, "Bob", "taken@x.com", "dev", 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);
        _employees.Verify(r => r.UpdateAsnc(It.IsAny<Employee>()), Times.Never);
    }


    [Fact]
    public async Task DeleteAsync_WhenEmployeeDoesNotExist_ReturnsNotFoundAndDoesNotDelete()
    {
        _employees.Setup(r => r.GetById(99)).ReturnsAsync((Employee)null!);

        var result = await CreateService().DeleteAsync(99);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
        _employees.Verify(r => r.DeleteAsync(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenEmployeeExists_DeletesIt()
    {
        var employee = new Employee { Id = 3, FullName = "Bob", Email = "bob@x.com" };
        _employees.Setup(r => r.GetById(3)).ReturnsAsync(employee);

        var result = await CreateService().DeleteAsync(3);

        Assert.True(result.IsSuccess);
        _employees.Verify(r => r.DeleteAsync(employee), Times.Once);
    }
}
