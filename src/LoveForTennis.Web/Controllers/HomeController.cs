using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LoveForTennis.Web.Models;
using System.Text.Json;

namespace LoveForTennis.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, HttpClient httpClient, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7111";
            var response = await _httpClient.GetAsync($"{apiBaseUrl}/api/dummy");
            
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var dummies = JsonSerializer.Deserialize<List<DummyDto>>(jsonContent, options) ?? new List<DummyDto>();
                return View(dummies);
            }
            else
            {
                _logger.LogError("API call failed with status code: {StatusCode}", response.StatusCode);
                return View(new List<DummyDto>());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API");
            return View(new List<DummyDto>());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
