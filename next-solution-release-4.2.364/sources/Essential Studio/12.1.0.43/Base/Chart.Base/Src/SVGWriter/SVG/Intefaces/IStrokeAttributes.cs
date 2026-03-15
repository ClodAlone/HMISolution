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
    /// Contains the stroke members.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
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
        /// Gets or sets the stroke line caps.
        /// </summary>
        /// <value>The stroke line caps.</value>
        EStrokeLinecap StrokeLinecap { get; set; }

        /// <summary>
        /// Gets or sets the stroke line join.
        /// </summary>
        /// <value>The stroke line join.</value>
        EStrokeLinejoin StrokeLinejoin { get; set; }

        /// <summary>
        /// Gets or sets the stroke miter limit.
        /// </summary>
        /// <value>The stroke miter limit.</value>
        Number StrokeMiterlimit { get; set; }

        /// <summary>
        /// Gets or sets the stroke dash array.
        /// </summary>
        /// <value>The stroke dash array.</value>
        FloatArray StrokeDasharray { get; set; }

        /// <summary>
        /// Gets or sets the stroke dash offset.
        /// </summary>
        /// <value>The stroke dash offset.</value>
        Length StrokeDashoffset { get; set; }

        /// <summary>
        /// Gets or sets the stroke opacity.
        /// </summary>
        /// <value>The stroke opacity.</value>
        Opacity StrokeOpacity { get; set; }
    }
}
