using System.Threading.Tasks;
using Whois.NET;

namespace Desafio.Umbler.Services.Clients
{
    public class WhoisClientAdapter : IWhoisClient
    {
        public async Task<WhoisQueryResult> QueryAsync(string query)
        {
            var response = await WhoisClient.QueryAsync(query);
            return new WhoisQueryResult(response.Raw, response.OrganizationName);
        }
    }
}
