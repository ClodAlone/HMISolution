using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;
using Mindscape.WpfElements.UnitTests;

namespace Mindscape.WpfElements.UnitTests.DataGridTests
{
  [TestFixture]
  public class DataGridExporterTests
  {
    private IList<TestClass1> _staticData;
    private DataGrid _dataGrid;

    public DataGridExporterTests()
    {
      _staticData = new List<TestClass1>();
      for (int i = 0; i < 2; i++)
      {
        _staticData.Add(new TestClass1() { Property1 = i, Property2 = i.ToString() });
      }
    }

    [SetUp]
    public void SetUp()
    {
      _dataGrid = new DataGrid();
      _dataGrid.ItemsSource = _staticData;
    }

    [Test]
    [STAThread]
    public void ExportStringOutputIsValid()
    {
      string shouldBe =
        "\"Property1\"," + "\"Property2\"" + "\r\n" +
        "\"" + "0" + "\"," + "\"" + "0" + "\"\r\n" + "\"" + "1" + "\"," + "\"" + "1" + "\"\r\n";

      string exportResult = DataGridExporter.Csv.WriteString(_dataGrid);
      Assert.AreEqual(shouldBe, exportResult);
    }
  }
}
