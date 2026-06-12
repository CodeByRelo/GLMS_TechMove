using GLMS.Core.Entities;
using GLMS.Web.Services.Interfaces;

namespace GLMS.Web.Services
{
    public class ClientApiService : IClientService
    {
        private readonly ApiClient _api;

        public ClientApiService(ApiClient api)
        {
            _api = api;
        }

        public Task<List<Client>> GetAllClientsAsync()
        {
            return _api.GetAsync<List<Client>>("api/clients");
        }

        public Task<Client> GetClientByIdAsync(int id)
        {
            return _api.GetAsync<Client>($"api/clients/{id}");
        }

        public Task CreateClientAsync(Client client)
        {
            return _api.PostAsync("api/clients", client);
        }

        public Task UpdateClientAsync(Client client)
        {
            return _api.PutAsync($"api/clients/{client.Id}", client);
        }

        public Task DeleteClientAsync(int id)
        {
            return _api.DeleteAsync($"api/clients/{id}");
        }
    }
}