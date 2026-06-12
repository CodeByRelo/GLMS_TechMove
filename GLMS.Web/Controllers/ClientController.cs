using GLMS.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace GLMS.Web.Controllers
{
    public class ClientController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClientController> _logger;
        private readonly string _baseUrl;

        private string ClientsUrl => $"{_baseUrl}api/clients";

        public ClientController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ClientController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _baseUrl = configuration["ApiSettings:BaseUrl"]!;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching clients from API...");

            var clients = await _httpClient.GetFromJsonAsync<List<Client>>(ClientsUrl);
            return View(clients);
        }

        // =========================
        // CREATE (GET)
        // =========================
        public IActionResult Create()
        {
            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (!ModelState.IsValid)
                return View(client);

            var response = await _httpClient.PostAsJsonAsync(ClientsUrl, client);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create client via API.");
                return View(client);
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var client = await _httpClient.GetFromJsonAsync<Client>($"{ClientsUrl}/{id}");

            if (client == null)
                return NotFound();

            return View(client);
        }

        // =========================
        // EDIT
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _httpClient.GetFromJsonAsync<Client>($"{ClientsUrl}/{id}");

            if (client == null)
                return NotFound();

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(client);

            var response = await _httpClient.PutAsJsonAsync($"{ClientsUrl}/{id}", client);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to update client via API.");
                return View(client);
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _httpClient.GetFromJsonAsync<Client>($"{ClientsUrl}/{id}");

            if (client == null)
                return NotFound();

            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"{ClientsUrl}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to delete client via API.");
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}