namespace ShoppingBasket.Models
{
    public class BasketLineForCreation
    {
        public Guid ProductId { get; set; }
        public Decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}