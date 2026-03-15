#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.DOM;
using Syncfusion.RDL.Internal;
using System.IO;
using System.Globalization;
using Syncfusion.RDL.Controls;
using System.Collections;
using System.ComponentModel;

#if !WINRT
using Syncfusion.Linq;
#endif

namespace Syncfusion.RDL.ItemModel
{
    /// <summary>
    /// A model for MapModel. Contains information about size, position, data and style for a Map.
    /// </summary>
    internal class MapModel : ReportItemModeler
    {
        #region members

        MapItemExp mapPropertiesExp;

        #endregion

        #region  properties

        public MapItemExpVal MapProperties
        {
            get;
            set;
        }

        internal int PrintPageColumnCount
        {
            get;
            set;
        }

        internal Dictionary<int, PageInfo> PageSizes
        {
            get;
            set;
        }

        internal List<double> PageWidths
        {
            get;
            set;
        }

        internal Dictionary<int, PageInfo> PrintPageSizes
        {
            get;
            set;
        }

        internal List<double> PrintPageWidths
        {
            get;
            set;
        }

        internal List<ReportingMapData> MapModelData
        {
            get;
            set;
        }

        internal MapInfo MapEngineInfo
        {
            get; 
            set;
        }

        #endregion

        #region Constructors

        internal MapModel()
        {
        }

        internal MapModel(ReportItem reportItem, ReportModel pageModel, bool isTablixChild, string dataSetName)
        {
            this.Model = pageModel;
            this.ReportItem = reportItem;
            this.ModelType = ModelType.MapModel;
            this.Name = reportItem.Name;
            this.IsTablixChild = isTablixChild;

            Map map = (reportItem as Map);
            this.DataSetName = map.DataSetName;
            this.DataSetFields = new List<DataField>();

            if (map.PageBreak != null)
            {
                this.PageBreak = map.PageBreak.BreakLocation;
            }

            if (this.DataSetName == null)
            {
                this.DataSetName = dataSetName;
            }
            if (reportItem.Top != null)
            {
                this.Top = reportItem.Top.PixelValue;
            }

            if (reportItem.Left != null)
            {
                this.Left = reportItem.Left.PixelValue;
            }

            if (reportItem.Width != null)
            {
                this.Width = reportItem.Width.PixelValue;
            }

            if (reportItem.Height != null)
            {
                this.Height = reportItem.Height.PixelValue;
            }

            this.ParseMap();
        }

        void ParseMap()
        {
            try
            {
                Map mapModel = this.ReportItem as Map;
                this.mapPropertiesExp = new MapItemExp();

                this.MapEngineInfo = new MapInfo();
                
                if (mapModel.MapDataRegions != null)
                {
                    this.mapPropertiesExp.MapDataRegions = new List<MapDataRegionExp>();

                    foreach (var dataregion in mapModel.MapDataRegions)
                    {
                        this.mapPropertiesExp.MapDataRegions.Add(GetMapDataRegionExp(dataregion));
                    }
                }

                this.mapPropertiesExp.ToolTip = this.GetExpressionKey(mapModel.ToolTip);
                this.mapPropertiesExp.DocumentMapLabel = this.GetExpressionKey(mapModel.DocumentMapLabel);
                this.mapPropertiesExp.DataSetName = this.GetExpressionKey(mapModel.DataSetName);
                
                var dataRegion = this.ReportItem as DataRegion;
                if (dataRegion != null)
                {
                    this.ExpFilters = this.Model.ParseFilters(dataRegion.Filters, this.DataSetName, true);
                }
                if (mapModel.Visibility != null && mapModel.Visibility.Hidden != null)
                {
                    this.mapPropertiesExp.Hidden = this.GetExpressionKey(mapModel.Visibility.Hidden);
                }

                this.mapPropertiesExp.Border = new BorderExp();
                RDL.DOM.Style style = mapModel.Style;

                if (style.Border != null)
                {
                    this.mapPropertiesExp.Border.Default = GetBorderExp(style.Border);
                }
                if (style.BottomBorder != null)
                {
                    this.mapPropertiesExp.Border.BottomBorder = new BorderExpProperties();
                    this.mapPropertiesExp.Border.BottomBorder.BorderBrush = this.GetExpressionKey(style.BottomBorder.Color);
                    this.mapPropertiesExp.Border.BottomBorder.BorderStyle = this.GetExpressionKey(style.BottomBorder.Style);

                    if (style.BottomBorder.Width != null)
                    {
                        this.mapPropertiesExp.Border.BottomBorder.Thickness = this.GetExpressionKey(style.BottomBorder.Width.size);
                    }
                }
                if (style.LeftBorder != null)
                {
                    this.mapPropertiesExp.Border.LeftBorder = new BorderExpProperties();
                    this.mapPropertiesExp.Border.LeftBorder.BorderBrush = this.GetExpressionKey(style.LeftBorder.Color);
                    this.mapPropertiesExp.Border.LeftBorder.BorderStyle = this.GetExpressionKey(style.LeftBorder.Style);

                    if (style.LeftBorder.Width != null)
                    {
                        this.mapPropertiesExp.Border.LeftBorder.Thickness = this.GetExpressionKey(style.LeftBorder.Width.size);
                    }
                }
                if (style.RightBorder != null)
                {
                    this.mapPropertiesExp.Border.RightBorder = new BorderExpProperties();
                    this.mapPropertiesExp.Border.RightBorder.BorderBrush = this.GetExpressionKey(style.RightBorder.Color);
                    this.mapPropertiesExp.Border.RightBorder.BorderStyle = this.GetExpressionKey(style.RightBorder.Style);

                    if (style.RightBorder.Width != null)
                    {
                        this.mapPropertiesExp.Border.RightBorder.Thickness = this.GetExpressionKey(style.RightBorder.Width.size);
                    }
                }
                if (style.TopBorder != null)
                {
                    this.mapPropertiesExp.Border.TopBorder = new BorderExpProperties();
                    this.mapPropertiesExp.Border.TopBorder.BorderBrush = this.GetExpressionKey(style.TopBorder.Color);
                    this.mapPropertiesExp.Border.TopBorder.BorderStyle = this.GetExpressionKey(style.TopBorder.Style);

                    if (style.TopBorder.Width != null)
                    {
                        this.mapPropertiesExp.Border.TopBorder.Thickness = this.GetExpressionKey(style.TopBorder.Width.size);
                    }
                }

                this.mapPropertiesExp.Background = GetBackgroundExp(mapModel.Style);
                this.mapPropertiesExp.TextAntiAliasingQuality = this.GetExpressionKey(mapModel.TextAntiAliasingQuality.ToString());
                this.mapPropertiesExp.ShadowIntensity = this.GetExpressionKey(mapModel.ShadowIntensity.ToString());
                this.mapPropertiesExp.TileLanguage = this.GetExpressionKey(mapModel.TileLanguage);
                this.mapPropertiesExp.MaximumTotalPointCount = mapModel.MaximumTotalPointCount;
                this.mapPropertiesExp.MaximumSpatialElementCount = mapModel.MaximumSpatialElementCount;
                this.mapPropertiesExp.AntiAliasing = this.GetExpressionKey(mapModel.AntiAliasing.ToString());
                this.mapPropertiesExp.Zindex = this.GetExpressionKey(mapModel.ZIndex.ToString());

                if (mapModel.MapViewport != null)
                {
                    this.mapPropertiesExp.MapViewport = GetMapViewportExp(mapModel.MapViewport);
                }

                if (mapModel.MapLegends != null)
                {
                    this.mapPropertiesExp.MapLegendsExp = new List<MapLegendExp>();
                    foreach (var legend in mapModel.MapLegends)
                    {
                        this.mapPropertiesExp.MapLegendsExp.Add(GetMapLegendExp(legend));
                    }
                }

                if (mapModel.MapTitles != null)
                {
                    this.mapPropertiesExp.MapTitlesExp = new List<MapTitleExp>();
                    foreach (var title in mapModel.MapTitles)
                    {
                        this.mapPropertiesExp.MapTitlesExp.Add(GetMapTitleExp(title));
                    }
                }

                if (mapModel.MapLayers != null)
                {
                    foreach (var layer in mapModel.MapLayers)
                    {
                        MapLayersExp(layer);
                    }
                }

                if (mapModel.MapDistanceScale != null)
                {
                    this.mapPropertiesExp.MapDistanceScale = GetMapDistanceScaleExp(mapModel.MapDistanceScale);
                }

                if (mapModel.MapColorScale != null)
                {
                    this.mapPropertiesExp.MapColorScale = GetMapColorScaleExp(mapModel.MapColorScale);
                }

                if (mapModel.MapBorderSkin != null)
                {
                    this.mapPropertiesExp.MapBorderSkin = GetMapBorderSkinExp(mapModel.MapBorderSkin);
                }

                this.mapPropertiesExp.ActionInfo = this.GetActionInfoExp(mapModel.ActionInfo);
            }
            catch { }
        }

        private MapDataRegionExp GetMapDataRegionExp(MapDataRegion mapDataregion)
        {
            MapDataRegionExp dataRegion = new MapDataRegionExp();

            if (mapDataregion.MapMember != null)
            {
                dataRegion.MapMember = new MapMemberExp();

                if (mapDataregion.MapMember.Group != null)
                {
                    dataRegion.MapMember.Group = new GroupExp();

                    if (mapDataregion.MapMember.Group.PageBreak != null)
                    {
                        dataRegion.MapMember.Group.BreakLocation = mapDataregion.MapMember.Group.PageBreak.BreakLocation.ToString();
                    }
                    if (mapDataregion.MapMember.Group.GroupExpressions != null)
                    {
                        dataRegion.MapMember.Group.GroupExpressions = new List<string>();
                        foreach (DOM.GroupExpression grpExp in mapDataregion.MapMember.Group.GroupExpressions)
                        {
                            dataRegion.MapMember.Group.GroupExpressions.Add(grpExp.Value);
                        }
                    }
                    if (mapDataregion.MapMember.Group.Filters != null)
                    {
                        dataRegion.MapMember.Group.Filters = new FiltersExp();
                        foreach (DOM.Filter filter in mapDataregion.MapMember.Group.Filters)
                        {
                            FilterExp fl = new FilterExp();
                            fl.FilterExpression = this.GetExpressionKey(filter.FilterExpression);
                            
                            if (filter.FilterValues != null)
                            {
                                fl.FilterValues = new FilterValuesExp();
                                foreach (DOM.FilterValue flValue in filter.FilterValues)
                                {
                                    FilterValueExp flv = new FilterValueExp();
                                    flv.DataType = flValue.DataType.ToString();
                                    flv.Value = flValue.Value;
                                    fl.FilterValues.Add(flv);
                                }
                            }

                            fl.Operator = filter.Operator.ToString();
                            dataRegion.MapMember.Group.Filters.Add(fl);
                        }
                    }

                    dataRegion.MapMember.Group.DataElementName = this.GetExpressionKey(mapDataregion.MapMember.Group.DataElementName);
                    dataRegion.MapMember.Group.DocumentMapLabel = this.GetExpressionKey(mapDataregion.MapMember.Group.DocumentMapLabel);
                    dataRegion.MapMember.Group.DomainScope = this.GetExpressionKey(mapDataregion.MapMember.Group.DomainScope);
                    dataRegion.MapMember.Group.Name = this.GetExpressionKey(mapDataregion.MapMember.Group.Name);
                    dataRegion.MapMember.Group.Parent = this.GetExpressionKey(mapDataregion.MapMember.Group.Parent);
                }
            }
            if (mapDataregion.Filters != null)
            {
                dataRegion.Filters = new FiltersExp();
                foreach (DOM.Filter filter in mapDataregion.Filters)
                {
                    FilterExp fl = new FilterExp();
                    fl.FilterExpression = this.GetExpressionKey(filter.FilterExpression);
                    
                    if (filter.FilterValues != null)
                    {
                        fl.FilterValues = new FilterValuesExp();
                        foreach (DOM.FilterValue flValue in filter.FilterValues)
                        {
                            FilterValueExp flv = new FilterValueExp();
                            flv.DataType = flValue.DataType.ToString();
                            flv.Value = flValue.Value;
                            fl.FilterValues.Add(flv);
                        }
                    }

                    fl.Operator = filter.Operator.ToString();
                    dataRegion.Filters.Add(fl);
                }
            }

            dataRegion.Name = this.GetExpressionKey(mapDataregion.Name);
            dataRegion.DataSetName = this.GetExpressionKey(mapDataregion.DataSetName);
            
            if (this.DataSetName == null)
            {
                this.DataSetName = this.mapPropertiesExp.DataSetName = mapDataregion.DataSetName;
            }

            return dataRegion;
        }

        private BackGroundExp GetBackgroundExp(Style style)
        {
            BackGroundExp background = new BackGroundExp();
            background.BackgroundColor = this.GetExpressionKey(style.BackgroundColor);
            background.BackgroundGradientType = this.GetExpressionKey(style.BackgroundGradientType.ToString());
            background.BackgroundGradientEndColor = this.GetExpressionKey(style.BackgroundGradientEndColor);
            background.BackgroundHatchType = this.GetExpressionKey(style.BackgroundHatchType.ToString());

            return background;
        }

        private BorderExpProperties GetBorderExp(Border border)
        {
            if (border != null)
            {
                BorderExpProperties borderExp = new BorderExpProperties();
                borderExp.BorderBrush = this.GetExpressionKey(border.Color);
                borderExp.BorderStyle = this.GetExpressionKey(border.Style);

                if (border.Width != null)
                {
                    border.Width = this.GetExpressionKey(border.Width.size);
                }

                return borderExp;
            }

            return null;
        }

        private TextboxActionInfoExp GetActionInfoExp(ActionInfo actionInfo)
        {
            return this.GetActionInfoExp(actionInfo, false);
        }

        private TextboxActionInfoExp GetActionInfoExp(ActionInfo actionInfo , bool isChild)
        {
            try
            {
            if (actionInfo != null)
            {
                TextboxActionInfoExp ActionInfo = new TextboxActionInfoExp();
                foreach (RDL.DOM.Action action in actionInfo.Actions)
                {
                    ActionInfo.BookmarkLink = isChild ? this.GetExpression(action.BookmarkLink) : this.GetExpressionKey(action.BookmarkLink);
                    ActionInfo.Hyperlink = isChild ? this.GetExpression(action.Hyperlink) : this.GetExpressionKey(action.Hyperlink);
                    if (action.Drillthrough != null)
                    {
                        ActionInfo.ReportName = isChild ? this.GetExpression(action.Drillthrough.ReportName) : this.GetExpressionKey(action.Drillthrough.ReportName);
                        ActionInfo.Parameters = new List<TextboxParameterExp>();

                        foreach (Parameter parameters in action.Drillthrough.Parameters)
                        {
                            TextboxParameterExp Parameter = new TextboxParameterExp();
                            Parameter.Name = isChild ? this.GetExpression(parameters.Name) : this.GetExpressionKey(parameters.Name);
                            Parameter.Omit = isChild ? this.GetExpression(parameters.Omit) : this.GetExpressionKey(parameters.Omit);
                            Parameter.Value = isChild ? this.GetExpression(parameters.Value) : this.GetExpressionKey(parameters.Value);
                            ActionInfo.Parameters.Add(Parameter);
                        }
                    }
                }

                return ActionInfo;
            }

            }
            catch 
            {

            }
            return null;
        }
                
        private MapBorderSkinExp GetMapBorderSkinExp(MapBorderSkin mapBorderSkin)
        {
            MapBorderSkinExp borderSkinExp = new MapBorderSkinExp();

            if (mapBorderSkin.Style != null)
            {
                borderSkinExp.Border = GetBorderExp(mapBorderSkin.Style.Border);
                borderSkinExp.BackGroundExp = GetBackgroundExp(mapBorderSkin.Style);
                borderSkinExp.Style = GetFontStyleExp(mapBorderSkin.Style);
            }

            borderSkinExp.MapBorderSkinType = this.GetExpressionKey(mapBorderSkin.MapBorderSkinType.ToString());

            return borderSkinExp;
        }

        private MapColorScaleExp GetMapColorScaleExp(MapColorScale mapColorScale)
        {
            MapColorScaleExp colorScaleExp = new MapColorScaleExp();

            if (mapColorScale.Style != null)
            {
                colorScaleExp.Border = GetBorderExp(mapColorScale.Style.Border);
                colorScaleExp.BackGroundExp = GetBackgroundExp(mapColorScale.Style);
                colorScaleExp.Style = GetFontStyleExp(mapColorScale.Style);
            }
            if (mapColorScale.MapLocation != null)
            {
                colorScaleExp.MapLocation = GetMapLocationExp(mapColorScale.MapLocation);
            }
            if (mapColorScale.MapSize != null)
            {
                colorScaleExp.MapSize = GetMapSizeExp(mapColorScale.MapSize);
            }
            if (mapColorScale.MapColorScaleTitle != null)
            {
                colorScaleExp.MapColorScaleTitle = new MapColorScaleTitleExp();

                if (mapColorScale.MapColorScaleTitle.Style != null)
                {
                    colorScaleExp.Border = GetBorderExp(mapColorScale.MapColorScaleTitle.Style.Border);
                    colorScaleExp.BackGroundExp = GetBackgroundExp(mapColorScale.MapColorScaleTitle.Style);
                    colorScaleExp.Style = GetFontStyleExp(mapColorScale.MapColorScaleTitle.Style);
                }

                colorScaleExp.MapColorScaleTitle.Caption = this.GetExpressionKey(mapColorScale.MapColorScaleTitle.Caption);
            }
            if (mapColorScale.BottomMargin != null)
            {
                colorScaleExp.BottomMargin = this.GetExpressionKey(mapColorScale.BottomMargin.size);
            }
            if (mapColorScale.TopMargin != null)
            {
                colorScaleExp.TopMargin = this.GetExpressionKey(mapColorScale.TopMargin.size);
            }
            if (mapColorScale.RightMargin != null)
            {
                colorScaleExp.RightMargin = this.GetExpressionKey(mapColorScale.RightMargin.size);
            }
            if (mapColorScale.LeftMargin != null)
            {
                colorScaleExp.LeftMargin = this.GetExpressionKey(mapColorScale.LeftMargin.size);
            }
            if (mapColorScale.TickMarkLength != null)
            {
                colorScaleExp.TickMarkLength = this.GetExpressionKey(mapColorScale.TickMarkLength.size.ToString());
            }
            if (mapColorScale.ActionInfo != null)
            {
                colorScaleExp.ActionInfo = GetActionInfoExp(mapColorScale.ActionInfo);
            }

            colorScaleExp.ColorBarBorderColor = this.GetExpressionKey(mapColorScale.ColorBarBorderColor);
            colorScaleExp.DockOutsideViewport = this.GetExpressionKey(mapColorScale.DockOutsideViewport.ToString());
            colorScaleExp.Hidden = this.GetExpressionKey(mapColorScale.Hidden.ToString());
            colorScaleExp.HideEndLabels = this.GetExpressionKey(mapColorScale.HideEndLabels.ToString());
            colorScaleExp.LabelBehaviour = this.GetExpressionKey(mapColorScale.LabelBehaviour.ToString());
            colorScaleExp.LabelFormat = this.GetExpressionKey(mapColorScale.LabelFormat);
            colorScaleExp.LabelPlacement = this.GetExpressionKey(mapColorScale.LabelPlacement.ToString());
            colorScaleExp.LabelInterval = this.GetExpressionKey(mapColorScale.LabelInterval.ToString());
            colorScaleExp.NoDataText = this.GetExpressionKey(mapColorScale.NoDataText);
            colorScaleExp.Position = this.GetExpressionKey(mapColorScale.Position.ToString());
            colorScaleExp.RangeGapColor = this.GetExpressionKey(mapColorScale.RangeGapColor);
            colorScaleExp.ToolTip = this.GetExpressionKey(mapColorScale.ToolTip);
            colorScaleExp.ZIndex = this.GetExpressionKey(mapColorScale.ZIndex.ToString());

            return colorScaleExp;
        }

        private MapDistanceScaleExp GetMapDistanceScaleExp(MapDistanceScale mapDistanceScale)
        {
            MapDistanceScaleExp distanceScaleExp = new MapDistanceScaleExp();

            if (mapDistanceScale.Style != null)
            {
                distanceScaleExp.Border = GetBorderExp(mapDistanceScale.Style.Border);
                distanceScaleExp.BackGroundExp = GetBackgroundExp(mapDistanceScale.Style);
                distanceScaleExp.Style = GetFontStyleExp(mapDistanceScale.Style);
            }
            if (mapDistanceScale.MapLocation != null)
            {
                distanceScaleExp.MapLocation = GetMapLocationExp(mapDistanceScale.MapLocation);
            }
            if (mapDistanceScale.MapSize != null)
            {
                distanceScaleExp.MapSize = GetMapSizeExp(mapDistanceScale.MapSize);
            }
            if (mapDistanceScale.BottomMargin != null)
            {
                distanceScaleExp.BottomMargin = this.GetExpressionKey(mapDistanceScale.BottomMargin.size);
            }
            if (mapDistanceScale.TopMargin != null)
            {
                distanceScaleExp.TopMargin = this.GetExpressionKey(mapDistanceScale.TopMargin.size);
            }
            if (mapDistanceScale.RightMargin != null)
            {
                distanceScaleExp.RightMargin = this.GetExpressionKey(mapDistanceScale.RightMargin.size);
            }
            if (mapDistanceScale.LeftMargin != null)
            {
                distanceScaleExp.LeftMargin = this.GetExpressionKey(mapDistanceScale.LeftMargin.size);
            }
            if (mapDistanceScale.ActionInfo != null)
            {
                distanceScaleExp.ActionInfo = GetActionInfoExp(mapDistanceScale.ActionInfo);
            }

            distanceScaleExp.DockOutsideViewport = this.GetExpressionKey(mapDistanceScale.DockOutsideViewport.ToString());
            distanceScaleExp.Hidden = this.GetExpressionKey(mapDistanceScale.Hidden.ToString());
            distanceScaleExp.ScaleBorderColor = this.GetExpressionKey(mapDistanceScale.ScaleBorderColor);
            distanceScaleExp.Position = this.GetExpressionKey(mapDistanceScale.Position.ToString());
            distanceScaleExp.ScaleColor = this.GetExpressionKey(mapDistanceScale.ScaleColor);
            distanceScaleExp.ToolTip = this.GetExpressionKey(mapDistanceScale.ToolTip);
            distanceScaleExp.ZIndex = this.GetExpressionKey(mapDistanceScale.ZIndex.ToString());

            return distanceScaleExp;
        }

        private MapSizeExp GetMapSizeExp(MapSize mapSize)
        {
            MapSizeExp sizeExp = new MapSizeExp();
            sizeExp.Height = this.GetExpressionKey(mapSize.Height.ToString());
            sizeExp.Width = this.GetExpressionKey(mapSize.Width.ToString());
            sizeExp.Unit = this.GetExpressionKey(mapSize.Unit.ToString());
            return sizeExp;
        }

        private void MapLayersExp(MapLayer mapLayer)
        {
            MapLayerExp layerproperty = null;

            if (!(mapLayer is MapTileLayer))
            {
                layerproperty = GetMapLayerExp(mapLayer);
            }
            if (mapLayer is MapPolygonLayer)
            {
                if (this.mapPropertiesExp.MapPolygonLayersExp == null)
                {
                    this.mapPropertiesExp.MapPolygonLayersExp = new List<MapPolygonLayerExp>();
                }

                var polygon = GetMapPolygonLayerExp(mapLayer as MapPolygonLayer);
                polygon.MapLayerExp = layerproperty;
                this.mapPropertiesExp.MapPolygonLayersExp.Add(polygon);
            }
            else if (mapLayer is MapPointLayer)
            {
                if (this.mapPropertiesExp.MapPointLayersExp == null)
                {
                    this.mapPropertiesExp.MapPointLayersExp = new List<MapPointLayerExp>();
                }

                var pointlayer = GetMapPointLayerExp(mapLayer as MapPointLayer);
                pointlayer.MapLayerExp = layerproperty;
                this.mapPropertiesExp.MapPointLayersExp.Add(pointlayer);
            }
            else if(mapLayer is MapLineLayer)
            {
                if (this.mapPropertiesExp.MapLineLayersExp == null)
                {
                    this.mapPropertiesExp.MapLineLayersExp = new List<MapLineLayerExp>();
                }

                var linelayer = GetMapLineLayerExp(mapLayer as MapLineLayer);
                linelayer.MapLayerExp = layerproperty;
                this.mapPropertiesExp.MapLineLayersExp.Add(linelayer);
            }
            else if (mapLayer is MapTileLayer)
            {
                if (this.mapPropertiesExp.MapTileLayersExp == null)
                {
                    this.mapPropertiesExp.MapTileLayersExp = new List<MapTileLayerExp>();
                }

                this.mapPropertiesExp.MapTileLayersExp.Add(GetMapTileLayerExp(mapLayer as MapTileLayer));
            }
        }

        private MapPolygonLayerExp GetMapPolygonLayerExp(MapPolygonLayer polygonLayer)
        {
            MapPolygonLayerExp mapPolygonLayer = new MapPolygonLayerExp();
            try
            {
                if (polygonLayer.MapSpatialData != null)
                {
                    mapPolygonLayer.MapSpatialData = GetMapSpatialDataExp(polygonLayer.MapSpatialData);
                }
                if (polygonLayer.MapBindingFieldPairs != null && polygonLayer.MapBindingFieldPairs.Count > 0)
                {
                    mapPolygonLayer.MapBindingFieldPairs = GetMapBindingFieldExp(polygonLayer.MapBindingFieldPairs);
                }
                if (polygonLayer.MapCenterPointRules != null)
                {
                    mapPolygonLayer.MapCenterPointRules = new MapPointRulesExp();
                    if (polygonLayer.MapCenterPointRules.MapColorRule != null)
                    {
                        mapPolygonLayer.MapCenterPointRules.MapColorRule = GetMapColorRuleExp(polygonLayer.MapCenterPointRules.MapColorRule);

                        if (!string.IsNullOrEmpty(mapPolygonLayer.MapCenterPointRules.MapColorRule.DataValue))
                        {
                            if (this.MapEngineInfo.MapColorLables == null)
                            {
                                this.MapEngineInfo.MapColorLables = new List<string>();
                            }
                            this.MapEngineInfo.MapColorLables.Add(this.GetExpression(mapPolygonLayer.MapCenterPointRules.MapColorRule.DataValue));                            
                        }
                    }
                    if (polygonLayer.MapCenterPointRules.MapMarkerRule != null)
                    {
                        mapPolygonLayer.MapCenterPointRules.MapMarkerRule = GetMapMarkerRuleExp(polygonLayer.MapCenterPointRules.MapMarkerRule);
                    }
                    if (polygonLayer.MapCenterPointRules.MapSizeRule != null)
                    {
                        mapPolygonLayer.MapCenterPointRules.MapSizeRule = GetMapSizeRuleExp(polygonLayer.MapCenterPointRules.MapSizeRule);
                    }
                }
                if (polygonLayer.MapCenterPointTemplate != null)
                {
                    mapPolygonLayer.MapCenterPointTemplate = GetMapPointTemplateExp(polygonLayer.MapCenterPointTemplate);
                }
                if (polygonLayer.MapPolygonTemplate != null)
                {
                    mapPolygonLayer.MapPolygonTemplate = GetMapPolygonTemplateExp(polygonLayer.MapPolygonTemplate);
                }
                if (polygonLayer.MapFieldDefinitions != null)
                {
                    mapPolygonLayer.MapFieldDefinitions = GetMapFieldDefExp(polygonLayer.MapFieldDefinitions);
                }
                if (polygonLayer.MapPolygonRules != null)
                {
                    mapPolygonLayer.MapPolygonRules = new MapPolygonRulesExp();
                    if (polygonLayer.MapPolygonRules.MapColorRule != null)
                    {
                        mapPolygonLayer.MapPolygonRules.MapColorRule = GetMapColorRuleExp(polygonLayer.MapPolygonRules.MapColorRule);
                        if (!string.IsNullOrEmpty(mapPolygonLayer.MapPolygonRules.MapColorRule.DataValue))
                        {
                            if (this.MapEngineInfo.MapColorRule == null)
                            {
                                this.MapEngineInfo.MapColorRule = new List<string>();
                            }
                            this.MapEngineInfo.MapColorRule.Add(this.GetExpression(mapPolygonLayer.MapPolygonRules.MapColorRule.DataValue));
                        }

                    }
                }
                if (polygonLayer.MapPolygons != null)
                {
                    mapPolygonLayer.MapPolygons = new List<MapPolygonExp>();
                    foreach (var polygon in polygonLayer.MapPolygons)
                    {
                        MapPolygonExp polygonExp = new MapPolygonExp();
                        if (polygon.MapCenterPointTemplate != null)
                        {
                            polygonExp.MapCenterPointTemplate = GetMapPointTemplateExp(polygon.MapCenterPointTemplate);
                        }
                        if (polygon.MapFields != null)
                        {
                            polygonExp.MapFields = GetMapFieldsExp(polygon.MapFields);
                        }
                        if (polygon.MapPolygonTemplate != null)
                        {
                            polygonExp.MapPolygonTemplate = GetMapPolygonTemplateExp(polygon.MapPolygonTemplate);
                        }
                        polygonExp.UseCustomCenterPointTemplate = this.GetExpressionKey(polygon.UseCustomCenterPointTemplate.ToString());
                        polygonExp.UseCustomPolygonTemplate = this.GetExpressionKey(polygon.UseCustomPolygonTemplate.ToString());
                        polygonExp.VectorData = this.GetExpressionKey(polygon.VectorData);
                        mapPolygonLayer.MapPolygons.Add(polygonExp);
                    }
                }
                mapPolygonLayer.DataElementName = this.GetExpressionKey(polygonLayer.DataElementName);
                mapPolygonLayer.MapDataRegionName = this.GetExpressionKey(polygonLayer.MapDataRegionName);
                mapPolygonLayer.DataElementOutput = this.GetExpressionKey(polygonLayer.DataElementOutput.ToString());

            }
            catch { }

            return mapPolygonLayer;
        }

        private MapPolygonTemplateExp GetMapPolygonTemplateExp(MapPolygonTemplate mapPolygonTemplate)
        {
            MapPolygonTemplateExp template = new MapPolygonTemplateExp();

            if (mapPolygonTemplate.Style != null)
            {
                if (mapPolygonTemplate.Style.Border != null)
                    template.Border = GetBorderExp(mapPolygonTemplate.Style.Border);
                template.Background = GetBackgroundExp(mapPolygonTemplate.Style);
                template.StyleExp = GetFontStyleExp(mapPolygonTemplate.Style);
            }
            if (mapPolygonTemplate.ActionInfo != null)
            {
                template.ActionInfo = GetActionInfoExp(mapPolygonTemplate.ActionInfo , true);
                this.MapEngineInfo.MapShapeActions = template.ActionInfo;
            }

            template.CenterPointOffsetX = this.GetExpressionKey(mapPolygonTemplate.CenterPointOffsetX.ToString());
            template.CenterPointOffsetY = this.GetExpressionKey(mapPolygonTemplate.CenterPointOffsetY.ToString());
            template.DataElementLabel = this.GetExpressionKey(mapPolygonTemplate.DataElementLabel);
            template.DataElementName = this.GetExpressionKey(mapPolygonTemplate.DataElementName);
            template.DataElementOutput = this.GetExpressionKey(mapPolygonTemplate.DataElementOutput.ToString());
            template.Hidden = this.GetExpressionKey(mapPolygonTemplate.Hidden.ToString());
            //template.Label = this.GetExpressionKey(mapPolygonTemplate.Label);
            template.Label = mapPolygonTemplate.Label;

            if (this.MapEngineInfo.MapDisplayLables == null)
            {
                this.MapEngineInfo.MapDisplayLables = new List<string>();                
            }
            this.MapEngineInfo.MapDisplayLables.Add(this.GetExpression(template.Label));

            template.LabelPlacement = this.GetExpressionKey(mapPolygonTemplate.LabelPlacement.ToString());
            template.OffsetX = this.GetExpressionKey(mapPolygonTemplate.OffsetX.ToString());
            template.OffsetY = this.GetExpressionKey(mapPolygonTemplate.OffsetY.ToString());
            template.ScaleFactor = this.GetExpressionKey(mapPolygonTemplate.ScaleFactor.ToString());
            template.ShowLabel = this.GetExpressionKey(mapPolygonTemplate.ShowLabel.ToString());
            template.ToolTip = this.GetExpressionKey(mapPolygonTemplate.ToolTip);

            return template;
        }

        private MapTileLayerExp GetMapTileLayerExp(MapTileLayer mapTileLayer)
        {
            MapTileLayerExp layer = new MapTileLayerExp();
            if (mapTileLayer.MapTiles != null)
            {
                layer.MapTiles = new List<MapTileExp>();
                foreach (var tile in mapTileLayer.MapTiles)
                {
                    MapTileExp mapTile = new ItemModel.MapTileExp();
                    mapTile.MIMEType = this.GetExpressionKey(tile.MIMEType);
                    mapTile.Name = this.GetExpressionKey(tile.Name);
                    mapTile.TileData = this.GetExpressionKey(tile.TileData);
                    layer.MapTiles.Add(mapTile);
                }
            }

            layer.TileStyle = this.GetExpressionKey(mapTileLayer.TileStyle.ToString());
            layer.Name = this.GetExpressionKey(mapTileLayer.Name);
            layer.Transparency = this.GetExpressionKey(mapTileLayer.Transparency.ToString());
            layer.VisibilityMode = this.GetExpressionKey(mapTileLayer.VisibilityMode.ToString());
            layer.MinimumZoom = this.GetExpressionKey(mapTileLayer.MinimumZoom.ToString());
            layer.MaximumZoom = this.GetExpressionKey(mapTileLayer.MaximumZoom.ToString());

            return layer;
        }

        private MapLineLayerExp GetMapLineLayerExp(MapLineLayer lineLayer)
        {
            MapLineLayerExp maplineLayer = new MapLineLayerExp();
            
            if (lineLayer.MapSpatialData != null)
            {
                maplineLayer.MapSpatialData = GetMapSpatialDataExp(lineLayer.MapSpatialData);
            }
            if (lineLayer.MapFieldDefinitions != null)
            {
                maplineLayer.MapFieldDefinitions = GetMapFieldDefExp(lineLayer.MapFieldDefinitions);
            }
            if (lineLayer.MapBindingFieldPairs != null)
            {
                maplineLayer.MapBindingFieldPairs = GetMapBindingFieldExp(lineLayer.MapBindingFieldPairs);
            }
            if (lineLayer.MapLineRules != null)
            {
                maplineLayer.MapLineRules = new MapLineRulesExp();

                if (lineLayer.MapLineRules.MapColorRule != null)
                {
                    maplineLayer.MapLineRules.MapColorRule = GetMapColorRuleExp(lineLayer.MapLineRules.MapColorRule);
                }
                if (lineLayer.MapLineRules.MapSizeRule != null)
                {
                    maplineLayer.MapLineRules.MapSizeRule = GetMapSizeRuleExp(lineLayer.MapLineRules.MapSizeRule);
                }
            }
            if (lineLayer.MapLines != null)
            {
                maplineLayer.MapLines = new List<MapLineExp>();
                foreach (var lines in lineLayer.MapLines)
                {
                    MapLineExp lineExpVal = new MapLineExp();
                    if (lines.MapFields != null)
                    {
                        lineExpVal.MapFields = GetMapFieldsExp(lines.MapFields);
                    }
                    if (lines.MapLineTemplate != null)
                    {
                        lineExpVal.MapLineTemplate = GetMapLineTemplateExp(lines.MapLineTemplate);    
                    }

                    lineExpVal.UseCustomLineTemplate = this.GetExpressionKey(lines.UseCustomLineTemplate.ToString());
                    lineExpVal.VectorData = this.GetExpressionKey(lines.VectorData);
                    maplineLayer.MapLines.Add(lineExpVal);
                }
            }
            if (lineLayer.MapLineTemplate != null)
            {
                maplineLayer.MapLineTemplate = GetMapLineTemplateExp(lineLayer.MapLineTemplate);
            }

            maplineLayer.MapDataRegionName = this.GetExpressionKey(lineLayer.MapDataRegionName);
            maplineLayer.DataElementName = this.GetExpressionKey(lineLayer.DataElementName);
            maplineLayer.DataElementOutput = this.GetExpressionKey(lineLayer.DataElementOutput.ToString());

            return maplineLayer;
        }

        private MapLineTemplateExp GetMapLineTemplateExp(MapLineTemplate mapLineTemplate)
        {
            MapLineTemplateExp template = new MapLineTemplateExp();

            if (mapLineTemplate.Style != null)
            {
                if (mapLineTemplate.Style.Border != null)
                    template.Border = GetBorderExp(mapLineTemplate.Style.Border);
                template.StyleExp = GetFontStyleExp(mapLineTemplate.Style);
            }
            if (mapLineTemplate.ActionInfo != null)
            {
                template.ActionInfo = GetActionInfoExp(mapLineTemplate.ActionInfo);
            }
            if (mapLineTemplate.Width != null)
            {
                template.Width = this.GetExpressionKey(mapLineTemplate.Width.size);
            }

            template.DataElementLabel = this.GetExpressionKey(mapLineTemplate.DataElementLabel);
            template.DataElementName = this.GetExpressionKey(mapLineTemplate.DataElementName);
            template.DataElementOutput = this.GetExpressionKey(mapLineTemplate.DataElementOutput.ToString());
            template.Hidden = this.GetExpressionKey(mapLineTemplate.Hidden.ToString());
            template.Label = this.GetExpressionKey(mapLineTemplate.Label);
            template.LabelPlacement = this.GetExpressionKey(mapLineTemplate.LabelPlacement.ToString());
            template.OffsetX = this.GetExpressionKey(mapLineTemplate.OffsetX.ToString());
            template.OffsetY = this.GetExpressionKey(mapLineTemplate.OffsetY.ToString());
            template.ToolTip = this.GetExpressionKey(mapLineTemplate.ToolTip);

            return template;
        }

        private MapPointLayerExp GetMapPointLayerExp(MapPointLayer pointLayer)
        {
            MapPointLayerExp mapPointLayer = new MapPointLayerExp();

            if (pointLayer.MapBindingFieldPairs != null)
            {
                mapPointLayer.MapBindingFieldPairs = GetMapBindingFieldExp(pointLayer.MapBindingFieldPairs);
            }
            if (pointLayer.MapFieldDefinitions != null)
            {
                mapPointLayer.MapFieldDefinitions = GetMapFieldDefExp(pointLayer.MapFieldDefinitions);
            }
            if (pointLayer.MapPointRules != null)
            {
                mapPointLayer.MapPointRules = new MapPointRulesExp();

                if (pointLayer.MapPointRules.MapColorRule != null)
                {
                    mapPointLayer.MapPointRules.MapColorRule = GetMapColorRuleExp(pointLayer.MapPointRules.MapColorRule);
                }
                if (pointLayer.MapPointRules.MapMarkerRule != null)
                {
                    mapPointLayer.MapPointRules.MapMarkerRule = GetMapMarkerRuleExp(pointLayer.MapPointRules.MapMarkerRule);
                }
                if (pointLayer.MapPointRules.MapSizeRule != null)
                {
                    mapPointLayer.MapPointRules.MapSizeRule = GetMapSizeRuleExp(pointLayer.MapPointRules.MapSizeRule);
                }
            }
            if (pointLayer.MapPoints != null)
            {
                mapPointLayer.MapPoints = new List<MapPointExp>();
                foreach (var point in pointLayer.MapPoints)
                {
                    MapPointExp pt = new MapPointExp();
                    pt.VectorData = this.GetExpressionKey(point.VectorData);
                    pt.UseCustomPointTemplate = this.GetExpressionKey(point.UseCustomPointTemplate.ToString());

                    if (point.MapPointTemplate != null)
                    {
                        pt.MapPointTemplate = GetMapPointTemplateExp(point.MapPointTemplate);
                    }
                    if (point.MapFields != null)
                    {
                        pt.MapFields = GetMapFieldsExp(point.MapFields);
                    }
                    mapPointLayer.MapPoints.Add(pt);
                }
            }
            if (pointLayer.MapPointTemplate != null)
            {
                mapPointLayer.MapPointTemplate = GetMapPointTemplateExp(pointLayer.MapPointTemplate);
            }
            if (pointLayer.MapSpatialData != null)
            {
                mapPointLayer.MapSpatialData = GetMapSpatialDataExp(pointLayer.MapSpatialData);
            }

            mapPointLayer.MapDataRegionName = this.GetExpressionKey(pointLayer.MapDataRegionName);
            mapPointLayer.DataElementName = this.GetExpressionKey(pointLayer.DataElementName);
            mapPointLayer.DataElementOutput = this.GetExpressionKey(pointLayer.DataElementOutput.ToString());

            return mapPointLayer;
        }

        private MapPointTemplateExp GetMapPointTemplateExp(MapPointTemplate mapPointTemplate)
        {
            MapPointTemplateExp template = new MapPointTemplateExp();

            if (mapPointTemplate.Style != null)
            {
                if (mapPointTemplate.Style.Border != null)
                    template.Border = GetBorderExp(mapPointTemplate.Style.Border);
                template.StyleExp = GetFontStyleExp(mapPointTemplate.Style);
            }
            if (mapPointTemplate.ActionInfo != null)
            {
                template.ActionInfo = GetActionInfoExp(mapPointTemplate.ActionInfo);
            }
            if (mapPointTemplate.Size != null)
            {
                template.Size = this.GetExpressionKey(mapPointTemplate.Size.size);
            }

            template.DataElementLabel = this.GetExpressionKey(mapPointTemplate.DataElementLabel);
            template.DataElementName = this.GetExpressionKey(mapPointTemplate.DataElementName);
            template.DataElementOutput = this.GetExpressionKey(mapPointTemplate.DataElementOutput.ToString());
            template.Hidden = this.GetExpressionKey(mapPointTemplate.Hidden.ToString());
            template.Label = this.GetExpressionKey(mapPointTemplate.Label);
            template.LabelPlacement = this.GetExpressionKey(mapPointTemplate.LabelPlacement.ToString());
            template.OffsetX = this.GetExpressionKey(mapPointTemplate.OffsetX.ToString());
            template.OffsetY = this.GetExpressionKey(mapPointTemplate.OffsetY.ToString());
            template.ToolTip = this.GetExpressionKey(mapPointTemplate.ToolTip);

            return template;
        }

        private MapSizeRuleExp GetMapSizeRuleExp(MapSizeRule mapSizeRule)
        {
            MapSizeRuleExp sizeRule = new MapSizeRuleExp();
            sizeRule.BucketCount = this.GetExpressionKey(mapSizeRule.BucketCount.ToString());
            sizeRule.DataElementName = this.GetExpressionKey(mapSizeRule.DataElementName);
            sizeRule.DataElementOutput = this.GetExpressionKey(mapSizeRule.DataElementOutput.ToString());
            //sizeRule.DataValue = this.GetExpressionKey(mapSizeRule.DataValue);
            sizeRule.DataValue = mapSizeRule.DataValue;

            if (this.MapEngineInfo.MapBubbleLables == null)
            {
                this.MapEngineInfo.MapBubbleLables = new List<string>();
            }
            this.MapEngineInfo.MapBubbleLables.Add(this.GetExpression(mapSizeRule.DataValue));

            sizeRule.DistributionType = this.GetExpressionKey(mapSizeRule.DistributionType.ToString());
            sizeRule.EndValue = this.GetExpressionKey(mapSizeRule.EndValue);
            if (mapSizeRule.StartSize != null)
            {
                sizeRule.StartSize = this.GetExpressionKey(mapSizeRule.StartSize.size);
            }
            if (mapSizeRule.EndSize != null)
            {
                sizeRule.EndSize = this.GetExpressionKey(mapSizeRule.EndSize.size);
            }

            return sizeRule;
        }

        private MapMarkerRuleExp GetMapMarkerRuleExp(MapMarkerRule mapMarkerRule)
        {
            MapMarkerRuleExp marker = new MapMarkerRuleExp();
            marker.BucketCount = this.GetExpressionKey(mapMarkerRule.BucketCount.ToString());
            marker.DataElementName = this.GetExpressionKey(mapMarkerRule.DataElementName);
            marker.DataElementOutput = this.GetExpressionKey(mapMarkerRule.DataElementOutput.ToString());
            //marker.DataValue = this.GetExpressionKey(mapMarkerRule.DataValue);
            marker.DataValue = mapMarkerRule.DataValue;
            marker.DistributionType = this.GetExpressionKey(mapMarkerRule.DistributionType.ToString());
            marker.EndValue = this.GetExpressionKey(mapMarkerRule.EndValue);
            return marker;
        }

        private MapColorRuleExp GetMapColorRuleExp(MapColorRule mapColorRule)
        {
            MapColorRuleExp colorRule = new MapColorRuleExp();
            if (mapColorRule is MapColorRangeRule)
            {
                var rangeRule = mapColorRule as MapColorRangeRule;
                if (rangeRule != null)
                {
                    colorRule.MapColorRangeRule = new MapColorRangeRuleExp();
                    colorRule.MapColorRangeRule.StartColor = this.GetExpressionKey(rangeRule.StartColor);
                    colorRule.MapColorRangeRule.EndColor = this.GetExpressionKey(rangeRule.EndColor);
                    colorRule.MapColorRangeRule.MiddleColor = this.GetExpressionKey(rangeRule.MiddleColor);
                }
            }
            else if (mapColorRule is MapColorPaletteRule)
            {
                var paletteRule = mapColorRule as MapColorPaletteRule;
                if (paletteRule != null)
                {
                    colorRule.ColorPalette = new MapColorPaletteRuleExp();
                    colorRule.ColorPalette.Palette = this.GetExpressionKey(paletteRule.Palette.ToString());
                }
            }
            else if(mapColorRule is MapCustomColorRule)
            {
                var customColor = mapColorRule as MapCustomColorRule;
                if (customColor != null && customColor.MapCustomColors != null && customColor.MapCustomColors.MapCustomColor != null)
                {
                    colorRule.MapCustomColors = new List<string>();
                    foreach (var color in customColor.MapCustomColors.MapCustomColor)
                    {
                        colorRule.MapCustomColors.Add(this.GetExpressionKey(color));
                    }
                }
            }

            colorRule.BucketCount = this.GetExpressionKey(mapColorRule.BucketCount.ToString());
            colorRule.DataElementName = this.GetExpressionKey(mapColorRule.DataElementName);
            colorRule.DataElementOutput = this.GetExpressionKey(mapColorRule.DataElementOutput.ToString());
            //colorRule.DataValue = this.GetExpressionKey(mapColorRule.DataValue);
            colorRule.DataValue = mapColorRule.DataValue;

            if (!string.IsNullOrEmpty(mapColorRule.DataValue))
            {
                if (this.MapEngineInfo.MapColorLables == null)
                {
                    this.MapEngineInfo.MapColorLables = new List<string>();
                }
                this.MapEngineInfo.MapColorLables.Add(this.GetExpression(mapColorRule.DataValue));
            }

            colorRule.DistributionType = this.GetExpressionKey(mapColorRule.DistributionType.ToString());
            colorRule.EndValue = this.GetExpressionKey(mapColorRule.EndValue);
            colorRule.StartValue = this.GetExpressionKey(mapColorRule.StartValue);
            colorRule.LegendName = this.GetExpressionKey(mapColorRule.LegendName);
            colorRule.LegendText = this.GetExpressionKey(mapColorRule.LegendText);
            
            return colorRule;
        }

        private List<MapFieldExp> GetMapFieldsExp(MapFields mapFields)
        {
            List<MapFieldExp> fieldcoll = new List<MapFieldExp>();
            foreach (var field in mapFields)
            {
                MapFieldExp fieldval = new MapFieldExp();
                fieldval.Name = this.Model.ExpressionEngine.GetEvalExpressionString(field.Name);
                fieldval.Value = this.Model.ExpressionEngine.GetEvalExpressionString(field.Value);
                fieldcoll.Add(fieldval);
            }
            return fieldcoll;
        }

        private List<MapFieldDefinitionExp> GetMapFieldDefExp(MapFieldDefinitions mapFieldDefinitions)
        {
            List<MapFieldDefinitionExp> fielddefinition = new List<MapFieldDefinitionExp>();
            foreach (var defi in mapFieldDefinitions)
            {
                MapFieldDefinitionExp df = new MapFieldDefinitionExp();
                df.DataType = this.GetExpressionKey(defi.DataType.ToString());
                df.Name = this.GetExpressionKey(defi.Name);
                fielddefinition.Add(df);
            }
            return fielddefinition;
        }

        private List<MapBindingFieldPairExp> GetMapBindingFieldExp(MapBindingFieldPairs mapBindingFieldPairs)
        {
            List<MapBindingFieldPairExp> fieldpairs = new List<MapBindingFieldPairExp>();
            foreach (var field in mapBindingFieldPairs)
            {
                MapBindingFieldPairExp pair = new MapBindingFieldPairExp();
                //pair.BindingExpression = this.Model.ExpressionEngine.GetEvalExpressionString(field.BindingExpression);
                //pair.FieldName = this.Model.ExpressionEngine.GetEvalExpressionString(field.FieldName);
                pair.BindingExpression = field.BindingExpression;
                pair.FieldName = field.FieldName;
                fieldpairs.Add(pair);

                if (this.MapEngineInfo.MapFieldValues == null)
                {
                    this.MapEngineInfo.MapFieldValues = new List<string>();                    
                }
                this.MapEngineInfo.MapFieldValues.Add(this.GetExpression(pair.BindingExpression));
            }

            return fieldpairs;
        }

        private MapSpatialDataExp GetMapSpatialDataExp(MapSpatialData mapSpatialData)
        {
            MapSpatialDataExp spatialDataExp = new MapSpatialDataExp();

            if ((mapSpatialData as MapShapefile) != null)
            {
                var shapefile = (mapSpatialData as MapShapefile);
                spatialDataExp.MapShapefile = new MapShapefileExp();

                if (shapefile.MapFieldNames != null && shapefile.MapFieldNames.MapFieldName != null && shapefile.MapFieldNames.MapFieldName.Count > 0)
                {
                    spatialDataExp.MapShapefile.MapFieldNames = new List<string>();

                    foreach (var field in shapefile.MapFieldNames.MapFieldName)
                    {
                        spatialDataExp.MapShapefile.MapFieldNames.Add(this.GetExpressionKey(field));
                    }
                }

                spatialDataExp.MapShapefile.Source = this.GetExpressionKey(shapefile.Source);
            }
            if ((mapSpatialData as MapSpatialDataRegion) != null)
            {
                spatialDataExp.MapSpatialDataRegionExp = new MapSpatialDataRegionExp();
                spatialDataExp.MapSpatialDataRegionExp.VectorData = this.GetExpressionKey((mapSpatialData as MapSpatialDataRegion).VectorData);
            }
            if ((mapSpatialData as MapSpatialDataSet) != null)
            {
                var spatialDataSet = (mapSpatialData as MapSpatialDataSet);
                spatialDataExp.MapSpatialDataSetExp = new MapSpatialDataSetExp();

                if (spatialDataSet.MapFieldNames != null && spatialDataSet.MapFieldNames.MapFieldName != null && spatialDataSet.MapFieldNames.MapFieldName.Count > 0)
                {
                    spatialDataExp.MapSpatialDataSetExp.MapFieldNames = new List<string>();
                    foreach (var field in spatialDataSet.MapFieldNames.MapFieldName)
                    {
                        spatialDataExp.MapSpatialDataSetExp.MapFieldNames.Add(this.GetExpressionKey(field));
                    }
                }

                spatialDataExp.MapSpatialDataSetExp.SpatialField = this.GetExpressionKey(spatialDataSet.SpatialField);
                spatialDataExp.MapSpatialDataSetExp.DataSetName = this.GetExpressionKey(spatialDataSet.DataSetName);
            }

            return spatialDataExp;
        }

        private MapLayerExp GetMapLayerExp(MapLayer mapLayer)
        {
            MapLayerExp layer = new MapLayerExp();
            layer.MaximumZoom = this.GetExpressionKey(mapLayer.MaximumZoom.ToString());
            layer.MinimumZoom = this.GetExpressionKey(mapLayer.MinimumZoom.ToString());
            layer.Name = this.GetExpressionKey(mapLayer.Name);
            layer.Transparency = this.GetExpressionKey(mapLayer.Transparency.ToString());
            layer.VisibilityMode = this.GetExpressionKey(mapLayer.VisibilityMode.ToString());
            return layer;
        }

        private MapTitleExp GetMapTitleExp(MapTitle mapTitle)
        {
            MapTitleExp title = new MapTitleExp();
            if (mapTitle.Style != null)
            {
                title.Border = GetBorderExp(mapTitle.Style.Border);
                title.BackGroundExp = GetBackgroundExp(mapTitle.Style);
                title.Style = GetFontStyleExp(mapTitle.Style);
            }
            if (mapTitle.MapLocation != null)
            {
                title.MapLocation = GetMapLocationExp(mapTitle.MapLocation);
            }
            if (mapTitle.MapSize != null)
            {
                title.MapSize = new MapSizeExp();
                title.MapSize.Height = this.GetExpressionKey(mapTitle.MapSize.Height.ToString());
                title.MapSize.Width = this.GetExpressionKey(mapTitle.MapSize.Width.ToString());
                title.MapSize.Unit = this.GetExpressionKey(mapTitle.MapSize.Unit.ToString());
            }
            if (mapTitle.BottomMargin != null)
            {
                title.BottomMargin = this.GetExpressionKey(mapTitle.BottomMargin.size);
            }
            if (mapTitle.TopMargin != null)
            {
                title.TopMargin = this.GetExpressionKey(mapTitle.TopMargin.size);
            }
            if (mapTitle.RightMargin != null)
            {
                title.RightMargin = this.GetExpressionKey(mapTitle.RightMargin.size);
            }
            if (mapTitle.LeftMargin != null)
            {
                title.LeftMargin = this.GetExpressionKey(mapTitle.LeftMargin.size);
            }
            if (mapTitle.TextShadowOffset != null)
            {
                title.TextShadowOffset = this.GetExpressionKey(mapTitle.TextShadowOffset.size);
            }
            if (mapTitle.ActionInfo != null)
            {
                title.ActionInfo = GetActionInfoExp(mapTitle.ActionInfo);
            }
            
            title.DockOutsideViewport = this.GetExpressionKey(mapTitle.DockOutsideViewport.ToString());
            title.Hidden = this.GetExpressionKey(mapTitle.Hidden.ToString());
            title.Angle = this.GetExpressionKey(mapTitle.Angle.ToString());
            title.Name = this.GetExpressionKey(mapTitle.Name);
            title.Position = this.GetExpressionKey(mapTitle.Position.ToString());
            title.Text = this.GetExpressionKey(mapTitle.Text);
            title.ToolTip = this.GetExpressionKey(mapTitle.ToolTip);
            title.ZIndex = this.GetExpressionKey(mapTitle.ZIndex.ToString());

            return title;
        }

        private MapLocationExp GetMapLocationExp(MapLocation mapLocation)
        {
            MapLocationExp location = new MapLocationExp();
            location.Left = this.GetExpressionKey(mapLocation.Left.ToString());
            location.Top = this.GetExpressionKey(mapLocation.Top.ToString());
            location.Unit = this.GetExpressionKey(mapLocation.Unit.ToString());
            return location;
        }

        private MapLegendExp GetMapLegendExp(MapLegend maplegend)
        {
            MapLegendExp legend = new MapLegendExp();

            if (maplegend.Style != null)
            {
                legend.Border = GetBorderExp(maplegend.Style.Border);
                legend.BackGroundExp = GetBackgroundExp(maplegend.Style);
                legend.Style = GetFontStyleExp(maplegend.Style);
            }
            if (maplegend.MapLocation != null)
            {
                legend.MapLocation = GetMapLocationExp(maplegend.MapLocation);
            }
            if (maplegend.MapSize != null)
            {
                legend.MapSize = GetMapSizeExp(maplegend.MapSize);
            }
            if (maplegend.MapLegendTitle != null)
            {
                legend.MapLegendTitle = new MapLegendTitleExp();

                if (maplegend.MapLegendTitle.Style != null)
                {
                    legend.MapLegendTitle.BackGroundExp = GetBackgroundExp(maplegend.MapLegendTitle.Style);
                    legend.MapLegendTitle.Style = GetFontStyleExp(maplegend.MapLegendTitle.Style);
                }

                legend.MapLegendTitle.Caption = this.GetExpressionKey(maplegend.MapLegendTitle.Caption);
                legend.MapLegendTitle.TitleSeparator = this.GetExpressionKey(maplegend.MapLegendTitle.TitleSeparator.ToString());
                legend.MapLegendTitle.TitleSeparatorColor = this.GetExpressionKey(maplegend.MapLegendTitle.TitleSeparatorColor);
            }
            if (maplegend.BottomMargin != null)
            {
                legend.BottomMargin = this.GetExpressionKey(maplegend.BottomMargin.size);
            }
            if (maplegend.TopMargin != null)
            {
                legend.TopMargin = this.GetExpressionKey(maplegend.TopMargin.size);
            }
            if (maplegend.RightMargin != null)
            {
                legend.RightMargin = this.GetExpressionKey(maplegend.RightMargin.size);
            }
            if (maplegend.LeftMargin != null)
            {
                legend.LeftMargin = this.GetExpressionKey(maplegend.LeftMargin.size);
            }
            if (maplegend.ActionInfo != null)
            {
                legend.ActionInfo = GetActionInfoExp(maplegend.ActionInfo);
            }
            if (maplegend.MinFontSize!=null)
            {
                legend.MinFontSize = this.GetExpressionKey(maplegend.MinFontSize.size);
            }

            legend.AutoFitTextDisabled = this.GetExpressionKey(maplegend.AutoFitTextDisabled.ToString());
            legend.DockOutsideViewport = this.GetExpressionKey(maplegend.DockOutsideViewport.ToString());
            legend.EquallySpacedItems = this.GetExpressionKey(maplegend.EquallySpacedItems.ToString());
            legend.Hidden = this.GetExpressionKey(maplegend.Hidden.ToString());
            legend.InterlacedRows = this.GetExpressionKey(maplegend.InterlacedRows.ToString());
            legend.InterlacedRowsColor = this.GetExpressionKey(maplegend.InterlacedRowsColor);
            legend.Layout = this.GetExpressionKey(maplegend.Layout.ToString());
            legend.Name = this.GetExpressionKey(maplegend.Name);
            legend.Position = this.GetExpressionKey(maplegend.Position.ToString());
            legend.TextWrapThreshold = this.GetExpressionKey(maplegend.TextWrapThreshold.ToString());
            legend.ToolTip = this.GetExpressionKey(maplegend.ToolTip);
            legend.ZIndex = this.GetExpressionKey(maplegend.ZIndex.ToString());

            return legend;
        }

        private MapViewportExp GetMapViewportExp(MapViewport mapViewport)
        {
            MapViewportExp Viewport = new MapViewportExp();
            
            if (mapViewport.Style != null)
            {
                Viewport.Border = GetBorderExp(mapViewport.Style.Border);
                Viewport.BackGroundExp = GetBackgroundExp(mapViewport.Style);
                Viewport.Style = GetFontStyleExp(mapViewport.Style);
            }

            if (mapViewport.MapMeridians != null)
            {
                Viewport.MapMeridians = new MapGridLinesExp();

                if (mapViewport.MapMeridians.Style != null)
                {
                    Viewport.MapMeridians.Border = GetBorderExp(mapViewport.MapMeridians.Style.Border);
                    Viewport.MapMeridians.BackGroundExp = GetBackgroundExp(mapViewport.MapMeridians.Style);
                    Viewport.MapMeridians.Style = GetFontStyleExp(mapViewport.MapMeridians.Style);
                }

                Viewport.MapMeridians.Hidden = this.GetExpressionKey(mapViewport.MapMeridians.Hidden.ToString());
                Viewport.MapMeridians.Interval = this.GetExpressionKey(mapViewport.MapMeridians.Interval.ToString());
                Viewport.MapMeridians.LabelPosition = this.GetExpressionKey(mapViewport.MapMeridians.LabelPosition.ToString());
                Viewport.MapMeridians.ShowLabels = this.GetExpressionKey(mapViewport.MapMeridians.ShowLabels.ToString());
            }
            if (mapViewport.MapLimits != null)
            {
                Viewport.MapLimits = new MapLimitsExp();
                Viewport.MapLimits.MaximumX = this.GetExpressionKey(mapViewport.MapLimits.MaximumX.ToString());
                Viewport.MapLimits.MaximumY = this.GetExpressionKey(mapViewport.MapLimits.MaximumY.ToString());
                Viewport.MapLimits.MinimumX = this.GetExpressionKey(mapViewport.MapLimits.MinimumX.ToString());
                Viewport.MapLimits.MinimumY = this.GetExpressionKey(mapViewport.MapLimits.MinimumY.ToString());
            } 
            if (mapViewport.MapLocation != null)
            {
                Viewport.MapLocation = GetMapLocationExp(mapViewport.MapLocation);
            }
            if (mapViewport.MapParallels != null)
            {
                Viewport.MapParallels = new MapGridLinesExp();
                
                if (mapViewport.MapParallels.Style != null)
                {
                    Viewport.MapParallels.Border = GetBorderExp(mapViewport.MapParallels.Style.Border);
                    Viewport.MapParallels.BackGroundExp = GetBackgroundExp(mapViewport.MapParallels.Style);
                    Viewport.MapParallels.Style = GetFontStyleExp(mapViewport.MapParallels.Style);
                }

                Viewport.MapParallels.Hidden = this.GetExpressionKey(mapViewport.MapParallels.Hidden.ToString());
                Viewport.MapParallels.Interval = this.GetExpressionKey(mapViewport.MapParallels.Interval.ToString());
                Viewport.MapParallels.LabelPosition = this.GetExpressionKey(mapViewport.MapParallels.LabelPosition.ToString());
                Viewport.MapParallels.ShowLabels = this.GetExpressionKey(mapViewport.MapParallels.ShowLabels.ToString());
            } 
            if (mapViewport.MapSize != null)
            {
                Viewport.MapSize = GetMapSizeExp(mapViewport.MapSize);
            }
            if (mapViewport.MapView != null)
            {
                Viewport.MapView = GetMapViewExp(mapViewport.MapView);
            }

            if (mapViewport.BottomMargin != null)
            {
                Viewport.BottomMargin = this.GetExpressionKey(mapViewport.BottomMargin.size);
            }
            if (mapViewport.TopMargin != null)
            {
                Viewport.TopMargin = this.GetExpressionKey(mapViewport.TopMargin.size);
            }
            if (mapViewport.RightMargin != null)
            {
                Viewport.RightMargin = this.GetExpressionKey(mapViewport.RightMargin.size);
            }
            if (mapViewport.LeftMargin != null)
            {
                Viewport.LeftMargin = this.GetExpressionKey(mapViewport.LeftMargin.size);
            }
            if (mapViewport.ContentMargin != null)
            {
                Viewport.ContentMargin = this.GetExpressionKey(mapViewport.ContentMargin.size);
            }

            Viewport.MaximumZoom = this.GetExpressionKey(mapViewport.MaximumZoom.ToString());
            Viewport.MinimumZoom = this.GetExpressionKey(mapViewport.MinimumZoom.ToString());
            Viewport.GridUnderContent = this.GetExpressionKey(mapViewport.GridUnderContent.ToString());
            Viewport.ProjectionCenterX = this.GetExpressionKey(mapViewport.ProjectionCenterX.ToString());
            Viewport.ProjectionCenterY = this.GetExpressionKey(mapViewport.ProjectionCenterY.ToString());
            Viewport.MapCoordinateSystem = this.GetExpressionKey(mapViewport.MapCoordinateSystem.ToString());
            Viewport.SimplificationResolution = this.GetExpressionKey(mapViewport.SimplificationResolution.ToString());
            Viewport.MapProjection = this.GetExpressionKey(mapViewport.MapProjection.ToString());
            Viewport.ZIndex = this.GetExpressionKey(mapViewport.ZIndex.ToString());

            return Viewport;

        }

        private MapViewExp GetMapViewExp(MapView mapView)
        {
            MapViewExp viewExp = new MapViewExp();
            if (mapView is MapCustomView)
            {
                var customView = (mapView as MapCustomView);
                if (customView != null)
                {
                    viewExp.CenterX = this.GetExpressionKey(customView.CenterX.ToString());
                    viewExp.CenterY = this.GetExpressionKey(customView.CenterY.ToString());
                }
            }
            else if (mapView is MapElementView)
            {
                var elementView = (mapView as MapElementView);
                if (elementView != null)
                {
                    viewExp.LayerName = this.GetExpressionKey(elementView.LayerName);
                    if (elementView.MapBindingFieldPairs != null && elementView.MapBindingFieldPairs.Count > 0)
                        viewExp.MapBindingFieldPairs = GetMapBindingFieldExp(elementView.MapBindingFieldPairs);
                }
            }

            viewExp.Zoom = this.GetExpressionKey(mapView.Zoom.ToString());

            return viewExp;
        }

        private StyleExp GetFontStyleExp(Style style)
        {
            StyleExp styleExp = new StyleExp();
            styleExp.Font = new FontExp();
            styleExp.Format = this.GetExpressionKey(styleExp.Format);
            styleExp.Language = this.GetExpressionKey(styleExp.Language);
            styleExp.Font.FontFamily = this.GetExpressionKey(style.FontFamily);
            styleExp.Font.FontStyle = this.GetExpressionKey(style.FontStyle);
            styleExp.Font.FontWeight = this.GetExpressionKey(style.FontWeight);

            if (style.FontSize != null)
            {
                styleExp.Font.FontSize = this.GetExpressionKey(style.FontSize.size);
            }

            return styleExp;
        }

        private string GetFieldName(string fieldExpr)
        {
            if (fieldExpr != null)
            {
                string fieldName = fieldExpr.ToLower();

                if (fieldName.ToLower().Contains("fields!") && fieldName.ToLower().Contains(".value"))
                {
                    fieldExpr = fieldExpr.Substring(fieldName.IndexOf("fields!", StringComparison.CurrentCultureIgnoreCase) + 7);
                    fieldExpr = fieldExpr.Substring(0, fieldExpr.IndexOf(".value", StringComparison.CurrentCultureIgnoreCase));
                }

                return fieldExpr;
            }
            return string.Empty;
        }

        private string GetExpressionKey(string value)
        {
            if (this.IsTablixChild && value != null && value.Contains("RowNumber("))
            {
                this.HasRowNumber = true;
            }

            string key = this.Model.ExpressionEngine.GetExpressionKey(value, this.DataSetName,this.IsTablixChild);
            this.AddDataFields(key);
            return key;
        }

        private string GetExpression(string value)
        {
            if (this.IsTablixChild && value != null && value.Contains("RowNumber("))
            {
                this.HasRowNumber = true;
            }

            string key = this.Model.ExpressionEngine.GetExpressionKey(value, this.DataSetName , true);
            this.AddDataFields(key);
            return key;
        }


        private void AddDataFields(string key)
        {
            if (this.IsTablixChild)
            {
                this.Model.ExpressionEngine.AddTablixDataFields(key, this);
            }
        }

        void IntializeEngineValues()
        {
            if (this.RowNumbers != null)
                this.Model.ExpressionEngine.RowNumbers = this.RowNumbers;
            if (this.GroupLevels != null)
                this.Model.ExpressionEngine.GroupLevels = this.GroupLevels;

            if (this.DataSource == null)
            {
                var viewAdv = (from view in this.Model.ProcessedData.DataSourceObjects
                               where view.Key == this.DataSetName
                               select view.Value).SingleOrDefault();

                this.DataSource = this.Model.ProcessedData.FilterItemSoruce(viewAdv, this.ExpFilters);

            }
            else
            {
                this.DataSource = this.Model.ProcessedData.FilterItemSoruce(this.DataSource as IEnumerable, this.ExpFilters);
            }
        }

        #endregion

#if !SILVERLIGHT
        private ReportingMap reportingMap { get; set; }

        internal List<ShapeActionInfo> GetPathData()
        {
#if !SyncfusionFramework3_5
            return reportingMap.GetShapePaths();
#else
            return null;
#endif
        }

        internal Stream GetImageStream()
        {
            reportingMap = new ReportingMap(this);
            return (new ImageConversion().CovertToImage(reportingMap));
        }
#else
        internal Stream GetImageStream()
        {
#if !WINRT
            System.Windows.UIElement map = ReportModel.UICollection[this.Name];
            System.Windows.Media.Imaging.WriteableBitmap _bitmap = new System.Windows.Media.Imaging.WriteableBitmap(map, map.RenderTransform);
            MemoryStream fs = new MemoryStream();
            int width = _bitmap.PixelWidth;
            int height = _bitmap.PixelHeight;

            Syncfusion.Windows.Chart.ChartImage ei = new Syncfusion.Windows.Chart.ChartImage(width, height);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int pixel = _bitmap.Pixels[(i * width) + j];
                    ei.SetPixel(j, i,
                                (byte)((pixel >> 16) & 0xFF),
                                (byte)((pixel >> 8) & 0xFF),
                                (byte)(pixel & 0xFF),
                                (byte)((pixel >> 24) & 0xFF)
                    );
                }
            }
            Stream png = ei.GetStream();
            int len = (int)png.Length;
            byte[] bytes = new byte[len];
            png.Read(bytes, 0, len);
            fs.Write(bytes, 0, len);
            fs.Position = 0;
            return fs;
#else
            return null;
#endif
        }
#endif

        #region overridden methods

        public override void Evaluate()
        {
            this.IntializeEngineValues();            
            try
            {
                using (MapEngine engine = new MapEngine())
                {
                    engine.DataSource = this.DataSource;
                    engine.Model = this;
                    this.MapModelData = engine.PopulateMapValues();
                }

                this.MapProperties = new MapItemExpVal();
                this.MapProperties.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(mapPropertiesExp.ToolTip);
                this.MapProperties.DocumentMapLabel = this.Model.ExpressionEngine.GetEvalExpressionString(mapPropertiesExp.DocumentMapLabel);

                if (mapPropertiesExp.Hidden != null)
                {
                    this.MapProperties.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPropertiesExp.Hidden));
                }

                if (mapPropertiesExp.Border != null)
                {
                    this.MapProperties.Border = new BorderExpval();
                    if (mapPropertiesExp.Border.Default != null)
                    {
                        this.MapProperties.Border.Default = GetBorderExpVal(mapPropertiesExp.Border.Default);
                    }
                    if (mapPropertiesExp.Border.LeftBorder != null)
                    {
                        this.MapProperties.Border.LeftBorder = GetBorderExpVal(mapPropertiesExp.Border.LeftBorder);
                    }
                    if (mapPropertiesExp.Border.TopBorder != null)
                    {
                        this.MapProperties.Border.TopBorder = GetBorderExpVal(mapPropertiesExp.Border.TopBorder);
                    }
                    if (mapPropertiesExp.Border.RightBorder != null)
                    {
                        this.MapProperties.Border.RightBorder = GetBorderExpVal(mapPropertiesExp.Border.RightBorder);
                    }
                    if (mapPropertiesExp.Border.BottomBorder != null)
                    {
                        this.MapProperties.Border.BottomBorder = GetBorderExpVal(mapPropertiesExp.Border.BottomBorder);
                    }
                }

                this.MapProperties.Background = GetBackgroundExpVal(mapPropertiesExp.Background);
                this.MapProperties.TextAntiAliasingQuality = TryEnum<TextAntiAliasingQuality>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPropertiesExp.TextAntiAliasingQuality));
                this.MapProperties.ShadowIntensity = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapPropertiesExp.ShadowIntensity)).FloatValue;
                this.MapProperties.TileLanguage = this.Model.ExpressionEngine.GetEvalExpressionString(mapPropertiesExp.TileLanguage);
                this.MapProperties.MaximumTotalPointCount = mapPropertiesExp.MaximumTotalPointCount;
                this.MapProperties.MaximumSpatialElementCount = mapPropertiesExp.MaximumSpatialElementCount;
                this.MapProperties.AntiAliasing = TryEnum<AntiAliasing>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPropertiesExp.AntiAliasing));
                this.MapProperties.Zindex = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPropertiesExp.Zindex));

                if (this.mapPropertiesExp.MapViewport != null)
                {
                    this.MapProperties.MapViewport = GetMapViewportExpVal(mapPropertiesExp.MapViewport);
                }
                if (this.mapPropertiesExp.MapLegendsExp != null)
                {
                    this.MapProperties.MapLegends = new List<MapLegendExpVal>();
                    foreach (var legend in this.mapPropertiesExp.MapLegendsExp)
                    {
                        this.MapProperties.MapLegends.Add(GetMapLegendExpVal(legend));
                    }
                }
                if (this.mapPropertiesExp.MapTitlesExp != null)
                {
                    this.MapProperties.MapTitles = new List<MapTitleExpVal>();
                    foreach (var title in this.mapPropertiesExp.MapTitlesExp)
                    {
                        this.MapProperties.MapTitles.Add(GetMapTitleExpVal(title));
                    }
                }
                if (this.mapPropertiesExp.MapPolygonLayersExp != null)
                {
                    this.MapProperties.MapPolygonLayers = new List<MapPolygonLayerExpVal>();
                    foreach (var layer in this.mapPropertiesExp.MapPolygonLayersExp)
                    {
                        this.MapProperties.MapPolygonLayers.Add(GetMapPolygonLayerExpVal(layer));
                    }
                }
                if (this.mapPropertiesExp.MapPointLayersExp != null)
                {
                    this.MapProperties.MapPointLayers = new List<MapPointLayerExpVal>();
                    foreach (var layer in this.mapPropertiesExp.MapPointLayersExp)
                    {
                        this.MapProperties.MapPointLayers.Add(GetMapPointLayerExpVal(layer));
                    }
                }
                if (this.mapPropertiesExp.MapLineLayersExp != null)
                {
                    this.MapProperties.MapLineLayers = new List<MapLineLayerExpVal>();
                    foreach (var layer in this.mapPropertiesExp.MapLineLayersExp)
                    {
                        this.MapProperties.MapLineLayers.Add(GetMapLineLayerExpVal(layer));
                    }
                }
                if (this.mapPropertiesExp.MapTileLayersExp != null)
                {
                    this.MapProperties.MapTileLayers = new List<MapTileLayerExpVal>();
                    foreach (var tileLayer in this.mapPropertiesExp.MapTileLayersExp)
                    {
                        this.MapProperties.MapTileLayers.Add(GetMapTileLayerExpVal(tileLayer));
                    }
                }
                if (this.mapPropertiesExp.MapDistanceScale != null)
                {
                    this.MapProperties.MapDistanceScale = GetMapDistanceScaleExpVal(this.mapPropertiesExp.MapDistanceScale);
                }
                if (this.mapPropertiesExp.MapColorScale != null)
                {
                    this.MapProperties.MapColorScale = GetMapColorScaleExpVal(this.mapPropertiesExp.MapColorScale);
                }
                if (this.mapPropertiesExp.MapBorderSkin != null)
                {
                    this.MapProperties.MapBorderSkin = GetMapBorderSkinExpVal(this.mapPropertiesExp.MapBorderSkin);
                }

                if (this.mapPropertiesExp.MapDataRegions != null)
                {
                    this.MapProperties.MapDataRegions = new List<MapDataRegionExpVal>();

                    foreach (var dataregion in this.mapPropertiesExp.MapDataRegions)
                    {
                        this.MapProperties.MapDataRegions.Add(GetMapDataRegionExpVal(dataregion));
                    }
                }

                this.MapProperties.ActionInfo = this.GetActionInfoExpVal(this.mapPropertiesExp.ActionInfo);

                base.Evaluate();
            }
            catch (Exception ex)
            {
                this.Model.ExceptionDetails.Add("Getting following exception while evaluate the expression in " + this.ReportItem.Name + " " + ex.Message);
            }
        }

        private MapTileLayerExpVal GetMapTileLayerExpVal(MapTileLayerExp mapTileLayer)
        {
            MapTileLayerExpVal layer = new MapTileLayerExpVal();
            if (mapTileLayer.MapTiles != null)
            {
                layer.MapTiles = new List<MapTileExpVal>();
                foreach (var tile in mapTileLayer.MapTiles)
                {
                    MapTileExpVal mapTile = new ItemModel.MapTileExpVal();
                    mapTile.MIMEType = this.GetExpressionKey(tile.MIMEType);
                    mapTile.Name = this.GetExpressionKey(tile.Name);
                    mapTile.TileData = this.GetExpressionKey(tile.TileData);
                    layer.MapTiles.Add(mapTile);
                }
            }

            layer.TileStyle = TryEnum<TileStyle>(this.Model.ExpressionEngine.GetEvalExpressionString(mapTileLayer.TileStyle));
            layer.Name = this.Model.ExpressionEngine.GetEvalExpressionString(mapTileLayer.Name);
            layer.Transparency = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapTileLayer.Transparency));
            layer.VisibilityMode = TryEnum<VisibilityMode>(this.Model.ExpressionEngine.GetEvalExpressionString(mapTileLayer.VisibilityMode));
            layer.MinimumZoom = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapTileLayer.MinimumZoom));
            layer.MaximumZoom = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapTileLayer.MaximumZoom));

            return layer;
        }

        private MapLineLayerExpVal GetMapLineLayerExpVal(MapLineLayerExp lineLayer)
        {
            MapLineLayerExpVal maplineLayer = new MapLineLayerExpVal();

            if (lineLayer.MapLayerExp != null)
            {
                maplineLayer.MapLayerExpVal = GetMapLayerExpVal(lineLayer.MapLayerExp);
            }
            if (lineLayer.MapSpatialData != null)
            {
                maplineLayer.MapSpatialData = GetMapSpatialDataExpVal(lineLayer.MapSpatialData);
            }
            if (lineLayer.MapFieldDefinitions != null)
            {
                maplineLayer.MapFieldDefinitions = GetMapFieldDefExpVal(lineLayer.MapFieldDefinitions);
            }
            if (lineLayer.MapBindingFieldPairs != null)
            {
                maplineLayer.MapBindingFieldPairs = GetMapBindingFieldExpVal(lineLayer.MapBindingFieldPairs);
            }
            if (lineLayer.MapLineRules != null)
            {
                maplineLayer.MapLineRules = new MapLineRulesExpVal();
                
                if (lineLayer.MapLineRules.MapColorRule != null)
                {
                    maplineLayer.MapLineRules.MapColorRule = GetMapColorRuleExpVal(lineLayer.MapLineRules.MapColorRule);
                }
                if (lineLayer.MapLineRules.MapSizeRule != null)
                {
                    maplineLayer.MapLineRules.MapSizeRule = GetMapSizeRuleExpVal(lineLayer.MapLineRules.MapSizeRule);
                }
            }
            if (lineLayer.MapLines != null)
            {
                maplineLayer.MapLines = new List<MapLineExpVal>();
                foreach (var lines in lineLayer.MapLines)
                {
                    MapLineExpVal lineExpVal = new MapLineExpVal();
                    if (lines.MapFields != null)
                    {
                        lineExpVal.MapFields = GetMapFieldsExpVal(lines.MapFields);
                        if (lines.VectorData != null && lineExpVal.MapFields != null && lineExpVal.MapFields.Count > 0)
                        {
                            lineExpVal.MapFields.Add("VectorData", Convert.FromBase64String(this.Model.ExpressionEngine.GetEvalExpressionString(lines.VectorData)));
                        }
                    }
                    if (lines.MapLineTemplate != null)
                    {
                        lineExpVal.MapLineTemplate = GetMapLineTemplateExpVal(lines.MapLineTemplate); 
                    }
                    
                    lineExpVal.UseCustomLineTemplate = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(lines.UseCustomLineTemplate));
                    //lineExpVal.VectorData = this.Model.ExpressionEngine.GetEvalExpressionString(lines.VectorData);
                    maplineLayer.MapLines.Add(lineExpVal);
                }
            }
            if (lineLayer.MapLineTemplate != null)
            {
                maplineLayer.MapLineTemplate = GetMapLineTemplateExpVal(lineLayer.MapLineTemplate);
            }

            maplineLayer.MapDataRegionName = this.Model.ExpressionEngine.GetEvalExpressionString(lineLayer.MapDataRegionName);
            maplineLayer.DataElementName = this.Model.ExpressionEngine.GetEvalExpressionString(lineLayer.DataElementName);
            maplineLayer.DataElementOutput = TryEnum<DataElementOutputs>(this.Model.ExpressionEngine.GetEvalExpressionString(lineLayer.DataElementOutput));

            return maplineLayer;
        }

        private MapLineTemplateExpVal GetMapLineTemplateExpVal(MapLineTemplateExp mapLineTemplateExp)
        {
            MapLineTemplateExpVal template = new MapLineTemplateExpVal();

            if (mapLineTemplateExp.Border != null)
            {
                template.BorderExpval = GetBorderExpVal(mapLineTemplateExp.Border);
            }
            if (mapLineTemplateExp.ActionInfo != null)
            {
                template.ActionInfo = GetActionInfoExpVal(mapLineTemplateExp.ActionInfo);
            }
            if (mapLineTemplateExp.StyleExp != null)
            {
                template.StyleExpval = GetFontStyleExpVal(mapLineTemplateExp.StyleExp);
            }
            if (mapLineTemplateExp.Width != null)
            {
                template.Width = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapLineTemplateExp.Width));
            }

            template.DataElementLabel = this.Model.ExpressionEngine.GetEvalExpressionString(mapLineTemplateExp.DataElementLabel);
            template.DataElementName = this.Model.ExpressionEngine.GetEvalExpressionString(mapLineTemplateExp.DataElementName);
            template.DataElementOutput = TryEnum<DataElementOutputs>(this.Model.ExpressionEngine.GetEvalExpressionString(mapLineTemplateExp.DataElementOutput));
            template.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapLineTemplateExp.Hidden));
            //template.Label = this.Model.ExpressionEngine.GetEvalExpressionString(mapLineTemplateExp.Label);
            template.Label = mapLineTemplateExp.Label;
            template.LabelPlacement = TryEnum<LabelPlacement>(this.Model.ExpressionEngine.GetEvalExpressionString(mapLineTemplateExp.LabelPlacement));
            template.OffsetX = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapLineTemplateExp.OffsetX));
            template.OffsetY = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapLineTemplateExp.OffsetY));
            template.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(mapLineTemplateExp.ToolTip);

            return template;
        }

        private MapPointLayerExpVal GetMapPointLayerExpVal(MapPointLayerExp pointLayer)
        {
            MapPointLayerExpVal mapPointLayer = new MapPointLayerExpVal();

            if (pointLayer.MapLayerExp != null)
            {
                mapPointLayer.MapLayerExpVal = GetMapLayerExpVal(pointLayer.MapLayerExp);
            }
            if (pointLayer.MapBindingFieldPairs != null)
            {
                mapPointLayer.MapBindingFieldPairs = GetMapBindingFieldExpVal(pointLayer.MapBindingFieldPairs);
            }
            if (pointLayer.MapFieldDefinitions != null)
            {
                mapPointLayer.MapFieldDefinitions = GetMapFieldDefExpVal(pointLayer.MapFieldDefinitions);
            }
            if (pointLayer.MapPointRules != null)
            {
                mapPointLayer.MapPointRules = new MapPointRulesExpVal();

                if (pointLayer.MapPointRules.MapColorRule != null)
                {
                    mapPointLayer.MapPointRules.MapColorRule = GetMapColorRuleExpVal(pointLayer.MapPointRules.MapColorRule);
                }
                if (pointLayer.MapPointRules.MapMarkerRule != null)
                {
                    mapPointLayer.MapPointRules.MapMarkerRule = GetMapMarkerRuleExpVal(pointLayer.MapPointRules.MapMarkerRule);
                }
                if (pointLayer.MapPointRules.MapSizeRule != null)
                {
                    mapPointLayer.MapPointRules.MapSizeRule = GetMapSizeRuleExpVal(pointLayer.MapPointRules.MapSizeRule);
                }
            }
            if (pointLayer.MapPoints != null)
            {
                mapPointLayer.MapPoints = new List<MapPointExpVal>();
                foreach (var point in pointLayer.MapPoints)
                {
                    MapPointExpVal pt = new MapPointExpVal();
                    //pt.VectorData = this.Model.ExpressionEngine.GetEvalExpressionString(point.VectorData);
                    pt.UseCustomPointTemplate = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(point.UseCustomPointTemplate));
                    
                    if (point.MapPointTemplate != null)
                    {
                        pt.MapPointTemplate = GetMapPointTemplateExpVal(point.MapPointTemplate);
                    }
                    if (point.MapFields != null)
                    {
                        pt.MapFields = GetMapFieldsExpVal(point.MapFields);
                        if (point.VectorData != null && pt.MapFields != null && pt.MapFields.Count > 0)
                        {
                            pt.MapFields.Add("VectorData", Convert.FromBase64String(this.Model.ExpressionEngine.GetEvalExpressionString(point.VectorData)));
                        }
                    }
                    mapPointLayer.MapPoints.Add(pt);
                }
            }
            if (pointLayer.MapPointTemplate != null)
            {
                mapPointLayer.MapPointTemplate = GetMapPointTemplateExpVal(pointLayer.MapPointTemplate);
            }
            if (pointLayer.MapSpatialData != null)
            {
                mapPointLayer.MapSpatialData = GetMapSpatialDataExpVal(pointLayer.MapSpatialData);
            }

            mapPointLayer.MapDataRegionName = this.Model.ExpressionEngine.GetEvalExpressionString(pointLayer.MapDataRegionName);
            mapPointLayer.DataElementName = this.Model.ExpressionEngine.GetEvalExpressionString(pointLayer.DataElementName);
            mapPointLayer.DataElementOutput = TryEnum<DataElementOutputs>(this.Model.ExpressionEngine.GetEvalExpressionString(pointLayer.DataElementOutput));

            return mapPointLayer;
        }

        private MapPolygonLayerExpVal GetMapPolygonLayerExpVal(MapPolygonLayerExp polygonLayer)
        {
            MapPolygonLayerExpVal mapPolygonLayer = new MapPolygonLayerExpVal();

            if (polygonLayer.MapLayerExp != null)
            {
                mapPolygonLayer.MapLayerExpVal = GetMapLayerExpVal(polygonLayer.MapLayerExp);
            }
            if (polygonLayer.MapSpatialData != null)
            {
                mapPolygonLayer.MapSpatialData = GetMapSpatialDataExpVal(polygonLayer.MapSpatialData);
            }
            if (polygonLayer.MapBindingFieldPairs != null && polygonLayer.MapBindingFieldPairs.Count > 0)
            {
                mapPolygonLayer.MapBindingFieldPairs = GetMapBindingFieldExpVal(polygonLayer.MapBindingFieldPairs);
            }
            if (polygonLayer.MapCenterPointRules != null)
            {
                mapPolygonLayer.MapCenterPointRules = new MapPointRulesExpVal();
                if (polygonLayer.MapCenterPointRules.MapColorRule != null)
                {
                    mapPolygonLayer.MapCenterPointRules.MapColorRule = GetMapColorRuleExpVal(polygonLayer.MapCenterPointRules.MapColorRule);
                }
                if (polygonLayer.MapCenterPointRules.MapMarkerRule != null)
                {
                    mapPolygonLayer.MapCenterPointRules.MapMarkerRule = GetMapMarkerRuleExpVal(polygonLayer.MapCenterPointRules.MapMarkerRule);
                }
                if (polygonLayer.MapCenterPointRules.MapSizeRule != null)
                {
                    mapPolygonLayer.MapCenterPointRules.MapSizeRule = GetMapSizeRuleExpVal(polygonLayer.MapCenterPointRules.MapSizeRule);
                }
            }
            if (polygonLayer.MapCenterPointTemplate != null)
            {
                mapPolygonLayer.MapCenterPointTemplate = GetMapPointTemplateExpVal(polygonLayer.MapCenterPointTemplate);
            }
            if (polygonLayer.MapPolygonTemplate != null)
            {
                mapPolygonLayer.MapPolygonTemplate = GetMapPolygonTemplateExpVal(polygonLayer.MapPolygonTemplate);
            }
            if (polygonLayer.MapFieldDefinitions != null)
            {
                mapPolygonLayer.MapFieldDefinitions = GetMapFieldDefExpVal(polygonLayer.MapFieldDefinitions);
            }
            if (polygonLayer.MapPolygonRules != null)
            {
                mapPolygonLayer.MapPolygonRules = new MapPolygonRulesExpVal();
                if (polygonLayer.MapPolygonRules.MapColorRule != null)
                {
                    mapPolygonLayer.MapPolygonRules.MapColorRule = GetMapColorRuleExpVal(polygonLayer.MapPolygonRules.MapColorRule);
                }
            }
            if (polygonLayer.MapPolygons != null)
            {
                mapPolygonLayer.MapPolygons = new List<MapPolygonExpVal>();
                foreach (var polygon in polygonLayer.MapPolygons)
                {
                    MapPolygonExpVal polygonExpVal = new MapPolygonExpVal();
                    
                    if (polygon.MapCenterPointTemplate != null)
                    {
                        polygonExpVal.MapCenterPointTemplate = GetMapPointTemplateExpVal(polygon.MapCenterPointTemplate);
                    }
                    if (polygon.MapFields != null)
                    {
                        polygonExpVal.MapFields = GetMapFieldsExpVal(polygon.MapFields);
                        if (polygon.VectorData != null && polygonExpVal.MapFields != null &&
                            polygonExpVal.MapFields.Count > 0)
                        {
                            polygonExpVal.MapFields.Add("VectorData",Convert.FromBase64String(this.Model.ExpressionEngine.GetEvalExpressionString(polygon.VectorData)));
                        }
                    }
                    if (polygon.MapPolygonTemplate != null)
                    {
                        polygonExpVal.MapPolygonTemplate = GetMapPolygonTemplateExpVal(polygon.MapPolygonTemplate);
                    }
                    polygonExpVal.UseCustomCenterPointTemplate = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(polygon.UseCustomCenterPointTemplate));
                    polygonExpVal.UseCustomPolygonTemplate = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(polygon.UseCustomPolygonTemplate));
                   // polygonExpVal.VectorData = this.Model.ExpressionEngine.GetEvalExpressionString(polygon.VectorData);
                    mapPolygonLayer.MapPolygons.Add(polygonExpVal);
                }
            }

            mapPolygonLayer.DataElementName = this.Model.ExpressionEngine.GetEvalExpressionString(polygonLayer.DataElementName);
            mapPolygonLayer.MapDataRegionName = this.Model.ExpressionEngine.GetEvalExpressionString(polygonLayer.MapDataRegionName);
            mapPolygonLayer.DataElementOutput = TryEnum<DataElementOutputs>(this.Model.ExpressionEngine.GetEvalExpressionString(polygonLayer.DataElementOutput));

            return mapPolygonLayer;
        }

        private MapPolygonTemplateExpVal GetMapPolygonTemplateExpVal(MapPolygonTemplateExp mapPolygonTemplateExp)
        {
            MapPolygonTemplateExpVal template = new MapPolygonTemplateExpVal();

            if (mapPolygonTemplateExp.Border != null)
            {
                template.BorderExpval = GetBorderExpVal(mapPolygonTemplateExp.Border);
            }
            if (mapPolygonTemplateExp.Background!=null)
            {
                template.BackGroundExpVal = GetBackgroundExpVal(mapPolygonTemplateExp.Background);
            }
            if (mapPolygonTemplateExp.ActionInfo != null)
            {
                template.ActionInfo = GetActionInfoExpVal(mapPolygonTemplateExp.ActionInfo);
            }
            if (mapPolygonTemplateExp.StyleExp != null)
            {
                template.StyleExpval = GetFontStyleExpVal(mapPolygonTemplateExp.StyleExp);
            }

            template.CenterPointOffsetX = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPolygonTemplateExp.CenterPointOffsetX));
            template.CenterPointOffsetY = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPolygonTemplateExp.CenterPointOffsetY));
            template.DataElementLabel = this.Model.ExpressionEngine.GetEvalExpressionString(mapPolygonTemplateExp.DataElementLabel);
            template.DataElementName = this.Model.ExpressionEngine.GetEvalExpressionString(mapPolygonTemplateExp.DataElementName);
            template.DataElementOutput = TryEnum<DataElementOutputs>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPolygonTemplateExp.DataElementOutput));
            template.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPolygonTemplateExp.Hidden));
            //template.Label = this.Model.ExpressionEngine.GetEvalExpressionString(mapPolygonTemplateExp.Label);
            template.Label = mapPolygonTemplateExp.Label;
            template.LabelPlacement = TryEnum<LabelPlacement>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPolygonTemplateExp.LabelPlacement));
            template.OffsetX = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPolygonTemplateExp.OffsetX));
            template.OffsetY = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPolygonTemplateExp.OffsetY));
            template.ScaleFactor = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPolygonTemplateExp.ScaleFactor));
            template.ShowLabel = TryEnum<BooleanOptions>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPolygonTemplateExp.ShowLabel));
            template.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(mapPolygonTemplateExp.ToolTip);

            return template;
        }

        private MapColorRuleExpVal GetMapColorRuleExpVal(MapColorRuleExp mapColorRuleExp)
        {
            MapColorRuleExpVal colorRule = new MapColorRuleExpVal();
            if (mapColorRuleExp.MapColorRangeRule != null)
            {
                colorRule.MapColorRangeRule = new MapColorRangeRuleExpVal();
                colorRule.MapColorRangeRule.StartColor = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorRuleExp.MapColorRangeRule.StartColor);
                colorRule.MapColorRangeRule.EndColor = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorRuleExp.MapColorRangeRule.EndColor);
                colorRule.MapColorRangeRule.MiddleColor = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorRuleExp.MapColorRangeRule.MiddleColor);
            }
            else if (mapColorRuleExp.ColorPalette != null)
            {
                colorRule.ColorPalette = new MapColorPaletteRuleExpVal();
                colorRule.ColorPalette.Palette = TryEnum<MapColorPalette>(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorRuleExp.ColorPalette.Palette));
            }
            else if (mapColorRuleExp.MapCustomColors != null)
            {
                colorRule.MapCustomColors = new List<string>();
                foreach (var color in mapColorRuleExp.MapCustomColors)
                {
                    colorRule.MapCustomColors.Add(this.Model.ExpressionEngine.GetEvalExpressionString(color));
                }
            }
            colorRule.BucketCount = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorRuleExp.BucketCount));
            colorRule.DataElementName = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorRuleExp.DataElementName);
            colorRule.DataElementOutput = TryEnum<DataElementOutputs>(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorRuleExp.DataElementOutput));
            //colorRule.DataValue = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorRuleExp.DataValue);
            colorRule.DataValue = mapColorRuleExp.DataValue;
            colorRule.DistributionType = TryEnum<DistributionType>(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorRuleExp.DistributionType));
            colorRule.EndValue = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorRuleExp.EndValue);
            colorRule.StartValue = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorRuleExp.StartValue);
            colorRule.LegendName = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorRuleExp.LegendName);
            colorRule.LegendText = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorRuleExp.LegendText);

            return colorRule;
        }

        private MapMarkerRuleExpVal GetMapMarkerRuleExpVal(MapMarkerRuleExp mapMarkerRuleExp)
        {
            MapMarkerRuleExpVal markerRule = new MapMarkerRuleExpVal();
            markerRule.BucketCount = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(mapMarkerRuleExp.BucketCount));
            markerRule.DataElementName = this.Model.ExpressionEngine.GetEvalExpressionString(mapMarkerRuleExp.DataElementName);
            markerRule.DataElementOutput = TryEnum<DataElementOutputs>(this.Model.ExpressionEngine.GetEvalExpressionString(mapMarkerRuleExp.DataElementOutput));
            //markerRule.DataValue = this.Model.ExpressionEngine.GetEvalExpressionString(mapMarkerRuleExp.DataValue);
            markerRule.DataValue = mapMarkerRuleExp.DataValue;
            markerRule.DistributionType = TryEnum<DistributionType>(this.Model.ExpressionEngine.GetEvalExpressionString(mapMarkerRuleExp.DistributionType));
            markerRule.EndValue = this.Model.ExpressionEngine.GetEvalExpressionString(mapMarkerRuleExp.EndValue);

            return markerRule;  
        }

        private MapSizeRuleExpVal GetMapSizeRuleExpVal(MapSizeRuleExp mapSizeRuleExp)
        {
            MapSizeRuleExpVal sizerule = new MapSizeRuleExpVal();
            sizerule.BucketCount = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(mapSizeRuleExp.BucketCount));
            sizerule.DataElementName = this.Model.ExpressionEngine.GetEvalExpressionString(mapSizeRuleExp.DataElementName);
            sizerule.DataElementOutput = TryEnum<DataElementOutputs>(this.Model.ExpressionEngine.GetEvalExpressionString(mapSizeRuleExp.DataElementOutput));
            //sizerule.DataValue = this.Model.ExpressionEngine.GetEvalExpressionString(mapSizeRuleExp.DataValue);
            sizerule.DataValue = mapSizeRuleExp.DataValue;
            sizerule.DistributionType = TryEnum<DistributionType>(this.Model.ExpressionEngine.GetEvalExpressionString(mapSizeRuleExp.DistributionType));
            sizerule.EndValue = this.Model.ExpressionEngine.GetEvalExpressionString(mapSizeRuleExp.EndValue);
            if (mapSizeRuleExp.StartSize != null)
            {
                sizerule.StartSize =this.Model.ExpressionEngine.GetEvalExpressionString(mapSizeRuleExp.StartSize);
            }
            if (mapSizeRuleExp.EndSize != null)
            {
                sizerule.EndSize = this.Model.ExpressionEngine.GetEvalExpressionString(mapSizeRuleExp.EndSize);
            }
            return sizerule;
        }

        private MapPointTemplateExpVal GetMapPointTemplateExpVal(MapPointTemplateExp mapPointTemplateExp)
        {
            MapPointTemplateExpVal template = new MapPointTemplateExpVal();

            if (mapPointTemplateExp.Border != null)
            {
                template.BorderExpval = GetBorderExpVal(mapPointTemplateExp.Border);
            }
            if (mapPointTemplateExp.ActionInfo != null)
            {
                template.ActionInfo = GetActionInfoExpVal(mapPointTemplateExp.ActionInfo);
            }
            if (mapPointTemplateExp.StyleExp != null)
            {
                template.StyleExpval = GetFontStyleExpVal(mapPointTemplateExp.StyleExp);
            }
            if (mapPointTemplateExp.Size != null)
            {
                template.Size = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapPointTemplateExp.Size));
            }

            template.DataElementLabel = this.Model.ExpressionEngine.GetEvalExpressionString(mapPointTemplateExp.DataElementLabel);
            template.DataElementName = this.Model.ExpressionEngine.GetEvalExpressionString(mapPointTemplateExp.DataElementName);
            template.DataElementOutput = TryEnum<DataElementOutputs>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPointTemplateExp.DataElementOutput));
            template.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPointTemplateExp.Hidden));
            //template.Label = this.Model.ExpressionEngine.GetEvalExpressionString(mapPointTemplateExp.Label);
            template.Label = mapPointTemplateExp.Label;
            template.LabelPlacement = TryEnum<LabelPlacement>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPointTemplateExp.LabelPlacement));
            template.OffsetX = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPointTemplateExp.OffsetX));
            template.OffsetY = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapPointTemplateExp.OffsetY));
            template.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(mapPointTemplateExp.ToolTip);
            
            return template;
        }

        private List<MapFieldDefinitionExpVal> GetMapFieldDefExpVal(List<MapFieldDefinitionExp> mapFieldDefinitions)
        {
            List<MapFieldDefinitionExpVal> fieldDefVal = new List<MapFieldDefinitionExpVal>();
            foreach (var defi in mapFieldDefinitions)
            {
                MapFieldDefinitionExpVal df = new MapFieldDefinitionExpVal();
                df.DataType = TryEnum<DataTypes>(this.Model.ExpressionEngine.GetEvalExpressionString(defi.DataType));
                df.Name = this.Model.ExpressionEngine.GetEvalExpressionString(defi.Name);
                fieldDefVal.Add(df);
            }
            return fieldDefVal;
        }

        private Dictionary<string,object> GetMapFieldsExpVal(List<MapFieldExp> mapFields)
        {
            Dictionary<string, object> fieldColl = new Dictionary<string, object>();
            foreach (var field in mapFields)
            {
                MapFieldExpVal fieldval = new MapFieldExpVal();
                fieldval.Name = this.Model.ExpressionEngine.GetEvalExpressionString(field.Name);
                fieldval.Value = this.Model.ExpressionEngine.GetEvalExpressionString(field.Value);
                fieldColl.Add(this.Model.ExpressionEngine.GetEvalExpressionString(field.Name), this.Model.ExpressionEngine.GetEvalExpressionString(field.Value).ToString());
            }
            return fieldColl;
        }

        private MapSpatialDataExpVal GetMapSpatialDataExpVal(MapSpatialDataExp mapSpatialDataExp)
        {
            MapSpatialDataExpVal spatialData = new MapSpatialDataExpVal();
            if (mapSpatialDataExp.MapShapefile != null)
            {
                spatialData.MapShapefile = new MapShapefileExpVal();
                if (mapSpatialDataExp.MapShapefile.MapFieldNames != null)
                {
                    spatialData.MapShapefile.MapFieldNames = new List<string>();
                    foreach (var field in mapSpatialDataExp.MapShapefile.MapFieldNames)
                    {
                        spatialData.MapShapefile.MapFieldNames.Add(this.Model.ExpressionEngine.GetEvalExpressionString(field));
                    }
                }
                spatialData.MapShapefile.Source = this.Model.ExpressionEngine.GetEvalExpressionString(mapSpatialDataExp.MapShapefile.Source);
            }
            if (mapSpatialDataExp.MapSpatialDataRegionExp != null)
            {
                spatialData.MapSpatialDataRegionExp = new MapSpatialDataRegionExpVal();
                spatialData.MapSpatialDataRegionExp.VectorData = this.Model.ExpressionEngine.GetEvalExpressionString(mapSpatialDataExp.MapSpatialDataRegionExp.VectorData);
            }
            if (mapSpatialDataExp.MapSpatialDataSetExp != null)
            {
                spatialData.MapSpatialDataSetExp = new MapSpatialDataSetExpVal();
                if (mapSpatialDataExp.MapSpatialDataSetExp.MapFieldNames != null)
                {
                    spatialData.MapSpatialDataSetExp.MapFieldNames = new List<string>();
                    foreach (var field in mapSpatialDataExp.MapSpatialDataSetExp.MapFieldNames)
                    {
                        spatialData.MapSpatialDataSetExp.MapFieldNames.Add(this.Model.ExpressionEngine.GetEvalExpressionString(field));
                    }
                }

                spatialData.MapSpatialDataSetExp.SpatialField = this.Model.ExpressionEngine.GetEvalExpressionString(mapSpatialDataExp.MapSpatialDataSetExp.SpatialField);
                spatialData.MapSpatialDataSetExp.DataSetName = this.Model.ExpressionEngine.GetEvalExpressionString(mapSpatialDataExp.MapSpatialDataSetExp.DataSetName);
            }

            return spatialData;
        }

        private List<MapBindingFieldPairExpVal> GetMapBindingFieldExpVal(List<MapBindingFieldPairExp> mapBindingFieldPairs)
        {
            List<MapBindingFieldPairExpVal> fieldpairs = new List<MapBindingFieldPairExpVal>();
            foreach (var fields in mapBindingFieldPairs)
            {
                MapBindingFieldPairExpVal pair = new MapBindingFieldPairExpVal();
                //pair.BindingExpression = this.Model.ExpressionEngine.GetEvalExpressionString(fields.BindingExpression);
                //pair.FieldName = this.Model.ExpressionEngine.GetEvalExpressionString(fields.FieldName);
                pair.BindingExpression = fields.BindingExpression;
                pair.FieldName = fields.FieldName;
                fieldpairs.Add(pair);
            }

            return fieldpairs;
        }

        private MapDataRegionExpVal GetMapDataRegionExpVal(MapDataRegionExp dataregion)
        {
            MapDataRegionExpVal mapDataRegion = new MapDataRegionExpVal();

            if (dataregion.MapMember != null)
            {
                mapDataRegion.MapMember = new MapMemberExpVal();

                if (dataregion.MapMember.Group != null)
                {
                    mapDataRegion.MapMember.Group = new GroupExpVal();

                    if (dataregion.MapMember.Group.BreakLocation != null)
                    {
                        mapDataRegion.MapMember.Group.BreakLocation = TryEnum<BreakLocation>(dataregion.MapMember.Group.BreakLocation);
                    }
                    if (dataregion.MapMember.Group.GroupExpressions != null)
                    {
                        mapDataRegion.MapMember.Group.GroupExpressions = new List<string>();
                        foreach (var grpExp in dataregion.MapMember.Group.GroupExpressions)
                        {
                            mapDataRegion.MapMember.Group.GroupExpressions.Add(this.Model.ExpressionEngine.GetEvalExpressionString(grpExp));
                        }
                    }
                    if (dataregion.MapMember.Group.Filters != null)
                    {
                        mapDataRegion.MapMember.Group.Filters = new FiltersExpVal();
                       
                        foreach (var filter in dataregion.MapMember.Group.Filters)
                        {
                            FilterExpVal fl = new FilterExpVal();
                            fl.FilterExpression = this.Model.ExpressionEngine.GetEvalExpressionString(filter.FilterExpression);
                            if (filter.FilterValues != null)
                            {
                                fl.FilterValues = new FilterValuesExpVal();
                                foreach (var flValue in filter.FilterValues)
                                {
                                    FilterValueExpVal flv = new FilterValueExpVal();
                                    flv.DataType = TryEnum<DataTypes>(flValue.DataType);
                                    flv.Value = this.Model.ExpressionEngine.GetEvalExpressionString(flValue.Value);
                                    fl.FilterValues.Add(flv);
                                }
                            }

                            fl.Operator = TryEnum<FilterOperators>(filter.Operator);
                            mapDataRegion.MapMember.Group.Filters.Add(fl);
                        }
                    }

                    mapDataRegion.MapMember.Group.DataElementName = this.Model.ExpressionEngine.GetEvalExpressionString(dataregion.MapMember.Group.DataElementName);
                    mapDataRegion.MapMember.Group.DocumentMapLabel = this.Model.ExpressionEngine.GetEvalExpressionString(dataregion.MapMember.Group.DocumentMapLabel);
                    mapDataRegion.MapMember.Group.DomainScope = this.Model.ExpressionEngine.GetEvalExpressionString(dataregion.MapMember.Group.DomainScope);
                    mapDataRegion.MapMember.Group.Name = this.Model.ExpressionEngine.GetEvalExpressionString(dataregion.MapMember.Group.Name);
                    mapDataRegion.MapMember.Group.Parent = this.Model.ExpressionEngine.GetEvalExpressionString(dataregion.MapMember.Group.Parent);
                }
            }
            if (dataregion.Filters != null)
            {
                mapDataRegion.Filters = new FiltersExpVal();
                foreach (var filter in dataregion.Filters)
                {
                    FilterExpVal fl = new FilterExpVal();
                    fl.FilterExpression = this.Model.ExpressionEngine.GetEvalExpressionString(filter.FilterExpression);
                    if (filter.FilterValues != null)
                    {
                        fl.FilterValues = new FilterValuesExpVal();
                        foreach (var flValue in filter.FilterValues)
                        {
                            FilterValueExpVal flv = new FilterValueExpVal();
                            flv.DataType = TryEnum<DataTypes>(this.Model.ExpressionEngine.GetEvalExpressionString(flValue.DataType));
                            flv.Value = this.Model.ExpressionEngine.GetEvalExpressionString(flValue.Value);
                            fl.FilterValues.Add(flv);
                        }
                    }
                    fl.Operator = TryEnum<FilterOperators>(this.Model.ExpressionEngine.GetEvalExpressionString(filter.Operator));
                    mapDataRegion.Filters.Add(fl);
                }
            }

            mapDataRegion.Name = this.Model.ExpressionEngine.GetEvalExpressionString(dataregion.Name);
            mapDataRegion.DataSetName = this.Model.ExpressionEngine.GetEvalExpressionString(dataregion.DataSetName);


            return mapDataRegion;
        }

        private TextboxActionInfoExpVal GetActionInfoExpVal(TextboxActionInfoExp actionInfoExp)
        {
            if (actionInfoExp != null)
            {
                TextboxActionInfoExpVal actionInfo = new TextboxActionInfoExpVal();
                actionInfo.BookmarkLink = this.Model.ExpressionEngine.GetEvalExpressionString(actionInfoExp.BookmarkLink);
                actionInfo.Hyperlink = this.Model.ExpressionEngine.GetEvalExpressionString(actionInfoExp.Hyperlink);
                actionInfo.ReportName = this.Model.ExpressionEngine.GetEvalExpressionString(actionInfoExp.ReportName);
                
                if (actionInfo.Parameters != null)
                {
                    actionInfo.Parameters = new List<TextboxParameterExpVal>();
                    foreach (var parameters in actionInfoExp.Parameters)
                    {
                        TextboxParameterExpVal Parameter = new TextboxParameterExpVal();
                        Parameter.Name = this.Model.ExpressionEngine.GetEvalExpressionString(parameters.Name);
                        Parameter.Omit = this.Model.ExpressionEngine.GetEvalExpressionString(parameters.Omit);
                        Parameter.Value = this.Model.ExpressionEngine.GetEvalExpressionString(parameters.Value);
                        actionInfo.Parameters.Add(Parameter);
                    }
                }

                return actionInfo;
            }

            return null;
        }

        private MapBorderSkinExpVal GetMapBorderSkinExpVal(MapBorderSkinExp mapBorderSkinExp)
        {
            MapBorderSkinExpVal borderSkin = new MapBorderSkinExpVal();
            if (mapBorderSkinExp.Border != null)
            {
                borderSkin.Border = GetBorderExpVal(mapBorderSkinExp.Border);
            }
            if (mapBorderSkinExp.BackGroundExp != null)
            {
                borderSkin.BackGroundExpVal = GetBackgroundExpVal(mapBorderSkinExp.BackGroundExp);
            }
            if (mapBorderSkinExp.Style != null)
            {
                borderSkin.Style = GetFontStyleExpVal(mapBorderSkinExp.Style);
            }

            borderSkin.MapBorderSkinType = TryEnum<BorderSkinType>(this.Model.ExpressionEngine.GetEvalExpressionString(mapBorderSkinExp.MapBorderSkinType));
            
            return borderSkin;
        }

        private StyleExpval GetFontStyleExpVal(StyleExp styleExp)
        {
            StyleExpval style = new StyleExpval();
            if (styleExp.Font != null)
            {
                style.Font = new FontExpval();
                style.Font.FontFamily = this.Model.ExpressionEngine.GetEvalExpressionString(styleExp.Font.FontFamily);
                style.Font.FontSize = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(styleExp.Font.FontSize)).PixelValue;
                style.Font.FontStyle = TryEnum<FontStyle>(this.Model.ExpressionEngine.GetEvalExpressionString(styleExp.Font.FontStyle));
                style.Font.FontWeight = TryEnum<FontWeight>(this.Model.ExpressionEngine.GetEvalExpressionString(styleExp.Font.FontWeight));
            }

            style.Format = this.Model.ExpressionEngine.GetEvalExpressionString(style.Format);
            style.Language = this.Model.ExpressionEngine.GetEvalExpressionString(style.Language);

            return style;
        }

        private MapColorScaleExpVal GetMapColorScaleExpVal(MapColorScaleExp mapColorScaleExp)
        {
            MapColorScaleExpVal colorScale = new MapColorScaleExpVal();

            if (mapColorScaleExp.Border != null)
            {
                colorScale.Border = GetBorderExpVal(mapColorScaleExp.Border);
            }
            if (mapColorScaleExp.BackGroundExp != null)
            {
                colorScale.BackGroundExpVal = GetBackgroundExpVal(mapColorScaleExp.BackGroundExp);
            }
            if (mapColorScaleExp.Style != null)
            {
                colorScale.Style = GetFontStyleExpVal(mapColorScaleExp.Style);
            }
            if (mapColorScaleExp.MapLocation != null)
            {
                colorScale.MapLocation = GetMapLocationExpVal(mapColorScaleExp.MapLocation);
            }
            if (mapColorScaleExp.MapSize != null)
            {
                colorScale.MapSize = GetMapSizeExpVal(mapColorScaleExp.MapSize);
            }
            if (mapColorScaleExp.MapColorScaleTitle != null)
            {
                colorScale.MapColorScaleTitle = new MapColorScaleTitleExpVal();

                if (mapColorScaleExp.MapColorScaleTitle.Border != null)
                {
                    colorScale.Border = GetBorderExpVal(mapColorScaleExp.MapColorScaleTitle.Border);
                }
                if (mapColorScaleExp.MapColorScaleTitle.BackGroundExp != null)
                {
                    colorScale.BackGroundExpVal = GetBackgroundExpVal(mapColorScaleExp.MapColorScaleTitle.BackGroundExp);
                }
                if (mapColorScaleExp.MapColorScaleTitle.Style != null)
                {
                    colorScale.Style = GetFontStyleExpVal(mapColorScaleExp.MapColorScaleTitle.Style);
                }

                colorScale.MapColorScaleTitle.Caption = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.MapColorScaleTitle.Caption);
            }
            if (mapColorScaleExp.BottomMargin != null)
            {
                colorScale.BottomMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.BottomMargin));
            }
            if (mapColorScaleExp.TopMargin != null)
            {
                colorScale.TopMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.TopMargin));
            }
            if (mapColorScaleExp.RightMargin != null)
            {
                colorScale.RightMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.RightMargin));
            }
            if (mapColorScaleExp.LeftMargin != null)
            {
                colorScale.LeftMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.LeftMargin));
            }
            if (mapColorScaleExp.TickMarkLength != null)
            {
                colorScale.TickMarkLength = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.TickMarkLength));
            }
            if (mapColorScaleExp.ActionInfo != null)
            {
                colorScale.ActionInfo = GetActionInfoExpVal(mapColorScaleExp.ActionInfo);
            }

            colorScale.ColorBarBorderColor = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.ColorBarBorderColor);
            colorScale.DockOutsideViewport = TryEnum<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.DockOutsideViewport));
            colorScale.Hidden = TryEnum<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.Hidden));
            colorScale.HideEndLabels = TryEnum<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.HideEndLabels));
            colorScale.LabelBehaviour = TryEnum<LabelBehaviour>(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.LabelBehaviour));
            colorScale.LabelFormat = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.LabelFormat);
            colorScale.LabelPlacement = TryEnum<LabelPlacement>(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.LabelPlacement));
            colorScale.LabelInterval = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.LabelInterval));
            colorScale.NoDataText = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.NoDataText);
            colorScale.Position = TryEnum<Positions>(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.Position));
            colorScale.RangeGapColor = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.RangeGapColor);
            colorScale.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.ToolTip);
            colorScale.ZIndex = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(mapColorScaleExp.ZIndex));

            return colorScale;
        }

        private MapLocationExpVal GetMapLocationExpVal(MapLocationExp mapLocationExp)
        {
            MapLocationExpVal maplocation = new MapLocationExpVal();
            maplocation.Left = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapLocationExp.Left));
            maplocation.Top = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapLocationExp.Top));
            maplocation.Unit = TryParse<Unit>(this.Model.ExpressionEngine.GetEvalExpressionString(mapLocationExp.Unit));
            return maplocation;
        }

        private MapSizeExpVal GetMapSizeExpVal(MapSizeExp mapSizeExp)
        {
            MapSizeExpVal mapsize = new MapSizeExpVal();
            mapsize.Height = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapSizeExp.Height));
            mapsize.Width = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapSizeExp.Width));
            mapsize.Unit = TryParse<Unit>(this.Model.ExpressionEngine.GetEvalExpressionString(mapSizeExp.Unit));
            return mapsize;
        }

        private MapDistanceScaleExpVal GetMapDistanceScaleExpVal(MapDistanceScaleExp mapDistanceScaleExp)
        {
            MapDistanceScaleExpVal distanceScale = new MapDistanceScaleExpVal();

            if (distanceScale.Border != null)
            {
                distanceScale.Border = GetBorderExpVal(mapDistanceScaleExp.Border);
            }
            if (distanceScale.Border != null)
            {
                distanceScale.BackGroundExpVal = GetBackgroundExpVal(mapDistanceScaleExp.BackGroundExp);
            }
            if (distanceScale.Border != null)
            {
                distanceScale.Style = GetFontStyleExpVal(mapDistanceScaleExp.Style);
            }
            if (mapDistanceScaleExp.MapLocation != null)
            {
                distanceScale.MapLocation = GetMapLocationExpVal(mapDistanceScaleExp.MapLocation);
            }
            if (mapDistanceScaleExp.MapSize != null)
            {
                distanceScale.MapSize = GetMapSizeExpVal(mapDistanceScaleExp.MapSize);
            }
            if (mapDistanceScaleExp.BottomMargin != null)
            {
                distanceScale.BottomMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapDistanceScaleExp.BottomMargin));
            }
            if (mapDistanceScaleExp.TopMargin != null)
            {
                distanceScale.TopMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapDistanceScaleExp.TopMargin));
            }
            if (mapDistanceScaleExp.RightMargin != null)
            {
                distanceScale.RightMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapDistanceScaleExp.RightMargin));
            }
            if (mapDistanceScaleExp.LeftMargin != null)
            {
                distanceScale.LeftMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapDistanceScaleExp.LeftMargin));
            }
            if (mapDistanceScaleExp.ActionInfo != null)
            {
                distanceScale.ActionInfo = GetActionInfoExpVal(mapDistanceScaleExp.ActionInfo);
            }

            distanceScale.DockOutsideViewport = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapDistanceScaleExp.DockOutsideViewport));
            distanceScale.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapDistanceScaleExp.Hidden));
            distanceScale.ScaleBorderColor = this.Model.ExpressionEngine.GetEvalExpressionString(mapDistanceScaleExp.ScaleBorderColor);
            distanceScale.Position = TryParse<Positions>(this.Model.ExpressionEngine.GetEvalExpressionString(mapDistanceScaleExp.Position));
            distanceScale.ScaleColor = this.Model.ExpressionEngine.GetEvalExpressionString(mapDistanceScaleExp.ScaleColor);
            distanceScale.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(mapDistanceScaleExp.ToolTip);
            distanceScale.ZIndex = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(mapDistanceScaleExp.ZIndex));

            return distanceScale;
        }

        private MapLayerExpVal GetMapLayerExpVal(MapLayerExp maplayerExp)
        {
            MapLayerExpVal layer = new MapLayerExpVal();
            layer.MaximumZoom = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(maplayerExp.MaximumZoom));
            layer.MinimumZoom = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(maplayerExp.MinimumZoom));
            layer.Name = this.Model.ExpressionEngine.GetEvalExpressionString(maplayerExp.Name);
            layer.Transparency = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(maplayerExp.Transparency));
            layer.VisibilityMode = TryEnum<VisibilityMode>(this.Model.ExpressionEngine.GetEvalExpressionString(maplayerExp.VisibilityMode));

            return layer;
        }

        private MapTitleExpVal GetMapTitleExpVal(MapTitleExp titleExp)
        {
            MapTitleExpVal title = new MapTitleExpVal();
            if (titleExp.Border != null)
            {
                title.Border = GetBorderExpVal(titleExp.Border);
            }
            if (titleExp.BackGroundExp != null)
            {
                title.BackGroundExpVal = GetBackgroundExpVal(titleExp.BackGroundExp);
            }
            if (titleExp.Style != null)
            {
                title.Style = GetFontStyleExpVal(titleExp.Style);
            }
            if (titleExp.MapLocation != null)
            {
                title.MapLocation = GetMapLocationExpVal(titleExp.MapLocation);
            }
            if (titleExp.MapSize != null)
            {
                title.MapSize = GetMapSizeExpVal(titleExp.MapSize);
            }
            if (titleExp.BottomMargin != null)
            {
                title.BottomMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(titleExp.BottomMargin));
            }
            if (titleExp.TopMargin != null)
            {
                title.TopMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(titleExp.TopMargin));
            }
            if (titleExp.RightMargin != null)
            {
                title.RightMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(titleExp.RightMargin));
            }
            if (titleExp.LeftMargin != null)
            {
                title.LeftMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(titleExp.LeftMargin));
            }
            if (titleExp.TextShadowOffset != null)
            {
                title.TextShadowOffset = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(titleExp.TextShadowOffset));
            }
            if (titleExp.ActionInfo != null)
            {
                title.ActionInfo = GetActionInfoExpVal(titleExp.ActionInfo);
            }

            title.DockOutsideViewport = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(titleExp.DockOutsideViewport));
            title.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(titleExp.Hidden));
            title.Angle = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(titleExp.Angle));
            title.Name = this.Model.ExpressionEngine.GetEvalExpressionString(titleExp.Name);
            title.Position = TryParse<Positions>(this.Model.ExpressionEngine.GetEvalExpressionString(titleExp.Position));
            title.Text = this.Model.ExpressionEngine.GetEvalExpressionString(titleExp.Text);
            title.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(titleExp.ToolTip);
            title.ZIndex = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(titleExp.ZIndex));

            return title;
        }

        private MapLegendExpVal GetMapLegendExpVal(MapLegendExp maplegend)
        {
            MapLegendExpVal legend = new MapLegendExpVal();

            if (maplegend.Style != null)
            {
                legend.Border = GetBorderExpVal(maplegend.Border);
            }
            if (maplegend.Style != null)
            {
                legend.BackGroundExpVal = GetBackgroundExpVal(maplegend.BackGroundExp);
            }
            if (maplegend.Style != null)
            {
                legend.Style = GetFontStyleExpVal(maplegend.Style);
            }
            if (maplegend.MapLocation != null)
            {
                legend.MapLocation = GetMapLocationExpVal(maplegend.MapLocation);
            }
            if (maplegend.MapSize != null)
            {
                legend.MapSize = GetMapSizeExpVal(maplegend.MapSize);
            }
            if (maplegend.MapLegendTitle != null)
            {
                legend.MapLegendTitle = new MapLegendTitleExpVal();

                if (maplegend.MapLegendTitle.Style != null)
                {
                    legend.MapLegendTitle.BackGroundExpVal = GetBackgroundExpVal(maplegend.MapLegendTitle.BackGroundExp);
                }
                if (maplegend.MapLegendTitle.Style != null)
                {
                    legend.MapLegendTitle.Style = GetFontStyleExpVal(maplegend.MapLegendTitle.Style);
                }

                legend.MapLegendTitle.Caption = this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.MapLegendTitle.Caption);
                legend.MapLegendTitle.TitleSeparator = TryEnum<Separator>(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.MapLegendTitle.TitleSeparator));
                legend.MapLegendTitle.TitleSeparatorColor = this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.MapLegendTitle.TitleSeparatorColor);
            }
            if (maplegend.BottomMargin != null)
            {
                legend.BottomMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.BottomMargin));
            }
            if (maplegend.TopMargin != null)
            {
                legend.TopMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.TopMargin));
            }
            if (maplegend.RightMargin != null)
            {
                legend.RightMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.RightMargin));
            }
            if (maplegend.LeftMargin != null)
            {
                legend.LeftMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.LeftMargin));
            }
            if (maplegend.ActionInfo != null)
            {
                legend.ActionInfo = GetActionInfoExpVal(maplegend.ActionInfo);
            }
            if (maplegend.MinFontSize != null)
            {
                legend.MinFontSize = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.MinFontSize));
            }

            legend.AutoFitTextDisabled = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.AutoFitTextDisabled));
            legend.DockOutsideViewport = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.DockOutsideViewport));
            legend.EquallySpacedItems = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.EquallySpacedItems));
            legend.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.Hidden));
            legend.InterlacedRows = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.InterlacedRows));
            legend.InterlacedRowsColor = this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.InterlacedRowsColor);
            legend.Layout = TryEnum<RDL.DOM.Layout>(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.Layout));
            legend.Name = this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.Name);
            legend.Position = TryEnum<Positions>(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.Position));
            legend.TextWrapThreshold = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.TextWrapThreshold));
            legend.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.ToolTip);
            legend.ZIndex = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(maplegend.ZIndex));

            return legend;
        }

        private MapViewportExpVal GetMapViewportExpVal(MapViewportExp mapViewportExp)
        {
            MapViewportExpVal Viewport = new MapViewportExpVal();

            if (mapViewportExp.Border != null)
            {
                Viewport.Border = GetBorderExpVal(mapViewportExp.Border);
            }
            if (mapViewportExp.BackGroundExp != null)
            {
                Viewport.BackGroundExpVal = GetBackgroundExpVal(mapViewportExp.BackGroundExp);
            }
            if (mapViewportExp.Style != null)
            {
                Viewport.Style = GetFontStyleExpVal(mapViewportExp.Style);
            }
            if (mapViewportExp.MapMeridians != null)
            {
                Viewport.MapMeridians = new MapGridLinesExpVal();

                if (mapViewportExp.MapMeridians.Border != null)
                {
                    Viewport.MapMeridians.Border = GetBorderExpVal(mapViewportExp.MapMeridians.Border);
                }
                if (mapViewportExp.MapMeridians.BackGroundExp != null)
                {
                    Viewport.MapMeridians.BackGroundExpVal = GetBackgroundExpVal(mapViewportExp.MapMeridians.BackGroundExp);
                }
                if (mapViewportExp.MapMeridians.Style != null)
                {
                    Viewport.MapMeridians.Style = GetFontStyleExpVal(mapViewportExp.MapMeridians.Style);
                }

                Viewport.MapMeridians.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapMeridians.Hidden));
                Viewport.MapMeridians.Interval = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapMeridians.Interval));
                Viewport.MapMeridians.LabelPosition = TryParse<LabelPosition>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapMeridians.LabelPosition));
                Viewport.MapMeridians.ShowLabels = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapMeridians.ShowLabels));
            }
            if (mapViewportExp.MapLimits != null)
            {
                Viewport.MapLimits = new MapLimitsExpVal();
                Viewport.MapLimits.MaximumX = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapLimits.MaximumX));
                Viewport.MapLimits.MaximumY = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapLimits.MaximumY));
                Viewport.MapLimits.MinimumX = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapLimits.MinimumX));
                Viewport.MapLimits.MinimumY = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapLimits.MinimumY));
            }
            if (mapViewportExp.MapLocation != null)
            {
                Viewport.MapLocation = GetMapLocationExpVal(mapViewportExp.MapLocation);
            }
            if (mapViewportExp.MapParallels != null)
            {
                Viewport.MapParallels = new MapGridLinesExpVal();

                if (mapViewportExp.MapParallels.Border != null)
                {
                    Viewport.MapParallels.Border = GetBorderExpVal(mapViewportExp.MapParallels.Border);
                }
                if (mapViewportExp.MapParallels.BackGroundExp != null)
                {
                    Viewport.MapParallels.BackGroundExpVal = GetBackgroundExpVal(mapViewportExp.MapParallels.BackGroundExp);
                }
                if (mapViewportExp.MapParallels.Style != null)
                {
                    Viewport.MapParallels.Style = GetFontStyleExpVal(mapViewportExp.MapParallels.Style);
                }
                Viewport.MapParallels.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapParallels.Hidden));
                Viewport.MapParallels.Interval = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapParallels.Interval));
                Viewport.MapParallels.LabelPosition = TryParse<LabelPosition>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapParallels.LabelPosition));
                Viewport.MapParallels.ShowLabels = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapParallels.ShowLabels));
            }
            if (mapViewportExp.MapSize != null)
            {
                Viewport.MapSize = GetMapSizeExpVal(mapViewportExp.MapSize);
            }
            if (mapViewportExp.MapView != null)
            {
                Viewport.MapView = GetMapViewExpVal(mapViewportExp.MapView);
            }

            if (mapViewportExp.BottomMargin != null)
            {
                Viewport.BottomMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.BottomMargin));
            }
            if (mapViewportExp.TopMargin != null)
            {
                Viewport.TopMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.TopMargin));
            }
            if (mapViewportExp.RightMargin != null)
            {
                Viewport.RightMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.RightMargin));
            }
            if (mapViewportExp.LeftMargin != null)
            {
                Viewport.LeftMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.LeftMargin));
            }
            if (mapViewportExp.ContentMargin != null)
            {
                Viewport.ContentMargin = new Size(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.ContentMargin));
            }

            Viewport.MaximumZoom = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MaximumZoom));
            Viewport.MinimumZoom = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MinimumZoom));
            Viewport.GridUnderContent = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.GridUnderContent));
            Viewport.ProjectionCenterX = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.ProjectionCenterX));
            Viewport.ProjectionCenterY = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.ProjectionCenterY));
            Viewport.MapCoordinateSystem = TryEnum<MapCoordinateSystem>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapCoordinateSystem));
            Viewport.SimplificationResolution = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.SimplificationResolution));
            Viewport.MapProjection = TryEnum<MapProjection>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.MapProjection));
            Viewport.ZIndex = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewportExp.ZIndex));

            return Viewport;
        }

        private MapViewExpVal GetMapViewExpVal(MapViewExp mapViewExp)
        {
            MapViewExpVal viewExpVal = new MapViewExpVal();
            if (!string.IsNullOrEmpty(mapViewExp.CenterX) || !string.IsNullOrEmpty(mapViewExp.CenterY))
            {
                viewExpVal.CenterX = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewExp.CenterX));
                viewExpVal.CenterY = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewExp.CenterY));
            }
            if (mapViewExp.MapBindingFieldPairs != null)
            {
                viewExpVal.LayerName = this.Model.ExpressionEngine.GetEvalExpressionString(mapViewExp.LayerName);
                viewExpVal.MapBindingFieldPairs = GetMapBindingFieldExpVal(mapViewExp.MapBindingFieldPairs);
            }

            viewExpVal.Zoom = TryParse<float>(this.Model.ExpressionEngine.GetEvalExpressionString(mapViewExp.Zoom));

            return viewExpVal;
        }

        private BackGroundExpVal GetBackgroundExpVal(BackGroundExp backGroundExp)
        {
            BackGroundExpVal background = new BackGroundExpVal();
            background.BackgroundColor = this.Model.ExpressionEngine.GetEvalExpressionString(backGroundExp.BackgroundColor);
            background.BackgroundGradientType = TryEnum<BackgroundGradientTypes>(this.Model.ExpressionEngine.GetEvalExpressionString(backGroundExp.BackgroundGradientType));
            background.BackgroundGradientEndColor = this.Model.ExpressionEngine.GetEvalExpressionString(backGroundExp.BackgroundGradientEndColor);
            background.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.Model.ExpressionEngine.GetEvalExpressionString(backGroundExp.BackgroundHatchType));

            return background;
        }

        private BorderExpvalProperties GetBorderExpVal(BorderExpProperties borderExpProperties)
        {
            BorderExpvalProperties border = new BorderExpvalProperties();
            border.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(borderExpProperties.BorderBrush);

            if (borderExpProperties.BorderStyle != null)
            {
                border.BorderStyle = TryEnum<BorderStyles>(this.Model.ExpressionEngine.GetEvalExpressionString(borderExpProperties.BorderStyle));
            }
            if (mapPropertiesExp.Border.Default.Thickness != null)
            {
                border.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(borderExpProperties.Thickness)).FloatValue;
            }

            return border;
        }

        public override IReportItemModeler GetModel()
        {
            MapModel mapModel = new MapModel();
            mapModel.IsTablixChild = this.IsTablixChild;
            mapModel.IsTablixInnerChild = this.IsTablixInnerChild;
            mapModel.ModelType = this.ModelType;
            mapModel.Model = this.Model;
            mapModel.ReportItem = this.ReportItem;
            mapModel.Name = this.Name;
            mapModel.Top = this.Top;
            mapModel.Left = this.Left;
            mapModel.Width = this.Width;
            mapModel.Height = this.Height;
            mapModel.mapPropertiesExp = this.mapPropertiesExp;
            mapModel.Hidden = this.Hidden;
            mapModel.DataSetFields = this.DataSetFields;

            return mapModel;
        }

        public override void DisposeEvalObjects()
        {
            this.MapProperties = null;
            base.DisposeEvalObjects();
        }

        public override void DisposeReportItemObj()
        {
            this.Model = null;
            this.ReportItem = null;
            this.DataSetFields = null;
            this.DataSource = null;
            this.FieldValues = null;
            this.RowNumbers = null;

            if (this.IsTablixInnerChild)
            {
                this.FlowLayoutInfo = null;
            }
            base.DisposeReportItemObj();
        }

        public override void UpdateHeight(double firstPageHeight, LayoutReportItemModel locationInfo, double preferredHeight, ReportItemViewMode itemViewMode)
        {
            locationInfo.ActualHeight = this.Height;
        }

        public override void UpdateWidth(double firstPageWidth, LayoutReportItemModel locationInfo, double preferredWidth, ReportItemViewMode itemViewMode)
        {
            locationInfo.ActualWidth = this.Width;
        }

        #endregion

        #region DataType Convert Wapper Method

        // clr generic datatype converter 
        public T TryParse<T>(object value)
        {
            try
            {
                T retValue = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                return retValue;
            }
            catch
            {
                return default(T);
            }
        }

        // common enum type converter
        public T TryEnum<T>(string enumMember)
        {
            try
            {
                T value = (T)Enum.Parse(typeof(T), enumMember, true);
                return value;
            }
            catch
            {
                return default(T);
            }
        }

        #endregion
    }

    internal class PointXY
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    internal class ShapeActionInfo
    {
        public List<List<PointXY>> Points { get; set; }
        public TextboxActionInfoExpVal ActionInfo { get; set; }
    }

    #region MapDatasCollection

    public class ReportingMapData
    {
        public object Value { get; set; }
        public object DisplayLabel { get; set; }
        public object BubbleValuePath { get; set; }
        public object ColorValuePath { get; set; }
        public object ColorRangeValue { get; set; }
        internal TextboxActionInfoExpVal ShapeActionInfo { get; set; }
    }

    #endregion
}
