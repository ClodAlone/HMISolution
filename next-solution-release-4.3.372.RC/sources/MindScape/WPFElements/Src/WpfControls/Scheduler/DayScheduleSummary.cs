using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using Infralution.Licensing;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for displaying the long schedule items of a single day.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class DayScheduleSummary : DayScheduleBase
  {
    static DayScheduleSummary()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DayScheduleSummary),
        new FrameworkPropertyMetadata(typeof(DayScheduleSummary)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DayScheduleSummary"/> class.
    /// </summary>
    public DayScheduleSummary()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing
    }
  }
}
