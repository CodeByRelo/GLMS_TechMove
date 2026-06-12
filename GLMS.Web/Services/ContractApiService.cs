using GLMS.Core.Entities;
using GLMS.Web.Services.Interfaces;

namespace GLMS.Web.Services
{
    public class ContractApiService : IContractService
    {
        private readonly ApiClient _api;

        public ContractApiService(ApiClient api)
        {
            _api = api;
        }

        public async Task<List<Contract>> GetAllContractsAsync()
        {
            return await _api.GetAsync<List<Contract>>("api/contracts")
                   ?? new List<Contract>();
        }

        public async Task<Contract?> GetContractByIdAsync(int id)
        {
            return await _api.GetAsync<Contract>($"api/contracts/{id}");
        }

        public async Task CreateContractAsync(Contract contract)
        {
            await _api.PostAsync("api/contracts", contract);
        }

        public async Task UpdateContractAsync(Contract contract)
        {
            await _api.PutAsync($"api/contracts/{contract.Id}", contract);
        }

        public async Task DeleteContractAsync(int id)
        {
            await _api.DeleteAsync($"api/contracts/{id}");
        }

        // =====================================================
        // MISSING METHODS (FIX FOR YOUR ERROR)
        // =====================================================

        public async Task<bool> IsContractActive(int contractId)
        {
            return await _api.GetAsync<bool>($"api/contracts/{contractId}/is-active");
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            return await _api.GetAsync<List<Client>>("api/clients")
                   ?? new List<Client>();
        }
    }
}