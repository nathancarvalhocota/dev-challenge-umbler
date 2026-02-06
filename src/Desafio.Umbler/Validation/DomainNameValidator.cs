using System;
using System.Linq;
using System.Net;

namespace Desafio.Umbler.Validation
{
    public static class DomainNameValidator
    {
        public static DomainValidationResult Validate(string domainName)
        {
            string normalized = domainName?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(normalized))
                return DomainValidationResult.Fail("DOMINIO_VAZIO", "O domínio não pode ser vazio.");

            normalized = normalized.ToLowerInvariant();

            if (normalized.Length > 253)
                return DomainValidationResult.Fail("DOMINIO_MUITO_LONGO", "O domínio excede o tamanho máximo permitido.");

            if (normalized.Any(char.IsWhiteSpace))
                return DomainValidationResult.Fail("DOMINIO_COM_ESPACOS", "O domínio não pode conter espaços.");

            if (normalized.StartsWith(".", StringComparison.Ordinal) || normalized.EndsWith(".", StringComparison.Ordinal))
                return DomainValidationResult.Fail("DOMINIO_TERMINA_COM_PONTO", "O domínio não pode iniciar ou terminar com ponto.");

            if (!normalized.Contains('.', StringComparison.Ordinal))
                return DomainValidationResult.Fail("DOMINIO_SEM_EXTENSAO", "O domínio deve conter uma extensão válida (ex: .com).");

            if (normalized.StartsWith("-", StringComparison.Ordinal) || normalized.EndsWith("-", StringComparison.Ordinal))
                return DomainValidationResult.Fail("LABEL_FORMATO_INVALIDO", "Partes do domínio não podem iniciar ou terminar com hífen.");

            if (normalized.Contains("://", StringComparison.Ordinal) ||
                normalized.Contains("/", StringComparison.Ordinal) ||
                normalized.Contains("\\", StringComparison.Ordinal) ||
                normalized.Contains("?", StringComparison.Ordinal) ||
                normalized.Contains("#", StringComparison.Ordinal) ||
                normalized.Contains("@", StringComparison.Ordinal) ||
                normalized.Contains(":", StringComparison.Ordinal))
                return DomainValidationResult.Fail("DOMINIO_COM_CARACTERES_INVALIDOS", "Informe apenas o nome do domínio, sem protocolo ou caminhos.");

            if (IPAddress.TryParse(normalized, out _))
                return DomainValidationResult.Fail("DOMINIO_EH_IP", "Endereço IP não é um domínio válido.");


            string[] labels = normalized.Split('.', StringSplitOptions.None);
            if (labels.Length < 2)
                return DomainValidationResult.Fail("DOMINIO_SEM_EXTENSAO", "O domínio deve conter uma extensão válida (ex: .com).");

            foreach (var label in labels)
            {
                if (string.IsNullOrEmpty(label))
                    return DomainValidationResult.Fail("LABEL_VAZIA", "Partes do domínio não podem estar vazias.");

                if (label.Length > 63)
                    return DomainValidationResult.Fail("LABEL_MUITO_LONGO", "Uma das partes do domínio excede o tamanho máximo permitido.");

                if (label.StartsWith("-", StringComparison.Ordinal) || label.EndsWith("-", StringComparison.Ordinal))
                    return DomainValidationResult.Fail("LABEL_FORMATO_INVALIDO", "Partes do domínio não podem iniciar ou terminar com hífen.");

                foreach (var character in label)
                    if (!char.IsLetterOrDigit(character) && character != '-')
                        return DomainValidationResult.Fail("LABEL_CARACTERES_INVALIDOS", "Partes do domínio possuem caracteres inválidos.");                    
            }

            string tld = labels[^1];
            if (tld.All(char.IsDigit))
                return DomainValidationResult.Fail("TLD_NUMERICO", "A extensão do domínio não pode ser apenas numérica.");
            

            return DomainValidationResult.Success(normalized);
        }
    }
}
