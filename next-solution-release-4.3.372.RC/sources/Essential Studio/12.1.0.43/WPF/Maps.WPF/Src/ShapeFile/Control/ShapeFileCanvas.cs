#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    ///  ShapeFileCanvas is Panel which arranges the Map shapes
    /// </summary>
    public class ShapeFileCanvas : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.ShapeFileCanvas"/> class.
        /// </summary>
        public ShapeFileCanvas()
        {
        }

        #region Latitude (Attached DependencyProperty)

        /// <summary>
        ///  Latitude is a Attached property which sets the Latitude point of the Map
        /// </summary>
        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.RegisterAttached("Latitude", typeof(double), typeof(ShapeFileCanvas), new PropertyMetadata(0d));

        /// <summary>
        ///  This Method is sets the value for Latitude
        /// </summary>
        /// <param name="o">ShapeFileCanvas</param>
        /// <param name="value">New Value</param>
        public static void SetLatitude(DependencyObject o, double value)
        {
            if (o != null)
            {
                o.SetValue(LatitudeProperty, value);
            }
        }

#if WPF
        /// <summary>
        /// This method Gets the Value of Latitude
        /// </summary>
        /// <param name="o"></param>
        [AttachedPropertyBrowsableForChildren(IncludeDescendants = true)]

#else

        /// <summary>
        /// This method Gets the Value of Latitude
        /// </summary>
        /// <param name="o"></param>
#endif
        public static double GetLatitude(DependencyObject o)
        {
            return (double)o.GetValue(LatitudeProperty);
        }

        #endregion

        #region Longitude (Attached DependencyProperty)

        /// <summary>
        ///  Longitude is a Attached property which sets the longitude point of the Map
        /// </summary>
        public static readonly DependencyProperty LongitudeProperty =
            DependencyProperty.RegisterAttached("Longitude", typeof(double), typeof(ShapeFileCanvas), new PropertyMetadata(0d));
        /// <summary>
        /// This method Sets the Value of Longitude
        /// </summary>
        /// <param name="value"></param>
        /// <param name="o"></param>
        public static void SetLongitude(DependencyObject o, double value)
        {
            if (o != null)
            {
                o.SetValue(LongitudeProperty, value);
            }
        }

#if WPF
        /// <summary>
        /// This method gets the value of Longitude
        /// </summary>
        /// <param name="o"></param>
        [AttachedPropertyBrowsableForChildren(IncludeDescendants = true)]
#else

        /// <summary>
        /// This method gets the value of Longitude
        /// </summary>
        /// <param name="o"></param>
#endif
        public static double GetLongitude(DependencyObject o)
        {
            return (double)o.GetValue(LongitudeProperty);
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether this instance is suspend or not.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if this instance ; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsInSuspend
        {
            get;
            internal set;
        }

#if WPF
        /// <summary>
        /// Starts the initialization process for this element.
        /// </summary>
        public override void BeginInit()
#else
            /// <summary>
        /// Starts the initialization process for this element.
        /// </summary>
        public void BeginInit()
#endif
        {
#if WPF
            base.BeginInit();
#endif
            if (!this.IsInSuspend)
            {
                this.IsInSuspend = true;
            }
        }

#if WPF
        /// <summary>
        /// Indicates that the initialization process for the element is complete.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException"><see cref="M:System.Windows.FrameworkElement.EndInit"/> was called without <see cref="M:System.Windows.FrameworkElement.BeginInit"/> having previously been
        /// called on the element.</exception>
        public override void EndInit()
#else
        /// <summary>
        /// Indicates that the initialization process for the element is complete.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException"><see cref="M:System.Windows.FrameworkElement.EndInit"/> was called without <see cref="M:System.Windows.FrameworkElement.BeginInit"/> having previously been
        /// called on the element.</exception>
        public void EndInit()
#endif
        {
#if WPF
            base.EndInit();
#endif
            if (this.IsInSuspend)
            {
                this.IsInSuspend = false;
#if WPF
                this.InvalidateVisual();
#else
                this.InvalidateArrange();
                this.InvalidateMeasure();
#endif
            }
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for
        /// child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its
        /// calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(System.Windows.Size constraint)
        {
            if (!MapControl.isXmlContent)
            {
                if (this.IsInSuspend || this.Children.Count == 0)
                {
                    return new Size();
                }
            }
            Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement element in this.Children)
            {
                if (element != null)
                {
                    element.Measure(availableSize);
                }
            }

            return new Size();
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a
        /// size for a <see cref="T:System.Windows.FrameworkElement"/> derived class. 
        /// </summary>
        /// <param name="arrangeSize">The final area within the parent that this element
        /// should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {

            if (this.Children.Count > 0)
            {
                var shapeFileControl = MapControl.FindParent<ShapeFileLayer>((FrameworkElement)this.Children[0]);




                if (!MapControl.isXmlContent)
                {
                    if (this.IsInSuspend || this.Children.Count == 0)
                    {
                        return arrangeSize;
                    }
                    if (shapeFileControl == null || (shapeFileControl != null && !shapeFileControl.HasFileData))
                    {
                        return arrangeSize;
                    }
                }
                foreach (UIElement element in this.Children)
                {
                    if (element == null)
                    {
                        continue;
                    }

                    double x = 0.0;
                    double y = 0.0;

                    var lat = ShapeFileCanvas.GetLatitude(element);
                    var lon = ShapeFileCanvas.GetLongitude(element);
                    var position = shapeFileControl.LatitudeLongitudeToPoint(new Point(lon, lat));
                    if (!IsNaN(position.X))
                    {
                        x = position.X;
                    }

                    if (!IsNaN(position.Y))
                    {
                        y = position.Y;
                    }

                    element.Arrange(new Rect(new Point(x, y), element.DesiredSize));

                }
            }

            return arrangeSize;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            // Fields
            [FieldOffset(0)]
            internal double DoubleValue;
            [FieldOffset(0)]
            internal ulong UintValue;
        }

        /// <summary>
        ///  This method returns the given double value is NaN or not
        /// </summary>
        /// <param name="value">Double Value</param>
        /// <returns>
        /// 	<see langword="true"/> if not NaN; <see langword="false"/> if NaN
        /// </returns>
        public static bool IsNaN(double value)
        {
            NanUnion union = new NanUnion
            {
                DoubleValue = value
            };
            ulong num = union.UintValue & 18442240474082181120L;
            ulong num2 = union.UintValue & ((ulong)0xfffffffffffffL);

            if ((num != 0x7ff0000000000000L) && (num != 18442240474082181120L))
            {
                return false;
            }

            return (num2 != 0L);
        }
    }
}
