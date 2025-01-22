using AutoMapper;
using ShoppingBasket.Models;
using ShoppingBasket.Repositories;

namespace ShoppingBasket.Service
{
    public class ShoppingBasketService : IShoppingBasketService
    {
        private readonly IShoppingBasketRepository _shoppingBasketRepository;
        private readonly IBasketLinesRepository _basketLinesRepository;
        private readonly IHttpClientFactory _httpClientFactory; // To call ProductCatalog API
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ShoppingBasketService(
            IShoppingBasketRepository shoppingBasketRepository,
            IBasketLinesRepository basketLinesRepository,
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            IConfiguration configuration)
        {
            _shoppingBasketRepository = shoppingBasketRepository;
            _basketLinesRepository = basketLinesRepository;
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ShoppingBasketDto> GetShoppingBasketByUserId(Guid userId)
        {
            var basket = await _shoppingBasketRepository.GetShoppingBasketByUserId(userId);
            return _mapper.Map<ShoppingBasketDto>(basket);
        }

        public async Task AddProductToBasket(Guid userId, BasketLineForCreation basketLineForCreation)
        {
            var basket = await _shoppingBasketRepository.GetShoppingBasketByUserId(userId);
            if (basket == null)
            {
                basket = await _shoppingBasketRepository.CreateShoppingBasket(userId);
            }

            // Fetch Product details from ProductCatalog API
            var productCatalogApiUrl = _configuration.GetValue<string>("ProductCatalogApiUrl");
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{productCatalogApiUrl}/api/products/{basketLineForCreation.ProductId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch product details for ProductId: {basketLineForCreation.ProductId}");
            }

            var productDto = await response.Content.ReadFromJsonAsync<ProductDto>();
            if (productDto == null)
            {
                throw new Exception("Invalid product details returned from ProductCatalog API.");
            }

            // Add product details to BasketLine
            var existingBasketLine = basket.BasketLines.FirstOrDefault(bl => bl.ProductId == basketLineForCreation.ProductId);
            if (existingBasketLine != null)
            {
                existingBasketLine.Quantity += basketLineForCreation.Quantity;
            }
            else
            {
                var basketLine = new BasketLine
                {
                    ProductId = basketLineForCreation.ProductId,
                    Price = productDto.Price,
                    Quantity = basketLineForCreation.Quantity,
                    ShoppingBasketId = basket.ShoppingBasketId
                };
                basket.BasketLines.Add(basketLine);
            }

            await _basketLinesRepository.SaveChanges();
        }

        public async Task RemoveProductFromBasket(Guid userId, Guid productId)
        {
            var basket = await _shoppingBasketRepository.GetShoppingBasketByUserId(userId);
            if (basket != null)
            {
                var basketLine = basket.BasketLines.FirstOrDefault(bl => bl.ProductId == productId);
                if (basketLine != null)
                {
                    basket.BasketLines.Remove(basketLine);
                    await _basketLinesRepository.SaveChanges();
                }
            }
        }

        public async Task ClearBasket(Guid userId)
        {
            var basket = await _shoppingBasketRepository.GetShoppingBasketByUserId(userId);
            if (basket != null)
            {
                basket.BasketLines.Clear();
                await _basketLinesRepository.SaveChanges();
            }
        }
    }
}