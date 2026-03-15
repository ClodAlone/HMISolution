//-------------------------------------------------------------------------------------------------
// <copyright file="ChartAppearanceSettings.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Xml.Serialization;
using System.Drawing;
using Syncfusion.Olap.Common;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Represents the appearance settings in current report for OLAP Chart control.
    /// </summary>
    [Serializable]
    public class ChartAppearanceSettings : ICloneable<ChartAppearanceSettings>
    {
        #region Private Variables
        Color _areaBackground;

        Color _borderColor;

        Color _chartBackground;

        Color _interiorBackground;

        Color _xAxisForeGround;

        Color _yAxisForeGround;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAppearanceSettings"/> class.
        /// </summary>
        public ChartAppearanceSettings()
        {
            this.IsXValues = false;
            this.IsYValues = false;
            this.IsSeriesName = false;
            this.LabelsVisibility = false;
            this.ChartColorPalette = "Default";
            this.ChartDockLegendPosition = "Top";
            this.ChartType = "Column";
            this.XAxisFontFace = "Verdana";
            this.XLabelFontWeight = FontStyle.Regular.ToString();
            this.YAxisFontFace = "Verdana";
            this.YLabelFontWeight = FontStyle.Regular.ToString();
            this.YAxisForeGround = Color.Black;
            this.XAxisForeGround = Color.Black;
            this.Enable3D = false;
            this.ShowHorizontalGridLines = true;
            this.ShowVerticalGridLines = true;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the area background.
        /// </summary>
        /// <value>The area background.</value>
        [XmlIgnoreAttribute]
        public Color AreaBackground 
        { 
            get 
            { 
                return _areaBackground; 
            } 
            
            set 
            { 
                _areaBackground = value; 
            } 
        }

        /// <summary>
        /// Gets or sets the area background HTML.
        /// </summary>
        /// <value>The area background HTML.</value>
        [XmlElement("AreaBackground")]
        public string AreaBackgroundHtml 
        { 
            get 
            { 
                return ColorTranslator.ToHtml(_areaBackground); 
            } 

            set 
            { 
                _areaBackground = ColorTranslator.FromHtml(value); 
            } 
        }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>The color of the border.</value>
        [XmlIgnoreAttribute]
        public Color BorderColor 
        { 
            get 
            { 
                return _borderColor; 
            } 

            set 
            { 
                _borderColor = value; 
            } 
        }

        /// <summary>
        /// Gets or sets the border color HTML.
        /// </summary>
        /// <value>The border color HTML.</value>
        [XmlElement("BorderColor")]
        public string BorderColorHtml 
        { 
            get 
            { 
                return ColorTranslator.ToHtml(_borderColor); 
            } 

            set 
            { 
                _borderColor = ColorTranslator.FromHtml(value); 
            } 
        }

        /// <summary>
        /// Gets or sets the chart background.
        /// </summary>
        /// <value>The chart background.</value>
        [XmlIgnoreAttribute]
        public Color ChartBackground 
        { 
            get 
            { 
                return _chartBackground; 
            } 
            
            set 
            { 
                _chartBackground = value; 
            } 
        }

        /// <summary>
        /// Gets or sets the legend representation.
        /// </summary>
        /// <value>The legend representation.</value>
        public string LegendSymbol { get; set; }

        /// <summary>
        /// Gets or sets the legend representation.
        /// </summary>
        /// <value>The legend representation.</value>
        public string SymbolRepresentation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [legend check box visibility].
        /// </summary>
        /// <value>
        /// <c>true</c> if [legend check box visibility]; otherwise, <c>false</c>.
        /// </value>
        public bool DataPointValue { get; set; }

        /// <summary>
        /// Gets or sets the chart background HTML.
        /// </summary>
        /// <value>The chart background HTML.</value>
        [XmlElement("ChartBackground")]
        public string ChartBackgroundHtml 
        { 
            get 
            { 
                return ColorTranslator.ToHtml(_chartBackground); 
            } 
            
            set 
            { 
                _chartBackground = ColorTranslator.FromHtml(value); 
            } 
        }

        ///// <summary>
        ///// Gets or sets the interior chart color.
        ///// </summary>
        ///// <value>The interior chart color.</value>

        //public BrushInfo InteriorColor { get; set; }

        /// <summary>
        /// Gets or sets the background chart color.
        /// </summary>
        /// <value>The background chart color.</value>
        //public BrushInfo BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the chart color palette.
        /// </summary>
        /// <value>The chart color palette.</value>
        public string ChartColorPalette { get; set; }

        /// <summary>
        /// Gets or sets the chart dock legend position.
        /// </summary>
        /// <value>The chart dock legend position.</value>
        public string ChartDockLegendPosition { get; set; }

        /// <summary>
        /// Gets or sets the type of the chart.
        /// </summary>
        /// <value>The type of the chart.</value>
        public string ChartType { get; set; }

        /// <summary>
        /// Gets or sets the gradient angle.
        /// </summary>
        /// <value>The gradient angle.</value>
        public double GradientAngle { get; set; }

        /// <summary>
        /// Gets or sets the interior background.
        /// </summary>
        /// <value>The interior background.</value>
        [XmlIgnoreAttribute]
        public Color InteriorBackground 
        { 
            get 
            { 
                return _interiorBackground; 
            } 
            
            set 
            { 
                _interiorBackground = value; 
            } 
        }

        /// <summary>
        /// Gets or sets the interior background HTML.
        /// </summary>
        /// <value>The interior background HTML.</value>
        [XmlElement("InteriorBackground")]
        public string InteriorBackgroundHtml 
        { 
            get 
            { 
                return ColorTranslator.ToHtml(_interiorBackground); 
            } 
            
            set 
            { 
                _interiorBackground = ColorTranslator.FromHtml(value); 
            } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is circle symbol.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is circle symbol; otherwise, <c>false</c>.
        /// </value>
        public bool IsCircleSymbol { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is label template1.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is label template1; otherwise, <c>false</c>.
        /// </value>
        public bool IsLabelTemplate1 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is label template2.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is label template2; otherwise, <c>false</c>.
        /// </value>
        public bool IsLabelTemplate2 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is label template3.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is label template3; otherwise, <c>false</c>.
        /// </value>
        public bool IsLabelTemplate3 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is rectangle symbol.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is rectangle symbol; otherwise, <c>false</c>.
        /// </value>
        public bool IsRectangleSymbol { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is series name.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is series name; otherwise, <c>false</c>.
        /// </value>
        public bool IsSeriesName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is triangle symbol.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is triangle symbol; otherwise, <c>false</c>.
        /// </value>
        public bool IsTriangleSymbol { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is X values.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is X values; otherwise, <c>false</c>.
        /// </value>
        public bool IsXValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is Y values.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is Y values; otherwise, <c>false</c>.
        /// </value>
        public bool IsYValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [labels visibility].
        /// </summary>
        /// <value><c>true</c> if [labels visibility]; otherwise, <c>false</c>.</value>
        public bool LabelsVisibility { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [legend check box visibility].
        /// </summary>
        /// <value>
        /// <c>true</c> if [legend check box visibility]; otherwise, <c>false</c>.
        /// </value>
        public bool LegendCheckBoxVisibility { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [legend visibility].
        /// </summary>
        /// <value><c>true</c> if [legend visibility]; otherwise, <c>false</c>.</value>
        public bool LegendVisibility { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [expander visibility].
        /// </summary>
        /// <value><c>true</c> if [expander visibility]; otherwise, <c>false</c>.</value>
        public bool ExpanderVisibility { get; set; }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>The stroke thickness.</value>
        public double StrokeThickness { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [symbols visibility].
        /// </summary>
        /// <value><c>true</c> if [symbols visibility]; otherwise, <c>false</c>.</value>
        public bool SymbolsVisibility { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [template visibility].
        /// </summary>
        /// <value><c>true</c> if [template visibility]; otherwise, <c>false</c>.</value>
        public bool TemplateVisibility { get; set; }

        /// <summary>
        /// Gets or sets the X axis font face.
        /// </summary>
        /// <value>The X axis font face.</value>
        public string XAxisFontFace { get; set; }

        /// <summary>
        /// Gets or sets the X axis fore ground.
        /// </summary>
        /// <value>The X axis fore ground.</value>
        [XmlIgnoreAttribute]
        public Color XAxisForeGround 
        { 
            get 
            { 
                return _xAxisForeGround; 
            } 

            set 
            { 
                _xAxisForeGround = value; 
            } 
        }

        /// <summary>
        /// Gets or sets the X axis fore ground HTML.
        /// </summary>
        /// <value>The X axis fore ground HTML.</value>
        [XmlElement("XAxisForeGround")]
        public string XAxisForeGroundHtml 
        { 
            get 
            { 
                return ColorTranslator.ToHtml(_xAxisForeGround); 
            } 
            
            set 
            { 
                _xAxisForeGround = ColorTranslator.FromHtml(value); 
            } 
        }

        /// <summary>
        /// Gets or sets the X label font weight.
        /// </summary>
        /// <value>The X label font weight.</value>
        [XmlElement("XAxisLabelFontWeight")]
        public string XLabelFontWeight { get; set; }

        /// <summary>
        /// Gets or sets the Y axis font face.
        /// </summary>
        /// <value>The Y axis font face.</value>
        public string YAxisFontFace { get; set; }

        /// <summary>
        /// Gets or sets the Y axis fore ground.
        /// </summary>
        /// <value>The Y axis fore ground.</value>
        [XmlIgnoreAttribute]
        public Color YAxisForeGround 
        { 
            get 
            { 
                return _yAxisForeGround; 
            } 
            
            set 
            { 
                _yAxisForeGround = value; 
            } 
        }

        /// <summary>
        /// Gets or sets the Y axis fore ground HTML.
        /// </summary>
        /// <value>The Y axis fore ground HTML.</value>
        [XmlElement("YAxisForeGround")]
        public string YAxisForeGroundHtml 
        { 
            get 
            { 
                return ColorTranslator.ToHtml(_yAxisForeGround); 
            } 
            
            set 
            { 
                _yAxisForeGround = ColorTranslator.FromHtml(value); 
            } 
        }

        /// <summary>
        /// Gets or sets the Y label font weight.
        /// </summary>
        /// <value>The Y label font weight.</value>
        [XmlElement("YAxisLabelFontWeight")]
        public string YLabelFontWeight { get; set; }

        /// <summary>
        /// Gets or sets a boolean value to enable 3D in Chart. (Applicatble for OlapClient and OlapChart Asp.Net Only)
        /// </summary>
        /// <c>true</c> if OlapChart or OlapClient Enable3D is true; otherwise, <c>false</c>.
        public bool Enable3D { get; set; }

        /// <summary>
        /// Gets or sets boolean value to enable Horizontal Grid lines in OlapChart
        /// </summary>
        /// <c>true</c>Horizontal Grid lines will be visible; otherwise, <c>false</c>.
        [XmlElement("ShowHorizontalGridLines")]
        public bool ShowHorizontalGridLines { get; set; }

        /// <summary>
        /// Gets or sets boolean value to enable Vertical Grid lines in OlapChart
        /// </summary>
        /// <c>true</c>Vertical Grid lines will be visible; otherwise, <c>false</c>.
        [XmlElement("ShowVerticalGridLines")]
        public bool ShowVerticalGridLines { get; set; }

        #endregion

        #region Public Methods 
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>returns Chartappearance Settings</returns>
        public ChartAppearanceSettings Clone()
        {
            ChartAppearanceSettings chartApperanceSettings = new ChartAppearanceSettings();
            chartApperanceSettings.AreaBackgroundHtml = this.AreaBackgroundHtml;
            chartApperanceSettings.BorderColorHtml = this.BorderColorHtml;
            chartApperanceSettings.ChartBackgroundHtml = this.ChartBackgroundHtml;
            chartApperanceSettings.ChartColorPalette = this.ChartColorPalette;
            chartApperanceSettings.ChartDockLegendPosition = this.ChartDockLegendPosition;
            chartApperanceSettings.ChartType = this.ChartType;
            chartApperanceSettings.GradientAngle = this.GradientAngle;
            chartApperanceSettings.InteriorBackgroundHtml = this.InteriorBackgroundHtml;
            chartApperanceSettings.IsCircleSymbol = this.IsCircleSymbol;
            chartApperanceSettings.IsLabelTemplate1 = this.IsLabelTemplate1;
            chartApperanceSettings.IsLabelTemplate2 = this.IsLabelTemplate2;
            chartApperanceSettings.IsLabelTemplate3 = this.IsLabelTemplate3;
            chartApperanceSettings.IsRectangleSymbol = this.IsRectangleSymbol;
            chartApperanceSettings.IsSeriesName = this.IsSeriesName;
            chartApperanceSettings.IsTriangleSymbol = this.IsTriangleSymbol;
            chartApperanceSettings.IsXValues = this.IsXValues;
            chartApperanceSettings.IsYValues = this.IsYValues;
            chartApperanceSettings.LabelsVisibility = this.LabelsVisibility;
            chartApperanceSettings.LegendCheckBoxVisibility = this.LegendCheckBoxVisibility;
            chartApperanceSettings.LegendVisibility = this.LegendVisibility;
            chartApperanceSettings.ExpanderVisibility = this.ExpanderVisibility;
            chartApperanceSettings.StrokeThickness = this.StrokeThickness;
            chartApperanceSettings.SymbolsVisibility = this.SymbolsVisibility;
            chartApperanceSettings.TemplateVisibility = this.TemplateVisibility;
            chartApperanceSettings.XAxisFontFace = this.XAxisFontFace;
            chartApperanceSettings.XAxisForeGroundHtml = this.XAxisForeGroundHtml;
            chartApperanceSettings.XLabelFontWeight = this.XLabelFontWeight;
            chartApperanceSettings.YAxisFontFace = this.YAxisFontFace;
            chartApperanceSettings.YAxisForeGroundHtml = this.YAxisForeGroundHtml;
            chartApperanceSettings.YLabelFontWeight = this.YLabelFontWeight;
            chartApperanceSettings.DataPointValue = this.DataPointValue;
            chartApperanceSettings.SymbolRepresentation = this.SymbolRepresentation;
            chartApperanceSettings.LegendSymbol = this.LegendSymbol;
            chartApperanceSettings.Enable3D = this.Enable3D;
            chartApperanceSettings.ShowHorizontalGridLines = this.ShowHorizontalGridLines;
            chartApperanceSettings.ShowVerticalGridLines = this.ShowVerticalGridLines;
            return chartApperanceSettings;
        }
        #endregion
    }
}

