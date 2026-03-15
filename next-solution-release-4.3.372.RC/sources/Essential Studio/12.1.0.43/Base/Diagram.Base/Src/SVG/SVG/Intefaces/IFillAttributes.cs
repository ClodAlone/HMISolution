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
    /// Fill attributes interface.
    /// </summary>
    public interface IFillAttributes
    {
        /// <summary>
        /// Gets or sets the fill.
        /// </summary>
        /// <value>The fill.</value>
        NoneColor Fill { get; set; }

        /// <summary>
        /// Gets or sets the fill opacity.
        /// </summary>
        /// <value>The fill opacity.</value>
        Opacity FillOpacity { get; set; }
    }
}
