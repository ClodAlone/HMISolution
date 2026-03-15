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
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Panel for Adding a elements on the Maps.
    /// </summary>    
    [System.ComponentModel.Description("Panel for Adding a elements on the Maps.")]
    public class MapElementPanel:Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.MapElementPanel">MapElementPanel</see> class. 
        /// </summary>
        [System.ComponentModel.Description("Initializes a new instance of the MapElementPanel class.")]
        public MapElementPanel()
        {

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
            if (this.Children.Count == 0)
            {
                return new Size();
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
        /// <param name="arrangeSize"></param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (this.Children.Count == 0)
            {
                return arrangeSize;
            }

            var shapeFileControl = ((FrameworkElement)this.Children[0]).FindParentElementOfType<ShapeFileLayer>();
            
            foreach (UIElement element in this.Children)
            {
                if (element == null)
                {
                    continue;
                }

                double x = 0.0;
                double y = 0.0;
                double lon = 0d;
                double lat = 0d;
                PropertyInfo childPropInfo = element.GetType().GetProperty("Latitude");
                if (childPropInfo != null)
                {
                    lat = (double)childPropInfo.GetValue(element, null);
                }
                PropertyInfo childPropInfo1 = element.GetType().GetProperty("Longitude");
                if (childPropInfo1 != null)
                {
                    lon = (double)childPropInfo1.GetValue(element, null);
                }

                var position = shapeFileControl.GetMapElementsPosition(new Point(lon, lat));
                if (!IsNaN(position.X))
                {
                    x = position.X - (element.DesiredSize.Width / 2 *shapeFileControl.ZoomFactor);
                }

                if (!IsNaN(position.Y))
                {
                    y = position.Y - (element.DesiredSize.Height / 2 * shapeFileControl.ZoomFactor);
                }

                element.Arrange(new Rect(new Point(x, y), element.DesiredSize));

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
