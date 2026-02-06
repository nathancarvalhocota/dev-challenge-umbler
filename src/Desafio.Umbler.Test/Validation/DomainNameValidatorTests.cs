using Desafio.Umbler.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Desafio.Umbler.Test.Validation
{
    [TestClass]
    public class DomainNameValidatorTests
    {
        [TestMethod]
        public void Validate_Domain_Without_Extension_Returns_Error()
        {
            var result = DomainNameValidator.Validate("umbler");

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("DOMINIO_SEM_EXTENSAO", result.Codigo);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Mensagem));
        }

        [TestMethod]
        public void Validate_Domain_Ending_With_Dot_Returns_Error()
        {
            var result = DomainNameValidator.Validate("umbler.com.");

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("DOMINIO_TERMINA_COM_PONTO", result.Codigo);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Mensagem));
        }

        [TestMethod]
        public void Validate_Ip_Address_Returns_Error()
        {
            var result = DomainNameValidator.Validate("127.0.0.1");

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("DOMINIO_EH_IP", result.Codigo);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Mensagem));
        }
    }
}
