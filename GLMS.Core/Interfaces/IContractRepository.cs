using GLMS.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.Core.Interfaces
{
    public interface IContractRepository : IRepository<Contract>
    {
        Task<List<Contract>> GetActiveContractsAsync();

        Task<Contract?> GetByIdAsync(int id);
        Task AddAsync(Contract contract);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(int id);
    }
}
