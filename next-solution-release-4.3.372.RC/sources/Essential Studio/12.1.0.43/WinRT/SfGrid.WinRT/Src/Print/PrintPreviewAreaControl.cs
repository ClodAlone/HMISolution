#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using Syncfusion.UI.Xaml.Utility;
using System.Windows;
using System.Windows.Controls;


namespace Syncfusion.UI.Xaml.Grid
{
    public class PrintPreviewAreaControl : Control, INotifyPropertyChanged, IDisposable
    {

        #region Fields

        private PrintManagerBase printManager;
        private bool isPageIndexSetFromOverride;

#if SILVERLIGHT
        private bool isLoaded;
#endif

        #endregion

        #region Ctor

        public PrintPreviewAreaControl()
        {
            DefaultStyleKey = typeof(PrintPreviewAreaControl);
        }
       

        #endregion

        #region UIElements

        private PrintPreviewPanel PartPrintWindowPanel;

        #endregion

        #region Properties

        private int totalPages;
        /// <summary>
        /// Gets the Total pages to print
        /// </summary>
        public int TotalPages
        {
            get
            {
                return totalPages;
            }
            internal set
            {
                totalPages = value;
                OnPropertyChanged("TotalPages");
            }
        }

        /// <summary>
        /// Gets or Sets the PrintManageBase
        /// </summary>
        public PrintManagerBase PrintManagerBase
        {
            get
            {
                if (printManager != null && !printManager.isPagesInitialized)
                    printManager.InitializePrint(true);
                return printManager;
            }
            set
            {
                printManager = value;
                OnPropertyChanged("PrintManagerBase");
            }
        }

        #endregion

        #region Dependency Properties

        #region PrintPageMargins Property

        /// <summary>
        /// Gets or sets the Margin for printing pages.
        /// </summary>
        /// <value>Thichness</value>
        /// <remarks></remarks>
        public Thickness PrintPageMargin
        {
            get { return (Thickness)GetValue(PrintPageMarginProperty); }
            set { SetValue(PrintPageMarginProperty, value); }
        }

        public static readonly DependencyProperty PrintPageMarginProperty =
            DependencyProperty.Register("PrintPageMargin", typeof(Thickness), typeof(PrintPreviewAreaControl),
                new PropertyMetadata(new Thickness(96), OnPrintMarginChanged));

        private static void OnPrintMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printCtrl = d as PrintPreviewAreaControl;
            if (printCtrl.PrintManagerBase != null)
                printCtrl.PrintManagerBase.PrintPageMargin = (Thickness) e.NewValue;
        }

        #endregion

        #region PrintPageHeight Property


        /// <summary>
        /// Gets or sets the Height for print page.
        /// </summary>
        /// <value>double</value>
        /// <remarks></remarks>
        public double PrintPageHeight
        {
            get { return (double)GetValue(PrintPageHeightProperty); }
            set { SetValue(PrintPageHeightProperty, value); }
        }

        public static readonly DependencyProperty PrintPageHeightProperty =
            DependencyProperty.Register("PrintPageHeight", typeof(double), typeof(PrintPreviewAreaControl),
                new PropertyMetadata(1122.52, OnPrintPageHeightChanged));

        private static void OnPrintPageHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printCtrl = d as PrintPreviewAreaControl;
            if (printCtrl.PrintManagerBase != null)
                printCtrl.PrintManagerBase.PrintPageHeight = (double) e.NewValue;
        }

        #endregion

        #region PrintPageWidth Property

        /// <summary>
        /// Gets or Sets the Width of printable page.
        /// </summary>
        public double PrintPageWidth
        {
            get { return (double)GetValue(PrintPageWidthProperty); }
            set { SetValue(PrintPageWidthProperty, value); }
        }

        public static readonly DependencyProperty PrintPageWidthProperty =
            DependencyProperty.Register("PrintPageWidth", typeof(double), typeof(PrintPreviewAreaControl),
                new PropertyMetadata(793.70, OnPrintPageWidthChanged));

        private static void OnPrintPageWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printCtrl = d as PrintPreviewAreaControl;
            if (printCtrl.PrintManagerBase != null)
                printCtrl.PrintManagerBase.PrintPageWidth = (double) e.NewValue;
        }


        #endregion

        #region PrintPageHeaderHeight Property


        /// <summary>
        /// Gets or sets the height for the print page header.
        /// </summary>
        /// <value>double</value>
        /// <remarks></remarks>
        public double PrintPageHeaderHeight
        {
            get { return (double)GetValue(PrintPageHeaderHeightProperty); }
            set { SetValue(PrintPageHeaderHeightProperty, value); }
        }

        public static readonly DependencyProperty PrintPageHeaderHeightProperty =
            DependencyProperty.Register("PrintPageHeaderHeight", typeof(double), typeof(PrintPreviewAreaControl),
                new PropertyMetadata(0.0, OnPrintPageHeaderHeightChanged));

        private static void OnPrintPageHeaderHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printCtrl = d as PrintPreviewAreaControl;
            if (printCtrl.PrintManagerBase != null)
                printCtrl.PrintManagerBase.PrintPageHeaderHeight = (double) e.NewValue;
        }


        #endregion

        #region PrintPageFooterHeight Property


        /// <summary>
        /// Gets or sets the height of Print page footer.
        /// </summary>
        /// <value> double </value>
        /// <remarks></remarks>
        public double PrintPageFooterHeight
        {
            get { return (double)GetValue(PrintPageFooterHeightProperty); }
            set { SetValue(PrintPageFooterHeightProperty, value); }
        }

        public static readonly DependencyProperty PrintPageFooterHeightProperty =
            DependencyProperty.Register("PrintPageFooterHeight", typeof(double), typeof(PrintPreviewAreaControl),
                new PropertyMetadata(0.0, OnPrintPageFooterHeightChanged));

        private static void OnPrintPageFooterHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printCtrl = d as PrintPreviewAreaControl;
            if (printCtrl.PrintManagerBase != null)
                printCtrl.PrintManagerBase.PrintPageFooterHeight = (double) e.NewValue;
        }


        #endregion

        #region PrintHeaderTemplate Property


        /// <summary>
        /// Gets or sets the Template for the print page header.
        /// </summary>
        /// <value>DataTemplate</value>
        /// <remarks></remarks>
        public DataTemplate PrintPageHeaderTemplate
        {
            get { return (DataTemplate)GetValue(PrintPageHeaderTemplateProperty); }
            set { SetValue(PrintPageHeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty PrintPageHeaderTemplateProperty =
            DependencyProperty.Register("PrintPageHeaderTemplate", typeof(DataTemplate), typeof(PrintPreviewAreaControl),
                new PropertyMetadata(null, OnPrintPageHeaderTemplateChanged));

        private static void OnPrintPageHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printCtrl = d as PrintPreviewAreaControl;
            if (printCtrl.PrintManagerBase != null)
                printCtrl.PrintManagerBase.PrintPageHeaderTemplate = (DataTemplate) e.NewValue;
        }


        #endregion

        #region PrintPageFooterTemplate


        /// <summary>
        /// Gets or sets the Template for Print page footer.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public DataTemplate PrintPageFooterTemplate
        {
            get { return (DataTemplate)GetValue(PrintPageFooterTemplateProperty); }
            set { SetValue(PrintPageFooterTemplateProperty, value); }
        }

        public static readonly DependencyProperty PrintPageFooterTemplateProperty =
            DependencyProperty.Register("PrintPageFooterTemplate", typeof(DataTemplate), typeof(PrintPreviewAreaControl),
                new PropertyMetadata(null, OnPrintPageFooterTemplateChanged));

        private static void OnPrintPageFooterTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printCtrl = d as PrintPreviewAreaControl;
            if (printCtrl.PrintManagerBase != null)
                printCtrl.PrintManagerBase.PrintPageFooterTemplate = (DataTemplate) e.NewValue;
        }



        #endregion

        #region PrintHeaderRowHeight Property

        /// <summary>
        /// Gets or Sets the Print Header Row Height
        /// </summary>
        public double PrintHeaderRowHeight
        {
            get { return (double)GetValue(PrintHeaderRowHeightProperty); }
            set { SetValue(PrintHeaderRowHeightProperty, value); }
        }

        public static readonly DependencyProperty PrintHeaderRowHeightProperty =
            DependencyProperty.Register("PrintHeaderRowHeight", typeof(double), typeof(PrintPreviewAreaControl),
                new PropertyMetadata(28d, OnPrintHeaderRowHeightChanged));

        private static void OnPrintHeaderRowHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printCtrl = d as PrintPreviewAreaControl;
            if (printCtrl.PrintManagerBase != null)
                printCtrl.PrintManagerBase.PrintHeaderRowHeight = (double) e.NewValue;
        }


        #endregion

        #region PrintRowHeight Property

        /// <summary>
        /// Gets or Sets the Print row Height
        /// </summary>
        public double PrintRowHeight
        {
            get { return (double)GetValue(PrintRowHeightProperty); }
            set { SetValue(PrintRowHeightProperty, value); }
        }

        public static readonly DependencyProperty PrintRowHeightProperty =
            DependencyProperty.Register("PrintRowHeight", typeof(double), typeof(PrintPreviewAreaControl),
                new PropertyMetadata(24d, OnPrintPageRowHeight));

        private static void OnPrintPageRowHeight(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printCtrl = d as PrintPreviewAreaControl;
            if (printCtrl.PrintManagerBase != null)
                printCtrl.PrintManagerBase.PrintRowHeight = (double) e.NewValue;
        }

        #endregion

        #region PrintSummaryRowHeight Property

        /// <summary>
        /// Gets or Sets the Print Summary Row's Height
        /// </summary>
        internal double PrintSummaryRowHeight
        {
            get { return (double)GetValue(PrintSummaryRowHeightProperty); }
            set { SetValue(PrintSummaryRowHeightProperty, value); }
        }

        internal static readonly DependencyProperty PrintSummaryRowHeightProperty =
            DependencyProperty.Register("PrintSummaryRowHeight", typeof(double), typeof(PrintPreviewAreaControl),
                new PropertyMetadata(24d, OnPrintsummaryRowHeight));

        private static void OnPrintsummaryRowHeight(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printCtrl = d as PrintPreviewAreaControl;
            if (printCtrl.PrintManagerBase != null)
                printCtrl.PrintManagerBase.PrintSummaryRowHeight = (double) e.NewValue;
        }

        #endregion

        #region PrintGroupCaptionRowHeight Property

        /// <summary>
        /// Gets or Sets the Print Group Caption row's Height.
        /// </summary>
        internal double PrintGroupCaptionRowHeight
        {
            get { return (double)GetValue(PrintGroupCaptionRowHeightProperty); }
            set { SetValue(PrintGroupCaptionRowHeightProperty, value); }
        }
        
        internal static readonly DependencyProperty PrintGroupCaptionRowHeightProperty =
            DependencyProperty.Register("PrintGroupCaptionRowHeight", typeof(double), typeof(PrintPreviewAreaControl),
                new PropertyMetadata(24d, OnPrintGroupCaptionChanged));

        private static void OnPrintGroupCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printCtrl = d as PrintPreviewAreaControl;
            if (printCtrl.PrintManagerBase != null)
                printCtrl.PrintManagerBase.PrintGroupCaptionRowHeight = (double) e.NewValue;
        }

        #endregion
        
        #region PrintOrientation Property

        /// <summary>
        /// Gets or Sets the Page Orientation of the Print Page
        /// </summary>
        public PrintOrientation PrintOrientation
        {
            get { return (PrintOrientation)GetValue(PrintOrientationProperty); }
            set { SetValue(PrintOrientationProperty, value); }
        }
        
        public static readonly DependencyProperty PrintOrientationProperty =
            DependencyProperty.Register("PrintOrientation", typeof(PrintOrientation), typeof(PrintPreviewAreaControl), new PropertyMetadata(PrintOrientation.Portrait, OnPrintOrientationChanged));

        private static void OnPrintOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printAreaCtrl = d as PrintPreviewAreaControl;
            if (printAreaCtrl.PrintManagerBase != null)
            {
                
                if (printAreaCtrl.PrintManagerBase.isSuspended) return;
                printAreaCtrl.PrintManagerBase.isSuspended = true;
                if (((PrintOrientation)e.NewValue == PrintOrientation.Portrait && printAreaCtrl.PrintManagerBase.PrintPageHeight < printAreaCtrl.PrintManagerBase.PrintPageWidth) ||
                    ((PrintOrientation)e.NewValue == PrintOrientation.Landscape && printAreaCtrl.PrintManagerBase.PrintPageHeight > printAreaCtrl.PrintManagerBase.PrintPageWidth))
                {
                    var width = printAreaCtrl.PrintManagerBase.PrintPageWidth;
                    var height = printAreaCtrl.PrintManagerBase.PrintPageHeight;
                    printAreaCtrl.PrintManagerBase.PrintPageWidth = height;
                    printAreaCtrl.PrintManagerBase.PrintPageHeight = width;
                }
                printAreaCtrl.PrintManagerBase.PrintPageOrientation = (PrintOrientation)e.NewValue;
                printAreaCtrl.PrintManagerBase.isSuspended = false;
                if (printAreaCtrl.PrintManagerBase.InValidate != null)
                    printAreaCtrl.PrintManagerBase.InValidate(false);
                printAreaCtrl.OnZoomFactorChanged(printAreaCtrl.ZoomFactor);
            }
        }
        
        #endregion

        #region PrintScaleOption Property

        /// <summary>
        /// Gets or Sets the Print Scale option for the Print page
        /// </summary>
        public PrintScaleOptions PrintScaleOption
        {
            get { return (PrintScaleOptions)GetValue(PrintScaleOptionProperty); }
            set { SetValue(PrintScaleOptionProperty, value); }
        }

        public static readonly DependencyProperty PrintScaleOptionProperty =
            DependencyProperty.Register("PrintScaleOption", typeof (PrintScaleOptions), typeof (PrintPreviewAreaControl),
                new PropertyMetadata(PrintScaleOptions.NoScaling, OnPrintScaleOptionChanged));

        private static void OnPrintScaleOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printAreaCtrl = d as PrintPreviewAreaControl;
            if (printAreaCtrl.PrintManagerBase != null)
                printAreaCtrl.PrintManagerBase.PrintScaleOption = (PrintScaleOptions) e.NewValue;
        }

        #endregion

        #region ZoomFactor Property

        /// <summary>
        /// Gets or Sets the Zoom Factor for the Zoom
        /// </summary>
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }
        
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof (double), typeof (PrintPreviewAreaControl),
                new PropertyMetadata(100.0, OnZoomFactorDependencyPropertyChanged));

        private static void OnZoomFactorDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printAreaCtrl = d as PrintPreviewAreaControl;
            printAreaCtrl.OnZoomFactorChanged((double) e.NewValue);
        }

        
        #endregion

        #region PageIndex Property

        /// <summary>
        /// Gets or Sets the Page index for the Print Page.
        /// </summary>
        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register("PageIndex", typeof (int), typeof (PrintPreviewAreaControl),
                new PropertyMetadata(1, OnPageIndexChanged));

        private static void OnPageIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var printAreaCtrl = d as PrintPreviewAreaControl;
            if (!printAreaCtrl.isPageIndexSetFromOverride && printAreaCtrl.PartPrintWindowPanel != null && printAreaCtrl.PrintManagerBase != null)
             {
                 printAreaCtrl.PartPrintWindowPanel.SetVerticalOffset(((int)e.NewValue - 1) *
                                                        (printAreaCtrl.PartPrintWindowPanel.ExtentHeight / printAreaCtrl.PrintManagerBase.pageCount));
             }
        }
        
        #endregion

        #endregion

        #region Private Methods

        private void OnZoomFactorChanged(double value)
        {
            if (PartPrintWindowPanel != null)
            {
                PartPrintWindowPanel.Child.Zoom(value);
                PartPrintWindowPanel.SetVerticalOffset((PartPrintWindowPanel.Child.PageIndex - 1) * (PartPrintWindowPanel.ExtentHeight / PrintManagerBase.pageCount));
            }

        }

#if SILVERLIGHT
        private void RefreshCommands()
        {
            PrintCommand.RaiseCanExecuteChanged();
            FirstCommand.RaiseCanExecuteChanged();
            PreviousCommand.RaiseCanExecuteChanged();
            NextCommand.RaiseCanExecuteChanged();
            LastCommand.RaiseCanExecuteChanged();
        }
#endif

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PartPrintWindowPanel = GetTemplateChild("PART_PrintWindowPanel") as PrintPreviewPanel;
            if (PartPrintWindowPanel == null) return;
            PartPrintWindowPanel.SetPrintManagerBase(PrintManagerBase);
            PartPrintWindowPanel.SetPageIndex = index =>
            {
                isPageIndexSetFromOverride = true;
                PageIndex = index;
                isPageIndexSetFromOverride = false;
#if SILVERLIGHT
                RefreshCommands();
#endif
            };
            PartPrintWindowPanel.InValidateParent = () =>
            {
                TotalPages = PrintManagerBase.pageCount;
            };
            TotalPages = PrintManagerBase.pageCount;
            PartPrintWindowPanel.Child.Loaded += OnPrintPreviewControlLoaded;
#if SILVERLIGHT
            RefreshCommands();
#endif
            
        }

        #endregion

        #region Events

        void OnPrintPreviewControlLoaded(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            if (isLoaded) return;
            isLoaded = true;
#endif
            var heightdelta = PrintPageHeight / 100;
            ZoomFactor = (int)(PartPrintWindowPanel.ViewportHeight / heightdelta);
        }

        #endregion

        #region Commands

        #region PrintCommand

        BaseCommand printCommand;
        public BaseCommand PrintCommand
        {
            get { return printCommand ?? (printCommand = new BaseCommand(OnprintCommandClicked, o => PrintManagerBase.pageCount > 0)); }
        }

        private void OnprintCommandClicked(object obj)
        {
#if WPF
            PrintManagerBase.PrintWithDialog();
#else
            PrintManagerBase.Print();
#endif

        }

        #endregion

#if WPF

        #region QuickPrintCommand

        BaseCommand quickPrintCommand;
        public BaseCommand QuickPrintCommand
        {
            get { return quickPrintCommand ?? (quickPrintCommand = new BaseCommand(OnQuickprintCommandClicked, o => PrintManagerBase.pageCount > 0)); }
        }

        private void OnQuickprintCommandClicked(object obj)
        {
            PrintManagerBase.Print();
        }

        #endregion

#endif

        #region FirstCommand

        BaseCommand firstCommand;
        public BaseCommand FirstCommand
        {
            get
            {
                return firstCommand ??
                       (firstCommand =
                           new BaseCommand(OnFirstCommandClicked,
                               o =>
                                   PrintManagerBase != null && PrintManagerBase.pageCount > 0 && PartPrintWindowPanel != null &&
                                   PartPrintWindowPanel.Child.PageIndex != 1));
            }
        }

        private void OnFirstCommandClicked(object obj)
        {
            if (PartPrintWindowPanel != null) PartPrintWindowPanel.SetVerticalOffset(0);
        }

        #endregion

        #region PreviousCommand

        BaseCommand previousCommand;
        public BaseCommand PreviousCommand
        {
            get
            {
                return previousCommand ?? (previousCommand = new BaseCommand(OnPreviousCommandClicked,
                    o => PrintManagerBase != null && PrintManagerBase.pageCount > 0 && PartPrintWindowPanel != null &&
                                   PartPrintWindowPanel.Child.PageIndex != 1));
            }
        }

        private void OnPreviousCommandClicked(object obj)
        {
            if (PartPrintWindowPanel == null) return;
            var pageIndex = PartPrintWindowPanel.Child.PageIndex;
            if (pageIndex > 1 && pageIndex <= PrintManagerBase.pageCount)
                PartPrintWindowPanel.SetVerticalOffset((pageIndex - 2) * (PartPrintWindowPanel.ExtentHeight / PrintManagerBase.pageCount));
        }

        #endregion

        #region NextCommand

        BaseCommand nextCommand;

        public BaseCommand NextCommand
        {
            get
            {
                return nextCommand ??
                       (nextCommand =
                           new BaseCommand(OnNextCommandClicked,
                               o => PrintManagerBase != null && PrintManagerBase.pageCount > 0 && PartPrintWindowPanel != null &&
                                    PartPrintWindowPanel.Child.PageIndex < PrintManagerBase.pageCount));
            }
        }

        private void OnNextCommandClicked(object obj)
        {
            if (PartPrintWindowPanel == null) return;
            var pageIndex =PartPrintWindowPanel.Child.PageIndex;
            if (pageIndex > 0 && pageIndex < PrintManagerBase.pageCount)
                PartPrintWindowPanel.SetVerticalOffset((pageIndex) * (PartPrintWindowPanel.ExtentHeight / PrintManagerBase.pageCount));
        }

        #endregion

        #region LastCommand

        BaseCommand lastCommand;

        public BaseCommand LastCommand
        {
            get { return lastCommand ?? (lastCommand = new BaseCommand(OnLastCommandClicked, o => PrintManagerBase != null && PrintManagerBase.pageCount > 0 && PartPrintWindowPanel != null && PartPrintWindowPanel.Child.PageIndex < PrintManagerBase.pageCount)); }
        }

        private void OnLastCommandClicked(object obj)
        {
            if (PartPrintWindowPanel != null)
                PartPrintWindowPanel.SetVerticalOffset((PrintManagerBase.pageCount - 1)*
                                                       (PartPrintWindowPanel.ExtentHeight / PrintManagerBase.pageCount));
        }

        #endregion

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Dispose Member

        public void Dispose()
        {
            printManager = null;
        }

        #endregion

    }
}
