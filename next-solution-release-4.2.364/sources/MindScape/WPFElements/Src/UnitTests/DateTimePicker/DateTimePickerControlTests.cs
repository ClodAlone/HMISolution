using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class DateTimePickerControlTests
  {
    [Test]
    [STAThread]
    public void Instantiate()
    {
      DateTimePicker control1 = new DateTimePicker();
      Assert.IsNotNull(control1);

      DropDownDatePicker control2 = new DropDownDatePicker();
      Assert.IsNotNull(control2);
    }
  }
}
