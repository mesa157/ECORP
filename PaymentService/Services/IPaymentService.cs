using System.Threading.Tasks;
using PaymentService.Model;

namespace PaymentService.Services
{
    public interface IPaymentService
    {
        Task<Payment> ProcessPayment(Payment payment);
        Task<Payment> UpdatePaymentStatus(int id, string status);
        Task<Payment> GetPaymentById(int id); // Add this method

    }
}
