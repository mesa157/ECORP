using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace ShoppingBasket.Models
{
    public class ShoppingBaskett
    {
        [Key]
        public Guid ShoppingBasketId { get; set; }
        public Guid UserId { get; set; }
        public List<BasketLine> BasketLines { get; set; }
    }
}
