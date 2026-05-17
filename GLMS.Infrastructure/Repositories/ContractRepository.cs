using GLMS.Core.Entities;
using GLMS.Core.Enums;
using GLMS.Core.Interfaces;
using GLMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GLMS.Infrastructure.Repositories
{
    public class ContractRepository : Repository<Contract>, IContractRepository
    {
        private readonly AppDbContext _context;

        public ContractRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Contract>> GetActiveContractsAsync()
        {
            return await _context.Contracts
                .Where(c => c.Status == ContractStatus.Active)
                .ToListAsync();
        }

        public async Task<Contract?> GetByIdAsync(int id)
        {
            return await _context.Contracts.FindAsync(id);
        }

        public async Task AddAsync(Contract contract)
        {
            await _context.Contracts.AddAsync(contract);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Contract contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Contract>> GetAllAsync()
        {
            return await _context.Contracts.ToListAsync();
        }
    }
}
