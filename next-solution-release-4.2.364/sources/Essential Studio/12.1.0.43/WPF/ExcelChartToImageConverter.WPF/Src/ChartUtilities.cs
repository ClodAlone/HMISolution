#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Syncfusion.UI.Xaml.Charts;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.ExcelChartToImageConverter
{
    /// <summary>
    /// It's assign the Excel Chart properties to SfChart.
    /// </summary>
    internal class ChartCommon
    {
        internal const string ExcelNumberFormat = "General";
        internal const string DefaultFont = "Calibri";
        private string _fontName;

        /// <summary>
        /// It's assign the Excel chart value axis settings to Sf chart Axis
        /// </summary>
        internal void SfNumericalAxis(RangeAxisBase secondaryAxis, IChartValueAxis valueAxis, IChartCategoryAxis categoryAxis)
        {
            NumericalAxis sfaxis = secondaryAxis as NumericalAxis;
            IChartValueAxis xlsiovalue = valueAxis;

            sfaxis.RangePadding = NumericalPadding.Normal;
            if (!categoryAxis.IsAutoCross)
            {
                //We need to do some calculation for get the Sfchart as MsExcel chart
                sfaxis.Origin = categoryAxis.CrossesAt - 2 + 0.5;
                sfaxis.ShowAxisNextToOrigin = true;
            }
            //For Font Setting
            sfaxis.FontFamily = new FontFamily(xlsiovalue.Font.FontName);
            sfaxis.FontSize = xlsiovalue.Font.Size;
            if (xlsiovalue.Font.Bold)
                sfaxis.FontWeight = FontWeights.Bold;
            else if (xlsiovalue.Font.Italic)
                sfaxis.FontStyle = FontStyles.Italic;

            sfaxis.Foreground = new SolidColorBrush(SfColor(xlsiovalue.Font.RGBColor));
            if (!xlsiovalue.Border.IsAutoLineColor)
                sfaxis.BorderBrush = new SolidColorBrush(SfColor(xlsiovalue.Border.LineColor));

            ////TODO:Axis BackGround Color Setting - Not Eqiuvalent to MsExcel 
            //if ((xlsiovalue as ChartAxisImpl).FrameFormat.Fill.FillType == ExcelFillType.SolidColor && (xlsiovalue as ChartAxisImpl).FrameFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            //{
            //    sfaxis.Background = new SolidColorBrush(SfColor((xlsiovalue as ChartAxisImpl).FrameFormat.Fill.ForeColor));
            //}

            //Minor Tick mark
            ExcelTickMark tickmart = xlsiovalue.MinorTickMark;

            switch (tickmart)
            {
                case ExcelTickMark.TickMark_Inside:
                    sfaxis.SmallTickLinesPosition = AxisElementPosition.Inside;
                    break;
                case ExcelTickMark.TickMark_Outside:
                    sfaxis.SmallTickLinesPosition = AxisElementPosition.Outside;
                    break;
            }
            if (xlsiovalue.MinorUnit > 0)
                sfaxis.SmallTicksPerInterval = (int)xlsiovalue.MinorUnit;

            //MajorTick Mark
            tickmart = xlsiovalue.MajorTickMark;

            switch (tickmart)
            {
                case ExcelTickMark.TickMark_Inside:
                    sfaxis.TickLinesPosition = AxisElementPosition.Inside;
                    break;
                case ExcelTickMark.TickMark_Outside:
                case ExcelTickMark.TickMark_Cross:
                    sfaxis.TickLinesPosition = AxisElementPosition.Outside;
                    break;                    
            }

            if (!xlsiovalue.HasMajorGridLines)
            {
                sfaxis.ShowGridLines = false;                
            }
            if (xlsiovalue.HasMinorGridLines)
            {
                sfaxis.SmallTicksPerInterval = 3;
            }

            sfaxis.LabelRotationAngle = xlsiovalue.TextRotationAngle;

            if (((xlsiovalue.TickLabelPosition == ExcelTickLabelPosition.TickLabelPosition_High || categoryAxis.ReversePlotOrder)
                && !(xlsiovalue.TickLabelPosition == ExcelTickLabelPosition.TickLabelPosition_High
                && categoryAxis.ReversePlotOrder)) || categoryAxis.IsMaxCross)
                sfaxis.OpposedPosition = true;

            if (xlsiovalue.MajorUnit > 0)
                sfaxis.Interval = xlsiovalue.MajorUnit;

            if (xlsiovalue.ReversePlotOrder)
                sfaxis.IsInversed = true;

            if (xlsiovalue.NumberFormat != ExcelNumberFormat)
                sfaxis.LabelFormat = xlsiovalue.NumberFormat;

            if (!xlsiovalue.Visible)
                sfaxis.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// It's assign the Excel chart category axis settings to Sf chart Axis
        /// </summary>
        /// <param name="sfchart">SfChart</param>
        /// <param name="xlsioChart">Excel Chart</param>
        internal void SfPrimaryAxis( SfChart sfchart, IChart xlsioChart)
        {
            CategoryAxis sfCataxis = sfchart.PrimaryAxis as CategoryAxis;
            IChartCategoryAxis xlsioCat = xlsioChart.PrimaryCategoryAxis;

            //For Font Setting
            sfCataxis.FontFamily = new FontFamily(xlsioCat.Font.FontName);
            sfCataxis.FontSize = xlsioCat.Font.Size;
            if (xlsioCat.Font.Bold)
                sfCataxis.FontWeight = FontWeights.Bold;
            else if (xlsioCat.Font.Italic)
                sfCataxis.FontStyle = FontStyles.Italic;
            sfCataxis.Foreground = new SolidColorBrush(SfColor(xlsioCat.Font.RGBColor));
            if (!xlsioChart.PrimaryValueAxis.IsAutoCross)
            {
                sfCataxis.Origin = xlsioChart.PrimaryValueAxis.CrossesAt;
                sfCataxis.ShowAxisNextToOrigin = true;
            }
            //For Tick marks
            ExcelTickMark tickmart = xlsioCat.MinorTickMark;
            switch (tickmart)
            {
                case ExcelTickMark.TickMark_Inside:
                    sfCataxis.TickLinesPosition = AxisElementPosition.Inside;
                    break;
                case ExcelTickMark.TickMark_Outside:
                    sfCataxis.TickLinesPosition = AxisElementPosition.Outside;
                    break;
            }

            tickmart = xlsioCat.MajorTickMark;

            switch (tickmart)
            {
                case ExcelTickMark.TickMark_Inside:
                    sfCataxis.TickLinesPosition = AxisElementPosition.Inside;
                    break;
                case ExcelTickMark.TickMark_Outside:
                    sfCataxis.TickLinesPosition = AxisElementPosition.Outside;
                    break;
            }

            if (!xlsioCat.HasMajorGridLines)
            {
                sfCataxis.ShowGridLines = false;
            }

            sfCataxis.LabelRotationAngle = xlsioCat.TextRotationAngle;

            sfCataxis.Interval = xlsioCat.TickLabelSpacing;

            if (xlsioCat.ReversePlotOrder)
            {
                sfCataxis.IsInversed = true;
            }

            if (((xlsioCat.TickLabelPosition == ExcelTickLabelPosition.TickLabelPosition_High || xlsioChart.PrimaryValueAxis.ReversePlotOrder)
                && !(xlsioCat.TickLabelPosition == ExcelTickLabelPosition.TickLabelPosition_High
                && xlsioChart.PrimaryValueAxis.ReversePlotOrder)) || xlsioChart.PrimaryValueAxis.IsMaxCross)
            {
                sfCataxis.OpposedPosition = true;
            }

            if (xlsioCat.TickLabelPosition == ExcelTickLabelPosition.TickLabelPosition_None)
                sfCataxis.Visibility = Visibility.Hidden;

            if (!xlsioCat.Visible)
                sfCataxis.Visibility = Visibility.Collapsed;

            if (xlsioCat.NumberFormat != ExcelNumberFormat)
                sfCataxis.LabelFormat = xlsioCat.NumberFormat;  
          
            // Default Plotoffset for all charts
            if (xlsioCat.IsBetween)
            {
                sfCataxis.LabelPlacement = LabelPlacement.BetweenTicks;
            }
        }

        /// <summary>
        /// It's assign the Excel chart Logerthmi axis settings to Sf chart Axis
        /// </summary>
        /// <param name="sfchart">SfChart</param>
        /// <param name="xlsioChart">Excel Chart</param>
        internal void SfLogerthmicAxis( SfChart sfchart, IChart xlsioChart)
        {
            LogarithmicAxis sfaxis = sfchart.SecondaryAxis as LogarithmicAxis;
            IChartValueAxis xlsiovalue = xlsioChart.PrimaryValueAxis;

            if (!xlsioChart.PrimaryCategoryAxis.IsAutoCross)
            {
                //We need to do some calculation for get the Sfchart as MsExcel chart
                sfaxis.Origin = xlsioChart.PrimaryCategoryAxis.CrossesAt - 2 + 0.5;
                sfaxis.ShowAxisNextToOrigin = true;
            }
            //For font and color Setting
            sfaxis.FontFamily = new FontFamily(xlsiovalue.Font.FontName);
            sfaxis.FontSize = xlsiovalue.Font.Size;
            if (xlsiovalue.Font.Bold)
                sfaxis.FontWeight = FontWeights.Bold;
            else if (xlsiovalue.Font.Italic)
                sfaxis.FontStyle = FontStyles.Italic;
            
            sfaxis.Foreground = new SolidColorBrush(SfColor(xlsiovalue.Font.RGBColor));
            if (!xlsiovalue.Border.IsAutoLineColor)
                sfaxis.BorderBrush = new SolidColorBrush(SfColor(xlsiovalue.Border.LineColor));

            ////TODO:Axis BackGround Color Setting - Not Eqiuvalent to MsExcel
            //if ((xlsiovalue as ChartAxisImpl).FrameFormat.Fill.FillType == ExcelFillType.SolidColor && (xlsiovalue as ChartAxisImpl).FrameFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            //{
            //    sfaxis.Background = new SolidColorBrush(SfColor((xlsiovalue as ChartAxisImpl).FrameFormat.Fill.ForeColor));
            //}

            //Minor Tick mark
            ExcelTickMark tickmart = xlsiovalue.MinorTickMark;

            switch (tickmart)
            {
                case ExcelTickMark.TickMark_Inside:
                    sfaxis.SmallTickLinesPosition = AxisElementPosition.Inside;
                    break;
                case ExcelTickMark.TickMark_Outside:
                    sfaxis.SmallTickLinesPosition = AxisElementPosition.Outside;
                    break;
            }
            if (xlsiovalue.MinorUnit > 0)
                sfaxis.SmallTicksPerInterval = (int)xlsiovalue.MinorUnit;

            //MajorTick Mark
            tickmart = xlsiovalue.MajorTickMark;

            switch (tickmart)
            {
                case ExcelTickMark.TickMark_Inside:
                    sfaxis.TickLinesPosition = AxisElementPosition.Inside;
                    break;
                case ExcelTickMark.TickMark_Outside:
                case ExcelTickMark.TickMark_Cross:
                    sfaxis.TickLinesPosition = AxisElementPosition.Outside;
                    break;
            }

            if (!xlsiovalue.HasMajorGridLines)
            {
                sfaxis.ShowGridLines = false;
            }
            if (xlsiovalue.HasMinorGridLines)
            {
                sfaxis.SmallTicksPerInterval = 3;
            }

            sfaxis.LabelRotationAngle = xlsiovalue.TextRotationAngle;

            if (((xlsiovalue.TickLabelPosition == ExcelTickLabelPosition.TickLabelPosition_High || xlsioChart.PrimaryCategoryAxis.ReversePlotOrder)
                && !(xlsiovalue.TickLabelPosition == ExcelTickLabelPosition.TickLabelPosition_High
                && xlsioChart.PrimaryCategoryAxis.ReversePlotOrder)) || xlsioChart.PrimaryCategoryAxis.IsMaxCross)
                sfaxis.OpposedPosition = true;

            if (xlsiovalue.MajorUnit > 0)
                sfaxis.Interval = xlsiovalue.MajorUnit;

            if (xlsiovalue.ReversePlotOrder)
                sfaxis.IsInversed = true;

            if (xlsiovalue.NumberFormat != ExcelNumberFormat)
                sfaxis.LabelFormat = xlsiovalue.NumberFormat;
        }

        /// <summary>
        /// It's assign the Excel chart DateAxis settings to Sf chart Axis
        /// </summary>
        /// <param name="sfchart">SfChart</param>
        /// <param name="xlsioChart">Excel Chart</param>
        internal void SfPrimaryDateAxis( SfChart sfchart, IChart xlsioChart)
        {
            DateTimeAxis sfdateaxis = sfchart.PrimaryAxis as DateTimeAxis;
            IChartCategoryAxis xlsioCat = xlsioChart.PrimaryCategoryAxis;
            ExcelTickMark tickmart = xlsioCat.MinorTickMark;

            //For Font Setting
            sfdateaxis.FontFamily = new FontFamily(xlsioCat.Font.FontName);
            sfdateaxis.FontSize = xlsioCat.Font.Size;
            if (xlsioCat.Font.Bold)
                sfdateaxis.FontWeight = FontWeights.Bold;
            else if (xlsioCat.Font.Italic)
                sfdateaxis.FontStyle = FontStyles.Italic;
            sfdateaxis.Foreground = new SolidColorBrush(SfColor(xlsioCat.Font.RGBColor));

            switch (tickmart)
            {
                case ExcelTickMark.TickMark_Inside:
                    sfdateaxis.TickLinesPosition = AxisElementPosition.Inside;
                    break;
                case ExcelTickMark.TickMark_Outside:
                    sfdateaxis.TickLinesPosition = AxisElementPosition.Inside;
                    break;
            }

            tickmart = xlsioCat.MajorTickMark;

            switch (tickmart)
            {
                case ExcelTickMark.TickMark_Inside:
                    sfdateaxis.TickLinesPosition = AxisElementPosition.Inside;
                    break;
                case ExcelTickMark.TickMark_Outside:
                    sfdateaxis.TickLinesPosition = AxisElementPosition.Inside;
                    break;
            }

            if (xlsioCat.HasMajorGridLines)
            {
                sfdateaxis.ShowGridLines = true;
            }

            sfdateaxis.LabelRotationAngle = xlsioCat.TextRotationAngle;

            sfdateaxis.Interval = xlsioCat.TickLabelSpacing;

            if (xlsioCat.ReversePlotOrder)
                sfdateaxis.IsInversed = true;

            if (xlsioCat.TickLabelPosition == ExcelTickLabelPosition.TickLabelPosition_High)
                sfdateaxis.OpposedPosition = true;
        }

        /// <summary>
        /// It's assign the Excel chart AxisTitle settings to Sf chart AxisTitle
        /// </summary>
        /// <param name="sfseco">SfChart Secondary Axis</param>
        /// <param name="sfprim">SfChart Primary Axis</param>
        /// <param name="xlsioValue">XlsIO Value Axis</param>
        /// <param name="xlsioCat">XlsIO Category Axis</param>
        internal void SfAxisTitle(RangeAxisBase sfseco, ChartAxis sfprim, IChartValueAxis xlsioValue, IChartCategoryAxis xlsioCat)
        {
            if (!string.IsNullOrEmpty(xlsioValue.Title))
            {
                NumericalAxis sfnum = sfseco as NumericalAxis;
                TextBlock valTitle = new TextBlock();
                SfTextBlock(valTitle, xlsioValue.TitleArea);
                valTitle.LayoutTransform = new RotateTransform(0);
                valTitle.Margin = new Thickness(0, 12, 0, 0);
                valTitle.Text = " " + xlsioValue.Title + " ";
                sfnum.Header = valTitle;
            }
            if (!string.IsNullOrEmpty(xlsioCat.Title))
            {
                CategoryAxis sfCategory = sfprim as CategoryAxis;
                TextBlock catTitle = new TextBlock();
                SfTextBlock(catTitle, xlsioValue.TitleArea);
                catTitle.LayoutTransform = new RotateTransform(0);
                catTitle.Margin = new Thickness(0, 0, 0, 12);
                catTitle.Text = " " + xlsioCat.Title + " ";
                sfCategory.Header = catTitle;
            }
        }

        /// <summary>
        /// It's assign Sf chart Legend settings
        /// </summary>
        /// <param name="sfchart">SfChart</param>
        /// <param name="xlsioChart">Excel Chart</param>
        internal void SfLegend( SfChart sfchart, IChart xlsioChart)
        {
            IChartLegend legend = xlsioChart.Legend;
            ChartLegend leg = new ChartLegend();

            #region
            //legco = new legedcoll(xlsioChart.Series[0].Values.Count);
            //for (int i = 1; i <= xlsioChart.Series[0].Values.Count; i++)
            //{
            //    legco.legend[i - 1].legtext = i.ToString();
            //}
            // leg.ItemsSource = legco.legend;
            //leg.DisplayMemberPath = "legtext";
            // DataTemplate myDataTemplate = leg.ItemTemplate;
            // TextBlock myTextBlock = (TextBlock)myDataTemplate.   
            //leg.DisplayMemberPath = "legtext";
            //leg.ItemsSource = legco.legend;
            ////leg.Name = "legtext";
            //Binding placeBinding = new Binding();
            //placeBinding.Source = legco.legend;
            //placeBinding.Path = new PropertyPath("legtext");

            //placeBinding.Mode = BindingMode.TwoWay;
            //StackPanel p = new StackPanel();
            //TextBlock n = new TextBlock();
            //n.SetBinding(TextBlock.TextProperty, placeBinding);
            //n.Text = "{Binding Item.legtext}";
            // leg.Header = n;
            //grid.Children.Add(n);
            //p.Children.Add(grid);
            //grid.Children.Add(n);
            // p.Children.Add(n);
            //leg.ItemsSource = legco.legend;
            //leg.DisplayMemberPath = "legtext";
            //leg.SetBinding(TextBlock.TextProperty, placeBinding);
            //dataTemplate.Resources.Add("leg", p);
            //leg.ItemTemplate = dataTemplate;
            #endregion

            sfchart.Legend = leg;
            
            ExcelLegendPosition pos = xlsioChart.Legend.Position;
            switch (pos)
            {
                case ExcelLegendPosition.Bottom:
                    leg.DockPosition = ChartDock.Bottom;
                    break;
                case ExcelLegendPosition.Left:
                    leg.DockPosition = ChartDock.Left;
                    break;
                case ExcelLegendPosition.Top:
                    leg.DockPosition = ChartDock.Top;
                    break;
                case ExcelLegendPosition.Right:
                    leg.DockPosition = ChartDock.Right;
                    break;
                case ExcelLegendPosition.Corner:
                    {
                        leg.VerticalAlignment = VerticalAlignment.Top;
                        leg.HorizontalAlignment = HorizontalAlignment.Right;
                    }
                    break;
            }          

            //Legent Fill settings
            if (xlsioChart.Legend.FrameFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb() && xlsioChart.Legend.FrameFormat.Fill.ForeColor.ToArgb()!= System.Drawing.Color.Empty.ToArgb())
            {
                SolidColorBrush myBrush = new SolidColorBrush(SfColor(xlsioChart.Legend.FrameFormat.Fill.ForeColor));
                (sfchart.Legend as ChartLegend).Background = myBrush;
            }

            //Font Settings
            if (xlsioChart.Legend.TextArea.RGBColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                (sfchart.Legend as ChartLegend).Foreground = new SolidColorBrush(SfColor(xlsioChart.Legend.TextArea.RGBColor));
            }

            //Boreder settings
            if (xlsioChart.Legend.TextArea.FrameFormat.Border.LineColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                (sfchart.Legend as ChartLegend).BorderBrush = new SolidColorBrush(SfColor(xlsioChart.Legend.TextArea.FrameFormat.Border.LineColor));

            }
            leg.FontSize = xlsioChart.Legend.TextArea.Size;
            _fontName = xlsioChart.Legend.TextArea.FontName;
            if (_fontName.StartsWith("+"))
            {
                _fontName = DefaultFont;
            }
            leg.FontFamily = new FontFamily(_fontName);

            (sfchart.Legend as ChartLegend).Margin = new Thickness(0, 7, 0, 11);            
        }

        /// <summary>
        /// It's used assign the XlsIo textarea properties to sf TextBlock
        /// </summary>
        /// <param name="sfTextArea">Wpf TextBlock</param>
        /// <param name="textArea">Xlsio Text Area</param>
        internal void SfTextBlock( TextBlock sfTextArea, IChartTextArea textArea)
        {
            _fontName = textArea.FontName;
            //TODO: Add support to parse the Latin font type in XlsIO.
            if (_fontName.StartsWith("+"))
            {
                _fontName = DefaultFont;
            }
            sfTextArea.FontFamily = new FontFamily(textArea.FontName);
            sfTextArea.FontSize = ApplicationImpl.ConvertUnitsStatic(textArea.Size, MeasureUnits.Point, MeasureUnits.Pixel);
            if (textArea.Bold)
                sfTextArea.FontWeight = FontWeights.Bold;
            else if (textArea.Italic)
                sfTextArea.FontStyle = FontStyles.Italic;
            if (textArea.Underline == ExcelUnderline.Single)
            {
                Underline underline = new Underline();
                sfTextArea.Inlines.Add(underline);
            }

            sfTextArea.LayoutTransform = new RotateTransform(textArea.TextRotationAngle);

            if (textArea.RGBColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                sfTextArea.Foreground = new SolidColorBrush(SfColor(textArea.RGBColor));
                if (textArea.FrameFormat.Fill.FillType == ExcelFillType.SolidColor && textArea.FrameFormat.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
                {
                    System.Drawing.Color textBodycolor = textArea.FrameFormat.Fill.ForeColor;
                    sfTextArea.Background = new SolidColorBrush(SfColor(textBodycolor));
                }
            }
        }

        /// <summary>
        /// It's assign the SfChart AxisTitle properties
        /// </summary>
        /// <param name="sfchart">sfChart</param>
        /// <param name="xlsioChart">Xlsio Chart</param>
        internal void SfChartTitle( SfChart sfchart, IChart xlsioChart)
        {
            IChartTextArea titleArea = xlsioChart.ChartTitleArea;
            TextBlock block = new TextBlock();
            SfTextBlock(block, titleArea);
            block.Margin = new Thickness(0, 8, 0, 14);
            block.Text = " " + xlsioChart.ChartTitle + " ";
            sfchart.Header = block;
        }

        /// <summary>
        /// It's assign the chart data label property
        /// </summary>
        /// <param name="serie">chart serie</param>
        /// <returns>sfChart DataLabel</returns>
        internal ChartAdornmentInfo SfChartDataLabel(IChartSerie serie)
        {
            IChartDataLabels label = serie.DataPoints[0].DataLabels;
            ChartAdornmentInfo info = new ChartAdornmentInfo
                {
                    ShowLabel = true,
                    ConnectorRotationAngle = label.TextRotationAngle
                };

            //Data Label position
            ExcelDataLabelPosition labelPos = label.Position;
            switch (labelPos)
            {
                case ExcelDataLabelPosition.Outside:
                case ExcelDataLabelPosition.Automatic:
                    info.AdornmentsPosition = AdornmentsPosition.Top;
                    break;
                case ExcelDataLabelPosition.OutsideBase:
                    info.AdornmentsPosition = AdornmentsPosition.Bottom;
                    break;
                default:
                    info.AdornmentsPosition = AdornmentsPosition.TopAndBottom;
                    break;
            }

            info.HorizontalAlignment = HorizontalAlignment.Center;

            if (serie.SerieFormat.IsMarkerSupported)
                info.ShowMarker = true;

            return info;
        }

        /// <summary>
        /// It's convert the drawing color to Media color
        /// </summary>
        /// <param name="chartcolor">Drawing color</param>
        /// <returns>Media color</returns>
        internal Color SfColor(System.Drawing.Color chartcolor)
        {
            Color sfTextColor = Color.FromRgb(chartcolor.R, chartcolor.G, chartcolor.B);
            return sfTextColor;
        }

        /// <summary>
        /// It's assign the chart plotArea settings
        /// </summary>
        /// <param name="sfChart">sfChart</param>
        /// <param name="chartArea">XlsiO chartArea</param>
        internal void SfPloatArea( SfChart sfChart, IChartFrameFormat chartArea)
        {
            if (chartArea.Fill.FillType == ExcelFillType.SolidColor && chartArea.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                sfChart.AreaBackground = new SolidColorBrush(SfColor(chartArea.Fill.ForeColor));
            }
            if (chartArea.Border.LineColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                sfChart.AreaBorderBrush = new SolidColorBrush(SfColor(chartArea.Border.LineColor));
            }
            if (chartArea.LineProperties.LineWeight != ExcelChartLineWeight.Hairline)
            {
                int borderWidth = (int)chartArea.LineProperties.LineWeight;
                sfChart.AreaBorderThickness = new Thickness(borderWidth);
            }
            else
            {
                //To set default border
                sfChart.AreaBorderThickness = new Thickness(0);
            }
        }

        /// <summary>
        /// It's assign the chartAre settings
        /// </summary>
        /// <param name="sfChart">sfChart</param>
        /// <param name="chartArea">XlsiO chartArea</param>
        internal void SfChartArea( SfChart sfChart, IChartFrameFormat chartArea)
        {
            if (chartArea.Fill.FillType == ExcelFillType.SolidColor && chartArea.Fill.ForeColor.ToArgb() != System.Drawing.Color.Black.ToArgb())
            {
                sfChart.Background = new SolidColorBrush(SfColor(chartArea.Fill.ForeColor));
            }
            if (chartArea.Border.LineColor.ToArgb() != System.Drawing.Color.Black.ToArgb() && chartArea.HasLineProperties)
            {
                sfChart.BorderBrush = new SolidColorBrush(SfColor(chartArea.Border.LineColor));
            }
            else
            {
                sfChart.BorderBrush = new SolidColorBrush(Colors.Black);
            }
            if (chartArea.LineProperties.LineWeight != ExcelChartLineWeight.Hairline)
            {
                int borderWidth = (int)chartArea.LineProperties.LineWeight;
                borderWidth += 1;
                sfChart.BorderThickness = new Thickness(borderWidth);
            }
            else
            {
                sfChart.BorderThickness = new Thickness(1);
            }
        }
    }
}
