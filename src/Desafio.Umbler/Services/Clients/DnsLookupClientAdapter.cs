using System.Linq;
using System.Threading.Tasks;
using DnsClient;

namespace Desafio.Umbler.Services.Clients
{
    public class DnsLookupClientAdapter : IDnsLookupClient
    {
        public async Task<DnsLookupResult> QueryARecordAsync(string domainName)
        {
            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(domainName, QueryType.ANY);
            var record = result.Answers.ARecords().FirstOrDefault();
            var ip = record?.Address?.ToString();
            var ttl = record?.TimeToLive ?? 0;
            return new DnsLookupResult(ip, ttl);
        }
    }
}
