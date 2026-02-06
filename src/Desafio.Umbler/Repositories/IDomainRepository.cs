using System.Threading.Tasks;
using Desafio.Umbler.Models;

namespace Desafio.Umbler.Repositories
{
    public interface IDomainRepository
    {
        Task<Domain> GetByNameAsync(string normalizedName);
        Task AddAsync(Domain domain);
        void Update(Domain domain);
        Task SaveChangesAsync();
    }
}
