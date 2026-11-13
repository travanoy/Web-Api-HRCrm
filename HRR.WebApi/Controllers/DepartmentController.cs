using HRR.Aplication.Results;
using HRR.Aplication.Services;
using HRR.Domain.Entities;
using HRR.WebApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRR.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentController(IDepartmentService departmentService) : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<DepartmentDto>>> GetAll([FromQuery] DepartmentQueryDto query)
        {
            var descending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            var result = await departmentService.GetPagedAsync(query.Page, query.PageSize, query.Search, query.SortBy, descending);
            if (!result.IsSuccess)
                return ToErrorResponse(result);

            var paged = result.Value!;

            var response = new PagedResult<DepartmentDto>
            {
                Items = paged.Items.Select(ToDto).ToList(),
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Create")]
        public async Task<ActionResult> Create(CreateDepartmentDto request)
        {
            var dep = await departmentService.CreateAsync(request.Name, request.Description);
            if (!dep.IsSuccess)
                return ToErrorResponse(dep);

            var dto = ToDto(dep.Value!);

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetById(int id)
        {
            var dep = await departmentService.GetByIdAsync(id);
            if (!dep.IsSuccess)
                return ToErrorResponse(dep);

            return Ok(ToDto(dep.Value!));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateDepartment department)
        {
            var result = await departmentService.UpdateAsync(id, department.Name, department.Description);
            return result.IsSuccess
                ? NoContent()
                : ToErrorResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await departmentService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return ToErrorResponse(result);
        }

        private static DepartmentDto ToDto(Department department)
        {
            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };
        }
    }
}
