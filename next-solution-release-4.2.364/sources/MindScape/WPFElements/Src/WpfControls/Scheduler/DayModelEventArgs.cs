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
namespace Mindscape.SilverlightElements.Internal
#else
namespace Mindscape.WpfElements.Internal
#endif
{
  internal class DayModelEventArgs : EventArgs
  {
    private readonly DayModel _day;

    public DayModelEventArgs(DayModel day)
    {
      _day = day;
    }

    public DayModel Day
    {
      get { return _day; }
    }
  }
}
