using System.Threading.Tasks;
using Desafio.Umbler.DTOs;
using Desafio.Umbler.Services;
using Desafio.Umbler.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Umbler.Controllers
{
    [Route("api")]
    public class DomainController : Controller
    {
        private readonly IDomainService _domainService;

        public DomainController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        [HttpGet, Route("domain/{domainName}")]
        public async Task<IActionResult> Get(string domainName)
        {
            DomainValidationResult validation = DomainNameValidator.Validate(domainName);

            if (!validation.IsValid)
                return BadRequest(new { codigo = validation.Codigo, mensagem = validation.Mensagem });


            DomainInfoDto dto = await _domainService.GetDomainInfoAsync(validation.NormalizedDomain);

            return Ok(dto);
        }
    }
}
