#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interactive tool for drawing orthogonal lines.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// </remarks>
    public class OrthogonalLineTool
        : LineBaseTool
    {
        #region Class initilatize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="OrthogonalLineTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public OrthogonalLineTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("OrthogonalLineTool"))
        { 
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates the line shape node.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <returns>The line shape node.</returns>
        protected override Node CreateNode(PointF ptStart, PointF ptEnd)
        {
            return null;
        }
        #endregion
    }
}
