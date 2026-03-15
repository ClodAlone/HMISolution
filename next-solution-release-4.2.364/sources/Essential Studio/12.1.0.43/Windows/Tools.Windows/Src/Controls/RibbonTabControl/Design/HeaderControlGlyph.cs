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
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <summary>
    /// Glyph for RibbonHeaderControl.
    /// </summary>
    public class HeaderControlGlyph
        : Glyph
    {
        #region Constants
        /// <summary>
        /// Width of group drop place highlighting rectangle width.
        /// </summary>
        internal const int DEF_GROUP_DROP_HIGHLIGHT_WIDTH = 4;
        #endregion

        #region Fields
        /// <summary>
        /// Design time instance of RibbonHeaderControl.
        /// </summary>
       private RibbonHeaderControl m_control;

        /// <summary>
        /// RibbonHeaderControl designer behavior service.
        /// </summary>
       private BehaviorService m_behaviorService;

        /// <summary>
        /// HeaderControlBehavior instance.
        /// </summary>
        private HeaderControlBehavior m_behavior;
        #endregion

        #region Static Fields
        /// <summary>
        /// Brush for drawing drop place highlighting.
        /// </summary>
        private static Brush _dropHighlightingBrush = new System.Drawing.Drawing2D.HatchBrush(HatchStyle.Percent50, Color.Black, Color.Gray);
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the HeaderControlGlyph class.
        /// </summary>
        /// <param name="control">Design time instance of RibbonTabGroup.</param>
        /// <param name="behaviorService">RibbonTabGroup designer behavior service.</param>
        /// <param name="headerControlBehavior">TabGroupBehavior instance.</param>
        public HeaderControlGlyph(RibbonHeaderControl control, BehaviorService behaviorService, HeaderControlBehavior headerControlBehavior)
            : base(headerControlBehavior)
        {
            if (control == null) throw new ArgumentNullException("control");
            if (behaviorService == null) throw new ArgumentNullException("behaviorService");
            if (headerControlBehavior == null) throw new ArgumentNullException("tabGroupBehavior");

            m_control = control;
            m_behaviorService = behaviorService;
            m_behavior = headerControlBehavior;
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

                return new Rectangle(edge.X, edge.Y, m_control.ClientSize.Width - 1, m_control.ClientSize.Height - 1);
            }
        }
        
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
        /// <param name="pe"> EventArgs that contains the event data.</param>
        public override void Paint(PaintEventArgs pe)
        {
            RibbonTabGroup group = m_behavior.GroupUnderDragDrop;

            if (group != null)
            {
                Point controlPoint = group.Bounds.Location;
                Point adornerPoint = m_behaviorService.ScreenToAdornerWindow(m_control.PointToScreen(controlPoint));

                if (m_behavior.DragDropCloserToRight)
                {
                    int x = adornerPoint.X + group.Width - 1;
                    pe.Graphics.FillRectangle(
                        _dropHighlightingBrush, x - DEF_GROUP_DROP_HIGHLIGHT_WIDTH + 1, adornerPoint.Y, DEF_GROUP_DROP_HIGHLIGHT_WIDTH, group.Height - 1);
                }
                else
                {
                    pe.Graphics.FillRectangle(_dropHighlightingBrush, adornerPoint.X, adornerPoint.Y, DEF_GROUP_DROP_HIGHLIGHT_WIDTH, group.Height - 1);
                }
            }
        }
        #endregion
    }
}
#endif