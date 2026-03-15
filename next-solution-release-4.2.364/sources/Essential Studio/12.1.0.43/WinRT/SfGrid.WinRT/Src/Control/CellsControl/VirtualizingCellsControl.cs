#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.Utility;
using System;
using System.Collections.Generic;
#if WinRT
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Syncfusion.Data;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using Syncfusion.Data;
using System.ComponentModel;
using System.Windows.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class VirtualizingCellsControl : ContentControl, IDisposable
    {
        #region Fields

        internal Panel ItemsPanel;
        internal Func<double> GetVisibleLineOrigin;
        internal Func<bool> AllowRowHoverHighlighting;
        private bool HasError;
        double oldOrigin;
        double oldActualWidth;

        #endregion

        #region Dependency Region

        /// <summary>
        /// Gets or sets SelectionBorder visiblity.
        /// Which is bind to the Selection Border visiblity property
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Visibility SelectionBorderVisiblity
        {
            get { return (Visibility)GetValue(SelectionBorderVisiblityProperty); }
            set { SetValue(SelectionBorderVisiblityProperty, value); }
        }

        /// <summary>
        /// Dependency registration for SelectionborderVisiblity
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty SelectionBorderVisiblityProperty =
            DependencyProperty.Register("SelectionBorderVisiblity", typeof(Visibility), typeof(VirtualizingCellsControl), new PropertyMetadata(Visibility.Collapsed, OnSelectionBorderVisiblityChanged));


        /// <summary>
        /// Gets or Sets HighlightSelectionBorder visiblity.
        /// Which is bind to the Selection Border visiblity property
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Visibility HighlightSelectionBorderVisiblity
        {
            get { return (Visibility)GetValue(HighlightSelectionBorderVisiblityProperty); }
            set { SetValue(HighlightSelectionBorderVisiblityProperty, value); }
        }

        /// <summary>
        /// Dependency registration for SelectionborderVisiblity
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty HighlightSelectionBorderVisiblityProperty =
            DependencyProperty.Register("HighlightSelectionBorderVisiblity", typeof(Visibility), typeof(VirtualizingCellsControl), new PropertyMetadata(Visibility.Collapsed, OnHighlightBorderVisiblityChanged));

        /// <summary>
        /// Gets or sets value for Selection Background.
        /// Which is bind to the Selection Border Background property.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        [Obsolete]
        public Brush RowHighlightBrush
        {
            get { return (Brush)GetValue(RowHighlightBrushProperty); }
            set { SetValue(RowHighlightBrushProperty, value); }
        }

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Dependeny registration for SelectionBackground.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty RowHighlightBrushProperty =
            DependencyProperty.Register("RowHighlightBrush", typeof(Brush), typeof(VirtualizingCellsControl), new PropertyMetadata(new SolidColorBrush(Colors.Gainsboro)));
#else
        /// <summary>
        /// Dependeny registration for SelectionBackground.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty RowHighlightBrushProperty =
            DependencyProperty.Register("RowHighlightBrush", typeof(Brush), typeof(VirtualizingCellsControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
#endif


        /// <summary>
        /// Gets or sets value for Selection Background.
        /// Which is bind to the Selection Border Background property.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Brush RowHoverBackgroundBrush
        {
            get { return (Brush)GetValue(RowHoverBackgroundBrushProperty); }
            set { SetValue(RowHoverBackgroundBrushProperty, value); }
        }

        /// <summary>
        /// Dependeny registration for SelectionBackground.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty RowHoverBackgroundBrushProperty =
            DependencyProperty.Register("RowHoverBackgroundBrush", typeof(Brush), typeof(VirtualizingCellsControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// Gets or sets value for Highlight Border Thickness.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Thickness RowHighlightBorderThickness
        {
            get { return (Thickness)GetValue(RowHighlightBorderThicknessProperty); }
            set { SetValue(RowHighlightBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Dependeny registration for SelectionBackground.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty RowHighlightBorderThicknessProperty =
            DependencyProperty.Register("RowHighlightBorderThickness", typeof(Thickness), typeof(VirtualizingCellsControl), new PropertyMetadata(new Thickness(1)));

        /// <summary>
        /// Gets or sets value for Selection Background.
        /// Which is bind to the Selection Border Background property.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Brush RowSelectionBrush
        {
            get { return (Brush)GetValue(RowSelectionBrushProperty); }
            set { SetValue(RowSelectionBrushProperty, value); }
        }

        /// <summary>
        /// Dependeny registration for SelectionBackground.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty RowSelectionBrushProperty =
            DependencyProperty.Register("RowSelectionBrush", typeof(Brush), typeof(VirtualizingCellsControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Gets or sets the GroupCaptionRowSelectionBrush
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Brush GroupRowSelectionBrush
        {
            get { return (Brush)GetValue(GroupRowSelectionBrushProperty); }
            set { SetValue(GroupRowSelectionBrushProperty, value); }
        }

        public static readonly DependencyProperty GroupRowSelectionBrushProperty =
            DependencyProperty.Register("GroupRowSelectionBrush", typeof(Brush), typeof(VirtualizingCellsControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(100, 120, 120, 120))));

        public RectangleGeometry SelectionBorderClipRect
        {
            get { return (RectangleGeometry)GetValue(SelectionBorderClipRectProperty); }
            set { SetValue(SelectionBorderClipRectProperty, value); }
        }

        public static readonly DependencyProperty SelectionBorderClipRectProperty =
            DependencyProperty.Register("SelectionBorderClipRect", typeof(RectangleGeometry), typeof(VirtualizingCellsControl), new PropertyMetadata(null));

        public RectangleGeometry HighlightBorderClipRect
        {
            get { return (RectangleGeometry)GetValue(HighlightBorderClipRectProperty); }
            set { SetValue(HighlightBorderClipRectProperty, value); }
        }

        public static readonly DependencyProperty HighlightBorderClipRectProperty =
            DependencyProperty.Register("HighlightBorderClipRect", typeof(RectangleGeometry), typeof(VirtualizingCellsControl), new PropertyMetadata(null));

        public RectangleGeometry RowBackgroundClip
        {
            get { return (RectangleGeometry)GetValue(RowBackgroundClipProperty); }
            set { SetValue(RowBackgroundClipProperty, value); }
        }

        public static readonly DependencyProperty RowBackgroundClipProperty =
            DependencyProperty.Register("RowBackgroundClip", typeof(RectangleGeometry), typeof(VirtualizingCellsControl), new PropertyMetadata(null));

        /// <summary>
        /// Property which decides whethe Focus border is visible or not.
        /// </summary>
        public Visibility CurrentFocusRowVisibility
        {
            get { return (Visibility)GetValue(CurrentFocusRowVisibilityProperty); }
            set { SetValue(CurrentFocusRowVisibilityProperty, value); }
        }

        public static readonly DependencyProperty CurrentFocusRowVisibilityProperty =
            DependencyProperty.Register("CurrentFocusRowVisibility", typeof(Visibility), typeof(VirtualizingCellsControl), new PropertyMetadata(Visibility.Collapsed, OnCurrentFocusRowVisiblityChanged));

        /// <summary>
        /// Preperty which holds the margin value to avoid the overlapping in Indent Cell.
        /// </summary>
        public Thickness CurrentFocusBorderMargin
        {
            get { return (Thickness)GetValue(CurrentFocusBorderMarginProperty); }
            set { SetValue(CurrentFocusBorderMarginProperty, value); }
        }

        public static readonly DependencyProperty CurrentFocusBorderMarginProperty =
            DependencyProperty.Register("CurrentFocusBorderMargin", typeof(Thickness), typeof(VirtualizingCellsControl), new PropertyMetadata(new Thickness(2, 2, 2, 2)));

        #endregion

        #region Dependency Call Back

        private static void OnSelectionBorderVisiblityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var rowControl = obj as VirtualizingCellsControl;
            rowControl.UpdateSelectionBorderClip();
        }

        private static void OnHighlightBorderVisiblityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rowControl = d as VirtualizingCellsControl;
            rowControl.UpdateHighlightBorderClip();
        }

        private static void OnCurrentFocusRowVisiblityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var rowControl = obj as VirtualizingCellsControl;
            rowControl.UpdateFocusRowPosition();
        }

        #endregion

        #region Ctor

        public VirtualizingCellsControl()
        {
            this.DefaultStyleKey = typeof(VirtualizingCellsControl);
            SetContent();
            this.IsTabStop = false;
            WireEvents();
        }

        protected virtual void SetContent()
        {
            this.Content = this.ItemsPanel = new OrientedCellsPanel();
        }

        #endregion

        #region Override

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            if (!(this is AddNewRowControl))
                this.ApplyValidationVisualState(HasError);
        }

#if WinRT
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
            base.OnPointerReleased(e);
        }
#endif

#if WinRT
        protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#else
        protected override void OnMouseEnter(MouseEventArgs e)   
#endif
        {
            if (AllowRowHoverHighlighting!=null && AllowRowHoverHighlighting())
                this.HighlightSelectionBorderVisiblity = Visibility.Visible;
#if WinRT
            base.OnPointerEntered(e);
#else
            base.OnMouseEnter(e);
#endif
        }

        
#if WinRT
        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#else
        protected override void OnMouseLeave(MouseEventArgs e)
#endif
        {
            if (AllowRowHoverHighlighting != null && AllowRowHoverHighlighting())
                this.HighlightSelectionBorderVisiblity = Visibility.Collapsed;
#if WinRT
            base.OnPointerExited(e);
#else
            base.OnMouseLeave(e);
#endif
        }

        #endregion

        #region internal methods

        internal void InitializeVirtualizingRowControl(Func<IList<IColumnElement>> columns, Func<int, VisibleLineInfo> getColumnVisibleLineInfo, Func<int, bool, double> getColumnSize)
        {
            var orientedCellsPanel = this.ItemsPanel as OrientedCellsPanel;
            if (orientedCellsPanel == null) return;
            orientedCellsPanel.GetVisibleColumns = columns;
            orientedCellsPanel.GetColumnVisibleLineInfo = getColumnVisibleLineInfo;
            orientedCellsPanel.GetVisibleColumnSize = getColumnSize;
        }        

        internal void UpdateSelectionBorderClip()
        {
            if (this.SelectionBorderVisiblity == Visibility.Visible && this.GetVisibleLineOrigin!=null)
            {
                double origin = this.GetVisibleLineOrigin();
                if (origin > 0)
                    this.SelectionBorderClipRect = new RectangleGeometry() { Rect = new Rect(new Point(origin, 0),new Size(this.ActualWidth,this.ActualHeight)) };
                else
                    this.SelectionBorderClipRect = null;
            }
            else
            {
                this.SelectionBorderClipRect = null;
            }
        }

        internal void UpdateHighlightBorderClip()
        {
            if (this.HighlightSelectionBorderVisiblity != Visibility.Visible ||
                this.GetVisibleLineOrigin == null) return;
            double orgin = this.GetVisibleLineOrigin();
            if (orgin >= 0)
            {
                this.HighlightBorderClipRect = new RectangleGeometry() { Rect = new Rect(new Point(orgin, 0), new Size(this.ActualWidth, this.ActualHeight)) };
            }
        }

        internal void UpdateRowBackgroundClip()
        {
            if (this.GetVisibleLineOrigin != null)
            {
                double origin = this.GetVisibleLineOrigin();
                if (origin > 0 && this.ActualWidth > 0 && (origin != oldOrigin || oldActualWidth != this.ActualWidth))
                {
                    RowBackgroundClip = new RectangleGeometry() { Rect = new Rect(new Point(origin, 0), new Size(this.ActualWidth, this.ActualHeight)) };
                    oldOrigin = origin;
                    oldActualWidth = ActualWidth;
                }
                else if (origin == 0 && RowBackgroundClip != null)
                {
                    RowBackgroundClip = null;
                    oldOrigin = origin;
                    oldActualWidth = ActualWidth;
                }
            }
        }

        /// <summary>
        /// Method which helps to update the CurrentFocus border position.
        /// </summary>
        internal void UpdateFocusRowPosition()
        {
            if (this.GetVisibleLineOrigin != null)
            {
                double origin = this.GetVisibleLineOrigin();
                CurrentFocusBorderMargin = new Thickness(origin + 2, 2, 2, 2);
            }
        }

        internal void SetError()
        {
            this.HasError = true;
            ApplyValidationVisualState(true);
        }

        internal void RemoveError()
        {
            this.HasError = false;
            ApplyValidationVisualState(false);
        }

        internal void ApplyValidationVisualState(bool hasError)
        {
             if (hasError)
                VisualStateManager.GoToState(this, "HasError", true);
             else
                VisualStateManager.GoToState(this, "NoError", true);
        }

        #endregion

        #region Private Methods

        private void WireEvents()
        {
            this.Loaded += OnLoaded;
            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AllowRowHoverHighlighting != null && AllowRowHoverHighlighting() && e.PreviousSize.Width == 0 && e.PreviousSize.Height == 0)
                this.HighlightSelectionBorderVisiblity = Visibility.Collapsed;
            this.UpdateRowBackgroundClip();
            this.UpdateSelectionBorderClip();
            this.UpdateFocusRowPosition();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.UpdateSelectionBorderClip();
        }

        private void UnwireEvents()
        {
            this.Loaded -= OnLoaded;
            this.SizeChanged -= OnSizeChanged;
        }

        #endregion

        public virtual void Dispose()
        {
            UnwireEvents();
            if(this.ItemsPanel!=null)
            {
                if (ItemsPanel is IDisposable)
                    (this.ItemsPanel as IDisposable).Dispose();
                this.ItemsPanel = null;
            }
            this.GetVisibleLineOrigin = null;
            this.AllowRowHoverHighlighting = null;
        }
    }

    [ClassReference(IsReviewed = false)]
    public class HeaderRowControl : VirtualizingCellsControl
    {
        #region Ctor

        public HeaderRowControl() : base()
        {
            this.DefaultStyleKey = typeof(HeaderRowControl);
        }

        #endregion
    }

    [ClassReference(IsReviewed = false)]
    public class TableSummaryRowControl : VirtualizingCellsControl
    {
        #region Dependency Region

        public TableSummaryRowType TableSummaryRowType
        {
            get { return (TableSummaryRowType)GetValue(TableSummaryRowTypeProperty); }
            set { SetValue(TableSummaryRowTypeProperty, value); }
        }

        public static readonly DependencyProperty TableSummaryRowTypeProperty =
            DependencyProperty.Register("TableSummaryRowType", typeof(TableSummaryRowType), typeof(TableSummaryRowControl), new PropertyMetadata(TableSummaryRowType.FooterSummaryRow, OnIsLastRowChanged));

        #endregion
       
        #region Ctor

        public TableSummaryRowControl() : base()
        {
            this.DefaultStyleKey = typeof(TableSummaryRowControl);
        }

        #endregion

        #region Overrides

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.ApplyVisualState(TableSummaryRowType);
        }

        #endregion

        #region Dependency Call Back

        private static void OnIsLastRowChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var row = obj as TableSummaryRowControl;
            if (row != null) row.ApplyVisualState((TableSummaryRowType)args.NewValue);
        }

        #endregion

        #region Private Methods

        private void ApplyVisualState(TableSummaryRowType value)
        {
            if (value == TableSummaryRowType.LastFooterSummaryRow)
                VisualStateManager.GoToState(this, "LastFooterRow", false);
            else if (value == TableSummaryRowType.HeaderSummaryRow)
                VisualStateManager.GoToState(this, "HeaderRow", false);
            else
                VisualStateManager.GoToState(this, "FooterRow", false);
        }

        #endregion
    }

    [ClassReference(IsReviewed = false)]
#if !WinRT
    [TemplatePart(Name = "PART_CaptionSummaryRowGrid", Type = typeof(System.Windows.Controls.Grid))]
#else
    [TemplatePart(Name = "PART_CaptionSummaryRowGrid", Type = typeof(Windows.UI.Xaml.Controls.Grid))]
#endif
    [TemplatePart(Name = "PART_CaptionSummaryRowBorder", Type = typeof(Border))]
    public class CaptionSummaryRowControl : VirtualizingCellsControl
    {
        #region Fields
        
        private Func<int,bool, double> GetColumnSize;
        private Func<bool> ShowRowHeader;
        private Func<int, VisibleLineInfo> GetColumnVisibleLineInfo;
        Point pointerPressedPosition ;

        #endregion

        #region Internal Properties
#if WinRT
        internal Windows.UI.Xaml.Controls.Grid RowPanel;
#else
        internal System.Windows.Controls.Grid RowPanel;
#endif
        internal Border ExpanderBorder;
        internal ExpandeChanged IsExpandedChanged;
        internal delegate void ExpandeChanged(bool isExpand);
        internal Func<bool> CheckForValidation;

        #endregion

        #region Ctor

        public CaptionSummaryRowControl()
        {
            this.DefaultStyleKey = typeof(CaptionSummaryRowControl);
        }

        #endregion

        #region Dependency Registration

        /// <summary>
        /// Gets or sets a value indicating whether this instance Expanded or Collapsed
        /// </summary>
        /// <value><see langword="true"/> if this instance ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(CaptionSummaryRowControl), new PropertyMetadata(false));

        #endregion

        #region Overrides

#if WinRT 
        protected override void OnApplyTemplate() 
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
#if WinRT
            RowPanel = this.GetTemplateChild("PART_CaptionSummaryRowGrid") as Windows.UI.Xaml.Controls.Grid;
#else
            RowPanel = this.GetTemplateChild("PART_CaptionSummaryRowGrid") as System.Windows.Controls.Grid;
#endif

            ExpanderBorder = this.GetTemplateChild("PART_CaptionSummaryRowBorder") as Border;

            var group = this.DataContext as Group;
            if (group != null)
                this.IsExpanded = group.IsExpanded;
        }

#if WinRT
        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            pointerPressedPosition = e.GetCurrentPoint(null).Position;
        }
#elif WPF
        protected override void OnPreviewMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            pointerPressedPosition = e.GetPosition(null);
        }
#else
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            pointerPressedPosition = e.GetPosition(null);
        }
#endif

#if WinRT    
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
           
            if (CheckForValidation != null && !CheckForValidation())
                return;

            var pointerReleasedRowPosition = e.GetCurrentPoint(null).Position;
            var ctrlKey = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control);
#elif WPF
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var element = GridUtil.FindDescendant(e.OriginalSource, typeof(GridRowHeaderCell));
            if (element != null)
                return;
            if (CheckForValidation != null && !CheckForValidation())
                return;
            var pointerReleasedRowPosition = e.GetPosition(null);
            var ctrlKey = (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
#else
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (CheckForValidation != null && !CheckForValidation())
                return;
            var pointerReleasedRowPosition = e.GetPosition(null);
            var ctrlKey = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
#endif

            double xPosChange = Math.Abs(pointerReleasedRowPosition.X - pointerPressedPosition.X);
            double yPosChange = Math.Abs(pointerReleasedRowPosition.Y - pointerPressedPosition.Y);

            if (xPosChange < 30 && yPosChange < 30)
            {
                if (this.DataContext is Group)
                {
#if WinRT
                    if (ctrlKey != CoreVirtualKeyStates.Down)
#else
                    if (!ctrlKey)
#endif
                    {
                        var group = this.DataContext as Group;
                        this.IsExpanded = !@group.IsExpanded;
                        this.IsExpandedChanged(this.IsExpanded);
                    }
                }
            }
            
#if WPF
            base.OnPreviewMouseLeftButtonUp(e);
#elif WinRT
            e.Handled = true;
            base.OnPointerReleased(e);
#else
            base.OnMouseLeftButtonUp(e);
#endif
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.RowPanel != null && this.GetColumnVisibleLineInfo != null)
            {
                var group = this.DataContext as Group;
                var line = this.GetColumnVisibleLineInfo(group.Level+ (this.ShowRowHeader() ? 1 : 0) - 1);
                ExpanderBorder.Width = GetColumnSize(group.Level + (this.ShowRowHeader() ? 1 : 0) - 1, false);
                if (this.ShowRowHeader())
                {
                    if (line != null)
                    {
                        var rowHeaderSize = GetColumnSize(0, false);
                        if (rowHeaderSize > line.Origin)
                            ExpanderBorder.Clip = new RectangleGeometry() { Rect = new Rect((rowHeaderSize - line.Origin), 0, ExpanderBorder.Width, finalSize.Height) };
                        else
                            ExpanderBorder.Clip = null;
                    }
                }
                if (line != null)
                    this.RowPanel.RenderTransform = new TranslateTransform() { X = line.Origin, Y = 0 };
            }
            return base.ArrangeOverride(finalSize);
        }

        #endregion

        #region Internal Methods

        internal void UpdateVisibleColumn(Func<IList<IColumnElement>> columns,Func<bool> showRowHeader, Func<int, VisibleLineInfo> getColumnVisibleLineInfo, Func<int,bool, double> getColumnSize)
        {
            this.GetColumnSize = getColumnSize;
            this.ShowRowHeader = showRowHeader;
            this.GetColumnVisibleLineInfo = getColumnVisibleLineInfo;
            base.InitializeVirtualizingRowControl(columns, getColumnVisibleLineInfo, getColumnSize);
        }

        #endregion

		#region Disopse
		
        public override void Dispose()
        {
            this.ExpanderBorder = null;
            this.GetColumnSize = null;
            this.GetColumnVisibleLineInfo = null;
            this.IsExpandedChanged = null;
            this.RowPanel = null;
            base.Dispose();
        }
		
		#endregion
    }

    [ClassReference(IsReviewed = false)]
    public class GroupSummaryRowControl : VirtualizingCellsControl
    {
        #region Ctor

        public GroupSummaryRowControl()
        {
            this.DefaultStyleKey = typeof(GroupSummaryRowControl);
        }

        #endregion

        #region Internal Methods

        internal void UpdateVisibleColumns(Func<IList<IColumnElement>> columns, Func<int, VisibleLineInfo> getColumnVisibleLineInfo, Func<int,bool, double> getColumnSize)
        {
            base.InitializeVirtualizingRowControl(columns, getColumnVisibleLineInfo, getColumnSize);
        }

        #endregion
    }

    [ClassReference(IsReviewed = false)]
    public class AddNewRowControl : VirtualizingCellsControl
    {   
        #region Ctor

        public AddNewRowControl():base()
        {
            DefaultStyleKey = typeof(AddNewRowControl);
        }

        #endregion

        #region Dependency Property

        /// <summary>
        /// Get or Set the text displayed in AddNewROw watermark.
        /// </summary>
        public string AddNewRowText
        {
            get { return (string)GetValue(AddNewRowTextProperty); }
            set { SetValue(AddNewRowTextProperty, value); }
        }

        public static readonly DependencyProperty AddNewRowTextProperty =
            DependencyProperty.Register("AddNewRowText", typeof(string), typeof(AddNewRowControl), new PropertyMetadata(GridResourceWrapper.AddNewRowText));

        /// <summary>
        /// Property which helps to position the AddNewRow text when the row header is displayed.
        /// </summary>
        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(AddNewRowControl), new PropertyMetadata(null));

        public RectangleGeometry  TextBorderClip
        {
            get { return (RectangleGeometry )GetValue(TextBorderClipProperty); }
            set { SetValue(TextBorderClipProperty, value); }
        }

        public static readonly DependencyProperty TextBorderClipProperty =
            DependencyProperty.Register("TextBorderClip", typeof(RectangleGeometry ), typeof(AddNewRowControl), new PropertyMetadata(null));
        
        #endregion

        #region Overrides
#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            WireEvents();
        }

        public override void Dispose()
        {
            base.Dispose();
            UnwireEvents();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the WaterMarkText wrapper clipping.
        /// </summary>
        internal void UpdateTextBorder()
        {
            if (this.GetVisibleLineOrigin != null)
            {
                double origin = this.GetVisibleLineOrigin();
                if (origin > 0)
                {
#if !WinRT
                    this.TextMargin = new Thickness(origin + 3, 0, 0, 0);
#else
                    this.TextMargin = new Thickness(origin + 8, 0, 0, 0);
#endif
                    this.TextBorderClip = new RectangleGeometry() { Rect = new Rect(new Point(origin, 0), new Size(this.DesiredSize.Width, this.DesiredSize.Height)) };
                }
                else
                {
#if !WinRT
                    this.TextMargin = new Thickness(3, 0, 0, 0);
#else
                    this.TextMargin = new Thickness(8, 0, 0, 0);
#endif
                    this.TextBorderClip = null;
                }
            }
        }

        private void WireEvents()
        {
            this.Loaded += OnLoaded;
        }

        /// <summary>
        /// Method which helps to modify the AddNewRow row initial state and clicpping.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
                VisualStateManager.GoToState(this, "Edit", true);
            UpdateTextBorder();
        }

        private void UnwireEvents()
        {
            this.Loaded -= OnLoaded;
        }

        #endregion
    }
}
