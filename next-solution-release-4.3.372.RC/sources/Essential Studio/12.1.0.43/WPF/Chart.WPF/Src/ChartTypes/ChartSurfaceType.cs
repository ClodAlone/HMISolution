#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;
    using System.Windows.Controls;
    using System.Collections;
    using System.Linq;
    using System.Globalization;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Class implementation for ChartSurfaceType
    /// </summary>
    public class ChartSurfaceType : ChartType
    {
        #region Implementation

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "Surface3D";
        }

        private List<IChartDataPoint> PointsCollection;// = new List<IChartDataPoint>();

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            VisibleSeriesCollection visibleSeries = series.Area.VisibleSeries;
            double segmentIndex = 0;
            PointsCollection = new List<IChartDataPoint>();
            for (int count = 0; count < visibleSeries.Count && (visibleSeries.Count > 1 && count + 1 < visibleSeries.Count && visibleSeries[count + 1].PointsCount > 0); count++)
            {
                segmentIndex += 1;
                ChartIndexedDataPoint[] points1 = this.GetSeriesVisiblePoints(visibleSeries[count]).ToArray();
                ChartIndexedDataPoint[] points2 = this.GetSeriesVisiblePoints(visibleSeries[count + 1]).ToArray();
                for (int i = 1, count1 = points1.Length; i < count1; i++)
                {
                    PointsCollection.Add(points1[i - 1].DataPoint);
                    PointsCollection.Add(points1[i].DataPoint);
                    PointsCollection.Add(points2[i - 1].DataPoint);
                    PointsCollection.Add(points2[i].DataPoint);
                    //series.Segments.Add(new ChartSurfaceSegment(points1[i-1].DataPoint,points1[i].DataPoint,points2[i-1].DataPoint,points2[i].DataPoint, points[i - 1], points[i], series, segmentIndex));
                }
            }
            series.Segments.Add(new ChartSurfaceSegment(PointsCollection, points, series, segmentIndex));
        }


        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <seealso cref="ChartType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            this.CalculateSegments(series, points);
        }
        #endregion

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartType.ChartTypeFlags Flags
        {
            get { return ChartTypeFlags.None | ChartTypeFlags.Indexed; }
        }
    }

    /// <summary>
    /// Class implementation for ChartSurfaceSegment
    /// </summary>
    public sealed class ChartSurfaceSegment : ChartSegment
    {
        /// <summary>
        /// Gets or Sets the Poits property
        /// </summary>
        public PointCollection Points { get; set; }

        /// <summary>
        /// Gets or sets the point1.
        /// </summary>
        /// <value>The point1.</value>
        public IChartDataPoint Point1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the point2.
        /// </summary>
        /// <value>The point2.</value>
        public IChartDataPoint Point2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the point3.
        /// </summary>
        /// <value>The point3.</value>
        public IChartDataPoint Point3
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the point4.
        /// </summary>
        /// <value>The point4.</value>
        public IChartDataPoint Point4
        {
            get;
            set;
        }


        private List<IChartDataPoint> PointsCollection = new List<IChartDataPoint>();

        /// <summary>
        /// Get or Set Point1 Property
        /// </summary>
        public Point3D Points1 { get; set; }
        /// <summary>
        /// Get or Set Point2 Property
        /// </summary>
        public Point3D Points2 { get; set; }
        /// <summary>
        /// Get or Set Point3 Property
        /// </summary>
        public Point3D Points3 { get; set; }
        /// <summary>
        /// Get or Set Point4 Property
        /// </summary>
        public Point3D Points4 { get; set; }
        /// <summary>
        /// Get or Set SegmentIndex Property
        /// </summary>
        private double SegmentIndex { get; set; }

        static ChartSurfaceSegment()
        {
            Type type = typeof(ChartSurfaceSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }


        /// <summary>
        /// Called when instance created for ChartSurfaceSegment
        /// </summary>
        /// <param name="points"></param>
        /// <param name="correspondingPoints"></param>
        /// <param name="series"></param>
        /// <param name="index"></param>
        public ChartSurfaceSegment(List<IChartDataPoint> points, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series, double index)
            : base(series, correspondingPoints)
        {
            PointsCollection = points;
            foreach (IChartDataPoint cdpt in points)
            {
                if (cdpt != null)
                {
                    xRange += cdpt.X;
                    yRange += cdpt.Y;
                }
            }
            SegmentIndex = index;
           // this.SetZRange(index);
            this.SetRange(series);
        }


        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {

        }

        /// <summary>
        /// Draw3s the D segment.
        /// </summary>
        /// <param name="transformer">The transformer.</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            MaterialGroup materialGroup;
            double next = PointsCollection.Count / SegmentIndex;
            double zindex = 0.02d;  // 1 / this.Series.Area.VisibleSeries.Count;
            for (int i = 3; i < PointsCollection.Count; i += 4)
            {
                GeometryModel3D model = new GeometryModel3D();
                Points1 = transformer.TransformToVisible(PointsCollection[i - 3].X, PointsCollection[i - 3].Y, zindex);
                Points2 = transformer.TransformToVisible(PointsCollection[i - 2].X, PointsCollection[i - 2].Y, zindex);
                Points3 = transformer.TransformToVisible(PointsCollection[i - 1].X, PointsCollection[i - 1].Y, zindex);
                Points4 = transformer.TransformToVisible(PointsCollection[i].X, PointsCollection[i].Y, zindex);
                model.Geometry = MeshGenerator.SurfaceSegment(Points1, Points2, Points3, Points4);

                DiffuseMaterial diffuseMaterial = new DiffuseMaterial();
                Binding binding = new Binding("Interior");
                binding.Source = Series;
                BindingOperations.SetBinding(diffuseMaterial, DiffuseMaterial.BrushProperty, binding);
                SpecularMaterial specularMaterial = new SpecularMaterial();
                specularMaterial.Brush = new SolidColorBrush(Color.FromRgb(85, 85, 85));
                specularMaterial.SpecularPower = 5d;
                
                materialGroup = new MaterialGroup();
                materialGroup.Children.Add(specularMaterial);
                materialGroup.Children.Add(diffuseMaterial);
                model.Material = materialGroup;
                TranslateTransform3D translate = new TranslateTransform3D(-0.5, -0.5, -0.5);
                RotateTransform3D rotate1 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(-1, 0, 0), 180));
                Transform3DGroup grp = new Transform3DGroup();
                grp.Children.Add(translate);
                grp.Children.Add(rotate1);
                model.Transform = grp;
                Geometry3DGroup.Children.Add(model);
                if (((i +1) % next) == 0)
                    zindex += 0.07d;
            }
        }
    }

}
