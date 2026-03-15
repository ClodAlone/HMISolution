using DocumentManager.ComponentService;
using Moq;
using NUnit.Framework;
using ScreenManager.ComponentService;
using ScreenSettings;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using static Utilities.WPF.DependencyObjectExtensions;
using DataAnalisysRTControl;
using TestContext = NUnit.Framework.TestContext;
using FastControls;
using System.Linq;
using System.Collections.Generic;

namespace ScreenManagerUnitTests
{
    [TestFixture]
    [RequiresThread(ApartmentState.STA)]
    public class ScreenDocument_PostBindConnectionStringSource_Fixture
    {
        private ScreenDocument _sut;
        private readonly string _testPath = TestContext.CurrentContext.TestDirectory;
        private readonly string _testScreenName = "DartAndSparkline.xaml";
        private string _testScreenPath;
        Canvas screenCanvas;

        [SetUp]
        public void Setup()
        {
            _testScreenPath = Path.Combine(_testPath, new ScreenManagerComponent().TypeLabel, _testScreenName);
            var parent = new Mock<IDocument>();
            _sut = ScreenDocument.FromFile(_testScreenPath, parent.Object);
            screenCanvas = _sut.GetCurrentXamlDocument(); //needs to be executed in a STA thread

            //var feMock = new Mock<FrameworkElement>();
            //var fe = feMock.Object;

            //var p = fe.GetType().GetProperty("ConnectionString");
            //if (p != null)
            //    p.SetValue(fe, "myfakeconnectionstring", null);
        }

        [Test]
        public void TestPostBinding_NoElements()
        {
            //Arrange
            var feEmptyList = new System.Collections.Generic.List<FrameworkElement>();

            //Act

            //Assert
            Assert.DoesNotThrow(() => _sut.PostBindConnectionStringSource(null));
            Assert.DoesNotThrow(() => _sut.PostBindConnectionStringSource(feEmptyList));
        }

        [Test]
        public void TestPostBinding_SingleElement_NullConnectionString()
        {
            //Arrange
            var dart = (from d in screenCanvas.GetChildrenOfType<DataAnalisysRT>() select d).FirstOrDefault();
            var feList = new List<FrameworkElement>() { dart };

            //Act
            _sut.PostBindConnectionStringSource(feList);

            //Assert
            Assert.That(dart.ConnectionString, Is.Null);
        }

        [Test]
        public void TestPostBinding_SingleElement_WithConnectionString()
        {
            //Arrange
            var dart = (from d in screenCanvas.GetChildrenOfType<DataAnalisysRT>() select d).FirstOrDefault();
            var initialConnectionString = "XpoProvider=MSSqlServer;data source=(local);integrated security=SSPI;initial catalog=gzNExT_104_IOServer;";
            dart.ConnectionString = initialConnectionString;
            var feList = new List<FrameworkElement>() { dart };

            //Act
            _sut.PostBindConnectionStringSource(feList);

            //Assert
            Assert.That(dart.ConnectionString, Is.EqualTo(initialConnectionString));
        }

        [Test]
        public void TestPostBinding_MultiElements_NullConnectionString()
        {
            //Arrange
            var dart = (from d in screenCanvas.GetChildrenOfType<DataAnalisysRT>() select d).FirstOrDefault();
            var spark = (from s in screenCanvas.GetChildrenOfType<SparklineChart>() select s).FirstOrDefault();
            var dataReaderModel = new DataReader.DataReaderModelXML(new DataReader.DataReaderModel("TestDataSource", String.Empty, null));
            spark.ControlDataSource = dataReaderModel;
            var feList = new List<FrameworkElement>() { dart, spark };

            //Act
            _sut.PostBindConnectionStringSource(feList);

            //Assert
            Assert.That(dart.ConnectionString, Is.Null);
            Assert.That(spark.ConnectionString, Is.Null);
            Assert.That(String.IsNullOrEmpty(spark.ControlDataSource?.ReaderModel?.Connection), Is.True);
        }

        [Test]
        public void TestPostBinding_MultiElements_WithConnectionString()
        {
            //Arrange
            var dart = (from d in screenCanvas.GetChildrenOfType<DataAnalisysRT>() select d).FirstOrDefault();
            var spark = (from s in screenCanvas.GetChildrenOfType<SparklineChart>() select s).FirstOrDefault();
            var initialConnectionString = "XpoProvider=MSSqlServer;data source=(local);integrated security=SSPI;initial catalog=gzNExT_104_IOServer;";
            var parsedConnectionString = DataReader.Helpers.XpoConversionHelper.GetConnectionStringFromXpoConnection(initialConnectionString);
            dart.ConnectionString = initialConnectionString;
            var dataReaderModel = new DataReader.DataReaderModelXML(new DataReader.DataReaderModel("TestDataSource", initialConnectionString, null));
            spark.ControlDataSource = dataReaderModel;
            var feList = new List<FrameworkElement>() { dart, spark };

            //Act
            _sut.PostBindConnectionStringSource(feList);

            //Assert
            Assert.That(dart.ConnectionString, Is.EqualTo(initialConnectionString));
            Assert.That(spark.ConnectionString, Is.Null);
            Assert.That(spark.ControlDataSource.ReaderModel.Connection, Is.EqualTo(parsedConnectionString));
        }

        [Test]
        public void TestPostBinding_SingleElement_NullControlDataSource()
        {
            //Arrange
            var spark = (from s in screenCanvas.GetChildrenOfType<SparklineChart>() select s).FirstOrDefault();
            var feList = new List<FrameworkElement>() { spark };

            //Act
            _sut.PostBindConnectionStringSource(feList);

            //Assert
            Assert.That(spark.ConnectionString, Is.Null);
            Assert.That(String.IsNullOrEmpty(spark.ControlDataSource?.ReaderModel?.Connection), Is.True);
        }
    }
}