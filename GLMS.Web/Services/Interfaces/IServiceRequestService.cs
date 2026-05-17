using GLMS.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.Web.Services.Interfaces
{
    public interface IServiceRequestService
    {
        Task<List<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task CreateServiceRequestAsync(ServiceRequest request);
        Task UpdateAsync(ServiceRequest request);
        Task DeleteAsync(int id);
    }
}
