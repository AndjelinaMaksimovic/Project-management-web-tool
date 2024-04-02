using Codedberries.Helpers;
using Codedberries.Models.DTOs;
using Codedberries.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codedberries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("createNewCategory")]
        public async Task<IActionResult> CreateNewCategory([FromBody] CreateCategoryDTO request)
        {
            try
            {
                await _categoryService.CreateNewCategory(HttpContext, request);

                return Ok("Category created successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred: {ex.Message}"));
            }
        }

        [HttpPost("allProjectCategories")]
        public async Task<IActionResult> GetAllProjectCategories([FromBody] ProjectIdDTO request)
        {
            try
            {
                var categories = await _categoryService.GetAllProjectCategories(HttpContext, request);
                
                return Ok(categories);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred: {ex.Message}"));
            }
        }
    }
}
