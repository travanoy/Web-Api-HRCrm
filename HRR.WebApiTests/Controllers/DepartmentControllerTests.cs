using HRR.Aplication.Results;
using HRR.Aplication.Services;
using HRR.Domain.Entities;
using HRR.WebApi.Controllers;
using HRR.WebApi.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HRR.WebApiTests.Controllers;

public class DepartmentControllerTests
{
    private readonly Mock<IDepartmentService> _service = new();

    private DepartmentController CreateController()
    {
        return new DepartmentController(_service.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }


    [Fact]
    public async Task Create_WhenNameConflict_Returns409()
    {
        _service.Setup(s => s.CreateAsync("Finance", "desc"))
                .ReturnsAsync(Result<Department>.Failure("duplicate", ErrorType.Conflict));

        var response = await CreateController()
            .Create(new CreateDepartmentDto { Name = "Finance", Description = "desc" });

        var result = Assert.IsType<ObjectResult>(response);
        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
    }

    [Fact]
    public async Task Create_WhenValid_ReturnsCreatedAtAction()
    {
        _service.Setup(s => s.CreateAsync("Finance", "desc"))
                .ReturnsAsync(Result<Department>.Success(new Department { Id = 10, Name = "Finance", Description = "desc" }));

        var response = await CreateController()
            .Create(new CreateDepartmentDto { Name = "Finance", Description = "desc" });

        var created = Assert.IsType<CreatedAtActionResult>(response);
        var dto = Assert.IsType<DepartmentDto>(created.Value);
        Assert.Equal(10, dto.Id);
        Assert.Equal("Finance", dto.Name);
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        _service.Setup(s => s.GetByIdAsync(99))
                .ReturnsAsync(Result<Department>.Failure("not found", ErrorType.NotFound));

        var response = await CreateController().GetById(99);

        var result = Assert.IsType<ObjectResult>(response.Result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GetAll_WhenSuccess_ReturnsPagedDtos()
    {
        _service.Setup(s => s.GetPagedAsync(1, 10, null, null, false))
                .ReturnsAsync(Result<PagedResult<Department>>.Success(new PagedResult<Department>
                {
                    Items = new List<Department> { new() { Id = 1, Name = "IT", Description = "d" } },
                    Page = 1,
                    PageSize = 10,
                    TotalCount = 1
                }));

        var response = await CreateController().GetAll(new DepartmentQueryDto());

        var ok = Assert.IsType<OkObjectResult>(response.Result);
        var paged = Assert.IsType<PagedResult<DepartmentDto>>(ok.Value);
        Assert.Equal(1, paged.TotalCount);
        Assert.Single(paged.Items);
        Assert.Equal("IT", paged.Items[0].Name);
    }
}
