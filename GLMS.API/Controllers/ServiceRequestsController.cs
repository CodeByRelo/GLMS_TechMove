using GLMS.API.Services.Interfaces;
using GLMS.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly IContractService _contractService;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsController(
            IServiceRequestService serviceRequestService,
            IContractService contractService,
            ICurrencyService currencyService)
        {
            _serviceRequestService = serviceRequestService;
            _contractService = contractService;
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _serviceRequestService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _serviceRequestService.GetByIdAsync(id);

            if (request == null)
                return NotFound();

            return Ok(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isActive = await _contractService.IsContractActive(request.ContractId);

            if (!isActive)
                return BadRequest("Contract is not active");

            request.Status = "Pending";
            request.CreatedDate = DateTime.Now;

            request.CostZAR = await _currencyService.ConvertUsdToZar(request.CostUSD);

            await _serviceRequestService.CreateServiceRequestAsync(request);

            return Ok(request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            request.CostZAR = await _currencyService.ConvertUsdToZar(request.CostUSD);

            await _serviceRequestService.UpdateAsync(request);

            return Ok(request);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _serviceRequestService.GetByIdAsync(id);

            if (existing == null)
                return NotFound();

            await _serviceRequestService.DeleteAsync(id);

            return Ok(new { message = "Deleted successfully" });
        }
    }
}