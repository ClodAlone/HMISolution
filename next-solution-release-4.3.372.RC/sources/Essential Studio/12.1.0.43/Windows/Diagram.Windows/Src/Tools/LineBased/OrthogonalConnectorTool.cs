#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Orthogonal Connector Tool is used to connect nodes in an orthogonal manner 
    /// by providing its start point and end point.
    /// </summary>
    public class OrthogonalConnectorTool
        : LineBaseTool
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="OrthogonalConnectorTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public OrthogonalConnectorTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("OrthogonalLinkTool"))
        {
            this.ToolCursor = this.ActionCursor = Cursors.Cross;
        }
        #endregion

        #region Class override
        /// <summary>
        /// Creates the line shape node.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <returns>The orthogonal line connector</returns>
        protected override Node CreateNode(PointF ptStart, PointF ptEnd)
        {
            LineBase toReturn;

            //flip the orthogonal connector path while holding Shift key
            if (Control.ModifierKeys == Keys.Shift)
                toReturn = new OrthogonalConnector(ptStart, ptEnd, true);
            else
                toReturn = new OrthogonalConnector(ptStart, ptEnd);

            ((ConnectorBase)toReturn).LineRoutingEnabled = true;
            SetDecorator(toReturn);

            return toReturn;
        }
        #endregion
    }
}
