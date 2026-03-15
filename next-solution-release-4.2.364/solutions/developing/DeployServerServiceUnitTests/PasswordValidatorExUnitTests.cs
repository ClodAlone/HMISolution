using UFUAInstallDeployServerService;

namespace DeployServerServiceUnitTests
{
    [TestClass]
    public class PasswordValidatorExUnitTests
    {
        static PasswordValidatorEx _sut;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {

        }

        [TestInitialize] 
        public void TestInitialize()
        {
            _sut = new PasswordValidatorEx();
        }

        [TestCleanup]
        public void TestCleanup()
        {

        }

        [TestMethod]
        public async Task NoRequirementsPassword()
        {
            //Arrange

            //Act
            var result = await _sut.ValidateAsync("123");

            //Assert
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task DefaultRequirementsPassword()
        {
            //Arrange
            _sut.RequiredLength = 6;
            _sut.RequireDigit = true;
            _sut.RequireLowercase = true;
            _sut.RequireUppercase = true;
            _sut.RequireNonLetterOrDigit = true;
            _sut.RequiredUniqueChars = 1;

            //Act
            var notvalid = await _sut.ValidateAsync("123");
            var valid = await _sut.ValidateAsync("Admin@123");

            //Assert
            Assert.IsFalse(notvalid.Succeeded);
            Assert.IsTrue(valid.Succeeded);
        }

        [TestMethod]
        public async Task StrongRequirementsPassword()
        {
            //Arrange
            _sut.RequiredLength = 15;
            _sut.RequireDigit = true;
            _sut.RequireLowercase = true;
            _sut.RequireUppercase = true;
            _sut.RequireNonLetterOrDigit = true;
            _sut.RequiredUniqueChars = 10;

            //Act
            var notvalid1 = await _sut.ValidateAsync("123");
            var notvalid2 = await _sut.ValidateAsync("Admin@123");
            var lengthButNotRequired = await _sut.ValidateAsync("Admin@123iiiiiiiii");
            var requiredButNotLength = await _sut.ValidateAsync("abcdefghijkl");
            var valid = await _sut.ValidateAsync("Admin@123opqrstuvz");

            //Assert
            Assert.IsFalse(notvalid1.Succeeded);
            Assert.IsFalse(notvalid2.Succeeded);
            Assert.IsFalse(lengthButNotRequired.Succeeded);
            Assert.IsFalse(requiredButNotLength.Succeeded);
            Assert.IsTrue(valid.Succeeded);
        }

        [TestMethod]
        public async Task NegativeUniqueCharsTestPassword()
        {
            //Arrange
            _sut.RequiredUniqueChars = int.MinValue;

            //Act
            var singleCharValid = await _sut.ValidateAsync("A");

            //Assert
            Assert.IsTrue(singleCharValid.Succeeded);
        }

        [TestMethod]
        public async Task ZeroUniqueCharsTestPassword()
        {
            //Arrange
            _sut.RequiredUniqueChars = 0;

            //Act
            var singleCharValid = await _sut.ValidateAsync("A");
            
            //Assert
            Assert.IsTrue(singleCharValid.Succeeded);
        }

        [TestMethod]
        public async Task EmptyNullTestPassword()
        {
            //Arrange
            _sut.RequiredLength = 0;
            _sut.RequireDigit = false;
            _sut.RequireLowercase = false;
            _sut.RequireUppercase = false;
            _sut.RequireNonLetterOrDigit = false;
            _sut.RequiredUniqueChars = 0;

            //Act
            var emptyPwdNeverAllowed = await _sut.ValidateAsync("");

            //Assert
            Assert.IsFalse(emptyPwdNeverAllowed.Succeeded);
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                var nullPwdNeverAllowed = await _sut.ValidateAsync(null);
            });
        }
    }
}