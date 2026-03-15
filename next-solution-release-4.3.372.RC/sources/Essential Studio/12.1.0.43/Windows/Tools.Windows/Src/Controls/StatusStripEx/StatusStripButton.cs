#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary> StatusStrip control extended. 
    /// </summary>
    [System.Windows.Forms.Design.ToolStripItemDesignerAvailability(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.StatusStrip)]
    public class StatusStripButton : ToolStripButton, IStatusItem2
    {
        protected [REDACTED] m_htBitmaps;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);
        #region Initialization
    
        public StatusStripButton()
            : base()
        {
            this.Margin = new Padding(0, 4, 0, 2);
            CTRLSIZE = this.Size;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether button have separator after itself. 
        /// </summary>
        [System.ComponentModel.DefaultValue(false)]
        [Description("Gets or sets a value indicating whether the button is the last component in the group.")]
        public bool EndOfGroup
        {
            get
            {
                return m_bEndOfGroup;
            }
            set
            {
                if (m_bEndOfGroup != value)
                {
                    m_bEndOfGroup = value;

                    if (this.Parent != null)
                    {
                        this.Parent.PerformLayout();
                    }
                }
            }
        }
        #region For Touch

        bool isScaling = false;

        bool _touchMode = false;
        /// <summary>
        /// gets or sets the touchmode
        /// </summary>
		[DefaultValue(false)]
        public virtual bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        /// applies the scaling
        /// </summary>
        /// <param name="scaleFactor"></param>
        public void ApplyScaleToControl(float sf)
        {
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * sf), (int)(CTRLSIZE.Height * sf));
            isScaling = false;
            this.Invalidate();
        }
        /// <summary>
        /// Font changed event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        #endregion

        #endregion
        #region IStatusItem Members

        /// <summary>
        /// Gets or sets a value indicating whether button must be shown in StatusStrip.
        /// </summary>
        bool IStatusItem.Active
        {
            get
            {
                return this.Available;
            }
            set
            {
                this.Available = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether button have separator after itself. 
        /// </summary>
        bool IStatusItem.EndOfGroup
        {
            get { return this.EndOfGroup; }
            set { this.EndOfGroup = value; }
        }

        /// <summary>
        /// Gets the text that is to be displayed on the item. 
        /// </summary>
        string IStatusItem.Text
        {
            get
            {
                return this.Text; 
            }
        }

        #endregion

        #region IStatusItem2 Members
        /// <summary>
        /// Gets or sets the text that is to be displayed in context menu. 
        /// </summary>
        [DefaultValue((string)null)]
        public string StatusString
        {
            get
            {
                return m_statusString;
            }
            set
            {
                m_statusString = value;
            }
        }
        #endregion
        
        #region Fields
        /// <summary> Indicates if button have separator after itself. </summary>
        private bool m_bEndOfGroup = false;
 
        private string m_statusString;
        #endregion
    }

    /// <summary> StatusStrip control extended. 
    /// </summary>
    [System.Windows.Forms.Design.ToolStripItemDesignerAvailability(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.StatusStrip)]
    public class StatusStripLabel : ToolStripLabel, IStatusItem2
    {
        #region Initialization
  
        public StatusStripLabel()
            : base()
        {
            this.Margin = new Padding(0, 4, 0, 2);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether label have separator after itself. 
        /// </summary>
        [System.ComponentModel.DefaultValue(false)]
        [Description("Gets or sets a value indicating whether the label is the last component in the group.")]
        public bool EndOfGroup
        {
            get
            {
                return m_bEndOfGroup;
            }
            set
            {
                if (m_bEndOfGroup != value)
                {
                    m_bEndOfGroup = value;

                    if (this.Parent != null)
                    {
                        this.Parent.PerformLayout();
                    }
                }
            }
        }
        #endregion

        #region IStatusItem Members

        /// <summary> 
        /// Gets or sets a value indicating whether label must be shown in StatusStrip. 
        /// </summary>
        bool IStatusItem.Active
        {
            get
            {
                return this.Available;
            }
            set
            {
                this.Available = value;
            }
        }

        /// <summary> 
        /// Gets or sets a value indicating whether label have separator after itself. 
        /// </summary>
        bool IStatusItem.EndOfGroup
        {
            get 
            { 
                return this.EndOfGroup; 
            }
            set 
            { 
                this.EndOfGroup = value; 
            }
        }

        /// <summary>
        /// Gets the text that is to be displayed on the item. 
        /// </summary>
        string IStatusItem.Text
        {
            get 
            {
                return this.Text; 
            }
        }

        #endregion

        #region IStatusItem2 Members
        /// <summary>
        /// Gets or sets the text that is to be displayed in context menu. 
        /// </summary>
        [DefaultValue((string)null)]
        public string StatusString
        {
            get
            {
                return m_statusString;
            }
            set
            {
                m_statusString = value;
            }
        }
        #endregion

        #region Fields
        /// <summary> 
        /// Indicates if label have separator after itself. 
        /// </summary>
        private bool m_bEndOfGroup = false;
     
        private string m_statusString;
        #endregion
    }

    /// <summary> StatusStrip control extended.
    /// </summary>
    [System.Windows.Forms.Design.ToolStripItemDesignerAvailability(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.StatusStrip)]
    public class StatusStripProgressBar : ToolStripProgressBar, IStatusItem2
    {
        #region Initialization

        public StatusStripProgressBar()
            : base()
        {
            ((ProgressBar)Control).Style = ProgressBarStyle.Continuous;
            this.Margin = new Padding(0, 4, 0, 2);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether label have separator after itself. 
        /// </summary>
        [System.ComponentModel.DefaultValue(false)]
        [Description("Gets or sets a value indicating whether the label is the last component in the group.")]
        public bool EndOfGroup
        {
            get
            {
                return m_bEndOfGroup;
            }
            set
            {
                if (m_bEndOfGroup != value)
                {
                    m_bEndOfGroup = value;

                    if (this.Parent != null)
                    {
                        this.Parent.PerformLayout();
                    }
                }
            }
        }
        #endregion

        #region IStatusItem Members

        /// <summary> 
        /// Gets or sets a value indicating whether label must be shown in StatusStrip. 
        /// </summary>
        bool IStatusItem.Active
        {
            get
            {
                return this.Available;
            }
            set
            {
                this.Available = value;
            }
        }

        /// <summary> 
        /// Gets or sets a value indicating whether label have separator after itself. 
        /// </summary>
        bool IStatusItem.EndOfGroup
        {
            get
            {
                return this.EndOfGroup;
            }
            set
            {
                this.EndOfGroup = value;
            }
        }

        /// <summary>
        /// Gets the text that is to be displayed on the item. 
        /// </summary>
        string IStatusItem.Text
        {
            get 
            { 
                return this.Text; 
            }
        }
        #endregion

        #region IStatusItem2 Members
        /// <summary>
        /// Gets or sets the text that is to be displayed in context menu. 
        /// </summary>
        [DefaultValue((string)null)]
        public string StatusString
        {
            get
            {
                return m_statusString;
            }
            set
            {
                m_statusString = value;
            }
        }
        #endregion
      
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);         
            e.Graphics .FillRectangle(new SolidBrush(Color.Brown),new Rectangle (0,0,this.Width ,this.Height ));            
        }
        #region Fields
        /// <summary> 
        /// Indicates if label have separator after itself. 
        /// </summary>
        private bool m_bEndOfGroup = false;
  
        private string m_statusString;
        #endregion
    }

    /// <summary> StatusStrip control extended. 
    /// </summary>
    [System.Windows.Forms.Design.ToolStripItemDesignerAvailability(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.StatusStrip)]
    public class StatusStripDropDownButton : ToolStripDropDownButton, IStatusItem2
    {
        #region Initialization

        public StatusStripDropDownButton()
            : base()
        {
            this.Margin = new Padding(0, 4, 0, 2);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether label have separator after itself. 
        /// </summary>
        [System.ComponentModel.DefaultValue(false)]
        [Description("Gets or sets a value indicating whether the label is the last component in the group.")]
        public bool EndOfGroup
        {
            get
            {
                return m_bEndOfGroup;
            }
            set
            {
                if (m_bEndOfGroup != value)
                {
                    m_bEndOfGroup = value;

                    if (this.Parent != null)
                    {
                        this.Parent.PerformLayout();
                    }
                }
            }
        }
        #endregion

        #region IStatusItem Members

        /// <summary> 
        /// Gets or sets a value indicating whether label must be shown in StatusStrip. 
        /// </summary>
        bool IStatusItem.Active
        {
            get
            {
                return this.Available;
            }
            set
            {
                this.Available = value;
            }
        }

        /// <summary> 
        /// Gets or sets a value indicating whether label have separator after itself. 
        /// </summary>
        bool IStatusItem.EndOfGroup
        {
            get
            {
                return this.EndOfGroup;
            }
            set
            {
                this.EndOfGroup = value;
            }
        }

        /// <summary>
        /// Gets the text that is to be displayed on the item. 
        /// </summary>
        string IStatusItem.Text
        {
            get
            { 
                return this.Text; 
            }
        }
        #endregion

        #region IStatusItem2 Members
        /// <summary>
        /// Gets or sets the text that is to be displayed in context menu. 
        /// </summary>
        [DefaultValue((string)null)]
        public string StatusString
        {
            get
            {
                return m_statusString;
            }
            set
            {
                m_statusString = value;
            }
        }
        #endregion
       
        #region Fields
        /// <summary> 
        /// Indicates if label have separator after itself. 
        /// </summary>
        private bool m_bEndOfGroup = false;
   
        private string m_statusString;
        #endregion
    }

    /// <summary> StatusStrip control extended.
    /// </summary>
    [System.Windows.Forms.Design.ToolStripItemDesignerAvailability(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.StatusStrip)]
    public class StatusStripSplitButton : ToolStripSplitButton, IStatusItem2
    {
        #region Initialization
            public StatusStripSplitButton()
            : base()
        {
            this.Margin = new Padding(0, 4, 0, 2);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether label have separator after itself. 
        /// </summary>
        [System.ComponentModel.DefaultValue(false)]
        [Description("Gets or sets a value indicating whether the label is the last component in the group.")]
        public bool EndOfGroup
        {
            get
            {
                return m_bEndOfGroup;
            }
            set
            {
                if (m_bEndOfGroup != value)
                {
                    m_bEndOfGroup = value;

                    if (this.Parent != null)
                    {
                        this.Parent.PerformLayout();
                    }
                }
            }
        }
        #endregion

        #region IStatusItem Members

        /// <summary> 
        /// Gets or sets a value indicating whether label must be shown in StatusStrip. 
        /// </summary>
        bool IStatusItem.Active
        {
            get
            {
                return this.Available;
            }
            set
            {
                this.Available = value;
            }
        }

        /// <summary> 
        /// Gets or sets a value indicating whetherlabel have separator after itself. 
        /// </summary>
        bool IStatusItem.EndOfGroup
        {
            get
            {
                return this.EndOfGroup;
            }
            set
            {
                this.EndOfGroup = value;
            }
        }

        /// <summary>
        /// Gets the text that is to be displayed on the item. 
        /// </summary>
        string IStatusItem.Text
        {
            get 
            { 
                return this.Text; 
            }
        }
        #endregion

        #region IStatusItem2 Members
        /// <summary>
        /// Gets or sets the text that is to be displayed in context menu. 
        /// </summary>
        [DefaultValue((string)null)]
        public string StatusString
        {
            get
            {
                return m_statusString;
            }
            set
            {
                m_statusString = value;
            }
        }
        #endregion
      
        #region Fields
        /// <summary> 
        /// Indicates if label have separator after itself. 
        /// </summary>
        private bool m_bEndOfGroup = false;

        private string m_statusString;
        #endregion
    }

    /// <summary> StatusStrip control extended.
    /// </summary>
    [System.Windows.Forms.Design.ToolStripItemDesignerAvailability(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.StatusStrip)]
    public class StatusStripPanelItem : ToolStripPanelItem, IStatusItem2
    {
        #region Initialization
        
        public StatusStripPanelItem()
            : base()
        {
            this.Margin = new Padding(0, 4, 0, 2);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether label have separator after itself. 
        /// </summary>
        [System.ComponentModel.DefaultValue(false)]
        [Description("Gets or sets a value indicating whether the label is the last component in the group.")]
        public bool EndOfGroup
        {
            get
            {
                return m_bEndOfGroup;
            }
            set
            {
                if (m_bEndOfGroup != value)
                {
                    m_bEndOfGroup = value;

                    if (this.Parent != null)
                    {
                        this.Parent.PerformLayout();
                    }
                }
            }
        }
        #endregion

        #region IStatusItem Members

        /// <summary> 
        /// Gets or sets a value indicating whether label must be shown in StatusStrip. 
        /// </summary>
        bool IStatusItem.Active
        {
            get
            {
                return this.Available;
            }
            set
            {
                this.Available = value;
            }
        }

        /// <summary> 
        /// Gets or sets a value indicating whether label have separator after itself. 
        /// </summary>
        bool IStatusItem.EndOfGroup
        {
            get
            {
                return this.EndOfGroup;
            }
            set
            {
                this.EndOfGroup = value;
            }
        }

        /// <summary>
        /// Gets the text that is to be displayed on the item. 
        /// </summary>
        string IStatusItem.Text
        {
            get 
            { 
                return this.Text; 
            }
        }
        #endregion

        #region IStatusItem2 Members
        /// <summary>
        /// Gets or sets the text that is to be displayed in context menu. 
        /// </summary>
        [DefaultValue((string)null)]
        public string StatusString
        {
            get
            {
                return m_statusString;
            }
            set
            {
                m_statusString = value;
            }
        }
        #endregion

        #region Fields
        /// <summary> 
        /// Indicates if label have separator after itself. 
        /// </summary>
        private bool m_bEndOfGroup = false;
 
        private string m_statusString;
        #endregion
    }
}

#endif
