using Microsoft.Extensions.Caching.Memory;
using UnifiedFrontend.Models.ProductCatalogModel;

namespace UnifiedFrontend.Services.ProductCatagory
{
    public class ProductCatalogApi
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public ProductCatalogApi(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            return await _cache.GetOrCreateAsync("categories", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await FetchCategoriesFromApi();
            });
        }

        public async Task<List<ProductViewModel>> GetProductsAsync(Guid? categoryId = null, string name = null, int page = 1, int pageSize = 10)
        {
            try
            {
                var url = $"api/products?page={page}&pageSize={pageSize}";

                if (categoryId.HasValue)
                    url += $"&categoryId={categoryId.Value}";

                if (!string.IsNullOrEmpty(name))
                    url += $"&name={Uri.EscapeDataString(name)}";

                return await _httpClient.GetFromJsonAsync<List<ProductViewModel>>(url);
            }
            catch (Exception ex)
            {
                // Log error
                return new List<ProductViewModel>();
            }
        }

        public async Task<List<ProductViewModel>> GetAllProductsAsync()
        {
            return await _cache.GetOrCreateAsync("allProducts", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                try
                {
                    // Fetch all products from the API without pagination
                    return await _httpClient.GetFromJsonAsync<List<ProductViewModel>>("api/products/all");
                }
                catch (Exception ex)
                {
                    // Log error
                    return new List<ProductViewModel>();
                }
            });
        }

        private async Task<List<CategoryViewModel>> FetchCategoriesFromApi()
        {
            try
            {
                var categories = await _httpClient.GetFromJsonAsync<List<CategoryViewModel>>("api/categories");
                foreach (var category in categories)
                {
                    category.Products = await GetProductsAsync(category.CategoryId);
                }
                return categories;
            }
            catch (Exception ex)
            {
                // Log error
                return new List<CategoryViewModel>();
            }
        }

        public async Task<ProductViewModel> GetProductByIdAsync(Guid productId)
        {
            return await _httpClient.GetFromJsonAsync<ProductViewModel>($"api/products/{productId}");
        }
    }
}
