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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Control representing a tab item with associated tab page.
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.None)]
    public class RibbonTabItem
        : ToolStripButton, IToolStripTabItem
    {
        #region Constants
        /// <summary>
        /// Width of tab item.
        /// </summary>
        private const int DEF_ITEM_WIDTH = 42;
        #endregion

        #region Fields
        /// <summary>
        /// Associated tab page.
        /// </summary>
        private RibbonTabPage m_page;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets associated tab page.
        /// </summary>
        [Browsable(false)]
        public RibbonTabPage Page
        {
            get
            {
                return m_page;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("Page");

                m_page = value;
            }
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the RibbonTabItem class.
        /// </summary>
        /// <param name="page">Ribbon tabpage</param>
        public RibbonTabItem(RibbonTabPage page)
            : this()
        {
            if (page == null) throw new ArgumentNullException("page");

            m_page = page;
        }

        /// <summary>
        /// Initializes a new instance of the RibbonTabItem class.
        /// </summary>
        public RibbonTabItem()
        {
            this.Text = this.Name;
            this.Width = DEF_ITEM_WIDTH;
            this.AutoSize = true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Raises Activated event.
        /// </summary>
        public void Activate()
        {
            if (!this.Checked)
            {
                this.Checked = true;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Activates the item.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Activate();
        }
        #endregion
    }
}
#endif