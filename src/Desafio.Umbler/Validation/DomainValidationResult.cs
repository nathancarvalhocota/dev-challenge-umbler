namespace Desafio.Umbler.Validation
{
    public sealed class DomainValidationResult
    {
        private DomainValidationResult(bool isValid, string codigo, string mensagem, string normalizedDomain)
        {
            IsValid = isValid;
            Codigo = codigo;
            Mensagem = mensagem;
            NormalizedDomain = normalizedDomain;
        }

        public bool IsValid { get; }
        public string Codigo { get; }
        public string Mensagem { get; }
        public string NormalizedDomain { get; }

        public static DomainValidationResult Success(string normalizedDomain)
        {
            return new DomainValidationResult(true, string.Empty, string.Empty, normalizedDomain);
        }

        public static DomainValidationResult Fail(string codigo, string mensagem)
        {
            return new DomainValidationResult(false, codigo, mensagem, string.Empty);
        }
    }
}
