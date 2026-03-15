#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart.Olap
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;


#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public class WaitingControl : Control
    {

        #region DependencyProperties
        /// <summary>
        /// Identifies the Geometry property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(WaitingControl),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the MaxRadiusPercent property.
        /// </summary>
        public static readonly DependencyProperty MaxRadiusPercentProperty =
            DependencyProperty.Register("MaxRadiusPercent", typeof(double), typeof(WaitingControl), new FrameworkPropertyMetadata(0.3d, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Identifies the MinRadiusPercent property.
        /// </summary>
        public static readonly DependencyProperty MinRadiusPercentProperty =
            DependencyProperty.Register("MinRadiusPercent", typeof(double), typeof(WaitingControl), new FrameworkPropertyMetadata(0.001d, FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="WaitingControl"/> class.
        /// </summary>
        static WaitingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaitingControl), new FrameworkPropertyMetadata(typeof(WaitingControl)));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the geometry. This is a dependency property.
        /// </summary>
        /// <value>The geometry.</value>
        //public Geometry Geometry
        //{
        //    get { return (Geometry)GetValue(GeometryProperty); }
        //    set { SetValue(GeometryProperty, value); }
        //}

        /// <summary>
        /// Gets or sets the max radius percent. This is a dependency property.
        /// </summary>
        /// <value>The max radius percent.</value>
        //public double MaxRadiusPercent
        //{
        //    get { return (double)GetValue(MaxRadiusPercentProperty); }
        //    set { SetValue(MaxRadiusPercentProperty, value); }
        //}

        /// <summary>
        /// Gets or sets the min radius percent. This is a dependency property.
        /// </summary>
        /// <value>The min radius percent.</value>
        //public double MinRadiusPercent
        //{
        //    get { return (double)GetValue(MinRadiusPercentProperty); }
        //    set { SetValue(MinRadiusPercentProperty, value); }
        //}
        #endregion

        #region Implementation
        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>
        /// The size of the control, up to the maximum specified by <paramref name="constraint"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            //  GeometryGroup geometry = new GeometryGroup();
            //  Point center = new Point(constraint.Width / 2, constraint.Height / 2);
            //  double mainRadius = Math.Min(constraint.Width, constraint.Height) / 2;
            //  double maxBubbleRadius = mainRadius * MaxRadiusPercent;
            //  double minBubbleRadius = mainRadius * MinRadiusPercent;
            //  mainRadius -= maxBubbleRadius;
            //  double bubbleRadiusDelta = (maxBubbleRadius - minBubbleRadius) / (2 * Math.PI * mainRadius / (maxBubbleRadius + minBubbleRadius));//2 * Math.PI * mainRadius / (maxBubbleRadius - minBubbleRadius);
            //  Point bubblePosition;
            //  double angle = 0;
            //  double borderAngle = Math.Asin((maxBubbleRadius + minBubbleRadius) / mainRadius);
            //  while (angle <= Math.PI * 2 - borderAngle)
            //  {
            //    bubblePosition = new Point(center.X + mainRadius * Math.Cos(angle), center.Y + mainRadius * Math.Sin(angle));
            //    geometry.Children.Add(new EllipseGeometry(bubblePosition, maxBubbleRadius, maxBubbleRadius));
            //    angle += 2 * Math.Asin((maxBubbleRadius - bubbleRadiusDelta / 2) / mainRadius);
            //    maxBubbleRadius -= bubbleRadiusDelta;
            //  }
            //  this.Geometry = geometry;
            //  bounds = constraint;
            //}
            return base.MeasureOverride(constraint);
        }
        #endregion
    }
}
