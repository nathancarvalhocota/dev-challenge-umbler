namespace Desafio.Umbler.Services.Clients
{
    public sealed class DnsLookupResult
    {
        public DnsLookupResult(string ip, int ttl)
        {
            Ip = ip;
            Ttl = ttl;
        }

        public string Ip { get; }
        public int Ttl { get; }
    }
}
