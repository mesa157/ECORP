namespace UnifiedFrontend.Models.CartModel
{
    public class BasketLine
    {
        public Guid BasketLineId { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
