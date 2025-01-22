using UnifiedFrontend.Models.CartModel;

namespace UnifiedFrontend.Services.CartService
{
    public class CartServiceApi
    {
        private readonly HttpClient _httpClient;

        public CartServiceApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ShoppingBasket> GetBasketAsync(Guid userId)
        {
            var basket = await _httpClient.GetFromJsonAsync<ShoppingBasket>($"api/shoppingbasket/{userId}");

            if (basket == null || basket.BasketLines == null)
            {
                return new ShoppingBasket { UserId = userId };
            }

            // Ensure product details are complete
            foreach (var line in basket.BasketLines)
            {
                if (string.IsNullOrEmpty(line.Name))
                {
                    var product = await _httpClient.GetFromJsonAsync<ProductDetails>($"api/products/{line.ProductId}");
                    if (product != null)
                    {
                        line.Name = product.Name;
                        line.Price = product.Price;
                    }
                }
            }

            return basket;
        }

        public async Task<bool> AddProductToBasketAsync(Guid userId, BasketLine basketLine)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/shoppingbasket/{userId}/basketlines", basketLine);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateQuantityAsync(Guid userId, Guid productId, int quantity)
        {
            var payload = new { Quantity = quantity };
            var response = await _httpClient.PutAsJsonAsync($"api/shoppingbasket/{userId}/basketlines/{productId}", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveProductFromBasketAsync(Guid userId, Guid productId)
        {
            var response = await _httpClient.DeleteAsync($"api/shoppingbasket/{userId}/basketlines/{productId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CheckoutAsync(Guid userId)
        {
            var response = await _httpClient.PostAsync($"api/shoppingbasket/{userId}/checkout", null);
            return response.IsSuccessStatusCode;
        }
    }
}
