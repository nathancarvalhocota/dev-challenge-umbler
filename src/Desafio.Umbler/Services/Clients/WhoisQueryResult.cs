namespace Desafio.Umbler.Services.Clients
{
    public sealed class WhoisQueryResult
    {
        public WhoisQueryResult(string raw, string organizationName)
        {
            Raw = raw;
            OrganizationName = organizationName;
        }

        public string Raw { get; }
        public string OrganizationName { get; }
    }
}
