#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Collections.Specialized;
using System.Collections.Generic;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [ContentProperty("ToolBars")]
    [TemplatePart(Name="PART_TrayPanel",Type=typeof(TrayPanel))]

#if WPF

    [SkinType(SkinVisualStyle = Skin.Blend,
      Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Blend/BlendToolBarTrayAdvStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
     Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/VS2010/VS2010ToolBarTrayAdvStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
     Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2010Blue/Office2010BlueToolBarTrayAdvStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
      Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2010Black/Office2010BlackToolBarTrayAdvStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
      Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2010Silver/Office2010SilverToolBarTrayAdvStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2007Blue/Office2007BlueToolBarTrayAdvStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
     Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2007Black/Office2007BlackToolBarTrayAdvStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2007Silver/Office2007SilverToolBarTrayAdvStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Default/DefaultToolBarTrayAdvStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Metro/MetroToolBarTrayAdvStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Transparent/TransparentToolBarTrayAdvStyle.xaml")]
#endif
    public class ToolBarTrayAdv: Control
    {
        #region private fields

        internal List<ToolBarBand> Bands;
        TrayPanel panel;
        private bool forceArrangeCall = false;
        private bool forceMeasureCall = false;
        #endregion

        #region internal fields

        internal ToolBarManager ToolBarManager = null;

        /// <summary>
        /// 
        /// </summary>
        public bool IsHostedInToolBarManager
        {
            get
            {
                return ToolBarManager != null;
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// Initializes the new instance of ToolBarTrayAdv
        /// </summary>
        public ToolBarTrayAdv()
        {
            DefaultStyleKey = typeof(ToolBarTrayAdv);
            ToolBars = new ObservableCollection<ToolBarAdv>();
            Bands = new List<ToolBarBand>();
            SubscribeEvents();
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or Sets the ToolBars
        /// </summary>
        public ObservableCollection<ToolBarAdv> ToolBars
        {
            get { return (ObservableCollection<ToolBarAdv>)GetValue(ToolBarsProperty); }
            internal set { SetValue(ToolBarsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolBars.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ToolBarsProperty =
            DependencyProperty.Register("ToolBars", typeof(ObservableCollection<ToolBarAdv>), typeof(ToolBarTrayAdv), null);


        /// <summary>
        /// Gets or Sets a value indicating whether ToolBarTray is locked
        /// </summary>
        public bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsLocked.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.Register("IsLocked", typeof(bool), typeof(ToolBarTrayAdv), new PropertyMetadata(false));


        /// <summary>
        /// Gets or Sets the orientation of the ToolBar
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ToolBarTrayAdv), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));  

        #endregion

        #region methods

        private static void OnOrientationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ToolBarTrayAdv tray = obj as ToolBarTrayAdv;

            foreach (ToolBarAdv adv in tray.ToolBars)
            {
                adv.Orientation = tray.Orientation;
            }
        }

        internal void UpdateVisualState()
        {
            foreach (ToolBarAdv adv in ToolBars)
            {
                adv.Orientation = Orientation;
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            panel = GetTemplateChild("PART_TrayPanel") as TrayPanel;
            InsertBars();
        }

        internal void ArrangeCall(Rect rect)
        {
            forceArrangeCall = true;

            Arrange(rect);
            if (forceArrangeCall)
                ArrangeOverride(new Size(rect.Width, rect.Height));
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0;
            double y = 0;
            double height = 0;
            double width = 0;
            foreach (ToolBarBand band in Bands)
            {
                Size size = band.ArrangeToolBars(x, y);
                if (Orientation == Orientation.Horizontal)
                    y += size.Height;
                else
                    x += size.Width;
                width += size.Width;
                height += size.Height;
            }

            if (Orientation == Orientation.Horizontal)
                finalSize.Height = height;
            else
                finalSize.Width = width;
            base.ArrangeOverride(finalSize);
            forceArrangeCall = false;
            Clip = new RectangleGeometry()
            {
                Rect = new Rect(new Point(), finalSize)
            };
            return finalSize;
        }

        internal void MeasureCall(bool inValidate, Size availableSize)
        {
            forceMeasureCall = true;
                Measure(availableSize);
            if(forceMeasureCall)
                MeasureSize(availableSize);
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            forceMeasureCall = false;
            Size size = MeasureSize(availableSize);
            base.MeasureOverride(availableSize);
            return size;
        }

        private Size MeasureSize(Size availableSize)
        {
            Size size = new Size();
            CorrectBandNos();
            double width = double.IsInfinity(availableSize.Width) ? double.MaxValue : availableSize.Width;
            double height = double.IsInfinity(availableSize.Height) ? double.MaxValue : availableSize.Height;

            foreach (ToolBarBand band in Bands)
            {
                band.IsWindowResizing = true;
                Size tempSize = band.Measure(availableSize);
                size.Width += Orientation == Orientation.Horizontal ? width : tempSize.Width;
                size.Height += Orientation == Orientation.Horizontal ? tempSize.Height : height;
                width = 0;
                height = 0;
            }

            return size;
        }

        internal DockArea FindDockArea(Point point)
        {
            if (IsHostedInToolBarManager)
            {
                return ToolBarManager.FindDockArea(point);
            }
            return DockArea.None;
        } 

        internal void DockToolBar(ToolBarAdv toolBarAdv, DockArea area)
        {
            if (IsHostedInToolBarManager && area != DockArea.None 
                && area != ToolBarManager.GetDockArea(toolBarAdv.Tray))
            {
                if (toolBarAdv.Tray != null && toolBarAdv.Tray.ToolBars.Contains(toolBarAdv))
                {
                    toolBarAdv.Tray.Remove(toolBarAdv);
                    ToolBarManager.Invalidate();
                }

                ToolBarTrayAdv tray = ToolBarManager.GetToolBarTray(area);
                if(tray == null)
                    tray = new ToolBarTrayAdv();

                tray.ToolBars.Add(toolBarAdv);
                toolBarAdv.Tray = tray;

                if (ToolBarManager.GetDockArea(toolBarAdv.Tray) == DockArea.None)
                    ToolBarManager.DockTray(tray, area);
                else
                {
                    toolBarAdv.Orientation = tray.Orientation;
                    InvalidateMeasure();
                    ToolBarManager.Invalidate();
                }
            }
        }

        internal void Remove(ToolBarAdv toolBar)
        {
            CorrectBandNos();
            ToolBars.Remove(toolBar);
            ToolBarBand band = Bands[toolBar.Band];
            if (band.ToolBars.Contains(toolBar))
            {
                band.ToolBars.Remove(toolBar);
            }

            if (band.ToolBars.Count == 0)
            {
                Bands.Remove(band);
            }

            CorrectBandNos();

            if (panel.Children.Contains(toolBar))
            {
                panel.Children.Remove(toolBar);
            }
        }

        private void SubscribeEvents()
        {
            ToolBars.CollectionChanged += new NotifyCollectionChangedEventHandler(ToolBarsCollectionChanged);
        }

        void ToolBarsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
                return;
            for (int i = 0; i < e.NewItems.Count; i++)
            {
                ToolBarAdv bar = e.NewItems[0] as ToolBarAdv;
                if (bar != null && e.Action == NotifyCollectionChangedAction.Add)
                {
                    bar.Tray = this;
                    InsertBand(bar.Band, bar);
                    InsertToPanel(bar);
                }
            }

            Bands.Sort(new Comparison<ToolBarBand>(ToolBarBand.CompareBand));
            
        }

        void CorrectBandNos()
        {
            int i = 0;
            foreach (ToolBarBand band in Bands)
            {
                band.BandNo = i;

                foreach (ToolBarAdv bar in band.ToolBars)
                {
                    bar.Band = i;
                }

                ++i;
            }
        }

        void InsertToPanel(ToolBarAdv bar)
        {
            if (panel != null && !panel.Children.Contains(bar))
            {
                if (bar.Parent != null && bar.Parent is TrayPanel)
                    (bar.Parent as TrayPanel).Children.Remove(bar);
                panel.Children.Add(bar);
            }
        }

        void InsertBars()
        {
            foreach (ToolBarAdv bar in ToolBars)
            {
                InsertToPanel(bar);
            }
        }

        internal ToolBarBand GetPreviousBand(ToolBarBand band)
        {
            if (Bands.Contains(band))
            {
                int index = Bands.IndexOf(band);

                if (index != 0)
                {
                    return Bands[--index];
                }
            }

            return null;
        }

        internal ToolBarBand GetNextBand(ToolBarBand band)
        {
            if (Bands.Contains(band))
            {
                int index = Bands.IndexOf(band);

                if (index != Bands.Count - 1)
                {
                    return Bands[++index];
                }
            }

            return null;
        }

        void InsertBand(int bandNo, ToolBarAdv bar)
        {
            ToolBarBand band = GetToolBarBand(bandNo);
            if (band != null)
            {
                band.Insert(bar);
            }
            else
            {
                band = new ToolBarBand();
                band.BandNo = bar.Band;
                band.Insert(bar);
                Bands.Add(band);
            }           
        }

        internal ToolBarBand GetBandFromPoint(Point point)
        {
            foreach (ToolBarBand band in Bands)
            {
                double topValue = OrientedValue.GetOrientedTopValue(band.BoundingRectangle, Orientation);
                double bottomValue = OrientedValue.GetOrientedBottomValue(band.BoundingRectangle, Orientation);
                double pointY = OrientedValue.GetOrientedYValue(point, Orientation);

                if (topValue < pointY && bottomValue > pointY)
                {
                    return band;
                }
            }

            return null;
        }

        internal ToolBarBand TryCreateNewBand(Point point)
        {
            CorrectBandNos();

            ToolBarBand band = GetLastBand();

            if (band != null && OrientedValue.GetOrientedBottomValue(band.BoundingRectangle, Orientation) 
                <= OrientedValue.GetOrientedYValue(point, Orientation))
            {
                ToolBarBand bnd = new ToolBarBand();
                bnd.BandNo = band.BandNo + 1;
                Bands.Add(bnd);
                return bnd;
            }

            band = GetFirstBand();

            if (band != null && OrientedValue.GetOrientedTopValue(band.BoundingRectangle, Orientation)
                >= OrientedValue.GetOrientedYValue(point, Orientation))
            {
                ToolBarBand bnd = new ToolBarBand();
                bnd.BandNo = 0;
                Bands.Insert(0, bnd);
                return bnd;
            }
            CorrectBandNos();
            return null;
        }

        internal ToolBarBand GetLastBand()
        {
            if(Bands.Count > 0)
                return Bands.Last();
            return null;
        }

        internal ToolBarBand GetFirstBand()
        {
            if (Bands.Count > 0)
                return Bands.First();
            return null;
        }

        internal void MoveBarToBand(ToolBarAdv bar, ToolBarBand band, double xPos)
        {
            if (bar != null && band != null && !double.IsNaN(xPos))
            {
                int pos = band.GetPosition(xPos);
                bar.ToolBarBand.Remove(bar);
                if (bar.ToolBarBand.ToolBars.Count == 0)
                {
                    Bands.Remove(bar.ToolBarBand);
                }
                band.InsertAt(pos, bar);
                InvalidateLayout();
                if(ToolBarManager != null)
                    ToolBarManager.Invalidate();
            }
        }

        internal void InvalidateLayout()
        {
            InvalidateMeasure();
            InvalidateArrange();
            UpdateLayout();
        }

        ToolBarBand GetToolBarBand(int bandNo)
        {
            foreach(ToolBarBand band in Bands)
            {
                if(band.BandNo == bandNo)
                {
                    return band;
                }
            }

            return null;
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class ToolBarBand
    {
        /// <summary>
        /// 
        /// </summary>
        public ToolBarBand()
        {
            ToolBars = new List<ToolBarAdv>();
        }

        /// <summary>
        /// 
        /// </summary>
        public double Size
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int BandNo
        {
            get;
            set;
        }

        private bool isWindowResizing = true;

        internal bool IsWindowResizing
        {

            get
            {
                return isWindowResizing;
            }
            set
            {
                isWindowResizing = value;
            }
        }

        internal List<ToolBarAdv> ToolBars
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Rect BoundingRectangle
        {
            get;
            internal set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolBar"></param>
        public void Insert(ToolBarAdv toolBar)
        {
            toolBar.ToolBarBand = this;
            ToolBars.Add(toolBar);
            //CorrectOrder();
        }

        /// <summary>
        /// Inserts tool bar at the specified index
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="bar"></param>
        public void InsertAt(int pos, ToolBarAdv bar)
        {
            bar.ToolBarBand = this;
            if (ToolBars.Count >= pos)
            {
                ToolBars.Insert(pos, bar);
            }
            else
            {
                ToolBars.Add(bar);
            }
        }

        /// <summary>
        /// Removes the toolbar
        /// </summary>
        /// <param name="bar"></param>
        public void Remove(ToolBarAdv bar)
        {
            if (ToolBars.Contains(bar))
            {
                ToolBars.Remove(bar);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CorrectOrder()
        {
            ToolBars.Sort(new Comparison<ToolBarAdv>(CompareBandIndex));
            CorrectBandIndexes();
        }

        void CorrectBandIndexes()
        {
            int i = 0;
            foreach (ToolBarAdv bar in ToolBars)
            {
                bar.BandIndex = i;
                ++i;
            }
        }

        /// <summary>
        /// Returns index of the ToolBar by specifying the  x cordinate.
        /// </summary>
        /// <param name="xPos"></param>
        /// <returns></returns>
        public int GetPosition(double xPos)
        {
            int i = -1;

            foreach (ToolBarAdv bar in ToolBars)
            {
                if (xPos < OrientedValue.GetOrientedRightValue(bar.BoundingRectangle, bar.Orientation))
                {
                    i = ToolBars.IndexOf(bar) + 1;
                    break;
                }
            }

            if (i == -1)
            {
                i = ToolBars.Count;
            }

            return i;
        }

        private int CompareBandIndex(ToolBarAdv bar1, ToolBarAdv bar2)
        {
            if (bar1.BandIndex > bar2.BandIndex)
                return 1;
            else if (bar1.BandIndex == bar2.BandIndex)
                return 0;
            else
                return -1;            
        }

        internal Size ArrangeToolBars(double x, double y)
        {
            double width = 0;
            double x1 = ToolBars[0].Orientation == Orientation.Horizontal ? x : y;
            Size = FindMaxSize();
            double wdth = 0;
            double hght = 0;
            foreach (ToolBarAdv bar in ToolBars)
            {
#if WPF
                if (bar.Visibility == Visibility.Visible || bar.Visibility == Visibility.Hidden)
#else
                if (bar.Visibility == Visibility.Visible)
#endif
                {
                wdth = bar.Orientation == Orientation.Horizontal ? bar.GetDesiredSize().Width : Size;
                hght = bar.Orientation == Orientation.Horizontal ? Size : bar.GetDesiredSize().Height;

                double xPos = bar.Orientation == Orientation.Horizontal ? x1 : x;
                double yPos = bar.Orientation == Orientation.Horizontal ? y : x1;

                bar.BoundingRectangle = new Rect(xPos, yPos, wdth, hght);
                bar.isArranged = false;
                bar.Arrange(bar.BoundingRectangle);
                if (!bar.isArranged)
                    bar.Arrange(new Size(wdth, hght));
                width += OrientedValue.GetOrientedWidthValue(bar.GetDesiredSize(), bar.Orientation);
                x1 += OrientedValue.GetOrientedWidthValue(bar.GetDesiredSize(), bar.Orientation);
                }
            }

            wdth = ToolBars[0].Orientation == Orientation.Horizontal ? width : Size;
            hght = ToolBars[0].Orientation == Orientation.Horizontal ? Size : width;

            BoundingRectangle = new Rect(x, y, wdth, hght);

            return new Size(wdth, hght);
        }

        internal Size Measure(Size availableSize)
        {
            return Measure(availableSize, ToolBars.Count - 1);
        }

        internal Size Measure(Size availableSize, int index)
        {
            bool canContinue = true;
            Size size = availableSize;
            double desiredSize = 0;
            int pointerIndex = index;

            for (int i = 0; i <= index; i++)
            {
                ToolBars[i].ClearTempItems();
            }
            while (canContinue)
            {
                canContinue = false;

                for (int i = index; i >= 0; i--)
                {
                    if (ToolBars[i].Visibility != Visibility.Collapsed)
                    {
                        if (ToolBars[i].CanToolStripItemsMoveToOverflow)
                        {
                            size = availableSize;
                            if (i == pointerIndex)
                            {
                                double sz = OrientedValue.GetOrientedWidthValue(availableSize, ToolBars[i].Orientation)
                                    - GetSizeExceptPointerElement(pointerIndex, IsWindowResizing);

                                double wdth = ToolBars[i].Orientation == Orientation.Horizontal ? Math.Max(0.0, sz) : size.Width;
                                double hght = ToolBars[i].Orientation == Orientation.Horizontal ? size.Height : Math.Max(0.0, sz);
                                size = new Size(wdth, hght);
                            }

                            ToolBars[i].Resize(size);
                            desiredSize += OrientedValue.GetOrientedWidthValue(ToolBars[i].GetDesiredSize(), ToolBars[i].Orientation);
                            canContinue |= ToolBars[i].CanToolStripItemsMoveToOverflow;
                            canContinue &= (desiredSize > OrientedValue.GetOrientedWidthValue(availableSize, ToolBars[i].Orientation));
                            canContinue &= (i != 0 && !IsWindowResizing);
                        }
                        else if (i == 0 && !IsWindowResizing)
                        {
                            canContinue = false;
                        }
                        else
                        {
                            desiredSize += OrientedValue.GetOrientedWidthValue(ToolBars[i].GetDesiredSize()
                                , ToolBars[i].Orientation);
                        }

                        if (!ToolBars[i].CanToolStripItemsMoveToOverflow && i == pointerIndex)
                            --pointerIndex;
                    }

                    desiredSize = 0;
                }
            }

            double orientedSize = FindMaxSize();
            size = new Size();
            foreach (ToolBarAdv bar in ToolBars)
            {
                size.Width += (bar.Orientation == Orientation.Horizontal) ?
                    bar.RequiredSize.Width : orientedSize;
                size.Height += (bar.Orientation == Orientation.Horizontal) ?
                    orientedSize : bar.RequiredSize.Height;
                orientedSize = 0;
            }

            Size = OrientedValue.GetOrientedHeightValue(size, ToolBars[0].Orientation);

            return size;
        }

        private double GetSizeExceptPointerElement(int pointerIndex, bool flag)
        {
            double size = 0;
            int count = flag ? ToolBars.Count : pointerIndex;
            for (int i = 0; i < count; i++)
            {
                if (i != pointerIndex)
                {
                    size += OrientedValue.GetOrientedWidthValue(ToolBars[i].GetDesiredSize()
                        , ToolBars[i].Orientation);
                }
            }

            return size;
        }

        double FindMaxSize()
        {
            double size = 0;

            foreach (ToolBarAdv bar in ToolBars)
            {
                size = Math.Max(OrientedValue.GetOrientedHeightValue(bar.DesiredSize, bar.Orientation), size);
            }

            return size;
        }

        internal static int CompareBand(ToolBarBand bar1, ToolBarBand bar2)
        {
            if (bar1.BandNo > bar2.BandNo)
                return 1;
            else if (bar1.BandNo == bar2.BandNo)
                return 0;
            else
                return -1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OrientedValue
    {
        /// <summary>
        /// Returns oriented x value. For horizontal orientation, it will return x and for vertical it will return y.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static double GetOrientedXValue(Point point, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
                return point.X;
            return point.Y;
        }

        /// <summary>
        /// Returns the oriented value. For horizontal orientation, it will return y and for vertical it will return x.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static double GetOrientedYValue(Point point, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
                return point.Y;
            return point.X;
        }

        /// <summary>
        /// Returns the oriented value. For horizontal orientation, it will return Top and for vertical it will return Left.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static double GetOrientedTopValue(Rect rect, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
                return rect.Top;
            return rect.Left;
        }

        /// <summary>
        /// Returns the oriented value. For horizontal orientation, it will return Bottom and for vertical it will return Right.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static double GetOrientedBottomValue(Rect rect, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
                return rect.Bottom;
            return rect.Right;
        }

        /// <summary>
        /// Returns the oriented value. For horizontal orientation, it will return Left and for vertical it will return Top.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static double GetOrientedLeftValue(Rect rect, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
                return rect.Left;
            return rect.Top;
        }

        /// <summary>
        /// Returns the oriented value. For horizontal orientation, it will return Right and for vertical it will return Bottom.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static double GetOrientedRightValue(Rect rect, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
                return rect.Right;
            return rect.Bottom;
        }

        /// <summary>
        /// Returns the oriented value. For horizontal orientation, it will return Width and for vertical it will return Height.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static double GetOrientedWidthValue(Size size, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
                return size.Width;
            return size.Height;
        }

        /// <summary>
        /// Returns the oriented value. For horizontal orientation, it will return Height and for vertical it will return Width.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static double GetOrientedHeightValue(Size size, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
                return size.Height;
            return size.Width;
        }

        /// <summary>
        /// Returns the oriented value. For horizontal orientation, it will return Width and for vertical it will return Height.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static double GetOrientedWidthValue(Rect rect, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
                return rect.Width;
            return rect.Height;
        }

        /// <summary>
        /// Returns the oriented value. For horizontal orientation, it will return Height and for vertical it will return Width.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static double GetOrientedHeightValue(Rect rect, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
                return rect.Height;
            return rect.Width;
        }

        /// <summary>
        /// Returns the oriented value. For vertical orientaion, height and width will be interchanged.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static Size GetOrientedSize(double width, double height, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
                return new Size(width, height);
            return new Size(height, width);
        }
    }
}
