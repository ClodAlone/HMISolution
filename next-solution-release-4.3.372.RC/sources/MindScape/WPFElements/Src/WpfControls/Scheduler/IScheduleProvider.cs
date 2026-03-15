using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides a <see cref="Schedule"/> object
  /// </summary>
  internal interface IScheduleProvider
  {
    /// <summary>
    /// Gets or sets the <see cref="Schedule"/> that this <see cref="IScheduleProvider"/> provides.
    /// </summary>
    Schedule Schedule { get; }

    DateRange SelectedDateRange { get; }

    SchedulerElement FindSchedulerElement(ScheduleItem item);
  }
}
