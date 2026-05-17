using GLMS.Core.Entities;
using GLMS.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GLMS.Web.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientController> _logger;

        public ClientController(
            IClientService clientService,
            ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching all clients...");

            var clients = await _clientService.GetAllClientsAsync();

            return View(clients);
        }

        // =========================
        // CREATE (GET)
        // =========================
        public IActionResult Create()
        {
            _logger.LogInformation("Loading Create Client page");

            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            _logger.LogInformation("POST Create Client triggered");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Client ModelState invalid");
                return View(client);
            }

            await _clientService.CreateClientAsync(client);

            _logger.LogInformation("Client created successfully");

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);

            if (client == null)
            {
                _logger.LogWarning($"Client not found: {id}");
                return NotFound();
            }

            return View(client);
        }

        // =========================
        // EDIT (GET)
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(client);
            }

            await _clientService.UpdateClientAsync(client);

            _logger.LogInformation($"Client updated: {client.Id}");

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE (GET)
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // =========================
        // DELETE (POST)
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _clientService.DeleteClientAsync(id);

            _logger.LogInformation($"Client deleted: {id}");

            return RedirectToAction(nameof(Index));
        }
    }
}