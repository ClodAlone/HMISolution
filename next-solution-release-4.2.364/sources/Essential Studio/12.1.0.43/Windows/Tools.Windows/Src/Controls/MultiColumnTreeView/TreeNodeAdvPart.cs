#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    /// <summary>
    /// This class holds information about the location and size of the node parts(eg plusminus,checkbox).
    /// </summary>
    public class TreeNodeAdvPart
    {
        #region Class members

        private TreeNodeAdv m_node;

        private bool m_visible = true;

        private Size m_size;

        internal int M_xIndentFromNodeLeft;
        #endregion

        #region Class properties

        protected TreeNodeAdv Node
        {
            get
            {
                return m_node;
            }
        }
        public Rectangle Bounds
        {
            get
            {
                return this.GetBounds();
            }
        }

        public Size Size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }

        public int Height
        {
            get
            {
                return m_size.Height;
            }
            set
            {
                m_size.Height = value;
            }
        }

        public int Width
        {
            get
            {
                return m_size.Width;
            }
            set
            {
                m_size.Width = value;
            }
        }
       
        public Point Location
        {
            get
            {
                return this.GetBounds().Location;
            }
        }

        public virtual bool Visible
        {
            get
            {
                return m_visible && m_node.Visible;
            }
            set
            {
                m_visible = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        internal TreeNodeAdvPart(TreeNodeAdv node)
        {
            m_node = node;
        }

        internal TreeNodeAdvPart(TreeNodeAdv node, Size size)
            : this(node)
        {
            m_size = size;
        }
        #endregion

        #region Class utility methods

        private Rectangle GetBounds()
        {
            if (!m_visible || m_node == null || m_node.Bounds == Rectangle.Empty)
            {
                return Rectangle.Empty;
            }

            int left = 0;

            if (GetIsMirrored())
            {
                left = m_node.NodeX - M_xIndentFromNodeLeft - this.Size.Width;
            }
            else
            {
                left = m_node.NodeX + M_xIndentFromNodeLeft;
            }

            int top = m_node.Bounds.Top + (m_node.Bounds.Height - this.Size.Height) / 2;
            return new Rectangle(new Point(left, top), this.Size);
        }

        protected bool GetIsMirrored()
        {
            return m_node.GetIsMirrored();
        }
        #endregion
    }

    internal class CheckBoxPart : TreeNodeAdvPart
    {
        #region Class Initialize/Finalize methods

        internal CheckBoxPart(TreeNodeAdv node) :
            base(node)
        {
        }

        internal CheckBoxPart(TreeNodeAdv node, Size size) :
            base(node, size)
        {
        }

        #endregion

        #region Class properties

        public override bool Visible
        {
            get
            {
                return base.Visible && this.Node.ShowCheckBox;
            }
            set
            {
                base.Visible = value;
            }
        }
        #endregion
    }

    internal class OptionButtonPart : TreeNodeAdvPart
    {
        #region Class Initialize/Finalize methods

        internal OptionButtonPart(TreeNodeAdv node) :
            base(node)
        {
        }

        internal OptionButtonPart(TreeNodeAdv node, Size size) :
            base(node, size)
        {
        }

        #endregion

        #region Class properties
       public override bool Visible
        {
            get
            {
                return base.Visible && this.Node.NodeStyle.ShowOptionButton;
            }
            set
            {
                base.Visible = value;
            }
        }
        #endregion
    }
}