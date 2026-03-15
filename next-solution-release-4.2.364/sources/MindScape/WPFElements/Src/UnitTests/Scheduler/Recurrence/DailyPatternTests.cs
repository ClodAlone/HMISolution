using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class DailyPatternTests
  {
    private static DateTime _testStart, _testEnd;
    private static readonly IRecurrencePattern _everyDay = new EveryNDaysRecurrencePattern(1);
    private static readonly IRecurrencePattern _every2Days = new EveryNDaysRecurrencePattern(2);
    private static readonly IRecurrencePattern _every3Days = new EveryNDaysRecurrencePattern(3);
    private static readonly IRecurrencePattern _every7Days = new EveryNDaysRecurrencePattern(7);
    private static readonly IRecurrencePattern _everyWeekday = new EveryWeekdayRecurrencePattern();

    public DailyPatternTests()
    {
      _testStart = new DateTime(2009, 1, 1);
      _testEnd = new DateTime(2009, 12, 1);
    }

    #region EverySoManyDays tests

    [Test]
    public void EveryOneDayAndOneOccurrence()
    {
      List<DateTime> expected = new List<DateTime>
      {
        new DateTime(2009, 8, 5)
      };

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(_everyDay, _testStart, _testEnd, new DateTime(2009, 8, 5), 1, expected);
    }

    [Test]
    public void EveryThreeDaysAndOneOccurrence()
    {
      List<DateTime> expected = new List<DateTime>
      {
        new DateTime(2009, 8, 5)
      };

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(_every3Days, _testStart, _testEnd, new DateTime(2009, 8, 5), 1, expected);
    }

    [Test]
    public void EveryOneDayAndNineOccurrences()
    {
      List<DateTime> expected = new List<DateTime>
      {
        new DateTime(2009, 7, 29),
        new DateTime(2009, 7, 30),
        new DateTime(2009, 7, 31),
        new DateTime(2009, 8, 1),
        new DateTime(2009, 8, 2),
        new DateTime(2009, 8, 3),
        new DateTime(2009, 8, 4),
        new DateTime(2009, 8, 5),
        new DateTime(2009, 8, 6),
      };

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(_everyDay, _testStart, _testEnd, new DateTime(2009, 7, 29), 9, expected);
    }

    [Test]
    public void EverySevenDaysAndElevenOccurrences()
    {
      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2009, 7, 29));
      expected.Add(new DateTime(2009, 8, 5));
      expected.Add(new DateTime(2009, 8, 12));
      expected.Add(new DateTime(2009, 8, 19));
      expected.Add(new DateTime(2009, 8, 26));
      expected.Add(new DateTime(2009, 9, 2));
      expected.Add(new DateTime(2009, 9, 9));
      expected.Add(new DateTime(2009, 9, 16));
      expected.Add(new DateTime(2009, 9, 23));
      expected.Add(new DateTime(2009, 9, 30));
      expected.Add(new DateTime(2009, 10, 7));

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(_every7Days, _testStart, _testEnd, new DateTime(2009, 7, 29), 11, expected);
    }

    #endregion

    #region EveryWeekday tests

    [Test]
    public void EveryWeekDayAndOneOccurrence()
    {
      List<DateTime> expected = new List<DateTime>
      {
        new DateTime(2009, 8, 5)
      };

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(_everyWeekday, _testStart, _testEnd, new DateTime(2009, 8, 5), 1, expected);
    }

    [Test]
    public void EveryWeekDayAndOneOccurrenceStartingOnSaturday()
    {
      List<DateTime> expected = new List<DateTime>
      {
        new DateTime(2009, 8, 3)
      };

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(_everyWeekday, _testStart, _testEnd, new DateTime(2009, 8, 1), 1, expected);
    }

    [Test]
    public void EveryWeekDayAndOneOccurrenceStartingOnSunday()
    {
      List<DateTime> expected = new List<DateTime>
      {
        new DateTime(2009, 8, 3)
      };

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(_everyWeekday, _testStart, _testEnd, new DateTime(2009, 8, 2), 1, expected);
    }

    [Test]
    public void EveryWeekDayAndFiveOccurrences()
    {
      List<DateTime> expected = new List<DateTime>
      {
        new DateTime(2009, 8, 3),
        new DateTime(2009, 8, 4),
        new DateTime(2009, 8, 5),
        new DateTime(2009, 8, 6),
        new DateTime(2009, 8, 7),
      };

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(_everyWeekday, _testStart, _testEnd, new DateTime(2009, 8, 3), 5, expected);
    }

    [Test]
    public void EveryWeekDayAndFiveOccurrencesStartingOnSaturday()
    {
      List<DateTime> expected = new List<DateTime>
      {
        new DateTime(2009, 8, 3),
        new DateTime(2009, 8, 4),
        new DateTime(2009, 8, 5),
        new DateTime(2009, 8, 6),
        new DateTime(2009, 8, 7),
      };

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(_everyWeekday, _testStart, _testEnd, new DateTime(2009, 8, 1), 5, expected);
    }

    [Test]
    public void EveryWeekDayAndFiveOccurrencesStartingOnSunday()
    {
      List<DateTime> expected = new List<DateTime>
      {
        new DateTime(2009, 8, 3),
        new DateTime(2009, 8, 4),
        new DateTime(2009, 8, 5),
        new DateTime(2009, 8, 6),
        new DateTime(2009, 8, 7),
      };

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(_everyWeekday, _testStart, _testEnd, new DateTime(2009, 8, 2), 5, expected);
    }

    [Test]
    public void EveryWeekDayAndFiveOccurrencesOverWeekend()
    {
      List<DateTime> expected = new List<DateTime>
      {
        new DateTime(2009, 7, 29),
        new DateTime(2009, 7, 30),
        new DateTime(2009, 7, 31),
        new DateTime(2009, 8, 3),
        new DateTime(2009, 8, 4),
      };

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(_everyWeekday, _testStart, _testEnd, new DateTime(2009, 7, 29), 5, expected);
    }

    [Test]
    public void EveryWeekDayAndSeventeenOccurrences()
    {
      List<DateTime> expected = new List<DateTime>
      {
        new DateTime(2009, 7, 29),
        new DateTime(2009, 7, 30),
        new DateTime(2009, 7, 31),
        new DateTime(2009, 8, 3),
        new DateTime(2009, 8, 4),
        new DateTime(2009, 8, 5),
        new DateTime(2009, 8, 6),
        new DateTime(2009, 8, 7),
        new DateTime(2009, 8, 10),
        new DateTime(2009, 8, 11),
        new DateTime(2009, 8, 12),
        new DateTime(2009, 8, 13),
        new DateTime(2009, 8, 14),
        new DateTime(2009, 8, 17),
        new DateTime(2009, 8, 18),
        new DateTime(2009, 8, 19),
        new DateTime(2009, 8, 20),
      };

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(_everyWeekday, _testStart, _testEnd, new DateTime(2009, 7, 29), 17, expected);
    }

    #endregion

    #region Infinite recurrence tests

    [Test]
    public void StillWorksFarInTheFuture()
    {
      DateTime patternStart = new DateTime(2110, 4, 8);
      Assert.IsTrue(_every2Days.Includes(patternStart, new DateTime(2110, 4, 8), DayOfWeek.Monday));
      Assert.IsFalse(_every2Days.Includes(patternStart, new DateTime(2110, 4, 9), DayOfWeek.Monday));
      Assert.IsTrue(_every2Days.Includes(patternStart, new DateTime(2110, 4, 10), DayOfWeek.Monday));
      Assert.IsFalse(_every2Days.Includes(patternStart, new DateTime(2110, 4, 11), DayOfWeek.Monday));
    }

    #endregion // Infinite recurrence tests

    #region Stop by end date recurrence tests

    [Test]
    public void StopByEndDate()
    {
      DateTime patternStart = new DateTime(2010, 4, 8);

      // Set the end date to be a day that the pattern can NOT include:
      Assert.IsTrue(_every2Days.Includes(patternStart, new DateTime(2010, 4, 8), new DateTime(2010, 4, 11), DayOfWeek.Monday));
      Assert.IsFalse(_every2Days.Includes(patternStart, new DateTime(2010, 4, 9), new DateTime(2010, 4, 11), DayOfWeek.Monday));
      Assert.IsTrue(_every2Days.Includes(patternStart, new DateTime(2010, 4, 10), new DateTime(2010, 4, 11), DayOfWeek.Monday));
      Assert.IsFalse(_every2Days.Includes(patternStart, new DateTime(2010, 4, 11), new DateTime(2010, 4, 11), DayOfWeek.Monday));

      // Set the end date to be a day that the pattern CAN include:
      Assert.IsTrue(_every2Days.Includes(patternStart, new DateTime(2010, 4, 8), new DateTime(2010, 4, 10), DayOfWeek.Monday));
      Assert.IsFalse(_every2Days.Includes(patternStart, new DateTime(2010, 4, 9), new DateTime(2010, 4, 10), DayOfWeek.Monday));
      Assert.IsTrue(_every2Days.Includes(patternStart, new DateTime(2010, 4, 10), new DateTime(2010, 4, 10), DayOfWeek.Monday));
      Assert.IsFalse(_every2Days.Includes(patternStart, new DateTime(2010, 4, 11), new DateTime(2010, 4, 10), DayOfWeek.Monday));
    }

    #endregion // Stop by end date recurrence tests
  }
}
