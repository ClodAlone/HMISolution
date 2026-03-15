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
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <summary>
    /// Behavior for ribbon header control.
    /// </summary>
    public class HeaderControlBehavior
        : Behavior
    {
        #region Constants
        /// <summary>
        /// Data format for drag drop operation.
        /// </summary>
        internal const string DEF_STR_GROUP_DRAGDROP_FORMAT = "RibbonTabGroupDragDrop";
        #endregion

        #region Fields
        /// <summary>
        /// Design time RibbonHeaderControl instance.
        /// </summary>
        private RibbonHeaderControl m_control;

        /// <summary>
        /// RibbonHeaderControl designer behavior service.
        /// </summary>
        private BehaviorService m_behaviorService;
        
        /// <summary>
        /// Rectangle used for beginning of drag drop operation.
        /// </summary>
        private Rectangle m_dragBoxFromMouseDown = Rectangle.Empty;

        /// <summary>
        /// Group that should be dragged if mouse moves enough for breaking m_dragBoxFromMouseDown.
        /// Defined because mouse pointer can jump to another item while moving inside m_dragBoxFromMouseDown.
        /// </summary>
        private RibbonTabGroup m_dragDropGroup;

        /// <summary>
        /// Group under drag drop ooperation.
        /// </summary>
        private RibbonTabGroup m_groupUnderDragDrop;

        /// <summary>
        /// Indicates whether mouse pointer during drag drop operation is situated closer to the right edge of the underlying group.
        /// </summary>
        private bool m_bDragDropCloserToRight;
        #endregion

        #region Properties
        /// <summary>
        /// Gets group under drag drop operation.
        /// </summary>
        public RibbonTabGroup GroupUnderDragDrop
        {
            get
            {
                return m_groupUnderDragDrop;
            }
            private set
            {
                if (m_groupUnderDragDrop != value)
                {
                    m_groupUnderDragDrop = value;
                    m_control.Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether mouse pointer during drag drop operation is situated closer to the right edge of the underlying group.
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
                    m_control.Invalidate(true);
                }
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the HeaderControlBehavior class.
        /// </summary>
        /// <param name="control">Design time instance of RibbonTabGroup.</param>
        /// <param name="behaviorService">Behavior Service</param>
        public HeaderControlBehavior(RibbonHeaderControl control, BehaviorService behaviorService)
        {
            if (control == null) throw new ArgumentNullException("control");
            if (behaviorService == null) throw new ArgumentNullException("behaviorService");

            m_control = control;
            m_behaviorService = behaviorService;
        }
        #endregion

        #region Overrides
     
        public override bool OnMouseDown(Glyph g, System.Windows.Forms.MouseButtons button, Point mouseLoc)
        {
            Point localPoint = m_control.PointToClient(m_behaviorService.AdornerWindowPointToScreen(mouseLoc));
            RibbonTabGroup group = m_control.GetChildAtPoint(localPoint) as RibbonTabGroup;

            ISelectionService selectionService = GetSelectionService();

            IList componentsToSelect = new ArrayList();

            if (group == null)
            {
                m_dragBoxFromMouseDown = Rectangle.Empty;
                m_dragDropGroup = null;

                componentsToSelect.Add(m_control);
            }
            else
            {
                Size dragSize = SystemInformation.DragSize;
                Point rectPoint = new Point(localPoint.X - dragSize.Width / 2, localPoint.Y - dragSize.Height / 2);
                m_dragBoxFromMouseDown = new Rectangle(rectPoint, dragSize);
                m_dragDropGroup = group;

                componentsToSelect.Add(group);
            }

            if (selectionService != null)
            {
                selectionService.SetSelectedComponents(componentsToSelect);
            }

            return false;
        }

        public override bool OnMouseUp(Glyph g, MouseButtons button)
        {
            m_dragBoxFromMouseDown = Rectangle.Empty;
            m_dragDropGroup = null;
            return false;
        }
        public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
        {
            Point localPoint = m_control.PointToClient(m_behaviorService.AdornerWindowPointToScreen(mouseLoc));
            RibbonTabGroup group = m_control.GetChildAtPoint(localPoint) as RibbonTabGroup;

            if ((button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (m_dragBoxFromMouseDown != Rectangle.Empty && !m_dragBoxFromMouseDown.Contains(localPoint.X, localPoint.Y))
                {
                    if (m_dragDropGroup == null) throw new Exception("Drag drop group can't be null at this point of program flow.");

                    DataObject objectToDrag = new DataObject(DEF_STR_GROUP_DRAGDROP_FORMAT, m_dragDropGroup);
                    DragDropEffects ddeffect = m_control.DoDragDrop(objectToDrag, DragDropEffects.All);

                    // Select dragged group.
                    ISelectionService selectionService = GetSelectionService();

                    if (selectionService != null)
                    {
                        IList componentsToSelect = new ArrayList();

                        // Unselect everything - set empty list.
                        selectionService.SetSelectedComponents(componentsToSelect);
                        componentsToSelect.Add(m_dragDropGroup);
                        selectionService.SetSelectedComponents(componentsToSelect);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Processes drag drop operation.
        /// </summary>
        /// <param name="g">Graphic object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public override void OnDragOver(Glyph g, DragEventArgs e)
        {
            Point mousePoint = new Point(e.X, e.Y);
            RibbonTabGroup curGroup = m_control.GetChildAtPoint(m_control.PointToClient(mousePoint)) as RibbonTabGroup;
            RibbonTabGroup newGroup = GetGroupFromDataObject(e.Data);

            if (curGroup != null)
            {
                this.DragDropCloserToRight = PointCloserToRightEdge(curGroup, mousePoint);
            }

            if (newGroup == null || curGroup == newGroup)
            {
                this.GroupUnderDragDrop = null;

                e.Effect = DragDropEffects.None;
            }
            else
            {
                this.GroupUnderDragDrop = curGroup;

                e.Effect = DragDropEffects.All;
            }
        }

        /// <summary>
        /// Processes drag drop operation.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public override void OnDragDrop(Glyph g, DragEventArgs e)
        {
            Point mousePoint = new Point(e.X, e.Y);
            RibbonTabGroup curGroup = m_control.GetChildAtPoint(m_control.PointToClient(mousePoint)) as RibbonTabGroup;

            RibbonTabGroup newGroup = GetGroupFromDataObject(e.Data);

            if (e.Effect == DragDropEffects.All)
            {
                // Correct underlying item if mouse pointer is closer to the right edge of it.
                if (curGroup != null && PointCloserToRightEdge(curGroup, mousePoint))
                {
                    int index = m_control.Groups.IndexOf(curGroup);

                    if (m_control.Groups.Count == index + 1)
                    {
                        // If it's the last group, new item will be just added to the groups collection.
                        curGroup = null;
                    }
                    else
                    {
                        curGroup = (RibbonTabGroup)m_control.Groups[index + 1];
                    }
                }

                if (curGroup != null)
                {
                    int index = m_control.Groups.IndexOf(curGroup);

                    // If new group is situated before the target item, decrement target index for the correct insertion before target item.
                    int newGroupIndex = m_control.Groups.IndexOf(newGroup);

                    if (newGroupIndex != -1 && newGroupIndex < index) index--;

                    RibbonHeaderControl header = newGroup.Parent as RibbonHeaderControl;
                    if (header != null) header.Groups.Remove(newGroup);

                    m_control.Groups.Insert(index, newGroup);
                }
                else
                {
                    m_control.Groups.Add(newGroup);
                }

                if (m_control.Site != null)
                {
                    IComponentChangeService changeService = m_control.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

                    if (changeService != null)
                    {
                        changeService.OnComponentChanged(m_control, null, null, null);
                    }
                }

                m_control.PerformLayout();

                this.GroupUnderDragDrop = null;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets bool indicating whether given point is closer to the right edge of the item than to the left edge.
        /// </summary>
        /// <param name="group">Group under the point.</param>
        /// <param name="point">Point that should be analysed. In adorner coordinates.</param>
        /// <returns>True if point is located closer to the right edge; otherwise false.</returns>
        private bool PointCloserToRightEdge(RibbonTabGroup group, Point point)
        {
            Point adornerItemLocation = m_behaviorService.ScreenToAdornerWindow(m_control.PointToScreen(group.Bounds.Location));

            if (point.X - adornerItemLocation.X > group.Width / 2) return true;

            return false;
        }

        /// <summary>
        /// Returns selection service.
        /// </summary>
        /// <returns>Selection service</returns>
        private ISelectionService GetSelectionService()
        {
            if (m_control.Site != null) return m_control.Site.GetService(typeof(ISelectionService)) as ISelectionService;

            return null;
        }

        /// <summary>
        /// Extracts group from IDataObject instance.
        /// </summary>
        /// <param name="data">IDataObject instance.</param>
        /// <returns>Extracted group.</returns>
        private RibbonTabGroup GetGroupFromDataObject(IDataObject data)
        {
            RibbonTabGroup result = data.GetData(DEF_STR_GROUP_DRAGDROP_FORMAT) as RibbonTabGroup;

            // If single item is dragged.
            if (result == null)
            {
                ToolStripItem item = data.GetData(TabGroupBehavior.DER_STR_ITEM_DRAGDROP_FORMAT) as ToolStripItem;

                if (item != null)
                {
                    RibbonTabGroup group = item.Owner as RibbonTabGroup;

                    if (group != null && group.SingleItem) result = group;
                }
            }

            return result;
        }
        #endregion
    }
}
#endif
