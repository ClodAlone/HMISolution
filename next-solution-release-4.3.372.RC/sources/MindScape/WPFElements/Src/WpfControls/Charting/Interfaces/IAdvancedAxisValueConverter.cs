using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.Charting
{
  internal interface IAdvancedAxisValueConverter
  {
    double GetNextAxisPlotPosition(double axisMinimum, double axisMaximum, double currentPlotPosition);

    double GetPreviousAxisPlotPosition(double axisMinimum, double axisMaximum, double currentPlotPosition);

    double NormalizeAxisPlotPosition(double axisMinimuum, double axisMaximum, double currentPlotPosition);

    // TODO: include a way to provide different formats based on the available label space.
    string GetFormat(double axisMinimum, double axisMaximum);
  }
}
