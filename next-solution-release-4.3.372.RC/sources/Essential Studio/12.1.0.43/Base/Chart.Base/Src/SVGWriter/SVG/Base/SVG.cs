#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Contains the SVG DOM constants (element, attributes names, defined values...).
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public static class SVG
    {
        #region Constants

        #region Names
        /// <summary>
        /// The name of "ENTITY" element of XML DOM.
        /// </summary>
        public const string NAME_ENTITY = "ENTITY";

        /// <summary>
        /// The name of "line" element of SVG DOM.
        /// </summary>
        public const string NAME_LINE = "line";

        /// <summary>
        /// The name of "ellipse" element of SVG DOM.
        /// </summary>
        public const string NAME_ELLIPSE = "ellipse";

        /// <summary>
        /// The name of "path" element of SVG DOM.
        /// </summary>
        public const string NAME_PATH = "path";

        /// <summary>
        /// The name of "polygon" element of SVG DOM.
        /// </summary>
        public const string NAME_POLYGON = "polygon";

        /// <summary>
        /// The name of "polyline" element of SVG DOM.
        /// </summary>
        public const string NAME_POLYLINE = "polyline";

        /// <summary>
        /// The name of "rect" element of SVG DOM.
        /// </summary>
        public const string NAME_RECT = "rect";

        /// <summary>
        /// The name of "text" element of SVG DOM.
        /// </summary>
        public const string NAME_TEXT = "text";

        /// <summary>
        /// The name of "svg" element of SVG DOM.
        /// </summary>
        public const string NAME_SVG = "svg";

        /// <summary>
        /// The name of "g" element of SVG DOM.
        /// </summary>
        public const string NAME_G = "g";

        /// <summary>
        /// The name of "defs" element of SVG DOM.
        /// </summary>
        public const string NAME_DEFS = "defs";

        /// <summary>
        /// The name of "linearGradient" element of SVG DOM.
        /// </summary>
        public const string NAME_LINEAR_GRADIENT = "linearGradient";

        /// <summary>
        /// The name of "radialGradient" element of SVG DOM.
        /// </summary>
        public const string NAME_RADIAL_GRADIENT = "radialGradient";

        /// <summary>
        /// The name of "pattern" element of SVG DOM.
        /// </summary>
        public const string NAME_PATTERN = "pattern";

        /// <summary>
        /// The name of "stop" element of SVG DOM.
        /// </summary>
        public const string NAME_STOP = "stop";

        /// <summary>
        /// The name of "image" element of SVG DOM.
        /// </summary>
        public const string NAME_IMAGE = "image";

        /// <summary>
        /// The name of "circle" element of SVG DOM.
        /// </summary>
        public const string NAME_CIRCLE = "circle";

        /// <summary>
        /// The name of "clipPath" element of SVG DOM.
        /// </summary>
        public const string NAME_CLIP_PATH = "clipPath";

        #endregion

        #region Attributes
        /// <summary>
        /// The name of "id" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_ID = "id";

        /// <summary>
        /// The name of "x" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_X = "x";

        /// <summary>
        /// The name of "y" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_Y = "y";

        /// <summary>
        /// The name of "width" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_WIDTH = "width";

        /// <summary>
        /// The name of "height" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_HEIGHT = "height";
        
        /// <summary>
        /// The name of "x1" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_X1 = "x1";

        /// <summary>
        /// The name of "x2" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_X2 = "x2";

        /// <summary>
        /// The name of "y1" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_Y1 = "y1";

        /// <summary>
        /// The name of "y2" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_Y2 = "y2";

        /// <summary>
        /// The name of "cx" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_CX = "cx";

        /// <summary>
        /// The name of "rx" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_RX = "rx";

        /// <summary>
        /// The name of "cy" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_CY = "cy";

        /// <summary>
        /// The name of "cy" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_RY = "ry";

        /// <summary>
        /// The name of "r" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_R = "r";

        /// <summary>
        /// The name of "fx" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_FX = "fx";

        /// <summary>
        /// The name of "fy" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_FY = "fy";

        /// <summary>
        /// The name of "dx" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_DX = "dx";

        /// <summary>
        /// The name of "dy" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_DY = "dy";


        /// <summary>
        /// The name of "d" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_DATA = "d";

        /// <summary>
        /// The name of "image" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_IMAGE = "image";

        /// <summary>
        /// The name of "points" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_POINTS = "points";

        /// <summary>
        /// The name of "style" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_STYLE = "style";

        /// <summary>
        /// The name of "textLength" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_TEXT_LENGTH = "textLength";

        /// <summary>
        /// The name of "LengthAdjust" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_LENGTH_ADJUST = "LengthAdjust";
        
        /// <summary>
        /// The name of "version" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_VERSION = "version";

        /// <summary>
        /// The name of "stroke" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_STROKE = "stroke";

        /// <summary>
        /// The name of "stroke-width" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_STROKE_WIDTH = "stroke-width";

        /// <summary>
        /// The name of "stroke-linecap" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_STROKE_LINECAP = "stroke-linecap";

        /// <summary>
        /// The name of "stroke-linejoin" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_STROKE_LINEJOIN = "stroke-linejoin";

        /// <summary>
        /// The name of "stroke-miterlimit" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_STROKE_MITERLIMIT = "stroke-miterlimit";

        /// <summary>
        /// The name of "stroke-dasharray" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_STROKE_DASHARRAY = "stroke-dasharray";

        /// <summary>
        /// The name of "stroke-opacity" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_STROKE_OPACITY = "stroke-opacity";

        /// <summary>
        /// The name of "stroke-dashoffset" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_STROKE_DASHOFFSET = "stroke-dashoffset";

        /// <summary>
        /// The name of "rotate" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_ROTATE = "rotate";

        /// <summary>
        /// The name of "fill" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_FILL = "fill";

        /// <summary>
        /// The name of "fill-opacity" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_FILL_OPACITY = "fill-opacity";

        /// <summary>
        /// The name of "fill-rule" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_FILL_FULE = "fill-rule";

        /// <summary>
        /// The name of "offset" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_OFFSET = "offset";

        /// <summary>
        /// The name of "stop-color" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_STOP_COLOR = "stop-color";

        /// <summary>
        /// The name of "stop-opacity" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_STOP_OPACITY = "stop-opacity";

        /// <summary>
        /// The name of "spreadMethod" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_SPREAD_METHOD = "spreadMethod";

        /// <summary>
        /// The name of "gradientUnits" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_GRADIENT_UNITS = "gradientUnits";

        /// <summary>
        /// The name of "opacity" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_OPACITY = "opacity";

        /// <summary>
        /// The name of "font-family" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_FONT_FAMILY = "font-family";

        /// <summary>
        /// The name of "font-weight" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_FONT_WEIGHT = "font-weight";

        /// <summary>
        /// The name of "font-style" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_FONT_STYLE = "font-style";

        /// <summary>
        /// The name of "font-variant" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_FONT_VARIANT = "font-variant";

        /// <summary>
        /// The name of "font-stretch" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_FONT_STRETCH = "font-stretch";

        /// <summary>
        /// The name of "font-size" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_FONT_SIZE = "font-size";

        /// <summary>
        /// The name of "font-size-adjust" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_FONT_SIZE_ADJUST = "font-size-adjust";

        /// <summary>
        /// The name of "transform" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_TRANSFORM = "transform";

        /// <summary>
        /// The name of "text-decoration" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_TEXT_DECORATION = "text-decoration";

        /// <summary>
        /// The name of "gradientTransform" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_GRADIENT_TRANSFORM = "gradientTransform";

        /// <summary>
        /// The name of "patternTransform" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_PATTERN_TRANSFORM = "patternTransform";

        /// <summary>
        /// The name of "viewBox" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_VIEW_BOX = "viewBox";

        /// <summary>
        /// The name of "preserveAspectRatio" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_PRESERVE_ASPECT_RATIO = "preserveAspectRatio";

        /// <summary>
        /// The name of "patternContentUnits" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_PATTERN_CONTENT_UNITS = "patternContentUnits";

        /// <summary>
        /// The name of "patternUnits" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_PATTERN_UNITS = "patternUnits";

        /// <summary>
        /// The name of "shape-rendering" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_SHAPE_RENDERING = "shape-rendering";
        
        /// <summary>
        /// The name of "clip-rule" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_CLIP_RULE = "clip-rule";

        /// <summary>
        /// The name of "clip-path" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_CLIP_PATH = "clip-path";

        /// <summary>
        /// The name of "clipPathUnits" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_CLIP_PATH_UNITS = "clipPathUnits";


        /// <summary>
        /// The name of "xlink:href" attribute of SVG DOM.
        /// </summary>
        public const string ATTR_HREF = "xlink:href";
        #endregion

        #region Values
        /// <summary>
        /// The "none" value.
        /// </summary>
        public const string VALUE_NONE = "none";

        /// <summary>
        /// The "M" value.
        /// </summary>
        public const string VALUE_M = "M";

        /// <summary>
        /// The "L" value.
        /// </summary>
        public const string VALUE_L = "L";

        /// <summary>
        /// The "C" value.
        /// </summary>
        public const string VALUE_C = "C";

        /// <summary>
        /// The "H" value.
        /// </summary>
        public const string VALUE_H = "H";

        /// <summary>
        /// The "V" value.
        /// </summary>
        public const string VALUE_V = "V";

        /// <summary>
        /// The "Q" value.
        /// </summary>
        public const string VALUE_Q = "Q";

        /// <summary>
        /// The "Z" value.
        /// </summary>
        public const string VALUE_Z = "Z";

        /// <summary>
        /// The "pad" value.
        /// </summary>
        public const string VALUE_PAD = "pad";

        /// <summary>
        /// The "url" value.
        /// </summary>
        public const string VALUE_URL = "url";

        /// <summary>
        /// The "bold" value.
        /// </summary>
        public const string VALUE_BOLD = "bold";

        /// <summary>
        /// The "lighter" value.
        /// </summary>
        public const string VALUE_LIGHTER = "lighter";

        /// <summary>
        /// The "100" value.
        /// </summary>
        public const string VALUE_100 = "100";

        /// <summary>
        /// The "200" value.
        /// </summary>
        public const string VALUE_200 = "200";

        /// <summary>
        /// The "300" value.
        /// </summary>
        public const string VALUE_300 = "300";

        /// <summary>
        /// The "400" value.
        /// </summary>
        public const string VALUE_400 = "400";

        /// <summary>
        /// The "500" value.
        /// </summary>
        public const string VALUE_500 = "500";

        /// <summary>
        /// The "600" value.
        /// </summary>
        public const string VALUE_600 = "600";

        /// <summary>
        /// The "700" value.
        /// </summary>
        public const string VALUE_700 = "700";

        /// <summary>
        /// The "800" value.
        /// </summary>
        public const string VALUE_800 = "800";

        /// <summary>
        /// The "900" value.
        /// </summary>
        public const string VALUE_900 = "900";

        /// <summary>
        /// The "butt" value.
        /// </summary>
        public const string VALUE_BUTT = "butt";

        /// <summary>
        /// The "bevel" value.
        /// </summary>
        public const string VALUE_BEVEL = "bevel";

        /// <summary>
        /// The "miter" value.
        /// </summary>
        public const string VALUE_MITER = "miter";

        /// <summary>
        /// The "medium" value.
        /// </summary>
        public const string VALUE_MEDIUM = "medium";

        /// <summary>
        /// The "round" value.
        /// </summary>
        public const string VALUE_ROUND = "round";

        /// <summary>
        /// The "square" value.
        /// </summary>
        public const string VALUE_SQUARE = "square";

        /// <summary>
        /// The "bolder" value.
        /// </summary>
        public const string VALUE_BOLDER = "bolder";

        /// <summary>
        /// The "italic" value.
        /// </summary>
        public const string VALUE_ITALIC = "italic";

        /// <summary>
        /// The "normal" value.
        /// </summary>
        public const string VALUE_NORMAL = "normal";

        /// <summary>
        /// The "wider" value.
        /// </summary>
        public const string VALUE_WIDER = "wider";

        /// <summary>
        /// The "narrower" value.
        /// </summary>
        public const string VALUE_NARROWER = "narrower";

        /// <summary>
        /// The "nonzero" value.
        /// </summary>
        public const string VALUE_NONZERO = "nonzero";

        /// <summary>
        /// The "evenodd" value.
        /// </summary>
        public const string VALUE_EVENODD = "evenodd";

        /// <summary>
        /// The "condensed" value.
        /// </summary>
        public const string VALUE_CONDENSED = "condensed";

        /// <summary>
        /// The "semi-condensed" value.
        /// </summary>
        public const string VALUE_SEMI_CONDENSED = "semi-condensed";

        /// <summary>
        /// The "extra-condensed" value.
        /// </summary>
        public const string VALUE_EXTRA_CONDENSED = "extra-condensed";

        /// <summary>
        /// The "ultra-condensed" value.
        /// </summary>
        public const string VALUE_ULTRA_CONDENSED = "ultra-condensed";

        /// <summary>
        /// The "expanded" value.
        /// </summary>
        public const string VALUE_EXPANDED = "expanded";

        /// <summary>
        /// The "semi-expanded" value.
        /// </summary>
        public const string VALUE_SEMI_EXPANDED = "semi-expanded";

        /// <summary>
        /// The "extra-expanded" value.
        /// </summary>
        public const string VALUE_EXTRA_EXPANDED = "extra-expanded";

        /// <summary>
        /// The "ultra-expanded" value.
        /// </summary>
        public const string VALUE_ULTRA_EXPANDED = "ultra-expanded";

        /// <summary>
        /// The "reflect" value.
        /// </summary>
        public const string VALUE_REPEAT = "repeat";

        /// <summary>
        /// The "reflect" value.
        /// </summary>
        public const string VALUE_REFLECT = "reflect";

        /// <summary>
        /// The "underline" value.
        /// </summary>
        public const string VALUE_UNDERLINE = "underline";

        /// <summary>
        /// The "crispEdges" value.
        /// </summary>
        public const string VALUE_CRISP_EDGES = "crispEdges";

        /// <summary>
        /// The "objectBoundingBox" value.
        /// </summary>
        public const string VALUE_OBJECT_BOUND_BOX = "objectBoundingBox";

        /// <summary>
        /// The "userSpaceOnUse" value.
        /// </summary>
        public const string VALUE_USER_SPACE_ON_USE = "userSpaceOnUse";

        /// <summary>
        /// The "data:;base64," value.
        /// </summary>
        public const string VALUE_IMAGE_TYPE = "data:;base64,";

        /// <summary>
        /// The "oblique" value.
        /// </summary>
        public const string VALUE_OBLIQUE = "oblique";

        /// <summary>
        /// The "small-caps" value.
        /// </summary>
        public const string VALUE_SMALL_CAPS = "small-caps";
        
        /// <summary>
        /// The "matrix" value.
        /// </summary>
        public const string VALUE_MATRIX = "matrix";

        /// <summary>
        /// The "translate" value.
        /// </summary>
        public const string VALUE_TRANSLATE = "translate";

        /// <summary>
        /// The "scale" value.
        /// </summary>
        public const string VALUE_SCALE = "scale";

        /// <summary>
        /// The "rotate" value.
        /// </summary>
        public const string VALUE_ROTATE = "rotate";

        /// <summary>
        /// The "skewX" value.
        /// </summary>
        public const string VALUE_SKEW_X = "skewX";

        /// <summary>
        /// The "skewY" value.
        /// </summary>
        public const string VALUE_SKEW_Y = "skewY";

        /// <summary>
        /// The "spacing" value.
        /// </summary>
        public const string VALUE_SPACING = "spacing";

        /// <summary>
        /// The "spacingAndGlyphs" value.
        /// </summary>
        public const string VALUE_SPACING_AND_GLYPHS = "spacingAndGlyphs";

        /// <summary>
        /// The "%" value.
        /// </summary>
        public const string VALUE_PERCENT = "%";

        /// <summary>
        /// The "em" value.
        /// </summary>
        public const string VALUE_EM = "em";

        /// <summary>
        /// The "ex" value.
        /// </summary>
        public const string VALUE_EX = "ex";

        /// <summary>
        /// The "px" value.
        /// </summary>
        public const string VALUE_PX = "px";

        /// <summary>
        /// The "cm" value.
        /// </summary>
        public const string VALUE_CM = "cm";

        /// <summary>
        /// The "mm" value.
        /// </summary>
        public const string VALUE_MM = "mm";

        /// <summary>
        /// The "in" value.
        /// </summary>
        public const string VALUE_IN = "in";

        /// <summary>
        /// The "pt" value.
        /// </summary>
        public const string VALUE_PT = "pt";

        /// <summary>
        /// The "pc" value.
        /// </summary>
        public const string VALUE_PC = "pc";

        /// <summary>
        /// The "cx" value.
        /// </summary>
        public const string VALUE_CX = "cx";

        /// <summary>
        /// The "cy" value.
        /// </summary>
        public const string VALUE_CY = "cy";

        /// <summary>
        /// The "fx" value.
        /// </summary>
        public const string VALUE_FX = "fx";

        /// <summary>
        /// The "fy" value.
        /// </summary>
        public const string VALUE_FY = "fy";

        /// <summary>
        /// The "#" value.
        /// </summary>
        public const string VALUE_HEX_COLOR = "#";

        /// <summary>
        /// The "rgb" value.
        /// </summary>
        public const string VALUE_RGB_COLOR = "rgb";

        /// <summary>
        /// The "r" value.
        /// </summary>
        public const string VALUE_R = "r";

        /// <summary>
        /// The "g" value.
        /// </summary>
        public const string VALUE_G = "g";

        /// <summary>
        /// The "b" value.
        /// </summary>
        public const string VALUE_B = "b";
        #endregion

        #endregion
    }
}
