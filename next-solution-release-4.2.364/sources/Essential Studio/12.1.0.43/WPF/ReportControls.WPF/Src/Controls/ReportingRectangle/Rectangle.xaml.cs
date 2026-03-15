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
    internal partial class ReportingRectangle
        : UserControl
    {
        bool isPrintMode = false;
        int currentPage = 0;

#if SILVERLIGHT
        private Border border = new Border();
#else
        private DashStyleBorder border = new DashStyleBorder();
#endif
        private RectangleModel RectangleModel
        {
            get;
            set;
        }

        private RectangleItemExpVal RectanglePro
        {
            get;
            set;
        }

        private double incrHeight = 0.0;

        internal bool IsPrintMode
        {
            get
            {
                return isPrintMode;
            }
            set
            {
                isPrintMode = value;

                if (!this.RectangleModel.IsTablixChild)
                {
                    if (isPrintMode)
                    {
                        this.Margin = new Thickness(RectangleModel.PrintPageInfo.ActualLeft, RectangleModel.PrintPageInfo.ActualTop, 0, 0);
                    }
                    else
                    {
                        this.Margin = new Thickness(RectangleModel.PageInfo.ActualLeft, RectangleModel.PageInfo.ActualTop, 0, 0);
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

                if (!this.RectangleModel.IsTablixChild)
                {
                    if (this.IsPrintMode)
                    {
                        this.Margin = new Thickness(this.currentPage % this.RectangleModel.PrintPageColumnCount == 0 ?
                                            this.RectangleModel.PrintPageInfo.ActualLeft : 0, this.currentPage < this.RectangleModel.PrintPageColumnCount ?
                                            this.RectangleModel.PrintPageInfo.ActualTop : 0, 0, 0);
                    }
                    else
                    {
                        this.Margin = new Thickness(this.RectangleModel.PageInfo.ActualLeft, this.currentPage == 0 ?
                                                    this.RectangleModel.PageInfo.ActualTop : 0, 0, 0);
                    }

                    this.Height = this.PageSizes[this.CurrentPage].Height + incrHeight;
                    this.Width = this.PageSizes[this.CurrentPage].Width;
                }
                else
                {
                    if (!this.IsPrintMode)
                    {
                        this.Height = this.RectangleModel.Height + incrHeight;
                        this.Width = this.RectangleModel.Width;
                    }
                    else
                    {
                        this.Height = this.PageSizes[this.CurrentPage].Height + incrHeight;
                        this.Width = this.PageSizes[this.CurrentPage].Width;
                    }
                }
            }
        }

        internal Dictionary<int, PageInfo> PageSizes
        {
            get
            {
                if (this.IsPrintMode)
                    return this.RectangleModel.PrintPageSizes;
                return this.RectangleModel.PageSizes;
            }
        }

        internal int GetPage(int page)
        {
            if (this.IsPrintMode)
            {
                return this.RectangleModel.PrintPageInfo.BelongsTo[page];
            }

            return this.RectangleModel.PageInfo.BelongsTo[page];
        }

        internal bool isRepeat = false;

        public ReportingRectangle(IReportItemModeler pageContent)
            : this(pageContent, false, 0)
        {

        }

        public ReportingRectangle(IReportItemModeler pageContent, bool isPrintMode, int currentPage)
        {
            this.InitializeComponent();
            this.RectangleModel = pageContent as RectangleModel;
            this.IsPrintMode = isPrintMode;
            this.currentPage = currentPage;
            this.RectanglePro = RectangleModel.RectItemExpPro;
            string controlName = pageContent.Name;
            this.Background = new ReportingBrushConverter().ConvertFromInvariantString(this.RectangleModel.RectItemExpPro.BackgroundColor);
            this.Visibility = this.RectangleModel.Hidden ? Visibility.Collapsed : Visibility.Visible;

            if (this.RectangleModel.IsTablixInnerChild)
            {
                this.Margin = new Thickness(this.RectangleModel.Left, this.RectangleModel.Top, 0, 0);
            }
            else if (!this.RectangleModel.IsTablixChild)
            {
                this.Margin = new Thickness(pageContent.PageInfo.ActualLeft, pageContent.PageInfo.ActualTop, 0, 0);
            }
        }

        internal void InnerReportItems()
        {
            if (this.PageSizes != null && this.PageSizes.ContainsKey(this.currentPage))
            {
                this.Height = this.PageSizes[this.currentPage].Height + incrHeight;
                this.Width = this.PageSizes[this.currentPage].Width;
            }
            else
            {
                this.Height = this.RectangleModel.Height + incrHeight;
                this.Width = this.RectangleModel.Width;
            }
            this.Initialize(this.Width, this.Height);
            if (this.RectangleModel.IsTablixChild)
            {
                foreach (var reportItem in this.RectangleModel.ReportItemModelers)
                {
                    this.ProcessReportItems(reportItem);
                }
            }
        }

        void ProcessReportItems(IReportItemModeler model)
        {
            switch (model.ModelType)
            {
                case ModelType.TextBoxModel:
                    ReportingTextBox richTextBox = new ReportingTextBox(model);
                    this.RectangleBodyContent.Children.Add(richTextBox);
                    break;
                case ModelType.GaugeModel:
                    ReportingGauge dataGauge = new ReportingGauge(model);
                    this.RectangleBodyContent.Children.Add(dataGauge);
                    break;
                case ModelType.ImageModel:
                    ReportingImage imageControl = new ReportingImage(model);
                    this.RectangleBodyContent.Children.Add(imageControl);
                    break;
                case ModelType.ChartModel:
                    ReportingChartControl dataChart = new ReportingChartControl(model);
                    this.RectangleBodyContent.Children.Add(dataChart);
                    break;
                case ModelType.LineModel:
                    ReportingLine pageLine = new ReportingLine(model);
                    this.RectangleBodyContent.Children.Add(pageLine);
                    break;
                case ModelType.RectangleModel:
                    ReportingRectangle dataRect = new ReportingRectangle(model, this.IsPrintMode, this.CurrentPage);
                    dataRect.IsPrintMode = this.IsPrintMode;
                    if (!this.RectangleModel.Model.EnableVirtualEvaluation)
                    {
                        dataRect.CurrentPage = dataRect.GetPage(this.CurrentPage);
                    }
                    this.RectangleBodyContent.Children.Add(dataRect);
                    dataRect.InnerReportItems();
                    break;
                case ModelType.SubReportModel:
                    ReportingSubReport dataSubReport = new ReportingSubReport(model);
                    this.RectangleBodyContent.Children.Add(dataSubReport);
                    break;
                case ModelType.TablixModel:
                    ReportingTablixControl dataGrid = new ReportingTablixControl(model);
                    dataGrid.IsPrintMode = this.IsPrintMode;
                    if (this.RectangleModel.Model.EnableVirtualEvaluation)
                    {

                        dataGrid.CurrentPage = 0;
                    }
                    else
                    {
                        dataGrid.CurrentPage = dataGrid.GetPage(this.CurrentPage);
                    }
                    dataGrid.UpdatePage();
                    this.RectangleBodyContent.Children.Add(dataGrid);
                    break;

#if !SyncfusionFramework3_5 && !SILVERLIGHT && !WINRT
                case ModelType.MapModel:
                    ReportingMap mapModel = new ReportingMap(model);
                    mapModel.IsPrintMode = this.IsPrintMode;
                    this.RectangleBodyContent.Children.Add(mapModel);
                    break;
#endif
            }
        }

        internal void UpdateRectangleHeight(int pageNo, bool viewMode)
        {
            try
            {
                if (this.RectangleModel.ReportItemModelers != null && RectangleModel.ReportItemModelers.Count > 0)
                {
                    var rPageNo = viewMode ? this.RectangleModel.PrintPageInfo.BelongsTo[pageNo] : this.RectangleModel.PageInfo.BelongsTo[pageNo];
                    var rPageSize = viewMode ? this.RectangleModel.PrintPageSizes : this.RectangleModel.PageSizes;

                    var tablix = this.RectangleModel.ReportItemModelers.Where(t => t.ModelType == ModelType.TablixModel);
                    if (tablix.Count() > 0)
                    {
                        foreach (TablixModel rModel in tablix)
                        {
                            var tPageNo = viewMode ? rModel.PrintPageInfo.BelongsTo[pageNo] : rModel.PageInfo.BelongsTo[pageNo];
                            var tPageSize = viewMode ? rModel.PrintPageSizes : rModel.PageSizes;

                            if (tPageSize[tPageNo] == tPageSize[0])
                            {
                                if (rPageSize[rPageNo].Height <= tPageSize[tPageNo].Height + rModel.Top)
                                {
                                    incrHeight = ((tPageSize[tPageNo].Height + rModel.Top) - rPageSize[rPageNo].Height) + 3;
                                }
                            }
                            else
                            {
                                if (rPageSize[rPageNo].Height - 1 <= tPageSize[tPageNo].Height)
                                {
                                    incrHeight = (tPageSize[tPageNo].Height - rPageSize[rPageNo].Height) + 2;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                incrHeight = 0.0;
            }
        }

        void Initialize(double width, double height)
        {
            ReportingBrushConverter converter = new ReportingBrushConverter();

            this.BorderBrush = new SolidColorBrush(Colors.Transparent);

            //// Setting default value for the rectangle content page.
            this.RectangleBodyContent.Width = 0.0;
            this.RectangleBodyContent.Height = 0.0;

            //// Setting width of the rectangle.
            this.Width = width;
            //// Setting the height of the rectangle.
            this.Height = height;

            this.border.Width = this.Width;
            this.border.Height = this.Height;

            if (this.RectangleModel.IsTablixChild)
            {
                //// Setting default value for the rectangle content page.
                this.RectangleBodyContent.Width = width;
                this.RectangleBodyContent.Height = height;
            }

            this.BorderThickness = new Thickness(0);
            this.border.BorderThickness = new Thickness(1);

            //// Setting default border thickness for the content part.
            if (this.RectanglePro.Border != null && this.RectanglePro.Border.Default != null
                && this.RectanglePro.Border.Default.BorderStyle != DOM.BorderStyles.None)
            {
                this.border.BorderThickness = new Thickness(this.RectanglePro.Border.Default.Thickness);

                //// Setting default border color for the rectangle.
                if (this.RectanglePro.Border.Default.BorderBrush != null)
                {
                    this.border.BorderBrush = (Brush)converter.ConvertFromString(this.RectanglePro.Border.Default.BorderBrush);
                }

                //// Setting default background color for the rectangle.
                if (this.RectanglePro.BackgroundColor != null)
                {
                    this.border.Background = (Brush)converter.ConvertFromString(this.RectanglePro.BackgroundColor);
                }
            }
#if !SILVERLIGHT
            IntializeBorderStyle();
#endif

            //// Adding the rectangle control to the parent window.
            this.RectangleBodyContent.Children.Add(this.border);

            this.InvalidateRectangle();
        }

#if !SILVERLIGHT
        void IntializeBorderStyle()
        {
            if (this.RectangleModel.RectItemExpPro.Border != null && this.RectangleModel.RectItemExpPro.Border.Default != null)
            {
                if (this.RectangleModel.RectItemExpPro.Border.Default.BorderStyle == DOM.BorderStyles.Dashed)
                    this.border.DashStyle = DashStyles.Dash;
                else if (this.RectangleModel.RectItemExpPro.Border.Default.BorderStyle == DOM.BorderStyles.DashDot)
                    this.border.DashStyle = DashStyles.DashDot;
                else if (this.RectangleModel.RectItemExpPro.Border.Default.BorderStyle == DOM.BorderStyles.DashDotDot)
                    this.border.DashStyle = DashStyles.DashDotDot;
                else if (this.RectangleModel.RectItemExpPro.Border.Default.BorderStyle == DOM.BorderStyles.Dotted)
                    this.border.DashStyle = DashStyles.Dot;
                else if (this.RectangleModel.RectItemExpPro.Border.Default.BorderStyle == DOM.BorderStyles.None)
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
            var height = this.Height + incrHeight;
            var width = this.Width;
            if (this.PageSizes != null && this.IsPrintMode)
            {
                if (this.PageSizes.ContainsKey(this.CurrentPage))
                {
                    this.Height = this.PageSizes[this.CurrentPage].Height + incrHeight;
                    this.Width = this.PageSizes[this.CurrentPage].Width;
                }
                else
                {
                    this.Height = this.RectangleModel.Height + incrHeight;
                    this.Width = this.RectangleModel.Width;
                }
            }

            //// Setting width of the rectangle.
            this.RectangleBodyContent.Height = height;
            //// Setting the height of the rectangle.
            this.RectangleBodyContent.Width = width;
            //// Setting width of the rectangle.
            this.border.Height = height;
            //// Setting the height of the rectangle.
            this.border.Width = width;
        }


        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="Syncfusion.RDL.Controls.ReportingRectangle.BorderThickness"/> dependency property. 
        /// </summary>
        static readonly new DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(ReportingRectangle), new PropertyMetadata(new Thickness(1), OnBorderThicknessPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.RDL.Controls.ReportingRectangle.BorderBrush"/> dependency property. 
        /// </summary>
        static readonly new DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(ReportingRectangle), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnBorderBrushPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.RDL.Controls.ReportingRectangle.Background"/> dependency property. 
        /// </summary>
        static readonly new DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(ReportingRectangle), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnBackgroundPropertyChanged));

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
            ReportingRectangle rectangle = dependencyObject as ReportingRectangle;

            if (rectangle != null)
            {
                if ((double)e.NewValue != (double)e.OldValue)
                {
                    rectangle.border.Width = (double)e.NewValue;
                    rectangle.RectangleBodyContent.Width = rectangle.border.Width;
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
            ReportingRectangle rectangle = dependencyObject as ReportingRectangle;

            if (rectangle != null)
            {
                if ((double)e.NewValue != (double)e.OldValue)
                {
                    rectangle.border.Height = (double)e.NewValue;
                    rectangle.RectangleBodyContent.Height = rectangle.border.Height;
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
            ReportingRectangle rectangle = dependencyObject as ReportingRectangle;

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
            ReportingRectangle rectangle = dependencyObject as ReportingRectangle;

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
            ReportingRectangle rectangle = dependencyObject as ReportingRectangle;

            if (rectangle != null)
            {
                if ((Brush)e.NewValue != null)
                {
                    rectangle.RectangleBodyContent.Background = (Brush)e.NewValue;
                }
            }
        }

        #endregion
    }

#if !SILVERLIGHT
    internal class DashStyleBorder : Border
    {
        public static readonly DependencyProperty DashStyleProperty = DependencyProperty.Register("DashStyle", typeof(DashStyle), typeof(DashStyleBorder), new PropertyMetadata(DashStyles.Solid, DashStylePropertyChanged));

        static void DashStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as Border).InvalidateVisual();
        }

        public DashStyle DashStyle
        {
            get
            {
                return (DashStyle)this.GetValue(DashStyleProperty);
            }
            set
            {
                if (this.DashStyle != value)
                {
                    this.SetValue(DashStyleProperty, value);
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            Thickness borderThickness = this.BorderThickness;
            Pen pen = new Pen(BorderBrush, borderThickness.Top);
            pen.DashStyle = DashStyle;
            double x = pen.Thickness * 0.5;
            dc.DrawRectangle(null, pen, new Rect(new Point(x, x), new Point(RenderSize.Width - x, RenderSize.Height - x)));
            Brush background = this.Background;

            if (background != null)
            {
                dc.DrawRectangle(background, null, new Rect(new Point(borderThickness.Left, borderThickness.Top), new Point(RenderSize.Width - borderThickness.Right, RenderSize.Height - borderThickness.Bottom)));
            }
        }
    }
#endif
}