#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///  PanMode is a Direction where the panning operation should be done
    /// </summary>
    public enum PanMode
    {
        /// <summary>
        ///  Panning is performed in left direction
        /// </summary>
        Left,
        /// <summary>
        ///  Panning is performed in Right direction
        /// </summary>
        Right,
        /// <summary>
        ///  Panning is performed in Top direction
        /// </summary>
        Top,
        /// <summary>
        ///  Panning is performed in Bottom direction
        /// </summary>
        Bottom,
        /// <summary>
        ///  Panning is performed in LeftTop direction
        /// </summary>
        LeftTop,
        /// <summary>
        ///  Panning is performed in LeftBottom direction
        /// </summary>
        LeftBottom,
        /// <summary>
        ///  Panning is performed in RightTop direction
        /// </summary>
        RightTop,
        /// <summary>
        ///  Panning is performed in RightBottom direction
        /// </summary>
        RightBottom
    }

    /// <summary>
    ///  Legend Position is a location where the legend need to place.
    /// </summary>
    public enum LegendPosition
    {
        /// <summary>
        ///Legend is placed at TopRight.
        /// </summary>
        TopRight,
        /// <summary>
        ///Legend is placed at TopLeft.
        /// </summary>
        TopLeft,
        /// <summary>
        ///Legend is placed at TopMiddle.
        /// </summary>
        TopMiddle,
        /// <summary>
        ///Legend is placed at BottomRight
        /// </summary>
        BottomRight,
        /// <summary>
        ///Legend is placed at BottomLeft
        /// </summary>
        BottomLeft,
        /// <summary>
        ///Legend is placed at BottomMiddle
        /// </summary>
        BottomMiddle,
        /// <summary>
        ///Legend is placed at LeftMiddle
        /// </summary>
        LeftMiddle,
        /// <summary>
        ///Legend is placed at RightMiddle
        /// </summary>
        RightMiddle,
        /// <summary>
        ///Legend is placed default position
        /// </summary>
        Default
    }

    /// <summary>
    ///  ColorPaletteMode is option to choose how the color should be apply for the Map
    /// shapes
    /// </summary>
    public enum ColorPaletteMode
    {
        /// <summary>
        ///Colors are arranged sequentially
        /// </summary>
        Sequential,

        /// <summary>
        ///Colors are arranged randomly
        /// </summary>
        Random
    }

    /// <summary>
    ///  ColorPalette is set of different colors to bel applied for the map
    /// </summary>
    public enum ColorPalettes
    {

        /// <summary>
        ///None of color applied for Map shapes
        /// </summary>
        None,
        /// <summary>
        ///Set of colors applied for Map shapes
        /// </summary>
        ColorPalette1,
        /// <summary>
        ///Set of colors applied for Map shapes
        /// </summary>
        ColorPalette2,
        /// <summary>
        ///Set of colors applied for Map shapes
        /// </summary>
        ColorPalette3,
        /// <summary>
        ///Set of colors applied for Map shapes
        /// </summary>
        ColorPalette4,
        /// <summary>
        ///Set of colors applied for Map shapes
        /// </summary>
        ColorPalette5,
        /// <summary>
        ///Set of colors applied for Map shapes
        /// </summary>
        MetroPalette,
        /// <summary>
        ///User defined colors applied for Map shapes
        /// </summary>
        CustomColorPalette
    }

    /// <summary>
    ///  This helps to choose the Mode od Zooming by mouse left button
    /// </summary>
    public enum ZoomingMode
    {
        /// <summary>
        ///Zooming is done in Single Click
        /// </summary>
        SingleClick,
        /// <summary>
        ///Zooming is done in Double Click
        /// </summary>
        DoubleClick
    }

    /// <summary>
    ///  This helps to choose the location where the navigation control should be placed
    /// </summary>
    public enum NavigationControlPositions
    {
        /// <summary>
        ///NavigationControl is placed at Top
        /// </summary>
        Top,
        /// <summary>
        ///NavigationControl is placed at Right
        /// </summary>
        Right,
        /// <summary>
        ///NavigationControl is placed at Bottom
        /// </summary>
        Bottom,
        /// <summary>
        ///NavigationControl is placed at Left
        /// </summary>
        Left
    }
    /// <summary>
    ///  VisualStyles helps to change the Background theme of Map control
    /// </summary>
    public enum VisualStyles
    {
        /// <summary>
        /// Default theme is set for Map Control 
        /// </summary>
        Default,
        /// <summary>
        /// Office2010Blue theme is set for Map Control 
        /// </summary>
        Office2010Blue,
        /// <summary>
        /// Office2010Silver theme is set for Map Control 
        /// </summary>
        Office2010Silver,
        /// <summary>
        /// Office2010Black theme is set for Map Control 
        /// </summary>
        Office2010Black,
        /// <summary>
        /// Blend theme is set for Map Control 
        /// </summary>
        Blend,
        /// <summary>
        /// VS2010 theme is set for Map Control 
        /// </summary>
        VS2010,
        /// <summary>
        /// Office2007Blue theme is set for Map Control 
        /// </summary>
        Office2007Blue,
        /// <summary>
        /// Office2007Silver theme is set for Map Control 
        /// </summary>
        Office2007Silver,
        /// <summary>
        /// Office2007Black theme is set for Map Control 
        /// </summary>
        Office2007Black
    }



    /// <summary>
    ///  Save mode helps to choose the Mode of saving type
    /// </summary>
    public enum SaveMode
    {
#if WPF
        /// <summary>
        /// Map is saved as xml file 
        /// </summary>
        Xml,
        /// <summary>
        /// Map is saved as ImageFile 
        /// </summary>
        Image
#else
        /// <summary>
        /// Map is saved as xml file 
        /// </summary>
        Xml
#endif
    }
    /// <summary>
    ///  PatheLabelPosition choose the location where the path lebel should place
    /// </summary>
    public enum PathLabelPosition
    {
        /// <summary>
        /// PathLabel is placed on the Map Point
        /// </summary>
        OnPoint,
        /// <summary>
        /// PathLabel is placed on the Map middle Point
        /// </summary>
        OnMiddlePoint
    }
    /// <summary>
    ///  This helps to choose the Type of LatitudeLogitudePoint
    /// </summary>
    public enum LatitudeLongitudeType
    {
        /// <summary>
        /// Custom Type is applied for Latitude and Longitude Point
        /// </summary>
        Custom,

        /// <summary>
        /// DMS Type is applied for Latitude and Longitude Point
        /// </summary>
        DMS,

        /// <summary>
        /// Decimal Type is applied for Latitude and Longitude Point
        /// </summary>
        Decimal
    }

}
