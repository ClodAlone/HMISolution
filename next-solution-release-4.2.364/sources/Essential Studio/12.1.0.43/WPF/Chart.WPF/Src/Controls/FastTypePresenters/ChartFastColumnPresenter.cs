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
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Chart
{
    //#if SyncfusionFramework4_0
    //    [System.ComponentModel.DesignTimeVisible(false)]
    //#endif
 //   public class ChartFastColumnPresenter : ChartFastSeriesPresenter
 //   {
 //       #region Members

 //       int noOfPoint = 0;

 //       #endregion

 //       public ChartFastColumnPresenter()
 //       {
 //           this.SizeChanged += new SizeChangedEventHandler(ChartFastColumnPresenter_SizeChanged);
 //       }

 //       void ChartFastColumnPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
 //       {
 //           //if (this.PointView.Count != noOfPoint)
 //           //{
 //           //    this.RenderPoints(true);
 //           //}

 //           this.AffectRender = true;

 //       }

 //       protected override void onDrawingPointsChanged(DependencyPropertyChangedEventArgs args)
 //       {
 //           base.onDrawingPointsChanged(args);
 //           if (this.PointView != null)
 //           {
 //               this.RenderPoints(false);
 //           }
 //       }


 //       internal void RenderPoints(bool redraw)
 //       {

 //           if (this.AffectRender == false)
 //               return;

 //if (this.SegmentInteriorList == null || (this.SegmentInteriorList != null && this.SegmentInteriorList.IsUpdated == false))
 //           {
 //               return;
 //           }
 // if (redraw == false && this.VisualChildrenCount != this.PointView.Count)
 //           {
 //               for (int i = 0; i < this.PointView.Count; i++)
 //               {
 //                   this.VisualCollection.Add(new DrawingVisual());
 //                   //   redraw = false;
 //               }

 //           }

 //           if (this.Interior.CanFreeze)
 //           {
 //               this.Interior.Freeze();
 //           }

 //           if (this.Stroke.CanFreeze)
 //           {
 //               this.Stroke.Freeze();
 //           }

 //           for (int i = 0; i < this.PointView.Count; i++)
 //           {
 //               DrawingVisual visual;
 //               if (redraw)
 //               {
 //                   visual = new DrawingVisual();
 //               }
 //               else
 //               {
 //                   visual = this.VisualCollection[i] as DrawingVisual;
 //               }
 //               ChartColumnDrawingValues values = this.PointView.GetItemAt(i) as ChartColumnDrawingValues;

 //               ChartFastSeriesPresenter.SetIndex(visual, i);
 //               ChartSegment segment = this.DataContext as ChartSegment;
 //               using (DrawingContext context = visual.RenderOpen())
 //               {
 //                   double strokeThickness = this.StrokeThickness;
 //                   if (this.StrokeThickness >= values.Width / 2)
 //                   {
 //                       strokeThickness = this.StrokeThickness / 4;
 //                   }

 //                   if (this.SegmentInteriorList.IsUpdated == true && this.SegmentInteriorList.DataCount == 0)// || (this.SegmentInteriorList != null && this.SegmentInteriorList.Count == 0))
 //                   {
 //                       context.DrawRectangle(this.Interior, new Pen(this.Stroke, strokeThickness), new Rect(values.X, values.Y, values.Width, values.Height));
 //                   }
 //                   else if (this.SegmentInteriorList.Count != 0)
 //                   {
 //                       if (this.SegmentInteriorList[i].Interior == null)
 //                       {
 //                           this.SegmentInteriorList[i].Interior = this.Interior;
 //                       }
 //                       if (this.SegmentInteriorList[i].Stroke == null)
 //                       {
 //                           this.SegmentInteriorList[i].Stroke = this.Stroke;
 //                       }
 //                       if (Double.IsNaN(this.SegmentInteriorList[i].StrokeThickness))
 //                       {
 //                           this.SegmentInteriorList[i].StrokeThickness = this.StrokeThickness;
 //                       }
 //                       else
 //                       {
 //                           strokeThickness = this.SegmentInteriorList[i].StrokeThickness;
 //                           if (this.SegmentInteriorList[i].StrokeThickness >= values.Width / 2)
 //                           {
 //                               strokeThickness = this.SegmentInteriorList[i].StrokeThickness / 4;
 //                           }
 //                       }
 //                       context.DrawRectangle(this.SegmentInteriorList[i].Interior, new Pen(this.SegmentInteriorList[i].Stroke, strokeThickness), new Rect(values.X, values.Y, values.Width, values.Height));

 //                   }

 //               }
 //               if (redraw)
 //               {
 //                   VisualCollection.Add(visual);
 //               }
 //           }

 //           if (redraw)
 //           {
 //               noOfPoint = this.PointView.Count;
 //           }
 //       }

 //       //protected override void OnRender(DrawingContext drawingContext)
 //       //{
 //       //    if (this.PointView != null && this.PointView.Count > 0)
 //       //    {
 //       //        ChartFastColumnSegment segment = null;
 //       //        if (this.DataContext != null)
 //       //        {
 //       //             segment = this.DataContext as ChartFastColumnSegment;
 //       //            IChartTransformer transform = ChartTransform.CreateTransformer(ChartAxesType.CartesianAxes, new Rect(this.RenderSize), segment.Series);
 //       //            segment.Update(transform);

 //       //            this.AffectRender = segment.affectRender;
 //       //        }

 //       //        if (this.PointView.Count != noOfPoint)
 //       //        {
 //       //            this.RenderPoints(true);
 //       //            //noOfPoint = this.PointView.Count;
 //       //        }
 //       //        else
 //       //        {
 //       //            this.RenderPoints(false);
 //       //        }

 //       //        if (segment != null)
 //       //        {
 //       //            segment.affectRender = false;
 //       //        }


 //       //    }
 //       //}
 //   }

   
    /// <summary>
    /// Class implementation for ChartColumnDrawingValues
    /// </summary>
     #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartColumnDrawingValues : DependencyObject
    {

        #region Dependency properties
        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(ChartColumnDrawingValues), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(ChartColumnDrawingValues), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(ChartColumnDrawingValues), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ChartColumnDrawingValues), new PropertyMetadata(0d));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X co-ordinate of segment. This is a dependency property.
        /// </summary>
        /// <value>The X value.</value>
        public double X
        {
            get
            {
                return (double)GetValue(XProperty);
            }

            set
            {
                SetValue(XProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y co-ordinate of segment. This is a dependency property.
        /// </summary>
        /// <value>The Y value.</value>
        public double Y
        {
            get
            {
                return (double)GetValue(YProperty);
            }

            set
            {
                SetValue(YProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of segment. This is a dependency property.
        /// </summary>
        /// <value>The width value.</value>
        public double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }

            set
            {
                SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of segment. This is a dependency property.
        /// </summary>
        /// <value>The height value.</value>
        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }

            set
            {
                SetValue(HeightProperty, value);
            }
        }
        #endregion
    }

    /// <summary>
        /// Class implementation for ColumnChartValuesCollection
    /// </summary>
    public class ColumnChartValuesCollection : ObservableCollection<ChartColumnDrawingValues>
    {
    }

}
