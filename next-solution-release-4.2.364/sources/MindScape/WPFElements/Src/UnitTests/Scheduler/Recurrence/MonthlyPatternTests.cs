using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class MonthlyPatternTests
  {
    private static readonly DateTime Jan1_2010 = new DateTime(2010, 1, 1);
    private static readonly DateTime Feb1_2010 = new DateTime(2010, 2, 1);
    private static readonly DateTime Feb2_2010 = new DateTime(2010, 2, 2);
    private static readonly DateTime Dec1_2010 = new DateTime(2010, 12, 1);

    private static readonly DateTime Apr1_2010 = new DateTime(2010, 4, 1);
    private static readonly DateTime Nov1_2010 = new DateTime(2010, 11, 1);
    private static readonly DateTime Jun1_2010 = new DateTime(2010, 6, 1);

    #region EveryDateOfMonth tests

    [Test]
    public void FixedDayOfMonth_IncludedIfInRange()
    {
      NthDayOfMonthRecurrencePattern pattern = new NthDayOfMonthRecurrencePattern(1, 1);

      Assert.IsTrue(pattern.Includes(Jan1_2010, Feb1_2010, 4, DayOfWeek.Monday));
    }

    [Test]
    public void FixedDayOfMonth_ExcludedIfOutOfRange()
    {
      NthDayOfMonthRecurrencePattern pattern = new NthDayOfMonthRecurrencePattern(1, 1);

      Assert.IsFalse(pattern.Includes(Jan1_2010, Dec1_2010, 4, DayOfWeek.Monday));
    }

    [Test]
    public void FixedDayOfMonth_ExcludedIfWrongDay()
    {
      NthDayOfMonthRecurrencePattern pattern = new NthDayOfMonthRecurrencePattern(1, 1);

      Assert.IsFalse(pattern.Includes(Jan1_2010, Feb2_2010, 4, DayOfWeek.Monday));
    }

    [Test]
    public void FixedDayOfMonth_ExcludedIfSkippedMonth()
    {
      NthDayOfMonthRecurrencePattern pattern = new NthDayOfMonthRecurrencePattern(1, 2);

      Assert.IsFalse(pattern.Includes(Jan1_2010, Feb1_2010, 4, DayOfWeek.Monday));
    }

    [Test]
    public void FixedDayOfMonth_UseLastDayOfMonthIfNotEnoughDays()
    {
      NthDayOfMonthRecurrencePattern pattern = new NthDayOfMonthRecurrencePattern(1, 30);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 1, 30));
      expected.Add(new DateTime(2010, 2, 28)); // Feb has less than 30 days, so we expect the last day of Feb to be used instead
      expected.Add(new DateTime(2010, 3, 30));

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Dec1_2010, Jan1_2010, 3, expected);
    }

    #endregion // EveryDateOfMonth tests

    #region EveryOccurenceOfMonth tests

    #region Nth day of month tests

    [Test]
    public void TheFirstDayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.First, DayOfRecurrence.Day);

      List<DateTime> expected = new List<DateTime>();
      // The first day of Feb, March and April:
      expected.Add(new DateTime(2010, 2, 1));
      expected.Add(new DateTime(2010, 3, 1));
      expected.Add(new DateTime(2010, 4, 1));

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Jun1_2010, expected[0], 3, expected);
    }

    [Test]
    public void TheSecondDayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Second, DayOfRecurrence.Day);

      List<DateTime> expected = new List<DateTime>();
      // The second day of Feb, March and April:
      expected.Add(new DateTime(2010, 2, 2));
      expected.Add(new DateTime(2010, 3, 2));
      expected.Add(new DateTime(2010, 4, 2));

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Jun1_2010, expected[0], 3, expected);
    }

    [Test]
    public void TheThirdDayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Third, DayOfRecurrence.Day);

      List<DateTime> expected = new List<DateTime>();
      // The third day of Feb, March and April:
      expected.Add(new DateTime(2010, 2, 3));
      expected.Add(new DateTime(2010, 3, 3));
      expected.Add(new DateTime(2010, 4, 3));

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Jun1_2010, expected[0], 3, expected);
    }

    [Test]
    public void TheFourthDayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Fourth, DayOfRecurrence.Day);

      List<DateTime> expected = new List<DateTime>();
      // The fourth day of Feb, March and April:
      expected.Add(new DateTime(2010, 2, 4));
      expected.Add(new DateTime(2010, 3, 4));
      expected.Add(new DateTime(2010, 4, 4));

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Jun1_2010, expected[0], 3, expected);
    }

    [Test]
    public void TheLastDayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Last, DayOfRecurrence.Day);

      List<DateTime> expected = new List<DateTime>();
      // The last day of Feb, March and April:
      expected.Add(new DateTime(2010, 2, 28));
      expected.Add(new DateTime(2010, 3, 31));
      expected.Add(new DateTime(2010, 4, 30));

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Jun1_2010, expected[0], 3, expected);
    }

    #endregion // Nth day of month tests

    #region Nth weekday of month tests

    [Test]
    public void TheFirstWeekdayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.First, DayOfRecurrence.Weekday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 3)); // May 2010 starts with Saturday
      expected.Add(new DateTime(2010, 6, 1)); // June 2010 starts with Tuesday
      expected.Add(new DateTime(2010, 7, 1)); // July 2010 starts with Thursday
      expected.Add(new DateTime(2010, 8, 2)); // August 2010 starts with Sunday

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 4, expected);
    }

    [Test]
    public void TheSecondWeekdayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Second, DayOfRecurrence.Weekday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 4)); // May 2010 starts with Saturday
      expected.Add(new DateTime(2010, 6, 2)); // June 2010 starts with Tuesday
      expected.Add(new DateTime(2010, 7, 2)); // July 2010 starts with Thursday
      expected.Add(new DateTime(2010, 8, 3)); // August 2010 starts with Sunday

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 4, expected);
    }

    [Test]
    public void TheThirdWeekdayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Third, DayOfRecurrence.Weekday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 5)); // May 2010 starts with Saturday
      expected.Add(new DateTime(2010, 6, 3)); // June 2010 starts with Tuesday
      expected.Add(new DateTime(2010, 7, 5)); // July 2010 starts with Thursday
      expected.Add(new DateTime(2010, 8, 4)); // August 2010 starts with Sunday

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 4, expected);
    }

    [Test]
    public void TheFourthWeekdayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Fourth, DayOfRecurrence.Weekday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 6)); // May 2010 starts with Saturday
      expected.Add(new DateTime(2010, 6, 4)); // June 2010 starts with Tuesday
      expected.Add(new DateTime(2010, 7, 6)); // July 2010 starts with Thursday
      expected.Add(new DateTime(2010, 8, 5)); // August 2010 starts with Sunday

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 4, expected);
    }

    [Test]
    public void TheLastWeekdayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Last, DayOfRecurrence.Weekday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 31)); // May 2010 ends with Monday the 31st
      expected.Add(new DateTime(2010, 6, 30)); // June 2010 ends with Wednesday the 30th
      expected.Add(new DateTime(2010, 7, 30)); // July 2010 ends with Saturday the 31st
      expected.Add(new DateTime(2010, 8, 31)); // August 2010 ends with Tuesday the 31st

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 4, expected);
    }

    #endregion // Nth weekday of month tests

    #region Nth weekend day of month tests

    [Test]
    public void TheFirstWeekendDayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.First, DayOfRecurrence.WeekendDay);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 1)); // May 2010 starts with Saturday
      expected.Add(new DateTime(2010, 6, 5)); // June 2010 starts with Tuesday

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 2, expected);
    }

    [Test]
    public void TheFSecondWeekendDayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Second, DayOfRecurrence.WeekendDay);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 2)); // May 2010 starts with Saturday
      expected.Add(new DateTime(2010, 6, 6)); // June 2010 starts with Tuesday

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 2, expected);
    }

    [Test]
    public void TheThirdWeekendDayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Third, DayOfRecurrence.WeekendDay);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 8)); // The 8th is the 3rd weekend day of May 2010
      expected.Add(new DateTime(2010, 6, 12)); // The 12th is the 3rd weekend day of June 2010

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 2, expected);
    }

    [Test]
    public void TheFourthWeekendDayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Fourth, DayOfRecurrence.WeekendDay);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 9)); // The 9th is the 4th weekend day of May 2010
      expected.Add(new DateTime(2010, 6, 13)); // The 13th is the 4th weekend day of June 2010

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 2, expected);
    }

    [Test]
    public void TheLastWeekendDayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Last, DayOfRecurrence.WeekendDay);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 30)); // May 2010 ends with Monday the 31st
      expected.Add(new DateTime(2010, 6, 27)); // June 2010 ends with Wednesday the 30th

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 2, expected);
    }

    #endregion // Nth weekend day of month tests

    #region Nth Monday of month tests

    [Test]
    public void TheFirstMondayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.First, DayOfRecurrence.Monday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 3)); // May 2010 starts with Saturday
      expected.Add(new DateTime(2010, 6, 7)); // June 2010 starts with Tuesday

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 2, expected);
    }

    [Test]
    public void TheSecondMondayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Second, DayOfRecurrence.Monday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 10)); // May 2010 starts with Saturday
      expected.Add(new DateTime(2010, 6, 14)); // June 2010 starts with Tuesday

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 2, expected);
    }

    [Test]
    public void TheThirdMondayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Third, DayOfRecurrence.Monday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 17)); // May 2010 starts with Saturday
      expected.Add(new DateTime(2010, 6, 21)); // June 2010 starts with Tuesday

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 2, expected);
    }

    [Test]
    public void TheFourthMondayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Fourth, DayOfRecurrence.Monday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 24)); // May 2010 starts with Saturday
      expected.Add(new DateTime(2010, 6, 28)); // June 2010 starts with Tuesday

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 2, expected);
    }

    [Test]
    public void TheLastMondayOfEveryMonth()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.Last, DayOfRecurrence.Monday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 31)); // May 2010 ends with Monday the 31st
      expected.Add(new DateTime(2010, 6, 28)); // The last Monday of June is the 4th Monday of June

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Apr1_2010, Nov1_2010, expected[0], 2, expected);
    }

    #endregion // Nth Monday of month tests

    #region MonthlyInterval tests

    [Test]
    public void SkipToNextMonthIfStartOfPatternComesAfterDayOfReccurence()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(1, Occurrence.First, DayOfRecurrence.Day);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 1));
      // Here we set the pattern start date to be the 2nd of January.
      // Since the pattern is set to occur on the first of each month, we expect the pattern to skip january
      // entirely, and take the 1st of Feburary to be the first date in the pattern.
      DateTime patternStartDate = new DateTime(2010, 1, 2);

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Jun1_2010, patternStartDate, 1, expected);
    }

    [Test]
    public void EveryOccurrenceOfMonth_SkipMultipleMonthsIfStartsLate()
    {
      // This is simalar to the SkipToNextMonthIfStartOfPatternComesAfterDayOfReccurence test, but
      // has a Monthly interval greater than 1.

      // This test is due to a bug that used to be in the monthly pattern.
      // According to MS Outlook, say we have a pattern that has an interval of 3 months.
      // And this pattern looks for the 2nd day of each month.
      // Suppose we start the pattern on the 7th of some month.
      // Then the pattern should skip ahead 3 months as apposed to 1.
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(3, Occurrence.Second, DayOfRecurrence.Day);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 5, 2));
      expected.Add(new DateTime(2010, 8, 2));

      DateTime patternStart = new DateTime(2010, 2, 7);
      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Nov1_2010, patternStart, 2, expected);
    }

    [Test]
    public void EveryOccurrenceOfMonth_ExludeIfSkippedMonth()
    {
      //This tests when the MonthlyInterval is an even number.
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(2, Occurrence.Second, DayOfRecurrence.Day);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 2));
      expected.Add(new DateTime(2010, 4, 2));
      expected.Add(new DateTime(2010, 6, 2));

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Nov1_2010, expected[0], 3, expected);
    }

    [Test]
    public void EveryOccurrenceOfMonth_ExludeIfSkipped2Months()
    {
      //This tests when the MonthlyInterval is an odd number.
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(3, Occurrence.Second, DayOfRecurrence.Day);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 2));
      expected.Add(new DateTime(2010, 5, 2));
      expected.Add(new DateTime(2010, 8, 2));

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Nov1_2010, expected[0], 3, expected);
    }

    #endregion // MonthlyInterval tests

    #endregion // EveryOccurenceOfMonth tests

    #region Infinite reccurrence tests

    [Test]
    public void StillWorksFarInTheFuture()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(2, Occurrence.Second, DayOfRecurrence.Day);

      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2110, 2, 2), DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2110, 3, 2), DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2110, 4, 2), DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2110, 4, 3), DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2110, 5, 2), DayOfWeek.Monday));
    }

    #endregion // Infinite reccurrence tests

    #region Stop by end date tests

    [Test]
    public void StopByEndDate()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(2, Occurrence.Second, DayOfRecurrence.Day);

      // End the pattern on a day that is not included:
      DateTime patternEnd = new DateTime(2010, 5, 2);
      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2010, 2, 2), patternEnd, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2010, 3, 2), patternEnd, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2010, 4, 2), patternEnd, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2010, 5, 2), patternEnd, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2010, 6, 2), patternEnd, DayOfWeek.Monday));

      // End the pattern on a day that is included:
      patternEnd = new DateTime(2010, 4, 2);
      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2010, 2, 2), patternEnd, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2010, 3, 2), patternEnd, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2010, 4, 2), patternEnd, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2010, 5, 2), patternEnd, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2010, 6, 2), patternEnd, DayOfWeek.Monday));
    }

    #endregion // Stop by end date tests
  }
}
