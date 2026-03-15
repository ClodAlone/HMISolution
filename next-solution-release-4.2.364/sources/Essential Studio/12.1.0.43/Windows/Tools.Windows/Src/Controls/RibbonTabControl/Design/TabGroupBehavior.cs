#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <summary>
    /// Behavior for ribbon tab group.
    /// </summary>
    public class TabGroupBehavior
        : Behavior
    {
        #region Constants
        /// <summary>
        /// Data format for drag drop operation.
        /// </summary>
        internal const string DER_STR_ITEM_DRAGDROP_FORMAT = "ToolStripItemDragDrop";
        #endregion

        #region Fields
        /// <summary>
        /// Design time RibbonTabGroup instance.
        /// </summary>
        private RibbonTabGroup m_control;

        /// <summary>
        /// RibbonTabGroup designer behavior service.
        /// </summary>
        private BehaviorService m_behaviorService;

        /// <summary>
        /// Rectangle used for beginning of drag drop operation.
        /// </summary>
        private Rectangle m_dragBoxFromMouseDown = Rectangle.Empty;

        /// <summary>
        /// Item that should be dragged if mouse moves enough for breaking m_dragBoxFromMouseDown.
        /// Defined because mouse pointer can jump to another item while moving inside m_dragBoxFromMouseDown.
        /// </summary>
        private ToolStripItem m_dragDropItem;

        /// <summary>
        /// Selected item.
        /// </summary>
        private ToolStripItem m_selectedItem;

        /// <summary>
        /// Item under mouse.
        /// </summary>
        private ToolStripItem m_itemUnderMouse;

        /// <summary>
        /// Indicates whether group is situated under mouse pointer.
        /// </summary>
        private bool m_bUnderMouse;

        /// <summary>
        /// Indicates whether drag drop operation is being currently performed.
        /// </summary>
        private bool m_bUnderDragDrop;

        /// <summary>
        /// Indicates whether mouse pointer during drag drop operation is situated closer to the right edge of the underlying item.
        /// </summary>
        private bool m_bDragDropCloserToRight;
        #endregion

        #region Properties
        /// <summary>
        /// Gets selected item.
        /// </summary>
        public ToolStripItem SelectedItem
        {
            get
            {
                if (m_control.Items.Count == 0) m_selectedItem = null;

                return m_selectedItem;
            }
        }

        /// <summary>
        /// Gets item under mouse pointer.
        /// </summary>
        public ToolStripItem ItemUnderMouse
        {
            get
            {
                return m_itemUnderMouse;
            }
            private set
            {
                if (m_itemUnderMouse != value)
                {
                    m_itemUnderMouse = value;
                    m_control.InvalidateWithInnerControl();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether group is situated under mouse pointer.
        /// </summary>
        public bool UnderMouse
        {
            get
            {
                return m_bUnderMouse;
            }
            internal set
            {
                if (m_bUnderMouse != value)
                {
                    m_bUnderMouse = value;
                    m_control.InvalidateWithInnerControl();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether drag drop operation is being currently performed.
        /// </summary>
        public bool UnderDragDrop
        {
            get
            {
                return m_bUnderDragDrop;
            }
        }

        /// <summary>
        /// Gets a value indicating whether mouse pointer during drag drop operation is situated closer to the right edge of the underlying item.
        /// </summary>
        public bool DragDropCloserToRight
        {
            get
            {
                return m_bDragDropCloserToRight;
            }
            private set
            {
                if (m_bDragDropCloserToRight != value)
                {
                    m_bDragDropCloserToRight = value;
                    m_control.InvalidateWithInnerControl();
                }
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the TabGroupBehavior class.
        /// </summary>
        /// <param name="control">Design time instance of RibbonTabGroup.</param>
        /// <param name="behaviorService">Behavior Service</param>
        public TabGroupBehavior(RibbonTabGroup control, BehaviorService behaviorService)
        {
            if (control == null) throw new ArgumentNullException("control");
            if (behaviorService == null) throw new ArgumentNullException("behaviorService");

            m_control = control;
            m_behaviorService = behaviorService;

            m_control.ItemAdded += new ToolStripItemEventHandler(M_control_ItemAdded);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Processes mouse click. Highlights selected item.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        /// <param name="button">Mouse button</param>
        /// <param name="mouseLoc">Mouse Location</param>
        /// <returns>Returns  bool value</returns>
        public override bool OnMouseDown(Glyph g, System.Windows.Forms.MouseButtons button, Point mouseLoc)
        {
            Point localPoint = m_control.PointToClient(m_behaviorService.AdornerWindowPointToScreen(mouseLoc));
            ToolStripItem item = (ToolStripItem)m_control.GetItemAt(localPoint);

            ISelectionService selectionService = null;

            if (m_control.Site != null)
            {
                selectionService = m_control.Site.GetService(typeof(ISelectionService)) as ISelectionService;
            }

            IList componentsToSelect = new ArrayList();

            if (item != null)
            {
                if (item is RibbonTabItem) ((RibbonTabItem)item).Activate();

                componentsToSelect.Add(item);
                m_selectedItem = item;

                Size dragSize = SystemInformation.DragSize;
                Point rectPoint = new Point(localPoint.X - dragSize.Width / 2, localPoint.Y - dragSize.Height / 2);
                m_dragBoxFromMouseDown = new Rectangle(rectPoint, dragSize);
                m_dragDropItem = item;
            }
            else
            {
                if (!m_control.SingleItem) componentsToSelect.Add(m_control);

                m_dragBoxFromMouseDown = Rectangle.Empty;
                m_dragDropItem = null;
            }

            RaiseGroupClicked(item);

            if (selectionService != null)
            {
                selectionService.SetSelectedComponents(componentsToSelect);
            }

            return false;
        }

        /// <summary>
        /// Cancels drag drop operation.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="button">Mouse button</param>
        /// <returns>Returns bool value</returns>
        public override bool OnMouseUp(Glyph g, MouseButtons button)
        {
            m_dragBoxFromMouseDown = Rectangle.Empty;
            m_dragDropItem = null;
            return false;
        }

        /// <summary>
        /// Processes drag drop operation.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        /// <param name="button">Mouse button</param>
        /// <param name="mouseLoc">Mouse location</param>
        /// <returns>Returns  bool value</returns>
        public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
        {
            Point localPoint = m_control.PointToClient(m_behaviorService.AdornerWindowPointToScreen(mouseLoc));
            ToolStripItem curItem = (ToolStripItem)m_control.GetItemAt(localPoint);

            this.ItemUnderMouse = curItem;

            if ((button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (m_dragBoxFromMouseDown != Rectangle.Empty && !m_dragBoxFromMouseDown.Contains(localPoint.X, localPoint.Y))
                {
                    if (m_dragDropItem == null) throw new Exception("Drag drop item can't be null at this point of program flow.");

                    DataObject objectToDrag = new DataObject(DER_STR_ITEM_DRAGDROP_FORMAT, m_dragDropItem);
                    DragDropEffects ddeffect = m_control.DoDragDrop(objectToDrag, DragDropEffects.All);

                    // Delete single item group if item was succesfully dragged.
                    if (m_control.SingleItem && m_control.Items.Count == 0)
                    {
                        if (m_control.Site != null)
                        {
                            IDesignerHost desHost = (IDesignerHost)m_control.Site.GetService(typeof(IDesignerHost));

                            if (desHost != null) desHost.DestroyComponent(m_control);
                        }
                    }
                    else
                    {
                        // Recreate InitializeComponents.
                        if (ddeffect != DragDropEffects.None)
                        {
                            if (m_control.Site != null)
                            {
                                IComponentChangeService changeService = m_control.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

                                if (changeService != null)
                                {
                                    changeService.OnComponentChanged(m_control, null, null, null);
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Processes drag drop operation.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public override void OnDragOver(Glyph g, DragEventArgs e)
        {
            Point mousePoint = new Point(e.X, e.Y);
            ToolStripItem curItem = m_control.GetItemAt(m_control.PointToClient(mousePoint)) as ToolStripItem;
            ToolStripItem newItem = e.Data.GetData(DER_STR_ITEM_DRAGDROP_FORMAT) as ToolStripItem;

            if (curItem != newItem && newItem != null) m_bUnderDragDrop = true;
            else m_bUnderDragDrop = false;

            if (m_bUnderDragDrop && curItem != null)
            {
                this.DragDropCloserToRight = PointCloserToRightEdge(curItem, mousePoint);
            }

            this.ItemUnderMouse = curItem;

            if (newItem == null || curItem == newItem)
            {
                e.Effect = DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.All;
            }
        }

        /// <summary>
        /// Processes drag drop operation.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public override void OnDragDrop(Glyph g, DragEventArgs e)
        {
            Point mousePoint = new Point(e.X, e.Y);
            ToolStripItem curItem = (ToolStripItem)m_control.GetItemAt(m_control.PointToClient(mousePoint));

            if (e.Data.GetDataPresent(DER_STR_ITEM_DRAGDROP_FORMAT))
            {
                ToolStripItem newItem = (ToolStripItem)e.Data.GetData(DER_STR_ITEM_DRAGDROP_FORMAT);

                if (e.Effect == DragDropEffects.All)
                {
                    if (!m_control.SingleItem)
                    {
                        // Correct underlying item if mouse pointer is closer to the right edge of it.
                        if (curItem != null)
                        {
                            if (PointCloserToRightEdge(curItem, mousePoint))
                            {
                                int index = m_control.Items.IndexOf(curItem);

                                if (m_control.Items.Count == index + 1)
                                {
                                    // New item will be just added to the group.
                                    curItem = null;
                                }
                                else
                                {
                                    curItem = m_control.Items[index + 1];
                                }
                            }
                        }

                        if (curItem != null)
                        {
                            int index = m_control.Items.IndexOf(curItem);

                            int newItemIndex = m_control.Items.IndexOf(newItem);

                            // If new item is situated in the same group before the target item,
                            // decrement target index for the correct insertion before target item.
                            if (newItemIndex != -1 && newItemIndex < index) index--;

                            m_control.Items.Insert(index, newItem);
                        }
                        else
                        {
                            m_control.Items.Add(newItem);
                        }

                        m_selectedItem = newItem;

                        RaiseGroupClicked(newItem);
                    }
                    else
                    {
                        if (NewItemDroppedAtSingleItemGroup != null)
                        {
                            NewItemDroppedAtSingleItemGroup(m_control, new NewItemDroppedAtSingleItemGroupEventArgs(newItem, !this.DragDropCloserToRight));

                            RaiseGroupClicked(newItem);
                        }
                    }

                    m_bUnderDragDrop = false;
                }
            }
        }

        /// <summary>
        /// Unmarks group as group under mouse pointer.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public override void OnDragLeave(Glyph g, EventArgs e)
        {
            this.UnderMouse = false;
            m_bUnderDragDrop = false;
        }

        /// <summary>
        /// Marks group as group under mouse pointer.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        /// <param name="e"> DrawEventArgs that contains the event data.</param>
        public override void OnDragEnter(Glyph g, DragEventArgs e)
        {
            this.UnderMouse = true;
        }

        /// <summary>
        /// Marks group as group under mouse pointer.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        /// <returns>Returns  bool value</returns>
        public override bool OnMouseEnter(Glyph g)
        {
            this.UnderMouse = true;
            return false;
        }

        /// <summary>
        /// Unmarks group as group under mouse pointer.
        /// </summary>
        /// <param name="g">Graphics Object</param>
        /// <returns>Returns  bool value</returns>
        public override bool OnMouseLeave(Glyph g)
        {
            this.UnderMouse = false;
            return false;
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised when group is clicked.
        /// </summary>
        public event ToolStripItemEventHandler GroupClicked;

        /// <summary>
        /// Raised when new single item has to be added.
        /// </summary>
        public event NewItemDroppedAtSingleItemGroupEventHandler NewItemDroppedAtSingleItemGroup;
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets bool indicating whether given point is closer to the right edge of the item than to the left edge.
        /// </summary>
        /// <param name="item">Item under the point.</param>
        /// <param name="point">Point that should be analysed. In adorner coordinates.</param>
        /// <returns>True if point is located closer to the right edge; otherwise false.</returns>
        private bool PointCloserToRightEdge(ToolStripItem item, Point point)
        {
            Point adornerItemLocation = m_behaviorService.ScreenToAdornerWindow(m_control.PointToScreen(item.Bounds.Location));

            if (point.X - adornerItemLocation.X > item.Width / 2) return true;

            return false;
        }

        /// <summary>
        /// Raises GroupClicked event.
        /// </summary>
        /// <param name="item">Item for ToolStripItemEventArgs.</param>
        private void RaiseGroupClicked(ToolStripItem item)
        {
            if (GroupClicked != null)
            {
                GroupClicked(m_control, new ToolStripItemEventArgs(item));
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Makes new item selected.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
      public void M_control_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            this.m_selectedItem = e.Item;
            m_control.InvalidateWithInnerControl();
        }
        #endregion
    }
}
#endif
