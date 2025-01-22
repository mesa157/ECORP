using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UnifiedFrontend.Models.PaymentModel;
using UnifiedFrontend.Services.PaymentServices;

namespace PaymentFrontend.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IActionResult> Index(string search = "", int page = 1)
        {
            var result = await _paymentService.GetPayments(search, page, 10);

            // Ensure result is not null
            if (result == null)
            {
                result = new PaginatedResult<Payment>
                {
                    Items = new List<Payment>(),
                    Page = page,
                    PageSize = 10,
                    TotalItems = 0
                };
            }

            ViewData["SearchQuery"] = search;
            return View(result);
        }


        // View Payment Details
        public async Task<IActionResult> Details(int id)
        {
            var payment = await _paymentService.GetPaymentById(id);
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }

        // Create Payment
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Payment payment)
        {
            if (ModelState.IsValid)
            {
                var result = await _paymentService.CreatePayment(payment);
                return RedirectToAction("Details", new { id = result.Id });
            }
            return View(payment);
        }

        // Update Payment Status (Admin)
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _paymentService.UpdatePaymentStatus(id, status);
            return RedirectToAction("Details", new { id });
        }
    }
}
