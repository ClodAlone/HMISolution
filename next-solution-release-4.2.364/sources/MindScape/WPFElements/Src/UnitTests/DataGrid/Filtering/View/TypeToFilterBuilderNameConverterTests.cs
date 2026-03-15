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
  public class TypeToFilterBuilderNameConverterTests
  {
    private TypeToFilterBuilderNameConverter _converter;

    [SetUp]
    public void SetUp()
    {
      _converter = new TypeToFilterBuilderNameConverter();
    }

    [Test]
    public void Convert_Null()
    {
      Assert.AreEqual("", _converter.Convert(null, typeof(string), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void Convert_UnsupportedObject()
    {
      Assert.AreEqual("", _converter.Convert(new Point(), typeof(string), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void Convert_String()
    {
      Assert.AreEqual("Arbitrary Value", _converter.Convert("Arbitrary Value", typeof(string), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void Convert_NonBuilderType()
    {
      Assert.AreEqual("", _converter.Convert(typeof(Person), typeof(string), null, CultureInfo.CurrentUICulture));
    }

    [Test]
    public void Convert()
    {
      Type type = typeof(StartsWithFilterBuilder);
      string name = new StartsWithFilterBuilder().Name;
      Assert.AreEqual(name, _converter.Convert(type, typeof(string), null, CultureInfo.CurrentUICulture));

      type = typeof(OrFilterBuilder);
      name = new OrFilterBuilder().Name;
      Assert.AreEqual(name, _converter.Convert(type, typeof(string), null, CultureInfo.CurrentUICulture));
    }
  }
}
