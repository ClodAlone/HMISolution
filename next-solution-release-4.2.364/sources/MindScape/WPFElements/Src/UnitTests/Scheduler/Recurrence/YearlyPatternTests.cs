using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class YearlyPatternTests
  {
    private static readonly DateTime Jan1_2010 = new DateTime(2010, 1, 1);
    private static readonly DateTime Feb1_2010 = new DateTime(2010, 2, 1);

    #region EveryDateOfYear tests

    [Test]
    public void EveryDateOfYear_IncludedIfInRange()
    {
      IRecurrencePattern pattern = new SpecificDateYearlyRecurrencePattern(1, 3, 9);

      Assert.IsTrue(pattern.Includes(Jan1_2010, new DateTime(2010, 3, 9), 4, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Jan1_2010, new DateTime(2011, 3, 9), 4, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Jan1_2010, new DateTime(2012, 3, 9), 4, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Jan1_2010, new DateTime(2013, 3, 9), 4, DayOfWeek.Monday));
    }

    [Test]
    public void EveryDateOfYear_ExcludeIfWrongMonth()
    {
      IRecurrencePattern pattern = new SpecificDateYearlyRecurrencePattern(1, 3, 9);

      Assert.IsFalse(pattern.Includes(Jan1_2010, new DateTime(2010, 4, 9), 4, DayOfWeek.Monday));
    }

    [Test]
    public void EveryDateOfYear_ExcludeIfWrongDayOfMonth()
    {
      IRecurrencePattern pattern = new SpecificDateYearlyRecurrencePattern(1, 3, 9);

      Assert.IsFalse(pattern.Includes(Jan1_2010, new DateTime(2010, 3, 10), 4, DayOfWeek.Monday));
    }

    [Test]
    public void EveryDateOfYear_ExcludeIfOutOfRange()
    {
      IRecurrencePattern pattern = new SpecificDateYearlyRecurrencePattern(1, 3, 9);

      //Too early:
      Assert.IsFalse(pattern.Includes(Jan1_2010, new DateTime(2009, 3, 9), 4, DayOfWeek.Monday));
      //Too late:
      Assert.IsFalse(pattern.Includes(Jan1_2010, new DateTime(2014, 3, 9), 4, DayOfWeek.Monday));
    }

    [Test]
    public void EveryDateOfYear_ExcludeIfSkippedYear()
    {
      IRecurrencePattern pattern = new SpecificDateYearlyRecurrencePattern(2, 3, 9);

      Assert.IsTrue(pattern.Includes(Jan1_2010, new DateTime(2010, 3, 9), 4, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Jan1_2010, new DateTime(2011, 3, 9), 4, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Jan1_2010, new DateTime(2012, 3, 9), 4, DayOfWeek.Monday));
    }

    [Test]
    public void EveryDateOfYear_SkipFirstYearIfStartingLate()
    {
      IRecurrencePattern pattern = new SpecificDateYearlyRecurrencePattern(1, 3, 9);

      // The pattern is set to use March 9th of every year.
      // We start the pattern on April 1st.
      // So the pattern should skip March 9th on the year that the pattern starts on.
      Assert.IsFalse(pattern.Includes(new DateTime(2010, 4, 1), new DateTime(2010, 3, 9), 4, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(new DateTime(2010, 4, 1), new DateTime(2011, 3, 9), 4, DayOfWeek.Monday));
    }

    [Test]
    public void EveryDateOfYear_UseLastDayIfDayOfMonthIsTooBig()
    {
      // The UI should never let the user input this kind of situation (30th of Feb)
      // But its still best to test it.
      IRecurrencePattern pattern = new SpecificDateYearlyRecurrencePattern(1, 2, 30);

      Assert.IsTrue(pattern.Includes(Jan1_2010, new DateTime(2010, 2, 28), 4, DayOfWeek.Monday));
    }

    #endregion // EveryDateOfYear tests

    #region EveryOccurrenceOfDateOfYear tests

    [Test]
    public void EveryOccurrenceOfDateOfYear_FirstDayOfApril()
    {
      IRecurrencePattern pattern = new YearlyPatternRecurrencePattern(1, 4, Occurrence.First, DayOfRecurrence.Day);

      Assert.IsTrue(pattern.Includes(Jan1_2010, new DateTime(2010, 4, 1), 2, DayOfWeek.Monday));
    }

    [Test]
    public void EveryOccurrenceOfDateOfYear_SkipFirstYearIfStartingLate()
    {
      IRecurrencePattern pattern = new YearlyPatternRecurrencePattern(1, 4, Occurrence.First, DayOfRecurrence.Day);

      // Even though the pattern start date is in 2010, the first valid date will
      // be in 2011 because the pattern start month comes after the 1st of april.
      Assert.IsFalse(pattern.Includes(new DateTime(2010, 6, 1), new DateTime(2010, 4, 1), 2, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(new DateTime(2010, 6, 1), new DateTime(2011, 4, 1), 2, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(new DateTime(2010, 6, 1), new DateTime(2012, 4, 1), 2, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(new DateTime(2010, 6, 1), new DateTime(2013, 4, 1), 2, DayOfWeek.Monday));
    }

    [Test]
    public void EveryOccurrenceOfDateOfYear_WorksIfStartOnSameMonth()
    {
      IRecurrencePattern pattern = new YearlyPatternRecurrencePattern(1, 4, Occurrence.Fourth, DayOfRecurrence.Day);

      // The pattern is set to use the 4th of April

      // Start the pattern on the 1st of April: (So the 4th of April 2010 will be hit)
      Assert.IsTrue(pattern.Includes(new DateTime(2010, 4, 1), new DateTime(2010, 4, 4), 2, DayOfWeek.Monday));

      // Start the pattern on the 6th of April: (So the 4th of April 2011 will be hit)
      Assert.IsFalse(pattern.Includes(new DateTime(2010, 4, 6), new DateTime(2010, 4, 4), 2, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(new DateTime(2010, 4, 6), new DateTime(2011, 4, 4), 2, DayOfWeek.Monday));

      // Start the pattern on the 4th of April: (So the pattern start date should be hit)
      Assert.IsTrue(pattern.Includes(new DateTime(2010, 4, 4), new DateTime(2010, 4, 4), 2, DayOfWeek.Monday));
    }

    [Test]
    public void EveryOccurrenceOfDateOfYear_LargerYearlyInterval()
    {
      IRecurrencePattern pattern = new YearlyPatternRecurrencePattern(2, 4, Occurrence.First, DayOfRecurrence.Day);

      Assert.IsTrue(pattern.Includes(Jan1_2010, new DateTime(2010, 4, 1), 2, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Jan1_2010, new DateTime(2011, 4, 1), 2, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Jan1_2010, new DateTime(2012, 4, 1), 2, DayOfWeek.Monday));
    }

    [Test]
    public void EveryOccurrenceOfDateOfYear_SkipMultipleYearsIfStartsLate()
    {
      // This test is similar to the EveryOccurrenceOfMonth_SkipMultipleMonthsIfStartsLate in MonthlyTests.
      IRecurrencePattern pattern = new YearlyPatternRecurrencePattern(3, 1, Occurrence.First, DayOfRecurrence.Day);

      // The pattern is set to use the 1st of January every 3 years.
      // But we start the pattern on a Febuary, so the pattern should skip ahead 3 years, not just 1.

      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2010, 1, 1), 2, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2011, 1, 1), 2, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2012, 1, 1), 2, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2013, 1, 1), 2, DayOfWeek.Monday));
    }

    #endregion // EveryOccurenceOfDateOfYear tests

    #region Infinite reccurrence tests

    [Test]
    public void StillWorksFarInTheFuture()
    {
      IRecurrencePattern pattern = new YearlyPatternRecurrencePattern(1, 4, Occurrence.First, DayOfRecurrence.Day);

      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2110, 4, 1), DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2111, 4, 1), DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2112, 4, 1), DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2113, 4, 2), DayOfWeek.Monday));
    }

    #endregion // Infinite reccurrence tests

    #region Stop by end date tests

    [Test]
    public void StopByEndDate()
    {
      IRecurrencePattern pattern = new YearlyPatternRecurrencePattern(2, 4, Occurrence.First, DayOfRecurrence.Day);

      // End on a day that is not included in the pattern
      DateTime patternEnd = new DateTime(2013, 4, 1);
      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2010, 4, 1), patternEnd, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2011, 4, 1), patternEnd, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2012, 4, 1), patternEnd, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2013, 4, 1), patternEnd, DayOfWeek.Monday));

      // End on a day that is included in the pattern
      patternEnd = new DateTime(2012, 4, 1);
      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2010, 4, 1), patternEnd, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2011, 4, 1), patternEnd, DayOfWeek.Monday));
      Assert.IsTrue(pattern.Includes(Feb1_2010, new DateTime(2012, 4, 1), patternEnd, DayOfWeek.Monday));
      Assert.IsFalse(pattern.Includes(Feb1_2010, new DateTime(2013, 4, 1), patternEnd, DayOfWeek.Monday));
    }

    #endregion // Stop by end date tests
  }
}
