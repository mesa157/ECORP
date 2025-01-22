using System.ComponentModel.DataAnnotations;

namespace UnifiedFrontend.Models.ProductCatalogModel
{
    public class ProductViewModel
    {
        public Guid ProductId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public int Price { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
