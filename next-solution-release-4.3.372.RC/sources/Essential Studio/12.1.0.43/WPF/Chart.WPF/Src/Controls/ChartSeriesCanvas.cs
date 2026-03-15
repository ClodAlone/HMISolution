#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Syncfusion.Windows.Chart
{
  /// <summary>
  /// 
  /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartSeriesCanvas : Canvas
  {
    #region Dependencies properties
    /// <summary>
    /// Using a DependencyProperty as the backing store for Series.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty SeriesProperty = 
      DependencyProperty.Register("Series", typeof(ChartSeries), typeof(ChartSeriesCanvas), 
        new PropertyMetadata(null, new PropertyChangedCallback(OnSeriesChanged)));
    /// <summary>
    /// Using a DependencyProperty as the backing store for XAxis.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty XAxisProperty =
      DependencyProperty.Register("XAxis", typeof(ChartAxis), typeof(ChartSeriesCanvas), 
        new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxeschanged)));
    /// <summary>
    /// Using a DependencyProperty as the backing store for YAxis.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty YAxisProperty =
      DependencyProperty.Register("YAxis", typeof(ChartAxis), typeof(ChartSeriesCanvas), 
        new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxeschanged)));
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the X axis.
    /// </summary>
    /// <value>The X axis.</value>
    public ChartAxis XAxis
    {
      get { return (ChartAxis)GetValue(XAxisProperty); }
      set { SetValue(XAxisProperty, value); }
}

	/// <summary>
    /// Gets or sets the Y axis.
    /// </summary>
    /// <value>The Y axis.</value>
    public ChartAxis YAxis
    {
      get { return (ChartAxis)GetValue(YAxisProperty); }
      set { SetValue(YAxisProperty, value); }
}

	/// <summary>
    /// Gets or sets the series.
    /// </summary>
    /// <value>The series.</value>
    public ChartSeries Series
    {
      get { return (ChartSeries)GetValue(SeriesProperty); }
      set { SetValue(SeriesProperty, value); }
    }
    #endregion

    #region Implementation
    /// <summary>
    /// Arranges the content of a <see cref="T:System.Windows.Controls.Canvas"></see> element.
    /// </summary>
    /// <param name="arrangeSize">The size that this <see cref="T:System.Windows.Controls.Canvas"></see> element should use to arrange its child elements.</param>
    /// <returns>
    /// A <see cref="T:System.Windows.Size"></see> that represents the arranged size of this <see cref="T:System.Windows.Controls.Canvas"></see> element and its descendants.
    /// </returns>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
      IChartTransformer transformer = this.CreateTransformer(new Rect(arrangeSize));

      foreach (UIElement element in base.InternalChildren)
      {
        ContentPresenter contentPresenter = element as ContentPresenter;
        (contentPresenter.DataContext as ChartSegment).Update(transformer);
      }

      return base.ArrangeOverride(arrangeSize);
}

	/// <summary>
    /// Measures the child elements of a <see cref="T:System.Windows.Controls.Canvas"></see> in anticipation of arranging them during the <see cref="M:System.Windows.Controls.Canvas.ArrangeOverride(System.Windows.Size)"></see> pass.
    /// </summary>
    /// <param name="constraint">An upper limit <see cref="T:System.Windows.Size"></see> that should not be exceeded.</param>
    /// <returns>
    /// A <see cref="T:System.Windows.Size"></see> that represents the size that is required to arrange child content.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
      IChartTransformer transformer = this.CreateTransformer(new Rect(constraint));

      foreach (UIElement element in base.InternalChildren)
      {
        ContentPresenter contentPresenter = element as ContentPresenter;
        (contentPresenter.DataContext as ChartSegment).Update(transformer);
      }

      return base.MeasureOverride(constraint);
}

	/// <summary>
    /// Creates the transformer.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <returns></returns>
    private IChartTransformer CreateTransformer(Rect viewport)
    {
      return ChartTransform.CreateTransformer(this.Series.Area.AreaType, viewport, this.Series);
}

	/// <summary>
    /// Called when visible range is changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public void OnVisibleRangeChanged(object sender, EventArgs e)
    {
      this.InvalidateArrange();
    }

    /// <summary>
    /// Called when series is changed.
    /// </summary>
    /// <param name="dObj">The d obj.</param>
    /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void OnSeriesChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
    {
      ChartSeriesCanvas canvas = dObj as ChartSeriesCanvas;

      if (canvas != null)
      {
      }
}

	/// <summary>
    /// Called when axes is changed.
    /// </summary>
    /// <param name="dObj">The d obj.</param>
    /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void OnAxeschanged(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
    {
      ChartSeriesCanvas canvas = dObj as ChartSeriesCanvas;

      if (canvas != null)
      {
        if (args.OldValue != null)
        {
          (args.OldValue as ChartAxis).Changed -= new EventHandler(canvas.OnVisibleRangeChanged);
        }

        if (args.NewValue != null)
        {
          (args.NewValue as ChartAxis).Changed += new EventHandler(canvas.OnVisibleRangeChanged);
        }
      }
    }
    #endregion
  }
}
