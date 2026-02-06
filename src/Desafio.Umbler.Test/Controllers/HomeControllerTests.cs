using Desafio.Umbler.Controllers;
using Desafio.Umbler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Desafio.Umbler.Test.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void Home_Index_returns_View()
        {
            //arrange 
            var controller = new HomeController();

            //act
            var response = controller.Index();
            var result = response as ViewResult;

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Home_Error_returns_View_With_Model()
        {
            //arrange 
            var controller = new HomeController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            //act
            var response = controller.Error();
            var result = response as ViewResult;
            var model = result.Model as ErrorViewModel;

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
        }
    }
}
