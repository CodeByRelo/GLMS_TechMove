using GLMS.Core.Entities;
using GLMS.Core.Enums;
using GLMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GLMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly ILogger<ContractsController> _logger;

        public ContractsController(
            IContractService contractService,
            ILogger<ContractsController> logger)
        {
            _contractService = contractService;
            _logger = logger;
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var contracts = await _contractService.GetAllContractsAsync();
            return Ok(contracts);
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null)
                return NotFound();

            return Ok(contract);
        }

        // =========================
        // CREATE CONTRACT
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Contract contract, IFormFile? file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (file != null && file.Length > 0)
            {
                var path = await SavePdf(file);

                if (path == null)
                    return BadRequest("Only PDF files are allowed.");

                contract.SignedAgreementPath = path;
            }

            await _contractService.CreateContractAsync(contract);

            _logger.LogInformation($"Contract created: {contract.Id}");

            return Ok(contract);
        }

        // =========================
        // FULL UPDATE CONTRACT
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] Contract contract, IFormFile? file)
        {
            if (id != contract.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (file != null && file.Length > 0)
            {
                var path = await SavePdf(file);

                if (path == null)
                    return BadRequest("Only PDF files are allowed.");

                contract.SignedAgreementPath = path;
            }

            await _contractService.UpdateContractAsync(contract);

            _logger.LogInformation($"Contract updated: {id}");

            return Ok(contract);
        }

        // =========================
        // ⭐ PART 3 REQUIRED: PATCH STATUS
        // =========================
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ContractStatus status)
        {
            if (!Enum.IsDefined(typeof(ContractStatus), status))
                return BadRequest("Invalid status value");

            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null)
                return NotFound();

            // avoid unnecessary DB writes
            if (contract.Status == status)
            {
                return Ok(new
                {
                    message = "Status already set",
                    id,
                    status
                });
            }

            contract.Status = status;

            await _contractService.UpdateContractAsync(contract);

            _logger.LogInformation($"Contract {id} status changed to {status}");

            return Ok(new
            {
                message = "Status updated successfully",
                id,
                status
            });
        }

        // =========================
        // DELETE CONTRACT
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null)
                return NotFound();

            await _contractService.DeleteContractAsync(id);

            _logger.LogInformation($"Contract deleted: {id}");

            return Ok(new { message = "Contract deleted successfully" });
        }

        // =========================
        // SEARCH / FILTER
        // =========================
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            DateTime? startDate,
            DateTime? endDate,
            string? status)
        {
            var contracts = await _contractService.GetAllContractsAsync();

            var filtered = contracts.Where(c =>
                (!startDate.HasValue || c.StartDate.Date >= startDate.Value.Date) &&
                (!endDate.HasValue || c.EndDate.Date <= endDate.Value.Date) &&
                (string.IsNullOrWhiteSpace(status) ||
                 (Enum.TryParse<ContractStatus>(status.Trim(), true, out var parsedStatus)
                  && c.Status == parsedStatus))
            ).ToList();

            return Ok(filtered);
        }

        // =========================
        // DOWNLOAD PDF
        // =========================
        [HttpGet("download")]
        public IActionResult Download(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return NotFound();

            // security fix: prevent path traversal
            if (!filePath.StartsWith("/contracts/"))
                return BadRequest("Invalid file path");

            var fullPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                filePath.TrimStart('/'));

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(fullPath);

            return File(bytes, "application/pdf", Path.GetFileName(fullPath));
        }

        // =========================
        // SAVE PDF HELPER
        // =========================
        private async Task<string?> SavePdf(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName);

            if (!string.Equals(ext, ".pdf", StringComparison.OrdinalIgnoreCase))
                return null;

            var fileName = $"{Guid.NewGuid()}.pdf";
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/contracts");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/contracts/" + fileName;
        }
    }
}