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
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    #region MiniToolBarDesigner
   public class MiniToolBarDesigner : ComponentDesigner
    {
        #region Constructors
       
        public MiniToolBarDesigner()
        {
            m_glyphs = new GlyphCollection();
        }
        #endregion

        #region Properties
       
        private BehaviorService BehaviorService
        {
            get { return this.GetService(typeof(BehaviorService)) as BehaviorService; }
        }
        #endregion

        #region Overrides
      
        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            m_toolBar = component as MiniToolBar;
            if (m_toolBar != null)
            {
                UpdateOwnerItem();

                m_toolStripSvc = ToolStripExService.Get(m_toolBar.Site);
                m_ribbonAdornerSvc = RibbonAdornerService.Get(m_toolBar.Site);

                ISelectionService selSvc = DesignerUtils.GetSelectionService(component);
                if (selSvc != null)
                {
                    selSvc.SelectionChanged += new EventHandler(OnSelectionChanged);
                }
                m_toolBar.Opening += new CancelEventHandler(OnOpening);
                m_toolBar.Opened += new EventHandler(OnOpened);
                m_toolBar.VisibleChanged += new EventHandler(OnVisibleChanged);
                m_toolBar.LayoutCompleted += new EventHandler(OnLayoutCompleted);
            }
        }
       
        protected override void Dispose(bool disposing)
        {
            ISelectionService selSvc = DesignerUtils.GetSelectionService(this.Component);
            if (selSvc != null)
            {
                selSvc.SelectionChanged -= new EventHandler(OnSelectionChanged);
            }
            if (m_toolBar != null)
            {
                m_toolBar.Opening -= new CancelEventHandler(OnOpening);
                m_toolBar.Opened -= new EventHandler(OnOpened);
                m_toolBar.VisibleChanged -= new EventHandler(OnVisibleChanged);
                m_toolBar.LayoutCompleted -= new EventHandler(OnLayoutCompleted);
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Event handlers
      
        private void OnSelectionChanged(object sender, EventArgs e)
        {
            MiniToolBar toolBar = this.Component as MiniToolBar;
            if (toolBar != null)
            {
                if (m_toolStripSvc != null)
                {
                    bool bSelected = GetIsSelected(m_toolStripSvc.PrimarySelection);

                    if (toolBar.Visible != bSelected)
                    {
                        if (bSelected)
                        {
                            Control root = DesignerUtils.GetRootControl(toolBar);
                            if (root != null)
                            {
                                DesignerUtils.UpdateDropDownParent(toolBar);
                                toolBar.Show();
                            }
                        }
                        else toolBar.Hide();
                    }
                }

                this.BehaviorService.Invalidate();
            }
        }
       
        private void OnOpening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
        }
        
      private void OnOpened(object sender, EventArgs e)
        {
            // Place dropdown between the root control and ToolStripAdornerWindow
            Control root = DesignerUtils.GetRootControl(m_toolBar);
            if (root != null)
            {
                IntPtr hPrev = WindowsAPI.GetWindow(root.Handle, GetWindowCmd.GW_HWNDPREV);

                if (hPrev != IntPtr.Zero && hPrev != m_toolBar.Handle)
                {
                    WindowsAPI.SetWindowPos(m_toolBar.Handle, hPrev, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
                }
            }
        }

       private void OnVisibleChanged(object sender, EventArgs e)
        {
            // Set Handled to "true" to disable trasparency changes in design mode
            HandledEventArgs he = e as HandledEventArgs;
            if (he != null)
            {
                he.Handled = true;
            }
        }

       private void OnLayoutCompleted(object sender, EventArgs e)
        {
            UpdateGlyphs();
        }
        #endregion

        #region Implementation

        private void UpdateOwnerItem()
        {
            IDesignerHost designerHost = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (designerHost != null)
            {
                ToolStripItem ownerItem = new MiniToolBarOwnerItem(designerHost);

                Control root = designerHost.RootComponent as Control;
                if (root != null)
                {
                    ToolStrip ts = new ToolStrip();

                    ts.Visible = false;
                    ts.Items.Add(ownerItem);

                    root.Controls.Add(ts);
                }

                m_toolBar.OwnerItem = ownerItem;
            }
        }

        private void UpdateGlyphs()
        {
            if (m_ribbonAdornerSvc != null)
            {
                GlyphCollection glyphs = m_ribbonAdornerSvc.Adorner.Glyphs;

                foreach (Glyph g in m_glyphs)
                {
                    glyphs.Remove(g);
                }

                m_glyphs.Clear();

                UpdateGlyphs(m_glyphs, m_toolBar.Items);

                foreach (Glyph g in m_glyphs)
                {
                    glyphs.Insert(0, g);
                }
            }
        }

        private void UpdateGlyphs(GlyphCollection glyphs, ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                Glyph g = DesignerUtils.GetToolStripItemGlyph(item);
                if (g != null)
                {
                    glyphs.Add(g);
                }

                ToolStripPanelItem panelItem = item as ToolStripPanelItem;
                if (panelItem != null)
                {
                    UpdateGlyphs(glyphs, panelItem.Items);
                }
            }
        }

        private bool GetIsSelected(object obj)
        {
            return m_toolBar.Equals(obj) || DesignerUtils.IsDropDownItem(m_toolBar, obj as ToolStripItem);
        }
        #endregion

        #region Fields

        private MiniToolBar m_toolBar;

        private RibbonAdornerService m_ribbonAdornerSvc;

        private ToolStripExService m_toolStripSvc;

       private GlyphCollection m_glyphs;

        #endregion

        #region *** MiniToolBarOwnerItem
      public class MiniToolBarOwnerItem : ToolStripDropDownItem
        {
            #region Constructors
  
            public MiniToolBarOwnerItem(IDesignerHost designerHost)
            {
                m_designerHost = designerHost;
            }
            #endregion

            #region Properties
        
            protected override Point DropDownLocation
            {
                get
                {
                    if (m_designerHost != null)
                    {
                        Control root = m_designerHost.RootComponent as Control;
                        if (root != null)
                        {
                            Point pt = root.DisplayRectangle.Location;
                            foreach (Control c in root.Controls)
                            {
                                if (c.Visible)
                                {
                                    switch (c.Dock)
                                    {
                                        case DockStyle.Top:
                                            if (pt.Y < c.Bottom)
                                            {
                                                pt.Y = c.Bottom;
                                            }
                                            break;
                                        case DockStyle.Left:
                                            if (pt.X < c.Right)
                                            {
                                                pt.X = c.Right;
                                            }
                                            break;
                                    }
                                }
                            }
                            return root.PointToScreen(pt);
                        }
                    }
                    return Point.Empty;
                }
            }
            #endregion

            #region Fields
           private IDesignerHost m_designerHost;
            #endregion
        }
        #endregion
    }
    #endregion

    #region AssociatedControlTypeConverter
   public class AssociatedControlTypeConverter : ReferenceConverter
    {
        #region Constructors
        public AssociatedControlTypeConverter(Type type)
            : base(type)
        { 
        }
        #endregion

        #region Overrides
      
        protected override bool IsValueAllowed(ITypeDescriptorContext context, object value)
        {
            if (context != null && value != null)
            {
                object instance = context.Instance;

                if (instance != null && instance.GetType() != value.GetType())
                {
                    IContainer container = context.Container;
                    if (container != null)
                    {
                        ComponentCollection components = container.Components;
                        if (components != null)
                        {
                            foreach (object obj in components)
                            {
                                if (obj == value)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        #endregion
    }
    #endregion
}
#endif