using Microsoft.AspNetCore.Mvc;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryController(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        // GET: api/Category
        // Public endpoint - get all categories for filtering
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories()
        {
            var categories = await _categoryRepo.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // GET: api/Category/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryById(int id)
        {
            var category = await _categoryRepo.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            return Ok(category);
        }

        // POST: api/Category
        // Admin only - create new category
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CreateCategoryDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if category name already exists
            if (await _categoryRepo.CategoryExistsAsync(dto.Name))
                return BadRequest(new { message = "Category with this name already exists" });

            var category = new Category
            {
                Name = dto.Name
            };

            var created = await _categoryRepo.CreateCategoryAsync(category);
            if (created == null)
                return StatusCode(500, new { message = "Failed to create category" });

            var categoryDto = await _categoryRepo.GetCategoryByIdAsync(created.CategoryID);
            return CreatedAtAction(nameof(GetCategoryById), new { id = created.CategoryID }, categoryDto);
        }

        // PUT: api/Category/{id}
        // Admin only - update category
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDTO>> UpdateCategory(int id, [FromBody] UpdateCategoryDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryRepo.GetCategoryEntityByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            // Check if new name conflicts with existing category
            if (await _categoryRepo.CategoryExistsAsync(dto.Name, id))
                return BadRequest(new { message = "Category with this name already exists" });

            category.Name = dto.Name;

            var updated = await _categoryRepo.UpdateCategoryAsync(category);
            if (updated == null)
                return StatusCode(500, new { message = "Failed to update category" });

            var categoryDto = await _categoryRepo.GetCategoryByIdAsync(id);
            return Ok(categoryDto);
        }

        // DELETE: api/Category/{id}
        // Admin only - delete category
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepo.GetCategoryEntityByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            // Check if category is in use
            var listingCount = await _categoryRepo.GetCategoryListingCountAsync(id);
            if (listingCount > 0)
                return BadRequest(new { message = $"Cannot delete category. It is used by {listingCount} listing(s)" });

            var success = await _categoryRepo.DeleteCategoryAsync(id);
            if (!success)
                return StatusCode(500, new { message = "Failed to delete category" });

            return Ok(new { message = "Category deleted successfully" });
        }

        // GET: api/Category/{id}/listings-count
        [HttpGet("{id}/listings-count")]
        public async Task<ActionResult<int>> GetCategoryListingCount(int id)
        {
            var count = await _categoryRepo.GetCategoryListingCountAsync(id);
            return Ok(new { categoryId = id, listingCount = count });
        }
    }
}