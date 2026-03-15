using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class MonthCalendarControlTests
  {
    [Test]
    [STAThread]
    public void Commands()
    {
      MonthCalendar control = new MonthCalendar();
      control.SelectedDate = new DateTime(2008, 3, 10);

      CalendarCommands.DayBack.Execute(null, control);
      Assert.AreEqual(new DateTime(2008, 3, 9), control.SelectedDate);

      CalendarCommands.DayForward.Execute(null, control);
      Assert.AreEqual(new DateTime(2008, 3, 10), control.SelectedDate);

      CalendarCommands.MonthBack.Execute(null, control);
      Assert.AreEqual(new DateTime(2008, 2, 10), control.SelectedDate);

      CalendarCommands.MonthForward.Execute(null, control);
      Assert.AreEqual(new DateTime(2008, 3, 10), control.SelectedDate);

      CalendarCommands.WeekBack.Execute(null, control);
      Assert.AreEqual(new DateTime(2008, 3, 3), control.SelectedDate);

      CalendarCommands.WeekForward.Execute(null, control);
      Assert.AreEqual(new DateTime(2008, 3, 10), control.SelectedDate);

      CalendarCommands.YearBack.Execute(null, control);
      Assert.AreEqual(new DateTime(2007, 3, 10), control.SelectedDate);

      CalendarCommands.YearForward.Execute(null, control);
      Assert.AreEqual(new DateTime(2008, 3, 10), control.SelectedDate);

      CalendarCommands.SelectToday.Execute(null, control);
      Assert.AreEqual(DateTime.Now.Date, control.SelectedDate.Date);

    }
  }
}
