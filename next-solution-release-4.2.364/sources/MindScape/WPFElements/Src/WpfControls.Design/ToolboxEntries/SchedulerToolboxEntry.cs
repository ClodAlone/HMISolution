using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry SchedulerToolboxEntry = new ToolboxEntry(

      typeof(Scheduler),
      "A control which displays a schedule using a set of calendar views",

      new DependencyProperty[] {
        Scheduler.FormatterProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Appearance", new DependencyProperty[] {
          Scheduler.MonthViewStyleProperty,
          Scheduler.DayViewStyleProperty,
          Scheduler.WeekViewStyleProperty,
          Scheduler.ToolBarContentProperty,
        }),
      }

      );
  }
}
