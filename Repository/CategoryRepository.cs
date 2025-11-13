using Microsoft.EntityFrameworkCore;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DBcontext _context;

        public CategoryRepository(DBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDTO
                {
                    CategoryID = c.CategoryID,
                    Name = c.Name,
                    ListingCount = c.BookListings.Count
                })
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories
                .Where(c => c.CategoryID == categoryId)
                .Select(c => new CategoryDTO
                {
                    CategoryID = c.CategoryID,
                    Name = c.Name,
                    ListingCount = c.BookListings.Count
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Category?> GetCategoryEntityByIdAsync(int categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task<bool> CategoryExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.Categories.Where(c => c.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.CategoryID != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<Category?> CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCategoryListingCountAsync(int categoryId)
        {
            return await _context.BookListings
                .CountAsync(b => b.CategoryID == categoryId);
        }

        public async Task SeedDefaultCategoriesAsync()
        {
            if (await _context.Categories.AnyAsync())
                return; // Already seeded

            var defaultCategories = new List<Category>
            {
                new Category { Name = "Fiction" },
                new Category { Name = "Non-Fiction" },
                new Category { Name = "Textbooks" },
                new Category { Name = "Science" },
                new Category { Name = "History" },
                new Category { Name = "Biography" },
                new Category { Name = "Self-Help" },
                new Category { Name = "Business" },
                new Category { Name = "Technology" },
                new Category { Name = "Children's Books" },
                new Category { Name = "Comics & Graphic Novels" },
                new Category { Name = "Poetry" },
                new Category { Name = "Art & Photography" },
                new Category { Name = "Cooking" },
                new Category { Name = "Religion & Spirituality" },
                new Category { Name = "Travel" },
                new Category { Name = "Other" }
            };

            await _context.Categories.AddRangeAsync(defaultCategories);
            await _context.SaveChangesAsync();
        }
    }
}