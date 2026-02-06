using System.Threading.Tasks;
using Desafio.Umbler.Models;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Umbler.Repositories
{
    public class DomainRepository : IDomainRepository
    {
        private readonly DatabaseContext _db;

        public DomainRepository(DatabaseContext db)
        {
            _db = db;
        }

        public Task<Domain> GetByNameAsync(string normalizedName)
        {
            return _db.Domains.FirstOrDefaultAsync(d => d.Name == normalizedName);
        }

        public Task AddAsync(Domain domain)
        {
            return _db.Domains.AddAsync(domain).AsTask();
        }

        public void Update(Domain domain)
        {
            _db.Domains.Update(domain);
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
