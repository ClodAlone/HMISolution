#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// OrgLineConnector tool is used to connect nodes in an org line shaped manner 
    /// by providing its start point and end point.
    /// </summary>
    public class OrgLineConnectorTool: LineBaseTool
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="OrgLineConnectorTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public OrgLineConnectorTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("OrgLineConnectorTool"))
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
        /// <returns>The z shaped connector</returns>
        protected override Node CreateNode(PointF ptStart, PointF ptEnd)
        {
            LineBase toReturn = new OrgLineConnector(ptStart, ptEnd);

            ((ConnectorBase)toReturn).LineRoutingEnabled = true;
            SetDecorator(toReturn);

            return toReturn;
        }
        #endregion
    }
}
