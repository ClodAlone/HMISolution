#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
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
    using System.ComponentModel;
    using System.Windows.Data;


    public class GridTreeHeaderCellControl : Control
    {
        
        private System.Windows.Controls.Grid mainGrid;
        public const double MinimumWidth = 25.0d;
        public ContentPresenter PART_ContentPresenter { get; private set; }
        private Border mainBorder;

        public GridTreeHeaderCellControl()
        {
            this.DefaultStyleKey = typeof(GridTreeHeaderCellControl);
           
        }

        #region SortVisibility
        
        /// <summary>
        /// DependencyProperty for SortVisibility.
        /// </summary>
        public static readonly DependencyProperty SortVisibilityProperty = DependencyProperty.Register("SortVisibility",typeof(Visibility),typeof(GridTreeHeaderCellControl),new PropertyMetadata(Visibility.Collapsed, OnSortVisibilityChanged));

        /// <summary>
        /// Gets or sets whether if the Sort Widget has to be shown.
        /// </summary>
        public Visibility SortVisibility
        {
            get
            {
                return (Visibility)this.GetValue(GridTreeHeaderCellControl.SortVisibilityProperty);
            }

            set
            {
                this.SetValue(GridTreeHeaderCellControl.SortVisibilityProperty, value);
            }
        }

        // private bool isSortVisibiltyChangedBeforeInitialization = false; Variable is assigned but it is never used

        private static void OnSortVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var headerCellControl = d as GridTreeHeaderCellControl;
            var value = (Visibility)args.NewValue;
            headerCellControl.SetSortStyle(value, headerCellControl.SortDirection);
        }

        #endregion

        #region SortDirection
        
        /// <summary>
        /// DependencyProperty for SortDirection.
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register(
            "SortDirection",
            typeof(ListSortDirection),
            typeof(GridTreeHeaderCellControl),
            new FrameworkPropertyMetadata(OnSortDirectionChanged));

        public ListSortDirection SortDirection
        {
            get
            {
                return (ListSortDirection)this.GetValue(GridTreeHeaderCellControl.SortDirectionProperty);
            }

            set
            {
                this.SetValue(GridTreeHeaderCellControl.SortDirectionProperty, value);
            }
        }

        private static void OnSortDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var headerCellControl = d as GridTreeHeaderCellControl;
            var sortDirection = (ListSortDirection)args.NewValue;
            headerCellControl.SetSortStyle(headerCellControl.SortVisibility, sortDirection);
        }

        #endregion
        
        #region SortPath
        
        public static readonly DependencyProperty SortPathProperty = DependencyProperty.Register(
            "SortPath",
            typeof(Geometry),
            typeof(GridTreeHeaderCellControl));

        public Geometry SortPath
        {
            get
            {
                return (Geometry)this.GetValue(GridTreeHeaderCellControl.SortPathProperty);
            }

            set
            {
                this.SetValue(GridTreeHeaderCellControl.SortPathProperty, value);
            }
        }

        #endregion

        #region SortBrush
        

        /// <summary>
        /// DependencyProperty for SortBrush.
        /// </summary>
        public static readonly DependencyProperty SortBrushProperty = DependencyProperty.Register(
            "SortBrush",
            typeof(Brush),
            typeof(GridTreeHeaderCellControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the brush value for the Sort Widget.
        /// </summary>
        public Brush SortBrush
        {
            get
            {
                return (Brush)this.GetValue(GridTreeHeaderCellControl.SortBrushProperty);
            }

            set
            {
                this.SetValue(GridTreeHeaderCellControl.SortBrushProperty, value);
            }
        }

        #endregion

        #region HoverBackground
        /// <summary>
        /// Header Hover Background Brush
        /// </summary>

        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register(
          "HoverBackground",
          typeof(Brush),
          typeof(GridTreeHeaderCellControl),
          new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush HoverBackground
        {
            get
            {
                return (Brush)this.GetValue(GridTreeHeaderCellControl.HoverBackgroundProperty);
            }

            set
            {
                this.SetValue(GridTreeHeaderCellControl.HoverBackgroundProperty, value);
            }
        }
        #endregion
        
        #region HoverForeground
        /// <summary>
        /// Header Hover Foreground Brush
        /// </summary>
        public Brush HoverForeground
        {
            get
            {
                return (Brush)this.GetValue(GridTreeHeaderCellControl.HoverForegroundProperty);
            }
            set
            {
                this.SetValue(GridTreeHeaderCellControl.HoverForegroundProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for HoverForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoverForegroundProperty =
            DependencyProperty.Register("HoverForeground", typeof(Brush), typeof(GridTreeHeaderCellControl), new FrameworkPropertyMetadata(Brushes.Transparent));
        #endregion
        
        #region SortWidgetBorderBrush
        /// <summary>
        /// Dependency Property for SortWidgetBorder Brush
        /// </summary>

        public Brush SortWidgetBorderBrush
        {
            get { return (Brush)GetValue(SortWidgetBorderBrushProperty); }
            set { SetValue(SortWidgetBorderBrushProperty, value); }
        }

       
        public static readonly DependencyProperty SortWidgetBorderBrushProperty =
            DependencyProperty.Register("SortWidgetBorderBrush", typeof(Brush), typeof(GridTreeHeaderCellControl), new PropertyMetadata(Brushes.Transparent));
        #endregion

        #region DataTemplate

        public static readonly DependencyProperty ContentDataTemplateProperty =
            DependencyProperty.Register("ContentDataTemplate", typeof (DataTemplate), typeof (GridTreeHeaderCellControl),
                                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the custom data template.
        /// </summary>
        /// <value>The custom data template.</value>
        public DataTemplate ContentDataTemplate
        {
            get { return (DataTemplate)this.GetValue(GridTreeHeaderCellControl.ContentDataTemplateProperty); }
            set { this.SetValue(GridTreeHeaderCellControl.ContentDataTemplateProperty, value); }
        }

        #endregion

        #region SortWidgetBorder

        public Brush SortWidgetBorderHoverBackgroundBrush
        {
            get { return (Brush) GetValue(SortWidgetBorderHoverBackgroundBrushProperty); }
            set { SetValue(SortWidgetBorderHoverBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty SortWidgetBorderHoverBackgroundBrushProperty =
            DependencyProperty.Register("SortWidgetBorderHoverBackgroundBrush", typeof (Brush),
                                        typeof (GridTreeHeaderCellControl), new PropertyMetadata(Brushes.Transparent));

        #endregion

        #region TextProperty

        /// <summary>
        /// DependencyProperty for Text.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof (string),
            typeof (GridTreeHeaderCellControl),
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return (string) this.GetValue(TextProperty); }

            set { this.SetValue(TextProperty, value); }
        }

        #endregion

        private void SetSortStyle(Visibility sortVisibility, ListSortDirection sortDirection)
        {
            switch (sortVisibility)
            {
                case Visibility.Visible:
                    if (sortDirection == ListSortDirection.Ascending)
                    {
                        this.SortPath = GridDataResourceWrapper.BorderPathAsc;
                    }
                    else if (sortDirection == ListSortDirection.Descending)
                    {
                        this.SortPath = GridDataResourceWrapper.BorderPathDesc;
                    }
                    break;
                default:
                    this.SortPath = null;
                    break;
            }
        }
        
        public bool IsInSuspend
        {
            get;
            internal set;

        }
        public GridRenderStyleInfo RenderStyle
        {
            get;
            internal set;
        }

        private bool isTemplateApplied = false;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.mainBorder = this.GetTemplateChild("PART_MainBorder") as Border;
            this.mainGrid = this.GetTemplateChild("PART_MainGrid") as System.Windows.Controls.Grid;

            this.PART_ContentPresenter = this.GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
            
            if (PART_ContentPresenter != null)
            {
                ApplyStyle(this.RenderStyle);   
            }

            this.isTemplateApplied = true;
            //this.SetProperties();

        }

        private void ApplyStyle(GridStyleInfo style)
        {
            if (style != null)
            {
                var uiElement = this.PART_ContentPresenter;
                Thickness margins = style.TextMargins.ToThickness();
                if (style.HasImageIndex)
                {
                    margins = style.AdjustImageWidthAndHeightToMargin(margins, this.RenderStyle.GridControl);
                }
                else
                {
                    margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, this.RenderStyle.GridControl, style.CellRowColumnIndex);
                }
                uiElement.SetValue(TextBox.MarginProperty, margins);
                GridFontInfo font = style.ReadOnlyFont;
                uiElement.SetValue(TextBox.FontFamilyProperty, font.FontFamily);
                uiElement.SetValue(TextBox.FontSizeProperty, font.FontSize);
                uiElement.SetValue(TextBox.FontStretchProperty, font.FontStretch);
                uiElement.SetValue(TextBox.FontWeightProperty, font.FontWeight);
                uiElement.SetValue(TextBox.FontStyleProperty, font.FontStyle);
                uiElement.SetValue(TextBox.TextDecorationsProperty, font.TextDecorations);
                if (font.Orientation != 0)
                    uiElement.RenderTransform = new RotateTransform(font.Orientation);

                var dataGrid = this.FindParentElementOfType<GridDataControl>();
                if (dataGrid != null)
                {
                    if (dataGrid.StackedHeaderRows.Count != 0)
                        uiElement.SetValue(TextBox.HorizontalAlignmentProperty, style.HorizontalAlignment);
                }
                uiElement.SetValue(TextBox.HorizontalContentAlignmentProperty, style.HorizontalAlignment);
                uiElement.SetValue(TextBox.VerticalAlignmentProperty, style.VerticalAlignment);
                uiElement.SetValue(TextBox.VerticalContentAlignmentProperty, style.VerticalAlignment);
                uiElement.SetValue(TextBox.TextWrappingProperty, style.TextWrapping);
                uiElement.Tag = style.TextWrapping;
                uiElement.SetValue(TextBox.CharacterCasingProperty, style.CharacterCasing);
                uiElement.SetValue(TextBox.AutoWordSelectionProperty, style.AutoWordSelection);
                uiElement.SetValue(TextBox.AcceptsReturnProperty, style.AcceptsReturn);
            }
        }

    }

    public class GridTreeMinWidthConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibility = (Visibility)value;
            if (visibility == Visibility.Visible)
            {
                return GridTreeHeaderCellControl.MinimumWidth;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
