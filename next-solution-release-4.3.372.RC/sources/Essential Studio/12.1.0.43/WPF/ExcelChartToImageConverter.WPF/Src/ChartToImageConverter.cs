#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Syncfusion.UI.Xaml.Charts;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation.Shapes;
using System.Threading;
using System.Windows.Media;


namespace Syncfusion.ExcelChartToImageConverter
{
    /// <summary>
    /// Excel Chart To Image Converter.
    /// </summary>
    public class ChartToImageConverter : IChartToImageConverter
    {
        /// <summary>
        /// To assign the input chart
        /// </summary>
        IChart _chart;
        /// <summary>
        /// Represent the quality of Image
        /// </summary>
        ScalingMode m_scalingMode = ScalingMode.Normal;
        
        GetChartSeries converter = new GetChartSeries();

        #region Properties
        /// <summary>
        /// It's represent the chart image Scaling 
        /// </summary>
        public ScalingMode ScalingMode
        {
            get
            {
                return m_scalingMode;
            }
            set
            {
                m_scalingMode = value;
            }
        }
        #endregion
        /// <summary>
        /// Excel chart to sf Chart conversion.
        /// </summary>
        /// <param name="excelChart">Excel chart</param>
        /// <param name="imageAsStream">Output Stream</param>
        /// <returns>SfChart</returns>
        public void SaveAsImage(IChart excelChart, Stream imageAsStream)
        {
            _chart = excelChart;
            //To change the current thread state as STA for ASP.Net and Asp.Net MVC
            Thread currenThread = new Thread(GetChartStream);
            currenThread.SetApartmentState(ApartmentState.STA);
            currenThread.Start(imageAsStream);
            currenThread.Join();
        }
        /// <summary>
        /// Assign the Sfchart Stream to input stream
        /// </summary>
        private void GetChartStream(object stream)
        {
            int chartWidth;
            int chartHeight;
            bool isPie = false;

            SfChart sfChart = new SfChart {PrimaryAxis = new CategoryAxis()};

            if (_chart.PrimaryValueAxis.IsLogScale)
            {
                sfChart.SecondaryAxis = new LogarithmicAxis();
            }
            else
            {
                sfChart.SecondaryAxis = new NumericalAxis();
            }
            sfChart.SecondaryAxis.MaximumLabels = 5;
            sfChart.Name = _chart.ChartType.ToString();

            foreach (IChartSerie serie in _chart.Series)
            {
                if (!serie.IsFiltered)
                {
                    GetChartSerie(serie.SerieType, sfChart, serie, out isPie);
                    if (isPie)
                        break;
                }
            }

            //Chart Title
            if (!string.IsNullOrEmpty(_chart.ChartTitle))
            converter.SfChartTitle(sfChart, _chart);

            if (!isPie)
            {
                //secondary axis effects
                if (_chart.PrimaryValueAxis.IsLogScale)
                {
                    converter.SfLogerthmicAxis(sfChart, _chart);                    
                }
                else
                {
                    converter.SfNumericalAxis(sfChart.SecondaryAxis, _chart.PrimaryValueAxis,_chart.PrimaryCategoryAxis);
                }
                
                //primary axis effects
                converter.SfPrimaryAxis(sfChart, _chart);                

                //newChart.PrimaryAxis = new DateTimeAxis();
                // converter.SfPrimaryDateAxis(ref newChart, chart);
                //Axis Title                
                converter.SfAxisTitle(sfChart.SecondaryAxis,sfChart.PrimaryAxis, _chart.PrimaryValueAxis, _chart.PrimaryCategoryAxis);
            }

            //Chart Legend
            if (_chart.HasLegend)
                converter.SfLegend(sfChart, _chart);

            //Chart PloatArea Setting
            if (_chart.HasPlotArea)
                converter.SfPloatArea(sfChart, _chart.PlotArea);

            //Chart Area Settings            
            converter.SfChartArea(sfChart, _chart.ChartArea);

            if ((_chart as ChartShapeImpl) == null)
            {
                chartWidth = (int)_chart.Width;
                chartHeight = (int)_chart.Height;
            }
            else
            {
                chartWidth = (_chart as ChartShapeImpl).Width;
                chartHeight = (_chart as ChartShapeImpl).Height;
            }

            // Assign the chart Width and Height
            sfChart.Width = chartWidth;
            sfChart.Height = chartHeight;
            sfChart.Measure(new Size(chartWidth, chartHeight));
            sfChart.Arrange(new Rect(0, 0, sfChart.Width , sfChart.Height));
            
            if (sfChart.Series.Count > 0)
            {
                sfChart.Save(new MemoryStream(), new JpegBitmapEncoder());
                Save(sfChart, stream);
            }
        }        
        /// <summary>
        /// Sfchart element is save as to image stream.
        /// </summary>
        /// <param name="sfchart">Sfchart Element</param>
        /// <param name="stream">output Stream</param>
        private void Save(SfChart sfchart, object stream)
        {
            Size size = new Size((int)sfchart.ActualWidth, (int)sfchart.ActualHeight);
            sfchart.RenderTransform = new ScaleTransform(1, 1, 0.5, 0.5);
            RenderOptions.SetEdgeMode(sfchart, EdgeMode.Aliased);
            switch (ScalingMode)
            {
                case XlsIO.ScalingMode.Normal:
                    RenderOptions.SetBitmapScalingMode(sfchart, BitmapScalingMode.Linear);
                    break;
                case XlsIO.ScalingMode.Best:
                    RenderOptions.SetBitmapScalingMode(sfchart, BitmapScalingMode.HighQuality);
                    break;
            }            
            sfchart.Measure(size);
            var rect = new Rect(size);
            sfchart.Arrange(rect);    
           
            Rendered(sfchart, size, stream);
        }
        /// <summary>
        /// Sf chart is rendered and save as to stream
        /// </summary>
        /// <param name="visual">Sfchart element</param>
        /// <param name="size">sf chart size</param>
        /// <param name="stream">output Stream</param>
        private void Rendered(Visual visual, Size size, object stream)
        {
            double dpi = (int)ScalingMode;
            double scale = dpi / 96;

            RenderTargetBitmap renderBitmap =
                new RenderTargetBitmap(
                (int)(size.Width * scale),
                (int)(size.Height * scale),
                (int)ScalingMode,
                (int)ScalingMode,
                PixelFormats.Pbgra32);

            renderBitmap.Render(visual);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();         
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            encoder.Save((MemoryStream)stream);            
        }
        /// <summary>
        /// Get Sf chart serie for Excel Chart serie
        /// </summary>
        /// <param name="serieType">Represent the Series Type</param>
        /// <param name="sfChart">Sfchart</param>
        /// <param name="serie">Excel Serie</param>
        /// <param name="isPie">ispie boolean for piechart</param>
        private void GetChartSerie(ExcelChartType serieType, SfChart sfChart, IChartSerie serie,out bool isPie)
        {
            isPie = false;
            switch (serieType)
            {

                case ExcelChartType.Area:
                    AreaSeries areaSeries = converter.SfAreaSeries(serie, _chart);
                    sfChart.Series.Add(areaSeries);
                    break;
                case ExcelChartType.Bar_Clustered:
                    BarSeries barSeries = converter.SfBarseries(serie, _chart);
                    sfChart.Series.Add(barSeries);
                    break;
                case ExcelChartType.Column_Clustered:
                    ColumnSeries columnSerie = converter.SfColumnSeries(serie, _chart);
                    sfChart.Series.Add(columnSerie);
                    break;
                case ExcelChartType.Pie:
                    PieSeries pieSerie = converter.SfPieSeries(serie, _chart);
                    sfChart.Series.Add(pieSerie);
                    isPie = true;
                    break;
                case ExcelChartType.Line:
                case ExcelChartType.Line_Markers:
                case ExcelChartType.Line_Stacked:
                    LineSeries lineSerie = converter.SfLineSeries(serie, _chart);
                    sfChart.Series.Add(lineSerie);
                    break;
                case ExcelChartType.Area_Stacked:
                    StackingAreaSeries stackAreaSeries = converter.SfStackAreaSeries(serie, _chart);
                    sfChart.Series.Add(stackAreaSeries);
                    break;
                case ExcelChartType.Bar_Stacked:
                    StackingBarSeries stackBarSerie = converter.SfStackBarSeries(serie, _chart);
                    sfChart.Series.Add(stackBarSerie);
                    break;
                case ExcelChartType.Column_Stacked:
                    StackingColumnSeries stackColumnSeries = converter.SfStackedColumnSeries(serie, _chart);
                    sfChart.Series.Add(stackColumnSeries);
                    break;
                case ExcelChartType.Area_Stacked_100:
                    StackingArea100Series stackArea100 = converter.SfStackArea100Series(serie, _chart);
                    sfChart.Series.Add(stackArea100);
                    break;
                case ExcelChartType.Bar_Stacked_100:
                    StackingBar100Series stackBar100 = converter.SfStackBar100Series(serie, _chart);
                    sfChart.Series.Add(stackBar100);
                    break;
                case ExcelChartType.Column_Stacked_100:
                    StackingColumn100Series stackColumn100 = converter.SfStackColum100Series(serie, _chart);
                    sfChart.Series.Add(stackColumn100);
                    break;
                case ExcelChartType.Radar:
                case ExcelChartType.Radar_Filled:
                case ExcelChartType.Radar_Markers:
                    RadarSeries radar = converter.SfRadarSeries(serie, _chart);
                    sfChart.Series.Add(radar);
                    break;
                case ExcelChartType.Scatter_Line:
                case ExcelChartType.Scatter_Line_Markers:
                case ExcelChartType.Scatter_Markers:
                case ExcelChartType.Scatter_SmoothedLine:
                case ExcelChartType.Scatter_SmoothedLine_Markers:
                    ScatterSeries scatter = converter.SfScatterrSeries(serie, _chart);
                    sfChart.Series.Add(scatter);
                    break;
                case ExcelChartType.Stock_OpenHighLowClose:
                    HiLoOpenCloseSeries stock = converter.SfStockSeries(serie, _chart);
                    sfChart.Series.Add(stock);
                    break;
                case ExcelChartType.Doughnut:

                    DoughnutSeries doughnut = converter.SfDoughnutSeries(serie, _chart);
                    sfChart.Series.Add(doughnut);
                    break;
            }
        }
    }       

    
    /// <summary>
    /// It's an chart data source. Collection of datapoint's.
    /// </summary>
    internal class ViewModel
    {
        internal ViewModel(int count)
        {
            Products = new ObservableCollection<ChartPoint>();
            for (int i = 0; i < count; i++)
            {
                Products.Add(new ChartPoint());
            }
        }
        internal ViewModel()
        {
            Products = new ObservableCollection<ChartPoint>();
        }

        internal ObservableCollection<ChartPoint> Products { get; set; }
    }

    /// <summary>
    /// It's represent the chart datapoint.
    /// </summary>
    internal class ChartPoint
    {
        public string X { get; set; }
        public double Value { get; set; }
    }
}
