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
#if WINDOWS_PHONE
using System.Windows;

#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.ApplicationModel;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for PieSegment3D
    /// </summary>
    public class PieSegment3D : ChartSegment3D
    {

        #region members

        readonly SfChart3D area;

        public double YData { get; set; }

        public double XData { get; set; }

        internal List<Point> Points = new List<Point>();

        private const double DtoR = Math.PI / 180d;

        readonly PieSeries3D series3D;

        internal Vector3D Center;

        private double inSideRadius;

        double depth;
        double radius;

        int pieIndex;
        int pieCount;

        int index;

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="PieSegment3D"/> class.
        /// </summary>
        public PieSegment3D()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PieSegment3D"/> class.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="center">The center.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="height">The height.</param>
        /// <param name="r">The r.</param>
        /// <param name="i">The i.</param>
        /// <param name="y">The y.</param>
        /// <param name="insideRadius">The inside radius.</param>
        public PieSegment3D(ChartSeries3D series, Vector3D center, double start, double end, double height, double r, int i, double y, double insideRadius)
        {
            Series = series3D = series as PieSeries3D;
            area = series.Area;
            Item = series.ActualData[i];
            StartValue = start;
            EndValue = end;
            depth = height;
            radius = r;
            if (series3D != null)
            {
                pieCount = series3D.GetCircularSeriesCount();
                pieIndex = series3D.GetPieSeriesIndex();
            }
            index = i;
            YData = y;
            Center = center;
            inSideRadius = insideRadius;
            if (series.CanAnimate) return;
            ActualEndValue = end;
            ActualStartValue = start;
        }

        public override void SetData(params double[] values)
        {
            StartValue = ActualStartValue = values[0];
            EndValue = ActualEndValue = values[1];
            depth = values[2];
            radius = values[3];
            YData = values[4];
            var center = new Vector3D(values[5], values[6], values[7]);
            Center = center;
            inSideRadius = values[8];
        }

        #endregion

        #region abstract

        public override UIElement CreateVisual(Size size)
        {
            return null;
        }

        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        public override void Update(IChartTransformer transformer)
        {
            CreateSector();
        }

        public override void OnSizeChanged(Size size)
        {
        }

        #endregion

        #region properties

        internal double StartValue { get; set; }

        internal double EndValue { get; set; }

        private static void OnValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var segment = (d as PieSegment3D);
            if (segment != null) segment.ScheduleRender();
        }

        private void ScheduleRender()
        {
#if NETFX_CORE
            IAsyncAction valueChanged;
            if (DesignMode.DesignModeEnabled)
                OnValuesChanged();
            else
                valueChanged= Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, OnValuesChanged);
#else
#if WPF
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(OnValuesChanged));
#else
            Dispatcher.BeginInvoke(OnValuesChanged);
#endif
#endif
        }


        private void OnValuesChanged()
        {
            if (!series3D.EnableAnimation) return;
            var indexOf = series3D.Segments.IndexOf(this);
            var g3D = area.Graphics3D;
            var polygons = area.Graphics3D.GetVisual();
            var items = polygons.Where(item => item.Tag == this);
            foreach (var item in items.ToList())
            {
                polygons.Remove(item);
            }
            if (indexOf != series3D.Segments.Count - 1) return;
            series3D.UpdateOnSeriesBoundChanged(Size.Empty);

            if (series3D.adornmentInfo != null)
            {
                var adornments = area.Graphics3D.GetVisual().OfType<UIElement3D>().ToList();
                foreach (var item in adornments)
                {
                    area.Graphics3D.Remove(item);
                    area.Graphics3D.AddVisual(item);
                }
            }
            if (pieIndex != 0) return;
            g3D.PrepareView();
            g3D.View(area.RootPanel);
        }

        /// <summary>
        /// Gets or sets the actual start value.
        /// </summary>
        /// <value>
        /// The actual start value.
        /// </value>
        internal double ActualStartValue
        {
            get { return (double)GetValue(ActualStartValueProperty); }
            set { SetValue(ActualStartValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualStartValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualStartValueProperty =
            DependencyProperty.Register("ActualStartValue", typeof(double), typeof(PieSegment3D), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the actual end value.
        /// </summary>
        /// <value>
        /// The actual end value.
        /// </value>
        internal double ActualEndValue
        {
            get { return (double)GetValue(ActualEndValueProperty); }
            set { SetValue(ActualEndValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualEndValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualEndValueProperty =
            DependencyProperty.Register("ActualEndValue", typeof(double), typeof(PieSegment3D), new PropertyMetadata(0d, OnValuesChanged));

        #endregion

        #region methods

        /// <summary>
        /// Creates the sector.
        /// </summary>
        internal Polygon3D[][] CreateSector()
        {
            Points.Clear();
            var count = (int)Math.Ceiling(ActualEndValue / 6d);
            if (count < 1d) return null;
            var res = new Polygon3D[4][];
            var f = ActualEndValue/count;

            var oPts = new Point[count + 1];
            var iPts = new Point[count + 1];

            for (var i = 0; i < count + 1; i++)
            {
                var ox = (float)(Center.X + radius * Math.Cos((ActualStartValue + i * f) * DtoR));
                var oy = (float)(Center.Y + radius * Math.Sin((ActualStartValue + i * f) * DtoR));

                oPts[i] = new Point(ox, oy);

                var ix = (float)(Center.X + inSideRadius * Math.Cos((ActualStartValue + i * f) * DtoR));
                var iy = (float)(Center.Y + inSideRadius * Math.Sin((ActualStartValue + i * f) * DtoR));

                iPts[i] = new Point(ix, iy);
                Points.Add(new Point(ox, oy));
            }

            var oPlgs = new Polygon3D[count];

            for (var i = 0; i < count; i++)
            {
                Vector3D[] vts = { new Vector3D( oPts[ i ].X, oPts[ i ].Y, 0 ),
                    new Vector3D( oPts[ i ].X, oPts[ i ].Y, depth ),
                    new Vector3D( oPts[ i+1 ].X, oPts[ i+1 ].Y, depth ),
                    new Vector3D( oPts[ i+1 ].X, oPts[ i+1 ].Y, 0 ) };


                oPlgs[i] = new Polygon3D(vts, this, index, Stroke, StrokeThickness, Interior);
            }

            res[1] = oPlgs;

            if (inSideRadius > 0)
            {
                var iPlgs = new Polygon3D[count];

                for (int i = 0; i < count; i++)
                {
                    var vts = new[]{ new Vector3D( iPts[ i ].X, iPts[ i ].Y, 0 ),
                                           new Vector3D( iPts[ i ].X, iPts[ i ].Y, depth ),
                                           new Vector3D( iPts[ i+1 ].X, iPts[ i+1 ].Y, depth ),
                                           new Vector3D( iPts[ i+1 ].X, iPts[ i+1 ].Y, 0 ) };

                    iPlgs[i] = new Polygon3D(vts, this, index, Stroke, StrokeThickness, Interior);
                }

                res[3] = iPlgs;
            }

            var tVtxs = new List<Vector3D>();
            var bVtxs = new List<Vector3D>();

            for (int i = 0; i < count + 1; i++)
            {
                tVtxs.Add(new Vector3D(oPts[i].X, oPts[i].Y, 0));
                bVtxs.Add(new Vector3D(oPts[i].X, oPts[i].Y, depth));
            }

            if (inSideRadius > 0)
            {
                for (int i = count; i > -1; i--)
                {
                    tVtxs.Add(new Vector3D(iPts[i].X, iPts[i].Y, 0));
                    bVtxs.Add(new Vector3D(iPts[i].X, iPts[i].Y, depth));
                }
            }
            else
            {
                tVtxs.Add(Center);
                bVtxs.Add(new Vector3D(Center.X, Center.Y, depth));
            }

            res[0] = new[]{ new Polygon3D( tVtxs.ToArray(), this, index, Stroke, StrokeThickness, Interior),
                                    new Polygon3D( bVtxs.ToArray(), this, index, Stroke, StrokeThickness, Interior ) };

            if (inSideRadius > 0)
            {
                Vector3D[] rvts =
                {
                    new Vector3D(oPts[0].X, oPts[0].Y, 0),
                    new Vector3D(oPts[0].X, oPts[0].Y, depth),
                    new Vector3D(iPts[0].X, iPts[0].Y, depth),
                    new Vector3D(iPts[0].X, iPts[0].Y, 0)
                };

                Vector3D[] lvts =
                {
                    new Vector3D(oPts[count].X, oPts[count].Y, 0),
                    new Vector3D(oPts[count].X, oPts[count].Y, depth),
                    new Vector3D(iPts[count].X, iPts[count].Y, depth),
                    new Vector3D(iPts[count].X, iPts[count].Y, 0)
                };

                res[2] = new[]
                {
                    new Polygon3D(rvts, this, index, Stroke, StrokeThickness, Interior),
                    new Polygon3D(lvts, this, index, Stroke, StrokeThickness, Interior)
                };

            }
            else
            {
                Vector3D[] rvts =
                {
                    new Vector3D(oPts[0].X, oPts[0].Y, 0),
                    new Vector3D(oPts[0].X, oPts[0].Y, depth),
                    new Vector3D(Center.X, Center.Y, depth),
                    new Vector3D(Center.X, Center.Y, 0)
                };

                Vector3D[] lvts =
                {
                    new Vector3D(oPts[count].X, oPts[count].Y, 0),
                    new Vector3D(oPts[count].X, oPts[count].Y, depth),
                    new Vector3D(Center.X, Center.Y, depth),
                    new Vector3D(Center.X, Center.Y, 0)
                };

                res[2] = new[]
                {
                    new Polygon3D(rvts, this, index, Stroke, StrokeThickness, Interior),
                    new Polygon3D(lvts, this, index, Stroke, StrokeThickness, Interior)
                };
            }

            //foreach (var polyColl in res)
            //{
            //    if (polyColl != null)
            //        foreach (var poly in polyColl)
            //        {
            //            series3D.Area.Graphics3D.AddVisual(poly);
            //        }
            //}
            return res;
        }
        #endregion
    }
}
