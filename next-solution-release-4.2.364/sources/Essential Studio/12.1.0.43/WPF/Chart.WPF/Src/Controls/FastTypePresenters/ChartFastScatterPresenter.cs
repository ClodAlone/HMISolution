#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Syncfusion.Windows.Chart
{
    //#if SyncfusionFramework4_0
    //    [System.ComponentModel.DesignTimeVisible(false)]
    //#endif
  //  public class ChartFastScatterPresenter : ChartFastSeriesPresenter
  //  {
  //      int noOfPoint = 0;


  //      public ChartFastScatterPresenter()
  //      {
  //          this.SizeChanged += new SizeChangedEventHandler(ChartFastScatterPresenter_SizeChanged);
  //      }

  //      void ChartFastScatterPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
  //      {
  //          //if (this.PointView.Count != noOfPoint)
  //          //{
  //          //    this.RenderPoints(true);
  //          //}
  //          this.AffectRender = true;
  //      }


  //      protected override void onDrawingPointsChanged(DependencyPropertyChangedEventArgs args)
  //      {
  //          base.onDrawingPointsChanged(args);
  //          if (this.PointView != null)
  //          {
  //              this.RenderPoints(false);
  //          }
  //      }

  //      internal void RenderPoints(bool redraw)
  //      {
  //          if (this.AffectRender == false)
  //              return;
  //if (this.SegmentInteriorList == null || (this.SegmentInteriorList != null && this.SegmentInteriorList.IsUpdated == false))
  //          {
  //              return;
  //         }
          
  //          if (redraw == false && this.VisualChildrenCount != this.PointView.Count)
  //          {
  //              for (int i = 0; i < this.PointView.Count; i++)
  //              {
  //                  this.VisualCollection.Add(new DrawingVisual());
  //                  //   redraw = false;
  //              }

  //          }

  //          if (this.Interior.CanFreeze)
  //          {
  //              this.Interior.Freeze();
  //          }

  //          if (this.Stroke.CanFreeze)
  //          {
  //              this.Stroke.Freeze();
  //          }

  //          for (int i = 0; i < this.PointView.Count; i++)
  //          {
  //              DrawingVisual visual;
  //              if (redraw)
  //              {
  //                  visual = new DrawingVisual();
  //              }
  //              else
  //              {
  //                  visual = this.VisualCollection[i] as DrawingVisual;
  //              }
  //              ScatterChartDrawingValues values = this.PointView.GetItemAt(i) as ScatterChartDrawingValues;
  //              ChartFastSeriesPresenter.SetIndex(visual, i);
  //              ChartSegment segment = this.DataContext as ChartSegment;
  //              using (DrawingContext context = visual.RenderOpen())
  //              {
  //                  if (this.SegmentInteriorList.IsUpdated == true && this.SegmentInteriorList.DataCount == 0)// || (this.SegmentInteriorList != null && this.SegmentInteriorList.Count == 0))
  //                  {
  //                      //return;
  //                      //context.DrawEllipse(this.Interior, new Pen(this.Stroke, this.StrokeThickness), values.CenterPoint, 5, 5);
  //                      context.DrawEllipse(this.Interior, new Pen(this.Stroke, this.StrokeThickness), values.CenterPoint, values.radiusX, values.radiusY);
  //                  }
  //                  else if (this.SegmentInteriorList.Count != 0)
  //                  {
  //                      if (this.SegmentInteriorList[i].Interior == null)
  //                      {
  //                          this.SegmentInteriorList[i].Interior = this.Interior;
  //                      }
  //                      if (this.SegmentInteriorList[i].Stroke == null)
  //                      {
  //                          this.SegmentInteriorList[i].Stroke = this.Stroke;
  //                      }
  //                      if (Double.IsNaN(this.SegmentInteriorList[i].StrokeThickness))
  //                      {
  //                          this.SegmentInteriorList[i].StrokeThickness = this.StrokeThickness;
  //                      }
  //                      context.DrawEllipse(this.SegmentInteriorList[i].Interior, new Pen(this.Stroke, this.SegmentInteriorList[i].StrokeThickness), values.CenterPoint, 5, 5);
  //                  }

  //              }
  //              if (redraw)
  //              {
  //                  VisualCollection.Add(visual);
  //              }
  //          }

  //          if (redraw)
  //          {
  //              noOfPoint = this.PointView.Count;
  //          }
  //      }

  //      protected override void OnRender(DrawingContext drawingContext)
  //      {
  //          if (this.PointView != null && this.PointView.Count > 0)
  //          {
  //              ChartFastScatterSegment segment = null;
  //              if (this.DataContext != null)
  //              {
  //                  segment = this.DataContext as ChartFastScatterSegment;
  //                  IChartTransformer transform = ChartTransform.CreateTransformer(ChartAxesType.CartesianAxes, new Rect(this.RenderSize), segment.Series);
  //                  segment.Update(transform);
  //                  this.AffectRender = segment.affectRender;
  //              }

  //              if (this.PointView.Count != noOfPoint)
  //              {
  //                  this.RenderPoints(true);
  //                  noOfPoint = this.PointView.Count;
  //              }
  //              else
  //              {
  //                  this.RenderPoints(false);
  //              }
  //              if (segment != null)
  //              {
  //                  segment.affectRender = false;
  //              }

  //          }
  //      }
  //  }

    
    /// <summary>
    /// Class implementation for ScatterChartDrawingValues
    /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScatterChartDrawingValues : DependencyObject
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty radiusXProperty =
            DependencyProperty.Register("radiusX", typeof(double), typeof(ScatterChartDrawingValues), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        public static readonly DependencyProperty radiusYProperty =
            DependencyProperty.Register("radiusY", typeof(double), typeof(ScatterChartDrawingValues), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the CenterPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterPointProperty =
          DependencyProperty.Register("CenterPoint", typeof(Point), typeof(ScatterChartDrawingValues), new PropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets CenterPointProperty
        /// </summary>
        public Point CenterPoint
        {
            set { SetValue(CenterPointProperty, value); }
            get { return (Point)GetValue(CenterPointProperty); }
        }

        /// <summary>
        /// Gets or sets the width of segment. This is a dependency property.
        /// </summary>
        /// <value>The width value.</value>
        public double radiusX//Width
        {
            get
            {
                return (double)GetValue(radiusXProperty);
            }

            set
            {
                SetValue(radiusXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of segment. This is a dependency property.
        /// </summary>
        /// <value>The height value.</value>
        public double radiusY//Height
        {
            get
            {
                return (double)GetValue(radiusYProperty);
            }

            set
            {
                SetValue(radiusYProperty, value);
            }
        }
        #endregion

    }

    /// <summary>
        /// Class implementation for ScatterChartDrawingValuesCollection
    /// </summary>
    public class ScatterChartDrawingValuesCollection : ObservableCollection<ScatterChartDrawingValues>
    {

    }
}
