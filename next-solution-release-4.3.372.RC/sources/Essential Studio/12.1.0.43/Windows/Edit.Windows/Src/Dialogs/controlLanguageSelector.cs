#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Edit;
using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
    /// <summary>
    /// Control for selecting language.
    /// </summary>
    [ToolboxItem(false)]
    public class ControlLanguageSelector
        : BaseControlEditControlConfigurator
    {
        #region Controls
        private System.Windows.Forms.ComboBox comboLanguages;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        #endregion

        #region Initialization/Finalization
        /// <summary>
        /// Initializes a new instance of the ControlLanguageSelector class.
        /// </summary>
        public ControlLanguageSelector()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            comboLanguages.SelectedIndexChanged += new EventHandler(ComboLanguages_SelectedIndexChanged);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboLanguages = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // comboLanguages
            // 
            this.comboLanguages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLanguages.Location = new System.Drawing.Point(0, 0);
            this.comboLanguages.Name = "comboLanguages";
            this.comboLanguages.Size = new System.Drawing.Size(400, 21);
            this.comboLanguages.TabIndex = 0;
            // 
            // ControlLanguageSelector
            // 
            this.Controls.Add(this.comboLanguages);
            this.Name = "ControlLanguageSelector";
            this.Size = new System.Drawing.Size(400, 21);
            this.ResumeLayout(false);

        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets selected language.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IConfigLanguage SelectedLanguage
        {
            get
            {
                if (Configuration == null || comboLanguages.SelectedIndex < 0)
                    return null;

                return (IConfigLanguage)Configuration.KnownLanguages[comboLanguages.SelectedIndex];
            }
            set
            {
                int index = (value != null) ?
                    Configuration.KnownLanguageNames.IndexOf(value.Language) : -1;

                if (comboLanguages.SelectedIndex != index)
                {
                    comboLanguages.SelectedIndex = index;
                    OnLanguageSelected();
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when user selects some language.
        /// </summary>
        [Category("Date")]
        public event EventHandler SelectedLanguageChanged;
        #endregion

        #region Overrides
        /// <summary>
        /// Sets heights of the control.
        /// </summary>
        /// <param name="e">The EventArgs.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Height = comboLanguages.Height;
        }

        /// <summary>
        /// Raised when configuration is changed.
        /// </summary>
        protected override void OnConfigurationChanged()
        {
            UpdateLanguagesList();
        }

        /// <summary>
        /// Updates languages list.
        /// </summary>
        protected override void OnEditControlChanged()
        {
            UpdateLanguagesList();
            base.OnEditControlChanged();
        }
        #endregion

        #region Virtual Methods
        /// <summary>
        /// Updates languages list.
        /// </summary>
        protected virtual void UpdateLanguagesList()
        {
            comboLanguages.Items.Clear();

            if (Configuration != null)
            {
                int selectedIndex = -1;

                foreach (IConfigLanguage lang in Configuration.KnownLanguages)
                {
                    int index = comboLanguages.Items.Add(lang.Language);

                    if (EditControl != null && lang.Language == EditControl.Language.Language)
                        selectedIndex = index;
                }

                comboLanguages.SelectedIndex = selectedIndex;
            }
        }

        /// <summary>
        /// Raises SelectedLanguageChanged event.
        /// </summary>
        protected virtual void OnLanguageSelected()
        {
            if (SelectedLanguageChanged != null)
            {
                SelectedLanguageChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// Calls OnLanguageSelected() method.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void ComboLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnLanguageSelected();
        }
        #endregion
    }
}
