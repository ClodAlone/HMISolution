#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.RDL.DOM;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.ItemModel;
using Syncfusion.UI.Xaml.Gauges;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using LinearPointer = Syncfusion.UI.Xaml.Gauges.LinearPointer;
using LinearPointerType = Syncfusion.UI.Xaml.Gauges.LinearPointerType;
using LinearScale = Syncfusion.UI.Xaml.Gauges.LinearScale;
using Orientation = Windows.UI.Xaml.Controls.Orientation;
using Visibility = Windows.UI.Xaml.Visibility;

namespace Syncfusion.RDL.Controls
{
    internal sealed class ReportingGauge : ContentControl
    {
        private DOM.Action action;

        internal GaugeModel GaugeModel
        {
            get;
            set;
        }

        internal IReportItemModeler Model
        {
            get;
            set;
        }

        public GaugePanelExpVal GaugeProperties
        {
            get;
            set;
        }

        ReportingBrushConverter brush;

        public ReportingGauge(IReportItemModeler model)
        {
            this.Model = model;
            GaugeModel = this.Model as GaugeModel;
            this.GaugeProperties = GaugeModel.GaugePanelProperties;
            brush = new ReportingBrushConverter();
            this.Height = this.GaugeModel.Height;
            this.Width = this.GaugeModel.Width;

            if (this.GaugeModel.PrintPageInfo != null)
            {
                Canvas.SetLeft(this, this.GaugeModel.PrintPageInfo.ActualLeft);
                Canvas.SetTop(this, this.GaugeModel.PrintPageInfo.ActualTop);
            }
            else if (this.GaugeModel.IsTablixChild && this.GaugeModel.PageInfo != null)
            {
                Canvas.SetLeft(this, this.GaugeModel.PageInfo.ActualLeft);
                Canvas.SetTop(this, this.GaugeModel.PageInfo.ActualTop);
            }
            else
            {
                Canvas.SetLeft(this, this.GaugeModel.Left);
                Canvas.SetTop(this, this.GaugeModel.Top);
            }

            if (this.GaugeProperties.RadialGauges != null)
            {
                this.Content = this.GetCircularGuage();
            }
            else if (this.GaugeProperties.Indicators != null)
            {
                ReportingIndicator gaugeControl = new ReportingIndicator(model);
                this.Content = gaugeControl;
            }
            else if (this.GaugeProperties.LinearGauges != null)
            {
                this.Content = this.GetLinearGuage();
            }
            this.ControlEvents();
        }

        void ControlEvents()
        {
            try
            {
                action = new DOM.Action();
                var actionInfo = this.GaugeModel.GaugePanelProperties;
                if (actionInfo != null)
                {
                    if (actionInfo.LinearGauges != null)
                    {
                        action.Hyperlink = actionInfo.LinearGauges.First().ActionInfo.Hyperlink;
                        action.Drillthrough = new Drillthrough()
                        {
                            Parameters = this.GetParameters(actionInfo.LinearGauges.First().ActionInfo.Parameters),
                            ReportName = actionInfo.LinearGauges.First().ActionInfo.ReportName
                        };
                    }
                    if (actionInfo.Indicators != null)
                    {
                        foreach (var indicator in actionInfo.Indicators)
                        {
                            if (indicator.ActionInfo != null)
                            {
                                action.Hyperlink = indicator.ActionInfo.First().Hyperlink;
                                action.Drillthrough = new Drillthrough()
                                {
                                    Parameters = this.GetParameters(indicator.ActionInfo.First().Parameters),
                                    ReportName = indicator.ActionInfo.First().ReportName
                                };
                            }
                        }
                    }
                    if (actionInfo.RadialGauges != null)
                    {
                        action.Hyperlink = actionInfo.RadialGauges.First().ActionInfo.Hyperlink;
                        action.Drillthrough = new Drillthrough()
                        {
                            Parameters = this.GetParameters(actionInfo.LinearGauges.First().ActionInfo.Parameters),
                            ReportName = actionInfo.LinearGauges.First().ActionInfo.ReportName
                        };
                    }

                    if (!string.IsNullOrEmpty(action.Hyperlink) || !string.IsNullOrEmpty(action.Drillthrough.ReportName))
                    {
                        this.PointerEntered += ReportingGauge_PointerEntered;
                        this.PointerExited += ReportingGauge_PointerExited;
                        this.PointerPressed += ReportingGauge_PointerPressed;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        void ReportingGauge_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(action.Hyperlink))
            {
                Windows.System.Launcher.LaunchUriAsync(new Uri(action.Hyperlink));
            }
            else if (!string.IsNullOrEmpty(action.Drillthrough.ReportName))
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
                this.GaugeModel.Model.DrillThroughReport(this.GaugeModel.Model,action.Drillthrough);
            }
        }

        void ReportingGauge_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }

        void ReportingGauge_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
        }

        DOM.Parameters GetParameters(List<ParameterExpVal> action)
        {
            if (action != null)
            {
                DOM.Parameters parameters = new Parameters();
                foreach (var para in action)
                {
                    DOM.Parameter parameter = new Parameter();
                    parameter.Name = para.Name;
                    parameter.Omit = para.Omit;
                    parameter.Value = para.Value;
                    parameters.Add(parameter);
                }
                return parameters;
            }
            return null;
        }

        SfCircularGauge GetCircularGuage()
        {
            SfCircularGauge cGuage = new SfCircularGauge();
            cGuage.Width = this.Width;
            cGuage.Height = this.Height;

            foreach (var radialgauge in this.GaugeProperties.RadialGauges)
            {
                if (radialgauge.Hidden)
                {
                    this.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                }

                if (this.GaugeProperties.Border != null && this.GaugeProperties.Border.Default != null)
                {
                    this.BorderBrush = brush.ConvertFromInvariantString(this.GaugeProperties.Border.Default.BorderBrush);
                    this.BorderThickness = new Thickness(this.GaugeProperties.Border.Default.Thickness);
                }
                double radius = 0;
                double tempHeight = this.Height;
                double tempWidth = this.Width;
                bool isFlag = true;
                foreach (var scales in radialgauge.GaugeScales)
                {
                    radius +=scales.Radius;
                    CircularScale cScale = new CircularScale();
                    if (isFlag)
                    {
                        if (this.Height > this.Width)
                        {
                            cScale.Width = cScale.Height = tempWidth;
                        }
                        else
                        {
                            cScale.Width = cScale.Height = tempHeight;
                        }
                        isFlag = false;
                    }
                    else
                    {
                        if (this.Height > this.Width)
                        {
                            tempWidth -= (radius * 2);
                            cScale.Width = cScale.Height = tempWidth;
                        }
                        else
                        {
                            tempHeight -= (radius * 2);
                            cScale.Width = cScale.Height = tempHeight;
                        }
                    }
                    cScale.StartAngle = scales.StartAngle + 90;
                    cScale.SweepAngle = scales.SweepAngle;
                    cScale.EndValue = scales.MaximumValue.Value;
                    cScale.StartValue = scales.MinimumValue.Value;

                    if (scales.MajorTickMark.Interval == 0 || double.IsNaN(scales.MajorTickMark.Interval))
                    {
                        cScale.Interval = (scales.MaximumValue.Value - scales.MinimumValue.Value) / 10;
                    }
                    else
                    {
                        cScale.Interval = scales.MajorTickMark.Interval;
                    }

                    if (scales.MinorTickMark.Interval == 0 || double.IsNaN(scales.MinorTickMark.Interval))
                    {
                        cScale.MinorTicksPerInterval = (int)(cScale.Interval / 2);
                    }
                    else
                    {
                        cScale.MinorTicksPerInterval = (int)(scales.MajorTickMark.Interval);
                    }


                    if (!scales.ReverseDirection)
                    {
                        cScale.SweepDirection = SweepDirection.Clockwise;
                    }
                    else
                    {
                        cScale.SweepDirection = SweepDirection.Counterclockwise;
                    }
                    if (scales.ScaleStyle.BorderWidth == 0)
                    {
                        cScale.RimStrokeThickness = 1;
                    }
                    else
                    {
                        cScale.RimStrokeThickness = scales.ScaleStyle.BorderWidth;
                    }

                    cScale.RimStroke = brush.ConvertFromInvariantString(scales.ScaleStyle.BorderColor);
                    cScale.LabelStroke = brush.ConvertFromInvariantString(scales.ScaleLabel.TextColor);
                    cScale.FontFamily = new FontFamily(scales.ScaleLabel.LabelStyle.FontFamily);
                    cScale.FontSize = scales.ScaleLabel.LabelStyle.FontSize;

                    cScale.TickLength = scales.MajorTickMark.Length;
                    cScale.TickStroke = brush.ConvertFromInvariantString(scales.MajorTickMark.TickMarkStyle.BorderColor);
                    cScale.TickStrokeThickness = scales.MajorTickMark.Width;
                    if (scales.MajorTickMark.TickMarkPlacement == DOM.Placement.Cross)
                    {
                        cScale.TickPosition = TickPosition.Cross;
                    }
                    else if (scales.MajorTickMark.TickMarkPlacement == DOM.Placement.Inside)
                    {
                        cScale.TickPosition = TickPosition.Inside;
                    }
                    else
                    {
                        cScale.TickPosition = TickPosition.Outside;
                    }

                    cScale.SmallTickLength = scales.MinorTickMark.Length;
                    cScale.SmallTickStroke = brush.ConvertFromInvariantString(scales.MinorTickMark.TickMarkStyle.BorderColor);
                    cScale.SmallTickStrokeThickness = scales.MinorTickMark.Width;
                    foreach (var radialpointer in scales.ScalePointer)
                    {
                        CircularPointer cPointer = new CircularPointer();
                        cPointer.Value = radialpointer.Value.Value;

                        if (radialpointer.RadialPointer.PointerType == DOM.RadialPointerType.Bar)
                        {
                            cPointer.PointerType = PointerType.RangePointer;

                            if (radialpointer.PointerStyle.BackgroundGradientType == Syncfusion.RDL.DOM.BackgroundGradientType.None)
                                cPointer.RangePointerStroke = new ReportingBrushConverter().ConvertFromInvariantString(radialpointer.PointerStyle.BackgroundColor);
                            else
                                cPointer.RangePointerStroke = GradientColor(radialpointer.PointerStyle.BackgroundColor, radialpointer.PointerStyle.BackgroundGradientEndcolor, radialpointer.PointerStyle.BackgroundGradientType);

                            if (!radialpointer.Hidden)
                            {
                                cPointer.RangePointerVisibility = Windows.UI.Xaml.Visibility.Visible;
                            }
                            else
                            {
                                cPointer.RangePointerVisibility = Windows.UI.Xaml.Visibility.Collapsed;
                            }
                        }
                        else if (radialpointer.RadialPointer.PointerType == DOM.RadialPointerType.Marker)
                        {
                            cPointer.PointerType = PointerType.SymbolPointer;

                            if (radialpointer.PointerStyle.BackgroundGradientType == Syncfusion.RDL.DOM.BackgroundGradientType.None)
                                cPointer.SymbolPointerStroke = new ReportingBrushConverter().ConvertFromInvariantString(radialpointer.PointerStyle.BackgroundColor);
                            else
                                cPointer.SymbolPointerStroke = GradientColor(radialpointer.PointerStyle.BackgroundColor, radialpointer.PointerStyle.BackgroundGradientEndcolor, radialpointer.PointerStyle.BackgroundGradientType);

                            if (!radialpointer.Hidden)
                            {
                                cPointer.SymbolPointerVisibility = Windows.UI.Xaml.Visibility.Visible;
                            }
                            else
                            {
                                cPointer.SymbolPointerVisibility = Windows.UI.Xaml.Visibility.Collapsed;
                            }
                        }
                        else
                        {
                            cPointer.PointerType = PointerType.NeedlePointer;

                            if (radialpointer.PointerStyle.BackgroundGradientType == Syncfusion.RDL.DOM.BackgroundGradientType.None)
                                cPointer.NeedlePointerStroke = new ReportingBrushConverter().ConvertFromInvariantString(radialpointer.PointerStyle.BackgroundColor);
                            else
                                cPointer.NeedlePointerStroke = GradientColor(radialpointer.PointerStyle.BackgroundColor, radialpointer.PointerStyle.BackgroundGradientEndcolor, radialpointer.PointerStyle.BackgroundGradientType);

                            if (!radialpointer.Hidden)
                            {
                                cPointer.NeedlePointerVisibility = Windows.UI.Xaml.Visibility.Visible;
                            }
                            else
                            {
                                cPointer.NeedlePointerVisibility = Windows.UI.Xaml.Visibility.Collapsed;
                            }
                        }

                        if (radialpointer.RadialPointer.CapProperties.CapStyle.BackgroundGradientType == Syncfusion.RDL.DOM.BackgroundGradientType.None)
                            cPointer.PointerCapStroke = new ReportingBrushConverter().ConvertFromInvariantString(radialpointer.RadialPointer.CapProperties.CapStyle.BackgroundColor);
                        else
                            cPointer.PointerCapStroke = GradientColor(radialpointer.RadialPointer.CapProperties.CapStyle.BackgroundColor, radialpointer.RadialPointer.CapProperties.CapStyle.BackgroundGradientEndcolor, radialpointer.RadialPointer.CapProperties.CapStyle.BackgroundGradientType);


                        cScale.Pointers.Add(cPointer);
                    }

                    foreach (var radialrange in scales.ScaleRange)
                    {
                        CircularRange cRange = new CircularRange();
                        if (radialrange.RangeStyle != null)
                        {
                            cRange.StrokeThickness = radialrange.RangeStyle.BorderWidth;

                            if (radialrange.RangeStyle.BackgroundGradientType == Syncfusion.RDL.DOM.BackgroundGradientType.None)
                                cRange.Stroke = new ReportingBrushConverter().ConvertFromInvariantString(radialrange.RangeStyle.BackgroundColor);
                            else
                                cRange.Stroke = GradientColor(radialrange.RangeStyle.BackgroundColor, radialrange.RangeStyle.BackgroundGradientEndcolor, radialrange.RangeStyle.BackgroundGradientType);

                        }
                        cRange.EndValue = radialrange.EndValue.Value;
                        cRange.StartValue = radialrange.StartValue.Value;

                        if (!string.IsNullOrEmpty(radialrange.ToolTip))
                        {
                            ToolTipService.SetToolTip(this, radialrange.ToolTip);
                        }
                        cScale.Ranges.Add(cRange);
                    }

                    if (radialgauge.GaugeScales.IndexOf(scales)==0)
                    {
                        cGuage.MainScale = cScale;
                    }
                    else
                    {
                        cGuage.SubScales.Add(cScale);
                    }
                }
            }

            return cGuage;
        }

        SfLinearGauge GetLinearGuage()
        {
            SfLinearGauge lGuage = new SfLinearGauge();
            lGuage.Width = this.Width;
            lGuage.Height = this.Height;
            foreach (var linear in this.GaugeProperties.LinearGauges)
            {
                if (linear.Orientation == DOM.Orientation.Horizontal)
                    lGuage.Orientation = Orientation.Horizontal;
                else if (linear.Orientation == DOM.Orientation.Vertical)
                    lGuage.Orientation = Orientation.Vertical;
                else
                    lGuage.Orientation = Orientation.Vertical;

                if (linear.Hidden)
                {
                    this.Visibility = Visibility.Collapsed;
                }

                if (!string.IsNullOrEmpty(linear.GaugeTooltip))
                {
                    ToolTipService.SetToolTip(this, linear.GaugeTooltip);
                }
                if (linear.GaugeFrame.BackFrameStyle != null)
                {
                    if (linear.GaugeFrame.BackFrameStyle.BackgroundGradientType == Syncfusion.RDL.DOM.BackgroundGradientType.None)
                    {
                        //lGuage.Background = new ReportingBrushConverter().ConvertFromInvariantString(linear.GaugeFrame.BackFrameStyle.BackgroundColor);
                    }
                    else
                    {
                        //lGuage.Background = GradientColor(linear.GaugeFrame.BackFrameStyle.BackgroundColor, linear.GaugeFrame.BackFrameStyle.BackgroundGradientEndcolor, linear.GaugeFrame.BackFrameStyle.BackgroundGradientType);
                    }
                }

                lGuage.Background = new SolidColorBrush(Colors.Transparent);

                if (this.GaugeProperties.Border != null && this.GaugeProperties.Border.Default != null)
                {
                    this.BorderBrush = brush.ConvertFromInvariantString(this.GaugeProperties.Border.Default.BorderBrush);
                    this.BorderThickness = new Thickness(this.GaugeProperties.Border.Default.Thickness);
                }               

                foreach (var scales in linear.GaugeScales)
                {
                    LinearScale lScale = new LinearScale();
                    lScale.Maximum = scales.MaximumValue.Value;
                    lScale.Minimum = scales.MinimumValue.Value;

                    if (lGuage.Orientation == Orientation.Vertical)
                        lScale.ScaleDirection = LinearScaleDirection.Backward;
                    else
                        lScale.ScaleDirection = LinearScaleDirection.Forward;

                    lScale.LabelStroke = brush.ConvertFromInvariantString(scales.ScaleLabel.TextColor);
                    lScale.FontFamily = new FontFamily(scales.ScaleLabel.LabelStyle.FontFamily);
                    lScale.FontSize = scales.ScaleLabel.LabelStyle.FontSize;
                    if (scales.ScaleLabel.ScalePlacment == DOM.Placement.Outside)
                    {
                        lScale.LabelPosition = LinearLabelsPosition.Above;
                    }
                    else
                    {
                        lScale.LabelPosition = LinearLabelsPosition.Below;
                    }

                    if (!double.IsNaN(scales.MajorTickMark.Interval))
                        lScale.Interval = scales.MajorTickMark.Interval;
                    else
                        lScale.Interval = (scales.MaximumValue.Value - scales.MinimumValue.Value) / 5;

                    if (!double.IsNaN(scales.MinorTickMark.Interval))
                        lScale.MinorTicksPerInterval = (int)scales.MinorTickMark.Interval;
                    else
                        lScale.MinorTicksPerInterval = (int)lScale.Interval / 4;

                    lScale.MajorTickSize = scales.MajorTickMark.Length;
                    lScale.MajorTickStroke = brush.ConvertFromInvariantString(scales.MajorTickMark.FillColor);
                    lScale.MajorTickStrokeThickness = scales.MajorTickMark.Width;

                    if (scales.MajorTickMark.TickMarkPlacement == DOM.Placement.Cross)
                    {
                        lScale.TickPosition = LinearTicksPosition.Cross;
                    }
                    else if (scales.MajorTickMark.TickMarkPlacement == DOM.Placement.Inside)
                    {
                        lScale.TickPosition = LinearTicksPosition.Below;
                    }
                    else
                    {
                        lScale.TickPosition = LinearTicksPosition.Above;
                    }

                    lScale.MinorTickSize = scales.MinorTickMark.Length;
                    lScale.MinorTickStroke = brush.ConvertFromInvariantString(scales.MinorTickMark.FillColor);
                    lScale.MinorTickStrokeThickness = scales.MinorTickMark.Width;

                    foreach (var pointer in scales.ScalePointer)
                    {
                        LinearPointer lPointer = new LinearPointer();
                        lPointer.Value = pointer.Value.Value;
                        lPointer.EnableAnimation = false;

                        if (pointer.LinearPointer.PointerType == DOM.LinearPointerType.Bar)
                        {
                            lPointer.PointerType = LinearPointerType.BarPointer;

                            if (pointer.PointerStyle.BackgroundGradientType == Syncfusion.RDL.DOM.BackgroundGradientType.None)
                            {
                                lPointer.BarPointerStroke = new ReportingBrushConverter().ConvertFromInvariantString(pointer.PointerStyle.BackgroundColor);
                            }
                            else
                            {
                                lPointer.BarPointerStroke = GradientColor(pointer.PointerStyle.BackgroundColor, pointer.PointerStyle.BackgroundGradientEndcolor, pointer.PointerStyle.BackgroundGradientType);
                            }

                            if (double.IsNaN(pointer.PointerWidth))
                            {
                                lPointer.BarPointerStrokeThickness = 5;
                            }
                            else
                            {
                                lPointer.BarPointerStrokeThickness = pointer.PointerWidth;
                            }
                        }
                        else
                        {
                            lPointer.PointerType = LinearPointerType.SymbolPointer;
                            if (pointer.PointerStyle.BackgroundGradientType == Syncfusion.RDL.DOM.BackgroundGradientType.None)
                            {
                                lPointer.SymbolPointerStroke = new ReportingBrushConverter().ConvertFromInvariantString(pointer.PointerStyle.BackgroundColor);
                            }
                            else
                            {
                                lPointer.SymbolPointerStroke = GradientColor(pointer.PointerStyle.BackgroundColor, pointer.PointerStyle.BackgroundGradientEndcolor, pointer.PointerStyle.BackgroundGradientType);
                            }

                            if (double.IsNaN(pointer.PointerWidth))
                            {
                                lPointer.SymbolPointerWidth = 5;
                            }
                            else
                            {
                                lPointer.SymbolPointerWidth = pointer.PointerWidth;
                            }
                        }

                        if (pointer.Hidden)
                        {
                            lPointer.ShowPointer = false;
                        }
                        else
                        {
                            lPointer.ShowPointer = true;
                        }
                        if (!string.IsNullOrEmpty(pointer.PointerTooltip))
                        {
                            ToolTipService.SetToolTip(lPointer, pointer.PointerTooltip);
                        }

                        lScale.Pointers.Add(lPointer);
                    }

                    foreach (var scalerange in scales.ScaleRange)
                    {
                        LinearRange lRange = new LinearRange();
                        if (scalerange.RangeStyle != null)
                        {
                            if (scalerange.RangeStyle.BackgroundGradientType == Syncfusion.RDL.DOM.BackgroundGradientType.None)
                            {
                                lRange.RangeStroke = new ReportingBrushConverter().ConvertFromInvariantString(scalerange.RangeStyle.BackgroundColor);
                            }
                            else
                            {
                                lRange.RangeStroke = GradientColor(scalerange.RangeStyle.BackgroundColor, scalerange.RangeStyle.BackgroundGradientEndcolor, scalerange.RangeStyle.BackgroundGradientType);
                            }
                        }

                        lRange.EndValue = scalerange.EndValue.Value;
                        lRange.StartValue = scalerange.StartValue.Value;
                        lRange.StartWidth = scalerange.StartWidth;
                        lRange.EndWidth = scalerange.EndWidth;

                        if (!string.IsNullOrEmpty(scalerange.ToolTip))
                        {
                            ToolTipService.SetToolTip(this, scalerange.ToolTip);
                        }

                        lScale.Ranges.Add(lRange);
                    }

                    lGuage.MainScale = lScale;
                }
                lGuage.Padding = new Thickness(0);
            }

            return lGuage;
        }

        LinearGradientBrush GradientColor(string color, string gradientEndColor, Syncfusion.RDL.DOM.BackgroundGradientType gradientType)
        {
            LinearGradientBrush lgb = new LinearGradientBrush();
            GradientStopCollection gsc = new GradientStopCollection();
            GradientStop gs = new GradientStop();

            Color bgcolor;
            Canvas colorCanvas = new Canvas();
            colorCanvas.Background = (Brush)new ReportingBrushConverter().ConvertFromString(color);
            SolidColorBrush sbrush = (SolidColorBrush)colorCanvas.Background;
            bgcolor = (Color)sbrush.Color;

            gs.Color = bgcolor;
            if (gradientType == Syncfusion.RDL.DOM.BackgroundGradientType.DiagonalLeft)
            {
                gs.Offset = 0.053;
                gsc.Add(gs);

                gs = new GradientStop();

                colorCanvas.Background = (Brush)new ReportingBrushConverter().ConvertFromString(gradientEndColor);
                sbrush = (SolidColorBrush)colorCanvas.Background;
                gs.Color = (Color)sbrush.Color;

                gs.Offset = 0.696;
                gsc.Add(gs);

                lgb.GradientStops = gsc;
                lgb.EndPoint = new Point(0.5, 1);
                lgb.StartPoint = new Point(0.5, 0);
            }
            else if (gradientType == Syncfusion.RDL.DOM.BackgroundGradientType.DiagonalRight)
            {
                gs.Offset = 1;
                gsc.Add(gs);

                gs = new GradientStop();

                colorCanvas.Background = (Brush)new ReportingBrushConverter().ConvertFromString(gradientEndColor);
                sbrush = (SolidColorBrush)colorCanvas.Background;
                gs.Color = (Color)sbrush.Color;

                gs.Offset = 0.001;
                gsc.Add(gs);

                lgb.GradientStops = gsc;
                lgb.EndPoint = new Point(0.981, 0.024);
                lgb.StartPoint = new Point(0.008, 0.996);
            }
            else if (gradientType == Syncfusion.RDL.DOM.BackgroundGradientType.HorizontalCenter)
            {
                gs.Offset = 0.05;
                gsc.Add(gs);
                GradientStop gs1 = new GradientStop();
                gs1.Color = gs.Color;
                gs1.Offset = 0.927;
                gsc.Add(gs1);

                gs = new GradientStop();

                colorCanvas.Background = (Brush)new ReportingBrushConverter().ConvertFromString(gradientEndColor);
                sbrush = (SolidColorBrush)colorCanvas.Background;
                gs.Color = (Color)sbrush.Color;

                gs.Offset = 0.464;
                gsc.Add(gs);

                lgb.GradientStops = gsc;
                lgb.EndPoint = new Point(0.389, 0.972);
                lgb.StartPoint = new Point(0.386, 0.058);
            }
            else if (gradientType == Syncfusion.RDL.DOM.BackgroundGradientType.VerticalCenter)
            {
                gs.Offset = 0.05;
                gsc.Add(gs);
                GradientStop gs1 = new GradientStop();
                gs1.Color = gs.Color;
                gs1.Offset = 0.927;
                gsc.Add(gs1);

                gs = new GradientStop();

                colorCanvas.Background = (Brush)new ReportingBrushConverter().ConvertFromString(gradientEndColor);
                sbrush = (SolidColorBrush)colorCanvas.Background;
                gs.Color = (Color)sbrush.Color;

                gs.Offset = 0.464;
                gsc.Add(gs);

                lgb.GradientStops = gsc;
                lgb.EndPoint = new Point(0.975, 0.617);
                lgb.StartPoint = new Point(0.014, 0.617);
            }
            else if (gradientType == Syncfusion.RDL.DOM.BackgroundGradientType.TopBottom)
            {
                gs.Offset = 1.0;
                gsc.Add(gs);

                gs = new GradientStop();

                colorCanvas.Background = (Brush)new ReportingBrushConverter().ConvertFromString(gradientEndColor);
                sbrush = (SolidColorBrush)colorCanvas.Background;
                gs.Color = (Color)sbrush.Color;

                gs.Offset = 0.0;
                gsc.Add(gs);

                lgb.GradientStops = gsc;
                lgb.EndPoint = new Point(0.5, 1);
                lgb.StartPoint = new Point(0.5, 0);
            }
            else
            {
                gs.Offset = 1.0;
                gsc.Add(gs);

                gs = new GradientStop();

                colorCanvas.Background = (Brush)new ReportingBrushConverter().ConvertFromString(gradientEndColor);
                sbrush = (SolidColorBrush)colorCanvas.Background;
                gs.Color = (Color)sbrush.Color;

                gs.Offset = 0.042;
                gsc.Add(gs);

                lgb.GradientStops = gsc;
                lgb.EndPoint = new Point(0.981, 0.482);
                lgb.StartPoint = new Point(-0.028, 0.494);
            }
            return lgb;
        }

    }

    internal class ReportingIndicator : UserControl
    {
        ReportingBrushConverter brush = new ReportingBrushConverter();

        public ReportingIndicator()
        {

        }

        public ReportingIndicator(IReportItemModeler pageContent)
        {
            double innerWidth = 0.0;
            double innerHeight = 0.0;
            GaugeModel gaugeModel = pageContent as GaugeModel;
            GaugePanelExpVal gaugePanelProperties = gaugeModel.GaugePanelProperties;
            this.Width = pageContent.Width;
            this.Height = pageContent.Height;
            this.BorderThickness = new Thickness(1);
            this.Background = (Brush)this.brush.ConvertFromInvariantString(gaugeModel.GaugePanelProperties.BackgroundColor);

            if (gaugeModel.GaugePanelProperties.Border != null && gaugeModel.GaugePanelProperties.Border.Default != null)
            {
                this.BorderBrush = (Brush)this.brush.ConvertFromInvariantString(gaugeModel.GaugePanelProperties.Border.Default.BorderBrush);
                this.BorderThickness = gaugeModel.GaugePanelProperties.Border.Default.Thickness == 0.0 ? new Thickness(1) : new Thickness(gaugeModel.GaugePanelProperties.Border.Default.Thickness);
            }

            StackPanel renderIndicator = new StackPanel();
            renderIndicator.Width = this.Width;
            renderIndicator.Height = this.Height;
            renderIndicator.Background = (Brush)this.brush.ConvertFromInvariantString(gaugeModel.GaugePanelProperties.BackgroundColor);

            if (this.Width > this.Height)
            {
                renderIndicator.Orientation = Orientation.Horizontal;
                innerWidth = (this.Width / gaugePanelProperties.Indicators.Count) - 2;
                innerHeight = this.Height - 2;
            }
            else
            {
                renderIndicator.Orientation = Orientation.Vertical;
                innerHeight = (this.Height / gaugePanelProperties.Indicators.Count) - 2;
                innerWidth = this.Width - 2;
            }

            foreach (var indicat in gaugePanelProperties.Indicators)
            {
                IndicatorStateExpVal indicator = null;

                var contentTemplate = new Viewbox();

                if (indicat.Hidden)
                {
                    renderIndicator.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrEmpty(indicat.ToolTip))
                {
                    ToolTipService.SetToolTip(renderIndicator, indicat.ToolTip);
                }

                var value = GetValue(indicat.TransformationType, indicat.IndicatorData.Value, indicat.MinimumValue.Value,
                                     indicat.MaximumValue.Value);

                var indicators = from data in indicat.IndicatorState where data.StartValue.Value <= value && data.EndValue.Value >= value select data;

                if (indicators.Any())
                {
                    indicator = indicators.First();
                    contentTemplate = this.GetTemplate(indicator.IndicatorStyle, indicator.FillColor);
                }
                else
                {
                    contentTemplate = this.GetTemplate(indicat.GaugeIndicatorStyle, indicat.FillColor);
                }
                renderIndicator.HorizontalAlignment = HorizontalAlignment.Center;
                renderIndicator.VerticalAlignment = VerticalAlignment.Center;
                contentTemplate.Width = innerWidth - 2;
                contentTemplate.Height = innerHeight - 3;
                if (indicat.Angle > 0)
                {
                    Point center = new Point((contentTemplate.Width - 2) / 2, (contentTemplate.Height - 3) / 2);
                    RotateTransform rotate = new RotateTransform();
                    rotate.Angle = indicat.Angle;
                    rotate.CenterX = center.X;
                    rotate.CenterY = center.Y;
                    contentTemplate.RenderTransform = rotate;
                }
                renderIndicator.Children.Add(contentTemplate);
            }
            this.Content = renderIndicator;
        }

        double GetValue(TransformationType type, double value, double min, double max)
        {
            if (type == TransformationType.Percentage)
            {
                return min + ((value / 100) * (max - min));
            }
            return value;
        }

        Viewbox GetTemplate(GaugeStateIndicatorStyles indicatorStyle, string fillColor)
        {
            Viewbox vBox = new Viewbox();
            vBox.Stretch = Stretch.Uniform;
            vBox.HorizontalAlignment = HorizontalAlignment.Center;
            vBox.VerticalAlignment = VerticalAlignment.Center;
            vBox.Margin = new Thickness(1);

            string template = "<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">";

            switch (indicatorStyle)
            {
                case GaugeStateIndicatorStyles.ArrowDown:
                    template +=
                        "<Path Data=\"F1M360.605,94.399L367.063,88.029L363.389,88.029L363.389,80.326L357.611,80.326L357.611,88.029L354.148,88.029z\" Stretch=\"Fill\" Margin=\"0.913,0.375,1.172,0.552\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M355.0625,88.4043L360.6055,93.8723L366.1485,88.4043L363.0135,88.4043L363.0135,80.7013L357.9865,80.7013L357.9865,88.4043z M360.6055,94.9253L353.2345,87.6543L357.2365,87.6543L357.2365,79.9513L363.7635,79.9513L363.7635,87.6543L367.9765,87.6543z\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,0.258,0.026\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.ArrowDownIncline:
                    template +=
                        "<Path Data=\"F1M563.55,143.892L563.612,134.823L561.013,137.421L555.567,131.974L551.481,136.059L556.929,141.506L554.479,143.955z\" Stretch=\"Fill\" Margin=\"0.53,0.53,0.339,0.489\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M552.0117,136.0591L557.4587,141.5061L555.3917,143.5731L563.1777,143.5191L563.2317,135.7341L561.0127,137.9521L555.5677,132.5041z M553.5677,144.3361L556.3987,141.5061L550.9507,136.0591L555.5677,131.4441L561.0127,136.8911L563.9927,133.9111L563.9217,144.2641z\" Fill=\"#FF4D4D4D\" Stretch=\"Fill\" Margin=\"0,0,-0.042,0.108\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.ArrowSide:
                    template +=
                        "<Path Data=\"F1M511.196,87.4349999999999L504.828,80.979L504.828,84.653L497.125,84.653L497.125,90.43L504.828,90.43L504.828,93.892z\" Stretch=\"Fill\" Margin=\"0.375,0.915,0.554,1.172\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M497.5,90.0547L505.203,90.0547L505.203,92.9777L510.67,87.4347L505.203,81.8927L505.203,85.0277L497.5,85.0277z M504.453,94.8057L504.453,90.8047L496.75,90.8047L496.75,84.2777L504.453,84.2777L504.453,80.0647L511.723,87.4347z\" Fill=\"#FF4D4D4D\" Stretch=\"Fill\" Margin=\"0,0,0.027,0.259\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.ArrowUp:
                    template +=
                        "<Path Data=\"F1M477.271,80.473L470.815,86.842L474.488,86.842L474.488,94.544L480.266,94.544L480.266,86.842L483.728,86.842z\" Stretch=\"Fill\" Margin=\"0.913,0.528,1.174,0.401\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M474.8633,94.1694L479.8903,94.1694L479.8903,86.4664L482.8133,86.4664L477.2713,80.9994L471.7293,86.4664L474.8633,86.4664z M480.6403,94.9194L474.1133,94.9194L474.1133,87.2164L469.9013,87.2164L477.2713,79.9454L484.6413,87.2164L480.6403,87.2164z\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,0.26,0.026\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.ArrowUpIncline:
                    template +=
                        "<Path Data=\"F1M509.083,134.043L500.016,133.982L502.613,136.58L497.166,142.027L501.251,146.112L506.698,140.665L509.146,143.113z\" Stretch=\"Fill\" Margin=\"0.53,0.381,0.49,0.489\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M497.6963,142.0269L501.2513,145.5819L506.6983,140.1349L508.7643,142.2019L508.7113,134.4159L500.9263,134.3639L503.1433,136.5799z M501.2513,146.6419L496.6353,142.0269L502.0833,136.5799L499.1043,133.6009L509.4553,133.6709L509.5283,144.0249L506.6983,141.1949z\" Fill=\"#FF4D4D4D\" Stretch=\"Fill\" Margin=\"0,0,0.107,-0.041\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.BoxesAllFilled:
                    template +=
                        "<Grid.ColumnDefinitions><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition></Grid.ColumnDefinitions><Grid.RowDefinitions><RowDefinition Height=\"*\"></RowDefinition><RowDefinition Height=\"*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"0\" Grid.Column=\"0\" Data=\"M317.031,512.5L323.031,512.5L323.031,506.5L317.031,506.5z\" Height=\"6\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Fill=\"" + fillColor + "\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"0\" Data=\"F1M317.281,512.25L322.781,512.25L322.781,506.75L317.281,506.75z M323.281,512.75L316.781,512.75L316.781,506.25L323.281,506.25z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"1\" Data=\"M324.968,512.5L330.968,512.5L330.968,506.5L324.968,506.5z\" Height=\"6\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Fill=\"" + fillColor + "\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"1\" Data=\"F1M325.219,512.25L330.719,512.25L330.719,506.75L325.219,506.75z M331.219,512.75L324.719,512.75L324.719,506.25L331.219,506.25z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"0\" Data=\"M317.031,520.437L323.031,520.437L323.031,514.438L317.031,514.438z\" Height=\"5.999\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Fill=\"" + fillColor + "\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"0\" Data=\"F1M317.281,520.187L322.781,520.187L322.781,514.687L317.281,514.687z M323.281,520.687L316.781,520.687L316.781,514.187L323.281,514.187z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"1\" Data=\"M324.968,520.437L330.968,520.437L330.968,514.438L324.968,514.438z\" Height=\"5.999\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Fill=\"" + fillColor + "\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"1\" Data=\"F1M325.219,520.187L330.719,520.187L330.719,514.687L325.219,514.687z M331.219,520.687L324.719,520.687L324.719,514.187L331.219,514.187z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Margin=\".3\"/>";
                    break;
                case GaugeStateIndicatorStyles.BoxesNoneFilled:
                    template +=
                        "<Grid.ColumnDefinitions><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition></Grid.ColumnDefinitions><Grid.RowDefinitions><RowDefinition Height=\"*\"></RowDefinition><RowDefinition Height=\"*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"0\" Grid.Column=\"0\" Data=\"M317.031,512.5L323.031,512.5L323.031,506.5L317.031,506.5z\" Height=\"6\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Fill=\"#FFCCCCCC\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"0\" Data=\"F1M317.281,512.25L322.781,512.25L322.781,506.75L317.281,506.75z M323.281,512.75L316.781,512.75L316.781,506.25L323.281,506.25z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"1\" Data=\"M324.968,512.5L330.968,512.5L330.968,506.5L324.968,506.5z\" Height=\"6\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Fill=\"#FFCCCCCC\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"1\" Data=\"F1M325.219,512.25L330.719,512.25L330.719,506.75L325.219,506.75z M331.219,512.75L324.719,512.75L324.719,506.25L331.219,506.25z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"0\" Data=\"M317.031,520.437L323.031,520.437L323.031,514.438L317.031,514.438z\" Height=\"5.999\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Fill=\"#FFCCCCCC\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"0\" Data=\"F1M317.281,520.187L322.781,520.187L322.781,514.687L317.281,514.687z M323.281,520.687L316.781,520.687L316.781,514.187L323.281,514.187z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"1\" Data=\"M324.968,520.437L330.968,520.437L330.968,514.438L324.968,514.438z\" Height=\"5.999\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Fill=\"#FFCCCCCC\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"1\" Data=\"F1M325.219,520.187L330.719,520.187L330.719,514.687L325.219,514.687z M331.219,520.687L324.719,520.687L324.719,514.187L331.219,514.187z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Margin=\".3\"/>";
                    break;
                case GaugeStateIndicatorStyles.BoxesOneFilled:
                    template +=
                        "<Grid.ColumnDefinitions><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition></Grid.ColumnDefinitions><Grid.RowDefinitions><RowDefinition Height=\"*\"></RowDefinition><RowDefinition Height=\"*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"0\" Grid.Column=\"0\" Data=\"M317.031,512.5L323.031,512.5L323.031,506.5L317.031,506.5z\" Height=\"6\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Fill=\"#FFCCCCCC\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"0\" Data=\"F1M317.281,512.25L322.781,512.25L322.781,506.75L317.281,506.75z M323.281,512.75L316.781,512.75L316.781,506.25L323.281,506.25z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"1\" Data=\"M324.968,512.5L330.968,512.5L330.968,506.5L324.968,506.5z\" Height=\"6\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Fill=\"#FFCCCCCC\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"1\" Data=\"F1M325.219,512.25L330.719,512.25L330.719,506.75L325.219,506.75z M331.219,512.75L324.719,512.75L324.719,506.25L331.219,506.25z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"0\" Data=\"M317.031,520.437L323.031,520.437L323.031,514.438L317.031,514.438z\" Height=\"5.999\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Fill=\"" + fillColor + "\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"0\" Data=\"F1M317.281,520.187L322.781,520.187L322.781,514.687L317.281,514.687z M323.281,520.687L316.781,520.687L316.781,514.187L323.281,514.187z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"1\" Data=\"M324.968,520.437L330.968,520.437L330.968,514.438L324.968,514.438z\" Height=\"5.999\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Fill=\"#FFCCCCCC\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"1\" Data=\"F1M325.219,520.187L330.719,520.187L330.719,514.687L325.219,514.687z M331.219,520.687L324.719,520.687L324.719,514.187L331.219,514.187z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Margin=\".3\"/>";
                    break;
                case GaugeStateIndicatorStyles.BoxesThreeFilled:
                    template +=
                        "<Grid.ColumnDefinitions><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition></Grid.ColumnDefinitions><Grid.RowDefinitions><RowDefinition Height=\"*\"></RowDefinition><RowDefinition Height=\"*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"0\" Grid.Column=\"0\" Data=\"M317.031,512.5L323.031,512.5L323.031,506.5L317.031,506.5z\" Height=\"6\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Fill=\"" + fillColor + "\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"0\" Data=\"F1M317.281,512.25L322.781,512.25L322.781,506.75L317.281,506.75z M323.281,512.75L316.781,512.75L316.781,506.25L323.281,506.25z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"1\" Data=\"M324.968,512.5L330.968,512.5L330.968,506.5L324.968,506.5z\" Height=\"6\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Fill=\"#FFCCCCCC\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"1\" Data=\"F1M325.219,512.25L330.719,512.25L330.719,506.75L325.219,506.75z M331.219,512.75L324.719,512.75L324.719,506.25L331.219,506.25z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"0\" Data=\"M317.031,520.437L323.031,520.437L323.031,514.438L317.031,514.438z\" Height=\"5.999\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Fill=\"" + fillColor + "\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"0\" Data=\"F1M317.281,520.187L322.781,520.187L322.781,514.687L317.281,514.687z M323.281,520.687L316.781,520.687L316.781,514.187L323.281,514.187z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"1\" Data=\"M324.968,520.437L330.968,520.437L330.968,514.438L324.968,514.438z\" Height=\"5.999\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Fill=\"" + fillColor + "\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"1\" Data=\"F1M325.219,520.187L330.719,520.187L330.719,514.687L325.219,514.687z M331.219,520.687L324.719,520.687L324.719,514.187L331.219,514.187z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Margin=\".3\"/>";
                    break;
                case GaugeStateIndicatorStyles.BoxesTwoFilled:
                    template +=
                        "<Grid.ColumnDefinitions><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition></Grid.ColumnDefinitions><Grid.RowDefinitions><RowDefinition Height=\"*\"></RowDefinition><RowDefinition Height=\"*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"0\" Grid.Column=\"0\" Data=\"M317.031,512.5L323.031,512.5L323.031,506.5L317.031,506.5z\" Height=\"6\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Fill=\"#FFCCCCCC\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"0\" Data=\"F1M317.281,512.25L322.781,512.25L322.781,506.75L317.281,506.75z M323.281,512.75L316.781,512.75L316.781,506.25L323.281,506.25z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"1\" Data=\"M324.968,512.5L330.968,512.5L330.968,506.5L324.968,506.5z\" Height=\"6\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Fill=\"#FFCCCCCC\" Margin=\".3\"/><Path Grid.Row=\"0\" Grid.Column=\"1\" Data=\"F1M325.219,512.25L330.719,512.25L330.719,506.75L325.219,506.75z M331.219,512.75L324.719,512.75L324.719,506.25L331.219,506.25z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Top\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"0\" Data=\"M317.031,520.437L323.031,520.437L323.031,514.438L317.031,514.438z\" Height=\"5.999\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Fill=\"" + fillColor + "\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"0\" Data=\"F1M317.281,520.187L322.781,520.187L322.781,514.687L317.281,514.687z M323.281,520.687L316.781,520.687L316.781,514.187L323.281,514.187z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"1\" Data=\"M324.968,520.437L330.968,520.437L330.968,514.438L324.968,514.438z\" Height=\"5.999\" Stretch=\"Fill\" Width=\"6\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Fill=\"" + fillColor + "\" Margin=\".3\"/><Path Grid.Row=\"1\" Grid.Column=\"1\" Data=\"F1M325.219,520.187L330.719,520.187L330.719,514.687L325.219,514.687z M331.219,520.687L324.719,520.687L324.719,514.187L331.219,514.187z\" Fill=\"#FF1B1464\" Height=\"6.5\" Stretch=\"Fill\" Width=\"6.5\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\" Margin=\".3\"/>";
                    break;
                case GaugeStateIndicatorStyles.ButtonPause:
                    break;
                case GaugeStateIndicatorStyles.ButtonPlay:
                    break;
                case GaugeStateIndicatorStyles.ButtonStop:
                    break;
                case GaugeStateIndicatorStyles.Circle:
                    template +=
                        "<Path Data=\"F1M330.9341,477.333C330.9341,481.331,327.6931,484.572,323.6951,484.572C319.6961,484.572,316.4551,481.331,316.4551,477.333C316.4551,473.335,319.6961,470.094,323.6951,470.094C327.6931,470.094,330.9341,473.335,330.9341,477.333\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" Margin=\"0.362,0.363,0.159,0.159\" UseLayoutRounding=\"False\"/><Path Data=\"F1M323.6948,470.4561C319.9028,470.4561,316.8168,473.5411,316.8168,477.3331C316.8168,481.1251,319.9028,484.2101,323.6948,484.2101C327.4868,484.2101,330.5718,481.1251,330.5718,477.3331C330.5718,473.5411,327.4868,470.4561,323.6948,470.4561 M323.6948,484.9351C319.5038,484.9351,316.0928,481.5241,316.0928,477.3331C316.0928,473.1421,319.5038,469.7311,323.6948,469.7311C327.8858,469.7311,331.2958,473.1421,331.2958,477.3331C331.2958,481.5241,327.8858,484.9351,323.6948,484.9351\" Fill=\" Black \" Stretch=\"Fill\" Margin=\"0,0,-0.203,-0.204\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.FaceFrown:
                    break;
                case GaugeStateIndicatorStyles.FaceNeutral:
                    break;
                case GaugeStateIndicatorStyles.FaceSmile:
                    break;
                case GaugeStateIndicatorStyles.Flag:
                    template +=
                        "<Path Data=\"F1M317.5352,384.9727C317.5352,384.9727,321.7282,382.6357,323.8852,385.3917C326.0412,388.1477,328.6772,388.8667,329.3962,388.6867C330.1152,388.5067,327.3592,392.3407,323.7652,391.0827C320.1712,389.8247,319.9312,391.3357,319.9312,391.3357z\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" Margin=\"0.445,0.359,0.579,7.352\" UseLayoutRounding=\"False\"/><Path Data=\"F1M321.3857,390.2227C322.0567,390.2227,322.8967,390.3987,323.8837,390.7437C324.3267,390.8987,324.7807,390.9777,325.2317,390.9777C327.0397,390.9777,328.4517,389.7247,328.9567,389.0647C327.7547,388.9727,325.4847,388.0197,323.6017,385.6137C322.9757,384.8127,322.0877,384.4077,320.9627,384.4077C319.7157,384.4077,318.5107,384.9017,317.9857,385.1487L320.0357,390.5927C320.3037,390.3907,320.7247,390.2227,321.3857,390.2227 M320.0107,392.5427L319.5947,391.4627L317.0897,384.8097L317.3607,384.6587C317.4307,384.6187,319.1227,383.6887,320.9627,383.6887C322.3207,383.6887,323.3987,384.1867,324.1677,385.1697C325.9827,387.4897,328.1827,388.3527,329.1437,388.3527C329.2347,388.3527,329.2867,388.3437,329.3097,388.3377L329.4407,388.3217C329.6857,388.3217,329.8707,388.5087,329.8707,388.7567C329.8707,389.4457,327.8937,391.6967,325.2317,391.6967C324.6997,391.6967,324.1657,391.6047,323.6467,391.4217C322.7357,391.1037,321.9757,390.9417,321.3857,390.9417C320.4067,390.9417,320.2887,391.3857,320.2837,391.4047z\" Fill=\"#FF4D4D4D\" Stretch=\"Fill\" Margin=\"0,0,0.219,6.146\" UseLayoutRounding=\"False\"/><Path Data=\"F1M322.229,398.9336L317.197,385.0956L317.873,384.8496L322.905,398.6876z\" Fill=\"Black\" Stretch=\"Fill\" Width=\"5.708\" HorizontalAlignment=\"Left\" Margin=\"0.107,1.161,0,-0.245\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.Image:
                    break;
                case GaugeStateIndicatorStyles.QuartersAllFilled:
                    template +=
                        "<Path Data=\"F1M330.9341,477.333C330.9341,481.331,327.6931,484.572,323.6951,484.572C319.6961,484.572,316.4551,481.331,316.4551,477.333C316.4551,473.335,319.6961,470.094,323.6951,470.094C327.6931,470.094,330.9341,473.335,330.9341,477.333\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" Margin=\"0.362,0.363,0.159,0.159\" UseLayoutRounding=\"False\"/><Path Data=\"F1M323.6948,470.4561C319.9028,470.4561,316.8168,473.5411,316.8168,477.3331C316.8168,481.1251,319.9028,484.2101,323.6948,484.2101C327.4868,484.2101,330.5718,481.1251,330.5718,477.3331C330.5718,473.5411,327.4868,470.4561,323.6948,470.4561 M323.6948,484.9351C319.5038,484.9351,316.0928,481.5241,316.0928,477.3331C316.0928,473.1421,319.5038,469.7311,323.6948,469.7311C327.8858,469.7311,331.2958,473.1421,331.2958,477.3331C331.2958,481.5241,327.8858,484.9351,323.6948,484.9351\" Fill=\"Black\" Stretch=\"Fill\" Margin=\"0,0,-0.203,-0.204\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.QuartersNoneFilled:
                    template +=
                        "<Path Data=\"F1M435.3311,477.333C435.3311,481.331,432.0901,484.572,428.0921,484.572C424.0931,484.572,420.8521,481.331,420.8521,477.333C420.8521,473.335,424.0931,470.094,428.0921,470.094C432.0901,470.094,435.3311,473.335,435.3311,477.333\" Fill=\"White\" Stretch=\"Fill\" Margin=\"0.362,0.363,0.159,0.159\" UseLayoutRounding=\"False\"/><Path Data=\"F1M428.0918,470.4561C424.2998,470.4561,421.2148,473.5411,421.2148,477.3331C421.2148,481.1251,424.2998,484.2101,428.0918,484.2101C431.8838,484.2101,434.9688,481.1251,434.9688,477.3331C434.9688,473.5411,431.8838,470.4561,428.0918,470.4561 M428.0918,484.9351C423.9008,484.9351,420.4898,481.5241,420.4898,477.3331C420.4898,473.1421,423.9008,469.7311,428.0918,469.7311C432.2828,469.7311,435.6938,473.1421,435.6938,477.3331C435.6938,481.5241,432.2828,484.9351,428.0918,484.9351\" Fill=\"Black\" Stretch=\"Fill\" Margin=\"0,0,-0.204,-0.204\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.QuartersOneFilled:
                    template +=
                        "<Path Data=\"F1M394.2959,477.7187C394.2959,481.7167,397.5379,484.9577,401.5349,484.9577C405.5339,484.9577,408.7749,481.7167,408.7749,477.7187C408.7749,473.7207,405.5339,470.4797,401.5349,470.4797C397.5379,470.4797,394.2959,473.7207,394.2959,477.7187\" Fill=\"White\" Stretch=\"Fill\" Margin=\"0.361,0.363,0.16,0.159\" UseLayoutRounding=\"False\"/><Path Data=\"F1M401.5352,470.8418C397.7432,470.8418,394.6582,473.9268,394.6582,477.7188C394.6582,481.5108,397.7432,484.5958,401.5352,484.5958C405.3282,484.5958,408.4132,481.5108,408.4132,477.7188C408.4132,473.9268,405.3282,470.8418,401.5352,470.8418 M401.5352,485.3208C397.3442,485.3208,393.9342,481.9098,393.9342,477.7188C393.9342,473.5278,397.3442,470.1168,401.5352,470.1168C405.7262,470.1168,409.1382,473.5278,409.1382,477.7188C409.1382,481.9098,405.7262,485.3208,401.5352,485.3208\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,-0.204,-0.204\" UseLayoutRounding=\"False\"/><Path Data=\"F1M401.5732,470.8643C405.5532,470.8853,408.4352,473.7333,408.4352,477.7183C408.4352,477.7193,408.5302,477.7213,408.5302,477.7223L401.5732,477.7223z\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" Width=\"6.957\" HorizontalAlignment=\"Right\" Margin=\"0,0.748,0.404,7.394\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.QuartersThreeFilled:
                    template +=
                        "<Path Data=\"F1M357.0332,477.333C357.0332,481.331,353.7922,484.572,349.7942,484.572C345.7952,484.572,342.5542,481.331,342.5542,477.333C342.5542,473.335,345.7952,470.094,349.7942,470.094C353.7922,470.094,357.0332,473.335,357.0332,477.333\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" Margin=\"0.363,0.363,0.158,0.159\" UseLayoutRounding=\"False\"/><Path Data=\"F1M349.7939,470.4561C346.0019,470.4561,342.9159,473.5411,342.9159,477.3331C342.9159,481.1251,346.0019,484.2101,349.7939,484.2101C353.5859,484.2101,356.6709,481.1251,356.6709,477.3331C356.6709,473.5411,353.5859,470.4561,349.7939,470.4561 M349.7939,484.9351C345.6029,484.9351,342.1919,481.5241,342.1919,477.3331C342.1919,473.1421,345.6029,469.7311,349.7939,469.7311C353.9849,469.7311,357.3949,473.1421,357.3949,477.3331C357.3949,481.5241,353.9849,484.9351,349.7939,484.9351\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,-0.203,-0.204\" UseLayoutRounding=\"False\"/><Path Data=\"F1M349.7573,470.4795C345.7763,470.4995,342.8943,473.3475,342.8943,477.3335C342.8943,477.3335,342.7993,477.3355,342.7993,477.3365L349.7573,477.3365z\" Fill=\"White\" Stretch=\"Fill\" Margin=\"0.608,0.748,7.434,7.395\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.QuartersTwoFilled:
                    template +=
                        "<Path Data=\"F1M383.1328,477.333C383.1328,481.331,379.8908,484.572,375.8938,484.572C371.8948,484.572,368.6538,481.331,368.6538,477.333C368.6538,473.335,371.8948,470.094,375.8938,470.094C379.8908,470.094,383.1328,473.335,383.1328,477.333\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" Margin=\"0.362,0.363,0.159,0.159\" UseLayoutRounding=\"False\"/><Path Data=\"F1M375.8936,470.4561C372.1016,470.4561,369.0156,473.5411,369.0156,477.3331C369.0156,481.1251,372.1016,484.2101,375.8936,484.2101C379.6856,484.2101,382.7706,481.1251,382.7706,477.3331C382.7706,473.5411,379.6856,470.4561,375.8936,470.4561 M375.8936,484.9351C371.7016,484.9351,368.2916,481.5241,368.2916,477.3331C368.2916,473.1421,371.7016,469.7311,375.8936,469.7311C380.0846,469.7311,383.4946,473.1421,383.4946,477.3331C383.4946,481.5241,380.0846,484.9351,375.8936,484.9351\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,-0.203,-0.204\" UseLayoutRounding=\"False\"/><Path Data=\"F1M375.8193,470.46C371.8543,470.5,369.0623,473.364,369.0623,477.338C369.0623,481.312,371.9283,484.176,375.8933,484.216z\" Fill=\"White\" Stretch=\"Fill\" Margin=\"0.771,0.729,7.398,0.515\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.SignalMeterFourFilled:
                    template +=
                        "<Grid.ColumnDefinitions><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition></Grid.ColumnDefinitions><Grid Grid.Column=\"0\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"75*\"></RowDefinition><RowDefinition Height=\"25*\"></RowDefinition></Grid.RowDefinitions><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" Height=\"auto\" Data=\"F1M487.444,481.307L485.532,481.307L485.532,475.57L487.444,475.57z\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\"/><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" Height=\"auto\" Data=\"F1M485.782,481.057L487.194,481.057L487.194,475.82L485.782,475.82z M487.694,481.557L485.282,481.557L485.282,475.32L487.694,475.32z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\"/></Grid><Grid Grid.Column=\"1\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"50*\"></RowDefinition><RowDefinition Height=\"50*\"></RowDefinition></Grid.RowDefinitions><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M491.15,481.307L489.237,481.307L489.237,473.657L491.15,473.657z\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\"/><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M489.487,481.057L490.9,481.057L490.9,473.907L489.487,473.907z M491.4,481.557L488.987,481.557L488.987,473.407L491.4,473.407z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\"/></Grid><Grid Grid.Column=\"2\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"25*\"></RowDefinition><RowDefinition Height=\"75*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M494.855,481.307L492.943,481.307L492.943,469.832L494.855,469.832z\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M493.193,481.057L494.605,481.057L494.605,470.082L493.193,470.082z M495.105,481.557L492.693,481.557L492.693,469.582L495.105,469.582z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/></Grid><Grid Grid.Column=\"3\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"Auto\"></RowDefinition><RowDefinition Height=\"*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M498.562,481.307L496.649,481.307L496.649,466.007L498.562,466.007z\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M496.898,481.057L498.311,481.057L498.311,466.257L496.898,466.257z M498.812,481.557L496.399,481.557L496.399,465.757L498.812,465.757z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/></Grid>";
                    break;
                case GaugeStateIndicatorStyles.SignalMeterNoneFilled:
                    template +=
                        "<Grid.ColumnDefinitions><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition></Grid.ColumnDefinitions><Grid Grid.Column=\"0\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"75*\"></RowDefinition><RowDefinition Height=\"25*\"></RowDefinition></Grid.RowDefinitions><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" Height=\"auto\" Data=\"F1M487.444,481.307L485.532,481.307L485.532,475.57L487.444,475.57z\" Fill=\"#FFCCCCCC\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\"/><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" Height=\"auto\" Data=\"F1M485.782,481.057L487.194,481.057L487.194,475.82L485.782,475.82z M487.694,481.557L485.282,481.557L485.282,475.32L487.694,475.32z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\"/></Grid><Grid Grid.Column=\"1\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"50*\"></RowDefinition><RowDefinition Height=\"50*\"></RowDefinition></Grid.RowDefinitions><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M491.15,481.307L489.237,481.307L489.237,473.657L491.15,473.657z\" Fill=\"#FFCCCCCC\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\"/><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M489.487,481.057L490.9,481.057L490.9,473.907L489.487,473.907z M491.4,481.557L488.987,481.557L488.987,473.407L491.4,473.407z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\"/></Grid><Grid Grid.Column=\"2\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"25*\"></RowDefinition><RowDefinition Height=\"75*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M494.855,481.307L492.943,481.307L492.943,469.832L494.855,469.832z\" Fill=\"#FFCCCCCC\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M493.193,481.057L494.605,481.057L494.605,470.082L493.193,470.082z M495.105,481.557L492.693,481.557L492.693,469.582L495.105,469.582z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/></Grid><Grid Grid.Column=\"3\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"Auto\"></RowDefinition><RowDefinition Height=\"*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M498.562,481.307L496.649,481.307L496.649,466.007L498.562,466.007z\" Fill=\"#FFCCCCCC\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M496.898,481.057L498.311,481.057L498.311,466.257L496.898,466.257z M498.812,481.557L496.399,481.557L496.399,465.757L498.812,465.757z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/></Grid>";
                    break;
                case GaugeStateIndicatorStyles.SignalMeterOneFilled:
                    template +=
                        "<Grid.ColumnDefinitions><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition></Grid.ColumnDefinitions><Grid Grid.Column=\"0\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"75*\"></RowDefinition><RowDefinition Height=\"25*\"></RowDefinition></Grid.RowDefinitions><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" Height=\"auto\" Data=\"F1M487.444,481.307L485.532,481.307L485.532,475.57L487.444,475.57z\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\"/><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" Height=\"auto\" Data=\"F1M485.782,481.057L487.194,481.057L487.194,475.82L485.782,475.82z M487.694,481.557L485.282,481.557L485.282,475.32L487.694,475.32z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\"/></Grid><Grid Grid.Column=\"1\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"50*\"></RowDefinition><RowDefinition Height=\"50*\"></RowDefinition></Grid.RowDefinitions><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M491.15,481.307L489.237,481.307L489.237,473.657L491.15,473.657z\" Fill=\"#FFCCCCCC\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\"/><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M489.487,481.057L490.9,481.057L490.9,473.907L489.487,473.907z M491.4,481.557L488.987,481.557L488.987,473.407L491.4,473.407z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\"/></Grid><Grid Grid.Column=\"2\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"25*\"></RowDefinition><RowDefinition Height=\"75*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M494.855,481.307L492.943,481.307L492.943,469.832L494.855,469.832z\" Fill=\"#FFCCCCCC\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M493.193,481.057L494.605,481.057L494.605,470.082L493.193,470.082z M495.105,481.557L492.693,481.557L492.693,469.582L495.105,469.582z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/></Grid><Grid Grid.Column=\"3\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"Auto\"></RowDefinition><RowDefinition Height=\"*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M498.562,481.307L496.649,481.307L496.649,466.007L498.562,466.007z\" Fill=\"#FFCCCCCC\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M496.898,481.057L498.311,481.057L498.311,466.257L496.898,466.257z M498.812,481.557L496.399,481.557L496.399,465.757L498.812,465.757z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/></Grid>";
                    break;
                case GaugeStateIndicatorStyles.SignalMeterThreeFilled:
                    template +=
                        "<Grid.ColumnDefinitions><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition></Grid.ColumnDefinitions><Grid Grid.Column=\"0\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"75*\"></RowDefinition><RowDefinition Height=\"25*\"></RowDefinition></Grid.RowDefinitions><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" Height=\"auto\" Data=\"F1M487.444,481.307L485.532,481.307L485.532,475.57L487.444,475.57z\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\"/><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" Height=\"auto\" Data=\"F1M485.782,481.057L487.194,481.057L487.194,475.82L485.782,475.82z M487.694,481.557L485.282,481.557L485.282,475.32L487.694,475.32z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\"/></Grid><Grid Grid.Column=\"1\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"50*\"></RowDefinition><RowDefinition Height=\"50*\"></RowDefinition></Grid.RowDefinitions><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M491.15,481.307L489.237,481.307L489.237,473.657L491.15,473.657z\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\"/><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M489.487,481.057L490.9,481.057L490.9,473.907L489.487,473.907z M491.4,481.557L488.987,481.557L488.987,473.407L491.4,473.407z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\"/></Grid><Grid Grid.Column=\"2\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"25*\"></RowDefinition><RowDefinition Height=\"75*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M494.855,481.307L492.943,481.307L492.943,469.832L494.855,469.832z\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M493.193,481.057L494.605,481.057L494.605,470.082L493.193,470.082z M495.105,481.557L492.693,481.557L492.693,469.582L495.105,469.582z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/></Grid><Grid Grid.Column=\"3\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"Auto\"></RowDefinition><RowDefinition Height=\"*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M498.562,481.307L496.649,481.307L496.649,466.007L498.562,466.007z\" Fill=\"#FFCCCCCC\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M496.898,481.057L498.311,481.057L498.311,466.257L496.898,466.257z M498.812,481.557L496.399,481.557L496.399,465.757L498.812,465.757z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/></Grid>";
                    break;
                case GaugeStateIndicatorStyles.SignalMeterTwoFilled:
                    template +=
                        "<Grid.ColumnDefinitions><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition><ColumnDefinition Width=\"*\"></ColumnDefinition></Grid.ColumnDefinitions><Grid Grid.Column=\"0\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"75*\"></RowDefinition><RowDefinition Height=\"25*\"></RowDefinition></Grid.RowDefinitions><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" Height=\"auto\" Data=\"F1M487.444,481.307L485.532,481.307L485.532,475.57L487.444,475.57z\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\"/><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" Height=\"auto\" Data=\"F1M485.782,481.057L487.194,481.057L487.194,475.82L485.782,475.82z M487.694,481.557L485.282,481.557L485.282,475.32L487.694,475.32z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\"/></Grid><Grid Grid.Column=\"1\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"50*\"></RowDefinition><RowDefinition Height=\"50*\"></RowDefinition></Grid.RowDefinitions><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M491.15,481.307L489.237,481.307L489.237,473.657L491.15,473.657z\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\"/><Path Margin=\".3,0,.3,0\" Grid.Row=\"1\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M489.487,481.057L490.9,481.057L490.9,473.907L489.487,473.907z M491.4,481.557L488.987,481.557L488.987,473.407L491.4,473.407z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Left\" UseLayoutRounding=\"False\"/></Grid><Grid Grid.Column=\"2\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"25*\"></RowDefinition><RowDefinition Height=\"75*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M494.855,481.307L492.943,481.307L492.943,469.832L494.855,469.832z\" Fill=\"#FFCCCCCC\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M493.193,481.057L494.605,481.057L494.605,470.082L493.193,470.082z M495.105,481.557L492.693,481.557L492.693,469.582L495.105,469.582z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/></Grid><Grid Grid.Column=\"3\" Height=\"auto\"><Grid.RowDefinitions><RowDefinition Height=\"Auto\"></RowDefinition><RowDefinition Height=\"*\"></RowDefinition></Grid.RowDefinitions><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M498.562,481.307L496.649,481.307L496.649,466.007L498.562,466.007z\" Fill=\"#FFCCCCCC\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/><Path Grid.Row=\"1\" Margin=\".3,0,.3,0\" VerticalAlignment=\"Bottom\" Height=\"auto\" Data=\"F1M496.898,481.057L498.311,481.057L498.311,466.257L496.898,466.257z M498.812,481.557L496.399,481.557L496.399,465.757L498.812,465.757z\" Fill=\"#FF999999\" Stretch=\"Fill\" HorizontalAlignment=\"Right\" UseLayoutRounding=\"False\"/></Grid>";
                    break;
                case GaugeStateIndicatorStyles.StarQuartersAllFilled:
                    template +=
                        "<Path Data=\"F1M271.415,436.371L266.567,437.074L270.075,440.494L269.247,445.321L273.582,443.041L277.917,445.321L277.09,440.494L280.598,437.074L275.75,436.371L273.582,431.977z\" Stretch=\"Fill\" Margin=\"0.537,0.566,0.432,0.09\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M273.582,442.7588L277.585,444.8628L276.821,440.4068L280.06,437.2488L275.584,436.5998L273.582,432.5428L271.581,436.5998L267.104,437.2488L270.343,440.4068L269.579,444.8628z M268.915,445.7778L269.806,440.5808L266.03,436.8998L271.249,436.1428L273.582,431.4118L275.916,436.1428L281.134,436.8998L277.358,440.5808L278.249,445.7778L273.582,443.3228z\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,-0.104,-0.366\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.StarQuartersNoneFilled:
                    template +=
                        "<Path Data=\"F1M193.6597,431.9775L191.4917,436.3715L186.6447,437.0745L190.1527,440.4945L189.3237,445.3205L193.6597,443.0405L197.9947,445.3205L197.1677,440.4945L200.6757,437.0745L195.8277,436.3715z\" Fill=\"White\" Stretch=\"Fill\" Margin=\"0.537,0.566,0.432,0.091\" UseLayoutRounding=\"False\"/><Path Data=\"F1M193.6597,442.7588L197.6627,444.8628L196.8987,440.4068L200.1387,437.2488L195.6617,436.5998L193.6597,432.5428L191.6577,436.5998L187.1817,437.2488L190.4217,440.4068L189.6557,444.8628z M188.9917,445.7778L189.8847,440.5808L186.1077,436.8998L191.3257,436.1428L193.6597,431.4118L195.9937,436.1428L201.2127,436.8998L197.4357,440.5808L198.3267,445.7778L193.6597,443.3228z\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,-0.105,-0.366\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.StarQuartersOneFilled:
                    template +=
                        "<Path Data=\"F1M213.6411,431.9775L215.8081,436.3715L220.6561,437.0745L217.1491,440.4945L217.9761,445.3205L213.6411,443.0405L209.3061,445.3205L210.1341,440.4945L206.6261,437.0745L211.4731,436.3715z\" Fill=\"White\" Stretch=\"Fill\" Margin=\"0.666,0.566,0.304,1.091\" UseLayoutRounding=\"False\"/><Path Data=\"F1M213.6411,442.7588L217.6441,444.8628L216.8801,440.4068L220.1191,437.2488L215.6421,436.5998L213.6411,432.5428L211.6391,436.5998L207.1631,437.2488L210.4021,440.4068L209.6381,444.8628z M208.9741,445.7778L209.8651,440.5808L206.0891,436.8998L211.3071,436.1428L213.6411,431.4118L215.9741,436.1428L221.1931,436.8998L217.4171,440.5808L218.3081,445.7778L213.6411,443.3228z\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0.129,0,-0.233,0.634\" UseLayoutRounding=\"False\"/><Path Data=\"F1M211.223,436.371L206.501,437.074L210.071,440.494L209.063,445.531L212.084,443.864L212.073,434.969z\" Stretch=\"Fill\" Width=\"5.583\" HorizontalAlignment=\"Left\" Margin=\"0.541,3.558,0,0.88\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M207.042,437.2461L210.343,440.4081L209.414,445.0521L211.834,443.7161L211.825,435.8611L211.376,436.6011z M208.711,446.0111L209.799,440.5801L205.96,436.9021L211.07,436.1411L212.323,434.0761L212.334,444.0111z\" Fill=\"#FFFBB03B\" Stretch=\"Fill\" Width=\"6.374\" HorizontalAlignment=\"Left\" Margin=\"0,2.665,0,0.4\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.StarQuartersThreeFilled:
                    template +=
                        "<Path Data=\"F1M253.6021,431.9775L255.7691,436.3715L260.6171,437.0745L257.1101,440.4945L257.9371,445.3205L253.6021,443.0405L249.2671,445.3205L250.0951,440.4945L246.5871,437.0745L251.4341,436.3715z\" Fill=\"White\" Stretch=\"Fill\" Margin=\"0.538,0.566,0.432,0.091\" UseLayoutRounding=\"False\"/><Path Data=\"F1M253.6021,442.7588L257.6051,444.8628L256.8411,440.4068L260.0801,437.2488L255.6031,436.5998L253.6021,432.5428L251.6001,436.5998L247.1241,437.2488L250.3631,440.4068L249.5991,444.8628z M248.9351,445.7778L249.8261,440.5808L246.0501,436.8998L251.2681,436.1428L253.6021,431.4118L255.9351,436.1428L261.1541,436.8998L257.3781,440.5808L258.2691,445.7778L253.6021,443.3228z\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0.001,0,-0.105,-0.366\" UseLayoutRounding=\"False\"/><Path Data=\"F1M251.402,436.371L246.587,437.074L250.11,440.494L249.29,445.321L253.49,443.099L255.697,444.271L255.697,436.224L253.602,431.977z\" Stretch=\"Fill\" Margin=\"0.538,0.566,5.352,0.09\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M247.125,437.248L250.378,440.406L249.622,444.862L253.49,442.816L255.447,443.855L255.447,436.283L253.601,432.539L251.568,436.6z M248.958,445.778L249.841,440.582L246.049,436.9L251.238,436.143L253.604,431.416L255.947,436.225L255.947,444.687L253.49,443.383z\" Fill=\"#FFFBB03B\" Stretch=\"Fill\" Margin=\"0,0.005,5.102,-0.367\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.StarQuartersTwoFilled:
                    template +=
                        "<Path Data=\"F1M343.1782,431.9775L345.3452,436.3715L350.1932,437.0745L346.6862,440.4945L347.5132,445.3205L343.1782,443.0405L338.8432,445.3205L339.6712,440.4945L336.1632,437.0745L341.0102,436.3715z\" Fill=\"White\" Stretch=\"Fill\" Margin=\"0.538,0.566,0.432,0.091\" UseLayoutRounding=\"False\"/><Path Data=\"F1M343.1782,442.7588L347.1812,444.8628L346.4172,440.4068L349.6562,437.2488L345.1792,436.5998L343.1782,432.5428L341.1762,436.5998L336.7002,437.2488L339.9392,440.4068L339.1752,444.8628z M338.5112,445.7778L339.4022,440.5808L335.6262,436.8998L340.8442,436.1428L343.1782,431.4118L345.5112,436.1428L350.7302,436.8998L346.9542,440.5808L347.8452,445.7778L343.1782,443.3228z\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,-0.104,-0.366\" UseLayoutRounding=\"False\"/><Path Data=\"F1M341.01,436.371L336.163,437.074L339.671,440.494L338.843,445.321L342.835,443.222L342.904,432.532z\" Stretch=\"Fill\" Width=\"6.741\" HorizontalAlignment=\"Left\" Margin=\"0.537,1.121,0,0.09\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M336.7002,437.249L339.9392,440.407L339.1752,444.863L342.5862,443.07L342.6482,433.617L341.1762,436.6z M338.5112,445.777L339.4022,440.581L335.6262,436.899L340.8442,436.143L343.1612,431.445L343.0842,443.373z\" Fill=\"#FFFBB03B\" Stretch=\"Fill\" Margin=\"0,0.034,7.465,-0.366\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.ThreeSignsCircle:
                    template +=
                       "<Path Data=\"F1M452.016,258.239C452.016,262.2,455.227,265.411,459.187,265.411L459.187,265.411C463.149,265.411,466.36,262.2,466.36,258.239L466.36,258.239C466.36,254.279,463.149,251.067,459.187,251.067L459.187,251.067C455.227,251.067,452.016,254.279,452.016,258.239\" Stretch=\"Fill\" Margin=\"0.375,0.376,0.281,0.28\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M459.1875,251.4419C455.4395,251.4419,452.3905,254.4909,452.3905,258.2389C452.3905,261.9869,455.4395,265.0359,459.1875,265.0359C462.9355,265.0359,465.9855,261.9869,465.9855,258.2389C465.9855,254.4909,462.9355,251.4419,459.1875,251.4419 M459.1875,265.7859C455.0265,265.7859,451.6405,262.3999,451.6405,258.2389C451.6405,254.0779,455.0265,250.6919,459.1875,250.6919C463.3495,250.6919,466.7355,254.0779,466.7355,258.2389C466.7355,262.3999,463.3495,265.7859,459.1875,265.7859\" Fill=\"#FF006837\" Stretch=\"Fill\" Margin=\"0,0,-0.095,-0.094\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.ThreeSignsDiamond:
                    template +=
                        "<Path Data=\"F1M351.916,256.019L359.104,263.159L366.293,256.019L359.104,248.879z\" Stretch=\"Fill\" Margin=\"0.531,0.529,0.092,0.191\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M352.4487,256.019L359.1047,262.631L365.7607,256.019L359.1047,249.407z M359.1047,263.688L351.3847,256.019L359.1047,248.35L366.8247,256.019z\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,-0.44,-0.338\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.ThreeSignsTriangle:
                    template +=
                        "<Path Data=\"F1M328.831,262.234L343.795,262.234L336.313,249.275z\" Stretch=\"Fill\" Margin=\"0.649,0.75,0.387,0.291\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M329.4805,261.8599L343.1455,261.8599L336.3135,250.0259z M344.4445,262.6099L328.1815,262.6099L336.3135,248.5259z\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,-0.263,-0.084\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.ThreeSymbolCheck:
                    template +=
                        "<Path Data=\"F1M312.862,360.946C312.862,364.915,316.081,368.134,320.051,368.134L320.051,368.134C324.021,368.134,327.24,364.915,327.24,360.946L327.24,360.946C327.24,356.976,324.021,353.756,320.051,353.756L320.051,353.756C316.081,353.756,312.862,356.976,312.862,360.946\" Stretch=\"Fill\" Margin=\"0.359,0.359,0.263,0.263\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M320.0508,354.1162C316.2848,354.1162,313.2208,357.1792,313.2208,360.9452C313.2208,364.7112,316.2848,367.7742,320.0508,367.7742C323.8168,367.7742,326.8808,364.7112,326.8808,360.9452C326.8808,357.1792,323.8168,354.1162,320.0508,354.1162 M320.0508,368.4932C315.8888,368.4932,312.5028,365.1072,312.5028,360.9452C312.5028,356.7832,315.8888,353.3972,320.0508,353.3972C324.2128,353.3972,327.5988,356.7832,327.5988,360.9452C327.5988,365.1072,324.2128,368.4932,320.0508,368.4932\" Fill=\"#FF006837\" Stretch=\"Fill\" Margin=\"0,0,-0.096,-0.096\" UseLayoutRounding=\"False\"/><Path Data=\"F1M324.1587,357.3896L323.3837,356.8626C323.1717,356.7196,322.8787,356.7736,322.7337,356.9866L318.9417,362.5796L317.1977,360.8366C317.0177,360.6556,316.7197,360.6556,316.5357,360.8366L315.8747,361.4986C315.6917,361.6816,315.6917,361.9776,315.8747,362.1606L318.5547,364.8416C318.7047,364.9926,318.9407,365.1086,319.1537,365.1086C319.3657,365.1086,319.5797,364.9746,319.7177,364.7736L324.2837,358.0386C324.4277,357.8276,324.3717,357.5346,324.1587,357.3896\" Fill=\"White\" Stretch=\"Fill\" Margin=\"3.235,3.385,3.139,3.289\" UseLayoutRounding=\"False\"/><Path Data=\"F1M316.8682,360.9404C316.8062,360.9404,316.7482,360.9634,316.7052,361.0064L316.0442,361.6684C316.0012,361.7114,315.9772,361.7684,315.9772,361.8304C315.9772,361.8904,316.0012,361.9484,316.0442,361.9924L318.7242,364.6724C318.9452,364.8934,319.3192,364.9314,319.5202,364.6374L324.0852,357.9044C324.1192,357.8544,324.1312,357.7944,324.1192,357.7344C324.1082,357.6734,324.0742,357.6224,324.0242,357.5874L323.2492,357.0604C323.1502,356.9954,322.9992,357.0224,322.9322,357.1214L318.9772,362.9544L317.0282,361.0064C316.9852,360.9634,316.9292,360.9404,316.8682,360.9404 M319.1532,365.3474C318.8902,365.3474,318.5892,365.2154,318.3852,365.0104L315.7052,362.3304C315.5722,362.1964,315.4982,362.0184,315.4982,361.8304C315.4982,361.6404,315.5722,361.4624,315.7052,361.3294L316.3672,360.6684C316.6362,360.3994,317.1032,360.4034,317.3682,360.6684L318.9062,362.2064L322.5352,356.8514C322.7462,356.5424,323.2052,356.4554,323.5172,356.6634L324.2932,357.1914C324.4492,357.2964,324.5552,357.4584,324.5912,357.6444C324.6262,357.8304,324.5872,358.0184,324.4812,358.1734L319.9162,364.9084C319.7292,365.1794,319.4372,365.3474,319.1532,365.3474\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"2.996,3.153,2.899,3.05\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.ThreeSymbolCross:
                    template +=
                        "<Path Data=\"F1M359.103,360.946C359.103,364.915,362.322,368.134,366.292,368.134L366.292,368.134C370.262,368.134,373.481,364.915,373.481,360.946L373.481,360.946C373.481,356.976,370.262,353.756,366.292,353.756L366.292,353.756C362.322,353.756,359.103,356.976,359.103,360.946\" Stretch=\"Fill\" Margin=\"0.36,0.359,0.262,0.263\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M366.292,354.1162C362.526,354.1162,359.462,357.1792,359.462,360.9452C359.462,364.7112,362.526,367.7742,366.292,367.7742C370.058,367.7742,373.122,364.7112,373.122,360.9452C373.122,357.1792,370.058,354.1162,366.292,354.1162 M366.292,368.4932C362.129,368.4932,358.743,365.1072,358.743,360.9452C358.743,356.7832,362.129,353.3972,366.292,353.3972C370.455,353.3972,373.841,356.7832,373.841,360.9452C373.841,365.1072,370.455,368.4932,366.292,368.4932\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,-0.098,-0.096\" UseLayoutRounding=\"False\"/><Path Data=\"F1M368.0479,360.9443L369.9789,359.0133C370.1739,358.8203,370.1739,358.5043,369.9789,358.3113L368.9259,357.2573C368.7329,357.0623,368.4159,357.0623,368.2229,357.2573L366.2909,359.1893L364.3599,357.2573C364.1669,357.0623,363.8499,357.0623,363.6569,357.2573L362.6049,358.3113C362.4089,358.5043,362.4089,358.8203,362.6049,359.0133L364.5349,360.9443L362.6049,362.8773C362.4089,363.0693,362.4089,363.3843,362.6049,363.5783L363.6569,364.6323C363.8499,364.8273,364.1659,364.8273,364.3599,364.6323L366.2909,362.7003L368.2239,364.6323C368.4159,364.8273,368.7329,364.8273,368.9259,364.6323L369.9789,363.5783C370.1739,363.3843,370.1739,363.0693,369.9789,362.8773z\" Fill=\"White\" Stretch=\"Fill\" Margin=\"3.715,3.714,3.618,3.618\" UseLayoutRounding=\"False\"/><Path Data=\"F1M366.291,362.3613L368.393,364.4633C368.49,364.5623,368.659,364.5603,368.756,364.4643L369.809,363.4093C369.859,363.3593,369.885,363.2953,369.885,363.2263C369.885,363.1593,369.859,363.0953,369.811,363.0483L367.709,360.9443L369.809,358.8433C369.859,358.7953,369.885,358.7303,369.885,358.6623C369.885,358.5933,369.859,358.5303,369.811,358.4823L368.756,357.4263C368.659,357.3293,368.489,357.3293,368.393,357.4263L366.291,359.5283L364.19,357.4273C364.092,357.3283,363.923,357.3303,363.828,357.4253L362.774,358.4803C362.724,358.5303,362.698,358.5933,362.698,358.6623C362.698,358.7303,362.724,358.7953,362.773,358.8433L364.874,360.9443L362.774,363.0463C362.724,363.0953,362.698,363.1593,362.698,363.2263C362.698,363.2953,362.724,363.3593,362.773,363.4083L363.827,364.4643C363.923,364.5613,364.093,364.5603,364.189,364.4643z M368.574,365.0183C368.377,365.0183,368.192,364.9413,368.053,364.8013L366.291,363.0393L364.529,364.8013C364.252,365.0813,363.764,365.0813,363.487,364.8013L362.435,363.7473C362.295,363.6093,362.218,363.4243,362.218,363.2263C362.219,363.0293,362.296,362.8443,362.437,362.7063L364.196,360.9443L362.435,359.1833C362.295,359.0453,362.218,358.8593,362.218,358.6613C362.219,358.4643,362.296,358.2793,362.437,358.1403L363.488,357.0893C363.762,356.8093,364.253,356.8083,364.53,357.0903L366.291,358.8503L368.053,357.0883C368.33,356.8103,368.818,356.8093,369.096,357.0893L370.148,358.1423C370.287,358.2793,370.364,358.4643,370.365,358.6613C370.365,358.8593,370.288,359.0453,370.147,359.1833L368.387,360.9443L370.148,362.7083C370.287,362.8443,370.364,363.0293,370.365,363.2263C370.365,363.4243,370.288,363.6093,370.147,363.7483L369.095,364.8013C368.957,364.9413,368.771,365.0183,368.574,365.0183\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"3.475,3.482,3.378,3.379\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.ThreeSymbolExclamation:
                    template +=
                        "<Path Data=\"F1M335.983,360.946C335.983,364.915,339.201,368.134,343.172,368.134L343.172,368.134C347.142,368.134,350.36,364.915,350.36,360.946L350.36,360.946C350.36,356.976,347.142,353.756,343.172,353.756L343.172,353.756C339.201,353.756,335.983,356.976,335.983,360.946\" Stretch=\"Fill\" Margin=\"0.36,0.359,0.263,0.263\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M343.1719,354.1162C339.4059,354.1162,336.3419,357.1792,336.3419,360.9452C336.3419,364.7112,339.4059,367.7742,343.1719,367.7742C346.9379,367.7742,350.0009,364.7112,350.0009,360.9452C350.0009,357.1792,346.9379,354.1162,343.1719,354.1162 M343.1719,368.4932C339.0089,368.4932,335.6229,365.1072,335.6229,360.9452C335.6229,356.7832,339.0089,353.3972,343.1719,353.3972C347.3339,353.3972,350.7199,356.7832,350.7199,360.9452C350.7199,365.1072,347.3339,368.4932,343.1719,368.4932\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,-0.097,-0.096\" UseLayoutRounding=\"False\"/><Path Data=\"F1M342.5928,362.6016L343.7198,362.6016L344.0018,362.6016L344.1258,355.8916L342.1438,355.8916L342.3128,362.6016z\" Fill=\"White\" Stretch=\"Fill\" Margin=\"6.521,2.494,6.497,5.796\" UseLayoutRounding=\"False\"/><Path Data=\"F1M342.5464,362.3623L343.7664,362.3623L343.8814,356.1313L342.3894,356.1313z M344.2374,362.8413L342.0784,362.8413L341.8984,355.6523L344.3704,355.6523z\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"6.276,2.255,6.252,5.556\" UseLayoutRounding=\"False\"/><Path Data=\"F1M343.9673,363.6914C343.7443,363.4814,343.4773,363.3754,343.1743,363.3754C342.8613,363.3754,342.5913,363.4824,342.3713,363.6924C342.1463,363.9064,342.0323,364.1704,342.0323,364.4764C342.0323,364.7724,342.1503,365.0374,342.3753,365.2444C342.5943,365.4444,342.8633,365.5454,343.1743,365.5454C343.4763,365.5454,343.7413,365.4454,343.9633,365.2444C344.1903,365.0384,344.3103,364.7734,344.3103,364.4764C344.3103,364.1704,344.1953,363.9054,343.9673,363.6914\" Fill=\"White\" Height=\"2.17\" Stretch=\"Fill\" Margin=\"6.41,0,6.312,2.852\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\"/><Path Data=\"F1M343.1738,363.6143C342.9228,363.6143,342.7138,363.6973,342.5368,363.8663C342.3588,364.0353,342.2718,364.2343,342.2718,364.4763C342.2718,364.7073,342.3608,364.9053,342.5378,365.0683C342.8818,365.3823,343.4448,365.3913,343.8028,365.0663C343.9808,364.9053,344.0708,364.7063,344.0708,364.4763C344.0708,364.2343,343.9838,364.0353,343.8028,363.8663C343.6238,363.6963,343.4178,363.6143,343.1738,363.6143 M343.1738,365.7853C342.8018,365.7853,342.4788,365.6633,342.2138,365.4213C341.9378,365.1673,341.7928,364.8413,341.7928,364.4763C341.7928,364.1013,341.9318,363.7793,342.2058,363.5183C342.7338,363.0163,343.5898,363.0053,344.1318,363.5163C344.4048,363.7733,344.5498,364.1053,344.5498,364.4763C344.5498,364.8423,344.4028,365.1693,344.1248,365.4223C343.8578,365.6633,343.5378,365.7853,343.1738,365.7853\" Fill=\"#FF333333\" Height=\"2.648\" Stretch=\"Fill\" Margin=\"6.17,0,6.073,2.612\" UseLayoutRounding=\"False\" VerticalAlignment=\"Bottom\"/>";
                    break;
                case GaugeStateIndicatorStyles.ThreeSymbolUnCircledCheck:
                    template +=
                        "<Path Data=\"F1M476.8516,352.2256L475.6496,351.4026C475.3206,351.1796,474.8656,351.2646,474.6416,351.5956L468.7556,360.3296L466.0496,357.6076C465.7706,357.3256,465.3086,357.3256,465.0246,357.6076L463.9976,358.6426C463.7146,358.9276,463.7146,359.3906,463.9976,359.6776L468.1576,363.8636C468.3886,364.0976,468.7546,364.2786,469.0846,364.2786C469.4146,364.2786,469.7476,364.0686,469.9606,363.7556L477.0446,353.2406C477.2696,352.9086,477.1826,352.4506,476.8516,352.2256\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" Margin=\"0.249,0.242,0.367,-0.242\" UseLayoutRounding=\"False\"/><Path Data=\"F1M465.5391,357.6455C465.4111,357.6455,465.2911,357.6955,465.2001,357.7855L464.1761,358.8185C463.9891,359.0065,463.9891,359.3125,464.1761,359.5015L468.3351,363.6875C468.5331,363.8885,468.8421,364.0285,469.0851,364.0285C469.3191,364.0285,469.5831,363.8665,469.7551,363.6145L476.8381,353.1005C476.9841,352.8835,476.9261,352.5785,476.7111,352.4325L475.5081,351.6085C475.3011,351.4695,474.9891,351.5295,474.8491,351.7355L468.7931,360.7225L465.8721,357.7835C465.7841,357.6945,465.6661,357.6455,465.5391,357.6455 M469.0851,364.5285C468.7071,364.5285,468.2721,364.3355,467.9791,364.0395L463.8201,359.8535C463.4411,359.4695,463.4421,358.8475,463.8201,358.4665L464.8471,357.4315C465.2201,357.0615,465.8621,357.0635,466.2271,357.4315L468.7191,359.9375L474.4351,351.4565C474.7231,351.0275,475.3571,350.9025,475.7891,351.1955L476.9931,352.0195C477.4381,352.3215,477.5551,352.9325,477.2521,353.3795L470.1681,363.8955C469.9001,364.2865,469.4861,364.5285,469.0851,364.5285\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,0.116,-0.492\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.ThreeSymbolUnCircledCross:
                    template +=
                        "<Path Data=\"F1M518.3691,356.8633L521.4231,353.7153C521.7311,353.4013,521.7311,352.8843,521.4231,352.5703L519.7581,350.8513C519.4521,350.5343,518.9531,350.5343,518.6471,350.8513L515.5961,354.0013L512.5421,350.8513C512.2381,350.5343,511.7371,350.5343,511.4331,350.8513L509.7681,352.5703C509.4601,352.8843,509.4601,353.3993,509.7681,353.7153L512.8201,356.8633L509.7681,360.0133C509.4601,360.3263,509.4601,360.8423,509.7681,361.1573L511.4331,362.8763C511.7371,363.1933,512.2371,363.1933,512.5421,362.8763L515.5961,359.7263L518.6481,362.8763C518.9531,363.1933,519.4521,363.1933,519.7581,362.8763L521.4231,361.1573C521.7311,360.8423,521.7311,360.3263,521.4231,360.0133z\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" Margin=\"0.25,0.239,0.633,0.261\" UseLayoutRounding=\"False\"/><Path Data=\"F1M515.5957,359.3672L518.8277,362.7022C519.0297,362.9112,519.3767,362.9112,519.5777,362.7022L521.2427,360.9832C521.4557,360.7662,521.4557,360.4032,521.2447,360.1892L518.0207,356.8632L521.2427,353.5412C521.4557,353.3242,521.4557,352.9612,521.2437,352.7452L519.5777,351.0252C519.3767,350.8152,519.0287,350.8172,518.8267,351.0252L515.5957,354.3602L512.3627,351.0252C512.1597,350.8152,511.8127,350.8182,511.6147,351.0242L509.9487,352.7442C509.7327,352.9642,509.7327,353.3202,509.9477,353.5402L513.1677,356.8632L509.9487,360.1872C509.7357,360.4032,509.7357,360.7662,509.9477,360.9822L511.6137,362.7022C511.8147,362.9112,512.1617,362.9102,512.3617,362.7032z M519.2027,363.3642C518.9237,363.3642,518.6627,363.2522,518.4677,363.0492L515.5957,360.0862L512.7217,363.0502C512.3327,363.4542,511.6417,363.4552,511.2527,363.0492L509.5887,361.3312C509.1857,360.9192,509.1857,360.2482,509.5907,359.8382L512.4727,356.8632L509.5887,353.8882C509.1867,353.4772,509.1867,352.8062,509.5897,352.3952L511.2537,350.6782C511.6397,350.2742,512.3327,350.2712,512.7227,350.6792L515.5957,353.6412L518.4677,350.6782C518.8587,350.2742,519.5467,350.2722,519.9377,350.6782L521.6027,352.3962C522.0057,352.8072,522.0057,353.4782,521.6007,353.8902L518.7177,356.8632L521.6027,359.8402C522.0057,360.2482,522.0057,360.9192,521.6017,361.3322L519.9377,363.0502C519.7417,363.2522,519.4817,363.3642,519.2027,363.3642\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,0.382,0.01\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.ThreeSymbolUnCircledExclamation:
                    template +=
                        "<Path Data=\"F1M494.3623,350.1133L494.1793,359.5103L492.3573,359.5103L492.1073,350.1133z M493.2973,363.6133C492.9153,363.6133,492.5873,363.4943,492.3193,363.2573C492.0503,363.0173,491.9163,362.7283,491.9163,362.3883C491.9163,362.0363,492.0503,361.7353,492.3193,361.4873C492.5873,361.2403,492.9153,361.1163,493.2973,361.1163C493.6683,361.1163,493.9893,361.2403,494.2603,361.4873C494.5333,361.7353,494.6693,362.0363,494.6693,362.3883C494.6693,362.7283,494.5333,363.0173,494.2603,363.2573C493.9893,363.4943,493.6683,363.6133,493.2973,363.6133\" Fill=\"" + fillColor + "\" Stretch=\"Fill\" Margin=\"0.251,0.25,-0.004,0.25\" UseLayoutRounding=\"False\"/><Path Data=\"F1M492.6006,359.2607L493.9346,359.2607L494.1076,350.3637L492.3646,350.3637z M494.4246,359.7607L492.1146,359.7607L491.8506,349.8637L494.6176,349.8637z M493.2966,361.3657C492.9756,361.3657,492.7106,361.4657,492.4896,361.6707C492.2716,361.8717,492.1656,362.1067,492.1656,362.3887C492.1656,362.6557,492.2706,362.8787,492.4856,363.0707C492.9216,363.4567,493.6316,363.4727,494.0966,363.0687C494.3136,362.8777,494.4186,362.6557,494.4186,362.3887C494.4186,362.1067,494.3126,361.8717,494.0926,361.6717C493.8666,361.4657,493.6066,361.3657,493.2966,361.3657 M493.2966,363.8637C492.8516,363.8637,492.4666,363.7227,492.1536,363.4447C491.8296,363.1557,491.6656,362.8007,491.6656,362.3887C491.6656,361.9647,491.8296,361.5997,492.1496,361.3037C492.4636,361.0137,492.8496,360.8657,493.2966,360.8657C493.7306,360.8657,494.1116,361.0127,494.4286,361.3027C494.7536,361.5987,494.9186,361.9637,494.9186,362.3887C494.9186,362.8017,494.7526,363.1567,494.4256,363.4447C494.1076,363.7227,493.7286,363.8637,493.2966,363.8637\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,-0.253,0\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.TrafficLight:
                    template +=
                        "<Path Data=\"F1M455.832,223.468C454.319,223.468,453.091,224.695,453.091,226.208L453.091,226.208L453.091,235.342C453.091,236.856,454.319,238.084,455.832,238.084L455.832,238.084L464.967,238.084C466.481,238.084,467.708,236.856,467.708,235.342L467.708,235.342L467.708,226.208C467.708,224.695,466.481,223.468,464.967,223.468L464.967,223.468z\" Stretch=\"Fill\" Margin=\"0.342,0.344,0.041,0.04\" UseLayoutRounding=\"False\" Fill=\"Black\"/><Path Data=\"F1M455.832,223.8105C454.51,223.8105,453.435,224.8855,453.435,226.2075L453.435,235.3425C453.435,236.6645,454.51,237.7405,455.832,237.7405L464.967,237.7405C466.289,237.7405,467.365,236.6645,467.365,235.3425L467.365,226.2075C467.365,224.8855,466.289,223.8105,464.967,223.8105z M464.967,238.4265L455.832,238.4265C454.132,238.4265,452.749,237.0425,452.749,235.3425L452.749,226.2075C452.749,224.5085,454.132,223.1245,455.832,223.1245L464.967,223.1245C466.667,223.1245,468.051,224.5085,468.051,226.2075L468.051,235.3425C468.051,237.0425,466.667,238.4265,464.967,238.4265\" Fill=\"#FF4D4D4D\" Stretch=\"Fill\" Margin=\"0,0,-0.302,-0.302\" UseLayoutRounding=\"False\"/><Path Data=\"F1M455.784,230.775C455.784,233.324,457.852,235.39,460.4,235.39L460.4,235.39C462.948,235.39,465.016,233.324,465.016,230.775L465.016,230.775C465.016,228.226,462.948,226.16,460.4,226.16L460.4,226.16C457.852,226.16,455.784,228.226,455.784,230.775\" Stretch=\"Fill\" Margin=\"3.035,3.036,2.733,2.734\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M455.7842,230.7754C455.7842,228.2264,457.8512,226.1604,460.3992,226.1604C462.9482,226.1604,465.0152,228.2264,465.0152,230.7754C465.0152,233.3244,462.9482,235.3904,460.3992,235.3904C457.8512,235.3904,455.7842,233.3244,455.7842,230.7754\" Fill=\"#FF353535\" Opacity=\"0.25\" Stretch=\"Fill\" Margin=\"3.036,3.036,2.733,2.734\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.TrafficLightUnrimmed:
                    template +=
                        "<Path Data=\"F1M452.016,258.239C452.016,262.2,455.227,265.411,459.187,265.411L459.187,265.411C463.149,265.411,466.36,262.2,466.36,258.239L466.36,258.239C466.36,254.279,463.149,251.067,459.187,251.067L459.187,251.067C455.227,251.067,452.016,254.279,452.016,258.239\" Stretch=\"Fill\" Margin=\"0.375,0.376,0.281,0.28\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M459.1875,251.4419C455.4395,251.4419,452.3905,254.4909,452.3905,258.2389C452.3905,261.9869,455.4395,265.0359,459.1875,265.0359C462.9355,265.0359,465.9855,261.9869,465.9855,258.2389C465.9855,254.4909,462.9355,251.4419,459.1875,251.4419 M459.1875,265.7859C455.0265,265.7859,451.6405,262.3999,451.6405,258.2389C451.6405,254.0779,455.0265,250.6919,459.1875,250.6919C463.3495,250.6919,466.7355,254.0779,466.7355,258.2389C466.7355,262.3999,463.3495,265.7859,459.1875,265.7859\" Fill=\"#FF006837\" Stretch=\"Fill\" Margin=\"0,0,-0.095,-0.094\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.TriangleDash:
                    template +=
                        "<Path Data=\"M339.98,113.563L326.501,113.563L326.501,110.674L339.98,110.674z\" Stretch=\"Fill\" Margin=\"0.375,0.375,0.146,0.736\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M326.876,113.188L339.605,113.188L339.605,111.049L326.876,111.049z M340.355,113.938L326.126,113.938L326.126,110.299L340.355,110.299z\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,-0.229,0.361\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.TriangleDown:
                    template +=
                        "<Path Data=\"F1M367.062,108.788L360.605,115.157L354.148,108.788z\" Stretch=\"Fill\" Margin=\"0.913,0.375,1.173,0.256\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M355.0625,109.1631L360.6055,114.6301L366.1475,109.1631z M360.6055,115.6841L353.2345,108.4131L367.9755,108.4131z\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,0.259,-0.271\" UseLayoutRounding=\"False\"/>";
                    break;
                case GaugeStateIndicatorStyles.TriangleUp:
                    template +=
                        "<Path Data=\"F1M299.896,115.304L306.352,108.935L312.809,115.304z\" Stretch=\"Fill\" Margin=\"0.914,0.528,1.173,0.103\" UseLayoutRounding=\"False\" Fill=\"" + fillColor + "\"/><Path Data=\"F1M300.8096,114.9287L311.8946,114.9287L306.3516,109.4617z M313.7226,115.6787L298.9816,115.6787L306.3516,108.4077z\" Fill=\"#FF333333\" Stretch=\"Fill\" Margin=\"0,0,0.259,-0.271\" UseLayoutRounding=\"False\"/>";
                    break;
            }

            template += "</Grid>";
            vBox.Child = XamlReader.Load(template) as Grid;
            return vBox;
        }
    }
}
