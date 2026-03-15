#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Contains clipping members.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public interface IClipingAttribute
    {
        /// <summary>
        /// Gets or sets the clip rule.
        /// </summary>
        /// <value>The clip rule.</value>
        EClipRule ClipRule 
        {
            get;

            set; 
        }

        /// <summary>
        /// Gets or sets the identifier of clip path.
        /// </summary>
        /// <value>The clip path.</value>
        string ClipPath 
        {
            get;

            set; 
        }
    }
}
