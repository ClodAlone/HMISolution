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
  public class SelectableObjectTests
  {
    [Test]
    public void ActualValueProperty()
    {
      Object obj = new Object();
      SelectableObject so = new SelectableObject(obj, "Arbitrary Value");

      Assert.AreSame(obj, so.ActualValue);
    }

    [Test]
    public void ValueProperty()
    {
      SelectableObject so = new SelectableObject(new Point(), "Arbitrary Value");

      Assert.AreEqual("Arbitrary Value", so.Value);
    }

    [Test]
    public void IsSelectedProperty_DefaultValue()
    {
      SelectableObject so = new SelectableObject(new Point(), "Arbitrary Value");

      Assert.IsFalse(so.IsSelected);
    }

    [Test]
    public void IsSelectedProperty()
    {
      SelectableObject so = new SelectableObject(new Point(), "Arbitrary Value");
      so.IsSelected = true;
      Assert.IsTrue(so.IsSelected);
      so.IsSelected = false;
      Assert.IsFalse(so.IsSelected);
    }

    [Test]
    public void IsSelected_RaisesPropertyChanged()
    {
      SelectableObject so = new SelectableObject(new Point(), "Arbitrary Value");
      bool raised = false;
      so.PropertyChanged += (o, e) => { raised = true; };
      Assert.IsFalse(raised);
      so.IsSelected = true;
      Assert.IsTrue(raised);
    }

    [Test]
    public void IsSelected_NotRaisedIfNotChanged()
    {
      SelectableObject so = new SelectableObject(new Point(), "Arbitrary Value");
      bool raised = false;
      so.PropertyChanged += (o, e) => { raised = true; };
      Assert.IsFalse(raised);
      so.IsSelected = false;
      Assert.IsFalse(raised);

      so.IsSelected = true;
      raised = false;
      so.IsSelected = true;
      Assert.IsFalse(raised);
    }
  }
}
