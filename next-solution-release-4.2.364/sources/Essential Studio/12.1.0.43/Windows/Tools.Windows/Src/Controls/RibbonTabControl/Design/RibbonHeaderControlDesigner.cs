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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

using Syncfusion.Windows.Forms.Tools.Controls.RibbonTabControl.Interfaces;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <summary>
    /// Designer for RibbonHeaderControl.
    /// </summary>
    public class RibbonHeaderControlDesigner
        : ParentControlDesigner, IHeaderDesigner
    {
        #region Fields
        /// <summary>
        /// Design time RibbonHeaderControl instance.
        /// </summary>
        private RibbonHeaderControl m_control;

        /// <summary>
        /// Collection of verbs.
        /// </summary>
        private DesignerVerbCollection m_verbs = new DesignerVerbCollection();

        /// <summary>
        /// Glyph for header control.
        /// </summary>
        private HeaderControlGlyph m_headerGlyph;

        /// <summary>
        /// Behavior for header control.
        /// </summary>
        private HeaderControlBehavior m_headerBehavior;

        /// <summary>
        /// Action list.
        /// </summary>
        private DesignerActionListCollection m_actionList = new DesignerActionListCollection();
        #endregion

        #region Static Fields
        /// <summary>
        /// Pen for drawing adornment border.
        /// </summary>
        private static Pen _adornmentsBorderPen = new Pen(Color.Gray);
        #endregion

        #region Initialization

        static RibbonHeaderControlDesigner()
        {
            _adornmentsBorderPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            m_control = (RibbonHeaderControl)Component;

            m_control.TabChanged += new RibbonTabItemChangedEventHandler(M_control_TabChanged);

            m_headerBehavior = new HeaderControlBehavior(m_control, BehaviorService);
            m_headerGlyph = new HeaderControlGlyph(m_control, BehaviorService, m_headerBehavior);
        }

        protected override void Dispose(bool disposing)
        {
            m_control.TabChanged -= new RibbonTabItemChangedEventHandler(M_control_TabChanged);

            base.Dispose(disposing);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Gets verbs collection.
        /// </summary>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                return m_verbs;
            }
        }

        /// <summary>
        /// Gets action list.
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                return m_actionList;
            }
        }

        protected override void OnPaintAdornments(System.Windows.Forms.PaintEventArgs pe)
        {
            base.OnPaintAdornments(pe);

            pe.Graphics.DrawRectangle(_adornmentsBorderPen, 0, 0, m_control.Width - 1, m_control.Height - 1);
        }

        public override GlyphCollection GetGlyphs(GlyphSelectionType selectiontype)
        {
            GlyphCollection glyphs = base.GetGlyphs(selectiontype);

            glyphs.Add(m_headerGlyph);

            return glyphs;
        }
        #endregion

        #region Event Handlers

      public void M_control_TabChanged(object sender, RibbonTabItemChangedEventArgs args)
        {
            ISite site = m_control.Site;

            if (site != null)
            {
                IComponentChangeService changeService = site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                changeService.OnComponentChanged(args.NewItem, null, null, null);
            }
        }
        #endregion

        #region IHeaderDesigner Implementation
        /// <summary>
        /// Retrieves designer of part of header control that manages tabs.
        /// </summary>
        /// <returns>Designer of part of header control that manages tabs.</returns>
        public RibbonHeaderControlDesigner GetTabHeaderDesigner()
        {
            return this;
        }
        #endregion
    }
}
#endif