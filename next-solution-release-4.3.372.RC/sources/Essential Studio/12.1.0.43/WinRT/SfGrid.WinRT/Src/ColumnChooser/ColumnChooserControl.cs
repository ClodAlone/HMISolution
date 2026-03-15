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
using System.Windows;
using System.Windows.Input;
using Syncfusion.Data.Extensions;
#if !WinRT
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Input;
using Windows.UI.Xaml.Media;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    /// <summary>
    /// Interface to customize the Column Chooser operation
    /// </summary>
    /// <remarks></remarks>
    public interface IColumnChooser
    {
        /// <summary>
        /// Add child for Column Chooser
        /// </summary>
        /// <param name="column"></param>
        /// <remarks></remarks>
        void AddChild(GridColumn column);

        /// <summary>
        /// Remove child in Column Chooser
        /// </summary>
        /// <param name="column"></param>
        /// <remarks></remarks>
        void RemoveChild(GridColumn column);

        /// <summary>
        /// Returns Column Chooser Rect
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        Rect GetControlRect();
    }

#if !WPF
    public class ColumnChooser : Control, IColumnChooser
    {
        #region Public Field
        /// <summary>
        /// Gets or sets ColumnChooser Popup window.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Popup Popup { get; set; }
        #endregion

        /// <summary>
        /// Gets DataGrid for ColumnChooser.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        protected SfDataGrid DataGrid { get; private set; }

        #region Fields
        Border closeButtonBorder;
        Point pointerPressedPoint;
        bool isPointerPressed;
        StackPanel chooserPanel;
        List<GridColumn> intialChildren = new List<GridColumn>();
        #endregion
        
        #region ctor
        public ColumnChooser(SfDataGrid dataGrid)
        {
            this.DefaultStyleKey = typeof(ColumnChooser);
            this.Popup = new Popup();
            Popup.Child = this;
            this.DataGrid = dataGrid;
        }
        #endregion

        #region Dependency properties

        /// <summary>
        /// Gets or sets Title for ColumnChooser Popup Window.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ColumnChooser), new PropertyMetadata(GridResourceWrapper.ColumnChooserTitle));

        /// <summary>
        /// Gets or sets WaterMarkText for Empty Column Chooser.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string WaterMarkText
        {
            get { return (string)GetValue(WaterMarkTextProperty); }
            set { SetValue(WaterMarkTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaterMarkText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaterMarkTextProperty =
            DependencyProperty.Register("WaterMarkText", typeof(string), typeof(ColumnChooser), new PropertyMetadata(GridResourceWrapper.ColumnChooserWaterMark));
        
        /// <summary>
        /// Gets or sets Visibility of WaterMarkText for Empty Column Chooser.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Visibility WaterMarkTextVisibility
        {
            get { return (Visibility)GetValue(WaterMarkTextVisibilityProperty); }
            set { SetValue(WaterMarkTextVisibilityProperty, value); }
        }

        public static readonly DependencyProperty WaterMarkTextVisibilityProperty =
            DependencyProperty.Register("WaterMarkTextVisibility", typeof(Visibility), typeof(ColumnChooser), new PropertyMetadata(Visibility.Visible));



        /// <summary>
        /// Gets or sets TitleBar background.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public SolidColorBrush TitleBarBackground
        {
            get { return (SolidColorBrush)GetValue(TitleBarBackgroundProperty); }
            set { SetValue(TitleBarBackgroundProperty, value); }
        }

        public static readonly DependencyProperty TitleBarBackgroundProperty =
            DependencyProperty.Register("TitleBarBackground", typeof(SolidColorBrush), typeof(ColumnChooser), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets TitleBar Foreground.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public SolidColorBrush TitleBarForeground
        {
            get { return (SolidColorBrush)GetValue(TitleBarForegroundProperty); }
            set { SetValue(TitleBarForegroundProperty, value); }
        }

        public static readonly DependencyProperty TitleBarForegroundProperty =
            DependencyProperty.Register("TitleBarForeground", typeof(SolidColorBrush), typeof(ColumnChooser), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets Text Alignment for the Title.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public TextAlignment TitleTextAlignment
        {
            get { return (TextAlignment)GetValue(TitleTextAlignmentProperty); }
            set { SetValue(TitleTextAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleTextAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleTextAlignmentProperty =
            DependencyProperty.Register("TitleTextAlignment", typeof(TextAlignment), typeof(ColumnChooser), new PropertyMetadata(null));

        

        #endregion

        #region Private methods
#if WinRT
        private void OnCloseButtonPressed(object sender, PointerRoutedEventArgs e)
#else
        private void OnCloseButtonPressed(object sender, MouseButtonEventArgs e)
#endif
        {
            isPointerPressed = false;
            (this.Parent as Popup).IsOpen = false;
        }
        #endregion

        #region Overrides
#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            this.Title = GridResourceWrapper.ColumnChooserTitle;
            this.WaterMarkText = GridResourceWrapper.ColumnChooserWaterMark;
            chooserPanel = this.GetTemplateChild("PART_ChooserPanel") as StackPanel;
            closeButtonBorder = this.GetTemplateChild("PART_CloseBorder") as Border;
#if WinRT
            closeButtonBorder.PointerPressed += OnCloseButtonPressed;
#else
            closeButtonBorder.MouseLeftButtonUp += OnCloseButtonPressed;
#endif
            this.DataGrid.Columns.ForEach(col =>
            {
                if (col.IsHidden)
                    intialChildren.Add(col);
            });
            intialChildren.ForEach(child => AddChild(child));
            intialChildren.Clear();
            base.OnApplyTemplate();
        }

#if WinRT
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            PointerPoint pp = e.GetCurrentPoint(null);

            double deltaV = pp.Position.Y;
            double deltaH = pp.Position.X;
            if (isPointerPressed && pp.Properties.IsLeftButtonPressed && deltaH != pointerPressedPoint.X && deltaV != pointerPressedPoint.Y)
            {
                this.CapturePointer(e.Pointer);
                (this.Parent as Popup).HorizontalOffset = deltaH - pointerPressedPoint.X;
                (this.Parent as Popup).VerticalOffset = deltaV - pointerPressedPoint.Y;
                e.Handled = true;
            }

        }
#else
        protected override void OnMouseMove(MouseEventArgs e)
        {
            var pp = e.GetPosition(null);
            
            if (isPointerPressed)
            {
                this.CaptureMouse();
                // Calculate the current position of the object.
                double deltaV = e.GetPosition(null).Y - pointerPressedPoint.Y;
                double deltaH = e.GetPosition(null).X - pointerPressedPoint.X;
                double newTop = deltaV + (double)(this.Parent as Popup).GetValue(Popup.VerticalOffsetProperty);
                double newLeft = deltaH + (double)(this.Parent as Popup).GetValue(Popup.HorizontalOffsetProperty);

                // Set new position of object.
                (this.Parent as Popup).SetValue(Popup.VerticalOffsetProperty, newTop);
                (this.Parent as Popup).SetValue(Popup.HorizontalOffsetProperty, newLeft);

                // Update position global variables.
                pointerPressedPoint.Y = e.GetPosition(null).Y;
                pointerPressedPoint.X = e.GetPosition(null).X;
            }
            base.OnMouseMove(e);
        }
#endif

#if WinRT
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
#else
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#endif
        {
            pointerPressedPoint.X = -1;
            pointerPressedPoint.Y = -1;
            isPointerPressed = false;
#if SILVERLIGHT
            this.ReleaseMouseCapture();
#else
            this.ReleasePointerCapture(e.Pointer);
#endif
            e.Handled = true;
        }
#if WinRT
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
#else
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#endif
        {
            isPointerPressed = true;
#if WinRT
            pointerPressedPoint = e.GetCurrentPoint(this).Position;
#else
            pointerPressedPoint = e.GetPosition(null);
#endif
            e.Handled = true;
        }

        #endregion

        #region Virtual methods
        /// <summary>
        /// Adds the Child for the column chooser whever the column gets hide
        /// </summary>
        /// <param name="column"></param>
        /// <remarks></remarks>
        public virtual void AddChild(GridColumn column)
        {
            if (chooserPanel == null)
            {
                intialChildren.Add(column);
                return;
            }
            if (this.chooserPanel.Children.ToList<ColumnChooserItem>().All(item => (item as ColumnChooserItem).Column.MappingName != column.MappingName) && this.DataGrid.View != null)
            {
                var chooserItem = new ColumnChooserItem(column);
                chooserItem.Controller = this.DataGrid.GridColumnDragDropController;
                chooserItem.ColumnName = column.HeaderText;
                this.chooserPanel.Children.Add(chooserItem);
            }
            if (this.chooserPanel.Children.Count == 0)
                this.WaterMarkTextVisibility = Visibility.Visible;
            else
                this.WaterMarkTextVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Remove the Child for the column chooser whever the column gets Unhide
        /// </summary>
        /// <param name="column"></param>
        /// <remarks></remarks>
        public virtual void RemoveChild(GridColumn column)
        {
            if (this.chooserPanel != null && this.chooserPanel.Children.Count > 0)
            {
                var element = this.chooserPanel.Children.ToList<ColumnChooserItem>().FirstOrDefault(item => (item as ColumnChooserItem).Column.MappingName == column.MappingName);
                if (element != null)
                    this.chooserPanel.Children.Remove(element);
            }
            if (this.chooserPanel != null && this.chooserPanel.Children.Count == 0)
                this.WaterMarkTextVisibility = Visibility.Visible;
            else
                this.WaterMarkTextVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Returns the Rect of the ColumnChooserControl
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual Rect GetControlRect()
        {
#if WinRT
            return new Rect(this.TransformToVisual(null).TransformPoint(new Point(0, 0)), this.DesiredSize);
#else
            if (!this.Popup.IsOpen)
                return Rect.Empty;
            return new Rect(this.TransformToVisual(null).Transform(new Point(0, 0)), this.DesiredSize);
#endif
        }

        /// <summary>
        /// Shows the ColumnChooser Window
        /// </summary>
        /// <remarks></remarks>
        public void ShowColumnChooser()
        {
            this.Popup.IsOpen = true;
        }
        #endregion
    }
#endif


    /// <summary>
    /// Column chooser item
    /// </summary>
    /// <remarks></remarks>
    public class ColumnChooserItem : Control
    {

        #region Ctor
        public ColumnChooserItem(GridColumn column)
        {
            DefaultStyleKey = typeof(ColumnChooserItem);
            Column = column;
            ColumnName = column.HeaderText != null ? column.HeaderText : column.MappingName;
        }
        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets GridColumn for ColumnChooserItem.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public GridColumn Column { get; set; }

        /// <summary>
        /// Gets or sets GridColumnDragDropController for ColumnChooserItem.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public GridColumnDragDropController Controller { get; set; }
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Gets or sets ColumnName for the ColumnChooserItem.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string ColumnName
        {
            get { return (string)GetValue(ColumnNameProperty); }
            set { SetValue(ColumnNameProperty, value); }
        }

        public static readonly DependencyProperty ColumnNameProperty =
            DependencyProperty.Register("ColumnName", typeof(string), typeof(ColumnChooserItem), new PropertyMetadata(string.Empty));
        #endregion

#if SILVERLIGHT

        #region Fields
        bool isLeftButtonPressed;
        #endregion

        #region Overrides
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            isLeftButtonPressed = true;
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            isLeftButtonPressed = false;
            base.OnMouseLeftButtonUp(e);
        }
        #endregion
#endif
        #region Overrides
#if WPF || SILVERLIGHT
        protected override void OnMouseLeave(MouseEventArgs e)
        {
           VisualStateManager.GoToState(this, "Normal", true);
            base.OnMouseLeave(e);
        }
#elif WinRT
        protected override void OnPointerExited(PointerRoutedEventArgs e)
         {
            VisualStateManager.GoToState(this,"Normal",true);
 	        base.OnPointerExited(e);
          }
#endif
#if WinRT
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
#else
        protected override void OnMouseMove(MouseEventArgs e)
#endif
        {
#if WPF
            if (e.LeftButton == MouseButtonState.Pressed)
#elif WinRT
            if(e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
#else
            if(isLeftButtonPressed)
#endif
            {
#if WinRT
                (this.Controller as GridColumnChooserController).Show((this.Controller as GridColumnChooserController).dataGrid.Columns.IndexOf(Column),e);
#else
#if SILVERLIGHT
                isLeftButtonPressed = false;
#endif
                (this.Controller as GridColumnChooserController).Show((this.Controller as GridColumnChooserController).dataGrid.Columns.IndexOf(Column), e);
#endif
            }
#if !WinRT
            VisualStateManager.GoToState(this, "MouseOver", true);
            base.OnMouseMove(e);
#else
            VisualStateManager.GoToState(this, "PointerOver", true);
            base.OnPointerMoved(e);
#endif
        }
        #endregion

    }
}
