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

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Specifies the cause of a <see cref="ScheduleItem"/> being created.
  /// </summary>
  public enum ScheduleItemCreationType
  {
    /// <summary>
    /// The user has input some text to trigger the creation of a <see cref="ScheduleItem"/>.
    /// </summary>
    TextInput,

    /// <summary>
    /// The user has double clicked on a day or time slot to trigger the creation of a <see cref="ScheduleItem"/>.
    /// </summary>
    DoubleClick,

    /// <summary>
    /// The user has hovered the mouse over a day or time slot and then clicked the create-here button that appeared.
    /// </summary>
    CreateHereButton,

    /// <summary>
    /// The user has used the add-item button found in the navigation tool bar.
    /// </summary>
    AddItemToolBarButton
  }
}
