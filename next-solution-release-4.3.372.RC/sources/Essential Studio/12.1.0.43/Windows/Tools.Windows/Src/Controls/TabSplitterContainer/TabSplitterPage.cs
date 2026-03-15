#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    [ToolboxItem(false)]
    [Designer(typeof(Design.TabSplitterPageDesigner))]
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
    [DockingAttribute(DockingBehavior.Never)]
#endif
    public class TabSplitterPage : Panel
    {
        #region Constructor
        static TabSplitterPage()
        {
            m_defaultImage = new Bitmap(typeof(TabSplitterContainer).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.TabSplitterContainer.TabSplitterDefaultImage.bmp"));
            m_defaultImage.MakeTransparent(Color.Magenta);
        }

        public TabSplitterPage()
        {
            base.Dock = DockStyle.None;
            base.AutoScroll = true;
        }
        #endregion

        #region Properties
   
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabSplitterPagesCollection Owner
        {
            get
            {
                return m_owner;
            }
            set
            {
                if (m_owner != value)
                {
                    if (m_owner != null)
                    {
                        m_owner.Remove(this);
                    }
                    if (value != null)
                    {
                        value.Add(this);
                    }
                }
            }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }
    
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DockStyle Dock
        {
            get { return base.Dock; }
            set { }
        }

        /// <summary>
        /// Gets or sets the image to be shown in the page's tab.
        /// </summary>
        [Category("Appearance"), Description("Indicates the image to be shown in the page's tab.")]
        public Image Image
        {
            get
            {
                if (m_image == null)
                {
                    return m_defaultImage;
                }
                return m_image;
            }
            set
            {
                if (m_image != value)
                {
                    m_image = value;
                    OnImageChanged();
                }
            }
        }
        private bool hide = false;
        public bool Hide
        {
            get
            {
                return hide;
            }
            set
            {
                hide = value; 
            }
        }
      
        /// <summary>
        /// Gets or sets the transparency color for the tab image.
        /// </summary>
        [Category("Appearance"), Description("Indicates the transparency color for the tab image.")]
        public Color ImageTransparentColor
        {
            get
            {
                return m_imageTransparentColor;
            }
            set
            {
                if (m_imageTransparentColor != value)
                {
                    m_imageTransparentColor = value;

                    if (value != Color.Empty)
                    {
                        Bitmap bmp = this.Image as Bitmap;
                        if (bmp != null)
                        {
                            bmp.MakeTransparent(value);
                            OnImageChanged();
                        }
                    }
                }
            }
        }

        [Category("Behavior"), Description("Gets or sets tooltip to be shown on the pages tab."), Localizable(true)]
        public string ToolTip
        {
            get
            {
                if (m_sToolTip == null || m_sToolTip == String.Empty)
                {
                    return this.Text;
                }
                return m_sToolTip;
            }
            set
            {
                m_sToolTip = value;
            }
        }
        #endregion

        #region Methods

        internal void SetOwner(TabSplitterPagesCollection owner)
        {
            m_owner = owner;
        }

        #endregion

        #region Overrides

        protected override void Dispose(bool disposing)
        {
            this.Owner = null;
            base.Dispose(disposing);
        }
        #endregion

        #region Implementation

        private void OnImageChanged()
        {
            if (ImageChanged != null)
            {
                ImageChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        #region ShouldSerialize/Reset methods
       public bool ShouldSerializeImage()
        {
            return m_image != null;
        }
       public void ResetImage()
        {
            m_image = null;
        }

       public bool ShouldSerializeImageTransparentColor()
        {
            return m_imageTransparentColor != Color.Empty;
        }
       public void ResetImageTransparentColor()
        {
            m_imageTransparentColor = Color.Empty;
        }

       public bool ShouldSerializeToolTip()
        {
            return m_sToolTip != null && m_sToolTip != String.Empty;
        }
       public void ResetToolTipText()
        {
            m_sToolTip = null;
        }
        #endregion

        #region Events

        [Description("Occurs when the Image property value has been changed")]
        public event EventHandler ImageChanged;
        #endregion

        #region Fields

        private TabSplitterPagesCollection m_owner = null;

        private Image m_image;

        private Color m_imageTransparentColor = Color.Empty;

        private string m_sToolTip;

        private static Bitmap m_defaultImage;
        #endregion
    }
}
