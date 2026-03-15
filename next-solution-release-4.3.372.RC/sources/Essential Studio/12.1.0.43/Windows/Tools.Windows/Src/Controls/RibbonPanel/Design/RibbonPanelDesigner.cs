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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Designer for RibbonPanel.
    /// </summary>
   public class RibbonPanelDesigner
        : ParentControlDesigner
    {
        #region Fields

        /// <summary>
        /// Collection of verbs.
        /// </summary>
        private DesignerVerbCollection m_verbs = new DesignerVerbCollection();

        /// <summary>
        /// Design time RibbonPanel instance.
        /// </summary>
        private RibbonPanel m_control;
        #endregion

        #region Static Fields
    
        private static Pen _adornmentBorderPen;
        #endregion

        #region Initialization

        static RibbonPanelDesigner()
        {
            _adornmentBorderPen = new Pen(Color.Black);
            _adornmentBorderPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        }
     
        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            m_control = (RibbonPanel)component;

            m_control.ToolStripAdded += new ToolStripEventHandler(OnToolStripAdded);
            m_control.ToolStripRemoved += new ToolStripEventHandler(OnToolStripRemoved);
            m_control.VisibleChanged += new EventHandler(OnPanelVisibleChanged);

            m_verbs.Add(new DesignerVerb("Add ToolStrip", new EventHandler(AddToolStrip)));

            foreach (Control control in m_control.Controls)
            {
                ToolStripEx ts = control as ToolStripEx;
                if (ts != null)
                {
                    OnToolStripAdded(m_control, new ToolStripEventArgs(ts));
                }
            }
        }
        #endregion

        #region Verbs
 
        public void AddToolStrip(object sender, EventArgs e)
        {
            if (this.Control.Site != null)
            {
                IDesignerHost designerHost = this.Control.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

                if (designerHost != null)
                {
                    ToolStripEx toolStrip = (ToolStripEx)designerHost.CreateComponent(typeof(ToolStripEx));
                    m_control.AddToolStrip(toolStrip);
                }
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Gets collection of verbs.
        /// </summary>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                return m_verbs;
            }
        }

        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            base.OnPaintAdornments(pe);
        }
   
        public override bool CanParent(Control control)
        {
            return control is ToolStripEx;
        }

        public override SelectionRules SelectionRules
        {
            get
            {
                SelectionRules result = base.SelectionRules;

                if (m_control.Parent is RibbonControlAdv)
                {
                    result = SelectionRules.None;
                }

                return result;
            }
        }

        protected override void OnMouseDragBegin(int x, int y)
        {
            IToolboxService toolboxService = this.GetService(typeof(IToolboxService)) as IToolboxService;

            if (toolboxService != null)
            {
                ToolboxItem item = toolboxService.GetSelectedToolboxItem((IDesignerHost)this.GetService(typeof(IDesignerHost)));

                if (item == null || item.TypeName == typeof(ToolStripEx).FullName)
                {
                    base.OnMouseDragBegin(x, y);
                }
            }
        }
        #endregion

        #region Event Handlers
   
       public void OnToolStripAdded(object sender, ToolStripEventArgs args)
        {
            ToolStripEx ts = args.ToolStrip as ToolStripEx;

            if (ts != null)
            {
                ts.StateChanging += new System.ComponentModel.CancelEventHandler(OnTolStripStateChanging);

                ts.State = ToolStripEx.ToolStripExState.Expanded;
            }
        }

       public void OnToolStripRemoved(object sender, ToolStripEventArgs args)
        {
            ToolStripEx ts = args.ToolStrip as ToolStripEx;

            if (ts != null)
            {
                ts.StateChanging -= new System.ComponentModel.CancelEventHandler(OnTolStripStateChanging);
            }
        }

        public void OnPanelVisibleChanged(object sender, EventArgs e)
        {
            Control c = this.Control;
            if (c != null)
            {
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(c).Find("Visible", false);
                bool value =(bool) descriptor.GetValue(c);
                if (descriptor != null && value != c.Visible)
                {                       
                    descriptor.SetValue(c, c.Visible);
                }
            }
        }

       public void OnTolStripStateChanging(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ToolStripEx ts = sender as ToolStripEx;

            if (ts != null && ts.State == ToolStripEx.ToolStripExState.Expanded)
            {
                e.Cancel = true;
            }
        }
        #endregion
    }
}
#endif