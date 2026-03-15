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
using System.Text;
using Syncfusion.RDL.DOM;

namespace Syncfusion.RDL.ItemModel
{
    internal class MapItemExpVal
    {
        public string ToolTip
        {
            get;
            set;
        }

        public BorderExpval Border
        {
            get;
            set;
        }

        public string DataSetName
        {
            get;
            set;
        }
        
        public string DocumentMapLabel
        {
            get;
            set;
        }

        public bool Hidden
        {
            get;
            set;
        }

        public int Zindex
        {
            get;
            set;
        }

        public AntiAliasing AntiAliasing
        {
            get;
            set;
        }

        public TextAntiAliasingQuality TextAntiAliasingQuality
        {
            get;
            set;
        }

        public float ShadowIntensity
        {
            get;
            set;
        }

        public int MaximumSpatialElementCount
        {
            get;
            set;
        }

        public int MaximumTotalPointCount
        {
            get;
            set;
        }

        public string TileLanguage
        {
            get;
            set;
        }
        
        public MapViewportExpVal MapViewport
        {
            get;
            set;
        }

        public List<MapPolygonLayerExpVal> MapPolygonLayers
        {
            get;
            set;
        }

        public List<MapPointLayerExpVal> MapPointLayers
        {
            get;
            set;
        }

        public List<MapLineLayerExpVal> MapLineLayers
        {
            get;
            set;
        }
        
        public List<MapTileLayerExpVal> MapTileLayers
        {
            get;
            set;
        }
        
        public List<MapLegendExpVal> MapLegends
        {
            get;
            set;
        }

        public List<MapTitleExpVal> MapTitles
        {
            get;
            set;
        }

        public MapDistanceScaleExpVal MapDistanceScale
        {
            get;
            set;
        }

        public MapColorScaleExpVal MapColorScale
        {
            get;
            set;
        }

        public MapBorderSkinExpVal MapBorderSkin
        {
            get;
            set;
        }

        public List<MapDataRegionExpVal> MapDataRegions
        {
            get;
            set;
        }

        public BackGroundExpVal Background
        {
            get;
            set;
        }

        public TextboxActionInfoExpVal ActionInfo
        {
            get;
            set;
        }
    }

    internal class MapDataRegionExpVal
    {
        public string Name { get; set; }
        public string DataSetName { get; set; }
        public FiltersExpVal Filters { get; set; }
        public MapMemberExpVal MapMember { get; set; }
    }
    
    internal class MapMemberExpVal
    {
        public GroupExpVal Group { get; set; }
        public MapMemberExpVal MapMember { get; set; }
    }

    internal class MapViewportExpVal : MapSubItemExpVal
    {
        public MapCoordinateSystem MapCoordinateSystem { get; set; }
        public MapProjection MapProjection { get; set; }
        public float ProjectionCenterX { get; set; }
        public float ProjectionCenterY { get; set; }
        public MapLimitsExpVal MapLimits { get; set; }
        public float MaximumZoom { get; set; }
        public float MinimumZoom { get; set; }
        public MapViewExpVal MapView { get; set; }
        public Size ContentMargin { get; set; }
        public MapGridLinesExpVal MapMeridians { get; set; }
        public MapGridLinesExpVal MapParallels { get; set; }
        public bool GridUnderContent { get; set; }
        public float SimplificationResolution { get; set; }
    }

    internal class MapLocationExpVal
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public Unit Unit { get; set; }
    }

    internal class MapSizeExpVal
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public Unit Unit { get; set; }
    }

    internal class MapLimitsExpVal
    {
        public float MinimumX { get; set; }
        public float MinimumY { get; set; }
        public float MaximumX { get; set; }
        public float MaximumY { get; set; }
    }

    internal class MapViewExpVal
    {
        public float Zoom { get; set; } 
        public float CenterX { get; set; }
        public float CenterY { get; set; }
        public string LayerName { get; set; }
        public List<MapBindingFieldPairExpVal> MapBindingFieldPairs { get; set; }
    }

    internal class MapBindingFieldPairExpVal
    {
        public string FieldName { get; set; }
        public string BindingExpression { get; set; }
    }

    internal class MapGridLinesExpVal
    {
        public bool Hidden { get; set; }
        public BorderExpvalProperties Border { get; set; }
        public BackGroundExpVal BackGroundExpVal { get; set; }
        public StyleExpval Style { get; set; }
        public float Interval { get; set; }
        public bool ShowLabels { get; set; }
        public LabelPosition LabelPosition { get; set; }
    }

    internal class MapLegendExpVal : MapDockableSubItemExpVal
    {
        public string Name { get; set; }
        public RDL.DOM.Layout Layout { get; set; }
        public MapLegendTitleExpVal MapLegendTitle { get; set; }
        public bool AutoFitTextDisabled { get; set; }
        public Size MinFontSize { get; set; }
        public bool InterlacedRows { get; set; }
        public string InterlacedRowsColor { get; set; }
        public bool EquallySpacedItems { get; set; }
        public int TextWrapThreshold { get; set; }
    }
    
    internal class MapDockableSubItemExpVal : MapSubItemExpVal
    {
        public Positions Position { get; set; }
        public bool DockOutsideViewport { get; set; }
        public bool Hidden { get; set; }
        public TextboxActionInfoExpVal ActionInfo { get; set; }
        public string ToolTip { get; set; }
    }

    internal class MapSubItemExpVal
    {
        public BorderExpvalProperties Border { get; set; }
        public BackGroundExpVal BackGroundExpVal { get; set; }
        public StyleExpval Style { get; set; }
        public MapLocationExpVal MapLocation { get; set; }
        public MapSizeExpVal MapSize { get; set; }
        public Size LeftMargin { get; set; }
        public Size RightMargin { get; set; }
        public Size TopMargin { get; set; }
        public Size BottomMargin { get; set; }
        public int ZIndex { get; set; }
    }

    internal class MapLegendTitleExpVal
    {
        public string Caption { get; set; }
        public Separator TitleSeparator { get; set; }
        public string TitleSeparatorColor { get; set; }
        public BackGroundExpVal BackGroundExpVal { get; set; }
        public StyleExpval Style { get; set; }
    }

    internal class MapTitleExpVal : MapDockableSubItemExpVal
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public float Angle { get; set; }
        public Size TextShadowOffset { get; set; }
    }

    internal class MapDistanceScaleExpVal : MapDockableSubItemExpVal
    {
        public string ScaleColor { get; set; }
        public string ScaleBorderColor { get; set; }
    }

    internal class MapColorScaleExpVal : MapDockableSubItemExpVal
    {
        public MapColorScaleTitleExpVal MapColorScaleTitle { get; set; }
        public Size TickMarkLength { get; set; }
        public string ColorBarBorderColor { get; set; }
        public int LabelInterval { get; set; }
        public string LabelFormat { get; set; }
        public LabelPlacement LabelPlacement { get; set; }
        public LabelBehaviour LabelBehaviour { get; set; }
        public bool HideEndLabels { get; set; }
        public string RangeGapColor { get; set; }
        public string NoDataText { get; set; }
    }

    internal class MapColorScaleTitleExpVal
    {
        public string Caption { get; set; }
        public BorderExpvalProperties Border { get; set; }
        public BackGroundExpVal BackGroundExpVal { get; set; }
        public StyleExpval Style { get; set; }
    }

    internal class MapBorderSkinExpVal
    {
        public BorderSkinType MapBorderSkinType { get; set; }
        public BorderExpvalProperties Border { get; set; }
        public BackGroundExpVal BackGroundExpVal { get; set; }
        public StyleExpval Style { get; set; }
    }

    internal class MapLayerExpVal
    {
        public string Name { get; set; }
        public VisibilityMode VisibilityMode { get; set; }
        public float MinimumZoom { get; set; }
        public float MaximumZoom { get; set; }
        public float Transparency { get; set; }
    }

    internal class MapPolygonLayerExpVal : MapVectorLayerExpVal
    {
        public MapPolygonTemplateExpVal MapPolygonTemplate { get; set; }
        public MapPolygonRulesExpVal MapPolygonRules { get; set; }
        public MapPointTemplateExpVal MapCenterPointTemplate { get; set; }
        public MapPointRulesExpVal MapCenterPointRules { get; set; }
        public List<MapPolygonExpVal> MapPolygons { get; set; }
    }

    internal class MapPointLayerExpVal : MapVectorLayerExpVal
    {
        public MapPointTemplateExpVal MapPointTemplate { get; set; }
        public MapPointRulesExpVal MapPointRules { get; set; }
        public List<MapPointExpVal> MapPoints { get; set; }
    }

    internal class MapLineLayerExpVal : MapVectorLayerExpVal
    {
        public MapLineTemplateExpVal MapLineTemplate { get; set; }
        public MapLineRulesExpVal MapLineRules { get; set; }
        public List<MapLineExpVal> MapLines { get; set; }
    }

    internal class MapPolygonExpVal : MapSpatialElementExpVal
    {
        public bool UseCustomPolygonTemplate { get; set; }
        public MapPolygonTemplateExpVal MapPolygonTemplate { get; set; }
        public bool UseCustomCenterPointTemplate { get; set; }
        public MapPointTemplateExpVal MapCenterPointTemplate { get; set; }
    }

    internal class MapSpatialElementExpVal
    {
        public Dictionary<string,object> MapFields { get; set; }
    }

    internal class MapFieldExpVal
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    internal class MapVectorLayerExpVal
    {
        public string MapDataRegionName { get; set; }
        public List<MapBindingFieldPairExpVal> MapBindingFieldPairs { get; set; }
        public List<MapFieldDefinitionExpVal> MapFieldDefinitions { get; set; }
        public MapSpatialDataExpVal MapSpatialData { get; set; }
        public string DataElementName { get; set; }
        public DataElementOutputs DataElementOutput { get; set; }
        public MapLayerExpVal MapLayerExpVal { get; set; }
    }

    internal class MapSpatialDataExpVal
    {
        public MapShapefileExpVal MapShapefile { get; set; }
        public MapSpatialDataSetExpVal MapSpatialDataSetExp { get; set; }
        public MapSpatialDataRegionExpVal MapSpatialDataRegionExp { get; set; }
    }

    internal class MapShapefileExpVal
    {
        public string Source { get; set; }
        public List<string> MapFieldNames { get; set; }
    }

    internal class MapSpatialDataRegionExpVal
    {
        public string VectorData { get; set; }
    }

    internal class MapSpatialDataSetExpVal
    {
        public string DataSetName { get; set; }
        public string SpatialField { get; set; }
        public List<string> MapFieldNames { get; set; }
    }

    internal class MapFieldDefinitionExpVal
    {
        public string Name { get; set; }
        public DataTypes DataType { get; set; }
    }

    internal class MapPolygonTemplateExpVal : MapSpatialElementTemplateExpVal
    {
        public float ScaleFactor { get; set; }
        public float CenterPointOffsetX { get; set; }
        public float CenterPointOffsetY { get; set; }
        public BooleanOptions ShowLabel { get; set; }
        public LabelPlacement LabelPlacement { get; set; }
    }

    internal class MapSpatialElementTemplateExpVal
    {
        public bool Hidden { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public BorderExpvalProperties BorderExpval { get; set; }
        public StyleExpval StyleExpval { get; set; }
        public BackGroundExpVal BackGroundExpVal { get; set; }
        public string Label { get; set; }
        public string ToolTip { get; set; }
        public TextboxActionInfoExpVal ActionInfo { get; set; }
        public string DataElementName { get; set; }
        public DataElementOutputs DataElementOutput { get; set; }
        public string DataElementLabel { get; set; }
    }
    
    internal class MapPolygonRulesExpVal
    {
        public MapColorRuleExpVal MapColorRule { get; set; }
    }

    internal class MapColorRuleExpVal : MapAppearanceRuleExpVal
    {
        public MapColorPaletteRuleExpVal ColorPalette { get; set; }
        public List<string> MapCustomColors { get; set; }
        public MapColorRangeRuleExpVal MapColorRangeRule { get; set; }
        public bool ShowInColorScale { get; set; }
    }

    internal class MapColorPaletteRuleExpVal
    {
        public MapColorPalette Palette { get; set; }
    }

    internal class MapColorRangeRuleExpVal
    {
        public string StartColor { get; set; }
        public string MiddleColor { get; set; }
        public string EndColor { get; set; }
    }

    internal class MapAppearanceRuleExpVal
    {
        public string DataValue { get; set; }
        public DistributionType DistributionType { get; set; }
        public int BucketCount { get; set; }
        public string StartValue { get; set; }
        public string EndValue { get; set; }
        public List<MapBucketExpVal> MapBuckets { get; set; }
        public string LegendName { get; set; }
        public string LegendText { get; set; }
        public string DataElementName { get; set; }
        public DataElementOutputs DataElementOutput { get; set; }
    }

    internal class MapBucketExpVal
    {
        public string StartValue { get; set; }
        public string EndValue { get; set; }
    }
    
    internal class MapMarkerTemplateExpVal : MapPointTemplateExpVal
    {
        public MapMarkerExpVal MapMarker { get; set; }
    }

    internal class MapPointTemplateExpVal : MapSpatialElementTemplateExpVal
    {
        public Size Size { get; set; }
        public LabelPlacement LabelPlacement { get; set; }
    }
    
    internal class MapMarkerRuleExpVal : MapAppearanceRuleExpVal
    {
        public List<MapMarkerExpVal> MapMarkers { get; set; }
    }

    internal class MapMarkerExpVal
    {
        public MapMarkerStyle MapMarkerStyle { get; set; }
        public MapMarkerImageExpVal MapMarkerImage { get; set; }
    }

    internal class MapMarkerImageExpVal
    {
        public Source Source { get; set; }
        public string Value { get; set; }
        public string MIMEType { get; set; }
        public string TransparentColor { get; set; }
        public ResizeMode ResizeMode { get; set; }
    }
    
    internal class MapPointRulesExpVal
    {
        public MapSizeRuleExpVal MapSizeRule { get; set; }
        public MapColorRuleExpVal MapColorRule { get; set; }
        public MapMarkerRuleExpVal MapMarkerRule { get; set; }
    }
    
    internal class MapSizeRuleExpVal : MapAppearanceRuleExpVal
    {
        public Size StartSize { get; set; }
        public Size EndSize { get; set; }
    }

    internal class MapPointExpVal : MapSpatialElementExpVal
    {
        public bool UseCustomPointTemplate { get; set; }
        public MapPointTemplateExpVal MapPointTemplate { get; set; }
    }

    internal class MapLineTemplateExpVal : MapSpatialElementTemplateExpVal
    {
        public Size Width { get; set; }
        public LabelPlacement LabelPlacement { get; set; }
    }

    internal class MapLineExpVal : MapSpatialElementExpVal
    {
        public bool UseCustomLineTemplate { get; set; }
        public MapLineTemplateExpVal MapLineTemplate { get; set; }
    }

    internal class MapLineRulesExpVal
    {
        public MapSizeRuleExpVal MapSizeRule { get; set; }
        public MapColorRuleExpVal MapColorRule { get; set; }
    }
    
    internal class MapTileLayerExpVal : MapLayerExpVal
    {
        public TileStyle TileStyle { get; set; }
        public List<MapTileExpVal> MapTiles { get; set; }
    }

    internal class MapTileExpVal
    {
        public string Name { get; set; }
        public string TileData { get; set; }
        public string MIMEType { get; set; }
    }
    
    internal class BackGroundExpVal
    {
        public string BackgroundColor { get; set; }
        public string BackgroundGradientEndColor { get; set; }
        public BackgroundHatchTypes BackgroundHatchType { get; set; }
        public BackgroundGradientTypes BackgroundGradientType { get; set; }
    }
}