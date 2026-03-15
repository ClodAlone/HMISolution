using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  public static class RecurrenceAssert
  {
    public static void OnlyTheseDaysOccurInPattern(IRecurrencePattern pattern, DateTime testStart, DateTime testEnd, DateTime patternStart, int maxOccurrences, IList<DateTime> expected)
    {
      if (testEnd < testStart)
      {
        Assert.Fail("The start time of the testing should come before the end time of the testing");
      }
      DateTime current = new DateTime(testStart.Year, testStart.Month, testStart.Day);
      while (!IsSameDay(current, testEnd))
      {
        bool expecting = CheckIncludedDateAndRemove(expected, current);
        if (expecting)
        {
          Assert.IsTrue(pattern.Includes(patternStart, current, maxOccurrences, DayOfWeek.Monday), "Pattern should have matched on " + current.ToString("dd/MM/yy") + " but did not");
        }
        else
        {
          Assert.IsFalse(pattern.Includes(patternStart, current, maxOccurrences, DayOfWeek.Monday), "Pattern should not have matched on " + current.ToString("dd/MM/yy") + " but did match");
        }
        current = current.AddDays(1);
      }
      Assert.AreEqual(expected.Count, 0, "A date that was expected to match the pattern did not do so: " + GetDateList(expected));
      // If the expected list is not empty, then they were never tested against the pattern.
      // In this case, the end date of this test wasn't large enough.
    }

    public static void OnlyTheseDaysOccurInPattern(IRecurrencePattern pattern, DateTime testStart, DateTime testEnd, DateTime patternStart, DateTime patternEnd, IList<DateTime> expected)
    {
      if (testEnd < testStart)
      {
        Assert.Fail("The start time of the testing should come before the end time of the testing");
      }
      DateTime current = new DateTime(testStart.Year, testStart.Month, testStart.Day);
      while (!IsSameDay(current, testEnd))
      {
        bool expecting = CheckIncludedDateAndRemove(expected, current);
        if (expecting)
        {
          Assert.IsTrue(pattern.Includes(patternStart, current, patternEnd, DayOfWeek.Monday), "Pattern should have matched on " + current.ToString("dd/MM/yy") + " but did not");
        }
        else
        {
          Assert.IsFalse(pattern.Includes(patternStart, current, patternEnd, DayOfWeek.Monday), "Pattern should not have matched on " + current.ToString("dd/MM/yy") + " but did match");
        }
        current = current.AddDays(1);
      }
      Assert.AreEqual(expected.Count, 0, "A date that was expected to match the pattern did not do so: " + GetDateList(expected));
      // If the expected list is not empty, then they were never tested against the pattern.
      // In this case, the end date of this test wasn't large enough.
    }

    private static bool CheckIncludedDateAndRemove(IList<DateTime> dates, DateTime date)
    {
      foreach (DateTime time in dates)
      {
        if (IsSameDay(time, date))
        {
          dates.Remove(time);
          return true;
        }
      }
      return false;
    }

    public static bool IsSameDay(DateTime time1, DateTime time2)
    {
      return time1.Date == time2.Date;
    }

    private static string GetDateList(IEnumerable<DateTime> dates)
    {
      string text = String.Join(", ", dates.Take(3).Select(d => d.ToString("dd/MM/yy")).ToArray());
      if (dates.Count() > 3)
      {
        text += String.Format("... ({0} total)", dates.Count());
      }
      return text;
    }
  }
}
