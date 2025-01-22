using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentService.Data;
using PaymentService.Model;
using PaymentService.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentService.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        private readonly ApplicationDbContext _context;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger, ApplicationDbContext context)
        {
            _paymentService = paymentService;
            _logger = logger;
            _context = context; // Injecting the ApplicationDbContext
        }

        // POST: api/payments
        [HttpPost]
        public async Task<IActionResult> PostPayment([FromBody] Payment payment)
        {
            if (payment == null)
            {
                return BadRequest("Payment details are required.");
            }

            var result = await _paymentService.ProcessPayment(payment);

            if (result.Status == "Succeeded")
            {
                return Ok(result);
            }

            return StatusCode(500, $"Payment failed: {result.Status}");
        }

        // PUT: api/payments/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("Status is required.");
            }

            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    return NotFound("Payment not found.");
                }

                payment.Status = status;
                _context.Payments.Update(payment);
                await _context.SaveChangesAsync();

                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating payment status: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the payment status.");
            }
        }

        // GET: api/payments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    return NotFound("Payment not found.");
                }

                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving payment: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the payment.");
            }
        }

        // GET: api/payments (with search and pagination)
        [HttpGet]
        public async Task<IActionResult> GetPayments([FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Payments.AsQueryable();

                // Search filter
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => p.TransactionId.Contains(search) || p.Status.Contains(search));
                }

                // Pagination
                var totalItems = await query.CountAsync();
                var payments = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new
                {
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    Payments = payments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving payments: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving payments.");
            }
        }
    }
}
