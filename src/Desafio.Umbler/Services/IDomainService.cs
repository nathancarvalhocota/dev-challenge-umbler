using System;
using System.Threading.Tasks;
using Desafio.Umbler.DTOs;

namespace Desafio.Umbler.Services
{
    public interface IDomainService
    {
        Task<DomainInfoDto> GetDomainInfoAsync(string domainName);
    }
}
    