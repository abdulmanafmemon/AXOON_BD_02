using FirstInternshipTask.Models;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Data;
using System.Text.Json;

namespace FirstInternshipTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ProductContext _context;

        public CurrencyController(IHttpClientFactory httpClientFactory, ProductContext context)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        [HttpGet("rates")]
        public async Task<IActionResult> GetCurrencyRates()
        {
            // Check for existing local data
            var localRate = _context.CurrencyRates.FirstOrDefault();

            // If data is missing or older than 1 hour, refresh it
            if (localRate == null || localRate.LastUpdated < DateTime.UtcNow.AddHours(-1))
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("https://api.exchangerate-api.com/v4/latest/USD");

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, new { error = "External API failed." });

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonDocument.Parse(json);
                var rates = data.RootElement.GetProperty("rates");

                var newRate = new CurrencyRate
                {
                    PKR = rates.GetProperty("PKR").GetDecimal(),
                    INR = rates.GetProperty("INR").GetDecimal(),
                    EUR = rates.GetProperty("EUR").GetDecimal(),
                    LastUpdated = DateTime.UtcNow
                };

                if (localRate != null)
                {
                    // Update existing row
                    localRate.PKR = newRate.PKR;
                    localRate.INR = newRate.INR;
                    localRate.EUR = newRate.EUR;
                    localRate.LastUpdated = newRate.LastUpdated;
                }
                else
                {
                    // Add new row
                    _context.CurrencyRates.Add(newRate);
                }

                await _context.SaveChangesAsync();
                return Ok(newRate);
            }

            // Return from local DB
            return Ok(localRate);
        }
    }

}
