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
    public class EmployeeController(IEmployeeService employeeService) : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<EmployeeDto>>> GetAll([FromQuery] PaginationDto query)
        {
            var result = await employeeService.GetPagedAsync(query.Page, query.PageSize);
            if (!result.IsSuccess)
                return ToErrorResponse(result);

            var paged = result.Value!;

            var response = new PagedResult<EmployeeDto>
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
        public async Task<ActionResult> Create(CreateEmployeeDto request)
        {
            var emp = await employeeService.CreateAsync(request.FullName, request.Email, request.Position, request.DepartmentId);
            if (!emp.IsSuccess)
                return ToErrorResponse(emp);

            var dto = ToDto(emp.Value!);

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetById(int id)
        {
            var emp = await employeeService.GetByIdAsync(id);
            if (!emp.IsSuccess)
                return ToErrorResponse(emp);

            return Ok(ToDto(emp.Value!));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateEmployee employee)
        {
            var emp = await employeeService.UpdateAsync(id, employee.FullName, employee.Email, employee.Position, employee.DepartmentId);
            if (!emp.IsSuccess)
                return ToErrorResponse(emp);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var emp = await employeeService.DeleteAsync(id);
            if (!emp.IsSuccess)
                return ToErrorResponse(emp);

            return NoContent();
        }

        private static EmployeeDto ToDto(Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Position = employee.Position,
                DepartmentId = employee.DepartmentId
            };
        }
    }
}
