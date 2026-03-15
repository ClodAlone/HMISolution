using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class XmlSerialisationTests
  {
    [Test]
    public void EveryNDays()
    {
      EveryNDaysRecurrencePattern pattern = new EveryNDaysRecurrencePattern(123);
      EveryNDaysRecurrencePattern reloaded = pattern.ToXml().ParseRecurrencePattern() as EveryNDaysRecurrencePattern;

      Assert.AreEqual(pattern.DailyInterval, reloaded.DailyInterval);
    }

    [Test]
    public void EveryWeekday()
    {
      EveryWeekdayRecurrencePattern pattern = new EveryWeekdayRecurrencePattern();

      Assert.IsInstanceOf<EveryWeekdayRecurrencePattern>(pattern.ToXml().ParseRecurrencePattern());
    }

    [Test]
    public void Weekly_SingleDay()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(3, DayOfWeek.Monday);
      WeeklyRecurrencePattern reloaded = pattern.ToXml().ParseRecurrencePattern() as WeeklyRecurrencePattern;

      Assert.AreEqual(pattern.WeeklyInterval, reloaded.WeeklyInterval);
      Assert.AreEqual(pattern.DaysOfWeek.Count, reloaded.DaysOfWeek.Count);
      Assert.IsTrue(reloaded.DaysOfWeek.Contains(DayOfWeek.Monday));
    }

    [Test]
    public void Weekly_MultipleDays()
    {
      WeeklyRecurrencePattern pattern = new WeeklyRecurrencePattern(3, DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Sunday);
      WeeklyRecurrencePattern reloaded = pattern.ToXml().ParseRecurrencePattern() as WeeklyRecurrencePattern;

      Assert.AreEqual(pattern.WeeklyInterval, reloaded.WeeklyInterval);
      Assert.AreEqual(pattern.DaysOfWeek.Count, reloaded.DaysOfWeek.Count);
      Assert.IsTrue(reloaded.DaysOfWeek.Contains(DayOfWeek.Monday));
      Assert.IsTrue(reloaded.DaysOfWeek.Contains(DayOfWeek.Thursday));
      Assert.IsTrue(reloaded.DaysOfWeek.Contains(DayOfWeek.Sunday));
    }

    [Test]
    public void NthDayOfMonth()
    {
      NthDayOfMonthRecurrencePattern pattern = new NthDayOfMonthRecurrencePattern(2, 17);
      NthDayOfMonthRecurrencePattern reloaded = pattern.ToXml().ParseRecurrencePattern() as NthDayOfMonthRecurrencePattern;

      Assert.AreEqual(pattern.MonthlyInterval, reloaded.MonthlyInterval);
      Assert.AreEqual(pattern.DayOfMonth, reloaded.DayOfMonth);
    }

    [Test]
    public void MonthlyPattern()
    {
      MonthlyPatternRecurrencePattern pattern = new MonthlyPatternRecurrencePattern(5, Occurrence.Last, DayOfRecurrence.WeekendDay);
      MonthlyPatternRecurrencePattern reloaded = pattern.ToXml().ParseRecurrencePattern() as MonthlyPatternRecurrencePattern;

      Assert.AreEqual(pattern.MonthlyInterval, reloaded.MonthlyInterval);
      Assert.AreEqual(pattern.Occurrence, reloaded.Occurrence);
      Assert.AreEqual(pattern.DayOfRecurrence, reloaded.DayOfRecurrence);
    }

    [Test]
    public void SpecificDateYearly()
    {
      SpecificDateYearlyRecurrencePattern pattern = new SpecificDateYearlyRecurrencePattern(4, 10, 28);
      SpecificDateYearlyRecurrencePattern reloaded = pattern.ToXml().ParseRecurrencePattern() as SpecificDateYearlyRecurrencePattern;

      Assert.AreEqual(pattern.YearlyInterval, reloaded.YearlyInterval);
      Assert.AreEqual(pattern.Month, reloaded.Month);
      Assert.AreEqual(pattern.DayOfMonth, reloaded.DayOfMonth);
    }

    [Test]
    public void YearlyPattern()
    {
      YearlyPatternRecurrencePattern pattern = new YearlyPatternRecurrencePattern(100, 1, Occurrence.First, DayOfRecurrence.Wednesday);
      YearlyPatternRecurrencePattern reloaded = pattern.ToXml().ParseRecurrencePattern() as YearlyPatternRecurrencePattern;

      Assert.AreEqual(pattern.YearlyInterval, reloaded.YearlyInterval);
      Assert.AreEqual(pattern.Month, reloaded.Month);
      Assert.AreEqual(pattern.Occurrence, reloaded.Occurrence);
      Assert.AreEqual(pattern.DayOfRecurrence, reloaded.DayOfRecurrence);
    }
  }
}
