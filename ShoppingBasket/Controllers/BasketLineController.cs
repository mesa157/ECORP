using Microsoft.AspNetCore.Mvc;
using ShoppingBasket.Models;
using ShoppingBasket.Service;

namespace ShoppingBasket.Controllers
{

    [Route("api/shoppingbasket/{userId}/basketlines")]
    [ApiController]
    public class BasketLinesController : ControllerBase
    {
        private readonly IShoppingBasketService _shoppingBasketService;

        public BasketLinesController(IShoppingBasketService shoppingBasketService)
        {
            _shoppingBasketService = shoppingBasketService;
        }

        [HttpPost]
        public async Task<ActionResult> AddProductToBasket(Guid userId, [FromBody] BasketLineForCreation basketLineForCreation)
        {
            if (basketLineForCreation == null || basketLineForCreation.Quantity <= 0)
            {
                return BadRequest("Invalid product details or quantity.");
            }

            try
            {
                await _shoppingBasketService.AddProductToBasket(userId, basketLineForCreation);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{productId}")]
        public async Task<ActionResult> RemoveProductFromBasket(Guid userId, Guid productId)
        {
            try
            {
                await _shoppingBasketService.RemoveProductFromBasket(userId, productId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    [Route("api/shoppingbasket")]
    [ApiController]
    public class ShoppingBasketController : ControllerBase
    {
        private readonly IShoppingBasketService _shoppingBasketService;

        public ShoppingBasketController(IShoppingBasketService shoppingBasketService)
        {
            _shoppingBasketService = shoppingBasketService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<ShoppingBasketDto>> GetShoppingBasket(Guid userId)
        {
            try
            {
                var basket = await _shoppingBasketService.GetShoppingBasketByUserId(userId);

                if (basket == null || basket.BasketLines == null || !basket.BasketLines.Any())
                {
                    return Ok(new ShoppingBasketDto
                    {
                        UserId = userId,
                        BasketLines = new List<BasketLineDto>()
                    });
                }

                return Ok(basket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the basket: {ex.Message}");
            }
        }
    }
}
