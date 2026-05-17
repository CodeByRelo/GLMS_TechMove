using GLMS.Core.Entities;
using GLMS.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GLMS.Web.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly IContractService _contractService;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<ServiceRequestController> _logger;

        public ServiceRequestController(
            IServiceRequestService serviceRequestService,
            IContractService contractService,
            ICurrencyService currencyService,
            ILogger<ServiceRequestController> logger)
        {
            _serviceRequestService = serviceRequestService;
            _contractService = contractService;
            _currencyService = currencyService;
            _logger = logger;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var requests = await _serviceRequestService.GetAllAsync();

            // Ensure contracts and clients are loaded for display
            foreach (var req in requests)
            {
                if (req.Contract == null)
                {
                    req.Contract = await _contractService.GetContractByIdAsync(req.ContractId);
                }
            }

            return View(requests);
        }

        // =========================
        // CREATE GET
        // =========================
        public async Task<IActionResult> Create()
        {
            await LoadContracts();
            return View();
        }

        // =========================
        // CREATE POST
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(ServiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                await LoadContracts();
                return View(request);
            }

            // Business Rule: Contract must be active
            bool active = await _contractService.IsContractActive(request.ContractId);
            if (!active)
            {
                ModelState.AddModelError("", "Cannot create request for inactive contract.");
                await LoadContracts();
                return View(request);
            }

            // Currency Conversion
            request.CostZAR = await _currencyService.ConvertUsdToZar(request.CostUSD);

            // Default values
            request.Status = "Pending";
            request.CreatedDate = System.DateTime.Now;

            await _serviceRequestService.CreateServiceRequestAsync(request);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var request = await _serviceRequestService.GetByIdAsync(id);
            if (request == null) return NotFound();
            return View(request);
        }

        // =========================
        // EDIT GET
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var request = await _serviceRequestService.GetByIdAsync(id);
            if (request == null) return NotFound();

            await LoadContracts();
            return View(request);
        }

        // =========================
        // EDIT POST
        // =========================
        [HttpPost]
        public async Task<IActionResult> Edit(ServiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                await LoadContracts();
                return View(request);
            }

            request.CostZAR = await _currencyService.ConvertUsdToZar(request.CostUSD);
            await _serviceRequestService.UpdateAsync(request);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE GET
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _serviceRequestService.GetByIdAsync(id);
            if (request == null) return NotFound();
            return View(request);
        }

        // =========================
        // DELETE POST
        // =========================
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _serviceRequestService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // LOAD CONTRACTS
        // =========================
        private async Task LoadContracts()
        {
            var contracts = await _contractService.GetAllContractsAsync();

            var contractList = contracts.Select(c => new
            {
                c.Id,
                Display = $"Contract #{c.Id} - {c.Client?.Name} ({c.ServiceLevel}) [{c.StartDate:yyyy-MM-dd} to {c.EndDate:yyyy-MM-dd}]"
            });

            ViewBag.Contracts = new SelectList(contractList, "Id", "Display");
        }
    }
}
