using Microsoft.EntityFrameworkCore;
using ShoppingBasket.DbContexts;
using ShoppingBasket.Models;

namespace ShoppingBasket.Repositories
{
    public class BasketLinesRepository : IBasketLinesRepository
    {
        private readonly ShoppingBasketDbContext _context;

        public BasketLinesRepository(ShoppingBasketDbContext context)
        {
            _context = context;
        }

        public async Task<BasketLine> AddOrUpdateBasketLine(Guid basketId, BasketLine basketLine)
        {
            var existingLine = await _context.BasketLines
                .Where(b => b.ShoppingBasketId == basketId && b.ProductId == basketLine.ProductId)
                .FirstOrDefaultAsync();

            if (existingLine == null)
            {
                basketLine.ShoppingBasketId = basketId;
                _context.BasketLines.Add(basketLine);
                return basketLine;
            }

            existingLine.Quantity += basketLine.Quantity;
            return existingLine;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}


