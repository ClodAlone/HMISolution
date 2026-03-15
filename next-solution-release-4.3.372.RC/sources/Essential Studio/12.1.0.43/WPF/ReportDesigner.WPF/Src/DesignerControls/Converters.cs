#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using Syncfusion.RDL.Internal;


namespace Syncfusion.Windows.Reports.Designer.Controls
{
    internal class ReportingConvertorUtil
    {
        public Brush GetBackGroundColor(string colorValue)
        {
            if (string.IsNullOrEmpty(colorValue) || (colorValue != null && colorValue.StartsWith("=")))
            {
                return System.Windows.Media.Brushes.Transparent;
            }

            return new ReportingBrushConverter().ConvertFromInvariantString(colorValue);
        }

        public Brush GetColor(string colorValue)
        {
            if (string.IsNullOrEmpty(colorValue) || (colorValue != null && colorValue.StartsWith("=")))
            {
                return System.Windows.Media.Brushes.Black;
            }

            return new ReportingBrushConverter().ConvertFromInvariantString(colorValue);
        }

        public RDL.DOM.BorderStyles GetBorderStyle(string styleValue)
        {
            if (string.IsNullOrEmpty(styleValue) || (styleValue != null && styleValue.StartsWith("=")))
            {
                return RDL.DOM.BorderStyles.None;
            }

            RDL.DOM.BorderStyles borderStyle = (RDL.DOM.BorderStyles)Enum.Parse(typeof(RDL.DOM.BorderStyles), styleValue);

            return borderStyle;
        }

        public Thickness GetBorderThickness(string borderthickness)
        {
            if (string.IsNullOrEmpty(borderthickness) || (borderthickness != null && borderthickness.StartsWith("=")))
            {
                return new Thickness(1);
            }

            double borderWidth = new RDL.DOM.Size(borderthickness).PixelValue;

            if (borderWidth > 0)
            {
                return new Thickness(borderWidth);
            }

            return new Thickness(1);
        }

        public double GetFontWeight(string sizeVal)
        {
            if (string.IsNullOrEmpty(sizeVal) || (sizeVal != null && sizeVal.StartsWith("=")))
            {
                return 18;
            }

            double fontSize = new RDL.DOM.Size(sizeVal).PixelValue;

            if (fontSize > 0)
            {
                return fontSize;
            }

            return 18;
        }

        public TextAlignment GetTextAlign(string textalignValue)
        {

            if (string.IsNullOrEmpty(textalignValue) || (textalignValue != null && textalignValue.StartsWith("=")) || textalignValue.ToLower() == "default" || textalignValue.ToLower() == "general")
            {
                return TextAlignment.Left;
            }

            return (TextAlignment)Enum.Parse(typeof(TextAlignment), textalignValue);
        }

        public double GetLineThickness(string thickness)
        {
            if (string.IsNullOrEmpty(thickness) || (thickness != null && thickness.StartsWith("=")))
            {
                return 1;
            }

            return new RDL.DOM.Size(thickness).PixelValue;
        }

        public FontStyle GetFontStyle(string fontstyle)
        {
            if (string.IsNullOrEmpty(fontstyle) || (fontstyle != null && fontstyle.StartsWith("=")) || fontstyle.ToLower() == "default" || fontstyle.ToLower() == "bold" || fontstyle.ToLower()=="normal")
            {
                return FontStyles.Normal;
            }
            else
            {
                return FontStyles.Italic;
            }
           
        }

        List<string> fontNames = null;
        public string GetFontFamily(string fontfamily)
        {
            if (string.IsNullOrEmpty(fontfamily) || (fontfamily != null && fontfamily.StartsWith("=")))
            {
                return "Arial";
            }

            if (fontNames == null)
            {
                fontNames = new List<string>();
                foreach (var font in Fonts.SystemFontFamilies)
                {
                    fontNames.Add(font.ToString());
                }
            }

            if (fontNames.Contains(fontfamily))
            {
                return fontfamily;
            }

            return "Arial";
        }

        public DoubleCollection GetLineStyle(string linestyle)
        {
            DoubleCollection dasharray=new DoubleCollection();

            if (string.IsNullOrEmpty(linestyle) || (linestyle != null && linestyle.StartsWith("=")) )
            {
                dasharray.Add(2);
                dasharray.Add(0);
                return dasharray;
            }
            else if (linestyle.ToLower() == "dashed")
            {
                dasharray.Add(3);
                dasharray.Add(1);
                return dasharray;
            }
            else if (linestyle.ToLower() == "dotted")
            {
                dasharray.Add(1);
                dasharray.Add(1);
                return dasharray;
            }
            else
            {
                dasharray.Add(2);
                dasharray.Add(0);
                return dasharray;
            }
        }

        public Stretch GetSizing(RDL.DOM.Sizing imageStretch)
        {
            if (imageStretch == RDL.DOM.Sizing.Fit)
            {
                return System.Windows.Media.Stretch.Fill;
            }
            else if (imageStretch == RDL.DOM.Sizing.AutoSize)
            {
                return System.Windows.Media.Stretch.None;
            }
            else if (imageStretch == RDL.DOM.Sizing.FitProportional)
            {
                return System.Windows.Media.Stretch.Uniform;
            }
            else
            {
                return System.Windows.Media.Stretch.None;
            }
        }

        public TextDecorationCollection GetTextDecoration(String textdecorationValue)
        {
            TextDecorationCollection decor = new TextDecorationCollection();
            if (!string.IsNullOrEmpty(textdecorationValue) || (textdecorationValue != null && !textdecorationValue.StartsWith("=")))
            {
                if (textdecorationValue == "Default")
                    textdecorationValue = "None";
                {
                    if (textdecorationValue.ToLower() == "underline")
                    {
                        decor.Add(TextDecorations.Underline);
                    }
                    else if (textdecorationValue.ToLower() == "baseline")
                    {
                        decor.Add(TextDecorations.Baseline);
                    }
                    else if (textdecorationValue.ToLower() == "overline")
                    {
                        decor.Add(TextDecorations.OverLine);
                    }
                    else if (textdecorationValue.ToLower() == "strikethrough" || textdecorationValue.ToLower() == "linethrough")
                    {
                        decor.Add(TextDecorations.Strikethrough);
                    }
                }
            }
            return decor;
        }

        public Syncfusion.Windows.Gauge.GaugeFrameType GetGaugeType(String styleValue)
        {
            if (string.IsNullOrEmpty(styleValue) || (styleValue != null && styleValue.StartsWith("=")))
            {
                return Syncfusion.Windows.Gauge.GaugeFrameType.FullCircle;
            }

            switch (styleValue)
            {
                case "Circular4":
                    {
                        return Syncfusion.Windows.Gauge.GaugeFrameType.CircularWithDarkOuterFrames; 
                    }

                case "Circular1":
                    {
                        return Syncfusion.Windows.Gauge.GaugeFrameType.FullCircle;
                    }

                case "Circular2":
                    {
                        return Syncfusion.Windows.Gauge.GaugeFrameType.CircularWithInnerLeftGradient;
                    }

                case "Circular3":
                    {
                        return Syncfusion.Windows.Gauge.GaugeFrameType.CircularWithInnerTopGradient;
                    }
                default:
                    {
                        return Syncfusion.Windows.Gauge.GaugeFrameType.FullCircle;
                    }
            }
        }

        public Syncfusion.Windows.Chart.ChartTypes GetChartType(string chartType)
        {

            if (string.IsNullOrEmpty(chartType) || chartType != null && chartType.StartsWith("="))
            {
                return Syncfusion.Windows.Chart.ChartTypes.Column;
            }
            else
            {
                return (Syncfusion.Windows.Chart.ChartTypes)Enum.Parse(typeof(Syncfusion.Windows.Chart.ChartTypes), chartType, true); ;
            }
        }

        public Syncfusion.Windows.Chart.Symbol GetAdornmentType(string adornmentType)
        {
            if (string.IsNullOrEmpty(adornmentType) || adornmentType != null && adornmentType.StartsWith("="))
            {
                return Syncfusion.Windows.Chart.Symbol.Diamond;
            }
            else
            {
                return (Syncfusion.Windows.Chart.Symbol)Enum.Parse(typeof(Syncfusion.Windows.Chart.Symbol), adornmentType, true);
            }
        }

        public double GetBorderWidth(string borderwidth)
        {
            if (string.IsNullOrEmpty(borderwidth) || (borderwidth != null && borderwidth.StartsWith("=")))
            {
                return 1;
            }

            double borderWidth = new RDL.DOM.Size(borderwidth).PixelValue;

            if (borderWidth > 0)
            {
                return borderWidth;
            }

            return 1;
        }

        public FontFamily GetFontFamilyName(string fontfamily)
        {
            if (string.IsNullOrEmpty(fontfamily) || (fontfamily != null && fontfamily.StartsWith("=")))
            {
                return new FontFamily("Arial");
            }

            return new FontFamily(fontfamily);
        }

        public FontWeight GetFontWeights(string fontWeights)
        {
            if (string.IsNullOrEmpty(fontWeights) || fontWeights == "Default" || (fontWeights != null && fontWeights.StartsWith("=")))
            {
                return FontWeights.Normal;
            }
            else
            {
                return (FontWeight)new FontWeightConverter().ConvertFromString(fontWeights);
            }
        }

        public Syncfusion.Windows.Chart.ChartAlignment GetChartAlignment(string chartaAlignment)
        {
            if (string.IsNullOrEmpty(chartaAlignment) || (chartaAlignment != null && chartaAlignment.StartsWith("=")))
            {
                return Syncfusion.Windows.Chart.ChartAlignment.Center;
            }

            return (Chart.ChartAlignment)Enum.Parse(typeof(Chart.ChartAlignment),chartaAlignment,true);
        }

        public double GetFontSize(string sizeVal)
        {
            if (string.IsNullOrEmpty(sizeVal) || (sizeVal != null && sizeVal.StartsWith("=")))
            {
                return 8;
            }

            double fontSize = new RDL.DOM.Size(sizeVal).PixelValue;

            if (fontSize > 0)
            {
                return fontSize;
            }

            return 8;
        }


        public System.Windows.Visibility GetVisiblity(bool value)
        {
            if (value)
            {
                return System.Windows.Visibility.Visible;
            }

            return System.Windows.Visibility.Collapsed;
        }

        public double GetPixelValue(string value)
        {
            if (string.IsNullOrEmpty(value) || (value != null && value.StartsWith("=")))
            {
                return 0;
            }

            return new RDL.DOM.Size(value).PixelValue;
        }

        public double GetRulerWidthValue(string value)
        {
            if (string.IsNullOrEmpty(value) || (value != null && value.StartsWith("=")))
            {
                return 0;
            }

            double pixelValue = new RDL.DOM.Size(value).FloatValue;
            return pixelValue;
        }

        public string GetSizeValue(double value,string sizeValue)
        {
            RDL.DOM.MeasurementUnits measure;
            if (string.IsNullOrEmpty(sizeValue))
            {
                measure = RDL.DOM.MeasurementUnits.In;
                if (Dialogs.ControlProperties.UnitType == RDL.DOM.ReportUnitType.Cm)
                {
                    measure = RDL.DOM.MeasurementUnits.Cm;
                }
            }
            else
            {
                RDL.DOM.Size size = new RDL.DOM.Size(sizeValue);
                measure = size.MeasurementUnit;
            }

            NumberFormatInfo info = new NumberFormatInfo();
            info.NumberDecimalDigits = 4;
            string calculatedValue = GetMeasuredValue(value, measure).ToString("N", info);
            return calculatedValue + GetMeasuredString(measure);
        }

        string GetMeasuredString(RDL.DOM.MeasurementUnits measurementUnit)
        {
            if (measurementUnit == RDL.DOM.MeasurementUnits.In)
            {
                return "in";
            }
            else if (measurementUnit == RDL.DOM.MeasurementUnits.Pt)
            {
                return "pt";
            }
            else if (measurementUnit == RDL.DOM.MeasurementUnits.Cm)
            {
                return "cm";
            }
            else if (measurementUnit == RDL.DOM.MeasurementUnits.Mm)
            {
                return "mm";
            }
            else if (measurementUnit == RDL.DOM.MeasurementUnits.Pc)
            {
                return "pc";
            }
            else
            {
                return string.Empty;
            }
        }

        double GetMeasuredValue(double pixelValue, RDL.DOM.MeasurementUnits measurementUnit)
        {
            if (measurementUnit == RDL.DOM.MeasurementUnits.In)
            {
                return pixelValue / 96;
            }
            else if (measurementUnit == RDL.DOM.MeasurementUnits.Pt)
            {
                return pixelValue / 1.333333333;
            }
            else if (measurementUnit == RDL.DOM.MeasurementUnits.Cm)
            {
                return pixelValue / 37.795275591;
            }
            else if (measurementUnit == RDL.DOM.MeasurementUnits.Mm)
            {
                return pixelValue / 3.7795275591;
            }
            else if (measurementUnit == RDL.DOM.MeasurementUnits.Pc)
            {
                return pixelValue / 16;
            }
            else
            {
                return pixelValue;
            }
        }
    }

    internal class BooleanVisiblityConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return System.Windows.Visibility.Visible;
            }

            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    internal class TablixGridLengthConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            bool focusedValue = (bool)value;

            if (focusedValue)
            {
                return new System.Windows.GridLength(20);
            }

            return new GridLength(0);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    internal class TablixMarginConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            bool focusedValue = (bool)value;

            if (focusedValue)
            {
                return new Thickness(-20, -20, 0, 0);
            }

            return new Thickness(0);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    internal class GridLengthConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return new System.Windows.GridLength((double)value);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return ((System.Windows.GridLength)value).Value;
        }
    }

    internal class RadiusConvertor : IMultiValueConverter
    {
        public object Convert(object[] value, System.Type targetType, object parameter, CultureInfo culture)
        {
            //double value1 = 50;
            //try
            //{
            //    double height = (double)value[0];
            //    double width = (double)value[1];

            //    if (height > width)
            //        value1 = width / 2;

            //    value1 = height / 2;
            //}
            //catch
            //{
            //}

            //if (value1 < 50)
            //    return 50;
            //else
            //return value1;

            return 100;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class BackGroundColorConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string colorValue = value as string;

            if (string.IsNullOrEmpty(colorValue) || (colorValue != null && colorValue.StartsWith("=")) || colorValue=="Embedded" )
            {
                return System.Windows.Media.Brushes.Transparent;
            }                                                                                                                 

            return new ReportingBrushConverter().ConvertFromInvariantString(colorValue);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return ((System.Windows.GridLength)value).Value;
        }
    }

    internal class ReportColorConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string colorValue = value as string;

            if (string.IsNullOrEmpty(colorValue) || (colorValue != null && colorValue.StartsWith("=")))
            {
                return System.Windows.Media.Brushes.Black;
            }

            return new ReportingBrushConverter().ConvertFromInvariantString(colorValue);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return ((System.Windows.GridLength)value).Value;
        }
    }

    internal class RectangleBorderStyleConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string styleValue = value as string;

            if (string.IsNullOrEmpty(styleValue) || (styleValue != null && styleValue.StartsWith("=")))
            {
                return RDL.DOM.BorderStyles.None;
            }

            RDL.DOM.BorderStyles borderStyle = (RDL.DOM.BorderStyles)Enum.Parse(typeof(RDL.DOM.BorderStyles), styleValue);

            return borderStyle;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return ((System.Windows.GridLength)value).Value;
        }
    }

    internal class GaugeConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string styleValue = value as string;

            if (string.IsNullOrEmpty(styleValue) || (styleValue != null && styleValue.StartsWith("=")))
            {
                return Syncfusion.Windows.Gauge.GaugeFrameType.FullCircle;
            }

            switch (styleValue)
            {
                case "Circular4":
                    {
                        return Syncfusion.Windows.Gauge.GaugeFrameType.CircularWithDarkOuterFrames; 
                    }

                case "Circular1":
                    {
                        return Syncfusion.Windows.Gauge.GaugeFrameType.FullCircle;
                    }

                case "Circular2":
                    {
                        return Syncfusion.Windows.Gauge.GaugeFrameType.CircularWithInnerLeftGradient;
                    }

                case "Circular3":
                    {
                        return Syncfusion.Windows.Gauge.GaugeFrameType.CircularWithInnerTopGradient;
                    }
                default:
                    {
                        return Syncfusion.Windows.Gauge.GaugeFrameType.FullCircle;
                    }
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return ((System.Windows.GridLength)value).Value;
        }
    }

    internal class RectangleBorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string borderthickness = value as string;
            
            if (string.IsNullOrEmpty(borderthickness) || (borderthickness != null && borderthickness.StartsWith("=")))
            {
                return new Thickness(1);
            }

            double borderWidth = new RDL.DOM.Size(borderthickness).PixelValue;

            if (borderWidth > 0)
            {
                return new Thickness(borderWidth);
            }

            return new Thickness(1);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    internal class FontColorConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string colorValue = value as string;

            if (string.IsNullOrEmpty(colorValue) || (colorValue != null && colorValue.StartsWith("=")))
            {
                return System.Windows.Media.Brushes.Black;
            }

            return new ReportingBrushConverter().ConvertFromInvariantString(colorValue);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return ((System.Windows.GridLength)value).Value;
        }
    }

    internal class FontWeightPropertyConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string sizeVal = value as string;

            if (string.IsNullOrEmpty(sizeVal) || (sizeVal != null && sizeVal.StartsWith("=")))
            {
                return 18;
            }

            double fontSize = new RDL.DOM.Size(sizeVal).PixelValue;

            if (fontSize > 0)
            {
                return fontSize;
            }

            return 18;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return ((System.Windows.GridLength)value).Value;
        }
    }

    internal class LineHeightConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string lineheightValue = value as string;

            if (string.IsNullOrEmpty(lineheightValue) || (lineheightValue != null && lineheightValue.StartsWith("=")))
            {
                return 0;
            }

            return new RDL.DOM.Size(lineheightValue).PixelValue;

        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return ((System.Windows.GridLength)value).Value;
        }
    }

    internal class LineColorConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string colorValue = value as string;

            if (string.IsNullOrEmpty(colorValue) || (colorValue != null && colorValue.StartsWith("=")))
            {
                return System.Windows.Media.Brushes.Black;
            }

            return new ReportingBrushConverter().ConvertFromInvariantString(colorValue);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    internal class LineThicknessConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string thickness = value as string;

            if (string.IsNullOrEmpty(thickness) || (thickness != null && thickness.StartsWith("=")))
            {
                return 1;
            }

            double thickValue = new RDL.DOM.Size(thickness).PixelValue;

            if (thickValue > 0)
            {
                return thickValue;
            }

            return 1;         
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    internal class ChartTypeConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string charttypeValue = value as string;
            if (string.IsNullOrEmpty(charttypeValue) || (charttypeValue != null && charttypeValue.StartsWith("=")))
            {
                return Syncfusion.Windows.Chart.ChartTypes.Column;
            }
            Syncfusion.Windows.Chart.ChartTypes chartType = (Syncfusion.Windows.Chart.ChartTypes)Enum.Parse(typeof(Syncfusion.Windows.Chart.ChartTypes), charttypeValue, true);
            return chartType;
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    internal class StrokeDashArrayConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string linestyle = value as string;
            System.Windows.Media.DoubleCollection dasharray = new System.Windows.Media.DoubleCollection();

            if (string.IsNullOrEmpty(linestyle) || (linestyle != null && linestyle.StartsWith("=")) || linestyle.ToLower()=="solid")
            {
                dasharray.Add(2);
                dasharray.Add(0);
                return dasharray;
            }
            else if (linestyle.ToLower() == "dashed")
            {
                dasharray.Add(3);
                dasharray.Add(1);
                return dasharray;
            }
            else
            {
                dasharray.Add(1);
                dasharray.Add(1);
                return dasharray;
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string visibile = value as string;

            if (string.IsNullOrEmpty(visibile) || (visibile != null && visibile.StartsWith("=")) || visibile.ToLower()=="true")
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    internal class LegendPosionConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string position = value as string;

            if (string.IsNullOrEmpty(position) || (position != null && position.StartsWith("=")))
            {
                return Syncfusion.Windows.Chart.ChartDock.Top;
            }

            if (position.Contains("Top"))
                position = "Top";
            if (position.Contains("Bottom"))
                position = "Bottom";
            if (position.Contains("Left"))
                position = "Left";
            if (position.Contains("Right"))
                position = "Right";

            return (Syncfusion.Windows.Chart.ChartDock)Enum.Parse(typeof(Syncfusion.Windows.Chart.ChartDock), position, true);
            
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    internal class LegendFontFamilyConverter : IValueConverter
    {
        List<string> fontNames = null;

        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string fontfamily = value as string;

            if (string.IsNullOrEmpty(fontfamily) || (fontfamily != null && fontfamily.StartsWith("=")))
            {
                return "Arial";
            }

            if (fontNames == null)
            {
                fontNames = new List<string>();
                foreach (var font in Fonts.SystemFontFamilies)
                {
                    fontNames.Add(font.ToString());
                }
            }

            if (fontNames.Contains(fontfamily))
            {
                return fontfamily;
            }

            return "Arial";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    internal class LegendFontstyleConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string fontstyle = value as string;

            if (string.IsNullOrEmpty(fontstyle) || (fontstyle != null && fontstyle.StartsWith("=")))
            {
                return "Default";
            }

            return fontstyle;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    internal class GradiendColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string primarycolor = values[0].ToString();
            string secondarycolor = values[1].ToString();
            string gradientstyle = values[2].ToString();
            string fillstyle = values[3].ToString();

            if (string.IsNullOrEmpty(fillstyle) || (fillstyle != null && fillstyle.StartsWith("=")) || (fillstyle.ToLower()=="solid"))
            {
                if ((primarycolor.Contains("DependencyProperty")) || (primarycolor != null && primarycolor.StartsWith("=")))
                {
                    return System.Windows.Media.Brushes.Transparent;
                }
                return new ReportingBrushConverter().ConvertFromInvariantString(primarycolor);
            }
            else
            {
                if ((primarycolor.Contains("DependencyProperty") || (primarycolor != null && primarycolor.StartsWith("="))) && (secondarycolor.Contains("DependencyProperty") || (secondarycolor != null && secondarycolor.StartsWith("="))))
                {
                    return new LinearGradientBrush(Colors.Transparent, Colors.Transparent, 50);
                }
                Color primary = ((Color)ColorConverter.ConvertFromString(primarycolor));
                Color secondary = ((Color)ColorConverter.ConvertFromString(secondarycolor));

                if ((string.IsNullOrEmpty(gradientstyle) || (gradientstyle != null && gradientstyle.StartsWith("=")) || gradientstyle.ToLower() == "none"))
                    return new ReportingBrushConverter().ConvertFromInvariantString(primarycolor);
                else if (gradientstyle.ToLower() == "leftright")
                    return new LinearGradientBrush(primary, secondary,0);
                else
                    return new LinearGradientBrush(primary, secondary, 90);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
