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
    /// Stroke attribute interface.
    /// </summary>
    public interface IStrokeAttributes
    {
        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        /// <value>The stroke.</value>
        NoneColor Stroke { get; set; }

        /// <summary>
        /// Gets or sets the width of the stroke.
        /// </summary>
        /// <value>The width of the stroke.</value>
        Length StrokeWidth { get; set; }

        /// <summary>
        /// Gets or sets the stroke linecap.
        /// </summary>
        /// <value>The stroke linecap.</value>
        EStrokeLinecap StrokeLinecap { get; set; }

        /// <summary>
        /// Gets or sets the stroke linejoin.
        /// </summary>
        /// <value>The stroke linejoin.</value>
        EStrokeLinejoin StrokeLinejoin { get; set; }

        /// <summary>
        /// Gets or sets the stroke miterlimit.
        /// </summary>
        /// <value>The stroke miterlimit.</value>
        Number StrokeMiterlimit { get; set; }

        /// <summary>
        /// Gets or sets the stroke dasharray.
        /// </summary>
        /// <value>The stroke dasharray.</value>
        FloatArray StrokeDasharray { get; set; }

        /// <summary>
        /// Gets or sets the stroke dashoffset.
        /// </summary>
        /// <value>The stroke dashoffset.</value>
        Length StrokeDashoffset { get; set; }

        /// <summary>
        /// Gets or sets the stroke opacity.
        /// </summary>
        /// <value>The stroke opacity.</value>
        Opacity StrokeOpacity { get; set; }
    }
}
