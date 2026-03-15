#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Syncfusion.UI.Xaml.Charts;
using Syncfusion.XlsIO;

namespace Syncfusion.ExcelChartToImageConverter
{
    internal class GetChartSeries : ChartCommon
    {
/*
        private string _chartTitle = "Chart Title";
*/

        internal int PieAngle = 270;
        bool _secondayAxisAchived;
        /// <summary>
        /// It returns the SfBar Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">xlsio chart</param>
        /// <returns>BarSeries</returns>
        internal BarSeries SfBarseries(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            BarSeries barSeries = new BarSeries {XBindingPath = "X",YBindingPath = "Value"};
            int categoryCount = 1;
            if (!serie.UsePrimaryAxis)
            {
                barSeries.YAxis = new NumericalAxis { ShowGridLines = false, RangePadding = NumericalPadding.Normal, OpposedPosition = true };
                SfNumericalAxis(barSeries.YAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                if (_secondayAxisAchived)
                {
                    barSeries.YAxis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //set the Secondary axis title
                    SfAxisTitle(barSeries.YAxis, barSeries.XAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                }
                _secondayAxisAchived = true;
            }
            
            IRange[] ran = serie.Values.Cells;

            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }

            }
            barSeries.ItemsSource = values;

            //if has datalabel
            if (serie.DataPoints.DefaultDataPoint != null)
            {
                IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
                if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                    barSeries.AdornmentsInfo = SfChartDataLabel(serie);
            }

            barSeries.Label = serie.Name;

            return barSeries;
        }

        /// <summary>
        /// It returns the SfArea Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>AreaSeries</returns>
        internal AreaSeries SfAreaSeries(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            AreaSeries area = new AreaSeries {XBindingPath = "X",YBindingPath = "Value"};
            int categoryCount = 1;
            if (!serie.UsePrimaryAxis)
            {
                area.YAxis = new NumericalAxis { ShowGridLines = false, RangePadding = NumericalPadding.Normal, OpposedPosition = true };
                SfNumericalAxis(area.YAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                if (_secondayAxisAchived)
                {
                    area.YAxis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //set the Secondary axis title
                    SfAxisTitle(area.YAxis, area.XAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                }
                _secondayAxisAchived = true;
            }
            
            IRange[] ran = serie.Values.Cells;

            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }

            }
            area.ItemsSource = values;

            if (serie.SerieFormat.Fill.FillType == ExcelFillType.SolidColor && serie.SerieFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                area.Palette = ChartColorPalette.Custom;
                ChartColorModel colormodel = new ChartColorModel(ChartColorPalette.Custom);
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.Fill.ForeColor));

                colormodel.CustomBrushes = new List<Brush> {brush};
                area.ColorModel = colormodel;
            }

            //if has datalabel
            //Initializing adornments for the data points information in series

            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                area.AdornmentsInfo = SfChartDataLabel(serie);

            area.Label = serie.Name;

            return area;
        }

        /// <summary>
        /// It returns the SfColumn Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>ColumnSeries</returns>
        internal ColumnSeries SfColumnSeries(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            ColumnSeries column = new ColumnSeries {XBindingPath = "X",YBindingPath = "Value"};
            int categoryCount = 1;
            if (!serie.UsePrimaryAxis)
            {
                column.YAxis = new NumericalAxis { ShowGridLines = false, RangePadding = NumericalPadding.Normal, OpposedPosition = true };
                SfNumericalAxis(column.YAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                if (_secondayAxisAchived)
                {
                    column.YAxis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //set the Secondary axis title
                    SfAxisTitle(column.YAxis, column.XAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                }
                _secondayAxisAchived = true;
            }
            IRange[] ran = serie.Values.Cells;

            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }

            }
            column.ItemsSource = values;

            //Series Fill Settings
            if (serie.SerieFormat.Fill.FillType == ExcelFillType.SolidColor && serie.SerieFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                column.Palette = ChartColorPalette.Custom;
                ChartColorModel colormodel = new ChartColorModel(ChartColorPalette.Custom);
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.Fill.ForeColor));

                colormodel.CustomBrushes = new List<Brush> {brush};
                column.ColorModel = colormodel;
            }

            //Series Border Settings
            if (serie.SerieFormat.LineProperties.LineColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.LineProperties.LineColor));
                column.BorderBrush = brush;
            }
            if (serie.SerieFormat.LineProperties.LineWeight != ExcelChartLineWeight.Hairline)
            {
                int borderWidth = (int)serie.SerieFormat.LineProperties.LineWeight;
                column.BorderThickness = new Thickness(borderWidth);
            }

            //Initializing adornments for the data points information in series

            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                column.AdornmentsInfo = SfChartDataLabel(serie);
            column.Label = serie.Name;

            return column;
        }

        /// <summary>
        /// It returns the SfLine Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>LineSeries</returns>
        internal LineSeries SfLineSeries(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            LineSeries line = new LineSeries {XBindingPath = "X",YBindingPath = "Value"};
            int categoryCount = 1;
            if (!serie.UsePrimaryAxis)
            {
                line.YAxis = new NumericalAxis { ShowGridLines = false, RangePadding = NumericalPadding.Normal, OpposedPosition = true };
                SfNumericalAxis(line.YAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                if (_secondayAxisAchived)
                {
                    line.YAxis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //set the Secondary axis title
                    SfAxisTitle(line.YAxis, line.XAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                }
                _secondayAxisAchived = true;
            }
            
            IRange[] ran = serie.Values.Cells;

            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }

            }
            line.ItemsSource = values;

            if (serie.SerieFormat.Fill.FillType == ExcelFillType.SolidColor && serie.SerieFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                line.Palette = ChartColorPalette.Custom;
                ChartColorModel colormodel = new ChartColorModel(ChartColorPalette.Custom);
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.Fill.ForeColor));

                colormodel.CustomBrushes = new List<Brush> {brush};
                line.ColorModel = colormodel;
            }

            //if has datalabel
            //Initializing adornments for the data points information in series

            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                line.AdornmentsInfo = SfChartDataLabel(serie);
            
            line.Label = serie.Name;

            line.ShowEmptyPoints = true;

            return line;
        }

        /// <summary>
        /// It returns the SfPie Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>PieSeries</returns>
        internal PieSeries SfPieSeries(IChartSerie serie, IChart xlsioChart)
        {
            PieSeries pieSerie = new PieSeries {XBindingPath = "X"};
            int categoryCount = 1;
            pieSerie.YBindingPath = "Value";
            IRange[] ran = serie.Values.Cells;
            ViewModel model = new ViewModel(ran.Length);
            ObservableCollection<ChartPoint> values = model.Products;
            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }
            }
            pieSerie.ItemsSource = values;

            //if has datalabel
            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                pieSerie.AdornmentsInfo = SfChartDataLabel(serie);

            pieSerie.StartAngle = (serie.SerieFormat.CommonSerieOptions.FirstSliceAngle + PieAngle) % 360;

            if (serie.SerieFormat.Percent != 0)
            {
                pieSerie.ExplodeAll = true;
                pieSerie.ExplodeRadius = serie.SerieFormat.Percent;
            }
            //Series Border Settings
            if (serie.SerieFormat.LineProperties.LineColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.LineProperties.LineColor));
                pieSerie.BorderBrush = brush;
            }
            else
            {
                Brush brush = new SolidColorBrush(SfColor(System.Drawing.Color.White));
                pieSerie.BorderBrush = brush;
            }
            if (serie.SerieFormat.LineProperties.LineWeight != ExcelChartLineWeight.Hairline)
            {
                int borderWidth = (int)serie.SerieFormat.LineProperties.LineWeight;
                borderWidth = borderWidth > 0 ? borderWidth : 1;
                pieSerie.BorderThickness = new Thickness(borderWidth);
            }

            return pieSerie;
        }

        /// <summary>
        /// It returns the SfDoughnut Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>DoughnutSeries</returns>
        internal DoughnutSeries SfDoughnutSeries(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            DoughnutSeries doughnut = new DoughnutSeries {XBindingPath = "X", YBindingPath = "Value"};
            int categoryCount = 1;
            IRange[] ran = serie.Values.Cells;

            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }

            }
            doughnut.ItemsSource = values;

            if (serie.SerieFormat.Fill.FillType == ExcelFillType.SolidColor && serie.SerieFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                doughnut.Palette = ChartColorPalette.Custom;
                ChartColorModel colormodel = new ChartColorModel(ChartColorPalette.Custom);
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.Fill.ForeColor));

                colormodel.CustomBrushes = new List<Brush> {brush};
                doughnut.ColorModel = colormodel;
            }

            //Initializing adornments for the data points information in series

            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                doughnut.AdornmentsInfo = SfChartDataLabel(serie);

            doughnut.Label = serie.Name;

            return doughnut;
        }

        /// <summary>
        /// It returns the SfStackingArea Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>StackingAreaSeries</returns>
        internal StackingAreaSeries SfStackAreaSeries(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            StackingAreaSeries stackArea = new StackingAreaSeries {XBindingPath = "X",YBindingPath = "Value"};
            int categoryCount = 1;
            if (!serie.UsePrimaryAxis)
            {
                stackArea.YAxis = new NumericalAxis { ShowGridLines = false, RangePadding = NumericalPadding.Normal, OpposedPosition = true };
                SfNumericalAxis(stackArea.YAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                if (_secondayAxisAchived)
                {
                    stackArea.YAxis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //set the Secondary axis title
                    SfAxisTitle(stackArea.YAxis, stackArea.XAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                }
                _secondayAxisAchived = true;
            }
            
            IRange[] ran = serie.Values.Cells;

            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }

            }
            stackArea.ItemsSource = values;

            //Series Fill Settings
            if (serie.SerieFormat.Fill.FillType == ExcelFillType.SolidColor && serie.SerieFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                stackArea.Palette = ChartColorPalette.Custom;
                ChartColorModel colormodel = new ChartColorModel(ChartColorPalette.Custom);
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.Fill.ForeColor));

                colormodel.CustomBrushes = new List<Brush> {brush};
                stackArea.ColorModel = colormodel;
            }

            //Series Border Settings
            if (serie.SerieFormat.LineProperties.LineColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.LineProperties.LineColor));
                stackArea.BorderBrush = brush;
            }
            if (serie.SerieFormat.LineProperties.LineWeight != ExcelChartLineWeight.Hairline)
            {
                int borderWidth = (int)serie.SerieFormat.LineProperties.LineWeight;
                stackArea.BorderThickness = new Thickness(borderWidth);
            }

            //Initializing adornments for the data points information in series

            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                stackArea.AdornmentsInfo = SfChartDataLabel(serie);

            stackArea.Label = serie.Name;


            return stackArea;
        }

        /// <summary>
        /// It returns the SfStackingBar Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>StackingBarSeries</returns>
        internal StackingBarSeries SfStackBarSeries(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            StackingBarSeries stackBar = new StackingBarSeries {XBindingPath = "X",YBindingPath = "Value"};
            int categoryCount = 1;
            if (!serie.UsePrimaryAxis)
            {
                stackBar.YAxis = new NumericalAxis { ShowGridLines = false, RangePadding = NumericalPadding.Normal, OpposedPosition = true };
                SfNumericalAxis(stackBar.YAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                if (_secondayAxisAchived)
                {
                    stackBar.YAxis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //set the Secondary axis title
                    SfAxisTitle(stackBar.YAxis, stackBar.XAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                }
                _secondayAxisAchived = true;
            }
            IRange[] ran = serie.Values.Cells;

            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;

                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }
            }
            stackBar.ItemsSource = values;

            //Series Fill Settings
            if (serie.SerieFormat.Fill.FillType == ExcelFillType.SolidColor && serie.SerieFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                stackBar.Palette = ChartColorPalette.Custom;
                ChartColorModel colormodel = new ChartColorModel(ChartColorPalette.Custom);
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.Fill.ForeColor));

                colormodel.CustomBrushes = new List<Brush> { brush };
                stackBar.ColorModel = colormodel;
            }

            //if has datalabel
            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                stackBar.AdornmentsInfo = SfChartDataLabel(serie);
            stackBar.Label = serie.Name;

            //barSeries.VisibilityOnLegend = Visibility.Visible;
            return stackBar;
        }

        /// <summary>
        /// It returns the SfStackingColumn Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>StackingColumnSeries</returns>
        internal StackingColumnSeries SfStackedColumnSeries(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            StackingColumnSeries stackColumn = new StackingColumnSeries {XBindingPath = "X",YBindingPath = "Value"};
            int categoryCount = 1;
            if (!serie.UsePrimaryAxis)
            {
                stackColumn.YAxis = new NumericalAxis { ShowGridLines = false, RangePadding = NumericalPadding.Normal, OpposedPosition = true };
                SfNumericalAxis(stackColumn.YAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                if (_secondayAxisAchived)
                {
                    stackColumn.YAxis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //set the Secondary axis title
                    SfAxisTitle(stackColumn.YAxis, stackColumn.XAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                }
                _secondayAxisAchived = true;
            }
            
            IRange[] ran = serie.Values.Cells;

            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }

            }
            stackColumn.ItemsSource = values;

            if (serie.SerieFormat.Fill.FillType == ExcelFillType.SolidColor && serie.SerieFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                stackColumn.Palette = ChartColorPalette.Custom;
                ChartColorModel colormodel = new ChartColorModel(ChartColorPalette.Custom);
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.Fill.ForeColor));

                colormodel.CustomBrushes = new List<Brush> {brush};
                stackColumn.ColorModel = colormodel;
            }

            //Initializing adornments for the data points information in series

            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                stackColumn.AdornmentsInfo = SfChartDataLabel(serie);

            stackColumn.Label = serie.Name;


            return stackColumn;
        }

        /// <summary>
        /// It returns the SfStackingColumn100 Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>StackingColumn100Series</returns>
        internal StackingColumn100Series SfStackColum100Series(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            StackingColumn100Series stackColumn100 = new StackingColumn100Series {XBindingPath = "X",YBindingPath = "Value"};
            int categoryCount = 1;
            if (!serie.UsePrimaryAxis)
            {
                stackColumn100.YAxis = new NumericalAxis { ShowGridLines = false, RangePadding = NumericalPadding.Normal, OpposedPosition = true };
                SfNumericalAxis(stackColumn100.YAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                if (_secondayAxisAchived)
                {
                    stackColumn100.YAxis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //set the Secondary axis title
                    SfAxisTitle(stackColumn100.YAxis, stackColumn100.XAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                }
                _secondayAxisAchived = true;
            }
            IRange[] ran = serie.Values.Cells;

            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }

            }
            stackColumn100.ItemsSource = values;

            //Series Fill Settings
            if (serie.SerieFormat.Fill.FillType == ExcelFillType.SolidColor && serie.SerieFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                stackColumn100.Palette = ChartColorPalette.Custom;
                ChartColorModel colormodel = new ChartColorModel(ChartColorPalette.Custom);
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.Fill.ForeColor));

                colormodel.CustomBrushes = new List<Brush> {brush};
                stackColumn100.ColorModel = colormodel;
            }

            //Series Border Settings
            if (serie.SerieFormat.LineProperties.LineColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.LineProperties.LineColor));
                stackColumn100.BorderBrush = brush;
            }
            if (serie.SerieFormat.LineProperties.LineWeight != ExcelChartLineWeight.Hairline)
            {
                int borderWidth = (int)serie.SerieFormat.LineProperties.LineWeight;
                stackColumn100.BorderThickness = new Thickness(borderWidth);
            }

            //Initializing adornments for the data points information in series

            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                stackColumn100.AdornmentsInfo = SfChartDataLabel(serie);

            stackColumn100.Label = serie.Name;


            return stackColumn100;
        }

        /// <summary>
        /// It returns the SfStackingArea100 Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>StackingArea100Series</returns>
        internal StackingArea100Series SfStackArea100Series(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            StackingArea100Series stackArea100 = new StackingArea100Series {XBindingPath = "X",YBindingPath = "Value"};
            int categoryCount = 1;
            if (!serie.UsePrimaryAxis)
            {
                stackArea100.YAxis = new NumericalAxis { ShowGridLines = false, RangePadding = NumericalPadding.Normal, OpposedPosition = true };
                SfNumericalAxis(stackArea100.YAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                if (_secondayAxisAchived)
                {
                    stackArea100.YAxis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //set the Secondary axis title
                    SfAxisTitle(stackArea100.YAxis, stackArea100.XAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                }
                _secondayAxisAchived = true;
            }
            
            IRange[] ran = serie.Values.Cells;

            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }

            }
            stackArea100.ItemsSource = values;

            //Series Fill Settings
            if (serie.SerieFormat.Fill.FillType == ExcelFillType.SolidColor && serie.SerieFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                stackArea100.Palette = ChartColorPalette.Custom;
                ChartColorModel colormodel = new ChartColorModel(ChartColorPalette.Custom);
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.Fill.ForeColor));

                colormodel.CustomBrushes = new List<Brush> {brush};
                stackArea100.ColorModel = colormodel;
            }

            //Series Border Settings
            if (serie.SerieFormat.LineProperties.LineColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.LineProperties.LineColor));
                stackArea100.BorderBrush = brush;
            }
            if (serie.SerieFormat.LineProperties.LineWeight != ExcelChartLineWeight.Hairline)
            {
                int borderWidth = (int)serie.SerieFormat.LineProperties.LineWeight;
                stackArea100.BorderThickness = new Thickness(borderWidth);
            }

            //Initializing adornments for the data points information in series

            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                stackArea100.AdornmentsInfo = SfChartDataLabel(serie);

            stackArea100.Label = serie.Name;


            return stackArea100;
        }

        /// <summary>
        /// It returns the SfStackingBar100 Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>StackingBar100Series</returns>
        internal StackingBar100Series SfStackBar100Series(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            StackingBar100Series stackBar100 = new StackingBar100Series {XBindingPath = "X",YBindingPath = "Value"};
            int categoryCount = 1;
            if (!serie.UsePrimaryAxis)
            {
                stackBar100.YAxis = new NumericalAxis { ShowGridLines = false, RangePadding = NumericalPadding.Normal, OpposedPosition = true };
                SfNumericalAxis(stackBar100.YAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                if (_secondayAxisAchived)
                {
                    stackBar100.YAxis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //set the Secondary axis title
                    SfAxisTitle(stackBar100.YAxis, stackBar100.XAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                }
                _secondayAxisAchived = true;
            }
            
            IRange[] ran = serie.Values.Cells;

            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }

            }
            stackBar100.ItemsSource = values;

            //Series Fill Settings
            if (serie.SerieFormat.Fill.FillType == ExcelFillType.SolidColor && serie.SerieFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                stackBar100.Palette = ChartColorPalette.Custom;
                ChartColorModel colormodel = new ChartColorModel(ChartColorPalette.Custom);
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.Fill.ForeColor));

                colormodel.CustomBrushes = new List<Brush> {brush};
                stackBar100.ColorModel = colormodel;
            }

            //Series Border Settings
            if (serie.SerieFormat.LineProperties.LineColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.LineProperties.LineColor));
                stackBar100.BorderBrush = brush;
            }
            if (serie.SerieFormat.LineProperties.LineWeight != ExcelChartLineWeight.Hairline)
            {
                int borderWidth = (int)serie.SerieFormat.LineProperties.LineWeight;
                stackBar100.BorderThickness = new Thickness(borderWidth);
            }

            //Initializing adornments for the data points information in series

            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                stackBar100.AdornmentsInfo = SfChartDataLabel(serie);

            stackBar100.Label = serie.Name;

            return stackBar100;
        }

        /// <summary>
        /// It returns the SfRadar Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>RadarSeries</returns>
        internal RadarSeries SfRadarSeries(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            RadarSeries radarSerie = new RadarSeries {XBindingPath = "X",YBindingPath = "Value"};
            int categoryCount = 1;
            IRange[] ran = serie.Values.Cells;
            if (!serie.UsePrimaryAxis)
            {
                radarSerie.YAxis = new NumericalAxis { ShowGridLines = false, RangePadding = NumericalPadding.Normal, OpposedPosition = true };
                SfNumericalAxis(radarSerie.YAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                if (_secondayAxisAchived)
                {
                    radarSerie.YAxis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //set the Secondary axis title
                    SfAxisTitle(radarSerie.YAxis, radarSerie.XAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                }
                _secondayAxisAchived = true;
            }
            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }

            }
            radarSerie.ItemsSource = values;

            //Series Fill Settings
            if (serie.SerieFormat.Fill.FillType == ExcelFillType.SolidColor && serie.SerieFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                radarSerie.Palette = ChartColorPalette.Custom;
                ChartColorModel colormodel = new ChartColorModel(ChartColorPalette.Custom);
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.Fill.ForeColor));

                colormodel.CustomBrushes = new List<Brush> {brush};
                radarSerie.ColorModel = colormodel;
            }
            radarSerie.ColorModel = null;
            radarSerie.Visibility = Visibility.Hidden;
            //Series Border Settings
            if (serie.SerieFormat.LineProperties.LineColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.LineProperties.LineColor));
                radarSerie.BorderBrush = brush;
            }
            if (serie.SerieFormat.LineProperties.LineWeight != ExcelChartLineWeight.Hairline)
            {
                int borderWidth = (int)serie.SerieFormat.LineProperties.LineWeight;
                radarSerie.BorderThickness = new Thickness(borderWidth);
            }

            //Initializing adornments for the data points information in series

            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                radarSerie.AdornmentsInfo = SfChartDataLabel(serie);

            radarSerie.Label = serie.Name;

            return radarSerie;
        }

        /// <summary>
        /// It returns the SfScatter Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>ScatterSeries</returns>
        internal ScatterSeries SfScatterrSeries(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            ScatterSeries scatterSerie = new ScatterSeries {XBindingPath = "X",YBindingPath = "Value"};
            int categoryCount = 1;
            if (!serie.UsePrimaryAxis)
            {
                scatterSerie.YAxis = new NumericalAxis { ShowGridLines = false, RangePadding = NumericalPadding.Normal, OpposedPosition = true };
                SfNumericalAxis(scatterSerie.YAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                if (_secondayAxisAchived)
                {
                    scatterSerie.YAxis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //set the Secondary axis title
                    SfAxisTitle(scatterSerie.YAxis, scatterSerie.XAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                }
                _secondayAxisAchived = true;
            }
            
            IRange[] ran = serie.Values.Cells;

            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }

            }
            scatterSerie.ItemsSource = values;

            //Series Fill Settings
            if (serie.SerieFormat.Fill.FillType == ExcelFillType.SolidColor && serie.SerieFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                scatterSerie.Palette = ChartColorPalette.Custom;
                ChartColorModel colormodel = new ChartColorModel(ChartColorPalette.Custom);
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.Fill.ForeColor));

                colormodel.CustomBrushes = new List<Brush> {brush};
                scatterSerie.ColorModel = colormodel;
            }

            //Series Border Settings
            if (serie.SerieFormat.LineProperties.LineColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.LineProperties.LineColor));
                scatterSerie.BorderBrush = brush;
            }
            if (serie.SerieFormat.LineProperties.LineWeight != ExcelChartLineWeight.Hairline)
            {
                int borderWidth = (int)serie.SerieFormat.LineProperties.LineWeight;
                scatterSerie.BorderThickness = new Thickness(borderWidth);
            }

            //Initializing adornments for the data points information in series

            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                scatterSerie.AdornmentsInfo = SfChartDataLabel(serie);

            scatterSerie.Label = serie.Name;

            return scatterSerie;
        }

        /// <summary>
        /// It returns the SLoOpenClose Serie.
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <param name="xlsioChart">Xlsio chart</param>
        /// <returns>HiLoOpenCloseSeries</returns>
        internal HiLoOpenCloseSeries SfStockSeries(IChartSerie serie, IChart xlsioChart)
        {
            ViewModel model = new ViewModel(serie.Values.Count);
            ObservableCollection<ChartPoint> values = model.Products;
            HiLoOpenCloseSeries hiLowOpen = new HiLoOpenCloseSeries {XBindingPath = "X"};
            int categoryCount = 1;
            if (!serie.UsePrimaryAxis)
            {
                hiLowOpen.YAxis = new NumericalAxis { ShowGridLines = false, RangePadding = NumericalPadding.Normal, OpposedPosition = true };
                SfNumericalAxis(hiLowOpen.YAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                if (_secondayAxisAchived)
                {
                    hiLowOpen.YAxis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //set the Secondary axis title
                    SfAxisTitle(hiLowOpen.YAxis, hiLowOpen.XAxis, xlsioChart.SecondaryValueAxis, xlsioChart.SecondaryCategoryAxis);
                }
                _secondayAxisAchived = true;
            }
            IRange[] ran = serie.Values.Cells;

            for (int val = 0; val < ran.Length; val++)
            {
                values[val].Value = ran[val].Number;
                {
                    values[val].X = serie.CategoryLabels != null ? serie.CategoryLabels.Cells[val].DisplayText : categoryCount++.ToString(CultureInfo.InvariantCulture);
                }

            }
            hiLowOpen.ItemsSource = values;

            //Series Fill Settings
            if (serie.SerieFormat.Fill.FillType == ExcelFillType.SolidColor && serie.SerieFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                hiLowOpen.Palette = ChartColorPalette.Custom;
                ChartColorModel colormodel = new ChartColorModel(ChartColorPalette.Custom);
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.Fill.ForeColor));

                colormodel.CustomBrushes = new List<Brush> {brush};
                hiLowOpen.ColorModel = colormodel;
            }

            //Series Border Settings
            if (serie.SerieFormat.LineProperties.LineColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                Brush brush = new SolidColorBrush(SfColor(serie.SerieFormat.LineProperties.LineColor));
                hiLowOpen.BorderBrush = brush;
            }
            if (serie.SerieFormat.LineProperties.LineWeight != ExcelChartLineWeight.Hairline)
            {
                int borderWidth = (int)serie.SerieFormat.LineProperties.LineWeight;
                hiLowOpen.BorderThickness = new Thickness(borderWidth);
            }

            //Initializing adornments for the data points information in series

            IChartDataLabels label = serie.DataPoints.DefaultDataPoint.DataLabels;
            if (label.IsCategoryName || label.IsSeriesName || label.IsValue)
                hiLowOpen.AdornmentsInfo = SfChartDataLabel(serie);

            hiLowOpen.Label = serie.Name;

            return hiLowOpen;
        }
    }
}
