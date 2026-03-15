#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Implements the "clipPath" element of SVG DOM.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class ClipPathElement : SuperElement
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ClipPathElement"/> class.
        /// </summary>
        public ClipPathElement()
            : base(SVG.NAME_CLIP_PATH)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the result path.
        /// </summary>
        /// <returns> Returns GraphicsPath object.</returns>
        internal GraphicsPath GetResultPath()
        {
            GraphicsPath result = new GraphicsPath();

            foreach (PathElement pe in this.Children)
            {
                if (pe != null)
                {
                    result.AddPath(pe.D.Path, false);
                }
            }

            return result;
        }
        #endregion
    }
}
