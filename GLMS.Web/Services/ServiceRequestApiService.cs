using GLMS.Core.Entities;
using GLMS.Web.Services.Interfaces;

namespace GLMS.Web.Services
{
    public class ServiceRequestApiService : IServiceRequestService
    {
        private readonly ApiClient _api;

        public ServiceRequestApiService(ApiClient api)
        {
            _api = api;
        }

        public async Task<List<ServiceRequest>> GetAllAsync()
        {
            return await _api.GetAsync<List<ServiceRequest>>("api/servicerequests") ?? new List<ServiceRequest>();
        }

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            return await _api.GetAsync<ServiceRequest>($"api/servicerequests/{id}");
        }

        public async Task CreateServiceRequestAsync(ServiceRequest request)
        {
            await _api.PostAsync("api/servicerequests", request);
        }

        public async Task UpdateAsync(ServiceRequest request)
        {
            await _api.PutAsync($"api/servicerequests/{request.Id}", request);
        }

        public async Task DeleteAsync(int id)
        {
            await _api.DeleteAsync($"api/servicerequests/{id}");
        }
    }
}