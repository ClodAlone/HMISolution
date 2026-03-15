using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UFUAEditor.ViewModels;
using UFUAModel;
using WPFUtilities.Services;

namespace WpfUtilitiesTests
{
    [TestFixture]
    public class CsvImportServiceTests
    {

        private string _filePath;
        private string _delimiter;
        private CsvImportService<ImportEngineeringUnit, UFUAEngineeringUnitMap> _mockCsvImportService;
        [SetUp]
        public void Setup()
        {
            _mockCsvImportService = new CsvImportService<ImportEngineeringUnit, UFUAEngineeringUnitMap>();
        }

        [Test]
        public void CsvImportService_ImportCsv_ResultSucceded()
        {
            //Arrange
            
            _filePath = @"MockFiles\UNECE_to_OPCUA_Mock.csv";
            _delimiter = ",";
            var mockResult = new List<ImportEngineeringUnit>()
            {
                new ImportEngineeringUnit(){ UNECECode="C81", UnitId=4405297, UnitName="rad", Description="radian" },
                new ImportEngineeringUnit(){ UNECECode="C25", UnitId=4403765, UnitName="mrad", Description="milliradian" }
            };

            //Act
            var result = _mockCsvImportService.ImportCsv(_filePath, _delimiter);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result[0].UNECECode, mockResult[0].UNECECode);
            Assert.AreEqual(result[0].UnitId, mockResult[0].UnitId);
            Assert.AreEqual(result[0].UnitName, mockResult[0].UnitName);
            Assert.AreEqual(result[0].Description, mockResult[0].Description);
            Assert.AreEqual(result[1].UNECECode, mockResult[1].UNECECode);
            Assert.AreEqual(result[1].UnitId, mockResult[1].UnitId);
            Assert.AreEqual(result[1].UnitName, mockResult[1].UnitName);
            Assert.AreEqual(result[1].Description, mockResult[1].Description);

        }

        [Test]
        public void CsvImportService_ImportCsv_FileNotFoundError()
        {
            //Arrange
            _filePath = @"MockFiles\WrongFilename.csv";
            _delimiter = ",";
            var mockResult = new List<ImportEngineeringUnit>()
            {
                new ImportEngineeringUnit(){ UNECECode="C81", UnitId=4405297, UnitName="rad", Description="radian" },
                new ImportEngineeringUnit(){ UNECECode="C25", UnitId=4403765, UnitName="mrad", Description="milliradian" }
            };

            //Act            

            //Assert
            Assert.Throws<Exception>(() => _mockCsvImportService.ImportCsv(_filePath, _delimiter));
        }

        [Test]
        public void CsvImportService_ImportCsv_WrongDelimiterError()
        {
            //Arrange
            _filePath = @"MockFiles\UNECE_to_OPCUA_Mock.csv";
            _delimiter = ".";
            var mockResult = new List<ImportEngineeringUnit>()
            {
                new ImportEngineeringUnit(){ UNECECode="C81", UnitId=4405297, UnitName="rad", Description="radian" },
                new ImportEngineeringUnit(){ UNECECode="C25", UnitId=4403765, UnitName="mrad", Description="milliradian" }
            };

            //Act
            

            //Assert
            var ex = Assert.Throws<Exception>(() => _mockCsvImportService.ImportCsv(_filePath, _delimiter));
            StringAssert.Contains($"File is not valid! Column 'UNECECode' must be present.", ex.Message.ToString());
        }
    }
}