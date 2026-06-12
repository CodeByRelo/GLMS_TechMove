using GLMS.Core.Entities;
using GLMS.Core.Interfaces;
using GLMS.API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.API.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            return await _clientRepository.GetAllAsync();
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            return await _clientRepository.GetByIdAsync(id);
        }

        public async Task CreateClientAsync(Client client)
        {
            await _clientRepository.AddAsync(client);
        }

        public async Task UpdateClientAsync(Client client)
        {
            _clientRepository.Update(client);
        }

        public async Task DeleteClientAsync(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client != null)
            {
                _clientRepository.Delete(client);
            }
        }
    }
}