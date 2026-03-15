using CommandManager;
using ScreenSettings.Entities;
using SVGHelper;
using System.Windows.Controls;
using System.Windows.Markup;

namespace DocSVGHelperUnitTests
{
    [TestClass]
    public class ValueCommandInheritanceUnitTests
    {
        static DocSVGHelper _sut;
        static SVGHelperList testTagReferences;
        readonly string entityName = "fakeEntity";
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var doc = new ScreenSettings.ScreenDocument();
            //doc.Parent = UFProjectDocument.FromFile([...]);
            var projectFullPath = Path.GetFullPath(@"ValueCommandInheritanceProject/MyTestPad.UFProject");
            doc.InitXamlDocument(projectFullPath, XamlWriter.Save(new Canvas()));

            testTagReferences = DocSVGHelper.ImportSVGTagUsed(doc);
            _sut = new DocSVGHelper(doc, null, new Dictionary<string, string>(), String.Empty);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            
        }

        [TestInitialize]
        public void TestInitialize()
        {
            
        }

        [TestCleanup]
        public void TestCleanup()
        {
            
        }

        [TestMethod]
        public void TestDefaultEditDisplayInheritance()
        {
            //Arrange
            var valueCommand = new ValueCommand();
            var parentEntityFrameworkElement = new EditDisplay.EditDisplay(); 
            var parentEntity = new ScreenEntity(parentEntityFrameworkElement);

            //Act
            _sut.AddCommandReference(entityName, valueCommand, null, null, new Dictionary<string, Dictionary<string, string>>(), null, parentEntity);

            //Assert
            Assert.IsNull(valueCommand.SVGMinValueReferenceId);
            Assert.IsNull(valueCommand.SVGMaxValueReferenceId);
            Assert.AreEqual(valueCommand.MinValue, Convert.ToDouble(parentEntityFrameworkElement.GetMinValue()));
            Assert.AreEqual(valueCommand.MaxValue, Convert.ToDouble(parentEntityFrameworkElement.GetMaxValue()));
            Assert.AreEqual(valueCommand.UseEUnit, parentEntityFrameworkElement.GetUseEngineeringUnit());
            Assert.AreEqual(valueCommand.Decimals, Convert.ToUInt32(parentEntityFrameworkElement.GetPrecisionDigit()));
        }

        [TestMethod]
        public void TestEditDisplayCustomPropValuesInheritance()
        {
            //Arrange
            var valueCommand = new ValueCommand();

            decimal controlMin = 16;
            decimal controlMax = 26;
            int precisionDigits = 6;
            bool bUseEUnit = false;
            var parentEntityFrameworkElement = new EditDisplay.EditDisplay()
            {
                MinValue = controlMin,
                MaxValue = controlMax,
                PrecisionDigits = precisionDigits,
                UseEUnit = bUseEUnit
            };
            var parentEntity = new ScreenEntity(parentEntityFrameworkElement);

            //Act
            _sut.AddCommandReference(entityName, valueCommand, null, null, new Dictionary<string, Dictionary<string, string>>(), null, parentEntity);

            //Assert
            Assert.AreEqual(valueCommand.MinValue, Convert.ToDouble(controlMin));
            Assert.AreEqual(valueCommand.MaxValue, Convert.ToDouble(controlMax));
            Assert.AreEqual(valueCommand.UseEUnit, bUseEUnit);
            Assert.AreEqual(valueCommand.Decimals, Convert.ToUInt32(precisionDigits));
        }

        [TestMethod]
        public void TestEditDisplayDynamicTagsInheritance()
        {
            //Arrange
            var tagMinValueReference = (from tag in testTagReferences where tag.SVGReferenceId == 2 select tag).First().Tag;
            var tagMaxValueReference = (from tag in testTagReferences where tag.SVGReferenceId == 3 select tag).First().Tag;
            var valueCommand = new ValueCommand();
            var parentEntityFrameworkElement = new EditDisplay.EditDisplay()
            {
                TagMinValue = new OPCUAViewModel.OPCUAXMLEntityReference() { TagReference = tagMinValueReference },
                TagMaxValue = new OPCUAViewModel.OPCUAXMLEntityReference() { TagReference = tagMaxValueReference }
            };
            var parentEntity = new ScreenEntity(parentEntityFrameworkElement);

            //Act
            _sut.AddCommandReference(entityName, valueCommand, null, null, new Dictionary<string, Dictionary<string, string>>(), null, parentEntity);

            //Assert
            Assert.IsNotNull(valueCommand.SVGMinValueReferenceId);
            Assert.IsNotNull(valueCommand.SVGMaxValueReferenceId);
        }

        [TestMethod]
        public void TestEditDisplayDynamicTagsInheritance_NullTagReference()
        {
            //Arrange
            var valueCommand = new ValueCommand();
            var parentEntityFrameworkElement = new EditDisplay.EditDisplay()
            {
                TagMinValue = new OPCUAViewModel.OPCUAXMLEntityReference() { TagReference = null },
                TagMaxValue = new OPCUAViewModel.OPCUAXMLEntityReference() { TagReference = null }
            };
            var parentEntity = new ScreenEntity(parentEntityFrameworkElement);

            //Act
            _sut.AddCommandReference(entityName, valueCommand, null, null, new Dictionary<string, Dictionary<string, string>>(), null, parentEntity);

            //Assert
            Assert.IsNull(valueCommand.SVGMinValueReferenceId);
            Assert.IsNull(valueCommand.SVGMaxValueReferenceId);
        }

        [TestMethod]
        public void TestEditDisplayDynamicTagsInheritance_InvalidTagReference()
        {
            //Arrange
            var valueCommand = new ValueCommand();
            var tagMinValueReference = (from tag in testTagReferences where tag.SVGReferenceId == 2 select tag).First().Tag;
            var tagMaxValueReference = (from tag in testTagReferences where tag.SVGReferenceId == 3 select tag).First().Tag;
            
            tagMinValueReference.AppName = null; //Invalidating the tag reference
            tagMaxValueReference.AppName = null;

            var parentEntityFrameworkElement = new EditDisplay.EditDisplay()
            {
                TagMinValue = new OPCUAViewModel.OPCUAXMLEntityReference() { TagReference = tagMinValueReference },
                TagMaxValue = new OPCUAViewModel.OPCUAXMLEntityReference() { TagReference = tagMinValueReference }
            };
            var parentEntity = new ScreenEntity(parentEntityFrameworkElement);

            //Act
            _sut.AddCommandReference(entityName, valueCommand, null, null, new Dictionary<string, Dictionary<string, string>>(), null, parentEntity);

            //Assert
            Assert.AreEqual(valueCommand.SVGMinValueReferenceId, -1);
            Assert.AreEqual(valueCommand.SVGMaxValueReferenceId, -1);
        }

        [TestMethod]
        public void TestInheritFromControlFalse()
        {
            //Arrange
            var valueCommand = new ValueCommand() { InheritFromControl = false };
            decimal controlMin = 16;
            decimal controlMax = 26;
            int precisionDigits = 6;
            var parentEntityFrameworkElement = new EditDisplay.EditDisplay() {
                MinValue = controlMin, 
                MaxValue = controlMax,
                PrecisionDigits = precisionDigits
            };
            var parentEntity = new ScreenEntity(parentEntityFrameworkElement);

            //Act
            _sut.AddCommandReference(entityName, valueCommand, null, null, new Dictionary<string, Dictionary<string, string>>(), null, parentEntity);

            //Assert
            Assert.AreNotEqual(valueCommand.MinValue, Convert.ToDouble(controlMin));
            Assert.AreNotEqual(valueCommand.MaxValue, Convert.ToDouble(controlMax));
            Assert.AreNotEqual(valueCommand.Decimals, Convert.ToUInt32(precisionDigits));
        }

        [TestMethod]
        public void TestControlNotInheritable()
        {
            //Arrange
            var valueCommand = new ValueCommand();
            double controlMin = 16;
            double controlMax = 26;
            var parentEntityFrameworkElement = new SpinControl.SpinControl()
            {
                MinValue = controlMin,
                MaxValue = controlMax,
            };
            var parentEntity = new ScreenEntity(parentEntityFrameworkElement);

            //Act
            _sut.AddCommandReference(entityName, valueCommand, null, null, new Dictionary<string, Dictionary<string, string>>(), null, parentEntity);

            //Assert
            Assert.AreNotEqual(valueCommand.MinValue, controlMin);
            Assert.AreNotEqual(valueCommand.MaxValue, controlMax);
        }
    }
}
