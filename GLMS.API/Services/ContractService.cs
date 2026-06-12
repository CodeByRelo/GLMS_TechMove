using GLMS.Core.Entities;
using GLMS.Core.Enums;
using GLMS.Core.Interfaces;
using GLMS.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.API.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IClientRepository _clientRepository;

        public ContractService(
            IContractRepository contractRepository,
            IClientRepository clientRepository
        )
        {
            _contractRepository = contractRepository;
            _clientRepository = clientRepository;
        }

        public async Task<List<Contract>> GetAllContractsAsync()
        {
            return await _contractRepository.GetAllAsync();
        }

        public async Task<Contract?> GetContractByIdAsync(int id)
        {
            return await _contractRepository.GetByIdAsync(id);
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            return await _clientRepository.GetAllAsync();
        }

        public async Task CreateContractAsync(Contract contract)
        {
            ApplyBusinessRules(contract);

            if (contract.StartDate > contract.EndDate)
            {
                throw new Exception("Start date cannot be after end date.");
            }

            await _contractRepository.AddAsync(contract);
        }

        public async Task UpdateContractAsync(Contract contract)
        {
            ApplyBusinessRules(contract);

            if (contract.StartDate > contract.EndDate)
            {
                throw new Exception("Start date cannot be after end date.");
            }

            await _contractRepository.UpdateAsync(contract);
        }

        public async Task DeleteContractAsync(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);

            if (contract == null)
            {
                throw new Exception("Contract not found.");
            }

            // Optional: prevent deleting active contracts
            if (contract.Status == ContractStatus.Active)
            {
                throw new Exception("Active contracts cannot be deleted.");
            }

            await _contractRepository.DeleteAsync(id);
        }

        public async Task<bool> IsContractActive(int contractId)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);
            return contract != null && contract.Status == ContractStatus.Active;
        }

        // 🔒 Centralized business rules for status logic
        private void ApplyBusinessRules(Contract contract)
        {
            // ONLY apply rules if status has NOT been manually set via PATCH

            // If user manually set a status like OnHold/Expired via PATCH,
            // we do NOT override it.

            if (contract.Status == ContractStatus.OnHold ||
                contract.Status == ContractStatus.Expired ||
                contract.Status == ContractStatus.Active)
            {
                return; // 🔥 PATCH wins
            }

            // Otherwise allow system defaults for new contracts
            if (contract.EndDate < DateTime.Now)
            {
                contract.Status = ContractStatus.Expired;
            }
            else if (contract.StartDate > DateTime.Now)
            {
                contract.Status = ContractStatus.Draft;
            }
            else
            {
                contract.Status = ContractStatus.Active;
            }
        }
    }
}
