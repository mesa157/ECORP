using System.ComponentModel.DataAnnotations;

namespace UnifiedFrontend.Models.PaymentModel
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter ISO code.")]
        public string Currency { get; set; }

        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }

        public string Status { get; set; }
        public string TransactionId { get; set; }

        [Required]
        [Display(Name = "Payment Token")]
        public string PaymentMethodToken { get; set; }
    }

}
