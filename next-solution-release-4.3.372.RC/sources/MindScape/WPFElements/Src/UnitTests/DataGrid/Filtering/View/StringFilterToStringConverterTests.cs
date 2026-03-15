using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Mindscape.WpfElements.WpfDataGrid;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class StringFilterToStringConverterTests
  {
    private StringFilterToStringConverter _converter;

    [SetUp]
    public void SetUp()
    {
      _converter = new StringFilterToStringConverter();
    }

    [Test]
    public void Convert_Null()
    {
      Assert.AreEqual("", _converter.Convert(null, typeof(string), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void Convert_UnsupportedFilter()
    {
      AndFilter filter = new AndFilter();
      Assert.AreEqual("", _converter.Convert(filter, typeof(string), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void Convert()
    {
      StartsWithFilter filter = new StartsWithFilter("Arbitrary Value", false);
      Assert.AreEqual("Arbitrary Value", _converter.Convert(filter, typeof(String), null, CultureInfo.CurrentUICulture));

      filter = new StartsWithFilter("Arbitrary Value", true);
      Assert.AreEqual("Arbitrary Value", _converter.Convert(filter, typeof(String), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void ConvertBack_null()
    {
      Assert.IsNull(_converter.ConvertBack(null, typeof(StartsWithFilter), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void ConvertBack_EmptyString()
    {
      Assert.IsNull(_converter.ConvertBack("", typeof(StartsWithFilter), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void ConvertBack()
    {
      StartsWithFilter filter = _converter.ConvertBack("Arbitrary Value", typeof(StartsWithFilter), null, CultureInfo.CurrentUICulture) as StartsWithFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("Arbitrary Value", filter.Value);
      Assert.IsFalse(filter.MatchCase);
    }
  }
}
