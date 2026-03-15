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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using Syncfusion.Windows.Shared;
    using Syncfusion.Windows.Documents;
    using Syncfusion.Windows.Controls.Grid;
    using System.Printing;
    using System.Windows.Xps;
    using Syncfusion.Windows.GridCommon;
    using System.Windows.Controls.Primitives;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Syncfusion.Windows.Controls.Grid.Resources;
    using System.Globalization;
    using System.Collections.Specialized;

    /// <summary>
    /// Implements a print dialog for the grid. It defines the designer for the Print dialog and exposes
    /// a number of properties and APIs to handle the UI requirements and define the interaction logic for 
    /// the Print Dialog. The users can use these properties to configure the Print and Print Preview options.
    /// </summary>
    public partial class GridPrintDialog : ChromelessWindow
    {
        private GridControlBase grid;
        /// <summary>
        /// Initializes a new <see cref="GridPrintDialog"/>.
        /// </summary>
        /// <param name="gridControlBase">The parent grid.</param>
        public GridPrintDialog(GridControlBase gridControlBase)
        {
            InitializeComponent();
            this.grid = gridControlBase;
            this.grid.PrintRange = GridRangeInfo.Table();
            var pDialog = new PrintDialog();
            pDialog.PrintTicket.PageMediaSize = new System.Printing.PageMediaSize(System.Printing.PageMediaSizeName.ISOA4);
            this.PrintSize = new System.Windows.Size(pDialog.PrintableAreaWidth, pDialog.PrintableAreaHeight);
            this.Loaded += new RoutedEventHandler(OnGridPrintDialogLoaded);
            this.btnOutputColor.LostFocus += this.btnOutputColor_LostFocus;
            this.btnZoom.LostFocus += this.btnZoom_LostFocus;
            this.btnPageSizes.LostFocus += this.btnPageSizes_LostFocus;
            this.FlowDirection = this.grid.FlowDirection;
        }

        protected virtual void ApplyTheme()
        {
            var gridPrintDlg = this;
            Grid layoutGrid = gridPrintDlg.FindName("LayoutRoot") as Grid;
            Border bottomBorder = gridPrintDlg.FindName("bottomBorder") as Border;
            LinearGradientBrush brush;
            string visualStyle = SkinStorage.GetVisualStyle(grid);
            switch (visualStyle)
            {
                case "Office2007Blue":
                case "DefaultOffice2007Blue":
                case "BureauBlue":
                case "TwilightBlue":
                case "ShinyBlue":
                    brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF395286"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF111E4D"), 1d)
                });
                    brush.StartPoint = new Point(0.5, 0.0597032);
                    brush.EndPoint = new Point(0.5, 0.911615);
                    SkinStorage.SetVisualStyle(gridPrintDlg, "Office2007Blue");
                    layoutGrid.Background = brush;
                    bottomBorder.Background = brush;
                    break;
                case "DefaultOffice2007Silver":
                case "Office2007Silver":
                    brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFBDC1C8"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FFEDEFF0"), 1d)
                });
                    brush.StartPoint = new Point(0.5, 0.940303);
                    brush.EndPoint = new Point(0.5, 0.0883908);
                    SkinStorage.SetVisualStyle(gridPrintDlg, "Office2007Silver");
                    layoutGrid.Background = brush;
                    txtCurrentPage.Foreground = Brushes.Black;
                    bottomBorder.Background = brush;
                    break;
                case "DefaultOffice2007Black":
                case "Office2007Black":
                case "SunBlack":
                case "ShinyRed":
                case "GlassyGreen":
                    brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF2A2A2A"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF010101"), 1d)
                });
                    brush.StartPoint = new Point(0.5, 0.0280268);
                    brush.EndPoint = new Point(0.5, 0.951414);
                    SkinStorage.SetVisualStyle(gridPrintDlg, "Office2007Black");
                    layoutGrid.Background = brush;
                    bottomBorder.Background = brush;
                    break;
                case "Blend":
                case "BureauBlack":
                    SkinStorage.SetVisualStyle(gridPrintDlg, "Blend");
                    brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF595959"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF424242"), 1d)
                });
                    brush.StartPoint = new Point(0.5, 0.0732399);
                    brush.EndPoint = new Point(0.5, 1.02406);
                    layoutGrid.Background = brush;
                    bottomBorder.Background = brush;
                    break;
                case "Office2003":
                    brush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF578FD4"), 0d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF538ACF"), 0.318681d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF3360A2"), 0.604396d),
                    new GradientStop(GridUtil.GetXamlConvertedValue<Color>("#FF305C9E"), 1d)
                });
                    brush.StartPoint = new Point(0.5, -0.0430693);
                    brush.EndPoint = new Point(0.5, 0.928826);
                    SkinStorage.SetVisualStyle(gridPrintDlg, "Office2003");
                    layoutGrid.Background = brush;
                    bottomBorder.Background = brush;

                    break;
                case "Default":
                    SkinStorage.SetVisualStyle(gridPrintDlg, "Default");
                    Brush systemBrush = SystemColors.ControlBrush;
                    layoutGrid.Background = systemBrush;
                    bottomBorder.Background = systemBrush;
                    break;
            }
            layoutBorder.Background = Brushes.WhiteSmoke;
        }

        void btnPageSizes_LostFocus(object sender, RoutedEventArgs e)
        {
            this.btnPageSizes.ClosePopup();
        }

        void btnZoom_LostFocus(object sender, RoutedEventArgs e)
        {
            this.btnZoom.ClosePopup();
        }

        void btnOutputColor_LostFocus(object sender, RoutedEventArgs e)
        {
            this.btnOutputColor.ClosePopup();
        }

        class ToggleStateManager
        {
            ObservableCollection<ToggleButton> buttons;
            public ToggleStateManager()
            {
                this.buttons = new ObservableCollection<ToggleButton>();
                this.buttons.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(buttons_CollectionChanged);
            }

            public ObservableCollection<ToggleButton> Buttons
            {
                get
                {
                    return this.buttons;
                }
            }

            void buttons_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    var button = e.NewItems[0] as ToggleButton;
                    var propDesc = DependencyPropertyDescriptor.FromProperty(ToggleButton.IsCheckedProperty, typeof(ToggleButton));
                    propDesc.AddValueChanged(button, OnIsCheckedChanged);
                }
            }

            private void OnIsCheckedChanged(object sender, EventArgs e)
            {
                var button = sender as ToggleButton;
                if (button.IsChecked == true)
                {
                    foreach (var btn in this.buttons)
                    {
                        if (btn != button)
                        {
                            btn.IsChecked = false;
                        }
                    }
                }
            }
        }

        ToggleStateManager toggleStateManager;
        private void OnGridPrintDialogLoaded(object sender, RoutedEventArgs e)
        {
            if (this.grid == null)
            {
                return;
            }

            var printPaginator = (IGridPrintPaginator)this.grid;
            printPaginator.SetPrintPageSize(this.PrintSize);
            var count = printPaginator.GetPrintTotalPageCount(this.PrintSize);
            if (count > 0)
            {
                this.CurrentPage = 0;

                var disabledPrev = this.TryFindResource("PreviousDisabled") as Brush;
                this.rectPrev.Fill = disabledPrev;

                var disabledFirst = this.TryFindResource("FirstDisabled") as Brush;
                this.rectFirst.Fill = disabledFirst;

                if (count == 1)
                {
                    var disabledNext = this.TryFindResource("NextDisabled") as Brush;
                    this.rectNext.Fill = disabledNext;

                    var disabledLast = this.TryFindResource("LastDisabled") as Brush;
                    this.rectLast.Fill = disabledLast;
                }
            }
            this.btnPageSizes.SplitListBox.Height = SystemParameters.PrimaryScreenHeight / 3;

            this.toggleStateManager = new ToggleStateManager();
            toggleStateManager.Buttons.Add(this.btnPageSizes.SplitToggleButton);
            toggleStateManager.Buttons.Add(this.btnOutputColor.SplitToggleButton);
            toggleStateManager.Buttons.Add(this.btnZoom.SplitToggleButton);
            this.ApplyTheme();
        }

        private static readonly DependencyProperty CultureProperty = DependencyProperty.Register(
            "Culture",
            typeof(CultureInfoReader),
            typeof(GridPrintDialog),
            new FrameworkPropertyMetadata(new CultureInfoReader()));

        internal CultureInfoReader Culture
        {
            get
            {
                return (CultureInfoReader)this.GetValue(GridPrintDialog.CultureProperty);
            }
        }

        private static readonly DependencyProperty PrintSizeProperty = DependencyProperty.Register(
            "PrintSize",
            typeof(System.Windows.Size),
            typeof(GridPrintDialog),
            new FrameworkPropertyMetadata(OnPrintSizeChanged));

        private static void OnPrintSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridPrintDlg = d as GridPrintDialog;
            gridPrintDlg.OnPrintSizeChanged((Size)args.NewValue);
        }

        private void OnPrintSizeChanged(Size size)
        {
            //this.layoutContent.MinWidth = this.PrintSize.Width;
            //this.layoutContent.MinHeight = this.PrintSize.Height;
        }

        /// <summary>
        /// Gets or sets the size of the content to be printed.
        /// </summary>
        public System.Windows.Size PrintSize
        {
            get
            {
                return (System.Windows.Size)this.GetValue(GridPrintDialog.PrintSizeProperty);
            }

            set
            {
                this.SetValue(GridPrintDialog.PrintSizeProperty, value);
            }
        }

        #region Navigation Properties

        private static readonly DependencyProperty CanMoveNextProperty = DependencyProperty.Register(
            "CanMoveNext",
            typeof(bool),
            typeof(GridPrintDialog),
            new FrameworkPropertyMetadata(OnCanMoveNextChanged));

        private static void OnCanMoveNextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridPrintDlg = d as GridPrintDialog;
            var value = (bool)args.NewValue;
            if (value)
            {
                var enabledNext = gridPrintDlg.TryFindResource("Next") as Brush;
                gridPrintDlg.rectNext.Fill = enabledNext;
            }
            else
            {
                var disabledNext = gridPrintDlg.TryFindResource("NextDisabled") as Brush;
                gridPrintDlg.rectNext.Fill = disabledNext;
            }
        }

        /// <summary>
        /// Specifies whether it is possible to navigate to the next page.
        /// </summary>
        public bool CanMoveNext
        {
            get
            {
                return (bool)this.GetValue(GridPrintDialog.CanMoveNextProperty);
            }
        }

        private static readonly DependencyProperty CanMovePreviousProperty = DependencyProperty.Register(
            "CanMovePrevious",
            typeof(bool),
            typeof(GridPrintDialog),
            new FrameworkPropertyMetadata(OnCanMovePreviousChanged));

        private static void OnCanMovePreviousChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridPrintDlg = d as GridPrintDialog;
            var value = (bool)args.NewValue;
            if (value)
            {
                var enabledPrev = gridPrintDlg.TryFindResource("Previous") as Brush;
                gridPrintDlg.rectPrev.Fill = enabledPrev;
            }
            else
            {
                var disabledPrev = gridPrintDlg.TryFindResource("PreviousDisabled") as Brush;
                gridPrintDlg.rectPrev.Fill = disabledPrev;
            }
        }

        /// <summary>
        /// Specifies whether it is possible to navigate to the previous page.
        /// </summary>
        public bool CanMovePrevious
        {
            get
            {
                return (bool)this.GetValue(GridPrintDialog.CanMovePreviousProperty);
            }
        }

        private static readonly DependencyProperty CanMoveLastProperty = DependencyProperty.Register(
            "CanMoveLast",
            typeof(bool),
            typeof(GridPrintDialog),
            new FrameworkPropertyMetadata(OnCanMoveLastChanged));

        private static void OnCanMoveLastChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridPrintDlg = d as GridPrintDialog;
            var value = (bool)args.NewValue;
            if (value)
            {
                var enabledLast = gridPrintDlg.TryFindResource("Last") as Brush;
                gridPrintDlg.rectLast.Fill = enabledLast;
            }
            else
            {
                var disabledLast = gridPrintDlg.TryFindResource("LastDisabled") as Brush;
                gridPrintDlg.rectLast.Fill = disabledLast;
            }
        }

        /// <summary>
        /// Specifies whether it is possible to navigate to the last page.
        /// </summary>
        public bool CanMoveLast
        {
            get
            {
                return (bool)this.GetValue(GridPrintDialog.CanMoveLastProperty);
            }
        }

        private static readonly DependencyProperty CanMoveFirstProperty = DependencyProperty.Register(
            "CanMoveFirst",
            typeof(bool),
            typeof(GridPrintDialog),
            new FrameworkPropertyMetadata(OnCanMoveFirstChanged));

        private static void OnCanMoveFirstChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridPrintDlg = d as GridPrintDialog;
            var value = (bool)args.NewValue;
            if (value)
            {
                var enabledFirst = gridPrintDlg.TryFindResource("First") as Brush;
                gridPrintDlg.rectFirst.Fill = enabledFirst;
            }
            else
            {
                var disabledFirst = gridPrintDlg.TryFindResource("FirstDisabled") as Brush;
                gridPrintDlg.rectFirst.Fill = disabledFirst;
            }
        }

        /// <summary>
        /// Specifies whether it is possible to navigate to the first page.
        /// </summary>
        public bool CanMoveFirst
        {
            get
            {
                return (bool)this.GetValue(GridPrintDialog.CanMoveFirstProperty);
            }
        }

        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register(
            "CurrentPage",
            typeof(int),
            typeof(GridPrintDialog),
            new FrameworkPropertyMetadata(-1, OnCurrentPageChanged));

        private static void OnCurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridPrintDlg = d as GridPrintDialog;
            var value = (int)args.NewValue;
            if (value > -1)
            {
                gridPrintDlg.SetPage(value);
                gridPrintDlg.SetValue(GridPrintDialog.CanMoveNextProperty, value != (gridPrintDlg.grid.PageCount - 1));
                gridPrintDlg.SetValue(GridPrintDialog.CanMoveLastProperty, value != (gridPrintDlg.grid.PageCount - 1));
                gridPrintDlg.SetValue(GridPrintDialog.CanMovePreviousProperty, value != 0);
                gridPrintDlg.SetValue(GridPrintDialog.CanMoveFirstProperty, value != 0);
            }
            else
            {
                gridPrintDlg.SetValue(GridPrintDialog.CanMoveNextProperty, false);
                gridPrintDlg.SetValue(GridPrintDialog.CanMoveLastProperty, false);
                gridPrintDlg.SetValue(GridPrintDialog.CanMovePreviousProperty, false);
                gridPrintDlg.SetValue(GridPrintDialog.CanMoveFirstProperty, false);
            }
        }

        private void SetPage(int page)
        {
            var printPaginator = this.grid as IGridPrintPaginator;
            var visual = printPaginator.GetPrintVisualAt(page, PrintSize); ;
            this.layoutContent.Content = visual;
            var totalPages = printPaginator.GetPrintTotalPageCount(this.PrintSize);
            this.txtCurrentPage.Text = string.Format(this.Culture.CurrentPageText, page + 1, totalPages);
        }

        /// <summary>
        /// Gets or sets the current printing page.
        /// </summary>
        public int CurrentPage
        {
            get
            {
                return (int)this.GetValue(GridPrintDialog.CurrentPageProperty);
            }

            set
            {
                this.SetValue(GridPrintDialog.CurrentPageProperty, value);
            }
        }

        public static readonly DependencyProperty PrintDataColumnNamesProperty = DependencyProperty.Register(
            "PrintDataColumnNames",
            typeof(StringCollection),
            typeof(GridPrintDialog),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Get or set the names of columns to be printed
        /// </summary>
        public StringCollection PrintDataColumnNames
        {
            get
            {
                return (StringCollection)this.GetValue(PrintDataColumnNamesProperty);
            }
            set
            {
                this.SetValue(PrintDataColumnNamesProperty, value);
            }
        }

        public static readonly DependencyProperty ScalingOptionsProperty = DependencyProperty.Register(
            "ScalingOptions",
            typeof(ScalingOptions),
            typeof(GridPrintDialog),
            new PropertyMetadata(ScalingOptions.NoScaling));

        /// <summary>
        /// Get or set the names of columns to be printed
        /// </summary>
        public ScalingOptions ScalingOptions
        {
            get
            {
                return (ScalingOptions)this.GetValue(ScalingOptionsProperty);
            }
            set
            {
                this.SetValue(ScalingOptionsProperty, value);
            }
        }

        private void movePrev_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentPage -= 1;
        }

        private void moveNext_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentPage += 1;
        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentPage = 0;
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            var printPaginator = this.grid as IGridPrintPaginator;
            var totalPages = printPaginator.GetPrintTotalPageCount(this.PrintSize);
            if (totalPages > 0)
            {
                this.CurrentPage = totalPages - 1;
            }
        }

        #endregion

        //private static readonly DependencyProperty CurrentOrientationProperty = DependencyProperty.Register(
        //    "CurrentOrientation",
        //    typeof(PageOrientation),
        //    typeof(GridPrintDialog),
        //    new FrameworkPropertyMetadata(PageOrientation.Landscape, OnOrientationChanged));

        //private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        //{
        //    var gridPrintDlg = d as GridPrintDialog;
        //    gridPrintDlg.OnOrientationChanged((PageOrientation)args.NewValue);
        //}

        //private void OnOrientationChanged(PageOrientation pageOrientation)
        //{
        //    this.SetPageSize(pageOrientation, this.CurrentPageMediaSize);
        //}

        //public PageOrientation CurrentOrientation
        //{
        //    get
        //    {
        //        return (PageOrientation)this.GetValue(GridPrintDialog.CurrentOrientationProperty);
        //    }
        //    set
        //    {
        //        this.SetValue(GridPrintDialog.CurrentOrientationProperty, value);
        //    }
        //}

        private void SetPageDimensions(PageMediaSizeName pageMediaSizeName)
        {
            this.CurrentPage = -1;
            var pDialog = new PrintDialog();
            pDialog.PrintTicket.PageMediaSize = new PageMediaSize(pageMediaSizeName);
            this.PrintSize = new System.Windows.Size(pDialog.PrintableAreaWidth, pDialog.PrintableAreaHeight);
            var printPaginator = this.grid as IGridPrintPaginator;
            printPaginator.SetPrintPageSize(this.PrintSize);
            var count = printPaginator.GetPrintTotalPageCount(this.PrintSize);
            if (count > 0)
            {
                this.CurrentPage = 0;
            }
        }

        private static readonly DependencyProperty CurrentPageMediaSizeProperty = DependencyProperty.Register(
            "CurrentPageMediaSize",
            typeof(PageMediaSizeName),
            typeof(GridPrintDialog),
            new FrameworkPropertyMetadata(PageMediaSizeName.ISOA4, OnPageMediaSizeChanged));

        private static void OnPageMediaSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridPrintDlg = d as GridPrintDialog;
            gridPrintDlg.OnPageMediaSizeChanged((PageMediaSizeName)args.NewValue);
        }

        private void OnPageMediaSizeChanged(PageMediaSizeName pageMediaSizeName)
        {
            this.SetPageDimensions(pageMediaSizeName);
        }

        /// <summary>
        /// Gets or sets the page size of the print media (eg. paper size).
        /// </summary>
        public PageMediaSizeName CurrentPageMediaSize
        {
            get
            {
                return (PageMediaSizeName)this.GetValue(GridPrintDialog.CurrentPageMediaSizeProperty);
            }

            set
            {
                this.SetValue(GridPrintDialog.CurrentPageMediaSizeProperty, value);
            }
        }

        private static readonly DependencyProperty CurrentOutputColorProperty = DependencyProperty.Register(
            "CurrentOutputColor",
            typeof(OutputColor),
            typeof(GridPrintDialog),
            new FrameworkPropertyMetadata(OutputColor.Color));

        /// <summary>
        /// Gets or sets the print color.
        /// </summary>
        public OutputColor CurrentOutputColor
        {
            get
            {
                return (OutputColor)this.GetValue(GridPrintDialog.CurrentOutputColorProperty);
            }

            set
            {
                this.SetValue(GridPrintDialog.CurrentOutputColorProperty, value);
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            printDialog.PrintTicket.PageMediaSize = new PageMediaSize(this.CurrentPageMediaSize);
            printDialog.PrintTicket.OutputColor = this.CurrentOutputColor;
            if (printDialog.ShowDialog() == true)
            {
                var printPaginator = (IGridPrintPaginator)this.grid;
                var tablePaginator = new GridPrintTablePaginator(this.PrintSize, printPaginator, printDialog);
                printDialog.PrintDocument(tablePaginator, printPaginator.PrintDescription);
            }
        }

        private static readonly DependencyProperty CurrentZoomFactorProperty = DependencyProperty.Register(
            "CurrentZoomFactor",
            typeof(object),
            typeof(GridPrintDialog),
            new FrameworkPropertyMetadata("100%", OnCurrentZoomChanged, CoerceCurrentZoomFactor));

        private static object CoerceCurrentZoomFactor(DependencyObject d, object value)
        {
            if (value != null)
            {
                var listBoxContent = ((ListBoxItem)value).Content;
                var zoomVal = listBoxContent.ToString();
                var pIdx = zoomVal.IndexOf("%");
                var res = zoomVal.Substring(0, pIdx);
                return res;
            }

            return value;
        }

        private static void OnCurrentZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridPrintDlg = d as GridPrintDialog;
            if (args.NewValue != null)
            {
                gridPrintDlg.OnCurrentZoomChanged(Convert.ToDouble(args.NewValue));
            }
        }

        /// <summary>
        /// Gets or sets the zoom factor for print preview.
        /// </summary>
        public object CurrentZoomFactor
        {
            get
            {
                return (string)this.GetValue(GridPrintDialog.CurrentZoomFactorProperty);
            }

            set
            {
                this.SetValue(GridPrintDialog.CurrentZoomFactorProperty, value);
            }
        }

        private void OnCurrentZoomChanged(double value)
        {
            var scaleFactor = value / 100;
            var scaleTransform = new ScaleTransform(scaleFactor, scaleFactor);
            var visual = this.layoutContentBorder as FrameworkElement;
            visual.LayoutTransform = scaleTransform;
        }

        internal class CultureInfoReader
        {
            public CultureInfoReader()
            {
                currentPageText = SR.GetString(CultureInfo.CurrentUICulture, "CurrentPageText");
                printText = SR.GetString(CultureInfo.CurrentUICulture, "PrintText");
                pageSizes = SR.GetString(CultureInfo.CurrentUICulture, "PageSizes");
                printOutputColor = SR.GetString(CultureInfo.CurrentUICulture, "PrintOutputColor");
                printZoom = SR.GetString(CultureInfo.CurrentUICulture, "PrintZoom");
                first = SR.GetString(CultureInfo.CurrentUICulture, "First");
                next = SR.GetString(CultureInfo.CurrentUICulture, "Next");
                previous = SR.GetString(CultureInfo.CurrentUICulture, "Previous");
                last = SR.GetString(CultureInfo.CurrentUICulture, "Last");
            }

            string currentPageText;
            public  string CurrentPageText
            {
                get
                {
                    return currentPageText;
                }
                set { currentPageText = value; }
            }

             string printText;
            public  string PrintText
            {
                get
                {
                    return printText;
                }
                set { printText = value; }
            }

             string pageSizes;
            public  string PageSizes
            {
                get
                {
                    return pageSizes;
                }
                set { pageSizes = value; }
            }

             string printOutputColor;
            public  string PrintOutputColor
            {
                get
                {
                    return printOutputColor;
                }
                set { printOutputColor = value; }
            }

             string printZoom;
            public  string PrintZoom
            {
                get
                {
                    return printZoom;
                }
                set { printZoom = value; }
            }

             string first;
            public  string First
            {
                get
                {
                    return first;
                }
                set { first = value; }
            }

             string next;
            public  string Next
            {
                get
                {
                    return next;
                }
                set { next = value; }
            }

             string previous;
            public  string Previous
            {
                get
                {
                    return previous;
                }
                set { previous = value; }
            }

             string last;
            public  string Last
            {
                get
                {
                    return last;
                }
                set { last = value; }
            }
        }
    }

    /// <summary>
    /// Defines extension methods to show the print dialog.
    /// </summary>
    public static class GridPrintExtensions
    {
        /// <summary>
        /// Displays the print dialog for the given grid control.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>A Nullable(T) value of type Boolean that signifies how a window was closed by the user.</returns>
        public static bool? ShowPrintDialog(this GridControlBase grid)
        {
            return ShowPrintDialog(grid, null);
        }

        /// <summary>
        /// Displays the print dialog for the given grid control.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="gridPrintAction">Creates a method that will open a GridPrintDialog.</param>
        /// <returns>A Nullable(T) value of type Boolean that signifies how a window was closed by the user.</returns>
        public static bool? ShowPrintDialog(this GridControlBase grid, Action<GridPrintDialog> gridPrintAction)
        {
            var gridPrintDlg = new GridPrintDialog(grid);
            Window w = grid.FindParentElementOfType<Window>();
            if (w != null)
            {
                gridPrintDlg.Owner = w;
                gridPrintDlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            gridPrintDlg.ScalingOptions = grid.ScalingOptions;
            if (gridPrintAction != null)
            {
                gridPrintAction(gridPrintDlg);
                grid.ScalingOptions = gridPrintDlg.ScalingOptions;
                return true;
            }
            grid.ScalingOptions = gridPrintDlg.ScalingOptions;
            var result = gridPrintDlg.ShowDialog();
            return result;
        }

        //private static GridPrintDialog GetDialog(GridControlBase grid, Action<GridPrintDialog> gridPrintAction)
        //{
        //    var gridPrintDlg = new GridPrintDialog(grid);
        //    LinearGradientBrush brush;
        //    if (gridPrintAction != null)
        //    {
        //        gridPrintAction(gridPrintDlg);
        //    }

        //    else
        //    {

        //        gridPrintDlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        //    }

        //    return gridPrintDlg;
        //}

        /// <summary>
        /// Displays the print dialog for the given grid data control.
        /// </summary>
        /// <param name="grid">The grid data control.</param>
        /// <returns>A Nullable(T) value of type Boolean that signifies how a window was closed by the user.</returns>
        public static bool? ShowPrintDialog(this GridDataControl grid)
        {
            var internalGrid = grid.InternalGrid;
            return internalGrid.ShowPrintDialog();
        }

        /// <summary>
        /// Displays the print dialog for the given grid data control.
        /// </summary>
        /// <param name="grid">The grid data control.</param>
        /// <param name="gridPrintAction">Creates a method that will open a GridPrintDialog.</param>
        /// <returns>A Nullable(T) value of type Boolean that signifies how a window was closed by the user.</returns>
        public static bool? ShowPrintDialog(this GridDataControl grid, Action<GridPrintDialog> gridPrintAction)
        {
            int colloc = 0;
            var gridPrintDlg = new GridPrintDialog(grid.InternalGrid);

            Window w = grid.FindParentElementOfType<Window>();
            if (w != null)
            {
                gridPrintDlg.Owner = w;
                gridPrintDlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            gridPrintDlg.ScalingOptions = grid.InternalGrid.ScalingOptions;
            if (gridPrintAction != null)
            {
                gridPrintAction(gridPrintDlg);
            }
            if (gridPrintDlg.PrintDataColumnNames != null)
            {
                grid.InternalGrid.PrintColumns = new int[gridPrintDlg.PrintDataColumnNames.Count];
                foreach (string colname in gridPrintDlg.PrintDataColumnNames)
                {
                    grid.InternalGrid.PrintColumns[colloc] = grid.Model.ResolveVisibleColumnIndexToPosition(grid.VisibleColumns.IndexOf(grid.VisibleColumns[colname]));
                    colloc++;
                }
            }
            grid.InternalGrid.ScalingOptions = gridPrintDlg.ScalingOptions;
            var result = gridPrintDlg.ShowDialog();
            return result;
        }

        /// <summary>
        /// Displays the print dialog for the given grid tree control.
        /// </summary>
        /// <param name="grid">The tree grid.</param>
        /// <returns>A Nullable(T) value of type Boolean that signifies how a window was closed by the user.</returns>
        public static bool? ShowPrintDialog(this GridTreeControl grid)
        {
            var internalGrid = grid.InternalGrid;
            return internalGrid.ShowPrintDialog();
        }

        /// <summary>
        /// Displays the print dialog for the given grid tree control.
        /// </summary>
        /// <param name="grid">The tree grid.</param>
        /// <param name="gridPrintAction">Creates a method that will open a GridPrintDialog.</param>
        /// <returns>A Nullable(T) value of type Boolean that signifies how a window was closed by the user.</returns>
        public static bool? ShowPrintDialog(this GridTreeControl grid, Action<GridPrintDialog> gridPrintAction)
        {
            var internalGrid = grid.InternalGrid;
            return internalGrid.ShowPrintDialog(gridPrintAction);
        }

        public static bool? Print(this GridControlBase grid)
        {
            return grid.Print(null);
        }
        public static bool? Print(this GridControlBase grid, Action<PrintDialog> gridPrintAction)
        {
            if (grid.PrintRange == null)
                grid.PrintRange = GridRangeInfo.Table();
            var printDialog = new PrintDialog();
            printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
            printDialog.PrintTicket.OutputColor = OutputColor.Grayscale;

            if (gridPrintAction != null)
            {
                gridPrintAction(printDialog);
                return true;
            }

            if (printDialog.ShowDialog() == true)
            {
                var PrintSize = new System.Windows.Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
                grid.SetPrintPageSize(PrintSize);
                var printPaginator = (IGridPrintPaginator)grid;
                var tablePaginator = new GridPrintTablePaginator(PrintSize, printPaginator, printDialog);
                printDialog.PrintDocument(tablePaginator, printPaginator.PrintDescription);
                return true;
            }
            else
                return false;
        }

        public static bool? Print(this GridDataControl grid, Action<PrintDialog> gridPrintAction)
        {
            var internalGrid = grid.InternalGrid;
            return internalGrid.Print(gridPrintAction);
        }

        public static bool? Print(this GridDataControl grid)
        {
            var internalGrid = grid.InternalGrid;
            return internalGrid.Print(null);
        }

        public static bool? Print(this GridTreeControl grid, Action<PrintDialog> gridPrintAction)
        {
            var internalGrid = grid.InternalGrid;
            return internalGrid.Print(gridPrintAction);
        }

        public static bool? Print(this GridTreeControl grid)
        {
            var internalGrid = grid.InternalGrid;
            return internalGrid.Print(null);
        }
    }

    public enum ScalingOptions
    {
        /// <summary>
        /// Print at their actual size
        /// </summary>
        NoScaling,
        /// <summary>
        /// Shrink the printout so that it is one page wide
        /// </summary>
        FitAllColumnsonOnePage,
        /// <summary>
        /// Shrink the printout so that it is one page high 
        /// </summary>
        FitAllRowsonOnePage,
        /// <summary>
        /// Shrink the printout so that it fits on one page 
        /// </summary>
        FitGridonOnePage
    }
}
