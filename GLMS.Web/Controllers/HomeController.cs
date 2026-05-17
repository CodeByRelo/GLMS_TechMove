using GLMS.Core.Enums;
using GLMS.Web.Models;
using GLMS.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GLMS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContractService _contractService;
        private readonly IServiceRequestService _serviceRequestService;

        public HomeController(IContractService contractService, IServiceRequestService serviceRequestService)
        {
            _contractService = contractService;
            _serviceRequestService = serviceRequestService;
        }

        public async Task<IActionResult> Index()
        {
            var contracts = await _contractService.GetAllContractsAsync();
            var requests = await _serviceRequestService.GetAllAsync();

            var dashboard = new HomeDashboardViewModel
            {
                TotalContracts = contracts.Count,
                ActiveContracts = contracts.Count(c => c.Status == ContractStatus.Active),
                ExpiredContracts = contracts.Count(c => c.Status == ContractStatus.Expired),

                // decided to treat Status as string instead of enum since ServiceRequest doesn't have a defined enum for status, and it allows for more flexibility in case of future additions
                PendingRequests = requests.Count(r => r.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)),
                CompletedRequests = requests.Count(r => r.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            };

            return View(dashboard);
        }

        // 🛡️ Privacy Policy Page
        public IActionResult Privacy()
        {
            return View();
        }
    }


}
