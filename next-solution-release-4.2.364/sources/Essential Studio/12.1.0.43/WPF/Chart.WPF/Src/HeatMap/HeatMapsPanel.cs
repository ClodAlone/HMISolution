// <copyright file="HeatMapsPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;   
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;   
    using System.Windows.Media;
    using System.Windows.Media.Imaging; 
    using System.Windows.Shapes; 

    /// <summary>
    /// The base container panel for the HeatMapControl and HeatMapItem. Derive your custom panels from this type.
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class HeatMapsPanel : Canvas
    {
        /// <summary>
        /// Initializes _totalWeight
        /// </summary>
        private double m_totalWeight;

        /// <summary>
        /// Initializes _items
        /// </summary>
        private List<HeatMapItemMeasure> m_items = new List<HeatMapItemMeasure>();

        /// <summary>
        /// Gets or sets the remaining area available for children.
        /// </summary>
        protected Rect AvailableArea { get; set; }

        /// <summary>
        /// Gets the list of item measures sorted by weight. This list is available after a call to MeasureOverride.
        /// </summary>
        protected List<HeatMapItemMeasure> SortedItemMeasuresList
        {
            get { return this.m_items; }
        }

        /// <summary>
        /// Gets the shortest (of the remaining area) side's length.
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
        /// calculates the bounds for the items (this information will be stored in the <see cref="SortedItemMeasuresList"/>).
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
                itemMeasure.AreaByWeight = area * itemMeasure.Item.Weight / m_totalWeight;
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
        /// <returns>The orientation</returns>
        protected virtual Orientation GetOrientation()
        {
            return this.AvailableArea.Width > this.AvailableArea.Height ? Orientation.Horizontal : Orientation.Vertical;
        }

        /// <summary>
        /// Returns the rect for a child based on the orientation.
        /// </summary>
        /// <param name="orientation">The orientation</param>
        /// <param name="item">The item value</param>
        /// <param name="x">The x value</param>
        /// <param name="y">The y value</param>
        /// <param name="width">The width value</param>
        /// <param name="height">The height value</param>
        /// <returns>The rectangle</returns>
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
        /// <param name="orientation">The orientation.</param>
        /// <param name="xPos">The X position.</param>
        /// <param name="yPos">The Y position.</param>
        /// <param name="width">The width value</param>
        /// <param name="height">The height value</param>
        /// <remarks></remarks>
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
        /// <param name="items">The HeatMapItemMeasure items</param>
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

                item.ComputedSize = rect.Size;
                item.ComputedLocation = rect.Location;

                this.ComputeNextPosition(orientation, ref curX, ref curY, rect.Width, rect.Height);
            }
        }

        /// <summary>
        /// Method to indicate whether color Weights should be processed
        /// </summary>
        /// <param name="item">The HeatMapItem item</param>
        /// <returns>The bool value whether to process color weights</returns>
        protected bool ShouldProcessColorWeights(HeatMapItem item)
        {
            HeatMapControl control = this.GetHeatMapControl();
            if (control != null)
            {
                if (control.ColorCalculationLevel == -1)
                {
                    return item.HasItems == false;
                }
                else
                {
                    return item.Level == control.ColorCalculationLevel;
                }
            }

            return false;
        }

        /// <summary>
        /// The IsValidSize method
        /// </summary>
        /// <param name="size">The size value</param>
        /// <returns>If any dim is NaN, return false;</returns>
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
        /// Initializes HeatMapControl _control
        /// </summary>
        private HeatMapControl m_control = null;

        /// <summary>
        /// The GetHeatMapControl method
        /// </summary>
        /// <returns>The HeatMapControl</returns>
        private HeatMapControl GetHeatMapControl()
        {
            if (this.m_control == null)
            {
                DependencyObject parent = this;
                while (parent != null && !(parent is HeatMapControl))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }

                this.m_control = parent as HeatMapControl;
            }

            return this.m_control;
        }
            
        /// <summary>
        /// The IsValidItem method
        /// </summary>
        /// <param name="item">The HeatMapItem item.</param>
        /// <returns>bool value indicating whether valid item</returns>
        private bool IsValidItem(HeatMapItem item)
        {
            return !double.IsNaN(item.Weight) && Math.Round(item.Weight, 2) != 0;
        }

        /// <summary>
        /// The PrepareChildren method
        /// </summary>
        /// <param name="control">The HeatMapControl control</param>
        private void PrepareChildren(HeatMapControl control)
        {
            this.m_totalWeight = 0;
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

                    if (this.IsValidItem(item) && item.ItemVisibility == Visibility.Visible)
                    {
                        //By default it is always visible.
                        item.Visibility = Visibility.Visible;

                        m_totalWeight += item.Weight;
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
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class HorizontalSlicesPanel : HeatMapsPanel
    {
        /// <summary>
        /// The GetOrientation method
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
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class VerticalSlicesPanel : HeatMapsPanel
    {
        /// <summary>
        /// The GetOrientation method
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
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class HeatMapItemMeasure : IDisposable
    {
        /// <summary>
        /// Initializes HeatMapItem item
        /// </summary>
        private HeatMapItem item;

        /// <summary>
        /// Initializes colorWeightsInfo
        /// </summary>
        private ColorWeightsInfo colorWeightsInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Chart.HeatMapItemMeasure">HeatMapItemMeasure</see> class. 
        /// </summary>
        /// <param name="item">The HeatMapItem item</param>
        /// <param name="colorWeightsInfo">ColorWeightsInfo colorWeightsInfo</param>
        internal HeatMapItemMeasure(HeatMapItem item, ColorWeightsInfo colorWeightsInfo)
        {
            this.item = item;
            this.item.ItemMeasure = this;

            this.colorWeightsInfo = colorWeightsInfo;
        }
        
        /// <summary>
        /// Used by the implementation to compare the weights of 2 items.
        /// </summary>
        /// <param name="x">The x value</param>
        /// <param name="y">The y value</param>
        /// <returns>The int order</returns>
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
 
        /// <summary>
        /// Clean up any resources being used.
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
        /// <value>The item value</value>
       public HeatMapItem Item 
       {
           get
           { 
               return this.item; 
           }
       }

       /// <summary>
       /// Gets or sets the Computed size for the item.
       /// </summary>
       /// <value>The computed size</value>
       public Size ComputedSize 
       { 
           get; 
           set; 
       }
        
       /// <summary>
       /// Gets or sets the Computed Location for the item.
       /// </summary>
       /// <value>The computed location point</value>
       /// <remarks></remarks>
        public Point ComputedLocation { get; set; }

        /// <summary>
        /// Gets the ColorWeightsInfo
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        internal ColorWeightsInfo ColorWeightsInfo 
        { 
            get 
            { 
                return this.colorWeightsInfo;
            }
        }
        
        /// <summary>
        /// Gets or sets the calculated area occupied by the item based on it's Weight and the size of the container.
        /// </summary>
        /// <value>The Area by weight</value>
        /// <remarks></remarks>
        public double AreaByWeight 
        { 
            get; 
            set; 
        }
         }

    /// <summary>
    /// Global info pertaining to the full bound list.
    /// </summary>
    /// <seealso cref="ColorWeightsInfo"/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ColorWeightsInfo
    {
        /// <summary>
        /// Initializes RenderTargetBitmap rtb
        /// </summary>
        /// <remarks></remarks>
        private static RenderTargetBitmap rtb;

        /// <summary>
        /// Initializes static members of the <see cref="T:Syncfusion.Windows.Chart.ColorWeightsInfo">ColorWeightsInfo</see> class. 
        /// </summary>
        /// <remarks></remarks>
        static ColorWeightsInfo()
        {
            rtb = new RenderTargetBitmap(100, 1, 100, 100, PixelFormats.Pbgra32);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Chart.ColorWeightsInfo">ColorWeightsInfo</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public ColorWeightsInfo()
        {
            this.ClearLowAndHigh();
        }
        
        /// <summary>
        /// Gets or sets the lowest value in the bound list.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double LowestValue { get; set; }
        
        /// <summary>
        /// Gets or sets the highest value in the bound list.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
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
        /// Return brush value from the given double value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lowColor"></param>
        /// <param name="medianColor"></param>
        /// <param name="highColor"></param>
        /// <param name="median"></param>
        /// <returns></returns>
        public Brush GetGradientFromValue(double value, Brush lowColor, Brush medianColor, Brush highColor, int median)
        {
            //BrushConverter conv = new BrushConverter();
                     

            LinearGradientBrush lowbrush = lowColor as LinearGradientBrush;
            LinearGradientBrush medbrush = medianColor as LinearGradientBrush;
            LinearGradientBrush highbrush = highColor as LinearGradientBrush;

            LinearGradientBrush gradientbrush = new LinearGradientBrush();
            gradientbrush.GradientStops.Add(new GradientStop() { Color = Colors.Transparent, Offset = 1 });
            if (lowbrush == null)
            {               
                lowbrush = gradientbrush as LinearGradientBrush;               
            }
            if (medbrush == null)
            {
                medbrush = gradientbrush as LinearGradientBrush;
            }
            if(highbrush == null)
            {
                highbrush = gradientbrush as LinearGradientBrush;
            }
            if (this.LowestValue == this.HighestValue)
            {
                return lowbrush;
            }

            if (value <= LowestValue)
            {
                return lowbrush;
            }

            if (value >= HighestValue)
            {
                return highbrush;
            }
         
            double MedianValue= (HighestValue+ LowestValue)/2;
            if (value == MedianValue)
            {
                return medbrush;
            }
            if (value > LowestValue && value < MedianValue)
            {
                if (value < ((MedianValue + LowestValue) / 2))
                {
                    #region Low
                    LinearGradientBrush lowbrush1 = lowbrush as LinearGradientBrush;
                    LinearGradientBrush bru = new LinearGradientBrush();
                    for (int i = (lowbrush1.GradientStops.Count-1); i >= 0; i--)
                    {
                        LinearGradientBrush brush = new LinearGradientBrush();
                        brush.StartPoint = new Point(0, 0.5);
                        brush.EndPoint = new Point(1, 0.5);
                      

                        brush.GradientStops.Add(new GradientStop() { Color = lowbrush1.GradientStops[i].Color, Offset = 0 });
                        if (i <= (medbrush.GradientStops.Count - 1))
                        {
                            brush.GradientStops.Add(new GradientStop() { Color = medbrush.GradientStops[i].Color, Offset = (double)median / 100 });
                        }
                        else
                        {
                            int j = (medbrush.GradientStops.Count - 1);
                            brush.GradientStops.Add(new GradientStop() { Color = medbrush.GradientStops[j].Color, Offset = (double)median / 100 });
                        }

                        if (i <= (highbrush.GradientStops.Count - 1))
                        {
                            brush.GradientStops.Add(new GradientStop() { Color = highbrush.GradientStops[i].Color, Offset = 1 });
                        }
                        else
                        {
                            int j = (highbrush.GradientStops.Count - 1);
                            brush.GradientStops.Add(new GradientStop() { Color = highbrush.GradientStops[j].Color, Offset = 1 });
                        }
                        

                        Rectangle rect = new Rectangle();
                        rect.Fill = brush;
                        rect.Measure(new Size(100, 1));
                        rect.Arrange(new Rect(0, 0, 100, 1));

                        // The rtb should also be 100 X 1
                        rtb.Render(rect);

                        // Calculate offset into the above rect for the point whose color value we are intrested in.
                        double range = HighestValue - LowestValue;

                        double valuePercentage = (value - LowestValue) / range;

                        valuePercentage *= 100;

                        if (valuePercentage > 99)
                        {
                            valuePercentage = 99;
                        }

                        int[] pixelData = new int[1];
                        rtb.CopyPixels(new Int32Rect((int)valuePercentage, 0, 1, 1), pixelData, 4, 0);
                        int color = pixelData[0];

                        byte a = (byte)((color & 0xff000000) >> 24);
                        byte r = (byte)((color & 0x00ff0000) >> 16);
                        byte g = (byte)((color & 0x0000ff00) >> 8);
                        byte b = (byte)(color & 0x000000ff);
                        Color col = Color.FromArgb(a, r, g, b);
                        bru.GradientStops.Add(new GradientStop(col, lowbrush1.GradientStops[i].Offset));
                    }
                 
                        
                  return bru;
                  
                    #endregion
                }
                else
                {
                    #region LowMedium
                    LinearGradientBrush medbrush1 = medbrush as LinearGradientBrush;
                    LinearGradientBrush bru = new LinearGradientBrush();
                    for (int i = (medbrush1.GradientStops.Count - 1); i >= 0; i--)
                    {
                        LinearGradientBrush brush = new LinearGradientBrush();
                        brush.StartPoint = new Point(0, 0.5);
                        brush.EndPoint = new Point(1, 0.5);

                        if (i <= (lowbrush.GradientStops.Count - 1))
                        {
                            brush.GradientStops.Add(new GradientStop() { Color = lowbrush.GradientStops[i].Color, Offset = 0 });
                        }
                        else
                        {
                            int j = (lowbrush.GradientStops.Count - 1);
                            brush.GradientStops.Add(new GradientStop() { Color = lowbrush.GradientStops[j].Color, Offset = 0 });
                        }   
                     
                        brush.GradientStops.Add(new GradientStop() { Color = medbrush1.GradientStops[i].Color, Offset = (double)median / 100 });

                        if(i<=(highbrush.GradientStops.Count-1))
                        {
                            brush.GradientStops.Add(new GradientStop() { Color = highbrush.GradientStops[i].Color, Offset = 1 });
                        }
                        else
                        {
                            int j = (highbrush.GradientStops.Count-1);
                            brush.GradientStops.Add(new GradientStop() { Color = highbrush.GradientStops[j].Color, Offset = 1 });
                        }
                    
                        Rectangle rect = new Rectangle();
                        rect.Fill = brush;
                        rect.Measure(new Size(100, 1));
                        rect.Arrange(new Rect(0, 0, 100, 1));

                        // The rtb should also be 100 X 1
                        rtb.Render(rect);

                        // Calculate offset into the above rect for the point whose color value we are intrested in.
                        double range = HighestValue - LowestValue;

                        double valuePercentage = (value - LowestValue) / range;

                        valuePercentage *= 100;

                        if (valuePercentage > 99)
                        {
                            valuePercentage = 99;
                        }

                        int[] pixelData = new int[1];
                        rtb.CopyPixels(new Int32Rect((int)valuePercentage, 0, 1, 1), pixelData, 4, 0);
                        int color = pixelData[0];

                        byte a = (byte)((color & 0xff000000) >> 24);
                        byte r = (byte)((color & 0x00ff0000) >> 16);
                        byte g = (byte)((color & 0x0000ff00) >> 8);
                        byte b = (byte)(color & 0x000000ff);
                        Color col = Color.FromArgb(a, r, g, b);
                        bru.GradientStops.Add(new GradientStop(col, medbrush1.GradientStops[i].Offset));
                    }                
                                 
                    return bru;            

                    #endregion
                }
            }
            if (value < HighestValue && value > MedianValue)
            {
                double highmedian = ((HighestValue - MedianValue) / 2);
                if (value < ((HighestValue + MedianValue) / 2))
                {
                    #region High
                    LinearGradientBrush highbrush1 = highbrush as LinearGradientBrush;
                    LinearGradientBrush bru = new LinearGradientBrush();
                    for (int i = (highbrush1.GradientStops.Count - 1); i >= 0; i--)
                    {
                        LinearGradientBrush brush = new LinearGradientBrush();
                        brush.StartPoint = new Point(0, 0.5);
                        brush.EndPoint = new Point(1, 0.5);

                        if (i <= (lowbrush.GradientStops.Count - 1))
                        {
                            brush.GradientStops.Add(new GradientStop() { Color = lowbrush.GradientStops[i].Color, Offset = 0 });
                        }
                        else
                        {
                            int j = (lowbrush.GradientStops.Count - 1);
                            brush.GradientStops.Add(new GradientStop() { Color = lowbrush.GradientStops[j].Color, Offset = 0 });
                        }
                        if (i <= (medbrush.GradientStops.Count - 1))
                        {
                            brush.GradientStops.Add(new GradientStop() { Color = medbrush.GradientStops[i].Color, Offset = (double)median / 100 });
                        }
                        else
                        {
                            int j = (medbrush.GradientStops.Count - 1);
                            brush.GradientStops.Add(new GradientStop() { Color = medbrush.GradientStops[j].Color, Offset = (double)median / 100 });
                        }
                      
                        brush.GradientStops.Add(new GradientStop() { Color = highbrush1.GradientStops[i].Color, Offset = 1 });
                        Rectangle rect = new Rectangle();
                        rect.Fill = brush;
                        rect.Measure(new Size(100, 1));
                        rect.Arrange(new Rect(0, 0, 100, 1));

                        // The rtb should also be 100 X 1
                        rtb.Render(rect);

                        // Calculate offset into the above rect for the point whose color value we are intrested in.
                        double range = HighestValue - LowestValue;

                        double valuePercentage = (value - LowestValue) / range;

                        valuePercentage *= 100;

                        if (valuePercentage > 99)
                        {
                            valuePercentage = 99;
                        }

                        int[] pixelData = new int[1];
                        rtb.CopyPixels(new Int32Rect((int)valuePercentage, 0, 1, 1), pixelData, 4, 0);
                        int color = pixelData[0];

                        byte a = (byte)((color & 0xff000000) >> 24);
                        byte r = (byte)((color & 0x00ff0000) >> 16);
                        byte g = (byte)((color & 0x0000ff00) >> 8);
                        byte b = (byte)(color & 0x000000ff);
                        Color col = Color.FromArgb(a, r, g, b);
                        bru.GradientStops.Add(new GradientStop(col, highbrush1.GradientStops[i].Offset));
                    }
                    return bru;
                   
                    #endregion;
                }
                else
                {
                    #region HighMedian
                    LinearGradientBrush medbrush1 = medbrush as LinearGradientBrush;
                    LinearGradientBrush bru = new LinearGradientBrush();
                    for (int i = (medbrush1.GradientStops.Count-1); i >= 0; i--)
                    {
                        LinearGradientBrush brush = new LinearGradientBrush();
                        brush.StartPoint = new Point(0, 0.5);
                        brush.EndPoint = new Point(1, 0.5);

                        if (i <= (lowbrush.GradientStops.Count - 1))
                        {
                            brush.GradientStops.Add(new GradientStop() { Color = lowbrush.GradientStops[i].Color, Offset = 0 });
                        }
                        else
                        {
                            int j = (lowbrush.GradientStops.Count - 1);
                            brush.GradientStops.Add(new GradientStop() { Color = lowbrush.GradientStops[j].Color, Offset = 0 });
                        }
                        
                        brush.GradientStops.Add(new GradientStop() { Color = medbrush1.GradientStops[i].Color, Offset = (double)median / 100 });

                        if (i <= (highbrush.GradientStops.Count - 1))
                        {
                            brush.GradientStops.Add(new GradientStop() { Color = highbrush.GradientStops[i].Color, Offset = 1 });
                        }
                        else
                        {
                            int j = (highbrush.GradientStops.Count - 1);
                            brush.GradientStops.Add(new GradientStop() { Color = highbrush.GradientStops[j].Color, Offset = 1 });
                        }                      

                        Rectangle rect = new Rectangle();
                        rect.Fill = brush;
                        rect.Measure(new Size(100, 1));
                        rect.Arrange(new Rect(0, 0, 100, 1));

                        // The rtb should also be 100 X 1
                        rtb.Render(rect);

                        // Calculate offset into the above rect for the point whose color value we are intrested in.
                        double range = HighestValue - LowestValue;

                        double valuePercentage = (value - LowestValue) / range;

                        valuePercentage *= 100;

                        if (valuePercentage > 99)
                        {
                            valuePercentage = 99;
                        }

                        int[] pixelData = new int[1];
                        rtb.CopyPixels(new Int32Rect((int)valuePercentage, 0, 1, 1), pixelData, 4, 0);
                        int color = pixelData[0];

                        byte a = (byte)((color & 0xff000000) >> 24);
                        byte r = (byte)((color & 0x00ff0000) >> 16);
                        byte g = (byte)((color & 0x0000ff00) >> 8);
                        byte b = (byte)(color & 0x000000ff);
                        Color col = Color.FromArgb(a, r, g, b);
                        bru.GradientStops.Add(new GradientStop(col, medbrush1.GradientStops[i].Offset));
                    }               
                        return bru;

                    #endregion
                }               
            }
            return lowColor;   
          
            
        }
        /// <summary>
        /// Provides the color for a weight value based on the low, median and high colors.
        /// </summary>
        /// <param name="value">The weight value that is between a low and high range.</param>
        /// <param name="lowColor">The color for the lowest value in the range.</param>
        /// <param name="medianColor">The color for the median value in the range.</param>
        /// <param name="highColor">The color for the high value in the range.</param>
        /// <param name="median">The median percentage (a value between 0 and 100).</param>
        /// <returns>The color value</returns>
        public Color GetColorFromValue(double value, Color lowColor, Color medianColor, Color highColor, int median)
        {
            if (this.LowestValue == this.HighestValue)
            {
                return lowColor;
            }

            if (value < LowestValue)
            {
                return lowColor;
            }

            if (value > HighestValue)
            {
                return highColor;
            }

            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0.5);
            brush.EndPoint = new Point(1, 0.5);
            brush.GradientStops.Add(new GradientStop() { Color = lowColor, Offset = 0 });
            brush.GradientStops.Add(new GradientStop() { Color = medianColor, Offset = (double)median / 100 });
            brush.GradientStops.Add(new GradientStop() { Color = highColor, Offset = 1 });
             
            Rectangle rect = new Rectangle();
            rect.Fill = brush;
            rect.Measure(new Size(100, 1));
            rect.Arrange(new Rect(0, 0, 100, 1));

            // The rtb should also be 100 X 1
            rtb.Render(rect);

            // Calculate offset into the above rect for the point whose color value we are intrested in.
            double range = HighestValue - LowestValue;

            double valuePercentage = (value - LowestValue) / range;

            valuePercentage *= 100;

            if (valuePercentage > 99)
            {
                valuePercentage = 99;
            }

            int[] pixelData = new int[1];
            rtb.CopyPixels(new Int32Rect((int)valuePercentage, 0, 1, 1), pixelData, 4, 0);
            int color = pixelData[0];

            byte a = (byte)((color & 0xff000000) >> 24);
            byte r = (byte)((color & 0x00ff0000) >> 16);
            byte g = (byte)((color & 0x0000ff00) >> 8);
            byte b = (byte)(color & 0x000000ff);
            return Color.FromArgb(a, r, g, b);
        }
     
        /// <summary>
        /// Clears the <see cref="LowestValue"/> setting and the <see cref="HighestValue"/> setting. These will be recalculated
        /// the next time the HeatMapItem's layout is updated.
        /// </summary>
        /// <seealso cref="ColorWeightsInfo"/>
        protected void ClearLowAndHigh()
        {
            this.LowestValue = double.PositiveInfinity;
            this.HighestValue = 0;
        }

         #region Color calculation using System.Drawing.Color

        /// <summary>
        /// The GetColorFromValueX method. Currently not used.
        /// </summary>
        /// <param name="value">The double value</param>
        /// <param name="lowColor">The Color lowColor</param>
        /// <param name="medianColor">The Color medianColor</param>
        /// <param name="highColor">The Color highColor</param>
        /// <param name="median">The int median</param>
        /// <returns>The color value</returns>
        private Color GetColorFromValueX(double value, Color lowColor, Color medianColor, Color highColor, int median)
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

        /// <summary>
        /// The get color from percentage method.
        /// </summary>
        /// <param name="left">The left color</param>
        /// <param name="right">The right color</param>
        /// <param name="per">The double percentage</param>
        /// <returns>The color value</returns>
        private Color GetColorFromPerc(Color left, Color right, double per)
        {
            if (per > 99)
            {
                per = 99;
            }

            System.Drawing.Color cLeft = System.Drawing.Color.FromArgb(left.A, left.R, left.G, left.B);
            System.Drawing.Color cRight = System.Drawing.Color.FromArgb(right.A, right.R, right.G, right.B);

            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.Point(0, 0), new System.Drawing.Point(99, 0), cRight, cLeft);

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(100, 1);
            System.Drawing.Graphics gph = System.Drawing.Graphics.FromImage(bmp);
            gph.FillRectangle(brush, new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), bmp.Size));
            System.Drawing.Color c = bmp.GetPixel((int)per, (int)0);

            return new Color() { A = c.A, B = c.B, R = c.R, G = c.G };
        }
        #endregion
    }
}
