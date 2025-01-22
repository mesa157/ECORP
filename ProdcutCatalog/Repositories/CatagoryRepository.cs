using Microsoft.EntityFrameworkCore;
using ProdcutCatalog.DbContexts;
using ProdcutCatalog.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdcutCatalog.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ProductCatalogDbContext _context;

        public CategoryRepository(ProductCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryById(string categoryId)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId.ToString() == categoryId);
        }

        public async Task AddCategory(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
    }
}
