//-------------------------------------------------------------------------------------------------
// <copyright file="Rectangle.xaml.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.ItemModel;
using Syncfusion.RDL.Internal;

namespace Syncfusion.RDL.Controls
{
    internal partial class ReportingSubReport
        : UserControl
    {
        bool isPrintMode = false;
        int currentPage = -1;

#if SILVERLIGHT
        private Border border = new Border();
#else
        private DashStyleBorder border = new DashStyleBorder();
#endif
        private SubReportModel SubReportModel
        {
            get;
            set;
        }

        private SubReportItemExpVal SubReportPro
        {
            get;
            set;
        }

        internal bool IsPrintMode
        {
            get
            {
                return isPrintMode;
            }
            set
            {                
                isPrintMode = value;

                if (!this.SubReportModel.IsTablixChild)
                {
                    if (isPrintMode)
                    {
                        this.Margin = new Thickness(SubReportModel.PrintPageInfo.ActualLeft, SubReportModel.PrintPageInfo.ActualTop, 0, 0);
                    }
                    else
                    {
                        this.Margin = new Thickness(SubReportModel.PageInfo.ActualLeft, SubReportModel.PageInfo.ActualTop, 0, 0);
                    }
                }
            }
        }

        public int CurrentPage
        {
            get
            {
                return this.currentPage;
            }
            set
            {
                this.currentPage = value;

                if (!this.SubReportModel.IsTablixChild)
                {
                    if (this.IsPrintMode)
                    {
                        this.Margin = new Thickness(this.currentPage % this.SubReportModel.PrintPageColumnCount == 0 ?
                                            this.SubReportModel.PrintPageInfo.ActualLeft : 0, this.currentPage < this.SubReportModel.PrintPageColumnCount ?
                                            this.SubReportModel.PrintPageInfo.ActualTop : 0, 0, 0);
                    }
                    else
                    {
                        this.Margin = new Thickness(this.SubReportModel.PageInfo.ActualLeft, this.currentPage == 0 ?
                                                    this.SubReportModel.PageInfo.ActualTop : 0, 0, 0);
                    }

                    this.Height = this.PageSizes[this.CurrentPage].Height;
                    this.Width = this.PageSizes[this.CurrentPage].Width;
                }
                else
                {
                    this.Height = this.SubReportModel.Height;
                    this.Width = this.SubReportModel.Width;
                }                
            }
        }

        internal Dictionary<int, PageInfo> PageSizes
        {
            get
            {
                if (this.IsPrintMode)
                    return this.SubReportModel.PrintPageSizes;
                return this.SubReportModel.PageSizes;
            }
        }

        internal int GetPage(int page)
        {
            if (this.IsPrintMode)
            {
                return this.SubReportModel.PrintPageInfo.BelongsTo[page];
            }

            return this.SubReportModel.PageInfo.BelongsTo[page];
        }

        public ReportingSubReport(IReportItemModeler pageContent)
        {
            this.InitializeComponent();
            this.SubReportModel = pageContent as SubReportModel;
            this.SubReportPro = SubReportModel.SubReportItemExpPro;
            string controlName = pageContent.Name;
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.Visibility = SubReportPro.Hidden ? Visibility.Collapsed : Visibility.Visible;

            if (this.SubReportModel.IsTablixInnerChild)
            {
                this.Margin = new Thickness(this.SubReportModel.Left, this.SubReportModel.Top, 0, 0);
            }
            else if (!this.SubReportModel.IsTablixChild)
            {
                this.Margin = new Thickness(pageContent.PageInfo.ActualLeft, pageContent.PageInfo.ActualTop, 0, 0);
            }
            else
            {
                this.Height = this.SubReportModel.Height;
                this.Width = this.SubReportModel.Width;
            }

            this.Initialize(pageContent.Width, pageContent.Height);

            if (this.SubReportModel.IsTablixChild)
            {
                foreach (var reportItem in pageContent.ReportItemModelers)
                {
                    this.ProcessReportItems(reportItem);
                }
            }
            this.Visibility = pageContent.Hidden ? Visibility.Collapsed : Visibility.Visible;
        }

        void ProcessReportItems(IReportItemModeler model)
        {
            switch (model.ModelType)
            {
                case ModelType.TextBoxModel:
                    ReportingTextBox richTextBox = new ReportingTextBox(model);
                    this.SubReportBodyContent.Children.Add(richTextBox);
                    break;
                case ModelType.GaugeModel:
                    ReportingGauge dataGauge = new ReportingGauge(model);
                    this.SubReportBodyContent.Children.Add(dataGauge);
                    break;
                case ModelType.ImageModel:
                    ReportingImage imageControl = new ReportingImage(model);
                    this.SubReportBodyContent.Children.Add(imageControl);
                    break;
                case ModelType.ChartModel:
                    ReportingChartControl dataChart = new ReportingChartControl(model);
                    this.SubReportBodyContent.Children.Add(dataChart);
                    break;
                case ModelType.LineModel:
                    ReportingLine pageLine = new ReportingLine(model);
                    this.SubReportBodyContent.Children.Add(pageLine);
                    break;
                case ModelType.SubReportModel:
                    ReportingSubReport dataRectangle = new ReportingSubReport(model);
                    dataRectangle.CurrentPage = 0;
                    this.SubReportBodyContent.Children.Add(dataRectangle);
                    break;
                case ModelType.RectangleModel:
                    ReportingRectangle dataSubReport = new ReportingRectangle(model);
                    this.SubReportBodyContent.Children.Add(dataSubReport);
                    break;
                case ModelType.TablixModel:
                    ReportingTablixControl dataGrid = new ReportingTablixControl(model);
                    this.SubReportBodyContent.Children.Add(dataGrid);
                    break;

#if !SyncfusionFramework3_5 && !SILVERLIGHT && !WINRT
                case ModelType.MapModel:
                    ReportingMap mapModel = new ReportingMap(model);
                    this.SubReportBodyContent.Children.Add(mapModel);
                    break;
#endif
            }
        }

        void Initialize(double width, double height)
        {
            ReportingBrushConverter converter = new ReportingBrushConverter();
            InitializeComponent();

            this.BorderBrush = new SolidColorBrush(Colors.Transparent);

            //// Setting default value for the rectangle content page.
            this.SubReportBodyContent.Width = 0.0;
            this.SubReportBodyContent.Height = 0.0;

            //// Setting width of the rectangle.
            this.Width = width;
            //// Setting the height of the rectangle.
            this.Height = height;

            this.border.Width = this.Width;
            this.border.Height = this.Height;

            if (this.SubReportModel.IsTablixChild)
            {
                //// Setting default value for the rectangle content page.
                this.SubReportBodyContent.Width = width;
                this.SubReportBodyContent.Height = height;
            }

            this.BorderThickness = new Thickness(0);
            this.border.BorderThickness = new Thickness(1);

            //// Setting default border thickness for the content part.
            if (this.SubReportPro.Border != null && this.SubReportPro.Border.Default != null
                && this.SubReportPro.Border.Default.BorderStyle != DOM.BorderStyles.None)
            {
                this.border.BorderThickness = new Thickness(this.SubReportPro.Border.Default.Thickness);

                //// Setting default border color for the rectangle.
                if (this.SubReportPro.Border.Default.BorderBrush != null)
                {
                    this.border.BorderBrush = (Brush)converter.ConvertFromString(this.SubReportPro.Border.Default.BorderBrush);
                }

                //// Setting default background color for the rectangle.
                if (this.SubReportPro.BackgroundColor != null)
                {
                    this.border.Background = (Brush)converter.ConvertFromString(this.SubReportPro.BackgroundColor);
                }
            }
#if !SILVERLIGHT
            IntializeBorderStyle();
#endif

            //// Adding the rectangle control to the parent window.
            this.SubReportBodyContent.Children.Add(this.border);

            this.Dispatcher.BeginInvoke(
#if !SILVERLIGHT
                DispatcherPriority.Background,
#endif
                (ThreadStart)(() =>
                {
                    this.InvalidateRectangle();
                }));
        }

#if !SILVERLIGHT
        void IntializeBorderStyle()
        {
            if (this.SubReportModel.SubReportItemExpPro.Border != null && this.SubReportModel.SubReportItemExpPro.Border.Default != null)
            {
                if (this.SubReportModel.SubReportItemExpPro.Border.Default.BorderStyle == DOM.BorderStyles.Dashed)
                    this.border.DashStyle = DashStyles.Dash;
                else if (this.SubReportModel.SubReportItemExpPro.Border.Default.BorderStyle == DOM.BorderStyles.DashDot)
                    this.border.DashStyle = DashStyles.DashDot;
                else if (this.SubReportModel.SubReportItemExpPro.Border.Default.BorderStyle == DOM.BorderStyles.DashDotDot)
                    this.border.DashStyle = DashStyles.DashDotDot;
                else if (this.SubReportModel.SubReportItemExpPro.Border.Default.BorderStyle == DOM.BorderStyles.Dotted)
                    this.border.DashStyle = DashStyles.Dot;
                else if (this.SubReportModel.SubReportItemExpPro.Border.Default.BorderStyle == DOM.BorderStyles.None)
                    this.border.BorderThickness = new Thickness(0);
                else
                    this.border.DashStyle = DashStyles.Solid;
            }
        }
#endif

        /// <summary>
        /// Invalidates the rectangle and re-calculate the rectangle's internal width and height to make the rectangle content fit into the rectangle.
        /// </summary>
        /// <remarks>This method must be called for updating the styles which are applied after the rectagle rendered.</remarks>
        void InvalidateRectangle()
        {
            //// Setting width of the rectangle.
            this.SubReportBodyContent.Height = this.Height;
            //// Setting the height of the rectangle.
            this.SubReportBodyContent.Width = this.Width;
            //// Setting width of the rectangle.
            this.border.Height = this.Height;
            //// Setting the height of the rectangle.
            this.border.Width = this.Width;
        }


        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="Syncfusion.RDL.Controls.ReportingSubReport.BorderThickness"/> dependency property. 
        /// </summary>
        static readonly new DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(ReportingSubReport), new PropertyMetadata(new Thickness(1), OnBorderThicknessPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.RDL.Controls.ReportingSubReport.BorderBrush"/> dependency property. 
        /// </summary>
        static readonly new DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(ReportingSubReport), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnBorderBrushPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.RDL.Controls.ReportingSubReport.Background"/> dependency property. 
        /// </summary>
        static readonly new DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(ReportingSubReport), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnBackgroundPropertyChanged));

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the border thickness of the rectangle.
        /// </summary>
        public new Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the border color of the rectangle.
        /// </summary>
        public new Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the background of the rectangle.
        /// </summary>
        public new Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        #endregion

        #region Helper Events

        /// <summary>
        /// Called when [width property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnWidthPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportingSubReport rectangle = dependencyObject as ReportingSubReport;

            if (rectangle != null)
            {
                if ((double)e.NewValue != (double)e.OldValue)
                {
                    rectangle.border.Width = (double)e.NewValue;
                    rectangle.SubReportBodyContent.Width = rectangle.border.Width;
                }
            }
        }

        /// <summary>
        /// Called when [height property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnHeightPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportingSubReport rectangle = dependencyObject as ReportingSubReport;

            if (rectangle != null)
            {
                if ((double)e.NewValue != (double)e.OldValue)
                {
                    rectangle.border.Height = (double)e.NewValue;
                    rectangle.SubReportBodyContent.Height = rectangle.border.Height;
                }
            }
        }

        /// <summary>
        /// Called when [border thickness property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnBorderThicknessPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportingSubReport rectangle = dependencyObject as ReportingSubReport;

            if (rectangle != null)
            {
                if ((Thickness)e.NewValue != null)
                {
                    rectangle.border.BorderThickness = (Thickness)e.NewValue;
                }
            }
        }

        /// <summary>
        /// Called when [border brush property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnBorderBrushPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportingSubReport rectangle = dependencyObject as ReportingSubReport;

            if (rectangle != null)
            {
                if ((Brush)e.NewValue != null)
                {
                    rectangle.border.BorderBrush = (Brush)e.NewValue;
                }
            }
        }

        /// <summary>
        /// Called when [background property changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        static void OnBackgroundPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportingSubReport rectangle = dependencyObject as ReportingSubReport;

            if (rectangle != null)
            {
                if ((Brush)e.NewValue != null)
                {
                    rectangle.SubReportBodyContent.Background = (Brush)e.NewValue;
                }
            }
        }

        #endregion
    }
}