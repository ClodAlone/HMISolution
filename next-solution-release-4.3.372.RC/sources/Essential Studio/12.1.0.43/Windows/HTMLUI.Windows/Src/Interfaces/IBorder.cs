#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Specifies the border style of the element.
    /// </summary>
    public enum BordersStyle
    {
        /// <summary>
        /// No borders.
        /// </summary>
        None,

        /// <summary>
        /// Border is a dotted line.
        /// </summary>
        Dotted,

        /// <summary>
        /// Border is a dashed line.
        /// </summary>
        Dashed,

        /// <summary>
        /// Border is a solid line.
        /// </summary>
        Solid,

        /// <summary>
        /// Border is a double line drawn on top of the background of the object.
        /// The sum of the two single lines and the space between them equals the
        /// border width value. The border width must be at least 3 pixels wide to
        /// draw a double border.
        /// </summary>
        Double,

        /// <summary>
        /// 3-D groove is drawn in colors based on the value.
        /// </summary>
        Groove,

        /// <summary>
        /// 3-D ridge is drawn in colors based on the value.
        /// </summary>
        Ridge,

        /// <summary>
        /// 3-D inset is drawn in colors based on the value.
        /// </summary>
        Inset,

        /// <summary>
        /// 3-D outset is drawn in colors based on the value
        /// </summary>
        Outset,
    }

    /// <summary>
    /// Interface that provides the properties needed for correct show border lines of
    /// any element.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public interface IBorder
    {
        /// <summary>
        /// Gets or sets the width of the border line.
        /// </summary>
        int Width 
        { 
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the style of the border line.
        /// </summary>
        BordersStyle Style 
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the color of the border line.
        /// </summary>
        Color Color 
        { 
            get;
            set; 
        }

        /// <summary>
        /// Clones object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        IBorder Clone();
    }
}