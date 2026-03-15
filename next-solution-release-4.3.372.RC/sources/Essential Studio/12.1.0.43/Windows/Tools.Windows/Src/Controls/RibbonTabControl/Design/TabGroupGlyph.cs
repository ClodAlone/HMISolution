#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <summary>
    /// Glyph for RibbonTabGroup.
    /// </summary>
    public class TabGroupGlyph
        : Glyph
    {
        #region Constants
        /// <summary>
        /// Width of item drop place highlighting rectangle width.
        /// </summary>
        internal const int DEF_ITEM_DROP_HIGHLIGHT_WIDTH = 2;
        #endregion

        #region Fields
        /// <summary>
        /// Design time instance of RibbonTabGroup.
        /// </summary>
      private RibbonTabGroup m_control;

        /// <summary>
        /// RibbonTabGroup designer behavior service.
        /// </summary>
        private BehaviorService m_behaviorService;

        /// <summary>
        /// TabGroupBehavior instance.
        /// </summary>
       private TabGroupBehavior m_behavior;
        #endregion

        #region Static Fields
        /// <summary>
        /// Pen for drawing item highlighting.
        /// </summary>
        private static Pen _itemHighlightingPen = new Pen(Color.Gray);

        /// <summary>
        /// Brush for drawing drop place highlighting.
        /// </summary>
        private static Brush _dropHighlightingBrush = new System.Drawing.Drawing2D.HatchBrush(HatchStyle.Percent50, Color.Black, Color.Gray);
        #endregion

        #region Initialization
  
        static TabGroupGlyph()
        {
            _itemHighlightingPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        }

        /// <summary>
        /// Initializes a new instance of the TabGroupGlyph class.
        /// </summary>
        /// <param name="control">Design time instance of RibbonTabGroup.</param>
        /// <param name="behaviorService">RibbonTabGroup designer behavior service.</param>
        /// <param name="tabGroupBehavior">TabGroupBehavior instance.</param>
        public TabGroupGlyph(RibbonTabGroup control, BehaviorService behaviorService, TabGroupBehavior tabGroupBehavior)
            : base(tabGroupBehavior)
        {
            if (control == null) throw new ArgumentNullException("control");
            if (behaviorService == null) throw new ArgumentNullException("behaviorService");
            if (tabGroupBehavior == null) throw new ArgumentNullException("tabGroupBehavior");

            m_control = control;
            m_behaviorService = behaviorService;
            m_behavior = tabGroupBehavior;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Gets bounds of items area.
        /// </summary>
        public override Rectangle Bounds
        {
            get
            {
                Point edge = m_behaviorService.ControlToAdornerWindow(m_control);

                return new Rectangle(
                    edge.X, edge.Y + RibbonTabGroup.CaptionHeight, m_control.ClientSize.Width, m_control.ClientSize.Height);
            }
        }

        /// <summary>
        /// Processes cursor if it is inside of Bounds.
        /// </summary>
        /// <param name="p">Curson point</param>
        /// <returns>Returns Cusrsor</returns>
        public override Cursor GetHitTest(Point p)
        {
            if (Bounds.Contains(p))
            {
                return Cursors.Default;
            }

            return null;
        }

        /// <summary>
        /// Marks selected item.
        /// </summary>
        /// <param name="pe">PaintEventArgs that contains the event data.</param>
        public override void Paint(PaintEventArgs pe)
        {
            ToolStripItem item = m_behavior.SelectedItem;

            if (item != null && m_control.Active)
            {
                Point controlPoint = item.Bounds.Location;
                Point adornerPoint = m_behaviorService.ScreenToAdornerWindow(m_control.PointToScreen(controlPoint));
                pe.Graphics.DrawRectangle(Pens.Black, new Rectangle(adornerPoint, new Size(item.Bounds.Width - 1, item.Bounds.Height - 1)));
            }

            if (m_behavior.UnderMouse)
            {
                item = m_behavior.ItemUnderMouse;

                if (item != null)
                {
                    Point controlPoint = item.Bounds.Location;
                    Point adornerPoint = m_behaviorService.ScreenToAdornerWindow(m_control.PointToScreen(controlPoint));

                    if (m_behavior.UnderDragDrop)
                    {
                        int width = m_control.SingleItem ? HeaderControlGlyph.DEF_GROUP_DROP_HIGHLIGHT_WIDTH : DEF_ITEM_DROP_HIGHLIGHT_WIDTH;

                        if (m_behavior.DragDropCloserToRight)
                        {
                            int x = adornerPoint.X + item.Width - 1;
                            pe.Graphics.FillRectangle(_dropHighlightingBrush, x - width - 1, adornerPoint.Y, width, item.Height - 1);
                        }
                        else
                        {
                            pe.Graphics.FillRectangle(_dropHighlightingBrush, adornerPoint.X, adornerPoint.Y, width, item.Height - 1);
                        }
                    }
                    else
                    {
                        pe.Graphics.DrawRectangle(
                            _itemHighlightingPen, new Rectangle(adornerPoint, new Size(item.Bounds.Width - 1, item.Bounds.Height - 1)));
                    }
                }
                else
                {
                    if (!m_control.SingleItem)
                    {
                        Point controlPoint = new Point(m_control.Bounds.Location.X, m_control.Bounds.Location.Y + RibbonTabGroup.CaptionHeight);
                        Point adornerPoint = m_behaviorService.ScreenToAdornerWindow(m_control.Parent.PointToScreen(controlPoint));
                        pe.Graphics.DrawRectangle(_itemHighlightingPen, new Rectangle(adornerPoint, new Size(m_control.Bounds.Width - 1, m_control.Bounds.Height - 1 - RibbonTabGroup.CaptionHeight)));                           
                    }
                }
            }
        }
        #endregion
    }
}
#endif