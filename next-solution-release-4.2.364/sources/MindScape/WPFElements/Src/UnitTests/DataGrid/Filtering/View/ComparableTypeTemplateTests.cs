using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Mindscape.WpfElements.WpfDataGrid;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class ComparableTypeTemplateTests
  {
    private ComparableTypeTemplate _ctt;

    [SetUp]
    public void SetUp()
    {
      _ctt = new ComparableTypeTemplate();
    }

    [Test]
    public void ComparableType_DefaultValue()
    {
      Assert.IsNull(_ctt.ComparableType);
    }

    [Test]
    public void ComparableTypeProperty()
    {
      _ctt.ComparableType = typeof(string);
      Assert.AreEqual(typeof(string), _ctt.ComparableType);

      _ctt.ComparableType = typeof(double);
      Assert.AreEqual(typeof(double), _ctt.ComparableType);

      _ctt.ComparableType = null;
      Assert.IsNull(_ctt.ComparableType);
    }

    [Test]
    public void Template_DefaultValue()
    {
      Assert.IsNull(_ctt.Template);
    }

    [Test]
    public void TemplateProperty()
    {
      DataTemplate template = new DataTemplate();
      _ctt.Template = template;
      Assert.AreSame(template, _ctt.Template);
    }

    [Test]
    public void Matches_String()
    {
      _ctt.ComparableType = typeof(string);
      ComparableFilterDescription description = new ComparableFilterDescription(typeof(string));
      Assert.IsTrue(_ctt.Matches(description));
    }

    [Test]
    public void Matches_Doubleg()
    {
      _ctt.ComparableType = typeof(double);
      ComparableFilterDescription description = new ComparableFilterDescription(typeof(double));
      Assert.IsTrue(_ctt.Matches(description));
    }

    [Test]
    public void Matches_DateTime()
    {
      _ctt.ComparableType = typeof(DateTime);
      ComparableFilterDescription description = new ComparableFilterDescription(typeof(DateTime));
      Assert.IsTrue(_ctt.Matches(description));
    }

    [Test]
    public void DoesNotMatchDifferentType()
    {
      _ctt.ComparableType = typeof(string);
      ComparableFilterDescription description = new ComparableFilterDescription(typeof(double));
      Assert.IsFalse(_ctt.Matches(description));
    }

    [Test]
    public void DoesNotMatchNull()
    {
      _ctt.ComparableType = typeof(string);
      Assert.IsFalse(_ctt.Matches(null));
    }

    [Test]
    public void DoesNotMatchInvalidObject()
    {
      _ctt.ComparableType = typeof(string);
      Assert.IsFalse(_ctt.Matches(new Point()));
    }

    [Test]
    public void DoesNotMatchIfComparableTypeIsNull()
    {
      _ctt.ComparableType = null;
      ComparableFilterDescription description = new ComparableFilterDescription(typeof(string));
      Assert.IsFalse(_ctt.Matches(description));
    }
  }
}
