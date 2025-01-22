namespace ShoppingBasket.Models
{
    public class ShoppingBasketDto
    {
        public Guid ShoppingBasketId { get; set; }
        public Guid UserId { get; set; }
        public List<BasketLineDto> BasketLines { get; set; }
    }
}
