using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using UnifiedFrontend.Models.CartModel;
using UnifiedFrontend.Models.ProductCatalogModel;

namespace UnifiedFrontend.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CartController> _logger;

        public CartController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<CartController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IActionResult> Index(Guid userId)
        {
            var apiUrl = _configuration["BackendServices2:CartService"];
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync($"{apiUrl}/api/shoppingbasket/{userId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch shopping basket for UserId: {UserId}. StatusCode: {StatusCode}", userId, response.StatusCode);
                    ViewBag.ErrorMessage = "Failed to load your cart. Please try again.";
                    return View(new ShoppingBasket { UserId = userId });
                }

                var basket = await response.Content.ReadFromJsonAsync<ShoppingBasket>();
                _logger.LogInformation("Loaded shopping basket for UserId: {UserId}. Items: {ItemCount}", userId, basket?.BasketLines?.Count ?? 0);
                return View(basket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching shopping basket for UserId: {UserId}", userId);
                ViewBag.ErrorMessage = "An error occurred while loading your cart.";
                return View(new ShoppingBasket { UserId = userId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid userId, Guid productId, int quantity)
        {
            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Quantity must be at least 1.";
                return RedirectToAction("Index", new { userId });
            }

            var cartApiUrl = _configuration["BackendServices2:CartService"];
            var productApiUrl = _configuration["BackendServices1:ProductCatalog"];
            var client = _httpClientFactory.CreateClient();

            try
            {
                // Fetch product details
                var productResponse = await client.GetAsync($"{productApiUrl}/api/products/{productId}");
                if (!productResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to fetch product details.";
                    _logger.LogWarning("Failed to fetch product details for ProductId: {ProductId}. StatusCode: {StatusCode}", productId, productResponse.StatusCode);
                    return RedirectToAction("Index", new { userId });
                }

                var product = await productResponse.Content.ReadFromJsonAsync<ProductViewModel>();

                // Add product to cart
                var basketLine = new BasketLine
                {
                    ProductId = productId,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity
                };

                var cartResponse = await client.PostAsJsonAsync($"{cartApiUrl}/api/shoppingbasket/{userId}/basketlines", basketLine);
                if (!cartResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to add the product to your cart.";
                    _logger.LogWarning("Failed to add product {ProductId} to cart for UserId: {UserId}. StatusCode: {StatusCode}", productId, userId, cartResponse.StatusCode);
                    return RedirectToAction("Index", new { userId });
                }

                TempData["SuccessMessage"] = "Product added to cart successfully.";

                // Fetch updated cart
                return RedirectToAction("Index", new { userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart for UserId: {UserId}", productId, userId);
                TempData["ErrorMessage"] = "An error occurred while adding the product to your cart.";
                return RedirectToAction("Index", new { userId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(Guid userId, Guid productId, int quantity)
        {
            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Quantity must be at least 1.";
                return RedirectToAction("Index", new { userId });
            }

            var apiUrl = _configuration["BackendServices2:CartService"];
            var client = _httpClientFactory.CreateClient();

            try
            {
                var payload = new { Quantity = quantity };
                var response = await client.PutAsJsonAsync($"{apiUrl}/api/shoppingbasket/{userId}/basketlines/{productId}", payload);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to update the quantity.";
                    _logger.LogWarning("Failed to update quantity for product {ProductId} in cart for UserId: {UserId}. StatusCode: {StatusCode}", productId, userId, response.StatusCode);
                    return RedirectToAction("Index", new { userId });
                }

                TempData["SuccessMessage"] = "Quantity updated successfully.";
                return RedirectToAction("Index", new { userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quantity for product {ProductId} in cart for UserId: {UserId}", productId, userId);
                TempData["ErrorMessage"] = "An error occurred while updating the quantity.";
                return RedirectToAction("Index", new { userId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(Guid userId, Guid productId)
        {
            var apiUrl = _configuration["BackendServices2:CartService"];
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.DeleteAsync($"{apiUrl}/api/shoppingbasket/{userId}/basketlines/{productId}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to remove the product from your cart.";
                    _logger.LogWarning("Failed to remove product {ProductId} from cart for UserId: {UserId}. StatusCode: {StatusCode}", productId, userId, response.StatusCode);
                    return RedirectToAction("Index", new { userId });
                }

                TempData["SuccessMessage"] = "Product removed from your cart successfully.";
                return RedirectToAction("Index", new { userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product {ProductId} from cart for UserId: {UserId}", productId, userId);
                TempData["ErrorMessage"] = "An error occurred while removing the product from your cart.";
                return RedirectToAction("Index", new { userId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Guid userId)
        {
            var apiUrl = _configuration["BackendServices2:CartService"];
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.PostAsync($"{apiUrl}/api/shoppingbasket/{userId}/checkout", null);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to complete the checkout.";
                    _logger.LogWarning("Failed to checkout for UserId: {UserId}. StatusCode: {StatusCode}", userId, response.StatusCode);
                    return RedirectToAction("Index", new { userId });
                }

                TempData["SuccessMessage"] = "Checkout completed successfully.";
                return RedirectToAction("Index", new { userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during checkout for UserId: {UserId}", userId);
                TempData["ErrorMessage"] = "An error occurred during checkout.";
                return RedirectToAction("Index", new { userId });
            }
        }
    }
}