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
    /// Interactive tool for inserting and editing rich text objects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This tool manages the insertion of new rich text nodes into a diagram
    /// and editing of existing rich text nodes. Activating this tool causes it
    /// to track mouse down, mouse move, and mouse up events and draw a
    /// tracking rectangle. The rectangle drawn is used as the bounds of
    /// a new rich text node, which is inserted into the diagram using an
    /// InsertNodesCmd.
    /// </para>
    /// <para>
    /// This tool also listens for double-click events. If the user double
    /// clicks on a rich text node, this tool opens a text editor allowing the
    /// user to edit the text.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.RectangleToolBase"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.RichTextNode"/>
    /// </remarks>
    public class RichTextTool
        : RectangleToolBase
    {
        #region Class members
        /// <summary>
        /// Default text value assigned to new rich text nodes.
        /// </summary>
        private string m_strDefaultText = string.Empty;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public RichTextTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("RichTextTool"))
        {
            this.ToolCursor = this.ActionCursor = Cursors.Cross;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets default text value assigned to new text nodes created by this tool.
        /// </summary>
        public string DefaultText
        {
            get { return m_strDefaultText; }
            set { m_strDefaultText = value; }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates the node from given rectangle base.
        /// </summary>
        /// <param name="rectBounding">The bounding rectangle.</param>
        /// <returns>The rich text node.</returns>
        protected override Node CreateNode(RectangleF rectBounding)
        {
            return new RichTextNode(m_strDefaultText, rectBounding);
        }

        /// <summary>
        /// Method used to perform additional actions after node is inserted into document
        /// ( start TextEditor after Text node is inserted into document for example )
        /// </summary>
        /// <param name="nodeInserted">node inserted into document</param>
        protected override void ActionComplete(Node nodeInserted)
        {
            this.Controller.TextEditor.BeginEdit(nodeInserted, false);
        }
        #endregion
    }
}