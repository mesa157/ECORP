namespace UnifiedFrontend.Models.CartModel
{
    public class ShoppingBasket
    {
        public Guid ShoppingBasketId { get; set; }
        public Guid UserId { get; set; }
        public List<BasketLine> BasketLines { get; set; } = new();
        public decimal TotalPrice => BasketLines.Sum(line => line.Quantity * line.Price);
    }
}
