using NUnit.Framework;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests.Utils
{
  [TestFixture]
  public class StringUtilsTests
  {
    [Test]
    public void Humanise()
    {
      Assert.AreEqual("Date", StringUtils.Humanize("Date"));
      Assert.AreEqual("Date Of Birth", StringUtils.Humanize("DateOfBirth"));
      Assert.AreEqual("date_of_birth", StringUtils.Humanize("date_of_birth"));
    }
  }
}
