using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MyCountryApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CountryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetInfo([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "国名を入力してください" });

            var client = _httpClientFactory.CreateClient();
            var url = $"https://restcountries.com/v3.1/name/{Uri.EscapeDataString(name)}";
            var res = await client.GetAsync(url);

            if (!res.IsSuccessStatusCode)
                return NotFound(new { message = "国が見つかりませんでした" });

            var json = await res.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var country = doc.RootElement[0];

            var result = new
            {
                name = country.GetProperty("name").GetProperty("common").GetString(),
                capital = country.TryGetProperty("capital", out var cap) ? cap[0].GetString() : "(不明)",
                population = country.GetProperty("population").GetInt32(),
                flag = country.GetProperty("flags").GetProperty("png").GetString(),
                currency = country.GetProperty("currencies").EnumerateObject().First().Value.GetProperty("name").GetString(),
                language = country.GetProperty("languages").EnumerateObject().First().Value.GetString()
            };

            return Ok(result);
        }
    }
}
