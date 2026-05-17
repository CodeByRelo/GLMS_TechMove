using GLMS.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.Web.Services.Interfaces
{
    public interface IClientService
    {
        Task<List<Client>> GetAllClientsAsync();
        Task<Client> GetClientByIdAsync(int id);
        Task CreateClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task DeleteClientAsync(int id);
    }
}