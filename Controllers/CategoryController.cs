using EventParkingReservation.DTOs.Category;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController :
        ControllerBase
    {
        private readonly ICategoryService
            _service;

        public CategoryController(
            ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(
                await _service.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult>
            GetById(int id)
        {
            try
            {
                return Ok(
                    await _service
                        .GetByIdAsync(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Create(CreateCategoryDto dto)
        {
            try
            {
                var result =
                    await _service
                        .CreateAsync(dto);

                return StatusCode(
                    StatusCodes
                        .Status201Created,
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Update(
                int id,
                UpdateCategoryDto dto)
        {
            try
            {
                return Ok(
                    await _service
                        .UpdateAsync(
                            id,
                            dto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
