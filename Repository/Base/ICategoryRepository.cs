using ShelfLife.DTOs;
using ShelfLife.Models;

namespace ShelfLife.Repository.Base
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO?> GetCategoryByIdAsync(int categoryId);
        Task<Category?> GetCategoryEntityByIdAsync(int categoryId);
        Task<bool> CategoryExistsAsync(string name, int? excludeId = null);
        Task<Category?> CreateCategoryAsync(Category category);
        Task<Category?> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<int> GetCategoryListingCountAsync(int categoryId);
        Task SeedDefaultCategoriesAsync();
    }
}