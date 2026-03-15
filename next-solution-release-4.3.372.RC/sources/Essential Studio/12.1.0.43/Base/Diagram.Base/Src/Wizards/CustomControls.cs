#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Wizard button
    /// </summary>
    [ToolboxItem(false)]
    public class ImageButton : Button
    {
        private Image m_normalImage = null;
        private Image m_hoverImage = null;

        protected override void OnMouseEnter(EventArgs e)
        {
            if (m_hoverImage != null)
                this.Image = m_hoverImage;
            this.Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (m_normalImage != null)
                this.Image = m_normalImage;
            this.Invalidate();
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Gets or sets the normal image.
        /// </summary>
        /// <value>The normal image.</value>
        public Image NormalImage
        {
            get
            {
                return m_normalImage;
            }
            set
            {
                if (m_normalImage != value)
                {
                    m_normalImage = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the hover image.
        /// </summary>
        /// <value>The hover image.</value>
        public Image HoverImage
        {
            get
            {
                return m_hoverImage;
            }
            set
            {
                if (m_hoverImage != value)
                {
                    m_hoverImage = value;
                    this.Invalidate();
                }
            }
        }
    }
}
