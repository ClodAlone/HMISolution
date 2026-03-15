using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using Mindscape.WpfElements.WpfDataGrid;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class ObjectFilterToObjectConverterTests
  {
    private ObjectFilterToObjectConverter _converter;

    [SetUp]
    public void SetUp()
    {
      _converter = new ObjectFilterToObjectConverter();
    }

    [Test]
    public void Convert_Null()
    {
      Assert.IsNull(_converter.Convert(null, typeof(object), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void Convert_UnsupportedObject()
    {
      Assert.IsNull(_converter.Convert(new Point(), typeof(object), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void Convert_EqualsFilter()
    {
      EqualsFilter filter = new EqualsFilter("Abitrary Value");
      Assert.AreEqual("Abitrary Value", _converter.Convert(filter, typeof(object), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void ConvertBack_Null()
    {
      Assert.IsNull(_converter.ConvertBack(null, typeof(EqualsFilter), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void ConvertBack_EmptyString()
    {
      Assert.IsNull(_converter.ConvertBack("", typeof(EqualsFilter), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void ConvertBack()
    {
      EqualsFilter filter = _converter.ConvertBack("Arbitrary Value", typeof(EqualsFilter), null, CultureInfo.CurrentUICulture) as EqualsFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("Arbitrary Value", filter.Value);

      filter = _converter.ConvertBack(42.666, typeof(EqualsFilter), null, CultureInfo.CurrentUICulture) as EqualsFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual(42.666, filter.Value);
    }
  }
}
