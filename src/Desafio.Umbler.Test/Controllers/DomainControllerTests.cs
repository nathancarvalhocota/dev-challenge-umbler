using Desafio.Umbler.Controllers;
using Desafio.Umbler.DTOs;
using Desafio.Umbler.Services;
using Desafio.Umbler.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Desafio.Umbler.Test.Controllers
{
    [TestClass]
    public class DomainControllerTests
    {
        [TestMethod]
        public void Domain_In_Database()
        {
            //arrange 
            var service = new Mock<IDomainService>();
            service.Setup(s => s.GetDomainInfoAsync("test.com")).ReturnsAsync(new DomainInfoDto { Name = "test.com", Ip = "192.168.0.1" });

            var controller = new DomainController(service.Object);

            //act
            var response = controller.Get("test.com");
            var result = response.Result as OkObjectResult;
            var obj = result.Value as DomainInfoDto;
            Assert.AreEqual("192.168.0.1", obj.Ip);
            Assert.AreEqual("test.com", obj.Name);
        }

        [TestMethod]
        public void Domain_Returns_Dto_For_Existing_Domain()
        {
            //arrange 
            var service = new Mock<IDomainService>();
            service.Setup(s => s.GetDomainInfoAsync("test.com")).ReturnsAsync(new DomainInfoDto { Name = "test.com" });

            var controller = new DomainController(service.Object);

            //act
            var response = controller.Get("test.com");
            var result = response.Result as OkObjectResult;
            var obj = result.Value as DomainInfoDto;
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void Domain_Invalid_Returns_BadRequest()
        {
            //arrange 
            var service = new Mock<IDomainService>(MockBehavior.Strict);
            var controller = new DomainController(service.Object);

            //act
            var response = controller.Get("127.0.0.1");
            var result = response.Result as BadRequestObjectResult;

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);

            var payload = result.Value;
            var codigo = payload.GetType().GetProperty("codigo")?.GetValue(payload)?.ToString();
            var mensagem = payload.GetType().GetProperty("mensagem")?.GetValue(payload)?.ToString();
            var expected = DomainNameValidator.Validate("127.0.0.1");
            Assert.AreEqual(expected.Codigo, codigo);
            Assert.AreEqual(expected.Mensagem, mensagem);
        }

        [TestMethod]
        public void Domain_Moking_LookupClient()
        {
            //arrange 
            var service = new Mock<IDomainService>();
            service.Setup(s => s.GetDomainInfoAsync("test.com")).ReturnsAsync(new DomainInfoDto { Name = "test.com" });

            var controller = new DomainController(service.Object);

            //act
            var response = controller.Get("test.com");
            var result = response.Result as OkObjectResult;
            var obj = result.Value as DomainInfoDto;

            //assert
            Assert.IsNotNull(obj);
            service.Verify(s => s.GetDomainInfoAsync("test.com"), Times.Once);
        }
    }
}
