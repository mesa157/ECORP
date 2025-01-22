//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Caching.Memory;
//using ProdcutCatalog.Entities;
//using ProdcutCatalog.Models;
//using ProdcutCatalog.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace ProdcutCatalog.Controllers
//{
//    [Route("api/products")]
//    [ApiController]
//    public class ProductController : ControllerBase
//    {
//        private readonly IProductRepository _productRepository; // Dependency for data access
//        private readonly IMemoryCache _cache; // Dependency for caching

//        public ProductController(IProductRepository productRepository, IMemoryCache cache)
//        {
//            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
//            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
//        }

//        // Get all products with optional caching
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<ProductDto>>> Get(
//            [FromQuery] Guid? categoryId = null,
//            [FromQuery] string name = null,
//            [FromQuery] decimal? minPrice = null,
//            [FromQuery] decimal? maxPrice = null)
//        {
//            string cacheKey = $"products_{categoryId}_{name}_{minPrice}_{maxPrice}";

//            // Attempt to retrieve from cache
//            if (!_cache.TryGetValue(cacheKey, out List<ProductDto> products))
//            {
//                // Fetch from the database if not in cache
//                var result = await _productRepository.GetProducts(categoryId ?? Guid.Empty, name, minPrice, maxPrice);

//                products = new List<ProductDto>();
//                foreach (var product in result)
//                {
//                    products.Add(new ProductDto
//                    {
//                        ProductId = product.ProductId,
//                        Name = product.Name,
//                        Price = product.Price,
//                        Description = product.Description,
//                        ImageUrl = product.ImageUrl,
//                        CategoryId = product.CategoryId,
//                        CategoryName = product.Category?.Name // Safeguard against null Category
//                    });
//                }

//                // Cache the results for 10 minutes
//                _cache.Set(cacheKey, products, TimeSpan.FromMinutes(10));
//            }

//            return Ok(products);
//        }

//        // Get product by ID with caching
//        [HttpGet("{productId}")]
//        public async Task<ActionResult<ProductDto>> GetById(Guid productId)
//        {
//            if (productId == Guid.Empty)
//            {
//                return BadRequest("Invalid product ID.");
//            }

//            string cacheKey = $"product_{productId}";

//            if (!_cache.TryGetValue(cacheKey, out ProductDto product))
//            {
//                var result = await _productRepository.GetProductById(productId);
//                if (result == null)
//                {
//                    return NotFound($"Product with ID {productId} not found.");
//                }

//                product = new ProductDto
//                {
//                    ProductId = result.ProductId,
//                    Name = result.Name,
//                    Price = result.Price,
//                    Description = result.Description,
//                    ImageUrl = result.ImageUrl,
//                    CategoryId = result.CategoryId,
//                    CategoryName = result.Category?.Name // Safeguard against null Category
//                };

//                // Cache the product for 10 minutes
//                _cache.Set(cacheKey, product, TimeSpan.FromMinutes(10));
//            }

//            return Ok(product);
//        }
//    }
//}
