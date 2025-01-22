using ShoppingBasket.Models;

namespace ShoppingBasket.Service
{
    public interface IShoppingBasketService
    {
        Task<ShoppingBasketDto> GetShoppingBasketByUserId(Guid userId);
        Task AddProductToBasket(Guid userId, BasketLineForCreation basketLineForCreation);
        Task RemoveProductFromBasket(Guid userId, Guid productId);
        Task ClearBasket(Guid userId);
    }

}
