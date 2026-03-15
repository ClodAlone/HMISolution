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
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Controls;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    
#if !WPF
    /// <summary>
    /// 
    /// </summary>
    public class FloatingToolBar: WindowControl
#else
        [CLSCompliant(true)]
    public class FloatingToolBar : ContentControl
#endif
    {
        internal WrapPanel panel = null;
        private double resizeOffset = double.NaN;
        private Thumb topThumb = null;
        private Thumb bottomThumb = null;
        private Thumb leftThumb = null;
        private Popup PART_DropPopUp = null;
        private Thumb rightThumb = null;
        private Thumb titleThumb = null;
        private FrameworkElement client = null;
        private ToolBarAdv toolBar = null;
        private bool forceDrag = false;

        private double maxLineSize = 0;
        private double minLineSize = 0;
        private bool isDragging = false;

#if WPF 
        private Button closbutton = null;
#endif
        internal ToolBarManager Manager
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ToolBarAdv ToolBar
        {
            get
            {
                return toolBar;
            }
            set
            {
                toolBar = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ToolBarIteminfo> ToolBarItemInfoCollection
        {
            get
            {
                return toolBar.ToolBarItemInfoCollection;
            }
            set
            {
                toolBar.ToolBarItemInfoCollection = value;
            }
        }

        /// <summary>
        /// Gets or Sets resource dictionary from which ToolBarAdv will look up for framework element's styles
        /// </summary>
        public ResourceDictionary ControlsResourceDictionary
        {
            get { return (ResourceDictionary)GetValue(ControlsResourceDictionaryProperty); }
            set { SetValue(ControlsResourceDictionaryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlsResourceDictionary.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ControlsResourceDictionaryProperty =
            DependencyProperty.Register("ControlsResourceDictionary", typeof(ResourceDictionary), typeof(FloatingToolBar), new PropertyMetadata(

#if !WPF
new System.Windows.ResourceDictionary() { Source = new Uri("/Syncfusion.Shared.Silverlight;component/Controls/ToolBarAdv/Themes/ToolbarResources.xaml", UriKind.RelativeOrAbsolute) }
#else
                new System.Windows.ResourceDictionary() { Source = new Uri("/Syncfusion.Shared.Wpf;component/Controls/ToolBarAdv/Themes/ToolBarResources.xaml", UriKind.RelativeOrAbsolute) }
#endif
, new PropertyChangedCallback(OnControlsResourceDictionaryPropertyChanged)));

        internal bool ForceDrag
        {
            get
            {
                return forceDrag;
            }
            set
            {
                forceDrag = value;
                if (titleThumb != null)
                {
                    if (forceDrag)
                        titleThumb.CaptureMouse();
                    else
                        titleThumb.ReleaseMouseCapture();
                }
            }
        }

#if WPF
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(FloatingToolBar), new UIPropertyMetadata(String.Empty));

        internal Popup popup;

#endif

        /// <summary>
        /// 
        /// </summary>
        public FloatingToolBar()
        {
            DefaultStyleKey = typeof(FloatingToolBar);
            panel = new WrapPanel();
#if !WPF

            this.ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.Manual;
#endif
            Content = panel;
            Unloaded += new RoutedEventHandler(FloatingToolBar_Unloaded);
#if !WPF
            Closed += new ClosedEventHandler(FloatingToolBar_Closed);
#endif
        }

#if !WPF
        void FloatingToolBar_Closed(object sender, ClosedEventArgs e)
        {
            ToolBar.ChangeStateInternally(ToolBarState.Hidden);
        }
#endif

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            GetTemplateChild();
            base.OnApplyTemplate();
        }

        private void GetTemplateChild()
        {
#if WPF
            if (closbutton != null)
            {
                closbutton.Click -= new RoutedEventHandler(closbutton_Click);
            }
#endif
            if (topThumb != null)
                topThumb.DragDelta -= new DragDeltaEventHandler(ResizeDragDelta);
            if (bottomThumb != null)
                bottomThumb.DragDelta -= new DragDeltaEventHandler(ResizeDragDelta);
            if (leftThumb != null)
                leftThumb.DragDelta -= new DragDeltaEventHandler(ResizeDragDelta);
            if (rightThumb != null)
                rightThumb.DragDelta -= new DragDeltaEventHandler(ResizeDragDelta);
            if (titleThumb != null)
            {
                titleThumb.DragDelta -= new DragDeltaEventHandler(titleThumb_DragDelta);
                titleThumb.DragStarted -= new DragStartedEventHandler(titleThumb_DragStarted);
                titleThumb.MouseMove -= new MouseEventHandler(titleThumb_MouseMove);
                titleThumb.MouseLeftButtonUp -= new MouseButtonEventHandler(titleThumb_MouseLeftButtonUp);
                titleThumb.DragCompleted -= new DragCompletedEventHandler(titleThumb_DragCompleted);
            }
            PART_DropPopUp = this.GetTemplateChild("PART_DropPopUp") as Popup;
            topThumb = this.GetTemplateChild("PART_TopThumb") as Thumb;
            bottomThumb = this.GetTemplateChild("PART_BottomThumb") as Thumb;
            leftThumb = this.GetTemplateChild("PART_LeftThumb") as Thumb;
            rightThumb = this.GetTemplateChild("PART_RightThumb") as Thumb;
            titleThumb = this.GetTemplateChild("PART_TitleFloatingThumb") as Thumb;
            client = GetTemplateChild("PART_Client") as FrameworkElement;
#if WPF
            closbutton = this.GetTemplateChild("PART_CloseButton") as Button;
            if (closbutton != null)
            {
                closbutton.Click += new RoutedEventHandler(closbutton_Click);
            }
#endif
            if (topThumb != null)
                topThumb.DragDelta += new DragDeltaEventHandler(ResizeDragDelta);
            if (bottomThumb != null)
                bottomThumb.DragDelta += new DragDeltaEventHandler(ResizeDragDelta);
            if (leftThumb != null)
                leftThumb.DragDelta += new DragDeltaEventHandler(ResizeDragDelta);
            if (rightThumb != null)
                rightThumb.DragDelta += new DragDeltaEventHandler(ResizeDragDelta);
            if (titleThumb != null)
            {
                titleThumb.DragDelta += new DragDeltaEventHandler(titleThumb_DragDelta);
                titleThumb.DragStarted += new DragStartedEventHandler(titleThumb_DragStarted);
                titleThumb.MouseMove += new MouseEventHandler(titleThumb_MouseMove);
                titleThumb.MouseLeftButtonUp += new MouseButtonEventHandler(titleThumb_MouseLeftButtonUp);
                titleThumb.DragCompleted += new DragCompletedEventHandler(titleThumb_DragCompleted);
            }

            if (titleThumb != null)
            {
                if (ForceDrag)
                    titleThumb.CaptureMouse();
                else
                    titleThumb.ReleaseMouseCapture();
            }
#if !WPF

            Application.Current.RootVisual.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
#endif
        }

        void titleThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isDragging = false;
        }

#if WPF
        void closbutton_Click(object sender, RoutedEventArgs e)
        {
            if (popup != null)
                popup.IsOpen = false;
            ToolBar.ChangeStateInternally(ToolBarState.Hidden);
        }
#endif

        private void OnMouseDown(object sender, MouseButtonEventArgs args)
        {
            if (PART_DropPopUp != null)
            {
                PART_DropPopUp.IsOpen = false;
            }
        }

        private static void OnControlsResourceDictionaryPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as FloatingToolBar).OnControlsResourceDictionaryPropertyChanged(args);
        }

        private void OnControlsResourceDictionaryPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (ToolBar != null && ToolBarManager.GetToolBarState(this.ToolBar) == ToolBarState.Floating)
                InsertItems(ToolBar);

            ApplyStyleForControls();
        }

        internal void ApplyStyleForControls()
        {
            ResourceDictionary dictionary = ControlsResourceDictionary;
            if ( ToolBar!=null && ToolBar.generatedConatiner != null)
            {
                foreach (FrameworkElement ele in ToolBar.generatedConatiner.Values)
                {
#if SILVERLIGHT
                if (dictionary.Contains("ToolBar" + ele.GetType().Name + "Style") && !DesignerProperties.IsInDesignTool)
                {
                    
                    ele.Style = dictionary["ToolBar" + ele.GetType().Name + "Style"] as Style;
                }
#endif
#if WPF
                    if (dictionary.Contains("ToolBar" + ele.GetType().Name + "Style"))
                    {
                        ele.Style = dictionary["ToolBar" + ele.GetType().Name + "Style"] as Style;
                    }
#endif
                }
            }
        }


        void titleThumb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ForceDrag = false;
        }

        void titleThumb_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(Manager);
            if (ForceDrag)
            {
#if !WPF

                Point screenPoints = e.GetPosition(null);
                Left = screenPoints.X - 10;
                Top = screenPoints.Y - 10;
                ToolBar.FloatingBarLocation = new Point(Left, Top);
#else
                if (this.popup != null)
                {
                    WindowInterop.POINT interopPoint = new WindowInterop.POINT();
                    WindowInterop.GetCursorPos(out interopPoint);
                    popup.HorizontalOffset = interopPoint.x - 10;
                    popup.VerticalOffset = interopPoint.y - 10;
                    ToolBar.FloatingBarLocation = new Point(popup.HorizontalOffset, popup.VerticalOffset);
                }
#endif
            }
            if(ForceDrag || isDragging)
                FindDockArea(point);
        }

        void titleThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            isDragging = true;
            ((Control)client).Focus();
#if !WPF
            BringToFront();
#endif
        }

        void titleThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            MoveWindow(e.HorizontalChange, e.VerticalChange);
        }

        internal void MoveWindow(double horizontalChange, double verticalChange)
        {
#if !WPF

            Left += horizontalChange;
            Top += verticalChange;
            this.ToolBar.FloatingBarLocation = new Point(Left, Top);
#else
            if (this.popup != null)
            {
                this.popup.HorizontalOffset += horizontalChange;
                this.popup.VerticalOffset += verticalChange;
                this.ToolBar.FloatingBarLocation = new Point(popup.HorizontalOffset, popup.VerticalOffset);
            }
#endif
        }

        private void FindDockArea(Point point)
        {
            DockArea area = Manager.FindDockArea(point);
            ToolBarTrayAdv tray = Manager.GetToolBarTray(area);
            if (tray != null && (tray.IsLocked || !Manager.CanDock(area)))
                return;
            if (area != DockArea.None)
            {
                this.Visibility = Visibility.Collapsed;
#if !WPF
                this.Close();
#else
                if (popup != null)
                {
                    popup.IsOpen = false;
                }
#endif
                panel.Children.Clear();
                if (Manager.FloatingToolBars.Contains(ToolBar.floatingToolBar))
                {
                    Manager.FloatingToolBars.Remove(ToolBar.floatingToolBar);
                }
                ToolBar.floatingToolBar = null;
                Manager.DockToolBar(ToolBar, area);
                ForceDrag = false;
                ToolBar.IsDragging = true;
                ToolBar.ChangeStateInternally(ToolBarState.Docking);
            }
        }

        void ResizeDragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            Size size = panel.DesiredSize;
#if !WPF
            switch (thumb.Name)
            {
                case "PART_TopThumb":
                    Resize(e.VerticalChange);
                    Top = Top + (size.Height - panel.DesiredSize.Height);
                    break;
                case "PART_BottomThumb":
                    Resize(-e.VerticalChange);
                    break;
                case "PART_LeftThumb":
                    Resize(-e.HorizontalChange);
                    Left = Left + (size.Width - panel.DesiredSize.Width);
                    break;
                case "PART_RightThumb":
                    Resize(e.HorizontalChange);
                    break;
            }
#else
            switch (thumb.Name)
            {
                case "PART_TopThumb":
                    Resize(e.VerticalChange);
                    if (this.popup != null)
                    {
                        this.popup.VerticalOffset += (size.Height - panel.DesiredSize.Height);
                    }
                    break;
                case "PART_BottomThumb":
                    Resize(-e.VerticalChange);
                    break;
                case "PART_LeftThumb":
                    Resize(-e.HorizontalChange);
                    if (popup != null)
                    {
                        this.popup.HorizontalOffset += (size.Width - panel.DesiredSize.Width);
                    }
                    break;
                case "PART_RightThumb":
                    Resize(e.HorizontalChange);
                    break;
            }
#endif
        }

        private void Resize(double change)
        {
            Size panelSize = new Size(panel.ActualWidth, panel.ActualHeight);
            bool canResize = panelSize.Width + change > minLineSize;
            if (canResize)
            {
                double realChange = change;
                panel.Width = double.NaN;
                if (!double.IsNaN(resizeOffset))
                {
                    realChange = resizeOffset;
                }
                panel.Measure(new Size(panel.ActualWidth + realChange, double.PositiveInfinity));
                if (panel.DesiredSize.Width == panelSize.Width 
                    && panel.DesiredSize.Width < maxLineSize)
                {
                    if (double.IsNaN(resizeOffset))
                        resizeOffset = 0;
                    resizeOffset += change;
                }
                else
                {
                    resizeOffset = double.NaN;
                }
                panel.Width = panel.DesiredSize.Width;
                InvalidateMeasure();
            }
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
            return base.ArrangeOverride(finalSize);
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
            Size size = base.MeasureOverride(availableSize);
            MeasureLineSizes();
            return size;
        }

        private double GetMaxLineSize()
        {
            double width = 0;

            foreach (UIElement element in panel.Children)
            {
                width += element.DesiredSize.Width;
            }

            return width;
        }

        private double GetMinLineSize()
        {
            double width = 0;

            foreach (UIElement element in panel.Children)
            {
                width = Math.Max(width, element.DesiredSize.Width);
            }

            return width;
        }

        private void MeasureLineSizes()
        {
            maxLineSize = GetMaxLineSize();
            minLineSize = GetMinLineSize();
        }

        internal void InsertItems(ToolBarAdv toolBar)
        {
            panel.Children.Clear();

            foreach (object obj in toolBar.Items)
            {
                toolBar.InsertItemToPanel(panel, obj);
            }
        }

        internal void ReleaseResources()
        {
#if !WPF
            Closed -= new ClosedEventHandler(FloatingToolBar_Closed);
#endif
            if (topThumb != null)
                topThumb.DragDelta -= new DragDeltaEventHandler(ResizeDragDelta);
            if (bottomThumb != null)
                bottomThumb.DragDelta -= new DragDeltaEventHandler(ResizeDragDelta);
            if (leftThumb != null)
                leftThumb.DragDelta -= new DragDeltaEventHandler(ResizeDragDelta);
            if (rightThumb != null)
                rightThumb.DragDelta -= new DragDeltaEventHandler(ResizeDragDelta);
            if (titleThumb != null)
            {
                titleThumb.DragDelta -= new DragDeltaEventHandler(titleThumb_DragDelta);
                titleThumb.DragStarted -= new DragStartedEventHandler(titleThumb_DragStarted);
                titleThumb.MouseMove -= new MouseEventHandler(titleThumb_MouseMove);
                titleThumb.MouseLeftButtonUp -= new MouseButtonEventHandler(titleThumb_MouseLeftButtonUp);
            }
            Unloaded -= new RoutedEventHandler(FloatingToolBar_Unloaded);
        }

        void FloatingToolBar_Unloaded(object sender, RoutedEventArgs e)
        {
            ReleaseResources();
        }
    }
}
