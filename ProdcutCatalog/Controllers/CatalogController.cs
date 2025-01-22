using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProdcutCatalog.Entities;
using ProdcutCatalog.Models;
using ProdcutCatalog.Repositories;

namespace ProductCatalog.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var result = await _productRepository.GetProducts();
            return Ok(_mapper.Map<List<ProductDto>>(result));
        }

        // Get all products, with filtering options
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Get(
            [FromQuery] Guid categoryId,
            [FromQuery] string name,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var result = await _productRepository.GetProducts(categoryId, name, minPrice, maxPrice);
            return Ok(_mapper.Map<List<ProductDto>>(result));
        }

        // Get product by ID
        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductDto>> GetById(Guid productId)
        {
            var result = await _productRepository.GetProductById(productId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ProductDto>(result));
        }

        // Create a new product
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _productRepository.AddProduct(product);
            return CreatedAtAction(nameof(GetById), new { productId = product.ProductId }, productDto);
        }

        // Update product details
        [HttpPut("{productId}")]
        public async Task<ActionResult> Update(Guid productId, ProductDto productDto)
        {
            var existingProduct = await _productRepository.GetProductById(productId);
            if (existingProduct == null)
            {
                return NotFound();
            }

            var product = _mapper.Map(productDto, existingProduct);
            await _productRepository.UpdateProduct(product);
            return NoContent();
        }

        // Delete a product
        [HttpDelete("{productId}")]
        public async Task<ActionResult> Delete(Guid productId)
        {
            var existingProduct = await _productRepository.GetProductById(productId);
            if (existingProduct == null)
            {
                return NotFound();
            }

            await _productRepository.DeleteProduct(productId);
            return NoContent();
        }
    }
}

