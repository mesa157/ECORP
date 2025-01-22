using UnifiedFrontend.Models.PaymentModel;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace UnifiedFrontend.Services.PaymentServices
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7091/api/payments"); // Replace with your API URL
        }

        public async Task<PaginatedResult<Payment>> GetPayments(string search = "", int page = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetFromJsonAsync<PaginatedResult<Payment>>($"?search={search}&page={page}&pageSize={pageSize}");
            return response;
        }

        public async Task<Payment> GetPaymentById(int id)
        {
            return await _httpClient.GetFromJsonAsync<Payment>($"/{id}");
        }

        public async Task<Payment> CreatePayment(Payment payment)
        {
            var response = await _httpClient.PostAsJsonAsync("", payment);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Payment>();
        }

        public async Task<Payment> UpdatePaymentStatus(int id, string status)
        {
            var response = await _httpClient.PutAsJsonAsync($"/{id}/status", status);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Payment>();
        }
    }
}
