#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    internal class TreeViewAdvEditor : MultiColumnTreeView
    {
        #region Class members
        /// <summary>
        /// Controls bitmaps.
        /// Key - control.
        /// value - bitmap.
        /// </summary>
        private Hashtable m_htControlsBitmaps = new Hashtable();
        #endregion

        #region Class properties

        internal override bool NeedUpdateCustomControls
        {
            get
            {
                // must be false in designer editor.
                return false;
            }
            set
            {
                base.NeedUpdateCustomControls = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        internal TreeViewAdvEditor()
        {
            // must be false for designer editor.
            this.AutoControlsAdding = false;
            this.AutoAdjustMultiLineHeight = false;
        }
        #endregion

        #region Class utility methods

        /// <summary>
        /// Paint control in bitmap if need and save bitmap in collection.
        /// If Control has been painted return bitmap from collection.
        /// </summary>
        /// <param name="control">Control object</param>
        /// <param name="controlBounds">cCntrol Bounds</param>
        /// <returns>Return bitmap from collection</returns>
        private Bitmap GetControlBitmap(Control control, Rectangle controlBounds)
        {
            Bitmap bt = null;

            if (m_htControlsBitmaps.ContainsKey(control))
            {
                bt = (Bitmap)m_htControlsBitmaps[control];
            }
            else
            {
                bool lastVisible = control.Visible;
                Point lastLocation = control.Location;

                control.Size = controlBounds.Size;

                if (!lastVisible)
                {
                    control.Location = new Point(-1000, -1000);
                    control.Visible = true;
                }

                bt = ActiveXSnapshot.PrintWindow(control);
                m_htControlsBitmaps.Add(control, bt);

                if (!lastVisible)
                {
                    control.Visible = lastVisible;
                    control.Location = lastLocation;
                }
            }

            return bt;
        }
        #endregion

        protected override void DrawNode(Graphics g, Rectangle clip, TreeNodeAdv node, int y,  Point mousePos, bool mouseDown, bool background)                                   
        {
            base.DrawNode(g, clip, node, y, mousePos, mouseDown, background);

            if (node.CustomControl != null)
            {
                Rectangle controlBounds = node.GetCustomControlBounds();

                g.DrawImage(GetControlBitmap(node.CustomControl, controlBounds), controlBounds);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.Left)
            {
                // IN the designer tree, force a disabled node to be selected:
                TreeNodeAdv mouseDownNode = this.GetNodeAtPoint(e.X, e.Y);

                if (mouseDownNode != null && mouseDownNode != this.Root
                  && mouseDownNode.Enabled == false
                  && !this.SelectedNodes.Contains(mouseDownNode))
                {
                    if (this.SetSelectedNode(mouseDownNode, this.SelectedNodes, TreeViewAdvAction.ByMouse))
                    {
                        this.ActiveNode = mouseDownNode;

                        // Important that you return here, otherwise the behavior is as if
                        // the user clicked on a selected node.
                        // return;
                    }
                }
            }
            base.OnMouseDown(e);
        }    
    }
}