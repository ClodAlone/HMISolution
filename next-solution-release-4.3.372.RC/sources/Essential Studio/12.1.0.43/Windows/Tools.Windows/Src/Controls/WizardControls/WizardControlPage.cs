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

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// The WizardControlPage is used with the WizardControl.
    /// </summary>
    [Designer(typeof(WizardPageDesigner))]
    [ToolboxItem(false)]
    public class WizardControlPage : WizardPage
    {
        #region Variables
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private bool helpVisible = true;
        private bool helpEnabled = true;
        private bool finishVisible = true;
        private bool finishEnabled = true;
        private bool nextVisible = true;
        private bool nextEnabled = true;
        private bool backVisible = true;
        private bool backEnabled = true;
        private bool cancelVisible = true;
        private bool cancelEnabled = true;

        private bool fullPage = false;
        private bool cancelOverFinish = true;

        private string description = "This is the description of the Wizard Page";

        /// <summary>
        /// Occurs to notify that the cancel button was clicked.
        /// </summary>
        [Category("Wizard Events"), Description("Occurs to notify that the cancel button was clicked.")]
        public event System.EventHandler CancelClick;

        /// <summary>
        /// Occurs to notify that the back button was clicked.
        /// </summary>
        [Category("Wizard Events"), Description("Occurs to notify that the back button was clicked.")]
        public event System.EventHandler BackClick;

        /// <summary>
        /// Occurs to notify that the next button was clicked.
        /// </summary>
        [Category("Wizard Events"), Description("Occurs to notify that the next button was clicked.")]
        public event System.EventHandler NextClick;

        /// <summary>
        /// Occurs to notify that the finish button was clicked.
        /// </summary>
        [Category("Wizard Events"), Description("Occurs to notify that the finish button was clicked.")]
        public event System.EventHandler FinishClick;

        /// <summary>
        /// Occurs to notify that the help button was clicked.
        /// </summary>
        [Category("Wizard Events"), Description("Occurs to notify that the help button was clicked.")]
        public event System.EventHandler HelpClick;

        /// <summary>
        /// Occurs to validate a page before a new page is selected.
        /// </summary>
        /// <remarks>You can validate this page
        /// and cancel new page selection, if necessary.</remarks>
        [Category("Wizard Events"), Description("Occurs to validate a page before a new page is selected.")]
        public event CancelEventHandler ValidatePage;

        #endregion

        #region EventMethods
        private void OnCancelClick(EventArgs e)
        {
            if (CancelClick != null)
            {
                CancelClick(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="CancelClick"/> event.
        /// </summary>
        public void RaiseCancelClick()
        {
            OnCancelClick(EventArgs.Empty);
        }

        private void OnBackClick(EventArgs e)
        {
            if (BackClick != null)
            {
                BackClick(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="BackClick"/> event.
        /// </summary>
        public void RaiseBackClick()
        {
            OnBackClick(EventArgs.Empty);
        }

        private void OnNextClick(EventArgs e)
        {
            if (NextClick != null)
            {
                NextClick(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="NextClick"/> event.
        /// </summary>
        public void RaiseNextClick()
        {
            OnNextClick(EventArgs.Empty);
        }

        private void OnFinishClick(EventArgs e)
        {
            if (FinishClick != null)
            {
                FinishClick(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="FinishClick"/> event.
        /// </summary>
        public void RaiseFinishClick()
        {
            OnFinishClick(EventArgs.Empty);
        }

        private void OnValidatePage(CancelEventArgs e)
        {
            if (ValidatePage != null)
            {
                ValidatePage(this, e);
            }
        }
    
        /// <summary>
        /// Raises the <see cref="ValidatePage"/> event.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void RaiseValidatePage(CancelEventArgs e)
        {
            OnValidatePage(e);
        }
        private void OnHelpClick(EventArgs e)
        {
            if (HelpClick != null)
            {
                HelpClick(this, e);
            }
        }
      
        /// <summary>
        /// Raises the <see cref="HelpClick"/> event.
        /// </summary>
        public void RaiseHelpClick()
        {
            OnHelpClick(EventArgs.Empty);
        }

        #endregion
        #region Properties
       
        #region Page Properties
        /// <summary>
        /// Gets or sets the description of the page. Appears in the Description Label of the BannerPanel of the WizardControl.
        /// </summary>
        [Description("The description of the page. Appears in the Description Label of the BannerPanel of the WizardControl.")]
        [Category("Page")]
        [Localizable(true)]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Description
        {
            get  
            {
                return description;
            }
            set
            {
                if (description != value)
                {
                    description = value;
                    OnSettingsChanged(EventArgs.Empty);
                }
            }
        }
     
        /// <summary>
        /// Gets or sets a value indicating whether the BannerPanel of the WizardControl is hidden when this page is selected.
        /// </summary>
        [Description("Determines if the BannerPanel of the WizardControl is hidden when this page is selected.")]
        [Category("Page")]
        public bool FullPage
        {
            get 
            { 
                return fullPage; 
            }
            set
            {
                fullPage = value;
                OnSettingsChanged(EventArgs.Empty);
            }
        }

        #endregion

        #region Button Properties
        /// <summary>
        /// Gets or sets a value indicating whether the Cancel button is positioned over the Finish button.
        /// </summary>
        /// <remarks>This will override the <see cref="FinishVisible"/> setting if true.</remarks>
        [Description("Determines if the Cancel button is positioned over the Finish button. Will override the FinishVisible setting if true.")]
        [Category("Buttons")]
        [DefaultValue(true)]
        public bool CancelOverFinish
        {
            get 
            {
                return cancelOverFinish;
            }
            set 
            { 
                cancelOverFinish = value; 
                OnSettingsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether  the Help button is visible.
        /// </summary>
        [Description("Determines if the Help button is visible.")]
        [Category("Buttons")]
        [DefaultValue(true)]
        public bool HelpVisible
        {
            get 
            { 
                return helpVisible; 
            }
            set
            {
                helpVisible = value;
                OnSettingsChanged(EventArgs.Empty);
            }
        }
      
        /// <summary>
        /// Gets or sets a value indicating whether the Help button is enabled.
        /// </summary>
        [Description("Determines if the Help button is enabled.")]
        [Category("Buttons")]
        [DefaultValue(true)]
        public bool HelpEnabled
        {
            get
            { 
                return helpEnabled;
            }
            set 
            { 
                helpEnabled = value;
                OnSettingsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Finish button is visible.
        /// </summary>
        /// <remarks>This will be overridden by the <see cref="CancelOverFinish"/> property.</remarks>
        [Description("Determines if the Finish button is visible. This will be overridden by the CancelOverFinish property.")]
        [Category("Buttons")]
        [DefaultValue(true)]
        public bool FinishVisible
        {
            get
            {
                return finishVisible;
            }
            set 
            { 
                finishVisible = value; 
                OnSettingsChanged(EventArgs.Empty); 
            }
        }
       
        /// <summary>
        /// Gets or sets a value indicating whether the Finish button is enabled.
        /// </summary>
        [Description("Determines if the Finish button is enabled.")]
        [Category("Buttons")]
        [DefaultValue(true)]
        public bool FinishEnabled
        {
            get
            { 
                return finishEnabled;
            }
            set 
            { 
                finishEnabled = value;
                OnSettingsChanged(EventArgs.Empty);
            }
        }
     
        /// <summary>
        /// Gets or sets a value indicating whether the Next button is visible.
        /// </summary>
        [Description("Determines if the Next button is visible.")]
        [Category("Buttons")]
        [DefaultValue(true)]
        public bool NextVisible
        {
            get 
            { 
                return nextVisible; 
            }
            set 
            { 
                nextVisible = value;
                OnSettingsChanged(EventArgs.Empty); 
            }
        }
      
        /// <summary>
        /// Gets or sets a value indicating whether the Next button is enabled.
        /// </summary>
        [Description("Determines if the Next button is enabled.")]
        [Category("Buttons")]
        [DefaultValue(true)]
        public bool NextEnabled
        {
            get 
            { 
                return nextEnabled; 
            }
            set 
            { 
                nextEnabled = value;
                OnSettingsChanged(EventArgs.Empty); 
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the Back button is visible.
        /// </summary>
        [Description("Determines if the Back button is visible.")]
        [Category("Buttons")]
        [DefaultValue(true)]
        public bool BackVisible
        {
            get
            { 
                return backVisible; 
            }
            set 
            {
                backVisible = value;
                OnSettingsChanged(EventArgs.Empty); 
            }
        }
     
        /// <summary>
        /// Gets or sets a value indicating whether the Back button is enabled.
        /// </summary>
        [Description("Determines if the Back button is enabled.")]
        [Category("Buttons")]
        [DefaultValue(true)]
        public bool BackEnabled
        {
            get 
            { 
                return backEnabled; 
            }
            set
            {
                backEnabled = value; 
                OnSettingsChanged(EventArgs.Empty);
            }
        }
      
        /// <summary>
        /// Gets or sets a value indicating whether the Cancel button is visible.
        /// </summary>
        [Description("Determines if the Cancel button is visible.")]
        [Category("Buttons")]
        [DefaultValue(true)]
        public bool CancelVisible
        {
            get 
            { 
                return cancelVisible; 
            }
            set
            { 
                cancelVisible = value; 
                OnSettingsChanged(EventArgs.Empty); 
            }
        }
       
        /// <summary>
        /// Gets or sets a value indicating whether the Cancel button is enabled.
        /// </summary>
        [Description("Determines if the Cancel button is enabled.")]
        [Category("Buttons")]
        [DefaultValue(true)]
        public bool CancelEnabled
        {
            get
            {
                return cancelEnabled; 
            }
            set
            { 
                cancelEnabled = value; 
                OnSettingsChanged(EventArgs.Empty);
            }
        }
        #endregion
        #endregion

        public WizardControlPage(System.ComponentModel.IContainer container)
        {
            // Required for Windows.Forms Class Composition Designer support.
            container.Add(this);
            InitializeComponent();

            // TODO: Add any constructor code after InitializeComponent call
        }

        public WizardControlPage()
        {
            // Required for Windows.Forms Class Composition Designer support.
            InitializeComponent();

            // TODO: Add any constructor code after InitializeComponent cal
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