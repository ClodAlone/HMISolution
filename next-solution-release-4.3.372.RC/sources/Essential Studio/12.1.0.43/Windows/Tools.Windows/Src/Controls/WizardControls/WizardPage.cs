#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// The WizardContainer is used with the WizardControl. It holds a collection of WizardPage controls.
    /// </summary>
    [ToolboxItem(false)]
    [Designer(typeof(WizardContainerDesigner))]
    public class WizardContainer : Panel
    { 
    }

    /// <summary>
    /// The WizardControlPage is used with the WizardControl.
    /// </summary>
    [ToolboxItem(false)]
    public class WizardPage : Syncfusion.Windows.Forms.Tools.GradientPanel, IComponent
    {
       #region Variables
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private WizardPage nextPage = null;
        private WizardPage previousPage = null;

        private string title = "Page Title";
        private string layoutName;

        /// <summary>
        /// Fired when the settings (Title or LayoutName) of the page are changed.
        /// </summary>
        [Description("Fired when the settings (Title or LayoutName) of the page are changed")]
        [Category("Page")]
        public event System.EventHandler SettingsChanged;
      
        /// <summary>
        /// Occurs when this page has been selected.
        /// </summary>
        [Category("Wizard Events"), Description("Occurs when this page has been selected.")]
        public event System.EventHandler PageLoad;

        #endregion
        #region Event Methods
        /// <summary>
        /// Raises the PageLoad event
        /// </summary>
        public void RaisePageLoad()
        {
            OnPageLoad(EventArgs.Empty);
        }

        protected virtual void OnPageLoad(System.EventArgs e)
        {
            if (PageLoad != null)
            {
                try
                {
                    PageLoad(this, e);
                }
                catch 
                {
                }
            }
        }
        protected virtual void OnSettingsChanged(System.EventArgs e)
        {
            if (SettingsChanged != null)
            {
                try
                {
                    SettingsChanged(this, e);
                }
                catch
                { 
                }
            }
        }

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the next page.
        /// </summary>
        public WizardPage NextPage
        {
            get 
            { 
                return nextPage;
            }
            set
            {
                nextPage = value;
                if (value != null && value.PreviousPage != this)
                    value.PreviousPage = this;
            }
        }

        /// <summary>
        /// Gets or sets the previous page.
        /// </summary>
        public WizardPage PreviousPage
        {
            get 
            {
                return previousPage;
            }
            set
            {
                previousPage = value;
                if (value != null && value.NextPage != this)
                    value.NextPage = this;
            }
        }
        #region Page Properties

        /// <summary>
        /// Gets or sets the name of the page as seen by the Wizard. Used with the SelectedPage property of the Wizard.
        /// </summary>
        [Description("The name of the page as seen by the Wizard. Used with the SelectedPage property of the Wizard.")]
        [Category("Page")]
        public string LayoutName
        {
            get 
            { 
                return layoutName;
            }
            set
            {
                if (value == String.Empty || value == null)
                {
                    if (this.DesignMode)
                        MessageBox.Show("Cannot set null or empty value to LayoutName.");
                    return;
                }
                if (layoutName != value)
                {
                    layoutName = value;
                    OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the title of the page. Appears in the Title Label of the Wizard.
        /// </summary>
        [Description("The title of the page. Appears in the TitleLabel of the Wizard.")]
        [Category("Page")]
        [Localizable(true)]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Title
        {
            get 
            { 
                return title; 
            }
            set
            {
                if (title != value)
                {
                    title = value;
                    OnSettingsChanged(EventArgs.Empty);
                }
            }
        }
        #endregion

        #endregion

        public WizardPage(System.ComponentModel.IContainer container)
        {
            // Required for Windows.Forms Class Composition Designer support
            container.Add(this);
            InitializeComponent();
            this.BorderStyle = BorderStyle.None;
    
            // TODO: Add any constructor code after InitializeComponent call
        }

        public WizardPage()
        {
            // Required for Windows.Forms Class Composition Designer support
            InitializeComponent();
            this.BorderStyle = BorderStyle.None;

            // TODO: Add any constructor code after InitializeComponent call
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion
    }
}