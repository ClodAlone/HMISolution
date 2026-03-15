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
using System.Collections.Generic;

namespace Mindscape.WpfElements.Charting
{
  internal static class StackedSeriesUtils
  {
    internal static void GetDataPointDimensions(DataSeries series, Dictionary<double, double> previousCache, Dictionary<double, double> cache, Point point, out double dependentLow, out double dependentHigh)
    {
      if (previousCache != null)
      {
        double dependentValue;
        previousCache.TryGetValue(point.X, out dependentValue);
        point.Y += dependentValue;
      }
      if (cache != null)
      {
        cache[point.X] = point.Y;
      }
      dependentLow = point.Y;
      dependentHigh = point.Y;
    }

    internal static void AnalyseStackedSeries(PolarSeries series, Dictionary<double, double> previousCache, Dictionary<double, double> cache)
    {
      if (series.ItemsSource != null && series.ItemsSource.Count > 0 && ((series.ThetaAxis != null) || (series.RhoAxis != null)))
      {
        IAxisValueConverter thetaConverter = series.ThetaAxis.ValueConverter;
        IAxisValueConverter rhoConverter = series.RhoAxis.ValueConverter;
        bool hasConverter = thetaConverter != null || rhoConverter != null;
        double minTheta = Double.MaxValue;
        double maxTheta = Double.MinValue;
        double minRho = Double.MaxValue;
        double maxRho = Double.MinValue;
        double thetaSpacing = 0;
        double previousTheta = 0;

        int count = 0;
        foreach (object o in series.ItemsSource)
        {
          //CartesianDataPoint dataPoint = new CartesianDataPoint();
          //dataPoint.DataContext = o;
          //Point point = series.GetPoint(dataPoint, series.ReverseAxes);
          PolarPoint point = hasConverter ? series.GetPoint(o, thetaConverter, rhoConverter) : series.GetPoint(o);
          if (series.ReverseAxes)
          {
            point = new PolarPoint(point.Theta, point.Rho);
          }
          /*if (series.ReverseAxes)
          {
            if (previousCache != null)
            {
              double dependentValue;
              previousCache.TryGetValue(point.Y, out dependentValue);
              point.X += dependentValue;
            }
            if (cache != null)
            {
              cache[point.Y] = point.X;
            }
          }*/
          //else
          {
            if (previousCache != null)
            {
              double dependentValue;
              previousCache.TryGetValue(point.Theta, out dependentValue);
              point.Rho += dependentValue;
            }
            if (cache != null)
            {
              cache[point.Theta] = point.Rho;
            }
          }

          minTheta = Math.Min(point.Theta, minTheta);
          maxTheta = Math.Max(point.Theta, maxTheta);
          //minY = Math.Min(point.Y, minY);
          //maxY = Math.Max(point.Y, maxY);
          if (point.Rho < minRho)
          {
            minRho = point.Rho;
          }
          if (point.Rho > maxRho)
          {
            maxRho = point.Rho;
          }
          count++;

          thetaSpacing = point.Theta - previousTheta;
          previousTheta = point.Theta;
        }
        PolarAxisBase dependentAxis = series.RhoAxis;// series.ReverseAxes ? series.ThetaAxis : series.RhoAxis;
        PolarAxisBase independentAxis = series.ThetaAxis;// series.ReverseAxes ? series.RhoAxis : series.ThetaAxis;

        if (series.Series[0] == series)
        {
          if (independentAxis != null && independentAxis.IsAuto)
          {
            independentAxis.Minimum = Math.Round(minTheta);
            independentAxis.Maximum = Math.Round(maxTheta + thetaSpacing);
          }
          if (dependentAxis != null && dependentAxis.IsAuto)
          {
            if (minRho >= 0)
            {
              minRho++;
            }
            dependentAxis.Minimum = Math.Min(0, Math.Floor(minRho) - 1);
            dependentAxis.Maximum = Math.Round(maxRho) + 1;
          }
        }
        else
        {
          if (independentAxis != null && independentAxis.IsAuto)
          {
            independentAxis.Minimum = Math.Min(independentAxis.Minimum, Math.Round(minTheta));
            independentAxis.Maximum = Math.Max(independentAxis.Maximum, Math.Round(maxTheta + thetaSpacing));
          }
          if (dependentAxis != null && dependentAxis.IsAuto)
          {
            if (minRho >= 0)
            {
              minRho++;
            }
            dependentAxis.Minimum = Math.Min(dependentAxis.Minimum, Math.Min(0, Math.Floor(minRho) - 1));
            dependentAxis.Maximum = Math.Max(dependentAxis.Maximum, Math.Round(maxRho) + 1);
          }
        }
      }
    }
  }
}
