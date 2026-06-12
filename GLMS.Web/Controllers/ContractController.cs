using GLMS.Core.Entities;
using GLMS.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace GLMS.Web.Controllers
{
    public class ContractController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ContractController> _logger;
        private readonly string _apiBaseUrl;

        private string ApiUrl => $"{_apiBaseUrl}api/contracts";

        public ContractController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ContractController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;

            _apiBaseUrl = configuration["ApiSettings:BaseUrl"]
                ?? throw new Exception("API BaseUrl missing in configuration");
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var contracts = await _httpClient.GetFromJsonAsync<List<Contract>>(ApiUrl);
            return View(contracts ?? new List<Contract>());
        }

        // =========================
        // CREATE
        // =========================
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile file)
        {
            if (!ModelState.IsValid)
                return View(contract);

            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid() + "_" + file.FileName;
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/contracts");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var path = Path.Combine(folder, fileName);

                using var stream = System.IO.File.Create(path);
                await file.CopyToAsync(stream);

                contract.SignedAgreementPath = "/contracts/" + fileName;
            }

            var response = await _httpClient.PostAsJsonAsync(ApiUrl, contract);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to create contract via API");
                return View(contract);
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var contract = await _httpClient.GetFromJsonAsync<Contract>($"{ApiUrl}/{id}");
            return contract == null ? NotFound() : View(contract);
        }

        // =========================
        // EDIT
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _httpClient.GetFromJsonAsync<Contract>($"{ApiUrl}/{id}");
            return contract == null ? NotFound() : View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile file)
        {
            if (id != contract.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(contract);

            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid() + "_" + file.FileName;
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/contracts");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var path = Path.Combine(folder, fileName);

                using var stream = System.IO.File.Create(path);
                await file.CopyToAsync(stream);

                contract.SignedAgreementPath = "/contracts/" + fileName;
            }

            var response = await _httpClient.PutAsJsonAsync($"{ApiUrl}/{id}", contract);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to update contract via API");
                return View(contract);
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // PATCH STATUS
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (!Enum.TryParse<ContractStatus>(status, true, out var parsedStatus))
            {
                TempData["Error"] = "Invalid status.";
                return RedirectToAction(nameof(Index));
            }

            var request = new HttpRequestMessage(
                HttpMethod.Patch,
                $"{ApiUrl}/{id}/status"
            )
            {
                Content = JsonContent.Create(parsedStatus)
            };

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Failed to update status.";
            else
                TempData["Success"] = "Status updated.";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _httpClient.GetFromJsonAsync<Contract>($"{ApiUrl}/{id}");
            return contract == null ? NotFound() : View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiUrl}/{id}");

            if (!response.IsSuccessStatusCode)
                _logger.LogError($"Failed to delete contract {id}");

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // 🔥 PDF DOWNLOAD (RESTORED CLEANLY)
        // =========================
        public IActionResult Download(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return NotFound();

            if (!filePath.StartsWith("/contracts/"))
                return BadRequest("Invalid file path");

            var fullPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                filePath.TrimStart('/')
            );

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(fullPath);

            return File(bytes, "application/pdf", Path.GetFileName(fullPath));
        }
    }
}