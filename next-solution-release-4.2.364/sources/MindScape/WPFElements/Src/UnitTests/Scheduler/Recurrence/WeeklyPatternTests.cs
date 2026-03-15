using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class WeeklyPatternTests
  {
    private static readonly DateTime Jan1_2010 = new DateTime(2010, 1, 1);
    private static readonly DateTime Apr1_2010 = new DateTime(2010, 4, 1);
    private static readonly DateTime May1_2010 = new DateTime(2010, 5, 1);

    [Test]
    public void EverySunday_1_Occurrence()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Sunday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 21)); // Sunday 21st Feb

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Apr1_2010, expected[0], 1, expected);
    }

    [Test]
    public void EveryMonday_3_Occurrences()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Monday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 15)); // Monday 15th Feb
      expected.Add(new DateTime(2010, 2, 22)); // Monday 22nd Feb
      expected.Add(new DateTime(2010, 3, 1)); // Monday 1st March

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Apr1_2010, expected[0], 3, expected);
    }

    [Test]
    public void EverySecondMonday_3_Occurrences()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(2, DayOfWeek.Monday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 15)); // Monday 15th Feb
      // Skipped Monday 22nd Feb
      expected.Add(new DateTime(2010, 3, 1));  // Monday 1st March
      // Skipped Monday 8th March
      expected.Add(new DateTime(2010, 3, 15)); // Monday 15th March

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Apr1_2010, expected[0], 3, expected);
    }

    [Test]
    public void EveryMondayAndWednesday_3_Occurrences()
    {
      // This is mainly to test for the right number of occurences when there are more than one DayOfWeek in the pattern.
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Monday, DayOfWeek.Wednesday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 15)); // Monday 15th Feb
      expected.Add(new DateTime(2010, 2, 17)); // Wednesday 17th Feb
      expected.Add(new DateTime(2010, 2, 22)); // Monday 22nd Feb

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Apr1_2010, expected[0], 3, expected);
    }

    [Test]
    public void EveryTuesdayAndSaturday_3_Occurrences()
    {
      // This is mostly to test the case when monday is not used.
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Tuesday, DayOfWeek.Saturday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 16)); // Tuesday 16th Feb
      expected.Add(new DateTime(2010, 2, 20)); // Saturday 20th Feb
      expected.Add(new DateTime(2010, 2, 23)); // Tuesday 23rd Feb

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Apr1_2010, expected[0], 3, expected);
    }

    [Test]
    public void EveryMondayAndSunday_3_Occurrences()
    {
      // Testing the case when both the first and last days of the week is used
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Monday, DayOfWeek.Sunday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 15)); // Monday 15th Feb
      expected.Add(new DateTime(2010, 2, 21)); // Sunday 21st Feb
      expected.Add(new DateTime(2010, 2, 22)); // Monday 22nd Feb

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Apr1_2010, expected[0], 3, expected);
    }

    [Test]
    public void TuesdayAndFridayButStartOnWednesday_3_Occurrences()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Tuesday, DayOfWeek.Friday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 19)); // Tuesday 19th Feb
      expected.Add(new DateTime(2010, 2, 23)); // Friday 23rd Feb
      expected.Add(new DateTime(2010, 2, 26)); // Tuesday 26th Feb

      // The pattern is set to use Tuesday and Friday of each week.
      // Here we set the pattern to start on a Wednesday, so it should skip the Tuesday of the first week.
      DateTime patternStartDate = new DateTime(2010, 2, 17);

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Apr1_2010, patternStartDate, 3, expected);
    }

    [Test]
    public void MoreDaysThanOccurrences()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 15)); // Monday 15th Feb
      expected.Add(new DateTime(2010, 2, 16)); // Tuesday 16th Feb
      expected.Add(new DateTime(2010, 2, 18)); // Thursday 18th Feb

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Apr1_2010, expected[0], 3, expected);
    }

    [Test]
    public void MoreDaysThanOccurrencesAndStartLate()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 18)); // Thursday 18th Feb
      expected.Add(new DateTime(2010, 2, 20)); // Saturday 20th Feb
      expected.Add(new DateTime(2010, 2, 22)); // Monday 22nd Feb

      DateTime patternStartDate = new DateTime(2010, 2, 17);

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Apr1_2010, patternStartDate, 3, expected);
    }

    [Test]
    public void SameNumberOfDaysAsOccurrences()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Thursday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 15)); // Monday 15th Feb
      expected.Add(new DateTime(2010, 2, 16)); // Tuesday 16th Feb
      expected.Add(new DateTime(2010, 2, 18)); // Thursday 18th Feb

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Apr1_2010, expected[0], 3, expected);
    }

    [Test]
    public void SameNumberOfDaysAsOccurrencesAndStartLate()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Thursday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 18)); // Thursday 18th Feb
      expected.Add(new DateTime(2010, 2, 22)); // Monday 22nd Feb
      expected.Add(new DateTime(2010, 2, 23)); // Tuesday 23rd Feb

      DateTime patternStartDate = new DateTime(2010, 2, 17);

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Apr1_2010, patternStartDate, 3, expected);
    }

    [Test]
    public void ManyOccurrences()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Sunday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 16)); // Tuesday 16th Feb
      expected.Add(new DateTime(2010, 2, 18)); // Thursday 18th Feb
      expected.Add(new DateTime(2010, 2, 21)); // Sunday 21st Feb
      expected.Add(new DateTime(2010, 2, 23)); // Tuesday 23rd Feb
      expected.Add(new DateTime(2010, 2, 25)); // Thursday 25th Feb
      expected.Add(new DateTime(2010, 2, 28)); // Sunday 28th Feb
      expected.Add(new DateTime(2010, 3, 2)); // Tuesday 2nd March
      expected.Add(new DateTime(2010, 3, 4)); // Thursday 4th March
      expected.Add(new DateTime(2010, 3, 7)); // Sunday 7th March
      expected.Add(new DateTime(2010, 3, 9)); // Tuesday 9th March

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Apr1_2010, expected[0], 10, expected);
    }

    [Test]
    public void OccurAllWeek()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
                                                                       DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 15)); // Monday 15th Feb
      expected.Add(new DateTime(2010, 2, 16)); // Tuesday 16th Feb
      expected.Add(new DateTime(2010, 2, 17)); // Wednesday 17th Feb
      expected.Add(new DateTime(2010, 2, 18)); // Thursday 18th Feb
      expected.Add(new DateTime(2010, 2, 19)); // Friday 19th Feb
      expected.Add(new DateTime(2010, 2, 20)); // Saturday 20th Feb
      expected.Add(new DateTime(2010, 2, 21)); // Sunday 21st Feb

      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, Apr1_2010, expected[0], 7, expected);
    }

    [Test]
    public void SkipMultipleWeeksIfStartLate()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(3, DayOfWeek.Monday);

      // The pattern is set to occur on Monday every 3 weeks.
      // We start the pattern on a wednesday, so the pattern should skip ahead 3 weeks.
      // This is the behavior seen in MS Outlook.

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 2, 22)); // Monday 22nd Feb
      expected.Add(new DateTime(2010, 3, 15)); // Monday 15th March
      expected.Add(new DateTime(2010, 4, 5)); // Monday 5th April

      DateTime patternStart = new DateTime(2010, 2, 3); // Wednesday 3rd Feb
      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, May1_2010, patternStart, 3, expected);
    }

    [Test]
    public void StillIncludeDaysFarInTheFuture()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Monday);

      Assert.IsTrue(pattern.Includes(Jan1_2010, new DateTime(2110, 4, 7), DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Jan1_2010, new DateTime(2110, 12, 22), DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Jan1_2010, new DateTime(2112, 5, 30), DayOfWeek.Monday));

      // Not on Monday:
      Assert.IsFalse(pattern.Includes(Jan1_2010, new DateTime(2112, 5, 13), DayOfWeek.Monday));
    }

    [Test]
    public void EveryWednesdayAndEndDateIsFriday()
    {
      // Here we are testing the Includes(DateTime, DateTime, DateTime) method.
      // The end date occurs on a DayOfWeek later than the one the pattern uses.
      // That is, end date is a Friday, and the pattern uses Wednesday.
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Wednesday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 3, 10)); // Wednesday 10th March
      expected.Add(new DateTime(2010, 3, 17)); // Wednesday 17th March
      expected.Add(new DateTime(2010, 3, 24)); // Wednesday 24th March

      DateTime patternStart = new DateTime(2010, 3, 8); // Monday 8th March
      DateTime patternEnd = new DateTime(2010, 3, 26); // Friday 26th March
      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, May1_2010, patternStart, patternEnd, expected);
    }

    [Test]
    public void EveryWednesdayAndEndDateIsMonday()
    {
      // Here we are testing the Includes(DateTime, DateTime, DateTime) method.
      // The end date occurs on a DayOfWeek earlier than the one the pattern uses.
      // That is, end date is a Monday, and the pattern uses Wednesday.
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Wednesday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 3, 10)); // Wednesday 10th March
      expected.Add(new DateTime(2010, 3, 17)); // Wednesday 17th March

      DateTime patternStart = new DateTime(2010, 3, 8); // Monday 8th March
      DateTime patternEnd = new DateTime(2010, 3, 22); // Friday 26th March
      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, May1_2010, patternStart, patternEnd, expected);
    }

    [Test]
    public void EveryWednesdayAndEndDateIsWednesday()
    {
      // Here we are testing the Includes(DateTime, DateTime, DateTime) method.
      // The end date occurs on the same DayOfWeek as the one the pattern uses.
      // That is, end date is a Wednesday, and the pattern uses Wednesday.
      // So this tests that the pattern includes the end date if it is valid.
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(1, DayOfWeek.Wednesday);

      List<DateTime> expected = new List<DateTime>();
      expected.Add(new DateTime(2010, 3, 10)); // Wednesday 10th March
      expected.Add(new DateTime(2010, 3, 17)); // Wednesday 17th March
      expected.Add(new DateTime(2010, 3, 24)); // Wednesday 24th March

      DateTime patternStart = new DateTime(2010, 3, 8); // Monday 8th March
      DateTime patternEnd = new DateTime(2010, 3, 24); // Wednesday 26th March
      RecurrenceAssert.OnlyTheseDaysOccurInPattern(pattern, Jan1_2010, May1_2010, patternStart, patternEnd, expected);
    }
  }
}
