#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Diagnostics;
using System.Collections.Generic;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [ContentProperty("Content")]
    [StyleTypedProperty(Property="FloatingToolBarStyle", StyleTargetType=typeof(FloatingToolBar))]
#if WPF
    [SkinType(SkinVisualStyle = Skin.Blend,
      Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Blend/BlendToolBarManagerStyle.xaml")]  
    [SkinType(SkinVisualStyle = Skin.VS2010,
     Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/VS2010/VS2010ToolBarManagerStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
     Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2010Blue/Office2010BlueToolBarManagerStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
      Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2010Black/Office2010BlackToolBarManagerStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
      Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2010Silver/Office2010SilverToolBarManagerStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2007Blue/Office2007BlueToolBarManagerStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
     Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2007Black/Office2007BlackToolBarManagerStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2007Silver/Office2007SilverToolBarManagerStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Default/DefaultToolBarManagerStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
        Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Metro/MetroToolBarManagerStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
        Type = typeof(ToolBarManager), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Transparent/TransparentToolBarManagerStyle.xaml")]
#endif
    public class ToolBarManager: Control
    {
        #region private fields

        internal ContentControl content = null;
        private ToolBarManagerPanel panel = null;
        private bool needToInvalidate = true;
        private Size currentSize;
        internal List<FloatingToolBar> FloatingToolBars;

        #endregion

        #region Dependecy Properties

        /// <summary>
        /// Gets or Sets the ToolBarTrayAdv displayed at the left side of ToolBarManager
        /// </summary>
        public ToolBarTrayAdv LeftToolBarTray
        {
            get { return (ToolBarTrayAdv)GetValue(LeftToolBarTrayProperty); }
            set { SetValue(LeftToolBarTrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftToolBarTray.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LeftToolBarTrayProperty =
            DependencyProperty.Register("LeftToolBarTray", typeof(ToolBarTrayAdv), typeof(ToolBarManager), new PropertyMetadata(OnLeftTrayChanged));

        /// <summary>
        /// Gets or Sets the ToolBarTrayAdv displayed at the Right side of ToolBarManager
        /// </summary>
        public ToolBarTrayAdv RightToolBarTray
        {
            get { return (ToolBarTrayAdv)GetValue(RightToolBarTrayProperty); }
            set { SetValue(RightToolBarTrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftToolBarTray.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RightToolBarTrayProperty =
            DependencyProperty.Register("RightToolBarTray", typeof(ToolBarTrayAdv), typeof(ToolBarManager), new PropertyMetadata(OnRightTrayChanged));

        /// <summary>
        /// Gets or Sets the ToolBarTrayAdv displayed at the Top side of ToolBarManager
        /// </summary>
        public ToolBarTrayAdv TopToolBarTray
        {
            get { return (ToolBarTrayAdv)GetValue(TopToolBarTrayProperty); }
            set { SetValue(TopToolBarTrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftToolBarTray.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TopToolBarTrayProperty =
            DependencyProperty.Register("TopToolBarTray", typeof(ToolBarTrayAdv), typeof(ToolBarManager), new PropertyMetadata(OnTopTrayChanged));

        /// <summary>
        /// Gets or Sets the ToolBarTrayAdv displayed at the Bottom side of ToolBarManager
        /// </summary>
        public ToolBarTrayAdv BottomToolBarTray
        {
            get { return (ToolBarTrayAdv)GetValue(BottomToolBarTrayProperty); }
            set { SetValue(BottomToolBarTrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftToolBarTray.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty BottomToolBarTrayProperty =
            DependencyProperty.Register("BottomToolBarTray", typeof(ToolBarTrayAdv), typeof(ToolBarManager), new PropertyMetadata(OnBottomTrayChanged));

        /// <summary>
        /// Gets or Sets the Content of ToolBarManager
        /// </summary>
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(ToolBarManager), new PropertyMetadata(null));


        /// <summary>
        /// Gets or Sets value indicating whether we can dock toolbar at Top position
        /// </summary>
        public bool CanDockAtTop
        {
            get { return (bool)GetValue(CanDockAtTopProperty); }
            set { SetValue(CanDockAtTopProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanDockAtTopTray.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CanDockAtTopProperty =
            DependencyProperty.Register("CanDockAtTop", typeof(bool), typeof(ToolBarManager), new PropertyMetadata(true));

        /// <summary>
        /// Gets or Sets value indicating whether we can dock toolbar at Bottom position
        /// </summary>
        public bool CanDockAtBottom
        {
            get { return (bool)GetValue(CanDockAtBottomProperty); }
            set { SetValue(CanDockAtBottomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanDockAtTopTray.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CanDockAtBottomProperty =
            DependencyProperty.Register("CanDockAtBottom", typeof(bool), typeof(ToolBarManager), new PropertyMetadata(true));

        /// <summary>
        /// Gets or Sets value indicating whether we can dock toolbar at Left position
        /// </summary>
        public bool CanDockAtLeft
        {
            get { return (bool)GetValue(CanDockAtLeftProperty); }
            set { SetValue(CanDockAtLeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanDockAtTopTray.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CanDockAtLeftProperty =
            DependencyProperty.Register("CanDockAtLeft", typeof(bool), typeof(ToolBarManager), new PropertyMetadata(true));

        /// <summary>
        /// Gets or Sets value indicating whether we can dock toolbar at Right position
        /// </summary>
        public bool CanDockAtRight
        {
            get { return (bool)GetValue(CanDockAtRightProperty); }
            set { SetValue(CanDockAtRightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanDockAtTopTray.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CanDockAtRightProperty =
            DependencyProperty.Register("CanDockAtRight", typeof(bool), typeof(ToolBarManager), new PropertyMetadata(true));

        internal static DockArea GetDockArea(DependencyObject obj)
        {
            return (DockArea)obj.GetValue(DockAreaProperty);
        }

        internal static void SetDockArea(DependencyObject obj, DockArea value)
        {
            obj.SetValue(DockAreaProperty, value);
        }

        // Using a DependencyProperty as the backing store for DockArea.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DockAreaProperty =
            DependencyProperty.RegisterAttached("DockArea", typeof(DockArea), typeof(ToolBarManager), new PropertyMetadata(DockArea.Top));

        /// <summary>
        /// Gets the tool bar state
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ToolBarState GetToolBarState(ToolBarAdv obj)
        {
            return (ToolBarState)obj.GetValue(ToolBarStateProperty);
        }

        /// <summary>
        /// Sets the tool bar state
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetToolBarState(ToolBarAdv obj, ToolBarState value)
        {
            obj.SetValue(ToolBarStateProperty, value);
        }

        // Using a DependencyProperty as the backing store for ToolBarState.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ToolBarStateProperty =
            DependencyProperty.RegisterAttached("ToolBarState", typeof(ToolBarState), typeof(ToolBarManager), new PropertyMetadata(ToolBarState.Docking, OnToolBarStateChanged));

        /// <summary>
        /// Gets or Sets a style of floating tool bar
        /// </summary>
        public Style FloatingToolBarStyle
        {
            get { return ( Style)GetValue(FloatingToolBarStyleProperty); }
            set { SetValue(FloatingToolBarStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FloatingToolBarStyle.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FloatingToolBarStyleProperty =
            DependencyProperty.Register("FloatingToolBarStyle", typeof( Style), typeof(ToolBarManager), new PropertyMetadata(null, OnFloatingToolBarStyleChanged));

        

        #endregion

        #region ctor

        /// <summary>
        /// Initializes the nwe instance of ToolBarManager
        /// </summary>
        public ToolBarManager()
        {          
            DefaultStyleKey = typeof(ToolBarManager);
            FloatingToolBars = new List<FloatingToolBar>();
#if WPF
            this.Loaded +=new RoutedEventHandler(ToolBarManager_Loaded);
#endif               
        }

#if WPF
        void  ToolBarManager_Loaded(object sender, RoutedEventArgs e)
        {
            Window wn = VisualUtils.FindRootVisual(this) as Window;
            if (wn != null)
            {
                wn.StateChanged -= new EventHandler(wn_StateChanged);
                wn.StateChanged += new EventHandler(wn_StateChanged);
            }
        }

        void wn_StateChanged(object sender, EventArgs e)
        {
            Window window = sender as Window;
            if (window.WindowState == WindowState.Minimized)
            {
                foreach (var item in FloatingToolBars)                
                    item.Visibility = System.Windows.Visibility.Collapsed;
                
            }
            else if (window.WindowState == WindowState.Maximized || window.WindowState == WindowState.Normal)
            {
                foreach (var item in FloatingToolBars)
                    item.Visibility = System.Windows.Visibility.Visible;
            }

        }
#endif
        #endregion

        # region Methods

        private static void OnTopTrayChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                (args.NewValue as ToolBarTrayAdv).ToolBarManager = obj as ToolBarManager;
            }

            (obj as ToolBarManager).OnTrayChanged(args);
        }

        private static void OnToolBarStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj != null)
            {
                (obj as ToolBarAdv).OnToolBarStateChanged((ToolBarState)args.OldValue, (ToolBarState)args.NewValue);
            }
        }

        private static void OnLeftTrayChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                (args.NewValue as ToolBarTrayAdv).ToolBarManager = obj as ToolBarManager;
                (args.NewValue as ToolBarTrayAdv).Orientation = Orientation.Vertical;
            }

            (obj as ToolBarManager).OnTrayChanged(args);
        }

       

        private static void OnRightTrayChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                (args.NewValue as ToolBarTrayAdv).ToolBarManager = obj as ToolBarManager;
                (args.NewValue as ToolBarTrayAdv).Orientation = Orientation.Vertical;
            }

            (obj as ToolBarManager).OnTrayChanged(args);
        }

        private static void OnBottomTrayChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                (args.NewValue as ToolBarTrayAdv).ToolBarManager = obj as ToolBarManager;
            }

            (obj as ToolBarManager).OnTrayChanged(args);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            content = this.GetTemplateChild("PART_Content") as ContentControl;
            panel = this.GetTemplateChild("PART_Panel") as ToolBarManagerPanel;
            InsertTrays();
            base.OnApplyTemplate();
        }

        private void OnTrayChanged(DependencyPropertyChangedEventArgs args)
        {
            InsertTray(args.OldValue as ToolBarTrayAdv, args.NewValue as ToolBarTrayAdv);
        }

        private void InsertTray(ToolBarTrayAdv oldTray, ToolBarTrayAdv newTray)
        {
            if (panel != null)
            {
                if (oldTray != null && panel.Children.Contains(oldTray))
                {
                    panel.Children.Remove(oldTray);
                }

                if (newTray != null && !panel.Children.Contains(newTray))
                {
                    panel.Children.Add(newTray);
                }
            }
        }

        private static void OnFloatingToolBarStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as ToolBarManager).OnFloatingToolBarStyleChanged(args);
        }

        private void OnFloatingToolBarStyleChanged(DependencyPropertyChangedEventArgs args)
        {
            foreach (FloatingToolBar toolBar in FloatingToolBars)
            {
                toolBar.Style = FloatingToolBarStyle;
            }
        }

        private void InsertTrays()
        {
            if (panel != null)
            {
                if (TopToolBarTray != null && !panel.Children.Contains(TopToolBarTray))
                {
                    RemoveFromParent(TopToolBarTray);
                    panel.Children.Add(TopToolBarTray);
                }
                if (BottomToolBarTray != null && !panel.Children.Contains(BottomToolBarTray))
                {
                    RemoveFromParent(BottomToolBarTray);
                    panel.Children.Add(BottomToolBarTray);
                }
                if (LeftToolBarTray != null && !panel.Children.Contains(LeftToolBarTray))
                {
                    RemoveFromParent(LeftToolBarTray);
                    panel.Children.Add(LeftToolBarTray);
                }
                if (RightToolBarTray != null && !panel.Children.Contains(RightToolBarTray))
                {
                    RemoveFromParent(RightToolBarTray);
                    panel.Children.Add(RightToolBarTray);
                }
            }
        }

        private void RemoveFromParent(ToolBarTrayAdv tray)
        {
            if (tray != null && tray.Parent is Panel)
            {
                if ((tray.Parent as Panel).Children.Contains(tray))
                {
                    (tray.Parent as Panel).Children.Remove(tray);
                }
            }
        }

        internal bool CanDock(DockArea area)
        {
            if (area == DockArea.Top)
                return CanDockAtTop;
            else if (area == DockArea.Left)
                return CanDockAtLeft;
            else if (area == DockArea.Right)
                return CanDockAtRight;
            else if (area == DockArea.Bottom)
                return CanDockAtBottom;

            return false;
        }

        internal void Remove(ToolBarTrayAdv tray)
        {
            RemoveFromParent(tray);

            if (TopToolBarTray == tray)
                TopToolBarTray = null;
            else if (LeftToolBarTray == tray)
                LeftToolBarTray = null;
            else if (RightToolBarTray == tray)
                RightToolBarTray = null;
            else if (BottomToolBarTray == tray)
                BottomToolBarTray = null;
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
            panel.Arrange(finalSize);
            base.ArrangeOverride(finalSize);

            return finalSize;
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
            Size tempSize = availableSize;
            if (double.IsInfinity(availableSize.Width))
                tempSize.Width = double.MaxValue;
            if (double.IsInfinity(availableSize.Height))
                tempSize.Height = double.MaxValue;

            availableSize = tempSize;
            currentSize = availableSize;

            MeasureCall(availableSize);
            
            base.MeasureOverride(availableSize);

            return availableSize;
        }

        private Size MeasureCall(Size availableSize)
        {
            double height = availableSize.Height;
            double width = availableSize.Width;
            if (TopToolBarTray != null)
            {
                TopToolBarTray.MeasureCall(needToInvalidate, availableSize);
                foreach (ToolBarBand band in TopToolBarTray.Bands)
                {
                    height -= band.Size;
                }
            }

            if (BottomToolBarTray != null)
            {
                BottomToolBarTray.MeasureCall(needToInvalidate, availableSize);
                foreach (ToolBarBand band in BottomToolBarTray.Bands)
                {
                    height -= band.Size;
                }
            }

            height = Math.Max(0.0, height);

            Size size = new Size(availableSize.Width, height);
            if (LeftToolBarTray != null)
            {
                LeftToolBarTray.MeasureCall(needToInvalidate, size);
                foreach (ToolBarBand band in LeftToolBarTray.Bands)
                {
                    width -= band.Size;
                }
            }

            if (RightToolBarTray != null)
            {
                RightToolBarTray.MeasureCall(needToInvalidate, size);
                foreach (ToolBarBand band in RightToolBarTray.Bands)
                {
                    width -= band.Size;
                }
            }
            width = Math.Max(0.0, width);

            if (content != null)
            {
                content.Measure(new Size(width, height));
            }

            return availableSize;
        }

        internal DockArea FindDockArea(Point point)
        {
            double y = 0;
            double x =0;
            double height =20;
            double size = 0;

            /************* TOP RECT CALCULATION *******************/

            size = GetSize(TopToolBarTray, Orientation.Horizontal);
            Rect topRect = new Rect(0, 0, ActualWidth, size);

            if (topRect.Contains(point))
            {
                return DockArea.Top;
            }

            /************* BOTTOM RECT CALCULATION *******************/

            size = GetSize(BottomToolBarTray, Orientation.Horizontal);
            y = ActualHeight - size;
            Rect bottomRect = new Rect(0, Math.Max(0, y), ActualWidth, size);

            if (bottomRect.Contains(point))
            {
                return DockArea.Bottom;
            }

            /************* LEFT RECT CALCULATION *******************/

            height = ActualHeight - (topRect.Height + bottomRect.Height);
            height = height < 0 ? 20 : height;
            size = GetSize(LeftToolBarTray, Orientation.Vertical);
            Rect leftRect = new Rect(0, topRect.Height, size, height);

            if (leftRect.Contains(point))
            {
                return DockArea.Left;
            }

            /************* RIGHT RECT CALCULATION *******************/

            size = GetSize(RightToolBarTray, Orientation.Vertical);
            x = ActualWidth - size;
            Rect rightRect = new Rect(Math.Max(0.0, x), topRect.Height, size, height);

            if (rightRect.Contains(point))
            {
                return DockArea.Right;
            }

           /********************************************************/

            return DockArea.None;
        }

        private double GetSize(ToolBarTrayAdv tray, Orientation orientation)
        {
            double value = 20;
            if(tray == null)
                return value;
            Size size = new Size(tray.ActualWidth, tray.ActualHeight);
            value = OrientedValue.GetOrientedHeightValue(size, orientation);
            value = value == 0 ? 20 : value;
            return value;
        }

        internal ToolBarTrayAdv GetToolBarTray(DockArea area)
        {
            if (area == DockArea.Top)
                return TopToolBarTray;
            else if (area == DockArea.Left)
                return LeftToolBarTray;
            else if (area == DockArea.Right)
                return RightToolBarTray;
            else
                return BottomToolBarTray;
        }

        internal DockArea GetDockArea(ToolBarTrayAdv tray)
        {
            if (tray != null)
            {
                if (LeftToolBarTray == tray)
                    return DockArea.Left;
                else if (TopToolBarTray == tray)
                    return DockArea.Top;
                else if (RightToolBarTray == tray)
                    return DockArea.Right;
                else if (BottomToolBarTray == tray)
                    return DockArea.Bottom;
                else
                    return DockArea.None;
            }

            return DockArea.None;
        }

        internal void DockTray(ToolBarTrayAdv tray, DockArea area)
        {
            if (tray != null && area != DockArea.None)
            {
                tray.ToolBarManager = this;

                if (area == DockArea.Top)
                {
                    tray.Orientation = Orientation.Horizontal;
                    TopToolBarTray = tray;
                }
                else if (area == DockArea.Left)
                {
                    tray.Orientation = Orientation.Vertical;
                    LeftToolBarTray = tray;
                }
                else if (area == DockArea.Right)
                {
                    tray.Orientation = Orientation.Vertical;
                    RightToolBarTray = tray;
                }
                else
                {
                    tray.Orientation = Orientation.Horizontal;
                    BottomToolBarTray = tray;
                }
                tray.UpdateVisualState();
                needToInvalidate = false;
                Invalidate();
            }
        }

        internal void DockToolBar(ToolBarAdv toolBar, DockArea area)
        {
            if (toolBar != null && area != DockArea.None)
            {
                ToolBarTrayAdv tray = GetToolBarTray(area);

                if (tray == null)
                    tray = new ToolBarTrayAdv();

                tray.ToolBars.Add(toolBar);
                toolBar.Tray = tray;
                needToInvalidate = false;
                if (GetDockArea(toolBar.Tray) == DockArea.None)
                {
                    DockTray(tray, area);
                }
                else
                {
                    toolBar.Orientation = tray.Orientation;
                    Invalidate();
                }
               
            }
        }

        /// <summary>
        /// Invalidates the entire layout
        /// </summary>
        public void InvalidateLayout()
        {   
            InvalidateMeasure();
            InvalidateArrange();
            this.UpdateLayout();
        }

        internal void Invalidate()
        {
            MeasureCall(currentSize);
            if (panel != null)
            {
                panel.InvalidateArrange();
            }
        }

        #endregion
    }

    internal enum DockArea
    {
        Top,

        Left,

        Right,

        Bottom,

        None
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ToolBarState
    {
        /// <summary>
        /// ToolBarAdv will be docked in any one of the ToolBarTrayAdv
        /// </summary>
        Docking,

        /// <summary>
        /// ToolBarAdv will be floating
        /// </summary>
        Floating,

        /// <summary>
        /// ToolBarAdv will be hidden
        /// </summary>
        Hidden
    }
}
