using Microsoft.EntityFrameworkCore;
using ProdcutCatalog.DbContexts;
using ProdcutCatalog.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdcutCatalog.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductCatalogDbContext _context;

        public ProductRepository(ProductCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var query = _context.Products.Include(x => x.Category).AsQueryable();

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts(Guid categoryId, string name, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products.Include(x => x.Category).AsQueryable();

            if (categoryId != Guid.Empty)
                query = query.Where(p => p.CategoryId == categoryId);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            return await query.ToListAsync();
        }

        public async Task<Product> GetProductById(Guid productId)
        {
            return await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(Guid productId)
        {
            var product = await GetProductById(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
