using Microsoft.EntityFrameworkCore;
using ShoppingBasket.DbContexts;
using ShoppingBasket.Models;

namespace ShoppingBasket.Repositories
{
    public class ShoppingBasketRepository : IShoppingBasketRepository
    {
        private readonly ShoppingBasketDbContext _context;

        public ShoppingBasketRepository(ShoppingBasketDbContext context)
        {
            _context = context;
        }

        public async Task<ShoppingBaskett> GetShoppingBasketByUserId(Guid userId)
        {
            return await _context.ShoppingBaskets.Include(b => b.BasketLines)
                .FirstOrDefaultAsync(b => b.UserId == userId);
        }

        public async Task<ShoppingBaskett> CreateShoppingBasket(Guid userId)
        {
            var basket = new ShoppingBaskett
            {
                ShoppingBasketId = Guid.NewGuid(),
                UserId = userId,
                BasketLines = new List<BasketLine>()
            };

            _context.ShoppingBaskets.Add(basket);
            await _context.SaveChangesAsync();

            return basket;
        }
    }
}
