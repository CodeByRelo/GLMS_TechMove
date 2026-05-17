using GLMS.Core.Entities;
using GLMS.Core.Interfaces;
using GLMS.Infrastructure.Data;

namespace GLMS.Infrastructure.Repositories
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(AppDbContext context) : base(context)
        {
        }
    }
}