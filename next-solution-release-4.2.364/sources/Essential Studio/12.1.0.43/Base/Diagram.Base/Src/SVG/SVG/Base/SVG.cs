#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// Class for SVG.
    /// </summary>
    public class SVG
    {
        #region Constants

        #region Names
        public const string NAME_ENTITY = "ENTITY";
        public const string NAME_LINE = "line";
        public const string NAME_ELLIPSE = "ellipse";
        public const string NAME_PATH = "path";
        public const string NAME_POLYGON = "polygon";
        public const string NAME_POLYLINE = "polyline";
        public const string NAME_RECT = "rect";
        public const string NAME_TEXT = "text";
        public const string NAME_SVG = "svg";
        public const string NAME_G = "g";
        public const string NAME_DEFS = "defs";
        public const string NAME_LINEAR_GRADIENT = "linearGradient";
        public const string NAME_RADIAL_GRADIENT = "radialGradient";
        public const string NAME_PATTERN = "pattern";
        public const string NAME_STOP = "stop";
        public const string NAME_IMAGE = "image";
        public const string NAME_CIRCLE = "circle";
        public const string NAME_CLIP_PATH = "clipPath";
        #endregion

        #region Attributes
        public const string ATTR_ID = "id";

        public const string ATTR_X = "x";
        public const string ATTR_Y = "y";
        public const string ATTR_WIDTH = "width";
        public const string ATTR_HEIGHT = "height";

        public const string ATTR_X1 = "x1";
        public const string ATTR_X2 = "x2";
        public const string ATTR_Y1 = "y1";
        public const string ATTR_Y2 = "y2";

        public const string ATTR_CX = "cx";
        public const string ATTR_RX = "rx";
        public const string ATTR_CY = "cy";
        public const string ATTR_RY = "ry";
        public const string ATTR_R = "r";
        public const string ATTR_FX = "fx";
        public const string ATTR_FY = "fy";
        public const string ATTR_DX = "dx";
        public const string ATTR_DY = "dy";

        public const string ATTR_DATA = "d";
        public const string ATTR_IMAGE = "image";
        public const string ATTR_POINTS = "points";

        public const string ATTR_STYLE = "style";
        public const string ATTR_TEXT_LENGTH = "textLength";
        public const string ATTR_LENGTH_ADJUST = "LengthAdjust";

        public const string ATTR_VERSION = "version";
        public const string ATTR_STROKE = "stroke";
        public const string ATTR_STROKE_WIDTH = "stroke-width";
        public const string ATTR_STROKE_LINECAP = "stroke-linecap";
        public const string ATTR_STROKE_LINEJOIN = "stroke-linejoin";
        public const string ATTR_STROKE_MITERLIMIT = "stroke-miterlimit";
        public const string ATTR_STROKE_DASHARRAY = "stroke-dasharray";
        public const string ATTR_STROKE_OPACITY = "stroke-opacity";
        public const string ATTR_STROKE_DASHOFFSET = "stroke-dashoffset";
        public const string ATTR_ROTATE = "rotate";

        public const string ATTR_FILL = "fill";
        public const string ATTR_FILL_OPACITY = "fill-opacity";
        public const string ATTR_FILL_FULE = "fill-rule";
        public const string ATTR_OFFSET = "offset";
        public const string ATTR_STOP_COLOR = "stop-color";
        public const string ATTR_STOP_OPACITY = "stop-opacity";
        public const string ATTR_SPREAD_METHOD = "spreadMethod";
        public const string ATTR_GRADIENT_UNITS = "gradientUnits";
        public const string ATTR_OPACITY = "opacity";

        public const string ATTR_FONT_FAMILY = "font-family";
        public const string ATTR_FONT_WEIGHT = "font-weight";
        public const string ATTR_FONT_STYLE = "font-style";
        public const string ATTR_FONT_VARIANT = "font-variant";
        public const string ATTR_FONT_STRETCH = "font-stretch";
        public const string ATTR_FONT_SIZE = "font-size";
        public const string ATTR_FONT_SIZE_ADJUST = "font-size-adjust";
        public const string ATTR_TRANSFORM = "transform";
        public const string ATTR_TEXT_DECORATION = "text-decoration";
        public const string ATTR_GRADIENT_TRANSFORM = "gradientTransform";
        public const string ATTR_PATTERN_TRANSFORM = "patternTransform";
        public const string ATTR_VIEW_BOX = "viewBox";
        public const string ATTR_PRESERVE_ASPECT_RATIO = "preserveAspectRatio";
        public const string ATTR_PATTERN_CONTENT_UNITS = "patternContentUnits";
        public const string ATTR_PATTERN_UNITS = "patternUnits";
        public const string ATTR_SHAPE_RENDERING = "shape-rendering";

        public const string ATTR_CLIP_RULE = "clip-rule";
        public const string ATTR_CLIP_PATH = "clip-path";
        public const string ATTR_CLIP_PATH_UNITS = "clipPathUnits";

        public const string ATTR_HREF = "xlink:href";

        #endregion

        #region Values
        public const string VALUE_NONE = "none";
        public const string VALUE_M = "M";
        public const string VALUE_L = "L";
        public const string VALUE_C = "C";
        public const string VALUE_H = "H";
        public const string VALUE_V = "V";
        public const string VALUE_Q = "Q";
        public const string VALUE_Z = "Z";
        public const string VALUE_PAD = "pad";
        public const string VALUE_URL = "url";
        public const string VALUE_BOLD = "bold";
        public const string VALUE_LIGHTER = "lighter";
        public const string VALUE_100 = "100";
        public const string VALUE_200 = "200";
        public const string VALUE_300 = "300";
        public const string VALUE_400 = "400";
        public const string VALUE_500 = "500";
        public const string VALUE_600 = "600";
        public const string VALUE_700 = "700";
        public const string VALUE_800 = "800";
        public const string VALUE_900 = "900";
        public const string VALUE_BUTT = "butt";
        public const string VALUE_BEVEL = "bevel";
        public const string VALUE_MITER = "miter";
        public const string VALUE_MEDIUM = "medium";
        public const string VALUE_ROUND = "round";
        public const string VALUE_SQUARE = "square";
        public const string VALUE_BOLDER = "bolder";
        public const string VALUE_ITALIC = "italic";
        public const string VALUE_NORMAL = "normal";
        public const string VALUE_WIDER = "wider";
        public const string VALUE_NARROWER = "narrower";
        public const string VALUE_NONZERO = "nonzero";
        public const string VALUE_EVENODD = "evenodd";
        public const string VALUE_CONDENSED = "condensed";
        public const string VALUE_SEMI_CONDENSED = "semi-condensed";
        public const string VALUE_EXTRA_CONDENSED = "extra-condensed";
        public const string VALUE_ULTRA_CONDENSED = "ultra-condensed";
        public const string VALUE_EXPANDED = "expanded";
        public const string VALUE_SEMI_EXPANDED = "semi-expanded";
        public const string VALUE_EXTRA_EXPANDED = "extra-expanded";
        public const string VALUE_ULTRA_EXPANDED = "ultra-expanded";
        public const string VALUE_REPEAT = "repeat";
        public const string VALUE_REFLECT = "reflect";
        public const string VALUE_UNDERLINE = "underline";
        public const string VALUE_CRISP_EDGES = "crispEdges";
        public const string VALUE_OBJECT_BOUND_BOX = "objectBoundingBox";
        public const string VALUE_USER_SPACE_ON_USE = "userSpaceOnUse";
        public const string VALUE_IMAGE_TYPE = "data:;base64,";
        public const string VALUE_OBLIQUE = "oblique";
        public const string VALUE_SMALL_CAPS = "small-caps";

        public const string VALUE_MATRIX = "matrix";
        public const string VALUE_TRANSLATE = "translate";
        public const string VALUE_SCALE = "scale";
        public const string VALUE_ROTATE = "rotate";
        public const string VALUE_SKEW_X = "skewX";
        public const string VALUE_SKEW_Y = "skewY";
        public const string VALUE_SPACING = "spacing";
        public const string VALUE_SPACING_AND_GLYPHS = "spacingAndGlyphs";

        public const string VALUE_PERCENT = "%";
        public const string VALUE_EM = "em";
        public const string VALUE_EX = "ex";
        public const string VALUE_PX = "px";
        public const string VALUE_CM = "cm";
        public const string VALUE_MM = "mm";
        public const string VALUE_IN = "in";
        public const string VALUE_PT = "pt";
        public const string VALUE_PC = "pc";
        public const string VALUE_CX = "cx";
        public const string VALUE_CY = "cy";
        public const string VALUE_FX = "fx";
        public const string VALUE_FY = "fy";

        public const string VALUE_HEX_COLOR = "#";
        public const string VALUE_RGB_COLOR = "rgb";
        public const string VALUE_R = "r";
        public const string VALUE_G = "g";
        public const string VALUE_B = "b";
        #endregion

        #endregion
    }
}
