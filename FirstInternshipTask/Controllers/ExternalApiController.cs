using Microsoft.AspNetCore.Mvc;

namespace FirstInternshipTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalApiController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public ExternalApiController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        //Currency

        [HttpGet("currency")]
        public async Task<IActionResult> GetCurrencyRates()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://api.exchangerate-api.com/v4/latest/USD");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, new { error = "Failed to fetch data from external API" });

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
