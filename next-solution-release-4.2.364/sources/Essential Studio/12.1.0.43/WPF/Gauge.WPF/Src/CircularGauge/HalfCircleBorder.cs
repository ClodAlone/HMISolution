// <copyright file="HalfCircleBorder.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the half circle border of the circular gauge control.
    /// </summary>
    /// <remarks>
    /// The <see cref="GaugeFrameType"/> should be set to <see cref="GaugeFrameType.HalfCircle"/>
    /// inorder to display a half circle shape <see cref="CircularGauge"/>
    /// </remarks>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="HalfCircleSample.Window1" Title="HalfCircleSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <syncfusion:CircularGauge Name="circularGauge" FrameType="HalfCircle">
    ///         <syncfusion:CircularGauge.Scales>
    ///             <syncfusion:CircularScale Name="circularScale" Radius="100"> 
    ///             </syncfusion:CircularScale>
    ///         </syncfusion:CircularGauge.Scales>
    ///     </syncfusion:CircularGauge>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace HalfCircleSample
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///         private CircularScale m_scale;
    ///         private CircularGauge m_gauge;
    ///         public Window1()
    ///         {
    ///             InitializeComponent();<para/>
    ///             m_scale = new CircularScale();
    ///             m_gauge = new CircularGauge();
    ///             m_scale.Radius = 116;
    ///             this.m_gauge.Scales.Add(m_scale);      
    ///             m_gauge.FrameType = GaugeFrameType.HalfCircle;
    ///             this.Content = m_gauge;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class HalfCircleBorder : Decorator
    {
        #region Events
        /// <summary>
        /// Event that is raised when <see cref="BorderWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback BorderWidthChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="BackgroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(HalfCircleBorder), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent), FrameworkPropertyMetadataOptions.AffectsRender));
        //new PropertyChangedCallback(OnBackgroundBrushChanged)
        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(HalfCircleBorder), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent), FrameworkPropertyMetadataOptions.AffectsRender));
        //new PropertyChangedCallback(OnBorderBrushChanged)

        /// <summary>
        /// Identifies the <see cref="BorderWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(double), typeof(HalfCircleBorder), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnBorderWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="CircleSweepDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CircleSweepDirectionProperty =
            DependencyProperty.Register("CircleSweepDirection", typeof(SweepDirection), typeof(HalfCircleBorder), new FrameworkPropertyMetadata(SweepDirection.Counterclockwise, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="InnerCircleRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerCircleRadiusProperty =
            DependencyProperty.Register("InnerCircleRadius", typeof(double), typeof(HalfCircleBorder), new FrameworkPropertyMetadata(15d, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="InnerCircleSweepDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerCircleSweepDirectionProperty =
            DependencyProperty.Register("InnerCircleSweepDirection", typeof(SweepDirection), typeof(HalfCircleBorder), new FrameworkPropertyMetadata(SweepDirection.Counterclockwise, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the background brush of the element.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public Brush BackgroundBrush
        {
            get
            {
                return (Brush)GetValue(BackgroundBrushProperty);
            }

            set
            {
                SetValue(BackgroundBrushProperty, value);
            }
        }





        /// <summary>
        /// Gets or sets Type of the Frame.
        /// </summary>
        /// <value>
        /// GaugeFrameType
        /// </value>
        public GaugeFrameType FrameType
        {
            get { return (GaugeFrameType)GetValue(FrameTypeProperty); }
            set { SetValue(FrameTypeProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for FrameType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FrameTypeProperty =
            DependencyProperty.Register("FrameType", typeof(GaugeFrameType), typeof(HalfCircleBorder),  new UIPropertyMetadata(GaugeFrameType.HalfCircle));

        

        /// <summary>
        /// Gets or sets the border brush of the element.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        /// <seealso cref="BackgroundBrush"/>
        public Brush BorderBrush
        {
            get
            {
                return (Brush)GetValue(BorderBrushProperty);
            }

            set
            {
                SetValue(BorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the border.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double BorderWidth
        {
            get
            {
                return (double)GetValue(BorderWidthProperty);
            }

            set
            {
                SetValue(BorderWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets half circle's sweep direction
        /// </summary>
        public SweepDirection CircleSweepDirection
        {
            get
            {
                return (SweepDirection)GetValue(CircleSweepDirectionProperty);
            }

            set
            {
                SetValue(CircleSweepDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets half circle's inner circle radius
        /// </summary>
        public double InnerCircleRadius
        {
            get
            {
                return (double)GetValue(InnerCircleRadiusProperty);
            }

            set
            {
                SetValue(InnerCircleRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets inner circle's sweep direction
        /// </summary>
        public SweepDirection InnerCircleSweepDirection
        {
            get
            {
                return (SweepDirection)GetValue(InnerCircleSweepDirectionProperty);
            }

            set
            {
                SetValue(InnerCircleSweepDirectionProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Implementation
        /// <summary>
        /// Converts polar coordinates to stage coordinates.
        /// </summary>
        /// <param name="radius">Radius of the Scale</param>
        /// <param name="angle">The angle between 0 and the point whose stage coordinates should be found</param>
        /// <param name="pointToShift">The point to shift to</param>
        /// <returns>Stage Coordinates</returns>
        internal Point ConvertToStageCoordinates(double radius, double angle, Point pointToShift)
        {
            Point point;
            if (SweepDirection.Counterclockwise == CircleSweepDirection)
            {
                point = new Point(radius * Math.Cos(angle * Math.PI / 90), radius * Math.Sin(angle * Math.PI / 90));
            }
            else
            {
                point = new Point(radius * Math.Cos(angle * Math.PI / 180), radius * Math.Sin(angle * Math.PI / 180));
            }

            point.X += pointToShift.X;
            point.Y += pointToShift.Y;
            return point;
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="BorderWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnBorderWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BorderWidthChanged != null)
            {
                this.BorderWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnBorderWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBorderWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HalfCircleBorder instance = (HalfCircleBorder)d;
            instance.OnBorderWidthChanged(e);
        }
        #endregion Implementation

        #region Overrides
        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="dc">The drawing instructions for a specific element.</param>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            PathGeometry path = new PathGeometry();
            Point centerPoint;
            Point startPoint1;
            Point endPoint1;
            Point startPoint2;
            Point endPoint2;
            Point lastPoint;
            double outerRadius = this.ActualWidth / 2;
            double innerRadius = InnerCircleRadius;

            if (CircleSweepDirection == SweepDirection.Counterclockwise)
            {
                centerPoint = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
                startPoint1 = this.ConvertToStageCoordinates(outerRadius, 0, centerPoint);
                endPoint1 = this.ConvertToStageCoordinates(outerRadius, 90, centerPoint);
                startPoint2 = this.ConvertToStageCoordinates(innerRadius, 0, centerPoint);
                endPoint2 = this.ConvertToStageCoordinates(innerRadius, 90, centerPoint);
                lastPoint = this.ConvertToStageCoordinates(outerRadius + (this.BorderWidth / 2), 90, centerPoint);
            }
            else
            {
                centerPoint = new Point(this.ActualWidth / 2, this.ActualHeight);
                startPoint1 = this.ConvertToStageCoordinates(outerRadius, 180, centerPoint);
                endPoint1 = this.ConvertToStageCoordinates(outerRadius, 0, centerPoint);
                startPoint2 = this.ConvertToStageCoordinates(innerRadius, 180, centerPoint);
                endPoint2 = this.ConvertToStageCoordinates(innerRadius, 0, centerPoint);
                lastPoint = this.ConvertToStageCoordinates(outerRadius + (this.BorderWidth / 2), 180, centerPoint);
            }

            ArcSegment asp1 = new ArcSegment(startPoint1, new Size(outerRadius, this.ActualHeight), 0, false, SweepDirection.Clockwise, true);
            ArcSegment aep1 = new ArcSegment(endPoint1, new Size(outerRadius, this.ActualHeight), 0, false, SweepDirection.Clockwise, true);
            LineSegment lep2 = new LineSegment(endPoint2, true);
            ArcSegment aep2 = new ArcSegment(endPoint2, new Size(innerRadius, innerRadius), 0, false, InnerCircleSweepDirection, true);
            ArcSegment asp2 = new ArcSegment(startPoint2, new Size(innerRadius, innerRadius), 0, false, InnerCircleSweepDirection, true);
            LineSegment lp = new LineSegment(lastPoint, true);

            PathFigure figure1 = new PathFigure(
                startPoint1,
                new PathSegment[] { asp1, aep1, lep2, aep2, asp2, lp },
                false);
            path.Figures.Add(figure1);
            TransformGroup group = new TransformGroup();
            if (this.FrameType == GaugeFrameType.LeftHalfCircle)
            {
                RotateTransform rotate = new RotateTransform(-90, centerPoint.X, centerPoint.Y); 
                group.Children.Add(rotate);
                path.Transform = group;
            }
            else if (this.FrameType == GaugeFrameType.RightHalfCircle)
            {
                RotateTransform rotate = new RotateTransform(90, centerPoint.X, centerPoint.Y); 
                group.Children.Add(rotate);
                path.Transform = group;
            }
         
            

            dc.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), path);
        }
        #endregion Overrides
    }
}
