using System;
using NUnit.Framework;
using System.ComponentModel;
using System.Reflection;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class ReflectionUtilitiesTests
  {
    public class ForBrowsabilityTesting
    {
      public int UnattributedProperty { get; set; }

      [Browsable(true)]
      public int BrowsableAttributedProperty { get; set; }

      [Browsable(false)]
      public int NonBrowsableAttributedProperty { get; set; }
    }
    [Test]
    public void IsBrowsable()
    {
      Type t = typeof(ForBrowsabilityTesting);
      PropertyInfo p;

      p = t.GetProperty("UnattributedProperty");
      Assert.IsTrue(ReflectionUtilities.IsBrowsable(p));

      p = t.GetProperty("BrowsableAttributedProperty");
      Assert.IsTrue(ReflectionUtilities.IsBrowsable(p));

      p = t.GetProperty("NonBrowsableAttributedProperty");
      Assert.IsFalse(ReflectionUtilities.IsBrowsable(p));
    }
  }
}
