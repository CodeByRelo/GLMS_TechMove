using GLMS.Core.Enums;
using GLMS.Web.Filters;
using GLMS.Web.Models;
using GLMS.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Web.Controllers
{
    [SessionAuth]
    public class HomeController : Controller
    {
        private readonly IContractService _contractService;
        private readonly IServiceRequestService _serviceRequestService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IContractService contractService,
            IServiceRequestService serviceRequestService,
            ILogger<HomeController> logger)
        {
            _contractService = contractService;
            _serviceRequestService = serviceRequestService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var contracts = await _contractService.GetAllContractsAsync();
                var requests = await _serviceRequestService.GetAllAsync();

                var dashboard = new HomeDashboardViewModel
                {
                    TotalContracts = contracts.Count,
                    ActiveContracts = contracts.Count(c => c.Status == ContractStatus.Active),
                    ExpiredContracts = contracts.Count(c => c.Status == ContractStatus.Expired),

                    PendingRequests = requests.Count(r =>
                        r.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)),

                    CompletedRequests = requests.Count(r =>
                        r.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                };

                return View(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard failed to load");

                return View(new HomeDashboardViewModel
                {
                    TotalContracts = 0,
                    ActiveContracts = 0,
                    ExpiredContracts = 0,
                    PendingRequests = 0,
                    CompletedRequests = 0
                });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}