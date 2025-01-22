using ShoppingBasket.Models;

namespace ShoppingBasket.Repositories
{
    public interface IBasketLinesRepository
    {
        Task<BasketLine> AddOrUpdateBasketLine(Guid basketId, BasketLine basketLine);

        Task SaveChanges();
    }
}
