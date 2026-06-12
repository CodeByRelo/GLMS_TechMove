using GLMS.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;

namespace GLMS.Web.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServiceRequestController> _logger;
        private readonly string _baseUrl;

        private string RequestsUrl => $"{_baseUrl}api/servicerequests";
        private string ContractsUrl => $"{_baseUrl}api/contracts";

        public ServiceRequestController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ServiceRequestController> logger)
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
            var requests = await _httpClient.GetFromJsonAsync<List<ServiceRequest>>(RequestsUrl);
            return View(requests);
        }

        // =========================
        // CREATE (GET)
        // =========================
        public async Task<IActionResult> Create()
        {
            await LoadContracts();
            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(ServiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                await LoadContracts();
                return View(request);
            }

            var response = await _httpClient.PostAsJsonAsync(RequestsUrl, request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to create service request via API");
                await LoadContracts();
                return View(request);
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var request = await _httpClient.GetFromJsonAsync<ServiceRequest>($"{RequestsUrl}/{id}");

            if (request == null)
                return NotFound();

            return View(request);
        }

        // =========================
        // EDIT (GET)
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var request = await _httpClient.GetFromJsonAsync<ServiceRequest>($"{RequestsUrl}/{id}");

            if (request == null)
                return NotFound();

            await LoadContracts();
            return View(request);
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ServiceRequest request)
        {
            if (id != request.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadContracts();
                return View(request);
            }

            var response = await _httpClient.PutAsJsonAsync($"{RequestsUrl}/{id}", request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to update service request via API");
                await LoadContracts();
                return View(request);
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE (GET)
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _httpClient.GetFromJsonAsync<ServiceRequest>($"{RequestsUrl}/{id}");

            if (request == null)
                return NotFound();

            return View(request);
        }

        // =========================
        // DELETE (POST)
        // =========================
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"{RequestsUrl}/{id}");

            if (!response.IsSuccessStatusCode)
                _logger.LogError($"Failed to delete service request {id}");

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // LOAD CONTRACTS (DROPDOWN)
        // =========================
        private async Task LoadContracts()
        {
            var contracts = await _httpClient.GetFromJsonAsync<List<Contract>>(ContractsUrl);

            var contractList = new List<object>();

            if (contracts != null)
            {
                foreach (var c in contracts)
                {
                    contractList.Add(new
                    {
                        c.Id,
                        Display = $"Contract #{c.Id} - {c.ServiceLevel}"
                    });
                }
            }

            ViewBag.Contracts = new SelectList(contractList, "Id", "Display");
        }
    }
}