#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.ItemModel;
using Syncfusion.RDL.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.RDL.Controls
{
    internal sealed class ReportingRectangle : ContentControl
    {
        int currentPage = -1;

        Rectangle rectborder;

        ReportingBrushConverter brush;

        RectangleItemExpVal RectProperties
        {
            get;
            set;
        }

        RectangleModel Rectmodel
        {
            get;
            set;
        }

        Canvas RectBodyControl
        {
            get;
            set;
        }

        internal IReportItemModeler Model
        {
            get;
            set;
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

                Canvas.SetLeft(this, this.currentPage % this.Rectmodel.PrintPageColumnCount == 0 ?this.Rectmodel.PrintPageInfo.ActualLeft : 0);
                Canvas.SetTop(this,  this.currentPage < this.Rectmodel.PrintPageColumnCount ? this.Rectmodel.PrintPageInfo.ActualTop : 0);

                this.Height = this.PageSizes[this.CurrentPage].Height;
                this.Width = this.PageSizes[this.CurrentPage].Width;

                this.rectborder.Height = this.PageSizes[this.CurrentPage].Height;
                this.rectborder.Width = this.PageSizes[this.CurrentPage].Width;
            }
        }

        internal Dictionary<int, PageInfo> PageSizes
        {
            get
            {
               return this.Rectmodel.PrintPageSizes;
            }
        }

        internal int GetPage(int page)
        {
            return this.Rectmodel.PrintPageInfo.BelongsTo[page];
        }

        public ReportingRectangle(IReportItemModeler model)
        {
            this.Model = model;
            this.Rectmodel = this.Model as RectangleModel;
            this.RectProperties = this.Rectmodel.RectItemExpPro;
            this.rectborder = new Rectangle();
            this.brush = new ReportingBrushConverter();
            this.RectBodyControl = new Canvas();
            this.Initialize();
            this.Content = this.RectBodyControl;
        }

        void Initialize()
        {
            this.Height = this.Rectmodel.Height;
            this.Width = this.Rectmodel.Width;
            this.rectborder.Height = this.Height;
            this.rectborder.Width = this.Width;

            if (this.Rectmodel.PrintPageInfo != null)
            {
                Canvas.SetLeft(this, this.Rectmodel.PrintPageInfo.ActualLeft);
                Canvas.SetTop(this, this.Rectmodel.PrintPageInfo.ActualTop);
            }
            else if (this.Rectmodel.IsTablixChild && this.Rectmodel.IsTablixInnerChild)
            {
                Canvas.SetLeft(this, this.Rectmodel.Left);
                Canvas.SetTop(this, this.Rectmodel.Top);
            }

            this.rectborder.Height=this.Height;
            this.rectborder.Width=this.Width;
            this.BorderThickness = new Thickness(0);

            if (this.RectProperties.BackgroundColor != null)
            {
                this.rectborder.Fill = new ReportingBrushConverter().ConvertFromString(this.RectProperties.BackgroundColor);
            }

            this.BorderBrush = new SolidColorBrush(Colors.Transparent);

            if (this.RectProperties.Border != null && this.RectProperties.Border.Default != null)
            {
                this.rectborder.StrokeThickness = this.RectProperties.Border.Default.Thickness;

                if (this.RectProperties.Border.Default.BorderBrush != null)
                {
                    this.rectborder.Stroke = (Brush)brush.ConvertFromInvariantString(this.RectProperties.Border.Default.BorderBrush);
                }
                if (this.RectProperties.BackgroundColor != null)
                {
                    this.rectborder.Fill = (Brush)brush.ConvertFromInvariantString(this.RectProperties.BackgroundColor);
                }

                if (this.RectProperties.Border.Default.BorderStyle == DOM.BorderStyles.Dashed)
                {
                    this.rectborder.StrokeDashArray = new DoubleCollection() { 3, 1 };
                }
                else if (this.RectProperties.Border.Default.BorderStyle == DOM.BorderStyles.DashDot)
                {
                    this.rectborder.StrokeDashArray = new DoubleCollection() { 4, 2, 1, 2 };
                }
                else if (this.RectProperties.Border.Default.BorderStyle == DOM.BorderStyles.Dotted)
                {
                    this.rectborder.StrokeDashArray = new DoubleCollection() { 1, 1 };
                }
                else if (this.RectProperties.Border.Default.BorderStyle == DOM.BorderStyles.None)
                {
                    this.rectborder.Stroke = new SolidColorBrush(Colors.Transparent);
                }
            }
          
            this.RectBodyControl.Children.Add(this.rectborder);

            if (this.Rectmodel.IsTablixChild)
            {
                foreach (var reportItem in this.Rectmodel.ReportItemModelers)
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
                    ReportingTextbox richTextBox = new ReportingTextbox(model);
                    this.RectBodyControl.Children.Add(richTextBox);
                    break;
                case ModelType.GaugeModel:
                    ReportingGauge dataGauge = new ReportingGauge(model);
                    this.RectBodyControl.Children.Add(dataGauge);
                    break;
                case ModelType.ImageModel:
                    ReportingImage imageControl = new ReportingImage(model);
                    this.RectBodyControl.Children.Add(imageControl);
                    break;
                case ModelType.ChartModel:
                    ReportingChartControl dataChart = new ReportingChartControl(model);
                    this.RectBodyControl.Children.Add(dataChart);
                    break;
                case ModelType.LineModel:
                    ReportingLine pageLine = new ReportingLine(model);
                    this.RectBodyControl.Children.Add(pageLine);
                    break;
                case ModelType.RectangleModel:
                    ReportingRectangle dataRectangle = new ReportingRectangle(model);
                    this.RectBodyControl.Children.Add(dataRectangle);
                    break;
                case ModelType.SubReportModel:
                    ReportingSubReport dataSubReport = new ReportingSubReport(model);
                    this.RectBodyControl.Children.Add(dataSubReport);
                    break;
                case ModelType.TablixModel:
                    ReportingTablixControl dataGrid = new ReportingTablixControl(model);
                    dataGrid.CurrentPage =dataGrid.GetPage(this.CurrentPage);
                    dataGrid.UpdatePage();
                    this.RectBodyControl.Children.Add(dataGrid);
                    break;
            }
        }
    }
}
