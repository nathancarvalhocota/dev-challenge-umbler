using System;
using System.Threading.Tasks;
using Desafio.Umbler.DTOs;
using Desafio.Umbler.Models;
using Desafio.Umbler.Repositories;
using Desafio.Umbler.Services.Clients;
using Desafio.Umbler.Validation;

namespace Desafio.Umbler.Services
{
    public class DomainService : IDomainService
    {
        private const string StatusOk = "OK";
        private const string StatusNoARecord = "NO_A_RECORD";
        private const string MessageNoARecord = "Domínio registrado, mas sem apontamento DNS (registro A).";

        private readonly IDomainRepository _repository;
        private readonly IWhoisClient _whoisClient;
        private readonly IDnsLookupClient _dnsLookupClient;

        public DomainService(IDomainRepository repository, IWhoisClient whoisClient, IDnsLookupClient dnsLookupClient)
        {
            _repository = repository;
            _whoisClient = whoisClient;
            _dnsLookupClient = dnsLookupClient;
        }

        public async Task<DomainInfoDto> GetDomainInfoAsync(string domainName)
        {
            DomainValidationResult validation = DomainNameValidator.Validate(domainName);
            if (!validation.IsValid)
                return null;
            
            domainName = validation.NormalizedDomain;

            Domain domain = await _repository.GetByNameAsync(domainName);

            if (domain == null)
            {
                var externalData = await QueryExternalDomainDataAsync(domainName);

                domain = new Domain
                {
                    Name = domainName,
                    Ip = externalData.Ip,
                    UpdatedAt = DateTime.Now,
                    WhoIs = externalData.WhoIsRaw,
                    Ttl = externalData.Ttl,
                    HostedAt = externalData.HostedAt
                };

                await _repository.AddAsync(domain);
            }

            if (DateTime.Now.Subtract(domain.UpdatedAt).TotalMinutes > domain.Ttl)
            {
                ExternalDomainData externalData = await QueryExternalDomainDataAsync(domainName);

                domain.Name = domainName;
                domain.Ip = externalData.Ip;
                domain.UpdatedAt = DateTime.Now;
                domain.WhoIs = externalData.WhoIsRaw;
                domain.Ttl = externalData.Ttl;
                domain.HostedAt = externalData.HostedAt;
            }

            await _repository.SaveChangesAsync();

            bool hasARecord = !string.IsNullOrWhiteSpace(domain.Ip);

            return new DomainInfoDto
            {
                Name = domain.Name,
                Ip = hasARecord ? domain.Ip : null,
                HostedAt = hasARecord ? domain.HostedAt : null,
                HasARecord = hasARecord,
                Status = hasARecord ? StatusOk : StatusNoARecord,
                Message = hasARecord ? null : MessageNoARecord
            };
        }

        private async Task<ExternalDomainData> QueryExternalDomainDataAsync(string domainName)
        {
            WhoisQueryResult response = await _whoisClient.QueryAsync(domainName);
            DnsLookupResult dnsResult = await _dnsLookupClient.QueryARecordAsync(domainName);
            string ip = dnsResult?.Ip;

            string hostedAt = null;
            if (!string.IsNullOrWhiteSpace(ip))
            {
                var hostResponse = await _whoisClient.QueryAsync(ip);
                hostedAt = hostResponse.OrganizationName;
            }

            return new ExternalDomainData(ip, response.Raw, hostedAt, dnsResult?.Ttl ?? 0);
        }

        private sealed class ExternalDomainData
        {
            public ExternalDomainData(string ip, string whoIsRaw, string hostedAt, int ttl)
            {
                Ip = ip;
                WhoIsRaw = whoIsRaw;
                HostedAt = hostedAt;
                Ttl = ttl;
            }

            public string Ip { get; }
            public string WhoIsRaw { get; }
            public string HostedAt { get; }
            public int Ttl { get; }
        }
    }

}
