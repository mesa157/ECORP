using System.ComponentModel.DataAnnotations;

namespace UnifiedFrontend.Models.ProductCatalogModel
{
    public class CategoryViewModel
    {
        public Guid CategoryId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string Name { get; set; }

        public List<ProductViewModel> Products { get; set; }
    }

}
