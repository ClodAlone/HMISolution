#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.Controls.RibbonTabControl.Interfaces;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Control representing header part of RibbonTabControl.
    /// </summary>
    [Designer(typeof(RibbonHeaderControlDesigner))]
    [ToolboxItem(false)]
    public partial class RibbonHeaderControl
        : UserControl, IRibbonHeaderControl
    {
        #region Constants
        /// <summary>
        /// Width of empty tab group.
        /// </summary>
        private const int DEF_EMPTY_GROUP_WIDTH = 80;

        /// <summary>
        /// Additional width to add while laying out ToolStripControlHost items.
        /// </summary>
        private const int DEF_TOOLSTRIPCONTROLHOST_ADD_WIDTH = 2;
        #endregion

        #region Fields
  
        private Office12ToolStripRenderer m_renderer;

        /// <summary>
        /// Collection of tab groups.
        /// </summary>
        private TabGroupsCollection m_groups = new TabGroupsCollection();

        private RibbonTabItem m_activetTab = null;

        private bool m_bTabChanging = false;

        /// <summary>
        /// Indicates whether with of control should be filled with RibbonTabItems.
        /// </summary>
        private bool m_bFillWidthWithItems = false;

        /// <summary>
        /// The first color of caption gradiend.
        /// </summary>
        private Color m_clrCaption1 = RibbonTabGroup.DEF_INITIAL_CAPTION_COLOR1;

        /// <summary>
        /// The second color of caption gradiend.
        /// </summary>
        private Color m_clrCaption2 = RibbonTabGroup.DEF_INITIAL_CAPTION_COLOR2;

        /// <summary>
        /// Color of groups caption font.
        /// </summary>
        private Color m_clrGroupsCaptionFont = RibbonTabGroup.DEF_INITIAL_CAPTION_FONT_COLOR;
        #endregion

        #region Properties
        /// <summary>
        /// Gets collection of tab groups.
        /// </summary>
        public TabGroupsCollection Groups
        {
            get
            {
                return m_groups;
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
                return m_bFillWidthWithItems;
            }
            set
            {
                if (m_bFillWidthWithItems != value)
                {
                    m_bFillWidthWithItems = value;

                    PerformLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets the first color of caption gradiend.
        /// </summary>
        public Color GroupsCaptionColor1
        {
            get
            {
                return m_clrCaption1;
            }
            set
            {
                m_clrCaption1 = value;

                foreach (RibbonTabGroup group in this.Groups)
                {
                    group.CaptionColor1 = m_clrCaption1;
                }
            }
        }

        /// <summary>
        /// Gets or sets the second color of caption gradiend.
        /// </summary>
        public Color GroupsCaptionColor2
        {
            get
            {
                return m_clrCaption2;
            }
            set
            {
                m_clrCaption2 = value;

                foreach (RibbonTabGroup group in this.Groups)
                {
                    group.CaptionColor2 = m_clrCaption2;
                }
            }
        }

        /// <summary>
        /// Gets or sets color of groups caption font.
        /// </summary>
        public Color GroupsCaptionFontColor
        {
            get
            {
                return m_clrGroupsCaptionFont;
            }
            set
            {
                m_clrGroupsCaptionFont = value;

                foreach (RibbonTabGroup group in this.Groups)
                {
                    group.CaptionFontColor = m_clrGroupsCaptionFont;
                }
            }
        }
        #endregion

        #region Initialization & Finalization
        /// <summary>
        /// Initializes a new instance of the RibbonHeaderControl class.
        /// </summary>
        public RibbonHeaderControl()
        {
            m_renderer = new Office12ToolStripRenderer(Office12ToolStripRenderer.ERENDERTYPE.TabBar);

            m_groups.GroupAdded += new RibbonTabGroupEventHandler(OnGroupAdded);
            m_groups.GroupRemoved += new RibbonTabGroupEventHandler(OnGroupRemoved);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds new ribbon tab group.
        /// </summary>
        /// <returns>Newly created tab group.</returns>
        public RibbonTabGroup AddGroup()
        {
            return AddGroup(new RibbonTabGroup());
        }

        /// <summary>
        /// Adds ribbon tab group to the control.
        /// </summary>
        /// <param name="group">RibbonTabGroup instance to be added.</param>
        /// <returns>Added RibbonTabGroup instance.</returns>
        public RibbonTabGroup AddGroup(RibbonTabGroup group)
        {
            if (group != null)
            {
                m_groups.Add(group);
            }
            return group;
        }

        /// <summary>
        /// Adds new ribbon tab item to the new ribbon tab group.
        /// </summary>
        /// <returns>Newly created tab item.</returns>
        public RibbonTabItem AddTabItem()
        {
            return (RibbonTabItem)AddItem(new RibbonTabItem());
        }

        /// <summary>
        /// Adds toolstrip item to the newly created tab group.
        /// </summary>
        /// <param name="item">ToolStripItem instance to be added.</param>
        /// <returns>Added ToolStripItem instance.</returns>
        public ToolStripItem AddItem(ToolStripItem item)
        {
            if (item != null)
            {
                return AddItem(item, AddGroup());
            }
            return null;
        }

        /// <summary>
        /// Adds new ribbon tab item to the specified tab group.
        /// </summary>
        /// <param name="group">RibbonTabGroup that has to host new item.</param>
        /// <returns>Newly created RibbonTabItem.</returns>
        public RibbonTabItem AddTabItem(RibbonTabGroup group)
        {
            if (group == null) throw new ArgumentNullException("group");

            return (RibbonTabItem)AddItem(new RibbonTabItem(), group);
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

            group.Items.Add(item);

            return item;
        }

        /// <summary>
        /// Returns control that has to be shown behind transparent childs.
        /// </summary>
        /// <returns>Required control.</returns>
        public Control GetParentForChildsTransparentRendering()
        {
            IRibbonHeaderControl parentHeader = this.Parent as IRibbonHeaderControl;

            if (parentHeader != null) return parentHeader.GetParentForChildsTransparentRendering();

            return this;
        }

        /// <summary>
        /// Returns header for tab control.
        /// </summary>
        /// <returns>Required tab header.</returns>
        public Control GetTabsHeader()
        {
            return this;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Lays out tab groups.
        /// </summary>
        /// <param name="e">LayoutEventArgs that contains the event data.</param>
        protected override void OnLayout(LayoutEventArgs e)
        {
            for (int i = 0, left = 0, count = m_groups.Count; i < count; i++)
            {
                RibbonTabGroup group = m_groups[i] as RibbonTabGroup;
                if (group != null)
                {
                    int nWidth = group.GetPreferredSize(Size.Empty).Width;

                    group.Height = this.ClientSize.Height;
                    group.Width = nWidth;
                    group.Left = left;

                    left += nWidth;
                }
            }

            Invalidate(true);
        }

        protected virtual bool OnTabChanging(RibbonTabItem oldTab, RibbonTabItem newTab)
        {
            RibbonTabItemChangingEventArgs args = new RibbonTabItemChangingEventArgs(oldTab, newTab);

            if (TabChanging != null)
            {
                TabChanging(this, args);
            }
            return !args.Cancel;
        }

        protected virtual void OnTabChanged(RibbonTabItem oldTab, RibbonTabItem newTab)
        {
            if (oldTab != null && oldTab.Page != null)
            {
                oldTab.Page.Visible = false;
            }

            if (newTab != null && newTab.Page != null)
            {
                newTab.Page.Visible = true;
            }

            if (TabChanged != null)
            {
                TabChanged(this, new RibbonTabItemChangedEventArgs(oldTab, newTab));
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes new ribbon tab item.
        /// </summary>
        /// <param name="ribbonTabItem">Item to initialize.</param>
        private void InitItem(RibbonTabItem ribbonTabItem)
        {
            if (ribbonTabItem != null)
            {
                ribbonTabItem.CheckStateChanged += new EventHandler(OnTabItemCheckStateChanged);

                if (ribbonTabItem.CheckState == CheckState.Checked)
                {
                    OnTabItemCheckStateChanged(ribbonTabItem, EventArgs.Empty);
                }
                else
                {
                    if (m_activetTab == null)
                    {
                        ribbonTabItem.CheckState = CheckState.Checked;
                    }
                }

                if (RibbonTabItemAdded != null)
                {
                    RibbonTabItemAdded(this, new RibbonTabItemEventArgs(ribbonTabItem));
                }
            }
        }
        #endregion

        #region Event Handlers
      
        public void OnTabItemCheckStateChanged(object sender, EventArgs e)
        {
            RibbonTabItem newTab = sender as RibbonTabItem;

            if (newTab != null)
            {
                if (!m_bTabChanging)
                {
                    m_bTabChanging = true;

                    if (newTab.CheckState == CheckState.Checked)
                    {
                        RibbonTabItem oldTab = m_activetTab;

                        if (OnTabChanging(oldTab, newTab))
                        {
                            if (oldTab != null)
                            {
                                oldTab.Checked = false;
                            }

                            m_activetTab = newTab;

                            OnTabChanged(oldTab, newTab);
                        }
                        else newTab.CheckState = CheckState.Unchecked;
                    }
                    else newTab.CheckState = CheckState.Checked;

                    m_bTabChanging = false;
                }
            }
        }

        /// <summary>
        /// Adds new tab group to the controls.
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="args">RibbonTabGroupEventArgs that contains the event data.</param>
       public void OnGroupAdded(object sender, RibbonTabGroupEventArgs args)
        {
            RibbonTabGroup group = args.Group;
            if (group != null)
            {
                group.Renderer = m_renderer;

                group.ItemAdded += new ToolStripItemEventHandler(OnItemAdded);
                group.ItemRemoved += new ToolStripItemEventHandler(OnItemRemoved);

                // If some items were added before.
                foreach (ToolStripItem item in group.Items)
                {
                    InitItem(item as RibbonTabItem);
                }
                this.Controls.Add(group);
            }
        }

        /// <summary>
        /// Lays out groups.
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="args">RibbonTabGroupEventArgs that contains the event data.</param>
       public void OnGroupRemoved(object sender, RibbonTabGroupEventArgs args)
        {
            // TODO : Remove group from Controls
        }

        /// <summary>
        /// Initializes new tab item.
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">ToolstripItemEventArgs that contains the event data.</param>
       public void OnItemAdded(object sender, ToolStripItemEventArgs e)
        {
            RibbonTabItem item = e.Item as RibbonTabItem;

            if (item != null)
            {
                InitItem(item);
            }
        }

        /// <summary>
        /// Performs layout.
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">ToolStripItemEventArgs that contains the event data.</param>
      public void OnItemRemoved(object sender, ToolStripItemEventArgs e)
        {
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised when newly created tab item is added to the control.
        /// </summary>
        public event RibbonTabItemEventHandler RibbonTabItemAdded;

        /// <summary>
        /// Raised when tab is about to be changed.
        /// </summary>
        public event RibbonTabItemChangingEventHandler TabChanging;

        /// <summary>
        /// Raised when tab is changed.
        /// </summary>
        public event RibbonTabItemChangedEventHandler TabChanged;
        #endregion
    }
}