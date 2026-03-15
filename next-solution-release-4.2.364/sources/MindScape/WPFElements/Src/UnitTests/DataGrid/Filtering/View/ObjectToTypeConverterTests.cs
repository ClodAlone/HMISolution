using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using Mindscape.WpfElements.WpfDataGrid;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class ObjectToTypeConverterTests
  {
    private ObjectToTypeConverter _converter;

    [SetUp]
    public void SetUp()
    {
      _converter = new ObjectToTypeConverter();
    }

    [Test]
    public void Convert_Null()
    {
      Assert.IsNull(_converter.Convert(null, typeof(Type), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void Convert()
    {
      Type type = _converter.Convert(DateTime.Now, typeof(Type), null, CultureInfo.CurrentUICulture) as Type;
      Assert.AreEqual(typeof(DateTime), type);

      type = _converter.Convert(3.5, typeof(Type), null, CultureInfo.CurrentUICulture) as Type;
      Assert.AreEqual(typeof(double), type);

      type = _converter.Convert(new Person(), typeof(Type), null, CultureInfo.CurrentUICulture) as Type;
      Assert.AreEqual(typeof(Person), type);
    }

    [Test]
    public void ConvertBack_Null()
    {
      Assert.IsNull(_converter.ConvertBack(null, typeof(object), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void ConvertBack()
    {
      Person person = _converter.ConvertBack(typeof(Person), typeof(object), null, CultureInfo.CurrentUICulture) as Person;
      Assert.IsNotNull(person);
    }
  }
}
