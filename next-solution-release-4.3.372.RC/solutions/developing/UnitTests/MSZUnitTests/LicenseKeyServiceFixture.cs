using MSZ.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using WPFUtilities.CryptString;

namespace MSZUnitTests
{
    [TestFixture]
    public class LicenseKeyServiceFixture
    {
        private readonly string _filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "lic");
        private LicenseKeyService _sut;

        [SetUp]
        public void SetUp()
        {
            const string modifier = "removed: ";
            var encryptedFilePath = CryptString.EncryptString(_filePath);
            _sut = new LicenseKeyService(modifier, encryptedFilePath);
        }

        [SetUp, TearDown]
        public void TearDown()
        {
            if (File.Exists(_filePath))
                File.Delete(_filePath);
        }

        [Test]
        public void InvalidFilePath_Read_EmptyListReturned()
        {
            //Arrange

            //Act
            var actual = _sut.ReadAll();

            //Assert
            Assert.IsEmpty(actual);
        }


        [Test]
        public void ExistingKeys_Read_PassedKeysReturned()
        {
            //Arrange
            var keys = new List<string> { "key1", "key2" };

            //Act
            var actual = _sut.ReadAll(keys);

            //Assert
            Assert.AreEqual(actual.Count, 2);
            Assert.AreEqual(actual[0], "key1");
            Assert.AreEqual(actual[1], "key2");
        }

        [Test]
        public void SingleValidKey_ReadAll_KeyReturned()
        {
            //Arrange
            File.WriteAllText(_filePath, "2NcQbloMuHvaU1DSWHRXPewuf9V3Ih8/zCLxmj0RRSA=");

            //Act
            var actual = _sut.ReadAll();

            //Assert
            Assert.AreEqual(actual.Count, 1);
            Assert.AreEqual(actual[0], "20202035");
        }

        [Test]
        public void MultipleDuplicatedValidKeys_ReadAll_SingleKeyReturned()
        {
            //Arrange
            File.WriteAllText(_filePath, "2NcQbloMuHvaU1DSWHRXPewuf9V3Ih8/zCLxmj0RRSA=\n\n" +
                                         "2NcQbloMuHvaU1DSWHRXPewuf9V3Ih8/zCLxmj0RRSA=\n\n\n");

            //Act
            var actual = _sut.ReadAll();

            //Assert
            Assert.AreEqual(actual.Count, 1);
            Assert.AreEqual(actual[0], "20202035");
        }

        [Test]
        public void MultipleValidKeys_ReadAll_KeysReturned()
        {
            //Arrange
            File.WriteAllText(_filePath, "2NcQbloMuHvaU1DSWHRXPewuf9V3Ih8/zCLxmj0RRSA=\n" +
                                         "qcNPJEsFcVvX6nBGSZPhBVVddQfqZKKiUkij+qWM+9M=\n" + "" +
                                         "Ltyi5HgtBjIY5LmuoexQ1HxWlW+ZfpxcwdUsSSnchSM=");
            
            //Act
            var actual = _sut.ReadAll();

            //Assert
            Assert.AreEqual(actual.Count, 3);
            Assert.AreEqual(actual[0], "20202035");
            Assert.AreEqual(actual[1], "20201111");
            Assert.AreEqual(actual[2], "20202222");
        }

        [Test]
        public void MultipleValidKeysWithDifferentModifiers_ReadAll_KeysReturned()
        {
            //Arrange
            File.WriteAllText(_filePath, "NSQxEgUWvFbAsPeJ0s3Wsg==\n" + //AAAAA: 20200000
                                         "qcNPJEsFcVvX6nBGSZPhBVVddQfqZKKiUkij+qWM+9M=\n" + "" +
                                         "dgHNI/FNlBI54M/T9+6ZqQ=="); //pippo: 20203333

            //Act
            var actual = _sut.ReadAll();

            //Assert
            Assert.AreEqual(actual.Count, 3);
            Assert.AreEqual(actual[0], "AAAAA: 20200000");
            Assert.AreEqual(actual[1], "20201111");
            Assert.AreEqual(actual[2], "pippo: 20203333");
        }

        [Test]
        public void NotEncryptedFilePath_Write_FileNotCreated()
        {
            //Arrange
            var sut = new LicenseKeyService("mmm", "fakePath");

            //Act
            sut.Write("aaa");

            //Assert
            Assert.IsFalse(File.Exists(_filePath));
        }

        [Test]
        public void EmptyFile_WriteCodes_CodeAdded()
        {
            //Arrange
            
            //Act
            _sut.Write("pippo");

            //Assert
            Assert.IsTrue(File.Exists(_filePath));
            Assert.AreEqual(File.ReadAllLines(_filePath).Length, 1);
            Assert.AreEqual(File.ReadAllLines(_filePath)[0], "pippo");
        }

        [Test]
        public void NotEmptyFile_WriteCode_CodeAppended()
        {
            //Arrange
            _sut.Write("pippo");

            //Act
            _sut.Write("pluto");

            //Assert
            Assert.IsTrue(File.Exists(_filePath));
            Assert.AreEqual(File.ReadAllLines(_filePath).Length, 2);
            Assert.AreEqual(File.ReadAllLines(_filePath)[0], "pippo");
            Assert.AreEqual(File.ReadAllLines(_filePath)[1], "pluto");
        }

        [Test]
        public void NotEmptyFile_WriteEmptyCode_CodeNotAppended()
        {
            //Arrange
            _sut.Write("pippo");

            //Act
            _sut.Write("");

            //Assert
            Assert.IsTrue(File.Exists(_filePath));
            Assert.AreEqual(File.ReadAllLines(_filePath).Length, 1);
            Assert.AreEqual(File.ReadAllLines(_filePath)[0], "pippo");
        }

        [Test]
        public void NotEmptyFile_WriteNullCode_CodeNotAppended()
        {
            //Arrange
            _sut.Write("pippo");

            //Act
            _sut.Write(null);

            //Assert
            Assert.IsTrue(File.Exists(_filePath));
            Assert.AreEqual(File.ReadAllLines(_filePath).Length, 1);
            Assert.AreEqual(File.ReadAllLines(_filePath)[0], "pippo");
        }

        [Test]
        public void NotEmptyFile_WriteWhitespaceCode_CodeNotAppended()
        {
            //Arrange
            _sut.Write("pippo");

            //Act
            _sut.Write("       ");

            //Assert
            Assert.IsTrue(File.Exists(_filePath));
            Assert.AreEqual(File.ReadAllLines(_filePath).Length, 1);
            Assert.AreEqual(File.ReadAllLines(_filePath)[0], "pippo");
        }

        [Test]
        public void ExistingFile_CleanAll_FileRemoved()
        {
            //Arrange
            _sut.Write("pippo");

            //Act
            _sut.CleanAll();

            //Assert
            Assert.IsFalse(File.Exists(_filePath));
        }

        [Test]
        public void NotEncryptedFilePath_CleanAll_FileNotCreated()
        {
            //Arrange
            var sut = new LicenseKeyService("mmm", "fakePath");

            //Act
            sut.CleanAll();

            //Assert
            Assert.IsFalse(File.Exists(_filePath));
        }

        [Test]
        public void NotExistingFile_Remove_NullReturned()
        {
            //Arrange

            //Act
            var actual = _sut.Remove("pippo");

            //Assert
            Assert.IsNull(actual);
            Assert.IsFalse(File.Exists(_filePath));
        }

        [Test]
        public void FileWithoutCode_Remove_NullReturned()
        {
            //Arrange
            _sut.Write("pippo");
            _sut.Write("pluto");

            //Act
            var actual = _sut.Remove("paperino");

            //Assert
            Assert.IsNull(actual);
            Assert.IsTrue(File.Exists(_filePath));
            Assert.AreEqual(File.ReadAllLines(_filePath).Length, 2);
            Assert.AreEqual(File.ReadAllLines(_filePath)[0], "pippo");
            Assert.AreEqual(File.ReadAllLines(_filePath)[1], "pluto");
        }

        [Test]
        public void FileWithCode_RemoveInvalidEncryptedCode_NullReturned()
        {
            //Arrange
            _sut.Write("pippo");
            _sut.Write("pluto");
            _sut.Write("paperino");

            //Act
            var actual = _sut.Remove("pluto");

            //Assert
            Assert.IsNull(actual);
            Assert.IsTrue(File.Exists(_filePath));
            Assert.AreEqual(File.ReadAllLines(_filePath).Length, 3);
            Assert.AreEqual(File.ReadAllLines(_filePath)[0], "pippo");
            Assert.AreEqual(File.ReadAllLines(_filePath)[1], "pluto");
            Assert.AreEqual(File.ReadAllLines(_filePath)[2], "paperino");
        }

        [Test]
        public void FileWithCode_Remove_CodeWithModifierReturned()
        {
            //Arrange
            _sut.Write("Y2JS5O3Sj9g+svC2DyIczw==");
            _sut.Write("pDdV9FPT+k1WoVTLAnBuSA==");
            _sut.Write("mKYgCmPio08i9RZVYEuS2304U2BC+BcD+Rw0texUYVA=");

            //Act
            var actual = _sut.Remove("pDdV9FPT+k1WoVTLAnBuSA==");

            //Assert
            Assert.AreEqual(actual, "Y6UKhEllfMNsZsWUUao9MA==");
            Assert.IsTrue(File.Exists(_filePath));
            Assert.AreEqual(File.ReadAllLines(_filePath).Length, 2);
            Assert.AreEqual(File.ReadAllLines(_filePath)[0], "Y2JS5O3Sj9g+svC2DyIczw==");
            Assert.AreEqual(File.ReadAllLines(_filePath)[1], "mKYgCmPio08i9RZVYEuS2304U2BC+BcD+Rw0texUYVA=");
        }

        [Test]
        public void FileWithCode_Remove_CodeWithoutModifierReturned()
        {
            //Arrange
            _sut.Write("U3kYp+mslS5dcRH5bytalA==");
            _sut.Write("Y6UKhEllfMNsZsWUUao9MA==");
            _sut.Write("9kfIG/jNSIg3SlqYqkD45Q==");

            //Act
            var actual = _sut.Remove("9kfIG/jNSIg3SlqYqkD45Q==");

            //Assert
            Assert.AreEqual(actual, "9kfIG/jNSIg3SlqYqkD45Q==");
            Assert.IsTrue(File.Exists(_filePath));
            Assert.AreEqual(File.ReadAllLines(_filePath).Length, 2);
            Assert.AreEqual(File.ReadAllLines(_filePath)[0], "U3kYp+mslS5dcRH5bytalA==");
            Assert.AreEqual(File.ReadAllLines(_filePath)[1], "Y6UKhEllfMNsZsWUUao9MA==");
        }
    }
}
