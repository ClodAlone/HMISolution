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
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    public class ToolTipDesigner : DocumentDesigner
    {
        #region *** ToolTipItemBehavior
       public class ToolTipItemBehavior : Behavior
        {
            #region Overrides
            public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
            {
                bool bResult = false;

                ToolTipItemGlyph glyph = g as ToolTipItemGlyph;
                if (glyph != null)
                {
                    glyph.Select();
                    bResult = true;
                }

                return bResult;
            }
            #endregion

            #region Properties
            public static ToolTipItemBehavior Instance
            {
                get
                {
                    return m_instance;
                }
            }
            #endregion

            #region Fields
            private static ToolTipItemBehavior m_instance = new ToolTipItemBehavior();
            #endregion
        }
        #endregion

        #region *** ToolTipItemGlyph
       public class ToolTipItemGlyph : Glyph
        {
            #region Constructors
            public ToolTipItemGlyph(ToolTipInfo.ToolTipItem item, ISite controlSite)
                : base(ToolTipItemBehavior.Instance)
            {
                m_item = item;
                m_controlSite = controlSite;
            }
            #endregion

            #region Methods
     
            internal void Select()
            {
                if (m_controlSite != null)
                {
                    ISelectionService selSvc = m_controlSite.GetService(typeof(ISelectionService)) as ISelectionService;
                    if (selSvc != null)
                    {
                        selSvc.SetSelectedComponents(new object[] { m_item });

                        BehaviorService behaviorSvc = m_controlSite.GetService(typeof(BehaviorService)) as BehaviorService;

                        if (behaviorSvc != null)
                        {
                            behaviorSvc.Invalidate();
                        }
                    }
                }
            }
            #endregion

            #region Overrides
  
            public override void Paint(System.Windows.Forms.PaintEventArgs pe)
            {
                Color color = this.Selected ? SystemColors.ControlText : Color.FromArgb(64, SystemColors.ControlText);

                using (Pen pen = new Pen(color))
                {
                    if (!this.Selected)
                    {
                        pen.DashStyle = DashStyle.Dot;
                    }
                    pe.Graphics.DrawRectangle(pen, Rectangle.Inflate(this.Bounds, -1, -1));
                }
            }
        
            public override Cursor GetHitTest(Point p)
            {
                if (this.Bounds.Contains(p))
                {
                    return Cursors.Default;
                }
                return null;
            }
            #endregion

            #region Properties
     
            public override Rectangle Bounds
            {
                get
                {
                    Rectangle rcBounds = Rectangle.Empty;

                    if (m_item != null)
                    {
                        rcBounds = m_item.Bounds;

                        if (m_controlSite != null)
                        {
                            Control control = m_controlSite.Component as Control;
                            if (control != null)
                            {
                                BehaviorService behaviorSvc = m_controlSite.GetService(typeof(BehaviorService)) as BehaviorService;
                                if (behaviorSvc != null)
                                {
                                    rcBounds.Location = behaviorSvc.MapAdornerWindowPoint(control.Handle, rcBounds.Location);
                                }
                            }
                        }
                    }

                    return rcBounds;
                }
            }

           public bool Selected
            {
                get
                {
                    bool bResult = false;

                    if (m_controlSite != null)
                    {
                        ISelectionService selSvc = m_controlSite.GetService(typeof(ISelectionService)) as ISelectionService;
                        if (selSvc != null)
                        {
                            bResult = selSvc.GetComponentSelected(m_item);
                        }
                    }

                    return bResult;
                }
            }
            #endregion

            #region Fields
           private ToolTipInfo.ToolTipItem m_item;
           private ISite m_controlSite;
            #endregion
        }
        #endregion

        #region Overrides
  
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            SubscribeToComponentEvents();
        }
     
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnsubscribeFromComponentEvents();
            }
            base.Dispose(disposing);
        }
  
        public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
        {
            GlyphCollection glyphs = base.GetGlyphs(selectionType);

            ToolTipControl toolTip = this.Component as ToolTipControl;
            if (toolTip != null)
            {
                ToolTipInfo toolTipInfo = toolTip.Info;

                if (!toolTipInfo.Header.Hidden)
                    glyphs.Add(new ToolTipItemGlyph(toolTipInfo.Header, toolTip.Site));
                if (!toolTipInfo.Body.Hidden)
                    glyphs.Add(new ToolTipItemGlyph(toolTipInfo.Body, toolTip.Site));
                if (!toolTipInfo.Footer.Hidden)
                    glyphs.Add(new ToolTipItemGlyph(toolTipInfo.Footer, toolTip.Site));
            }
            return glyphs;
        }
        #endregion

        #region Implementation

        public void SubscribeToComponentEvents()
        {
            ToolTipControl toolTip = this.Component as ToolTipControl;
            if (toolTip != null)
            {
                toolTip.SizeChanged += new EventHandler(OnSizeChanged);
                toolTip.PropertyChanged += new ToolTipPropertyChangedEventHandler(OnToolTipPropertyChanged);
            }
        }

        public void UnsubscribeFromComponentEvents()
        {
            ToolTipControl toolTip = this.Component as ToolTipControl;
            if (toolTip != null)
            {
                toolTip.SizeChanged -= new EventHandler(OnSizeChanged);
                toolTip.PropertyChanged -= new ToolTipPropertyChangedEventHandler(OnToolTipPropertyChanged);
            }
        }

        public void OnComponentChanged()
        {
            ISite site = this.Component.Site;
            if (site != null)
            {
                IComponentChangeService svc = site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                if (svc != null)
                {
                    svc.OnComponentChanging(this.Component, null);
                    svc.OnComponentChanged(this.Component, null, null, null);
                }
            }
        }
        #endregion

        #region Event Handlers

        public void OnSizeChanged(object sender, EventArgs e)
        {
            OnComponentChanged();
        }
       
       public void OnToolTipPropertyChanged(object sender, ToolTipPropertyID id)
        {
            if (id == ToolTipPropertyID.ItemHidden && sender is ToolTipInfo.ToolTipItem)
            {
                ToolTipInfo.ToolTipItem item = (ToolTipInfo.ToolTipItem)sender;

                if (item.Hidden)
                {
                    ISite site = this.Component.Site;
                    if (site != null)
                    {
                        ISelectionService selSvc = site.GetService(typeof(ISelectionService)) as ISelectionService;

                        if (selSvc != null && selSvc.GetComponentSelected(item))
                        {
                            if (selSvc.SelectionCount > 1)
                            {
                                selSvc.SetSelectedComponents(new object[] { item }, SelectionTypes.Remove);
                            }
                            else
                            {
                                selSvc.SetSelectedComponents(new object[] { this.Component }, SelectionTypes.Replace);
                            }
                        }
                    }
                }
                OnComponentChanged();
            }
        }
        #endregion
    }
}

#endif