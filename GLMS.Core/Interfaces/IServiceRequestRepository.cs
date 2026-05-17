using GLMS.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.Core.Interfaces
{
    public interface IServiceRequestRepository
    {
        Task<List<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task AddAsync(ServiceRequest request);
        Task UpdateAsync(ServiceRequest request);
        Task DeleteAsync(int id);
    }
}
