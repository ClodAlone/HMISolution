using DocumentManager.ComponentService;
using Moq;
using NUnit.Framework;
using UFUAEditor.Document;
using TestContext = NUnit.Framework.TestContext;

namespace UFUAEditorUnitTests
{
    [TestFixture]
    public class UFUAServerDocumentFixture
    {
        private UFUAServerDocument _sut;
        private readonly string _testPath = TestContext.CurrentContext.TestDirectory;

        [SetUp]
        public void SetUp()
        {
            var documentManager = new Mock<IDocumentManager>();
            var parent = new Mock<IDocument>();
            _sut = UFUAServerDocument.FromFile(_testPath, 
                documentManager.Object,
                parent.Object);
        }

        [Test]
        public void ComposeTagWithBackslash_GetUFUATagByName_TagReturned()
        {
            //Arrange

            //Act
            var tagFound = _sut.GetUFUATag("gs_IOTMespack\\gs_IoTMespack_s_Energy_Rn_PneumaticConsumption", "M1", false);

            //Assert
            Assert.AreEqual(tagFound.Name, "gs_IoTMespack_s_Energy_Rn_PneumaticConsumption");
            Assert.AreEqual(tagFound.OriginalName, "gs_IoTMespack_s_Energy_n_PneumaticConsumption");
            Assert.AreEqual(tagFound.FolderPath, "gs_IOTMespack");
            Assert.AreEqual(tagFound.NodeId.ToString(), "d25d7bf9-3d35-402e-b3d6-57ba19304f00");
        }

        [Test]
        public void ComposeTagWithBackslash_GetUFUATagByOriginalName_TagReturned()
        {
            //Arrange

            //Act
            var tagFound = _sut.GetUFUATag("gs_IOTMespack\\gs_IoTMespack_s_Energy_n_PneumaticConsumption", "M1", false);

            //Assert
            Assert.AreEqual(tagFound.Name, "gs_IoTMespack_s_Energy_Rn_PneumaticConsumption");
            Assert.AreEqual(tagFound.OriginalName, "gs_IoTMespack_s_Energy_n_PneumaticConsumption");
            Assert.AreEqual(tagFound.FolderPath, "gs_IOTMespack");
            Assert.AreEqual(tagFound.NodeId.ToString(), "d25d7bf9-3d35-402e-b3d6-57ba19304f00");
        }

        [Test]
        public void ComposeTagWithBackslash_GetUFUATagByOriginalFolderPath_TagReturned()
        {
            //Arrange

            //Act
            var tagFound = _sut.GetUFUATag("Parameters_TempCtrl\\RnRightWith", "M1", false);

            //Assert
            Assert.AreEqual(tagFound.Name, "RnRightWith");
            Assert.AreEqual(tagFound.OriginalFolderPath, "Parameters_TempCtrl");
            Assert.AreEqual(tagFound.FolderPath, "Parameters\\ElementPos");
            Assert.AreEqual(tagFound.NodeId.ToString(), "293cee6b-254b-493d-91a9-6dcd70b0b873");
        }

        [Test]
        public void ComposeTagWithUnderscore_GetUFUATagByName_TagReturned()
        {
            //Arrange

            //Act
            var tagFound = _sut.GetUFUATag("Pruebas_nBtnManual5", "M1", false);

            //Assert
            Assert.AreEqual(tagFound.Name, "nBtnManual5");
            Assert.AreEqual(tagFound.FolderPath, "Pruebas");
            Assert.AreEqual(tagFound.NodeId.ToString(), "01515fef-eff0-4912-b1cd-5aeb65a58757");
        }

        [Test]
        public void ComposeTagWithUnderscore_GetUFUATagByOriginalName_TagReturned()
        {
            //Arrange

            //Act
            var tagFound = _sut.GetUFUATag("Parameters_AxisControl_RnFastSpeedSP", "M1", false);

            //Assert
            Assert.AreEqual(tagFound.Name, "RnSpeedSP");
            Assert.AreEqual(tagFound.OriginalName, "RnFastSpeedSP");
            Assert.AreEqual(tagFound.FolderPath, "Parameters\\AxisControl");
            Assert.AreEqual(tagFound.NodeId.ToString(), "b22e63e7-7b12-4e2d-be0f-71617aeb8ae3");
        }

        [Test]
        public void ComposeTagWithUnderscore_GetUFUATagByOriginalFolderPath_TagReturned()
        {
            //Arrange

            //Act
            var tagFound = _sut.GetUFUATag("Parameters_TempCtrl_RnRightWith", "M1", false);

            //Assert
            Assert.AreEqual(tagFound.Name, "RnRightWith");
            Assert.AreEqual(tagFound.OriginalFolderPath, "Parameters_TempCtrl");
            Assert.AreEqual(tagFound.FolderPath, "Parameters\\ElementPos");
            Assert.AreEqual(tagFound.NodeId.ToString(), "293cee6b-254b-493d-91a9-6dcd70b0b873");
        }

        [Test]
        public void ComposeTagWithoutBackslash_GetUFUATagByName_TagReturned()
        {
            //Arrange

            //Act
            var tagFound = _sut.GetUFUATag("gs_Process_as_Selector_49__b_Button", "M1", false);

            //Assert
            Assert.AreEqual(tagFound.Name, "gs_Process_as_Selector_49__b_Button");
            Assert.IsEmpty(tagFound.FolderPath);
            Assert.AreEqual(tagFound.NodeId.ToString(), "6b7c0ada-1f6d-4a34-ad48-e361005cf4eb");
        }

        [Test]
        public void ComposeTagWithoutBackslash_GetUFUATagByOriginalName_TagReturned()
        {
            //Arrange

            //Act
            var tagFound = _sut.GetUFUATag("gs_IoTMespack_s_Infeed_n_ProductQuantity", "M1", false);

            //Assert
            Assert.AreEqual(tagFound.Name, "gs_IoTMespack_s_Infeed_Rn_ProductQuantity");
            Assert.AreEqual(tagFound.OriginalName, "gs_IoTMespack_s_Infeed_n_ProductQuantity");
            Assert.IsEmpty(tagFound.FolderPath);
            Assert.AreEqual(tagFound.NodeId.ToString(), "1d29cd09-6a2e-4084-bc1f-be23ed36c1d5");
        }
    }
}