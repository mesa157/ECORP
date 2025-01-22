namespace ShoppingBasket.Models
{
    public class BasketLineDto
    {
        public Guid BasketLineId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

   
}


