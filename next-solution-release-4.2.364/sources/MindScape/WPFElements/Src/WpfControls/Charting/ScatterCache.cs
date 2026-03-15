using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mindscape.WpfElements.Charting
{
  internal class ScatterCache
  {
    private double _minX;
    private double _maxX;
    private double _minY;
    private double _maxY;

    private double _xSize;
    private double _ySize;

    private int _count;

    private const int _cellCount = 23;

    private readonly Dictionary<Point, List<object>> _buckets = new Dictionary<Point, List<object>>();

    private readonly ScatterSeries _series;
    private ScatterCache _subCache;
    private Point _topLeft;
    private Point _bottomRight;

    internal ScatterCache(double minX, double maxX, double minY, double maxY, ScatterSeries series)
    {
      _minX = minX;
      _maxX = maxX;
      _minY = minY;
      _maxY = maxY;

      _xSize = (_maxX - _minX) / _cellCount;
      _ySize = (_maxY - _minY) / _cellCount;

      _series = series;
    }

    internal void Add(object obj, Point position)
    {
      IList bucket = GetBucketForPoint(position);
      bucket.Add(obj);
      _count++;
    }

    internal IEnumerable GetPointsToRender(double minX, double maxX, double minY, double maxY, bool allowsDataSampling)
    {
      IList<List<object>> buckets = GetBucketsInViewport(minX, maxX, minY, maxY);

      Point topLeft = GetBucketPoint(new Point(minX, maxY));
      Point bottomRight = GetBucketPoint(new Point(maxX, minY));
      int potentialRows = (int)((topLeft.Y + _ySize - bottomRight.Y) / _ySize);
      int potentialColumns = (int)((bottomRight.X + _xSize - topLeft.X) / _xSize);

      /*
#if DEBUG
      double x = topLeft.X;
      double y = bottomRight.Y;
      if (_series.XAxis != null)
      {
        for (int i = 0; i < potentialColumns; i++)
        {
          double physicalX = _series.XAxis.ConvertLogicalToPhysical(x);
          Border line = new Border() { Width = 1, Height = 10000, Background = Brushes.Red };
          Canvas.SetLeft(line, physicalX);
          _series.Canvas.Children.Add(line);
          x += _xSize;
        }
      }
      if (_series.YAxis != null)
      {
        for (int i = 0; i < potentialRows; i++)
        {
          double physicalY = _series.YAxis.ConvertLogicalToPhysical(y);
          Border line = new Border() { Width = 10000, Height = 1, Background = Brushes.Red };
          Canvas.SetTop(line, _series.Canvas.ActualHeight - physicalY);
          _series.Canvas.Children.Add(line);
          y += _ySize;
        }
      }
#endif
      */

      if (potentialColumns * potentialRows <= 9 && _count > 500)
      {
        IAxisValueConverter xConverter = _series.XAxis == null ? null : _series.XAxis.ValueConverter;
        IAxisValueConverter yConverter = _series.YAxis == null ? null : _series.YAxis.ValueConverter;
        bool hasConverter = xConverter != null || yConverter != null;
        bool hasBinding = _series.XBinding != null || _series.YBinding != null;

        if (_subCache == null || !_topLeft.Equals(topLeft) || !_bottomRight.Equals(bottomRight))
        {
          _subCache = new ScatterCache(topLeft.X, bottomRight.X + _xSize, bottomRight.Y, topLeft.Y + _ySize, _series);
          foreach (List<object> bucket in buckets)
          {
            foreach (object dataPoint in bucket)
            {
              Point point = (!hasBinding && !hasConverter && dataPoint is Point) ? (Point)dataPoint : _series.GetPoint(dataPoint, 0); // TODO: not sure what to do about the index parameter here. In general, for scatter series this shouldn't be a problem though.
              _subCache.Add(dataPoint, point);
            }
          }
          _topLeft = topLeft;
          _bottomRight = bottomRight;
        }
        foreach (object o in _subCache.GetPointsToRender(minX, maxX, minY, maxY, allowsDataSampling))
        {
          yield return o;
        }
      }
      else if (buckets.Count > 0)
      {
        int totalToRender = 500;
        int itemsPerBucket = Math.Max(totalToRender / buckets.Count, 1);
        foreach (IList bucket in buckets)
        {
          int step = bucket.Count / itemsPerBucket;
          if (step == 0)
          {
            step = 1;
          }
          step = AdjustIndexStep(step);
          if (!allowsDataSampling)
          {
            step = 1;
          }
          for (int i = 0; i < bucket.Count; i += step)
          {
            yield return bucket[i];
          }
        }
      }
    }

    private int AdjustIndexStep(int indexStep)
    {
      double step = indexStep;
      int count = 0;
      while (step > 1)
      {
        step /= 2.0;
        count++;
      }
      return (int)Math.Pow(2, count);
    }

    private IList<List<object>> GetBucketsInViewport(double minX, double maxX, double minY, double maxY)
    {
      IList<List<object>> result = new List<List<object>>();
      foreach (Point bucketPoint in _buckets.Keys)
      {
        if (IsBucketInViewport(bucketPoint, minX, maxX, minY, maxY))
        {
          result.Add(_buckets[bucketPoint]);
        }
      }
      return result;
    }

    private bool IsBucketInViewport(Point bucketPoint, double minX, double maxX, double minY, double maxY)
    {
      double left = bucketPoint.X;
      double right = left + _xSize;
      double bottom = bucketPoint.Y;
      double top = bottom + _ySize;
      return left <= maxX && right >= minX && top >= minY && bottom <= maxY;
    }

    private IList GetBucketForPoint(Point point)
    {
      Point bucketPoint = GetBucketPoint(point);
      List<object> list;
      _buckets.TryGetValue(bucketPoint, out list);
      if (list == null)
      {
        list = new List<object>();
        _buckets[bucketPoint] = list;
      }
      return list;
    }

    private Point GetBucketPoint(Point point)
    {
      double x = GetBucketX(point.X);
      double y = GetBucketY(point.Y);
      return new Point(x, y);
    }

    private double GetBucketX(double x)
    {
      if (x < _minX)
      {
        return _minX;
      }
      if (x > _maxX)
      {
        return _minX + (_xSize * (_cellCount - 1));
      }
      return _minX + (_xSize * ((int)((x - _minX) / _xSize)));
    }

    private double GetBucketY(double y)
    {
      if (y < _minY)
      {
        return _minY;
      }
      if (y > _maxY)
      {
        return _minY + (_ySize * (_cellCount - 1));
      }
      return _minY + (_ySize * ((int)((y - _minY) / _ySize)));
    }
  }
}
