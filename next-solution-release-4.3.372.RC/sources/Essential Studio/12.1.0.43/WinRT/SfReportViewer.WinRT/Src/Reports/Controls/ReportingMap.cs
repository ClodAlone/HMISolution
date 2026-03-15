#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Syncfusion.RDL.DOM;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.ItemModel;
using Syncfusion.UI.Xaml.Gauges;
using System;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.UI.Xaml.Maps;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;


namespace Syncfusion.UI.Xaml.Reports.Reports.Controls
{
    internal class ReportingMap : ContentControl
    {
        Thickness viewPosition;
        private Syncfusion.RDL.DOM.Action action;
        private bool isMouseEvent = false;

        internal MapModel MapModel
        {
            get;
            set;
        }

        internal Canvas MapControlPanel
        {
            get;
            set;
        }

        public ReportingMap(IReportItemModeler pageContent)
        {
            this.MapControlPanel = new Canvas();
            this.MapModel = pageContent as MapModel;
            this.MapControlPanel.Background = new ReportingBrushConverter().ConvertFromInvariantString("Transparent");
            this.Content = this.MapControlPanel;
            this.Name = pageContent.Name;

            Canvas.SetTop(MapControlPanel, 0);
            Canvas.SetLeft(MapControlPanel, 0);

            this.MapModel.Height = this.MapControlPanel.Height = this.Height = pageContent.Height;
            this.MapModel.Width = this.MapControlPanel.Width = this.Width = pageContent.Width;
            RenderMapControl();

            if (this.MapModel.MapProperties != null && this.MapModel.MapProperties.Background != null)
            {
                this.MapControlPanel.Background = new ReportingBrushConverter().ConvertFromInvariantString(this.MapModel.MapProperties.Background.BackgroundColor);
            }
            if (this.MapModel.MapProperties != null && this.MapModel.MapProperties.Border != null && this.MapModel.MapProperties.Border.Default != null)
            {
                double left;
                double right;
                double top;
                double bottom;
                left = right = top = bottom = this.MapModel.MapProperties.Border.Default.Thickness;

                if (this.MapModel.MapProperties.Border.LeftBorder != null)
                {
                    left = this.MapModel.MapProperties.Border.LeftBorder.Thickness;
                }
                if (this.MapModel.MapProperties.Border.RightBorder != null)
                {
                    right = this.MapModel.MapProperties.Border.RightBorder.Thickness;
                }
                if (this.MapModel.MapProperties.Border.TopBorder != null)
                {
                    top = this.MapModel.MapProperties.Border.TopBorder.Thickness;
                }
                if (this.MapModel.MapProperties.Border.BottomBorder != null)
                {
                    bottom = this.MapModel.MapProperties.Border.BottomBorder.Thickness;
                }

                this.BorderThickness = new Thickness(left, top, right, bottom);
                this.BorderBrush = new ReportingBrushConverter().ConvertFromInvariantString(this.MapModel.MapProperties.Border.Default.BorderBrush);

                if (this.MapModel.MapProperties.Border.Default.BorderStyle == Syncfusion.RDL.DOM.BorderStyles.None || this.MapModel.MapProperties.Border.Default.BorderStyle == Syncfusion.RDL.DOM.BorderStyles.Default)
                {
                    this.BorderThickness = new Thickness(0);
                }
            }
            else
            {
                this.BorderThickness = new Thickness(0);
            }
            if (pageContent.FlowLayoutInfo != null)
            {
                this.Margin = new Thickness(pageContent.FlowLayoutInfo.ActualLeft, pageContent.FlowLayoutInfo.ActualTop, 0, 0);
            }
            else if (pageContent.IsTablixChild)
            {
                this.Margin = new Thickness(pageContent.Left, pageContent.Top, 0, 0);
            }
        }

        private void RenderMapControl()
        {
            Syncfusion.UI.Xaml.Maps.SfMap sfMapControl = new Syncfusion.UI.Xaml.Maps.SfMap();
            ShapeFileLayer shapelayer = new ShapeFileLayer();

            if (this.MapModel.MapEngineInfo.MapShapeActions != null &&
                (!string.IsNullOrEmpty(this.MapModel.MapEngineInfo.MapShapeActions.BookmarkLink) ||
                 !string.IsNullOrEmpty(this.MapModel.MapEngineInfo.MapShapeActions.Hyperlink) ||
                 !string.IsNullOrEmpty(this.MapModel.MapEngineInfo.MapShapeActions.ReportName)))
            {
                sfMapControl.PointerMoved += sfMapControl_PointerMoved;
            }
            Windows.UI.Xaml.Controls.Border viewPortBorder = new Windows.UI.Xaml.Controls.Border();
            ReportingBrushConverter brushConverter = new ReportingBrushConverter();
            ReportingFontStyleConverter fontConverter = new ReportingFontStyleConverter();
            ReportingFontWeightConverter fontWeightConverter = new ReportingFontWeightConverter();
            viewPosition = new Thickness(0);
            Thickness margin = new Thickness(0);

            if (this.MapModel.MapProperties != null)
            {
                if (this.MapModel.MapProperties.MapTitles != null)
                {
                    foreach (var title in this.MapModel.MapProperties.MapTitles)
                    {
                        TextBlock mapTitle = new TextBlock();
                        Windows.UI.Xaml.Controls.Border titleBorder = new Windows.UI.Xaml.Controls.Border();
                        mapTitle.VerticalAlignment = VerticalAlignment.Center;
                        mapTitle.Text = title.Text;
                        ToolTipService.SetToolTip(mapTitle, title.ToolTip);

                        //if (title.Border != null && title.Border.BorderStyle!= BorderStyles.None)
                        //{
                        //    titleBorder.BorderBrush = brushConverter.ConvertFromInvariantString(title.Border.BorderBrush);
                        //    titleBorder.BorderThickness = new Thickness(title.Border.Thickness);
                        //}
                        if (title.Style != null)
                        {
                            if (title.Style.Font != null)
                            {
                                mapTitle.FontFamily = new FontFamily(title.Style.Font.FontFamily);
                                mapTitle.FontSize = title.Style.Font.FontSize;
                                mapTitle.TextAlignment = TextAlignment.Center;

                                if (title.Style.Font.FontStyle != Syncfusion.RDL.DOM.FontStyle.Default)
                                {
                                    mapTitle.FontStyle = new ReportingFontStyleConverter().ConvertFromInvariantString(title.Style.Font.FontStyle.ToString());
                                }
                                if (title.Style.Font.FontWeight != Syncfusion.RDL.DOM.FontWeight.Default)
                                {
                                    mapTitle.FontWeight = new ReportingFontWeightConverter().ConvertFromInvariantString(title.Style.Font.FontWeight.ToString());
                                }
                            }
                            if (!string.IsNullOrEmpty(title.Style.TextColor))
                            {
                                mapTitle.Foreground = brushConverter.ConvertFromInvariantString(title.Style.TextColor);
                            }
                        }
                        if (title.BackGroundExpVal != null)
                        {
                        }
                        if (title.BottomMargin != null)
                        {
                            margin.Bottom = title.BottomMargin.FloatValue;
                            viewPosition.Top += margin.Bottom;
                        }
                        if (title.TopMargin != null)
                        {
                            margin.Top = title.TopMargin.FloatValue;
                            viewPosition.Top += margin.Top;
                        }
                        if (title.LeftMargin != null)
                        {
                            margin.Left = title.LeftMargin.FloatValue;
                            //viewPosition.Left += margin.Left;
                        }
                        if (title.RightMargin != null)
                        {
                            margin.Right = title.RightMargin.FloatValue;
                            //viewPosition.Left += margin.Right;
                        }
                        if (title.MapLocation != null)
                        {
                            Canvas.SetTop(titleBorder, title.MapLocation.Left);
                            Canvas.SetLeft(titleBorder, title.MapLocation.Top);
                        }
                        else
                        {
                            if (title.DockOutsideViewport == true)
                            {
                                Canvas.SetTop(titleBorder, viewPosition.Top);
                                Canvas.SetLeft(titleBorder, viewPosition.Left);
                            }
                        }

                        titleBorder.Height = 30;
                        titleBorder.Width = this.Width - (margin.Right + margin.Left);
                        viewPosition.Top += titleBorder.Height;
                        titleBorder.Margin = new Thickness(margin.Left, margin.Top, margin.Right, margin.Bottom);
                        titleBorder.Child = mapTitle;
                        this.MapControlPanel.Children.Add(titleBorder);
                    }
                }
                if (this.MapModel.MapProperties.MapPolygonLayers != null)
                {
                    foreach (var layer in this.MapModel.MapProperties.MapPolygonLayers)
                    {
                        if ((layer.MapSpatialData != null && layer.MapSpatialData.MapShapefile != null) || layer.MapPolygons != null)
                        {
                            if (this.MapModel.MapEngineInfo.MapShapeActions != null && (!string.IsNullOrEmpty(this.MapModel.MapEngineInfo.MapShapeActions.BookmarkLink) ||
                            !string.IsNullOrEmpty(this.MapModel.MapEngineInfo.MapShapeActions.Hyperlink) || !string.IsNullOrEmpty(this.MapModel.MapEngineInfo.MapShapeActions.ReportName)))
                            {
                                shapelayer.EnableSelection = true;
                                shapelayer.ShapesSelected += shapelayer_ShapesSelected;
                            }
                            MapItemSetting lableSettings = new MapItemSetting();

                            if (layer.MapSpatialData != null && layer.MapSpatialData.MapShapefile != null)
                            {
                                shapelayer.Uri = new Uri(layer.MapSpatialData.MapShapefile.Source, UriKind.RelativeOrAbsolute).AbsolutePath;
                            }
                            else if (layer.MapPolygons != null && layer.MapPolygons.Count > 0)
                            {
                                var pathList = (from data in layer.MapPolygons select data.MapFields).ToList();
                                shapelayer.ShapeData = pathList;
                                shapelayer.ShapeDataPath = "VectorData";
                                pathList.Clear();
                            }
                            ShapeSetting shapeSetting = new ShapeSetting();
                            BubbleMarkerSetting bubblesetting = new BubbleMarkerSetting();
                            shapeSetting.ShapeFill = brushConverter.ConvertFromInvariantString("White");

                            if (layer.MapBindingFieldPairs != null)
                            {
                                shapelayer.ItemsSource = this.MapModel.MapModelData;

                                foreach (var bindingPair in layer.MapBindingFieldPairs)
                                {
                                    shapelayer.ShapeIDPath = "Value";
                                    shapelayer.ShapeIDTableField = bindingPair.FieldName;
                                }
                            }
                            if (layer.MapPolygonTemplate != null)
                            {
                                if (layer.MapPolygonTemplate.BorderExpval != null && layer.MapPolygonTemplate.BorderExpval.BorderStyle != BorderStyles.Default
                                    && layer.MapPolygonTemplate.BorderExpval.BorderStyle != BorderStyles.None)
                                {
                                    shapeSetting.ShapeStroke = brushConverter.ConvertFromInvariantString(layer.MapPolygonTemplate.BorderExpval.BorderBrush);
                                    shapeSetting.ShapeStrokeThickness = layer.MapPolygonTemplate.BorderExpval.Thickness;
                                }
                                if (layer.MapPolygonTemplate.BackGroundExpVal != null && layer.MapPolygonTemplate.BackGroundExpVal.BackgroundColor.ToLower() != "white")
                                {
                                    shapeSetting.ShapeFill = brushConverter.ConvertFromInvariantString(layer.MapPolygonTemplate.BackGroundExpVal.BackgroundColor);
                                }
                                if (layer.MapPolygonTemplate.StyleExpval != null)
                                {
                                    if (!string.IsNullOrEmpty(layer.MapPolygonTemplate.StyleExpval.TextColor))
                                    {
                                        lableSettings.MapItemForeground = brushConverter.ConvertFromInvariantString(layer.MapPolygonTemplate.StyleExpval.TextColor);
                                    }
                                    else
                                    {
                                        lableSettings.MapItemForeground = brushConverter.ConvertFromInvariantString("Black");
                                    }
                                    if (layer.MapPolygonTemplate.StyleExpval.Font != null)
                                    {
                                        lableSettings.MapItemFontFamily = new FontFamily(layer.MapPolygonTemplate.StyleExpval.Font.FontFamily);
                                        lableSettings.MapItemFontSize = layer.MapPolygonTemplate.StyleExpval.Font.FontSize;
                                        if (layer.MapPolygonTemplate.StyleExpval.Font.FontStyle != Syncfusion.RDL.DOM.FontStyle.Default)
                                        {
                                            lableSettings.MapItemFontStyle = new ReportingFontStyleConverter().ConvertFromInvariantString(layer.MapPolygonTemplate.StyleExpval.Font.FontStyle.ToString());
                                        }
                                    }
                                    shapelayer.MapItemSetting = lableSettings;
                                }
                                if (!string.IsNullOrEmpty(layer.MapPolygonTemplate.Label) && layer.MapPolygonTemplate.ShowLabel != BooleanOptions.False)
                                {
                                    string label = layer.MapPolygonTemplate.Label;
                                    if (label.StartsWith("#"))
                                    {
                                        shapeSetting.ShapeValuePath = label.Remove(0, 1);
                                    }
                                    else
                                    {
                                        shapeSetting.ShapeValuePath = "DisplayLabel";
                                    }
                                }
                            }
                            if (layer.MapPolygonRules != null && layer.MapPolygonRules.MapColorRule != null)
                            {
                                if (this.MapModel.MapModelData != null)
                                {
                                    bool setVisibility = false;
                                    if (string.IsNullOrEmpty(layer.MapPolygonTemplate.Label) || layer.MapPolygonTemplate.ShowLabel == BooleanOptions.False)
                                    {
                                        setVisibility = true;
                                        shapelayer.MapItemsVisibility = Windows.UI.Xaml.Visibility.Collapsed;
                                    }

                                    var colorValues = this.MapModel.MapModelData.Select(data => data.ColorValuePath).Where(data => data != null);

                                    if (colorValues != null && colorValues.Count() > 0)
                                    {
                                        shapeSetting.ShapeColorValuePath = "ColorValuePath";
                                        if (setVisibility)
                                        {
                                            shapeSetting.ShapeValuePath = "ColorValuePath";
                                        }
                                    }
                                    else
                                    {
                                        colorValues = this.MapModel.MapModelData.Select(data => data.Value).Where(data => data != null);
                                        if (colorValues != null && colorValues.Count() > 0)
                                        {
                                            shapeSetting.ShapeColorValuePath = "Value";
                                            if (setVisibility)
                                            {
                                                shapeSetting.ShapeValuePath = "Value";
                                            }
                                        }
                                    }
                                }
                                if (layer.MapPolygonRules.MapColorRule.MapColorRangeRule != null)
                                {
                                    if (this.MapModel.MapModelData != null)
                                    {
                                        var colorRule = layer.MapPolygonRules.MapColorRule.MapColorRangeRule;
                                        shapeSetting.FillSetting.ColorMappings = GetShapeColorMapping(layer.MapPolygonRules.MapColorRule,false);
                                    }
                                    else
                                    {
                                        shapeSetting.FillSetting.AutoFillColors = true;
                                    }
                                }
                                else if (layer.MapPolygonRules.MapColorRule.MapCustomColors != null)
                                {
                                    //Match customer colors to each polygons
                                }
                                else if (layer.MapPolygonRules.MapColorRule.ColorPalette != null)
                                {
                                    if (this.MapModel.MapModelData != null)
                                    {
                                        shapeSetting.FillSetting.ColorMappings = GetShapeColorMapping(null, true);
                                    }
                                    else
                                    {
                                        shapeSetting.FillSetting.AutoFillColors = true;
                                    }
                                }
                                if (!string.IsNullOrEmpty(layer.MapPolygonRules.MapColorRule.LegendName))
                                {
                                    if (this.MapModel.MapProperties.MapLegends != null)
                                    {
                                        var legend = (from len in this.MapModel.MapProperties.MapLegends
                                                      where len.Name.Equals(layer.MapPolygonRules.MapColorRule.LegendName)
                                                      select len).FirstOrDefault();

                                        if (legend != null)
                                        {
                                            if (legend.MapLegendTitle != null)
                                                shapelayer.LegendHeader = legend.MapLegendTitle.Caption;
                                            switch (legend.Position)
                                            {
                                                case Positions.RightBottom:
                                                case Positions.BottomRight:
                                                    shapelayer.LegendPosition = LegendPosition.BottomRight;
                                                    break;
                                                case Positions.LeftBottom:
                                                case Positions.BottomLeft:
                                                    shapelayer.LegendPosition = LegendPosition.BottomLeft;
                                                    break;
                                                case Positions.TopLeft:
                                                case Positions.LeftTop:
                                                    shapelayer.LegendPosition = LegendPosition.TopLeft;
                                                    break;
                                                case Positions.TopRight:
                                                case Positions.RightTop:
                                                    shapelayer.LegendPosition = LegendPosition.TopRight;
                                                    break;
                                                case Positions.LeftCenter:
                                                    shapelayer.LegendPosition = LegendPosition.MidLeft;
                                                    break;
                                                case Positions.RightCenter:
                                                    shapelayer.LegendPosition = LegendPosition.MidRight;
                                                    break;
                                                case Positions.BottomCenter:
                                                    shapelayer.LegendPosition = LegendPosition.BottomCenter;
                                                    break;
                                                case Positions.TopCenter:
                                                    shapelayer.LegendPosition = LegendPosition.TopCenter;
                                                    break;
                                            }

                                            shapelayer.LegendIcon = LegendIcons.Rectangle;
                                            shapelayer.LegendType = LegendType.Layers;
                                            shapelayer.LegendVisibility = Windows.UI.Xaml.Visibility.Visible;
                                        }
                                    }
                                }
                            }
                            if (layer.MapCenterPointRules != null)
                            {
                                if (layer.MapCenterPointRules.MapSizeRule != null)
                                {
                                    if (layer.MapCenterPointRules.MapSizeRule.StartSize != null)
                                    {
                                        bubblesetting.MinSize = layer.MapCenterPointRules.MapSizeRule.StartSize.PixelValue;
                                    }
                                    if (layer.MapCenterPointRules.MapSizeRule.EndSize != null)
                                    {
                                        bubblesetting.MaxSize = layer.MapCenterPointRules.MapSizeRule.EndSize.PixelValue;
                                    }
                                    string label = layer.MapCenterPointRules.MapSizeRule.DataValue;

                                    if (label.StartsWith("#"))
                                    {
                                        bubblesetting.ValuePath = label.Remove(0, 1);
                                    }
                                    else
                                    {
                                        bubblesetting.ValuePath = "BubbleValuePath";
                                    }
                                }
                                if (layer.MapCenterPointRules.MapColorRule != null)
                                {
                                    if (this.MapModel.MapModelData != null)
                                    {
                                        var colorValues = this.MapModel.MapModelData.Select(data => data.ColorValuePath).Where(data => data != null);

                                        if (colorValues != null && colorValues.Count() > 0)
                                        {
                                            bubblesetting.ColorValuePath = "ColorValuePath";
                                        }
                                        else
                                        {
                                            colorValues = this.MapModel.MapModelData.Select(data => data.Value).Where(data => data != null);
                                            if (colorValues != null && colorValues.Count() > 0)
                                            {
                                                bubblesetting.ColorValuePath = "Value";
                                            }
                                        }

                                        bubblesetting.Stroke = brushConverter.ConvertFromInvariantString("Black");
                                        bubblesetting.StrokeThickness = 2;

                                        bubblesetting.ColorMappings = GetShapeColorMapping(layer.MapCenterPointRules.MapColorRule, false);

                                        //bubblesetting.ColorMappings = GetBubbleColorMapping(layer.MapCenterPointRules.MapColorRule, true);
                                    }
                                    else
                                    {
                                        bubblesetting.Fill = brushConverter.ConvertFromInvariantString("#FFFFCC");
                                    }

                                    shapelayer.BubbleMarkerSetting = bubblesetting;
                                }
                            }

                            shapelayer.ShapeSettings = shapeSetting;
                            sfMapControl.Layers.Add(shapelayer);
                        }
                    }
                }
                if (this.MapModel.MapProperties.MapViewport != null)
                {
                    if (this.MapModel.MapProperties.MapViewport.Border != null && this.MapModel.MapProperties.MapViewport.Border.BorderStyle != BorderStyles.None
                        && this.MapModel.MapProperties.MapViewport.Border.BorderStyle != BorderStyles.Default)
                    {
                        viewPortBorder.BorderBrush = brushConverter.ConvertFromInvariantString(this.MapModel.MapProperties.MapViewport.Border.BorderBrush);
                        viewPortBorder.BorderThickness = new Thickness(this.MapModel.MapProperties.MapViewport.Border.Thickness);
                    }
                    if (this.MapModel.MapProperties.MapViewport.BackGroundExpVal != null)
                    {
                        viewPortBorder.Background = brushConverter.ConvertFromInvariantString(this.MapModel.MapProperties.MapViewport.BackGroundExpVal.BackgroundColor);
                        shapelayer.Background = brushConverter.ConvertFromInvariantString(this.MapModel.MapProperties.MapViewport.BackGroundExpVal.BackgroundColor);
                    }
                    if (this.MapModel.MapProperties.MapViewport.BottomMargin != null)
                    {
                        margin.Bottom = this.MapModel.MapProperties.MapViewport.BottomMargin.FloatValue;
                    }
                    if (this.MapModel.MapProperties.MapViewport.TopMargin != null)
                    {
                        margin.Top = this.MapModel.MapProperties.MapViewport.TopMargin.FloatValue;
                    }
                    if (this.MapModel.MapProperties.MapViewport.LeftMargin != null)
                    {
                        margin.Left = this.MapModel.MapProperties.MapViewport.LeftMargin.FloatValue;
                    }
                    if (this.MapModel.MapProperties.MapViewport.RightMargin != null)
                    {
                        margin.Right = this.MapModel.MapProperties.MapViewport.RightMargin.FloatValue;
                    }
                    if (this.MapModel.MapProperties.MapViewport.MaximumZoom != 0)
                    {
                        sfMapControl.MaxZoom = (int)this.MapModel.MapProperties.MapViewport.MaximumZoom;
                    }
                    if (this.MapModel.MapProperties.MapViewport.MapView != null)
                    {
                        if (this.MapModel.MapProperties.MapViewport.MapView.CenterX != 0 || this.MapModel.MapProperties.MapViewport.MapView.CenterY != 0)
                        {
                            //sfMapControl.Pan(this.MapModel.MapProperties.MapViewport.MapView.CenterX, this.MapModel.MapProperties.MapViewport.MapView.CenterY);
                        }
                        if (this.MapModel.MapProperties.MapViewport.MapView.Zoom != 0)
                        {
                            //sfMapControl.Zoom(this.MapModel.MapProperties.MapViewport.MapView.Zoom);
                        }
                    }

                    viewPortBorder.Margin = new Thickness(margin.Left, margin.Top, margin.Right, margin.Bottom);
                }

                sfMapControl.EnablePan = false;
                sfMapControl.EnableZoom = false;
                Canvas.SetLeft(viewPortBorder, viewPosition.Left);
                Canvas.SetTop(viewPortBorder, viewPosition.Top);
                sfMapControl.Height = this.Height - (viewPosition.Top + margin.Top + margin.Bottom + 4);
                sfMapControl.Width = this.Width - (viewPosition.Left + margin.Left + margin.Right + 4);
                viewPortBorder.Child = sfMapControl;
            }
            this.MapControlPanel.Children.Add(viewPortBorder);
        }

        private System.Collections.ObjectModel.ObservableCollection<ColorMapping> GetShapeColorMapping(MapColorRuleExpVal colorRule, bool isColorMap)
        {
            System.Collections.ObjectModel.ObservableCollection<ColorMapping> customeColors = new System.Collections.ObjectModel.ObservableCollection<ColorMapping>();
            var distinctCollection = this.MapModel.MapModelData.Select(data => data.ColorValuePath).Where(data => data != null).Distinct().ToList();

            if (distinctCollection != null && distinctCollection.Count() == 0)
            {
                distinctCollection = this.MapModel.MapModelData.Select(data => data.Value).Where(data => data != null).Distinct().ToList();
            }
            if (distinctCollection != null && distinctCollection.Count() == 0)
            {
                distinctCollection = this.MapModel.MapModelData.Select(data => data.DisplayLabel).Where(data => data != null).Distinct().ToList();
            }
            if (distinctCollection != null && distinctCollection.Count() > 0)
            {
                if (isColorMap)
                {
                    Random ran = new Random();
                    for (int i = 0; i < distinctCollection.Count; i++)
                    {
                        EqualsColorMapping colormap = new EqualsColorMapping();
                        colormap.Color = Color.FromArgb((byte)255, (byte)ran.Next(0, 255), (byte)ran.Next(0, 255),
                                                        (byte)ran.Next(0, 255));
                        colormap.Value = distinctCollection[i];
                        customeColors.Add(colormap);
                    }
                }
                else
                {
                    Color startColor =
                        (new ReportingBrushConverter().ConvertFromInvariantString(
                            (string.IsNullOrEmpty(colorRule.MapColorRangeRule.StartColor)
                                 ? "Green"
                                 : colorRule.MapColorRangeRule.StartColor)) as SolidColorBrush).Color;
                    Color endColor =
                        (new ReportingBrushConverter().ConvertFromInvariantString(
                            (string.IsNullOrEmpty(colorRule.MapColorRangeRule.EndColor)
                                 ? "Red"
                                 : colorRule.MapColorRangeRule.EndColor)) as SolidColorBrush).Color;
                    Color middleColor =
                        (new ReportingBrushConverter().ConvertFromInvariantString(
                            (string.IsNullOrEmpty(colorRule.MapColorRangeRule.MiddleColor)
                                 ? "Yellow"
                                 : colorRule.MapColorRangeRule.MiddleColor)) as SolidColorBrush).Color;

                    Color[] colorCollection = GetColors(startColor, middleColor, endColor, distinctCollection.Count);

                    try
                    {
                        object[] fromValues = null;
                        object[] toValues = null;

                        var rangeColorVal = this.MapModel.MapModelData.Select(data => data.ColorRangeValue).ToArray();

                        System.Type filedType = rangeColorVal.First().GetType();
                        Array.Sort(rangeColorVal);

                        object startVal = Convert.ChangeType(colorRule.StartValue, filedType, CultureInfo.InvariantCulture);
                        object endVal = Convert.ChangeType(colorRule.EndValue, filedType, CultureInfo.InvariantCulture);

                        if (colorRule.DistributionType == DistributionType.EqualInterval)
                        {
                            this.GetEqualIntervals(filedType, startVal, endVal, colorCollection.Length, ref fromValues,
                                                   ref toValues);
                            this.ColorComaparer(colorCollection, customeColors, distinctCollection, fromValues, toValues,
                                                filedType);
                        }
                        else if (colorRule.DistributionType == DistributionType.EqualDistribution)
                        {
                            this.GetEqualDistributionIntervals(filedType, rangeColorVal.ToList(), startVal, endVal,
                                                               colorCollection.Length, ref fromValues, ref toValues);
                            this.ColorComaparer(colorCollection, customeColors, distinctCollection, fromValues, toValues,
                                                filedType);
                        }
                        else if (colorRule.DistributionType == DistributionType.Optimal)
                        {
                            this.GetOptimalIntervals(filedType, rangeColorVal.ToList(), startVal, endVal,
                                                     colorCollection.Length, ref fromValues, ref toValues);
                            this.ColorComaparer(colorCollection, customeColors, distinctCollection, fromValues, toValues,
                                                filedType);
                        }
                        if (customeColors.Count == 0 && colorCollection.Length != 0)
                        {
                            for (int i = 0; i < colorCollection.Length; i++)
                            {
                                EqualsColorMapping colormap = new EqualsColorMapping();
                                colormap.Color = colorCollection[i];
                                colormap.Value = distinctCollection[i];
                                customeColors.Add(colormap);
                            }
                        }
                    }
                    catch
                    {
                        customeColors.Clear();
                        for (int i = 0; i < colorCollection.Length; i++)
                        {
                            EqualsColorMapping colormap = new EqualsColorMapping();
                            colormap.Color = colorCollection[i];
                            colormap.Value = distinctCollection[i];
                            customeColors.Add(colormap);
                        }
                    }
                }
            }
            return customeColors;
        }

        private List<Color> GetColorCollection(string start, string middle, string end, int stepsCount, bool autoFill)
        {
            List<Color> colorCollection = new List<Color>();
            if (!autoFill)
            {
                ReportingBrushConverter converter = new ReportingBrushConverter();
                Color startColor = (converter.ConvertFromInvariantString((string.IsNullOrEmpty(start) ? "Red" : start)) as SolidColorBrush).Color;
                Color endColor = (converter.ConvertFromInvariantString((string.IsNullOrEmpty(end) ? "Green" : end)) as SolidColorBrush).Color;
                colorCollection.Add(startColor);

                double aStep = (endColor.A - startColor.A) / stepsCount;
                double rStep = (endColor.R - startColor.R) / stepsCount;
                double gStep = (endColor.G - startColor.G) / stepsCount;
                double bStep = (endColor.B - startColor.B) / stepsCount;

                if (stepsCount > 2)
                {
                    for (int i = 0; i < stepsCount - 2; i++)
                    {
                        byte a = (byte)(startColor.A + (aStep * i));
                        var r = (byte)(startColor.R + (rStep * i));
                        var g = (byte)(startColor.G + (byte)(gStep * i));
                        var b = (byte)(startColor.B + (byte)(bStep * i));
                        colorCollection.Add(Color.FromArgb(a, r, g, b));
                    }
                }

                colorCollection.Add(endColor);
            }
            else
            {
                Random ran = new Random();
                for (int i = 0; i < stepsCount; i++)
                {
                    colorCollection.Add(Color.FromArgb((byte)255, (byte)ran.Next(0, 255), (byte)ran.Next(0, 255), (byte)ran.Next(0, 255)));
                }
            }

            return colorCollection;
        }


        private System.Collections.ObjectModel.ObservableCollection<ColorMapping> GetBubbleColorMapping(MapColorRuleExpVal colorRule, bool isColorMap)
        {
            System.Collections.ObjectModel.ObservableCollection<ColorMapping> customeColors = new System.Collections.ObjectModel.ObservableCollection<ColorMapping>();
            var distinctCollection = this.MapModel.MapModelData.Select(data => data.ColorValuePath).Where(data => data != null).Distinct().ToList();

            if (distinctCollection != null && distinctCollection.Count() == 0)
            {
                distinctCollection = this.MapModel.MapModelData.Select(data => data.BubbleValuePath).Where(data => data != null).Distinct().ToList();
            }
            if (distinctCollection != null && distinctCollection.Count() != 0)
            {
                if (isColorMap)
                {
                    Random ran = new Random();
                    for (int i = 0; i < distinctCollection.Count; i++)
                    {
                        EqualsColorMapping colormap = new EqualsColorMapping();
                        colormap.Color = Color.FromArgb((byte)255, (byte)ran.Next(0, 255), (byte)ran.Next(0, 255),
                                                        (byte)ran.Next(0, 255));
                        colormap.Value = distinctCollection[i];
                        customeColors.Add(colormap);
                    }
                }
                else
                {
                    Color startColor =
                        (new ReportingBrushConverter().ConvertFromInvariantString(
                            (string.IsNullOrEmpty(colorRule.MapColorRangeRule.StartColor)
                                 ? "Green"
                                 : colorRule.MapColorRangeRule.StartColor)) as SolidColorBrush).Color;
                    Color endColor =
                        (new ReportingBrushConverter().ConvertFromInvariantString(
                            (string.IsNullOrEmpty(colorRule.MapColorRangeRule.EndColor)
                                 ? "Red"
                                 : colorRule.MapColorRangeRule.EndColor)) as SolidColorBrush).Color;
                    Color middleColor =
                        (new ReportingBrushConverter().ConvertFromInvariantString(
                            (string.IsNullOrEmpty(colorRule.MapColorRangeRule.MiddleColor)
                                 ? "Yellow"
                                 : colorRule.MapColorRangeRule.MiddleColor)) as SolidColorBrush).Color;

                    Color[] colorCollection = GetColors(startColor, middleColor, endColor, distinctCollection.Count);

                    try
                    {
                        object[] fromValues = null;
                        object[] toValues = null;

                        var rangeColorVal = this.MapModel.MapModelData.Select(data => data.ColorRangeValue).ToArray();

                        System.Type filedType = rangeColorVal.First().GetType();
                        Array.Sort(rangeColorVal);

                        object startVal = Convert.ChangeType(colorRule.StartValue, filedType, CultureInfo.InvariantCulture);
                        object endVal = Convert.ChangeType(colorRule.EndValue, filedType, CultureInfo.InvariantCulture);

                        if (colorRule.DistributionType == DistributionType.EqualInterval)
                        {
                            this.GetEqualIntervals(filedType, startVal, endVal, colorCollection.Length, ref fromValues,
                                                   ref toValues);
                            this.ColorComaparer(colorCollection, customeColors, distinctCollection, fromValues, toValues,
                                                filedType);
                        }
                        else if (colorRule.DistributionType == DistributionType.EqualDistribution)
                        {
                            this.GetEqualDistributionIntervals(filedType, rangeColorVal.ToList(), startVal, endVal,
                                                               colorCollection.Length, ref fromValues, ref toValues);
                            this.ColorComaparer(colorCollection, customeColors, distinctCollection, fromValues, toValues,
                                                filedType);
                        }
                        else if (colorRule.DistributionType == DistributionType.Optimal)
                        {
                            this.GetOptimalIntervals(filedType, rangeColorVal.ToList(), startVal, endVal,
                                                     colorCollection.Length, ref fromValues, ref toValues);
                            this.ColorComaparer(colorCollection, customeColors, distinctCollection, fromValues, toValues,
                                                filedType);
                        }
                        else
                        {
                            for (int i = 0; i < colorCollection.Length; i++)
                            {
                                EqualsColorMapping colormap = new EqualsColorMapping();
                                colormap.Color = colorCollection[i];
                                colormap.Value = distinctCollection[i];
                                customeColors.Add(colormap);
                            }
                        }
                    }
                    catch
                    {
                        customeColors.Clear();
                        for (int i = 0; i < colorCollection.Length; i++)
                        {
                            EqualsColorMapping colormap = new EqualsColorMapping();
                            colormap.Color = colorCollection[i];
                            colormap.Value = distinctCollection[i];
                            customeColors.Add(colormap);
                        }
                    }
                }
            }
            return customeColors;
        }
        void sfMapControl_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!isMouseEvent)
            {
                if (e.OriginalSource is Path || e.OriginalSource is TextBlock)
                {
                    var data = e.OriginalSource is Path
                               ? (e.OriginalSource as Path).DataContext as ReportingMapData
                               : ((e.OriginalSource as TextBlock).DataContext is Syncfusion.UI.Xaml.Maps.MapItem
                                      ? ((e.OriginalSource as TextBlock).DataContext as Syncfusion.UI.Xaml.Maps.MapItem)
                                            .Data as ReportingMapData
                                      : new ReportingMapData());

                    if (data != null && data.ShapeActionInfo != null && (!string.IsNullOrEmpty(data.ShapeActionInfo.BookmarkLink) || !string.IsNullOrEmpty(data.ShapeActionInfo.Hyperlink) || !string.IsNullOrEmpty(data.ShapeActionInfo.ReportName)))
                    {
                        Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
                    }
                    else
                    {
                        Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
                    }
                }
                else
                {
                    Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
                }
            }
        }

        void shapelayer_ShapesSelected(object sender, SelectionEventArgs args)
        {
            var data = ((args.Items as ObservableCollection<MapShape>)[0].DataContext) as ReportingMapData;
            if (data != null && data.ShapeActionInfo != null)
            {
                if (!string.IsNullOrEmpty(data.ShapeActionInfo.BookmarkLink))
                {
                    Windows.System.Launcher.LaunchUriAsync(new Uri(data.ShapeActionInfo.BookmarkLink));
                }
                else if (!string.IsNullOrEmpty(data.ShapeActionInfo.Hyperlink))
                {
                    Windows.System.Launcher.LaunchUriAsync(new Uri(data.ShapeActionInfo.Hyperlink));
                }
                else if (!string.IsNullOrEmpty(data.ShapeActionInfo.ReportName))
                {
                    var actionInfo = data.ShapeActionInfo;
                    Drillthrough drillthrough = new Drillthrough()
                    {
                        Parameters = this.GetParameters(actionInfo.Parameters),
                        ReportName = actionInfo.ReportName
                    };
                    this.MapModel.Model.DrillThroughReport(this.MapModel.Model, drillthrough);
                }
            }
        }

        Syncfusion.RDL.DOM.Parameters GetParameters(List<TextboxParameterExpVal> action)
        {
            if (action != null)
            {
                Syncfusion.RDL.DOM.Parameters parameters = new Parameters();
                foreach (var para in action)
                {
                    Syncfusion.RDL.DOM.Parameter parameter = new Parameter();
                    parameter.Name = para.Name;
                    parameter.Omit = para.Omit;
                    parameter.Value = para.Value;
                    parameters.Add(parameter);
                }
                return parameters;
            }
            return null;
        }

        private void MapEvent()
        {
            action = new Syncfusion.RDL.DOM.Action();
            var actionInfo = this.MapModel.MapProperties.ActionInfo;
            if (actionInfo != null)
            {
                action.Hyperlink = actionInfo.Hyperlink;
                action.Drillthrough = new Drillthrough()
                {
                    Parameters = this.GetParameters(actionInfo.Parameters),
                    ReportName = actionInfo.ReportName
                };

                if (!string.IsNullOrEmpty(action.Hyperlink) || !string.IsNullOrEmpty(action.Drillthrough.ReportName))
                {
                    isMouseEvent = true;
                    this.PointerEntered += ReportingMap_PointerEntered;
                    this.PointerExited += ReportingMap_PointerExited;
                    this.PointerPressed += ReportingMap_PointerPressed;
                }
            }
        }

        void ReportingMap_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(action.Hyperlink))
            {
                Windows.System.Launcher.LaunchUriAsync(new Uri(action.Hyperlink));
            }
            else if (!string.IsNullOrEmpty(action.Drillthrough.ReportName))
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
                this.MapModel.Model.DrillThroughReport(this.MapModel.Model, action.Drillthrough);
            }
        }

        void ReportingMap_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }

        void ReportingMap_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
        }

        private void ColorComaparer(Color[] colorCollection, ObservableCollection<ColorMapping> customeColors, List<object> distinctCollection, object[] fromValues, object[] toValues, System.Type fieldType)
        {
            var rangeColorVal = this.MapModel.MapModelData.Select(data => data.ColorRangeValue).ToArray();

            if (fieldType == typeof(int))
            {
                for (int j = 0; j < colorCollection.Length; j++)
                {
                    for (int i = 0; i < fromValues.Length; i++)
                    {
                        if ((int)fromValues[i] <= (int)rangeColorVal[j] && (int)toValues[i] >= (int)rangeColorVal[j])
                        {
                            EqualsColorMapping colormap = new EqualsColorMapping();
                            colormap.Value = distinctCollection[j];
                            colormap.Color = colorCollection[i];
                            customeColors.Add(colormap);
                            break;
                        }
                    }
                }
            }
            else if (fieldType == typeof(double) || fieldType == typeof(decimal))
            {
                for (int j = 0; j < colorCollection.Length; j++)
                {
                    for (int i = 0; i < fromValues.Length; i++)
                    {
                        if ((decimal)fromValues[i] <= (decimal)rangeColorVal[j] && (decimal)toValues[i] >= (decimal)rangeColorVal[j])
                        {
                            EqualsColorMapping colormap = new EqualsColorMapping();
                            colormap.Value = distinctCollection[j];
                            colormap.Color = colorCollection[i];
                            customeColors.Add(colormap);
                            break;
                        }
                    }
                }
            }
            else if (fieldType == typeof(DateTime))
            {
                for (int j = 0; j < colorCollection.Length; j++)
                {
                    for (int i = 0; i < fromValues.Length; i++)
                    {
                        int c1 = DateTime.Compare((DateTime)fromValues[i], (DateTime)rangeColorVal[j]);
                        int c2 = DateTime.Compare((DateTime)toValues[i], (DateTime)rangeColorVal[j]);

                        if ((c1 == -1 || c1 == 0) && (c2 == 1 || c2 == 0))
                        {
                            EqualsColorMapping colormap = new EqualsColorMapping();
                            colormap.Value = distinctCollection[j];
                            colormap.Color = colorCollection[i];
                            customeColors.Add(colormap);
                            break;
                        }
                    }
                }
            }
            else if (fieldType == typeof(TimeSpan))
            {
                for (int j = 0; j < colorCollection.Length; j++)
                {
                    for (int i = 0; i < fromValues.Length; i++)
                    {
                        int c1 = TimeSpan.Compare((TimeSpan)fromValues[i], (TimeSpan)rangeColorVal[j]);
                        int c2 = TimeSpan.Compare((TimeSpan)toValues[i], (TimeSpan)rangeColorVal[j]);

                        if ((c1 == -1 || c1 == 0) && (c2 == 1 || c2 == 0))
                        {
                            EqualsColorMapping colormap = new EqualsColorMapping();
                            colormap.Value = distinctCollection[j];
                            colormap.Color = colorCollection[i];
                            customeColors.Add(colormap);
                            break;
                        }
                    }
                }
            }
        }


        private Color[] GetColors(Color startColor, Color middleColor, Color endColor, int colorCount)
        {
            Color[] clr = new Color[colorCount];
            Windows.UI.Xaml.Shapes.Rectangle verticalFillRectangle = new Windows.UI.Xaml.Shapes.Rectangle();
            verticalFillRectangle.Width = 1;
            verticalFillRectangle.Height = colorCount;

            LinearGradientBrush myVerticalGradient = new LinearGradientBrush();

            myVerticalGradient.StartPoint = new Point(0.5, 0);
            myVerticalGradient.EndPoint = new Point(0.5, 1);

            GradientStop start = new GradientStop();
            start.Color = startColor;
            start.Offset = 0.0;

            GradientStop middle = new GradientStop();
            middle.Color = middleColor;
            middle.Offset = 0.5;

            GradientStop end = new GradientStop();
            end.Color = endColor;
            end.Offset = 1.0;

            myVerticalGradient.GradientStops.Add(start);
            myVerticalGradient.GradientStops.Add(middle);
            myVerticalGradient.GradientStops.Add(end);

            verticalFillRectangle.Fill = myVerticalGradient;

            clr[0] = startColor;
            for (int i = 1; i < colorCount; i++)
            {
                clr[i] = this.GetColorAtPoint(verticalFillRectangle, new Point(1, i + 1));
            }
            clr[colorCount - 1] = endColor;
            return clr;
        }

        private Color GetColorAtPoint(Windows.UI.Xaml.Shapes.Rectangle theRec, Point thePoint)
        {
            //Get properties
            LinearGradientBrush br = (LinearGradientBrush)theRec.Fill;

            double y3 = thePoint.Y;
            double x3 = thePoint.X;

            double x1 = br.StartPoint.X * theRec.Width;
            double y1 = br.StartPoint.Y * theRec.Height;
            Point p1 = new Point(x1, y1); //Starting point

            double x2 = br.EndPoint.X * theRec.Width;
            double y2 = br.EndPoint.Y * theRec.Height;
            Point p2 = new Point(x2, y2);  //End point

            //Calculate intersecting points 
            Point p4 = new Point(); //with tangent

            if (y1 == y2) //Horizontal case
            {
                p4 = new Point(x3, y1);
            }

            else if (x1 == x2) //Vertical case
            {
                p4 = new Point(x1, y3);
            }

            else //Diagnonal case
            {
                double m = (y2 - y1) / (x2 - x1);
                double m2 = -1 / m;
                double b = y1 - m * x1;
                double c = y3 - m2 * x3;

                double x4 = (c - b) / (m - m2);
                double y4 = m * x4 + b;
                p4 = new Point(x4, y4);
            }

            //Calculate distances relative to the vector start
            double d4 = dist(p4, p1, p2);
            double d2 = dist(p2, p1, p2);

            double x = d4 / d2;

            //Clip the input if before or after the max/min offset values
            double max = br.GradientStops.Max(n => n.Offset);
            if (x > max)
            {
                x = max;
            }
            double min = br.GradientStops.Min(n => n.Offset);
            if (x < min)
            {
                x = min;
            }

            //Find gradient stops that surround the input value
            GradientStop gs0 = br.GradientStops.Where(n => n.Offset <= x).OrderBy(n => n.Offset).Last();
            GradientStop gs1 = br.GradientStops.Where(n => n.Offset >= x).OrderBy(n => n.Offset).First();

            float y = 0f;
            if (gs0.Offset != gs1.Offset)
            {
                y = (float)((x - gs0.Offset) / (gs1.Offset - gs0.Offset));
            }

            //Interpolate color channels
            Color cx = new Color();
            if (br.ColorInterpolationMode == ColorInterpolationMode.ScRgbLinearInterpolation)
            {
                //float aVal = (gs1.Color.ScA - gs0.Color.ScA) * y + gs0.Color.ScA;
                //float rVal = (gs1.Color.ScR - gs0.Color.ScR) * y + gs0.Color.ScR;
                //float gVal = (gs1.Color.ScG - gs0.Color.ScG) * y + gs0.Color.ScG;
                //float bVal = (gs1.Color.ScB - gs0.Color.ScB) * y + gs0.Color.ScB;
                //cx = Color.FromScRgb(aVal, rVal, gVal, bVal);
            }
            else
            {
                byte aVal = (byte)((gs1.Color.A - gs0.Color.A) * y + gs0.Color.A);
                byte rVal = (byte)((gs1.Color.R - gs0.Color.R) * y + gs0.Color.R);
                byte gVal = (byte)((gs1.Color.G - gs0.Color.G) * y + gs0.Color.G);
                byte bVal = (byte)((gs1.Color.B - gs0.Color.B) * y + gs0.Color.B);
                cx = Color.FromArgb(aVal, rVal, gVal, bVal);
            }
            return cx;
        }

        //Helper method for GetColorAtPoint
        //Returns the signed magnitude of a point on a vector with origin po and pointing to pf
        private double dist(Point px, Point po, Point pf)
        {
            double d = Math.Sqrt((px.Y - po.Y) * (px.Y - po.Y) + (px.X - po.X) * (px.X - po.X));
            if (((px.Y < po.Y) && (pf.Y > po.Y)) ||
                ((px.Y > po.Y) && (pf.Y < po.Y)) ||
                ((px.Y == po.Y) && (px.X < po.X) && (pf.X > po.X)) ||
                ((px.Y == po.Y) && (px.X > po.X) && (pf.X < po.X)))
            {
                d = -d;
            }
            return d;
        }

        private void GetEqualDistributionIntervals(System.Type fieldType, List<object> sortedList, object fromValue, object toValue, int intervalCount, ref object[] fromValues, ref object[] toValues)
        {
            var startVal = fromValue;
            var endVal = toValue;
            if (sortedList.Count == 0)
            {
                this.GetEqualIntervals(fieldType, fromValue, toValue, intervalCount, ref fromValues, ref toValues);
            }
            else
            {
                if (intervalCount > sortedList.Count)
                {
                    intervalCount = sortedList.Count;
                }
                if (intervalCount < 2)
                {
                    fromValue = this.GetRoundedAverage(fieldType, fromValue, fromValue, true);
                    toValue = this.GetRoundedAverage(fieldType, toValue, toValue, false);
                    this.GetEqualIntervals(fieldType, fromValue, toValue, intervalCount, ref fromValues, ref toValues);
                }
                else
                {
                    fromValues = new object[intervalCount];
                    toValues = new object[intervalCount];
                    double num = ((double)sortedList.Count) / ((double)fromValues.Length);
                    double a = num;
                    fromValues[0] = this.GetRoundedAverage(fieldType, sortedList[0], sortedList[0], true);
                    object obj2 = this.GetRoundedAverage(fieldType, sortedList[(int)Math.Round((double)(a - 1.0))], sortedList[(int)Math.Round(a)], true);
                    toValues[0] = obj2;
                    for (int i = 1; i < (fromValues.Length - 1); i++)
                    {
                        a += num;
                        fromValues[i] = obj2;
                        obj2 = this.GetRoundedAverage(fieldType, sortedList[(int)Math.Round((double)(a - 1.0))], sortedList[(int)Math.Round(a)], true);
                        toValues[i] = obj2;
                    }
                    fromValues[fromValues.Length - 1] = obj2;
                    toValues[fromValues.Length - 1] = this.GetRoundedAverage(fieldType, sortedList[sortedList.Count - 1], sortedList[sortedList.Count - 1], false);

                    fromValues[0] = startVal;
                    toValues[toValues.Length - 1] = endVal;
                }
            }
        }

        private void GetEqualIntervals(System.Type fieldType, object fromValue, object toValue, int intervalCount, ref object[] fromValues, ref object[] toValues)
        {
            fromValues = new object[intervalCount];
            toValues = new object[intervalCount];
            if (fieldType == typeof(int))
            {
                int num = (int)fromValue;
                int num2 = (int)toValue;
                int num3 = (int)Math.Round((double)(((double)(num2 - num)) / ((double)fromValues.Length)));
                int num4 = num;
                for (int i = 0; i < fromValues.Length; i++)
                {
                    fromValues[i] = num4;
                    toValues[i] = num4 + num3;
                    num4 += num3;
                }
            }
            else if (fieldType == typeof(double))
            {
                decimal num6 = (decimal)((double)fromValue);
                decimal num7 = (decimal)((double)toValue);
                decimal num8 = (num7 - num6) / fromValues.Length;
                decimal num9 = num6;
                for (int j = 0; j < fromValues.Length; j++)
                {
                    fromValues[j] = (double)num9;
                    toValues[j] = (double)(num9 + num8);
                    num9 += num8;
                }
            }
            else if (fieldType == typeof(decimal))
            {
                decimal num11 = (decimal)fromValue;
                decimal num12 = (decimal)toValue;
                decimal num13 = (num12 - num11) / fromValues.Length;
                decimal num14 = num11;
                for (int k = 0; k < fromValues.Length; k++)
                {
                    fromValues[k] = num14;
                    toValues[k] = num14 + num13;
                    num14 += num13;
                }
            }
            else if (fieldType == typeof(DateTime))
            {
                DateTime time = (DateTime)fromValue;
                DateTime time2 = (DateTime)toValue;
                TimeSpan span6 = (TimeSpan)(time2 - time);
                TimeSpan span = new TimeSpan(span6.Ticks / ((long)fromValues.Length));
                DateTime time3 = time;
                for (int m = 0; m < fromValues.Length; m++)
                {
                    fromValues[m] = time3;
                    toValues[m] = time3 + span;
                    time3 += span;
                }
            }
            else if (fieldType == typeof(TimeSpan))
            {
                TimeSpan span2 = (TimeSpan)fromValue;
                TimeSpan span3 = (TimeSpan)toValue;
                TimeSpan span7 = span3 - span2;
                TimeSpan span4 = new TimeSpan(span7.Ticks / ((long)fromValues.Length));
                TimeSpan span5 = span2;
                for (int n = 0; n < fromValues.Length; n++)
                {
                    fromValues[n] = span5;
                    toValues[n] = span5 + span4;
                    span5 += span4;
                }
            }
            if (toValues.Length > 0)
            {
                toValues[toValues.Length - 1] = toValue;
            }
        }

        private string ToStringInvariant(object fieldValue)
        {
            if (fieldValue == null)
            {
                return string.Empty;
            }
          return (string)Convert.ChangeType(fieldValue,typeof(string), CultureInfo.InvariantCulture);          
        }

        private void GetOptimalIntervals(System.Type fieldType, List<object> sortedList, object fromValue, object toValue, int intervalCount, ref object[] fromValues, ref object[] toValues)
        {
            var startVal = fromValue;
            var endVal = toValue;

            if ((sortedList.Count == 0) || (this.ToStringInvariant(fromValue) == this.ToStringInvariant(toValue)))
            {
                this.GetEqualIntervals(fieldType, fromValue, toValue, intervalCount, ref fromValues, ref toValues);
            }
            else
            {
                intervalCount = Math.Min(intervalCount, sortedList.Count);
                if (intervalCount < 4)
                {
                    if (intervalCount < 1)
                    {
                        intervalCount = 1;
                    }
                    this.GetEqualIntervals(fieldType, fromValue, toValue, intervalCount, ref fromValues, ref toValues);
                }
                else
                {
                    if (intervalCount > (sortedList.Count - 3))
                    {
                        intervalCount = sortedList.Count - 3;
                    }
                    if (intervalCount < 4)
                    {
                        if (intervalCount < 1)
                        {
                            intervalCount = 1;
                        }
                        this.GetEqualIntervals(fieldType, fromValue, toValue, intervalCount, ref fromValues, ref toValues);
                    }
                    else
                    {
                        List<object> list = new List<object>();
                        for (int i = 0; i < sortedList.Count; i++)
                        {
                            list.Add(Convert.ChangeType(sortedList[i], fieldType, CultureInfo.InvariantCulture));
                        }
                        int[] jenksBreaks = this.GetJenksBreaks(list, intervalCount);
                        fromValues = new object[jenksBreaks.Length];
                        toValues = new object[jenksBreaks.Length];
                        fromValues[0] = this.GetRoundedAverage(fieldType, sortedList[0], sortedList[0], true);
                        object obj2 = this.GetRoundedAverage(fieldType, sortedList[jenksBreaks[0] - 1], sortedList[jenksBreaks[0]], true);
                        toValues[0] = obj2;
                        for (int j = 1; j < (fromValues.Length - 1); j++)
                        {
                            fromValues[j] = obj2;
                            obj2 = this.GetRoundedAverage(fieldType, sortedList[jenksBreaks[j] - 1], sortedList[jenksBreaks[j]], true);
                            toValues[j] = obj2;
                        }
                        fromValues[fromValues.Length - 1] = obj2;
                        toValues[fromValues.Length - 1] = this.GetRoundedAverage(fieldType, sortedList[sortedList.Count - 1], sortedList[sortedList.Count - 1], false);
                        fromValues[0] = startVal;
                        toValues[toValues.Length - 1] = endVal;
                    }
                }
            }
        }

        private int[] GetJenksBreaks(List<object> list, int itervalCount)
        {
            int count = list.Count;
            double[] numArray = list.Select(s => double.Parse(s.ToString())).ToArray();
            double[][] numArray2 = new double[count + 1][];
            double[][] numArray3 = new double[count + 1][];
            for (int i = 0; i <= count; i++)
            {
                numArray2[i] = new double[itervalCount + 1];
                numArray3[i] = new double[itervalCount + 1];
            }
            for (int j = 1; j <= itervalCount; j++)
            {
                numArray2[1][j] = 1.0;
                numArray3[1][j] = 0.0;
                for (int n = 2; n <= count; n++)
                {
                    numArray3[n][j] = double.MaxValue;
                }
            }
            double num5 = 0.0;
            for (int k = 2; k <= count; k++)
            {
                double num7 = 0.0;
                double num8 = 0.0;
                double[] numArray4 = numArray2[k];
                double[] numArray5 = numArray3[k];
                for (int num9 = 1; num9 <= k; num9++)
                {
                    int num10 = k - num9;
                    double num11 = numArray[num10];
                    num8 += num11 * num11;
                    num7 += num11;
                    num5 = num8 - ((num7 * num7) / ((double)num9));
                    if (num10 != 0)
                    {
                        double[] numArray6 = numArray3[num10];
                        for (int num12 = 2; num12 <= itervalCount; num12++)
                        {
                            if (numArray5[num12] >= (num5 + numArray6[num12 - 1]))
                            {
                                numArray4[num12] = num10 + 1;
                                numArray5[num12] = num5 + numArray6[num12 - 1];
                            }
                        }
                    }
                }
                numArray4[1] = 1.0;
                numArray5[1] = num5;
            }
            List<object> list2 = new List<object>();
            int index = count;
            for (int m = itervalCount; m >= 2; m--)
            {
                int num15 = ((int)numArray2[index][m]) - 2;
                if (num15 > 0)
                {
                    list2.Insert(0, num15);
                }
                index = Math.Max(((int)numArray2[index][m]) - 1, 0);
            }
            list2.Add(list.Count - 1);
            return list2.Cast<int>().ToArray();
        }

        private object GetRoundedAverage(System.Type fieldType, object value1, object value2, bool floor)
        {
            if (fieldType == typeof(int))
            {
                int num = (int)value1;
                int num2 = (int)value2;
                double d = num + (0.5 * (num2 - num));
                if (d != 0.0)
                {
                    int num4 = (int)Math.Log10(d);
                    double num5 = Math.Pow(10.0, (double)(num4 - 1));
                    d /= num5;
                    if (floor)
                    {
                        d = Math.Floor(d);
                    }
                    else
                    {
                        d = Math.Ceiling(d);
                    }
                    d *= num5;
                }
                return (int)d;
            }
            if (fieldType == typeof(double))
            {
                double num6 = (double)value1;
                double num7 = (double)value2;
                double num8 = num6 + (0.5 * (num7 - num6));
                if (num8 == 0.0)
                {
                    return num8;
                }
                int num9 = (int)Math.Log10(Math.Abs(num8));
                double num10 = Math.Pow(10.0, (double)(num9 - 1));
                num8 /= num10;
                if (floor)
                {
                    num8 = Math.Floor(num8);
                }
                else
                {
                    num8 = Math.Ceiling(num8);
                }
                return (num8 * num10);
            }
            if (fieldType == typeof(decimal))
            {
                decimal num11 = (decimal)value1;
                decimal num12 = (decimal)value2;
                decimal num13 = num11 + (0.5M * (num12 - num11));
                if (num13 == 0M)
                {
                    return num13;
                }
                int num14 = (int)Math.Log10((double)Math.Abs(num13));
                decimal num15 = (decimal)Math.Pow(10.0, (double)(num14 - 1));
                num13 /= num15;
                if (floor)
                {
                    num13 = decimal.Floor(num13);
                }
                else
                {
                    num13 = (decimal)Math.Ceiling((double)num13);
                }
                return (num13 * num15);
            }
            if (fieldType == typeof(DateTime))
            {
                DateTime time = (DateTime)value1;
                DateTime time2 = (DateTime)value2;
                TimeSpan span6 = (TimeSpan)(time2 - time);
                TimeSpan span = new TimeSpan(span6.Ticks / 2L);
                return (time + span);
            }
            TimeSpan span2 = (TimeSpan)value1;
            TimeSpan span3 = (TimeSpan)value2;
            TimeSpan span7 = span3 - span2;
            TimeSpan span4 = new TimeSpan(span7.Ticks / 2L);
            return (span2 + span4);
        }

    }
}
