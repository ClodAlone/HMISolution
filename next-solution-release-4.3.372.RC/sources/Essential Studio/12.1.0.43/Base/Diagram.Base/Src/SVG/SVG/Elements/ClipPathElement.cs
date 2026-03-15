#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing.Drawing2D;
using Syncfusion.SVG.IO;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// ClipPathElement class.
    /// </summary>
    public class ClipPathElement : SuperElement
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ClipPathElement"/> class.
        /// </summary>
        public ClipPathElement()
        {
            m_name = SVG.NAME_CLIP_PATH;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the result path.
        /// </summary>
        /// <returns>The graphics path</returns>
        public GraphicsPath GetResultPath()
        {
            GraphicsPath result = new GraphicsPath();

            for (int i = 0; i < m_children.Count; i++)
            {
                PathElement pe = m_children[i] as PathElement;

                if (pe != null)
                {
                    result.AddPath(pe.GetPath(), false);
                }
            }

            return result;
        }
        #endregion
    }
}
