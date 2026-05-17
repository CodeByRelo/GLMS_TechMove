using GLMS.Core.Entities;
using GLMS.Core.Enums;
using GLMS.Core.Interfaces;
using GLMS.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.Web.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _requestRepo;
        private readonly IContractRepository _contractRepo;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestService(
            IServiceRequestRepository requestRepo,
            IContractRepository contractRepo,
            ICurrencyService currencyService)
        {
            _requestRepo = requestRepo;
            _contractRepo = contractRepo;
            _currencyService = currencyService;
        }

        public async Task<List<ServiceRequest>> GetAllAsync()
        {
            return await _requestRepo.GetAllAsync();
        }

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            return await _requestRepo.GetByIdAsync(id);
        }

        public async Task CreateServiceRequestAsync(ServiceRequest request)
        {
            var contract = await _contractRepo.GetByIdAsync(request.ContractId);

            if (contract == null)
                throw new Exception("Contract not found");

            if (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
                throw new Exception("Cannot create request for inactive contract");

            // 💰 Currency conversion
            request.CostZAR = await _currencyService.ConvertUsdToZar(request.CostUSD);

            await _requestRepo.AddAsync(request);
        }

        public async Task UpdateAsync(ServiceRequest request)
        {
            var contract = await _contractRepo.GetByIdAsync(request.ContractId);

            if (contract == null)
                throw new Exception("Contract not found");

            if (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
                throw new Exception("Cannot update request for inactive contract");

            request.CostZAR = await _currencyService.ConvertUsdToZar(request.CostUSD);

            await _requestRepo.UpdateAsync(request);
        }

        public async Task DeleteAsync(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);

            if (request == null)
                throw new Exception("Service request not found");

            await _requestRepo.DeleteAsync(id);
        }
    }
}
