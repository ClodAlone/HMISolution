using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Linq;
#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Contains helper methods for serializing recurrence patterns to XML.
  /// </summary>
  public static class RecurrencePatternXmlSerializer
  {
    /// <summary>
    /// Gets an XElement containing the details of the recurrence pattern.
    /// </summary>
    /// <param name="pattern">The recurrence pattern to serialize.</param>
    /// <returns>An XElement representing the recurrence pattern.</returns>
    public static XElement ToXml(this IRecurrencePattern pattern)
    {
      if (pattern is EveryNDaysRecurrencePattern)
        return ToXml((EveryNDaysRecurrencePattern)pattern);
      if (pattern is EveryWeekdayRecurrencePattern)
        return ToXml((EveryWeekdayRecurrencePattern)pattern);
      if (pattern is WeeklyRecurrencePattern)
        return ToXml((WeeklyRecurrencePattern)pattern);
      if (pattern is NthDayOfMonthRecurrencePattern)
        return ToXml((NthDayOfMonthRecurrencePattern)pattern);
      if (pattern is MonthlyPatternRecurrencePattern)
        return ToXml((MonthlyPatternRecurrencePattern)pattern);
      if (pattern is SpecificDateYearlyRecurrencePattern)
        return ToXml((SpecificDateYearlyRecurrencePattern)pattern);
      if (pattern is YearlyPatternRecurrencePattern)
        return ToXml((YearlyPatternRecurrencePattern)pattern);

      throw new ArgumentException("pattern");
    }

    private static XElement ToXml(EveryNDaysRecurrencePattern pattern)
    {
      return new XElement("EveryNDays",
        new XAttribute("DailyInterval", pattern.DailyInterval));
    }

    private static XElement ToXml(EveryWeekdayRecurrencePattern pattern)
    {
      return new XElement("EveryWeekday");
    }

    private static XElement ToXml(WeeklyRecurrencePattern pattern)
    {
      return new XElement("Weekly",
        new XAttribute("WeeklyInterval", pattern.WeeklyInterval),
        new XAttribute("DaysOfWeek", pattern.DaysOfWeek.Aggregate(String.Empty, (s, d) => s + "," + d.ToString()).Trim(',')));
    }

    private static XElement ToXml(NthDayOfMonthRecurrencePattern pattern)
    {
      return new XElement("NthDayOfMonth",
        new XAttribute("MonthlyInterval", pattern.MonthlyInterval),
        new XAttribute("DayOfMonth", pattern.DayOfMonth));
    }

    private static XElement ToXml(MonthlyPatternRecurrencePattern pattern)
    {
      return new XElement("MonthlyPattern",
        new XAttribute("MonthlyInterval", pattern.MonthlyInterval),
        new XAttribute("Occurrence", pattern.Occurrence),
        new XAttribute("DayOfRecurrence", pattern.DayOfRecurrence));
    }

    private static XElement ToXml(SpecificDateYearlyRecurrencePattern pattern)
    {
      return new XElement("SpecificDate",
        new XAttribute("YearlyInterval", pattern.YearlyInterval),
        new XAttribute("Month", pattern.Month),
        new XAttribute("DayOfMonth", pattern.DayOfMonth));
    }

    private static XElement ToXml(YearlyPatternRecurrencePattern pattern)
    {
      return new XElement("YearlyPattern",
        new XAttribute("YearlyInterval", pattern.YearlyInterval),
        new XAttribute("Month", pattern.Month),
        new XAttribute("Occurrence", pattern.Occurrence),
        new XAttribute("DayOfRecurrence", pattern.DayOfRecurrence));
    }

    /// <summary>
    /// Parses a recurrence pattern from XML.
    /// </summary>
    /// <param name="xml">An XElement containing details of the recurrence pattern,
    /// as produced by the ToXml method.</param>
    /// <returns>An <see cref="IRecurrencePattern"/>.</returns>
    public static IRecurrencePattern ParseRecurrencePattern(this XElement xml)
    {
      switch (xml.Name.LocalName)
      {
        case "EveryNDays": return ParseEveryNDays(xml);
        case "EveryWeekday": return ParseEveryWeekday(xml);
        case "Weekly": return ParseWeekly(xml);
        case "NthDayOfMonth": return ParseNthDayOfMonth(xml);
        case "MonthlyPattern": return ParseMonthlyPattern(xml);
        case "SpecificDate": return ParseSpecificDate(xml);
        case "YearlyPattern": return ParseYearlyPattern(xml);
      }

      throw new ArgumentException("xml");
    }

    private static IRecurrencePattern ParseEveryNDays(XElement xml)
    {
      return new EveryNDaysRecurrencePattern((int)xml.Attribute("DailyInterval"));
    }

    private static IRecurrencePattern ParseEveryWeekday(XElement xml)
    {
      return new EveryWeekdayRecurrencePattern();
    }

    private static IRecurrencePattern ParseWeekly(XElement xml)
    {
      return new WeeklyRecurrencePattern((int)xml.Attribute("WeeklyInterval"), ((string)xml.Attribute("DaysOfWeek")).Split(',').Select(s => s.As<DayOfWeek>()).ToArray());
    }

    private static IRecurrencePattern ParseNthDayOfMonth(XElement xml)
    {
      return new NthDayOfMonthRecurrencePattern((int)xml.Attribute("MonthlyInterval"), (int)xml.Attribute("DayOfMonth"));
    }

    private static IRecurrencePattern ParseMonthlyPattern(XElement xml)
    {
      return new MonthlyPatternRecurrencePattern((int)xml.Attribute("MonthlyInterval"), xml.Attribute("Occurrence").As<Occurrence>(), xml.Attribute("DayOfRecurrence").As<DayOfRecurrence>());
    }

    private static IRecurrencePattern ParseSpecificDate(XElement xml)
    {
      return new SpecificDateYearlyRecurrencePattern((int)xml.Attribute("YearlyInterval"), (int)xml.Attribute("Month"), (int)xml.Attribute("DayOfMonth"));
    }

    private static IRecurrencePattern ParseYearlyPattern(XElement xml)
    {
      return new YearlyPatternRecurrencePattern((int)xml.Attribute("YearlyInterval"), (int)xml.Attribute("Month"), xml.Attribute("Occurrence").As<Occurrence>(), xml.Attribute("DayOfRecurrence").As<DayOfRecurrence>());
    }

    private static T As<T>(this string text)
    {
      return (T)Enum.Parse(typeof(T), text, true);
    }

    private static T As<T>(this XAttribute attribute)
    {
      return (T)Enum.Parse(typeof(T), (string)attribute, true);
    }
  }
}
