using Stripe;
using System;

namespace PaymentService.Model
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod Method { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethodToken { get; set; }
    }
}