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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// The base container panel for the HeatMapControl and HeatMapItem. Derive your custom panels from this type.
    /// </summary>
    public class HeatMapsPanel : Panel
    {
        private double _totalWeight;
        private List<HeatMapItemMeasure> _items = new List<HeatMapItemMeasure>();

        /// <summary>
        /// Gets or sets tracks the remaining area available for children.
        /// </summary>
        protected Rect AvailableArea { get; set; }

        /// <summary>
        /// Gets returns the list of item measures sorted by weight. This list is available after a call to MeasureOverride.
        /// </summary>
        protected List<HeatMapItemMeasure> SortedItemMeasuresList
        {
            get { return this._items; }
        }

        /// <summary>
        /// Gets returns the shortest (of the remaining area) side's length.
        /// </summary>
        protected double ShorterSideLength
        {
            get
            {
                return Math.Min(this.AvailableArea.Width, this.AvailableArea.Height);
            }
        }

        /// <summary>
        /// Calls Arrange on all the "visible" child items. "Visible" items are in the <see cref="SortedItemMeasuresList"/>.
        /// </summary>
        /// <param name="arrangeSize">The available size.</param>
        /// <returns>Simply returns the arrangeSize param value.</returns>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size arrangeSize)
        {
            foreach (HeatMapItemMeasure itemMeasure in this.SortedItemMeasuresList)
            {
                if (this.IsValidSize(itemMeasure.ComputedSize))
                {
                    itemMeasure.Item.Arrange(new Rect(itemMeasure.ComputedLocation, itemMeasure.ComputedSize));
                }

                itemMeasure.Item.IsColorInfoAvailable = true;
            }

            return arrangeSize;
        }

        /// <summary>
        /// Calculates the bounds for the items (this information will be stored in the <see cref="SortedItemMeasuresList"/>).
        /// Calls Measure on all the "visible" items.
        /// </summary>
        /// <param name="constraint">The constraint within which to layout the items.</param>
        /// <returns>Returns the passed in constraint in most cases. If the constraint is Double.Infinity* (as in some cases),
        /// then it would return a size based on the HeatMapControl.PreferredItemsPanelWidth and PreferredItemsPanelHeight.</returns>
        protected override Size MeasureOverride(System.Windows.Size constraint)
        {
            HeatMapControl control = this.GetHeatMapControl();

            if (double.IsInfinity(constraint.Height))
            {
                if (control == null)
                {
                    throw new InvalidOperationException("HeatMapsPanel.MeasureOverride is being called when it's not parented by the HeatMapControl, so cannot access the PreferredItemsPanelHeight property.");
                }

                constraint.Height = control.PreferredItemsPanelHeight;
            }

            if (double.IsInfinity(constraint.Width))
            {
                if (control == null)
                {
                    throw new InvalidOperationException("HeatMapsPanel.MeasureOverride is being called when it's not parented by the HeatMapControl, so cannot access the PreferredItemsPanelWidth property.");
                }

                constraint.Width = control.PreferredItemsPanelWidth;
            }

            this.AvailableArea = new Rect(0, 0, constraint.Width, constraint.Height);
            this.PrepareChildren(control);

            double area = this.AvailableArea.Width * this.AvailableArea.Height;
            foreach (HeatMapItemMeasure itemMeasure in this.SortedItemMeasuresList)
            {
                itemMeasure.AreaByWeight = area * itemMeasure.Item.Weight / _totalWeight;
            }

            this.ComputeItemBounds();

            foreach (HeatMapItemMeasure itemMeasure in this.SortedItemMeasuresList)
            {
                if (this.IsValidSize(itemMeasure.ComputedSize))
                {
                    itemMeasure.Item.Measure(itemMeasure.ComputedSize);
                }
            }

            return constraint;
        }

        // If any dim is NaN, return false;
        private bool IsValidSize(Size size)
        {
            if (Double.IsNaN(size.Width) == false && Double.IsNaN(size.Height) == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Override this method to compute the item bounds. Update AvailableArea as you allocate space between the children
        /// </summary>
        protected virtual void ComputeItemBounds()
        {
            this.ComputeBySlicing(this.SortedItemMeasuresList);
        }

        /// <summary>
        /// Could be used for slice and dice type layouts to determine the orientation to use for children layout.
        /// </summary>
        /// <returns>Type : Orientation</returns>
        protected virtual Orientation GetOrientation()
        {
            return this.AvailableArea.Width > this.AvailableArea.Height ? Orientation.Horizontal : Orientation.Vertical;
        }

        /// <summary>
        /// Returns the rect for a child based on the orientation.
        /// </summary>
        /// <param name="orientation">To check whether its a Horizontal Orientation</param>
        /// <param name="item">To get AreaByWeight using this item.</param>
        /// <param name="x">To get x position.</param>
        /// <param name="y">To get y position.</param>
        /// <param name="width">To get width position.</param>
        /// <param name="height">To get height position.</param>
        /// <returns>Type : Rect</returns>
        protected virtual Rect GetRectangle(Orientation orientation, HeatMapItemMeasure item, double x, double y, double width, double height)
        {
            if (orientation == Orientation.Horizontal)
            {
                return new Rect(x, y, item.AreaByWeight / height, height);
            }
            else
            {
                return new Rect(x, y, width, item.AreaByWeight / width);
            }
        }

        /// <summary>
        /// Increments x or y based on the orientation.
        /// </summary>
        protected virtual void ComputeNextPosition(Orientation orientation, ref double xPos, ref double yPos, double width, double height)
        {
            if (orientation == Orientation.Horizontal)
            {
                xPos += width;
            }
            else
            {
                yPos += height;
            }
        }

        /// <summary>
        /// Slices the items based on orientation.
        /// </summary>
        protected void ComputeBySlicing(List<HeatMapItemMeasure> items)
        {
            Orientation orientation = this.GetOrientation();

            double areaSum = 0;

            foreach (HeatMapItemMeasure item in items)
            {
                areaSum += item.AreaByWeight;
            }

            Rect currentRect;
            if (orientation == Orientation.Horizontal)
            {
                currentRect = new Rect(AvailableArea.X, AvailableArea.Y, areaSum / AvailableArea.Height, AvailableArea.Height);
                AvailableArea = new Rect(AvailableArea.X + currentRect.Width, AvailableArea.Y, Math.Max(0, AvailableArea.Width - currentRect.Width), AvailableArea.Height);
            }
            else
            {
                currentRect = new Rect(AvailableArea.X, AvailableArea.Y, AvailableArea.Width, areaSum / AvailableArea.Width);
                AvailableArea = new Rect(AvailableArea.X, AvailableArea.Y + currentRect.Height, AvailableArea.Width, Math.Max(0, AvailableArea.Height - currentRect.Height));
            }

            double curX = currentRect.X;
            double curY = currentRect.Y;

            foreach (HeatMapItemMeasure item in items)
            {
                Rect rect = this.GetRectangle(orientation, item, curX, curY, currentRect.Width, currentRect.Height);

                item.ComputedSize = new Size(rect.Width, rect.Height);
                item.ComputedLocation = new Point(rect.Left, rect.Top);

                this.ComputeNextPosition(orientation, ref curX, ref curY, rect.Width, rect.Height);
            }
        }

        private HeatMapControl _control = null;
        private HeatMapControl GetHeatMapControl()
        {
            if (this._control == null)
            {
                DependencyObject parent = this;
                while (parent != null && !(parent is HeatMapControl))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }

                this._control = parent as HeatMapControl;
            }

            return this._control;
        }

        private bool IsValidItem(HeatMapItem item)
        {
            return !double.IsNaN(item.Weight) && Math.Round(item.Weight, 0) != 0;
        }

        /// <summary>
        /// Method that determines whether the color should be processed.
        /// </summary>
        /// <param name="item">item is passed in this function.</param>
        /// <returns>Type : bool</returns>
        protected bool ShouldProcessColorWeights(HeatMapItem item)
        {
            HeatMapControl control = this.GetHeatMapControl();
            if (control != null)
            {
                return true;    ////Needs to come back here
            }
            else
            {
                return true;    ////item.Level == control.ColorCalculationLevel;
            }
        }

        private void SetupContentBinding(HeatMapItem heatmapitem, string sourceProp, DependencyProperty destProp)
        {
            if (heatmapitem.Header != null)
            {
                heatmapitem.DataContext = heatmapitem.Header;
                Binding binding = new Binding();
                binding.Source = heatmapitem.Header;
                binding.Path = new PropertyPath(sourceProp);
                heatmapitem.SetBinding(destProp, binding);
            }
        }

        private static HeatMapItem GetHeatMapItem(object elem)
        {
            if (elem is UIElement)
            {
                UIElement parent = (UIElement)VisualTreeHelper.GetParent((UIElement)elem);
                if (parent != null)
                {
                    if (parent.GetType() == typeof(HeatMapItem))
                    {
                        return (HeatMapItem)parent;
                    }
                    else
                    {
                        return GetHeatMapItem(parent);
                    }
                }
            }

            return null;
        }

        private void PrepareChildren(HeatMapControl control)
        {
            this._totalWeight = 0;
            this.SortedItemMeasuresList.Clear();

            if (control == null)
            {
                throw new Exception("HeatMapsPanel used outside the context of HeatMapControl.");
            }
            else
            {
                foreach (UIElement child in this.Children)
                {
                    HeatMapItem item = child as HeatMapItem;

                    if (item != null)
                    {
                        double tempdouble = item.ColorWeight;
                        bool tempbool = item.ColorWeight == double.NaN;
                        bool tempbool1 = tempdouble.ToString() == double.NaN.ToString();
                        if (tempdouble.ToString() == double.NaN.ToString())
                        {
                            HeatMapItem parentHeatmapitem = GetHeatMapItem(item);
                            this.SetupContentBinding(item, parentHeatmapitem.WeightValuePath, HeatMapItem.WeightProperty);
                            this.SetupContentBinding(item, parentHeatmapitem.ColorWeightValuePath, HeatMapItem.ColorWeightProperty);
                        }
                    }

                    if (item == null)
                    {
                        throw new Exception("HeatMapsPanel is being populated by something other than HeatMapItems.");
                    }

                    ColorWeightsInfo clrWeightsInfo = null;

                    if (this.ShouldProcessColorWeights(item))
                    {
                        clrWeightsInfo = this.GetHeatMapControl().ColorWeightsInfo;
                        if (clrWeightsInfo.LowestValue > item.ColorWeight)
                        {
                            clrWeightsInfo.LowestValue = item.ColorWeight;
                        }

                        if (clrWeightsInfo.HighestValue < item.ColorWeight)
                        {
                            clrWeightsInfo.HighestValue = item.ColorWeight;
                        }
                    }

                    HeatMapItemMeasure itemMeasure = new HeatMapItemMeasure(item, clrWeightsInfo);

                    if (this.IsValidItem(item))
                    {
                        item.Visibility = Visibility.Visible;

                        _totalWeight += item.Weight;
                        this.SortedItemMeasuresList.Add(itemMeasure);
                    }
                    else
                    {
                        item.Visibility = Visibility.Collapsed;
                    }
                }
            }

            this.SortedItemMeasuresList.Sort(HeatMapItemMeasure.CompareByWeightDescendingOrder);
        }
    }

    /// <summary>
    /// Panel that simply lays out it's children horizontally
    /// </summary>
    public class HorizontalSlicesPanel : HeatMapsPanel
    {
        /// <summary>
        /// Override the Orientation.
        /// </summary>
        /// <returns>Returns Orientation.Horizontal.</returns>
        protected override Orientation GetOrientation()
        {
            return Orientation.Horizontal;
        }
    }

    /// <summary>
    /// Panel that simply lays out it's children horizontally
    /// </summary>
    public class VerticalSlicesPanel : HeatMapsPanel
    {
        /// <summary>
        /// Override the Orientation.
        /// </summary>
        /// <returns>Returns Orientation.Horizontal.</returns>
        protected override Orientation GetOrientation()
        {
            return Orientation.Vertical;
        }
    }

    /// <summary>
    /// Provides additional information about a HeatMapItem that you can use within your DataTemplates. 
    /// See HeatMapItem.ItemMeasure property class reference for more info on how to use this type.
    /// </summary>
    public class HeatMapItemMeasure : IDisposable
    {
        private HeatMapItem item;
        private ColorWeightsInfo colorWeightsInfo;

        internal HeatMapItemMeasure(HeatMapItem item, ColorWeightsInfo colorWeightsInfo)
        {
            this.item = item;
            this.item.ItemMeasure = this;

            this.colorWeightsInfo = colorWeightsInfo;
        }

        /// <summary>
        /// Dispose the Item Measure
        /// </summary>
        public void Dispose()
        {
            if (this.item.ItemMeasure == this)
            {
                this.item.ItemMeasure = null;
            }
        }

        /// <summary>
        /// Gets the underlying HeatMapItem instance.
        /// </summary>
        public HeatMapItem Item
        {
            get
            {
                return this.item;
            }
        }

        /// <summary>
        /// Gets or sets Computed size for the item.
        /// </summary>
        public Size ComputedSize { get; set; }

        /// <summary>
        /// Gets or sets computed Location for the item.
        /// </summary>
        public Point ComputedLocation { get; set; }

        internal ColorWeightsInfo ColorWeightsInfo
        {
            get
            {
                return this.colorWeightsInfo;
            }
        }

        /// <summary>
        /// Gets or sets calculated area occupied by the item based on it's Weight and the size of the container.
        /// </summary>
        public double AreaByWeight { get; set; }

        /// <summary>
        /// Used by the implementation to compare the weights of 2 items.
        /// </summary>
        /// <param name="x">passing x value to check and return value.</param>
        /// <param name="y">passing y value to check and return value.</param>
        /// <returns>Type : int</returns>
        public static int CompareByWeightDescendingOrder(HeatMapItemMeasure x, HeatMapItemMeasure y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    return x.Item.Weight.CompareTo(y.Item.Weight) * -1;
                }
            }
        }
    }

    /// <summary>
    /// Global info pertaning to the full bound list.
    /// </summary>
    public class ColorWeightsInfo
    {
        /// <summary>
        /// Constructor to Clear Low and High.
        /// </summary>
        public ColorWeightsInfo()
        {
            this.ClearLowAndHigh();
        }

        /// <summary>
        /// Gets or sets Lowest value in the bound list.
        /// </summary>
        public double LowestValue { get; set; }

        /// <summary>
        /// Gets or sets highest value in the bound list.
        /// </summary>
        public double HighestValue { get; set; }

        /// <summary>
        /// Returns a value between the <see cref="LowestValue"/> and <see cref="HighestValue"/> based on the percentile specified.
        /// </summary>
        /// <param name="percentile">A value between 0 and 100.</param>
        /// <returns>A value between LowestValue and HighestValue.</returns>
        public double GetWeightValueForPercentile(int percentile)
        {
            if (percentile < 0 || percentile > 100)
            {
                throw new Exception("The argument should be within 0 to 100. Encountered " + percentile.ToString());
            }

            return this.LowestValue + ((this.HighestValue - this.LowestValue) * ((double)percentile / (double)100));
        }

        /// <summary>
        /// Clears the <see cref="LowestValue"/> setting and the <see cref="HighestValue"/> setting. These will be recalculated
        /// the next time the HeatMapItem's layout is updated.
        /// </summary>
        protected void ClearLowAndHigh()
        {
            this.LowestValue = double.PositiveInfinity;
            this.HighestValue = 0;
        }

        /// <summary>
        /// Method that calculates the color by accepting the value.
        /// </summary>
        /// <param name="value">Represents value based on which the color is determined.</param>
        /// <param name="lowColor">lowest color value</param>
        /// <param name="medianColor">Median color value</param>
        /// <param name="highColor">Highest Color value.</param>
        /// <param name="median">Median Value</param>
        /// <returns>Type : Color</returns>
        public Color GetColorFromValue(double value, Color lowColor, Color medianColor, Color highColor, int median)
        {
            if (value < LowestValue)
            {
                return lowColor;
            }

            if (value > HighestValue)
            {
                return highColor;
            }

            double range = HighestValue - LowestValue;

            double valuePercentage = (value - LowestValue) / range;

            valuePercentage *= 100;

            if (valuePercentage < median)
            {
                valuePercentage = (valuePercentage / median) * 100;
                return GetColorFromPerc(lowColor, medianColor, valuePercentage);
            }
            else
            {
                valuePercentage = ((valuePercentage - median) / (100 - median)) * 100;
                return GetColorFromPerc(medianColor, highColor, valuePercentage);
            }
        }

        private Color GetColorFromPerc(Color left, Color right, double per)
        {
            if (per > 99)
            {
                per = 99;
            }

            int a = (int)((right.A - left.A) * per / (double)100) + left.A;
            int r = (int)((right.R - left.R) * per / (double)100) + left.R;
            int g = (int)((right.G - left.G) * per / (double)100) + left.G;
            int b = (int)((right.B - left.B) * per / (double)100) + left.B;

            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }
    }
}