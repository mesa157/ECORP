namespace UnifiedFrontend.Models.PaymentModel
{
    public class PaymentViewModel
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string PaymentMethodToken { get; set; }
    }

}
