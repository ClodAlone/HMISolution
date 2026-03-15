#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// Clipping attribute interface.
    /// </summary>
    public interface IClipingAttribute
    {
        /// <summary>
        /// Gets or sets the clip rule.
        /// </summary>
        /// <value>The clip rule.</value>
        EClipRule ClipRule { get; set; }

        /// <summary>
        /// Gets or sets the clip path.
        /// </summary>
        /// <value>The clip path.</value>
        string ClipPath { get; set; }
    }
}
