using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry MonthCalendarToolboxEntry = new ToolboxEntry(

      typeof(MonthCalendar),
      "A control for displaying a monthly calendar",

      new DependencyProperty[] {
        MonthCalendar.VisibleMonthProperty,
        MonthCalendar.WeeksToDisplayProperty,
        MonthCalendar.MonthTextProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          MonthCalendar.SelectedDateProperty,
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          MonthCalendar.CultureProperty,
        })
      }

      );
  }
}
