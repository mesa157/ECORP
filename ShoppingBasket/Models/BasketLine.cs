namespace ShoppingBasket.Models
{

    public class BasketLine
    {
        public Guid BasketLineId { get; set; }
        public Guid ShoppingBasketId { get; set; }
        public Guid ProductId { get; set; }
        public Decimal Price { get; set; }
        public int Quantity { get; set; }

    }
}
