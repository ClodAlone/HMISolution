using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests.Model
{
  [TestFixture]
  public class SelectedGridItemChangedEventArgsTests
  {
    [Test]
    public void SimpleProperties()
    {
      PropertyGridRow o = new PropertyGridRow(new PropertyNode("Fie", typeof(string).GetProperty("Length"), null));
      PropertyGridRow n = new PropertyGridRow(new PropertyNode("Fie", typeof(string).GetProperty("Length"), null));
      SelectedGridItemChangedEventArgs e = new SelectedGridItemChangedEventArgs(o, n);

      Assert.AreEqual(o, e.OldSelection);
      Assert.AreEqual(n, e.NewSelection);
    }
  }
}
