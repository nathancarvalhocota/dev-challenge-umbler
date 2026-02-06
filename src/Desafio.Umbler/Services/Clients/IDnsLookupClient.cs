using System.Threading.Tasks;

namespace Desafio.Umbler.Services.Clients
{
    public interface IDnsLookupClient
    {
        Task<DnsLookupResult> QueryARecordAsync(string domainName);
    }
}
