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

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <summary>
    /// Designer for RibbonTabPage.
    /// </summary>
    public class RibbonTabPageDesigner
        : ParentControlDesigner
    {
        #region Fields
       
        /// <summary>
        /// Design time RibbonHeaderControl instance.
        /// </summary>
        private RibbonTabPage m_control;
       
        /// <summary>
        /// Collection of verbs.
        /// </summary>
        private DesignerVerbCollection m_verbs = new DesignerVerbCollection();
        #endregion

        #region Static Fields
        /// <summary>
        /// Pen for drawing adornment border.
        /// </summary>
        private static Pen _adornmentsBorderPen = new Pen(Color.Gray);
        #endregion

        #region Initialization
   
        static RibbonTabPageDesigner()
        {
            _adornmentsBorderPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
        }
    
        /// <summary>
        /// Initializes new instance of RibbonHeaderControlDesigner.
        /// </summary>
        /// <param name="component">Component value</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            m_control = (RibbonTabPage)Component;

            if (m_control.Site != null)
            {
                DesignerActionService actionService = m_control.Site.GetService(typeof(DesignerActionService)) as DesignerActionService;

                if (actionService != null)
                {
                    actionService.Clear();
                }
            }
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
        /// Paints dash border.
        /// </summary>
        /// <param name="pe"> PaintEventArgs that contains the event data.</param>
        protected override void OnPaintAdornments(System.Windows.Forms.PaintEventArgs pe)
        {
            base.OnPaintAdornments(pe);

            pe.Graphics.DrawRectangle(_adornmentsBorderPen, 0, 0, m_control.Width - 1, m_control.Height - 1);
        }
        #endregion
    }
}
#endif