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

namespace Syncfusion.Windows.Forms.Gauge
{   
    #region Enum
    /// <summary>
    /// Enum for different visual styles of Gauge control
    /// </summary>
    public enum ThemeStyle
    {
        /// <summary>
        /// Blue color
        /// </summary>
        Blue,

        /// <summary>
        /// silver color
        /// </summary>
        Silver,

        /// <summary>
        /// black color
        /// </summary>
        Black,

        /// <summary>
        /// Metro color
        /// </summary>
        Metro,

        /// <summary>
        ///  None
        /// </summary>
        None
    }
    /// <summary>
    /// Enum for different frametypes of Gauge control
    /// </summary>
    public enum FrameType
    {
        FullCircle,
        HalfCircle
    }
    /// <summary>
    /// Enum for different frametypes of Linear Gauge control
    /// </summary>
    public enum LinearFrameType
    {
        Vertical,
        Horizontal
    }
    /// <summary>
    /// Enum for text orientation styles in Gauge control
    /// </summary>
    public enum TextOrientation
    {
        Horizontal,
        SlideOver
    }
    /// <summary>
    /// Enum for tick placement in Gauge control
    /// </summary>
    public enum TickPlacement
    {
        Inside,
        OutSide
    }
    /// <summary>
    /// Enum for label placement in Gauge control
    /// </summary>
    public enum LabelPlacement
    {
        Inside,
        Outside
    }
    /// <summary>
    /// Enum for needle styles in Gauge control
    /// </summary>
    public enum NeedleStyle
    {
        Default,
        Advanced,
        Pointer
    }

    /// <summary>
    /// Enum for charactertype in digital gauge
    /// </summary>
    public enum CharacterType
    {
        SevenSegment,
        FourteenSegment,
        SixteenSegment,
        DotMatrixSegment
    }
    /// <summary>
    /// Enum for pointer placement in linear gauge
    /// </summary>
    public enum Placement
    {
        Near,
        Center,
        Far
    }
    #endregion
}
