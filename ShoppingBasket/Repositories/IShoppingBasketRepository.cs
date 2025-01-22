using ShoppingBasket.Models;
namespace ShoppingBasket.Repositories
{
    public interface IShoppingBasketRepository
    {
        Task<ShoppingBaskett> GetShoppingBasketByUserId(Guid userId);
        Task<ShoppingBaskett> CreateShoppingBasket(Guid userId);
    }
}
