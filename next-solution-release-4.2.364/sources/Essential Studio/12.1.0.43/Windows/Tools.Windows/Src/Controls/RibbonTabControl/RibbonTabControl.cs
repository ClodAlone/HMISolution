#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Tools.Controls.RibbonTabControl.Interfaces;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Control that allows grouping child controls into tab pages.
    /// </summary>
    [Designer(typeof(RibbonTabControlDesigner))]
    [ToolboxItem(false)]
    [ToolboxBitmap(typeof(RibbonTabControl), "ToolboxIcons.RibbonTabControl.bmp")]
    public partial class RibbonTabControl
        : UserControl
    {
        #region Fields
        /// <summary>
        /// Header control.
        /// </summary>
        private IRibbonHeaderControl m_header;
        #endregion

        #region Properties
        /// <summary>
        /// Gets collection of tab groups.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(false)]
        public TabGroupsCollection Groups
        {
            get
            {
                return m_header.Groups;
            }
        }

        /// <summary>
        /// Gets or sets Size of the header.
        /// </summary>
        public Size HeaderSize
        {
            get
            {
                return ((Control)m_header).Size;
            }
            set
            {
                ((Control)m_header).Size = value;
            }
        }

        /// <summary>
        /// Gets or sets Docking style of the header.
        /// </summary>
        [DefaultValue(DockStyle.Top)]
        public DockStyle HeaderDock
        {
            get
            {
                return ((Control)m_header).Dock;
            }
            set
            {
                ((Control)m_header).Dock = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether with of control should be filled with RibbonTabItems.
        /// </summary>
        [DefaultValue(false)]
        public bool FillWidthWithItems
        {
            get
            {
                return m_header.FillWidthWithItems;
            }
            set
            {
                m_header.FillWidthWithItems = value;
            }
        }

        /// <summary>
        /// Gets or sets the first color of caption gradiend.
        /// </summary>
        public Color GroupsCaptionColor1
        {
            get
            {
                return this.Header.GroupsCaptionColor1;
            }
            set
            {
                this.Header.GroupsCaptionColor1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the second color of caption gradiend.
        /// </summary>
        public Color GroupsCaptionColor2
        {
            get
            {
                return this.Header.GroupsCaptionColor2;
            }
            set
            {
                this.Header.GroupsCaptionColor2 = value;
            }
        }

        /// <summary>
        /// Gets or sets color of groups caption font.
        /// </summary>
        public Color GroupsCaptionFontColor
        {
            get
            {
                return this.Header.GroupsCaptionFontColor;
            }
            set
            {
                this.Header.GroupsCaptionFontColor = value;
            }
        }
        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets header control.
        /// </summary>
        protected internal IRibbonHeaderControl Header
        {
            get
            {
                return m_header;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the RibbonTabControl class.
        /// </summary>
        public RibbonTabControl()
        {
            m_header = GetHeader();

            Control header = m_header as Control;

            if (header == null) throw new Exception("Header must be a control.");

            header.Dock = System.Windows.Forms.DockStyle.Top;
            header.Location = new System.Drawing.Point(0, 0);
            header.Name = "ribbonHeaderControl";

            // header.Size = new System.Drawing.Size( 490, 67 );
            header.TabIndex = 0;

            m_header.RibbonTabItemAdded += new RibbonTabItemEventHandler(OnRibbonTabItemAdded);

            this.Controls.Add(header);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds new ribbon tab group.
        /// </summary>
        /// <returns>Newly created tab group.</returns>
        public RibbonTabGroup AddGroup()
        {
            return m_header.AddGroup();
        }

        /// <summary>
        /// Adds ribbon tab group to the control.
        /// </summary>
        /// <param name="group">RibbonTabGroup instance to be added.</param>
        /// <returns>Added RibbonTabGroup instance.</returns>
        public RibbonTabGroup AddGroup(RibbonTabGroup group)
        {
            if (group == null) throw new ArgumentNullException("group");

            return m_header.AddGroup(group);
        }

        /// <summary>
        /// Adds new ribbon tab item to the new ribbon tab group.
        /// </summary>
        /// <returns>Newly created tab item.</returns>
        public RibbonTabItem AddTabItem()
        {
            return m_header.AddTabItem();
        }

        /// <summary>
        /// Adds toolstrip item to the newly created tab group.
        /// </summary>
        /// <param name="item">ToolStripItem instance to be added.</param>
        /// <returns>Added ToolStripItem instance.</returns>
        public ToolStripItem AddItem(ToolStripItem item)
        {
            if (item == null) throw new ArgumentNullException("item");

            return m_header.AddItem(item);
        }

        /// <summary>
        /// Adds new ribbon tab item to the specified tab group.
        /// </summary>
        /// <param name="group">RibbonTabGroup that has to host new item.</param>
        /// <returns>Newly created RibbonTabItem..</returns>
        public RibbonTabItem AddTabItem(RibbonTabGroup group)
        {
            if (group == null) throw new ArgumentNullException("group");

            return m_header.AddTabItem(group);
        }

        /// <summary>
        /// Adds toolstrip item to the specified tab group.
        /// </summary>
        /// <param name="item">ToolStripItem instance to be added.</param>
        /// <param name="group">RibbonTabGroup that has to host the item.</param>
        /// <returns>Added ToolStripItem instance.</returns>
        public ToolStripItem AddItem(ToolStripItem item, RibbonTabGroup group)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (group == null) throw new ArgumentNullException("group");

            return m_header.AddItem(item, group);
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Gets tab group located at the specified point in screen coordinates.
        /// </summary>
        /// <param name="p">Point in screen coordinates.</param>
        /// <returns>Group located at the specified point.</returns>
        protected internal RibbonTabGroup GetGroupAtScreenPoint(Point p)
        {
            if (p.IsEmpty) throw new ArgumentOutOfRangeException("p");

            Point clientPoint = ((Control)m_header).PointToClient(p);
            return ((Control)m_header).GetChildAtPoint(clientPoint) as RibbonTabGroup;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Hides all pages.
        /// </summary>
        private void HidePages()
        {
            foreach (Control control in this.Controls)
            {
                if (!(control is RibbonTabPage)) continue;

                RibbonTabPage page = (RibbonTabPage)control;
                page.Visible = false;
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Retrieves header for the control.
        /// </summary>
        /// <returns>IRibbonHeaderControl instance.</returns>
        protected virtual IRibbonHeaderControl GetHeader()
        {
            return new RibbonHeaderControl();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Creates new tab page (if needed) and initializes.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="args"> RibbonTabItemEventArgs that contains the event data.</param>
        private void OnRibbonTabItemAdded(object sender, RibbonTabItemEventArgs args)
        {
            RibbonTabPage page = args.Item.Page;

            // In runtime pages are added via initializer.
            if (!this.Controls.Contains(page))
            {
                this.Controls.Add(page);
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Shows initial page.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            HidePages();

            foreach (RibbonTabGroup group in this.Groups)
            {
                foreach (ToolStripItem item in group.Items)
                {
                    RibbonTabItem ribbonTabItem = item as RibbonTabItem;
                    if (ribbonTabItem != null && ribbonTabItem.Checked && ribbonTabItem.Page != null)
                    {
                        ribbonTabItem.Page.Visible = true;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Correctly docks new tab page.
        /// </summary>
        /// <param name="e"> ControlEventArgs that contains the event data.</param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            RibbonTabPage page = e.Control as RibbonTabPage;

            if (page != null)
            {
                // For the Dock.Fill to work properly.
                this.Controls.SetChildIndex(page, 0);

                page.Dock = DockStyle.Fill;
            }

            if (e.Control is RibbonHeaderControl)
            {
                this.Controls.SetChildIndex(e.Control, this.Controls.Count - 1);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised when tab is about to be changed.
        /// </summary>
        public event RibbonTabItemChangingEventHandler TabChanging
        {
            add
            {
                m_header.TabChanging += value;
            }
            remove
            {
                m_header.TabChanging -= value;
            }
        }

        /// <summary>
        /// Raised when tab is changed.
        /// </summary>
        public event RibbonTabItemChangedEventHandler TabChanged
        {
            add
            {
                m_header.TabChanged += value;
            }
            remove
            {
                m_header.TabChanged -= value;
            }
        }
        #endregion

        #region Serialization Required Methods
        public bool ShouldSerializeGroupsCaptionColor1()
        {
            return this.GroupsCaptionColor1 != RibbonTabGroup.DEF_INITIAL_CAPTION_COLOR1;
        }
        public void ResetGroupsCaptionColor1()
        {
            this.GroupsCaptionColor1 = RibbonTabGroup.DEF_INITIAL_CAPTION_COLOR1;
        }
        public bool ShouldSerializeGroupsCaptionColor2()
        {
            return this.GroupsCaptionColor2 != RibbonTabGroup.DEF_INITIAL_CAPTION_COLOR2;
        }
        public void ResetGroupsCaptionColor2()
        {
            this.GroupsCaptionColor2 = RibbonTabGroup.DEF_INITIAL_CAPTION_COLOR2;
        }
        public bool ShouldSerializeGroupsCaptionFontColor()
        {
            return this.GroupsCaptionFontColor != RibbonTabGroup.DEF_INITIAL_CAPTION_FONT_COLOR;
        }
        public void ResetGroupsCaptionFontColor()
        {
            this.GroupsCaptionFontColor = RibbonTabGroup.DEF_INITIAL_CAPTION_FONT_COLOR;
        }
        #endregion
    }
}
#endif