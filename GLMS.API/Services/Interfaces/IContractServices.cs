using GLMS.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.API.Services.Interfaces
{
    public interface IContractService
    {
        Task<List<Contract>> GetAllContractsAsync();

        Task<Contract> GetContractByIdAsync(int id);

        Task CreateContractAsync(Contract contract);

        Task UpdateContractAsync(Contract contract);

        Task DeleteContractAsync(int id);

        Task<bool> IsContractActive(int contractId);

        Task<List<Client>> GetClientsAsync();
    }
}