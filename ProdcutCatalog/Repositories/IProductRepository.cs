using ProdcutCatalog.Entities;

namespace ProdcutCatalog.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts(Guid categoryId, string name, decimal? minPrice, decimal? maxPrice);
        Task<Product> GetProductById(Guid productId);
        Task AddProduct(Product product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(Guid productId);
        Task<IEnumerable<Product>> GetProducts();
    }
}
