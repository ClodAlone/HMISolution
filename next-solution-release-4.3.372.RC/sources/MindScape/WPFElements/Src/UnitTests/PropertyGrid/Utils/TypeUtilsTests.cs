using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using NUnit.Framework;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests.Utils
{
  [TestFixture]
  public class TypeUtilsTests
  {
    private class StringCollection : Collection<string> { }

    [Test]
    public void MatchesGenericCollections()
    {
      Type collectionOfString = typeof(Collection<string>);
      Assert.IsTrue(TypeUtilities.IsGenericCollection(collectionOfString));
      Type listOfDateTime = typeof(List<DateTime>);
      Assert.IsTrue(TypeUtilities.IsGenericCollection(listOfDateTime));
    }

    [Test]
    public void MatchesDerivedClass()
    {
      Assert.IsTrue(TypeUtilities.IsGenericCollection(typeof(StringCollection)));
    }

    [Test]
    public void DoesNotMatchNonGenericCollection()
    {
      Assert.IsFalse(TypeUtilities.IsGenericCollection(typeof(ArrayList)));
      Assert.IsFalse(TypeUtilities.IsGenericCollection(typeof(int[])));
      Type dictionary = typeof(Dictionary<string, object>);
      Assert.IsFalse(TypeUtilities.IsGenericCollection(dictionary));
   }

    [Test]
    public void DoesNotMatchNonCollection()
    {
      Assert.IsFalse(TypeUtilities.IsGenericCollection(typeof(int)));
    }
  }
}
