using ExternalAuthentication.Services;
using NUnit.Framework;
using System.Collections.Generic;

namespace ExternalAuthenticationUnitTests
{
    [TestFixture]
    public class GetRolesListServiceTests
    {
        [Test]
        public void GetRolesListService_Execute_MultipleRolesResult()
        {
            //Arrange
            var inputString = "role1, role2";
            var getRolesListService = new GetRolesListService();
            var resultMock = new List<string>()
            {
                "role1",
                "role2"
            };
            
            //Act
            var result = getRolesListService.Execute(inputString);

            //Assert
            Assert.AreEqual(result, resultMock);
        }

        [Test]
        public void GetRolesListService_Execute_SingleRoleResult()
        {
            //Arrange
            var inputString = "role1";
            var getRolesListService = new GetRolesListService();
            var resultMock = new List<string>()
            {
                "role1"
            };

            //Act
            var result = getRolesListService.Execute(inputString);

            //Assert
            Assert.AreEqual(result, resultMock);
        }

        [Test]
        public void GetRolesListService_Execute_InputStringIsEmpty()
        {
            //Arrange
            var inputString = string.Empty;
            var getRolesListService = new GetRolesListService();
            var resultMock = new List<string>();

            //Act
            var result = getRolesListService.Execute(inputString);

            //Assert
            Assert.AreEqual(result, resultMock);
        }

        [Test]
        public void GetRolesListService_Execute_InputStringEqualsComma()
        {
            //Arrange
            var inputString = ",";
            var getRolesListService = new GetRolesListService();
            var resultMock = new List<string>();

            //Act
            var result = getRolesListService.Execute(inputString);

            //Assert
            Assert.AreEqual(result, resultMock);
        }
    }
}
