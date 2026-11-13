using HRR.Aplication.Interfaces;
using HRR.Aplication.Results;
using HRR.Aplication.Services;
using HRR.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace HRR.AplicationTests.Services;

public class DepartmentServiceTests
{
    private readonly Mock<IDepartmentRepository> _repo = new();
    private readonly Mock<ILogger<DepartmentService>> _logger = new();

    private DepartmentService CreateService() => new(_repo.Object, _logger.Object);


    [Fact]
    public async Task CreateAsync_WhenNameIsTaken_ReturnsConflictAndDoesNotSave()
    {
        _repo.Setup(r => r.ExistsByNameAsync("Finance", null)).ReturnsAsync(true);

        var result = await CreateService().CreateAsync("Finance", "desc");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);
        _repo.Verify(r => r.AddAsync(It.IsAny<Department>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenNameIsFree_CreatesAndSavesDepartment()
    {
        _repo.Setup(r => r.ExistsByNameAsync("Finance", null)).ReturnsAsync(false);
        Department? saved = null;
        _repo.Setup(r => r.AddAsync(It.IsAny<Department>()))
             .Callback<Department>(d => saved = d)
             .Returns(Task.CompletedTask);

        var result = await CreateService().CreateAsync("Finance", "desc");

        Assert.True(result.IsSuccess);
        Assert.NotNull(saved);
        Assert.Equal("Finance", saved!.Name);
        Assert.Equal("desc", saved.Description);
        _repo.Verify(r => r.AddAsync(It.IsAny<Department>()), Times.Once);
    }


    [Theory]
    [InlineData(1, 999, 0, 50)]
    [InlineData(0, 10, 0, 10)]
    [InlineData(-5, 10, 0, 10)]
    [InlineData(3, 20, 40, 20)]
    [InlineData(1, 0, 0, 10)]
    [InlineData(1, -3, 0, 10)]
    public async Task GetPagedAsync_NormalizesPageAndPageSize(
        int page, int pageSize, int expectedSkip, int expectedTake)
    {
        _repo.Setup(r => r.GetPagedAsync(
                 It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<bool>()))
             .ReturnsAsync(new List<Department>());
        _repo.Setup(r => r.CountAsync(It.IsAny<string?>())).ReturnsAsync(0);

        var result = await CreateService().GetPagedAsync(page, pageSize, null, null, false);

        Assert.True(result.IsSuccess);
        _repo.Verify(r => r.GetPagedAsync(expectedSkip, expectedTake, null, "id", false), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_AppliesSearchToItemsAndCount()
    {
        _repo.Setup(r => r.GetPagedAsync(0, 10, "Fin", "id", false))
             .ReturnsAsync(new List<Department> { new() { Id = 1, Name = "Finance" } });
        _repo.Setup(r => r.CountAsync("Fin")).ReturnsAsync(1);

        var result = await CreateService().GetPagedAsync(1, 10, "Fin", null, false);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.TotalCount);
        Assert.Single(result.Value.Items);
        _repo.Verify(r => r.GetPagedAsync(0, 10, "Fin", "id", false), Times.Once);
        _repo.Verify(r => r.CountAsync("Fin"), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_WithUnknownSortField_ReturnsValidationAndDoesNotQuery()
    {
        var result = await CreateService().GetPagedAsync(1, 10, null, "salary", false);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        _repo.Verify(r => r.GetPagedAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }
}
