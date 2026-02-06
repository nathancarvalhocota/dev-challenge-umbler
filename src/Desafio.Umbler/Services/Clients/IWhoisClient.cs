using System.Threading.Tasks;

namespace Desafio.Umbler.Services.Clients
{
    public interface IWhoisClient
    {
        Task<WhoisQueryResult> QueryAsync(string query);
    }
}
