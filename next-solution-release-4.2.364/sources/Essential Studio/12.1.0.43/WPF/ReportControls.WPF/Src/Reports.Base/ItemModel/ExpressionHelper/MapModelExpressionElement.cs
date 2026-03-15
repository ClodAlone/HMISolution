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
    internal class MapItemExp
    {
        public string ToolTip
        {
            get;
            set;
        }

        public BorderExp Border
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

        public string Hidden
        {
            get;
            set;
        }

        public string Zindex
        {
            get;
            set;
        }

        public string AntiAliasing
        {
            get;
            set;
        }

        public string TextAntiAliasingQuality
        {
            get;
            set;
        }

        public string ShadowIntensity
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

        public BackGroundExp Background
        {
            get;
            set;
        }

        public MapViewportExp MapViewport
        {
            get;
            set;
        }

        public List<MapPolygonLayerExp> MapPolygonLayersExp
        {
            get;
            set;
        }

        public List<MapLineLayerExp> MapLineLayersExp
        {
            get;
            set;
        }

        public List<MapPointLayerExp> MapPointLayersExp
        {
            get;
            set;
        }

        public List<MapTileLayerExp> MapTileLayersExp
        {
            get;
            set;
        }

        public List<MapLegendExp> MapLegendsExp
        {
            get;
            set;
        }

        public List<MapTitleExp> MapTitlesExp
        {
            get;
            set;
        }

        public MapDistanceScaleExp MapDistanceScale
        {
            get;
            set;
        }

        public MapColorScaleExp MapColorScale
        {
            get;
            set;
        }

        public MapBorderSkinExp MapBorderSkin
        {
            get;
            set;
        }

        public List<MapDataRegionExp> MapDataRegions
        {
            get;
            set;
        }

        public TextboxActionInfoExp ActionInfo
        {
            get;
            set;
        }
    }

    internal class MapDataRegionExp
    {
        public string Name { get; set; }
        public string DataSetName { get; set; }
        public FiltersExp Filters { get; set; }
        public MapMemberExp MapMember { get; set; }
    }

    internal class MapMemberExp
    {
        public GroupExp Group { get; set; }
        public MapMemberExp MapMember { get; set; }
    }

    internal class MapViewportExp : MapSubItemExp
    {
        public string MapCoordinateSystem { get; set; }
        public string MapProjection { get; set; }
        public string ProjectionCenterX { get; set; }
        public string ProjectionCenterY { get; set; }
        public MapLimitsExp MapLimits { get; set; }
        public string MaximumZoom { get; set; }
        public string MinimumZoom { get; set; }
        public MapViewExp MapView { get; set; }
        public string ContentMargin { get; set; }
        public MapGridLinesExp MapMeridians { get; set; }
        public MapGridLinesExp MapParallels { get; set; }
        public string GridUnderContent { get; set; }
        public string SimplificationResolution { get; set; }
    }

    internal class MapLocationExp
    {
        public string Left { get; set; }
        public string Top { get; set; }
        public string Unit { get; set; }
    }

    internal class MapSizeExp
    {
        public string Width { get; set; }
        public string Height { get; set; }
        public string Unit { get; set; }
    }

    internal class MapLimitsExp
    {
        public string MinimumX { get; set; }
        public string MinimumY { get; set; }
        public string MaximumX { get; set; }
        public string MaximumY { get; set; }
    }

    internal class MapViewExp
    {
        public string Zoom { get; set; }
        public string CenterX { get; set; }
        public string CenterY { get; set; }
        public string LayerName { get; set; }
        public List<MapBindingFieldPairExp> MapBindingFieldPairs { get; set; }
    }

    internal class MapBindingFieldPairExp
    {
        public string FieldName { get; set; }
        public string BindingExpression { get; set; }
    }

    internal class MapGridLinesExp
    {
        public string Hidden { get; set; }
        public BorderExpProperties Border { get; set; }
        public BackGroundExp BackGroundExp { get; set; }
        public StyleExp Style { get; set; }
        public string Interval { get; set; }
        public string ShowLabels { get; set; }
        public string LabelPosition { get; set; }
    }

    internal class MapLegendExp : MapDockableSubItemExp
    {
        public string Name { get; set; }
        public string Layout { get; set; }
        public MapLegendTitleExp MapLegendTitle { get; set; }
        public string AutoFitTextDisabled { get; set; }
        public string MinFontSize { get; set; }
        public string InterlacedRows { get; set; }
        public string InterlacedRowsColor { get; set; }
        public string EquallySpacedItems { get; set; }
        public string TextWrapThreshold { get; set; }
    }

    internal class MapDockableSubItemExp : MapSubItemExp
    {
        public string Position { get; set; }
        public string DockOutsideViewport { get; set; }
        public string Hidden { get; set; }
        public TextboxActionInfoExp ActionInfo { get; set; }
        public string ToolTip { get; set; }
    }

    internal class MapSubItemExp
    {
        public BorderExpProperties Border { get; set; }
        public BackGroundExp BackGroundExp { get; set; }
        public StyleExp Style { get; set; }
        public MapLocationExp MapLocation { get; set; }
        public MapSizeExp MapSize { get; set; }
        public string LeftMargin { get; set; }
        public string RightMargin { get; set; }
        public string TopMargin { get; set; }
        public string BottomMargin { get; set; }
        public string ZIndex { get; set; }
    }

    internal class MapLegendTitleExp
    {
        public string Caption { get; set; }
        public string TitleSeparator { get; set; }
        public string TitleSeparatorColor { get; set; }
        public StyleExp Style { get; set; }
        public BackGroundExp BackGroundExp { get; set; }
    }

    internal class MapTitleExp : MapDockableSubItemExp
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Angle { get; set; }
        public string TextShadowOffset { get; set; }
    }

    internal class MapDistanceScaleExp : MapDockableSubItemExp
    {
        public string ScaleColor { get; set; }
        public string ScaleBorderColor { get; set; }
    }

    internal class MapColorScaleExp : MapDockableSubItemExp
    {
        public MapColorScaleTitleExp MapColorScaleTitle { get; set; }
        public string TickMarkLength { get; set; }
        public string ColorBarBorderColor { get; set; }
        public string LabelInterval { get; set; }
        public string LabelFormat { get; set; }
        public string LabelPlacement { get; set; }
        public string LabelBehaviour { get; set; }
        public string HideEndLabels { get; set; }
        public string RangeGapColor { get; set; }
        public string NoDataText { get; set; }
    }

    internal class MapColorScaleTitleExp
    {
        public string Caption { get; set; }
        public BorderExpProperties Border { get; set; }
        public BackGroundExp BackGroundExp { get; set; }
        public StyleExp Style { get; set; }
    }

    internal class MapBorderSkinExp
    {
        public string MapBorderSkinType { get; set; }
        public BorderExpProperties Border { get; set; }
        public BackGroundExp BackGroundExp { get; set; }
        public StyleExp Style { get; set; }
    }

    internal class MapLayerExp
    {
        public string Name { get; set; }
        public string VisibilityMode { get; set; }
        public string MinimumZoom { get; set; }
        public string MaximumZoom { get; set; }
        public string Transparency { get; set; }
    }

    internal class MapPolygonLayerExp : MapVectorLayerExp
    {
        public MapPolygonTemplateExp MapPolygonTemplate { get; set; }
        public MapPolygonRulesExp MapPolygonRules { get; set; }
        public MapPointTemplateExp MapCenterPointTemplate { get; set; }
        public MapPointRulesExp MapCenterPointRules { get; set; }
        public List<MapPolygonExp> MapPolygons { get; set; }
    }

    internal class MapPointLayerExp : MapVectorLayerExp
    {
        public MapPointTemplateExp MapPointTemplate { get; set; }
        public MapPointRulesExp MapPointRules { get; set; }
        public List<MapPointExp> MapPoints { get; set; }
    }
    
    internal class MapLineLayerExp : MapVectorLayerExp
    {
        public MapLineTemplateExp MapLineTemplate { get; set; }
        public MapLineRulesExp MapLineRules { get; set; }
        public List<MapLineExp> MapLines { get; set; }
    }

    internal class MapPolygonExp : MapSpatialElementExp
    {
        public string UseCustomPolygonTemplate { get; set; }
        public MapPolygonTemplateExp MapPolygonTemplate { get; set; }
        public string UseCustomCenterPointTemplate { get; set; }
        public MapPointTemplateExp MapCenterPointTemplate { get; set; }
    }

    internal class MapSpatialElementExp
    {
        public string VectorData { get; set; }
        public List<MapFieldExp> MapFields { get; set; }
    }

    internal class MapFieldExp
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    internal class MapVectorLayerExp
    {
        public string MapDataRegionName { get; set; }
        public List<MapBindingFieldPairExp> MapBindingFieldPairs { get; set; }
        public List<MapFieldDefinitionExp> MapFieldDefinitions { get; set; }
        public MapSpatialDataExp MapSpatialData { get; set; }
        public string DataElementName { get; set; }
        public string DataElementOutput { get; set; }
        public MapLayerExp MapLayerExp { get; set; }
    }
    
    internal class MapSpatialDataExp
    {
        public MapShapefileExp MapShapefile { get; set; }
        public MapSpatialDataSetExp MapSpatialDataSetExp { get; set; }
        public MapSpatialDataRegionExp MapSpatialDataRegionExp { get; set; }
    }

    internal class MapShapefileExp
    {
        public string Source { get; set; }
        public List<string> MapFieldNames { get; set; }
    }
    
    internal class MapSpatialDataRegionExp
    {
        public string VectorData { get; set; }
    }
    
    internal class MapSpatialDataSetExp
    {
        public string DataSetName { get; set; }
        public string SpatialField { get; set; }
        public List<string> MapFieldNames { get; set; }
    }

    internal class MapFieldDefinitionExp
    {
        public string Name { get; set; }
        public string DataType { get; set; }
    }

    internal class MapPolygonTemplateExp : MapSpatialElementTemplateExp
    {
        public string ScaleFactor { get; set; }
        public string CenterPointOffsetX { get; set; }
        public string CenterPointOffsetY { get; set; }
        public string ShowLabel { get; set; }
        public string LabelPlacement { get; set; }
    }

    internal class MapSpatialElementTemplateExp
    {
        public string Hidden { get; set; }
        public string OffsetX { get; set; }
        public string OffsetY { get; set; }
        public BorderExpProperties Border { get; set; }
        public BackGroundExp Background { get; set; }
        public StyleExp StyleExp { get; set; }
        public string Label { get; set; }
        public string ToolTip { get; set; }
        public TextboxActionInfoExp ActionInfo { get; set; }
        public string DataElementName { get; set; }
        public string DataElementOutput { get; set; }
        public string DataElementLabel { get; set; }
    }
    
    internal class MapPolygonRulesExp
    {
        public MapColorRuleExp MapColorRule { get; set; }
    }

    internal class MapColorRuleExp : MapAppearanceRuleExp
    {
        public List<string> MapCustomColors { get; set; }
        public MapColorPaletteRuleExp ColorPalette { get; set; }
        public MapColorRangeRuleExp MapColorRangeRule { get; set; }
        public string ShowInColorScale { get; set; }
    }

    internal class MapColorPaletteRuleExp
    {
        public string Palette { get; set; }
    }

    internal class MapColorRangeRuleExp
    {
        public string StartColor { get; set; }
        public string MiddleColor { get; set; }
        public string EndColor { get; set; }
    }

    internal class MapAppearanceRuleExp
    {
        public string DataValue { get; set; }
        public string DistributionType { get; set; }
        public string BucketCount { get; set; }
        public string StartValue { get; set; }
        public string EndValue { get; set; }
        public List<MapBucketExp> MapBuckets { get; set; }
        public string LegendName { get; set; }
        public string LegendText { get; set; }
        public string DataElementName { get; set; }
        public string DataElementOutput { get; set; }
    }

    internal class MapBucketExp
    {
        public string StartValue { get; set; }
        public string EndValue { get; set; }
    }

    internal class MapMarkerTemplateExp : MapPointTemplateExp
    {
        public MapMarkerExp MapMarker { get; set; }
    }

    internal class MapPointTemplateExp : MapSpatialElementTemplateExp
    {
        public string Size { get; set; }
        public string LabelPlacement { get; set; }
    }
    
    internal class MapMarkerRuleExp : MapAppearanceRuleExp
    {
        public List<MapMarkerExp> MapMarkers { get; set; }
    }

    internal class MapMarkerExp
    {
        public string MapMarkerStyle { get; set; }
        public MapMarkerImageExp MapMarkerImage { get; set; }
    }

    internal class MapMarkerImageExp
    {
        public string Source { get; set; }
        public string Value { get; set; }
        public string MIMEType { get; set; }
        public string TransparentColor { get; set; }
        public string ResizeMode { get; set; }
    }
    
    internal class MapPointRulesExp
    {
        public MapSizeRuleExp MapSizeRule { get; set; }
        public MapColorRuleExp MapColorRule { get; set; }
        public MapMarkerRuleExp MapMarkerRule { get; set; }
    }
    
    internal class MapSizeRuleExp : MapAppearanceRuleExp
    {
        public string StartSize { get; set; }
        public string EndSize { get; set; }
    }

    internal class MapPointExp : MapSpatialElementExp
    {
        public string UseCustomPointTemplate { get; set; }
        public MapPointTemplateExp MapPointTemplate { get; set; }
    }

    internal class MapLineTemplateExp : MapSpatialElementTemplateExp
    {
        public string Width { get; set; }
        public string LabelPlacement { get; set; }
    }

    internal class MapLineExp : MapSpatialElementExp
    {
        public string UseCustomLineTemplate { get; set; }
        public MapLineTemplateExp MapLineTemplate { get; set; }
    }

    internal class MapLineRulesExp
    {
        public MapSizeRuleExp MapSizeRule { get; set; }
        public MapColorRuleExp MapColorRule { get; set; }
    }
    
    internal class MapTileLayerExp : MapLayerExp
    {
        public string TileStyle { get; set; }
        public List<MapTileExp> MapTiles { get; set; }
    }

    internal class MapTileExp
    {
        public string Name { get; set; }
        public string TileData { get; set; }
        public string MIMEType { get; set; }
    }

    internal class BackGroundExp
    {
        public string BackgroundColor { get; set; }
        public string BackgroundHatchType { get; set; }
        public string BackgroundGradientType { get; set; }
        public string BackgroundGradientEndColor { get; set; }
    }
}
