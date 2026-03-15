#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Directed Link Tool.
    /// </summary>
    [Obsolete("DirectedLineLinkTool class is obsolete.Use DirectedLineConnectorTool class instead")]
    public class DirectedLineLinkTool : DirectedLineConnectorTool
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectedLineLinkTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public DirectedLineLinkTool(DiagramController controller)
            : base(controller)
        {
        }
        #endregion
    }

    /// <summary>
    /// Link Tool.
    /// </summary>
    [Obsolete("LinkTool class is obsolete.Use LineConnectorTool class instead")]
    public class LinkTool : LineConnectorTool
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public LinkTool(DiagramController controller)
            : base(controller)
        {
        }
        #endregion
    }

    /// <summary>
    /// Line link tool.
    /// </summary>
    [Obsolete("LineLinkTool class is obsolete.Use LineConnectorTool class instead")]
    public class LineLinkTool : LineConnectorTool
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LineLinkTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public LineLinkTool(DiagramController controller)
            : base(controller)
        {
        }
        #endregion
    }

    /// <summary>
    /// Orthogonal link tool.
    /// </summary>
    [Obsolete("OrthogonalLinkTool class is obsolete.Use OrthogonalConnectorTool class instead")]
    public class OrthogonalLinkTool : OrthogonalConnectorTool
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="OrthogonalLinkTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public OrthogonalLinkTool(DiagramController controller)
            : base(controller)
        {
        }
        #endregion
    }

    /// <summary>
    /// Arc Tool
    /// </summary>
    [System.Obsolete("ArcTool class is obsolete.Use SplineTool class instead")]
    public class ArcTool : SplineTool
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ArcTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public ArcTool(DiagramController controller)
            : base(controller)
        {
        }
        #endregion
    }
}
