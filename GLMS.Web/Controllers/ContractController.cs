using GLMS.Core.Entities;
using GLMS.Core.Enums;
using GLMS.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GLMS.Web.Controllers
{
    public class ContractController : Controller
    {
        private readonly IContractService _contractService;
        private readonly ILogger<ContractController> _logger;

        public ContractController(
            IContractService contractService,
            ILogger<ContractController> logger)
        {
            _contractService = contractService;
            _logger = logger;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var contracts = await _contractService.GetAllContractsAsync();
            return View(contracts);
        }

        // =========================
        // CREATE (GET)
        // =========================
        public async Task<IActionResult> Create()
        {
            await LoadClients();
            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                await LoadClients();
                return View(contract);
            }

            if (file != null && file.Length > 0)
            {
                var result = await SavePdf(file);
                if (result == null)
                {
                    ModelState.AddModelError("", "Only PDF files are allowed.");
                    await LoadClients();
                    return View(contract);
                }
                contract.SignedAgreementPath = result;
            }

            await _contractService.CreateContractAsync(contract);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        // =========================
        // EDIT (GET)
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null) return NotFound();

            await LoadClients();
            return View(contract);
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile file)
        {
            if (id != contract.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadClients();
                return View(contract);
            }

            if (file != null && file.Length > 0)
            {
                var result = await SavePdf(file);
                if (result == null)
                {
                    ModelState.AddModelError("", "Only PDF files are allowed.");
                    await LoadClients();
                    return View(contract);
                }
                contract.SignedAgreementPath = result;
            }

            await _contractService.UpdateContractAsync(contract);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE (GET)
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        // =========================
        // DELETE (POST)
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _contractService.DeleteContractAsync(id);
                _logger.LogInformation($"Contract deleted: {id}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"Error deleting contract {id}");

                // Add a user-friendly error message
                ModelState.AddModelError("", ex.Message);

                // Reload the contract so we can show the Delete view again
                var contract = await _contractService.GetContractByIdAsync(id);
                if (contract == null)
                {
                    return NotFound();
                }

                return View(contract);
            }
        }


        // =========================
        // DOWNLOAD PDF
        // =========================
        public IActionResult Download(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return NotFound();

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
            if (!System.IO.File.Exists(fullPath)) return NotFound();

            var bytes = System.IO.File.ReadAllBytes(fullPath);
            return File(bytes, "application/pdf", Path.GetFileName(fullPath));
        }

        // =========================
        // LOAD CLIENTS
        // =========================
        private async Task LoadClients()
        {
            var clients = await _contractService.GetClientsAsync();
            ViewBag.Clients = new SelectList(clients, "Id", "Name");
        }

        // =========================
        // SAVE PDF HELPER
        // =========================
        private async Task<string?> SavePdf(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            if (extension.ToLower() != ".pdf") return null;

            var fileName = Guid.NewGuid() + ".pdf";
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/contracts");

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var filePath = Path.Combine(uploadFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/contracts/" + fileName;
        }

        public async Task<IActionResult> Search(DateTime? startDate, DateTime? endDate, string status)
        {
            var contracts = await _contractService.GetAllContractsAsync();

            var filtered = contracts.Where(c =>
                (!startDate.HasValue || c.StartDate >= startDate) &&
                (!endDate.HasValue || c.EndDate <= endDate) &&
                (string.IsNullOrEmpty(status) ||
                 (Enum.TryParse<ContractStatus>(status, true, out var parsedStatus) && c.Status == parsedStatus))
            ).ToList();

            return View("Index", filtered);
        }



    }
}
