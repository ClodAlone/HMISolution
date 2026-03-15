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
#if WINRT
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Maps
{

    public enum ShapeType
    {
        /// <summary>
        /// Nullshape / placeholder record.
        /// </summary>
        NullShape = 0,

        /// <summary>
        /// Point record, for defining point locations such as a city.
        /// </summary>
        Point = 1,

        /// <summary>
        /// One or more sets of connected points. Used to represent roads,
        /// hydrography, etc.
        /// </summary>
        PolyLine = 3,

        /// <summary>
        /// One or more sets of closed figures. Used to represent political
        /// boundaries for countries, lakes, etc.
        /// </summary>
        Polygon = 5,

        /// <summary>
        /// A cluster of points represented by a single shape record.
        /// </summary>
        Multipoint = 8

        // Unsupported types:
        // PointZ = 11,        
        // PolyLineZ = 13,        
        // PolygonZ = 15,        
        // MultiPointZ = 18,        
        // PointM = 21,        
        // PolyLineM = 23,        
        // PolygonM = 25,        
        // MultiPointM = 28,        
        // MultiPatch = 31
    }

    public enum ValueType
    {
        Double,
        Range
    }

    public enum SymbolTypes
    {
        Ellipse,
        Rectangle,
        Custom
    }
    public enum VisibleChild
    {
        Symbols,
        Labels
    }
    internal struct MaxMin
    {
        internal Int32 Max;
        internal Int32 Min;
    }

    public enum ShapeColorMode
    {
        Default,
        ColorPalette,
        HeatMap
    }

    public enum ColorPalettes
    {
        Metro,
        CoolBlue,
        CustomPalette
    }

    public enum PanMode
    {
        Right,
        Left,
        Top,
        Bottom
    }

    public enum LayerChangeMode
    {
        Default,
        Automatic
    }
    public enum SymbolColorMode
    {
        Default,
        HeatMap
    }

    public enum LatLonType
    {
        DMS,
        Decimal
    }

    public enum LegendIcons
    {
        Rectangle,
        Ellipse
    }

    public enum LegendType
    {
        Bubbles,
        Layers
    }

    public enum LegendPosition
    {
        Default,
        TopLeft,
        TopCenter,
        TopRight,
        MidLeft,
        Center,
        MidRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    public enum MapViews
    {
        NormalView,
        SmartView
    }

    public enum BubbleType
    {
        Circle,
        Rectangle,
        Diamond,
        Triangle,
        Trapezoid,
        Star,
        Pentagon,
        Pushpin,
        Custom
    }

#if !WINRT
    internal static class ExtensionMethods
    {
        internal static TypeInfo GetTypeInfo(this Type type)
        {
            return new TypeInfo(type);
        }
    }


    internal class TypeInfo
    {
        internal Type Type { get; set; }

        internal TypeInfo(Type type)
        {
            Type = type;
            Assembly = Type.Assembly;
        }
        internal Assembly Assembly { get; set; }
        internal PropertyInfo GetDeclaredProperty(string name)
        {
            return this.Type.GetProperty(name);
        }
    }
#endif
}
