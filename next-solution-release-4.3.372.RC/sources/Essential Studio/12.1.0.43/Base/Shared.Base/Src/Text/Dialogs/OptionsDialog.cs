#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Text
{
    internal class OptionsDialog : Form
    {
        SpellChecker spellChecker = null;

        #region SpellChecker

        /// <summary>
        /// Gets or sets the <see cref="SpellChecker"/> assiciated with this dialog.
        /// </summary>
        public SpellChecker SpellChecker
        {
            get { return spellChecker; }
            set { spellChecker = value; }
        }

        #endregion

        #region Constructor
        
        public OptionsDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Helper Methods

        public void UpdateOptionButtons()
        {
            this.OptionExcludeEmailAddress.Checked = SpellChecker.ExcludeEmailAddress;
            this.OptionExcludeFileNames.Checked = SpellChecker.ExcludeFileNames;
            this.OptionExcludeHtmlTags.Checked = SpellChecker.ExcludeHtmlTags;
            this.OptionExcludeInternetAddresses.Checked = SpellChecker.ExcludeInternetAddresses;
            this.OptionExcludeSpecialSymbols.Checked = SpellChecker.ExcludeSpecialSymbols;
            this.OptionExcludeWordsInMixedCase.Checked = SpellChecker.ExcludeWordsInMixedCase;
            this.OptionExcludeWordsInUpperCase.Checked = SpellChecker.ExcludeWordsInUpperCase;
            this.OptionExcludeWordsWithNumbers.Checked = SpellChecker.ExcludeWordsWithNumbers;
        }

        private void UpdateSpellChecker()
        {
            SpellChecker.ExcludeEmailAddress = this.OptionExcludeEmailAddress.Checked;
            SpellChecker.ExcludeFileNames = this.OptionExcludeFileNames.Checked;
            SpellChecker.ExcludeHtmlTags = this.OptionExcludeHtmlTags.Checked;
            SpellChecker.ExcludeInternetAddresses = this.OptionExcludeInternetAddresses.Checked;
            SpellChecker.ExcludeSpecialSymbols = this.OptionExcludeSpecialSymbols.Checked;
            SpellChecker.ExcludeWordsInMixedCase = this.OptionExcludeWordsInMixedCase.Checked;
            SpellChecker.ExcludeWordsInUpperCase = this.OptionExcludeWordsInUpperCase.Checked;
            SpellChecker.ExcludeWordsWithNumbers = this.OptionExcludeWordsWithNumbers.Checked;
        }

        internal void ShowOptionsDialog(SpellChecker spellChecker, Form owner)
        {
            this.SpellChecker = spellChecker;
            this.UpdateOptionButtons();

            this.ShowDialog(owner);
        }

        #endregion

        #region Event Handlers
        
        private void Btn_Ok_Click(object sender, EventArgs e)
        {
            UpdateSpellChecker();
            this.Owner.Activate();
            this.Hide();
        }

        #endregion

        #region Designer Variables
        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lbl_SpellCheckerOptions;
        private System.Windows.Forms.GroupBox separator1;
        private System.Windows.Forms.CheckBox OptionExcludeWordsInUpperCase;
        private System.Windows.Forms.CheckBox OptionExcludeSpecialSymbols;
        private System.Windows.Forms.CheckBox OptionExcludeWordsWithNumbers;
        private System.Windows.Forms.CheckBox OptionExcludeWordsInMixedCase;
        private System.Windows.Forms.CheckBox OptionExcludeHtmlTags;
        private System.Windows.Forms.CheckBox OptionExcludeFileNames;
        private System.Windows.Forms.CheckBox OptionExcludeInternetAddresses;
        private System.Windows.Forms.CheckBox OptionExcludeEmailAddress;
        private System.Windows.Forms.Button Btn_Cancel;
        private System.Windows.Forms.Button Btn_Ok;
        private System.Windows.Forms.GroupBox separator2;
        
        #endregion

        #region Dispose

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbl_SpellCheckerOptions = new System.Windows.Forms.Label();
            this.separator1 = new System.Windows.Forms.GroupBox();
            this.OptionExcludeWordsInUpperCase = new System.Windows.Forms.CheckBox();
            this.OptionExcludeSpecialSymbols = new System.Windows.Forms.CheckBox();
            this.OptionExcludeWordsWithNumbers = new System.Windows.Forms.CheckBox();
            this.OptionExcludeWordsInMixedCase = new System.Windows.Forms.CheckBox();
            this.OptionExcludeHtmlTags = new System.Windows.Forms.CheckBox();
            this.OptionExcludeFileNames = new System.Windows.Forms.CheckBox();
            this.OptionExcludeInternetAddresses = new System.Windows.Forms.CheckBox();
            this.OptionExcludeEmailAddress = new System.Windows.Forms.CheckBox();
            this.Btn_Cancel = new System.Windows.Forms.Button();
            this.Btn_Ok = new System.Windows.Forms.Button();
            this.separator2 = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.lbl_SpellCheckerOptions.AutoSize = true;
            this.lbl_SpellCheckerOptions.Location = new System.Drawing.Point(13, 13);
            this.lbl_SpellCheckerOptions.Name = "label1";
            this.lbl_SpellCheckerOptions.Size = new System.Drawing.Size(112, 13);
            this.lbl_SpellCheckerOptions.TabIndex = 0;
            this.lbl_SpellCheckerOptions.Text = SR.GetString(ResourceIdentifiers.SpellCheckerLabelOptions);
            // 
            // separator1
            // 
            this.separator1.Location = new System.Drawing.Point(10, 31);
            this.separator1.Name = "separator1";
            this.separator1.Size = new System.Drawing.Size(412, 3);
            this.separator1.TabIndex = 1;
            this.separator1.TabStop = false;
            // 
            // OptionExcludeWordsInUpperCase
            // 
            this.OptionExcludeWordsInUpperCase.AutoSize = true;
            this.OptionExcludeWordsInUpperCase.Location = new System.Drawing.Point(45, 48);
            this.OptionExcludeWordsInUpperCase.Name = "OptionExcludeWordsInUpperCase";
            this.OptionExcludeWordsInUpperCase.Size = new System.Drawing.Size(169, 17);
            this.OptionExcludeWordsInUpperCase.TabIndex = 0;
            this.OptionExcludeWordsInUpperCase.Text = SR.GetString(ResourceIdentifiers.SpellCheckerIgnoreUpperCase);
            this.OptionExcludeWordsInUpperCase.UseVisualStyleBackColor = true;
            // 
            // OptionExcludeSpecialSymbols
            // 
            this.OptionExcludeSpecialSymbols.AutoSize = true;
            this.OptionExcludeSpecialSymbols.Location = new System.Drawing.Point(45, 117);
            this.OptionExcludeSpecialSymbols.Name = "OptionExcludeSpecialSymbols";
            this.OptionExcludeSpecialSymbols.Size = new System.Drawing.Size(148, 17);
            this.OptionExcludeSpecialSymbols.TabIndex = 4;
            this.OptionExcludeSpecialSymbols.Text = SR.GetString(ResourceIdentifiers.SpellCheckerIgnoreSpecialCharacters);
            this.OptionExcludeSpecialSymbols.UseVisualStyleBackColor = true;
            // 
            // OptionExcludeWordsWithNumbers
            // 
            this.OptionExcludeWordsWithNumbers.AutoSize = true;
            this.OptionExcludeWordsWithNumbers.Location = new System.Drawing.Point(45, 94);
            this.OptionExcludeWordsWithNumbers.Name = "OptionExcludeWordsWithNumbers";
            this.OptionExcludeWordsWithNumbers.Size = new System.Drawing.Size(152, 17);
            this.OptionExcludeWordsWithNumbers.TabIndex = 2;
            this.OptionExcludeWordsWithNumbers.Text = SR.GetString(ResourceIdentifiers.SpellCheckerIgnoreWordsWithNumbers);
            this.OptionExcludeWordsWithNumbers.UseVisualStyleBackColor = true;
            // 
            // OptionExcludeWordsInMixedCase
            // 
            this.OptionExcludeWordsInMixedCase.AutoSize = true;
            this.OptionExcludeWordsInMixedCase.Location = new System.Drawing.Point(45, 71);
            this.OptionExcludeWordsInMixedCase.Name = "OptionExcludeWordsInMixedCase";
            this.OptionExcludeWordsInMixedCase.Size = new System.Drawing.Size(156, 17);
            this.OptionExcludeWordsInMixedCase.TabIndex = 1;
            this.OptionExcludeWordsInMixedCase.Text = SR.GetString(ResourceIdentifiers.SpellCheckerIgnoreMixedCase);
            this.OptionExcludeWordsInMixedCase.UseVisualStyleBackColor = true;
            // 
            // OptionExcludeHtmlTags
            // 
            this.OptionExcludeHtmlTags.AutoSize = true;
            this.OptionExcludeHtmlTags.Location = new System.Drawing.Point(231, 117);
            this.OptionExcludeHtmlTags.Name = "OptionExcludeHtmlTags";
            this.OptionExcludeHtmlTags.Size = new System.Drawing.Size(103, 17);
            this.OptionExcludeHtmlTags.TabIndex = 8;
            this.OptionExcludeHtmlTags.Text = SR.GetString(ResourceIdentifiers.SpellCheckerIgnoreHTMLTags);
            this.OptionExcludeHtmlTags.UseVisualStyleBackColor = true;
            // 
            // OptionExcludeFileNames
            // 
            this.OptionExcludeFileNames.AutoSize = true;
            this.OptionExcludeFileNames.Location = new System.Drawing.Point(231, 48);
            this.OptionExcludeFileNames.Name = "OptionExcludeFileNames";
            this.OptionExcludeFileNames.Size = new System.Drawing.Size(109, 17);
            this.OptionExcludeFileNames.TabIndex = 5;
            this.OptionExcludeFileNames.Text = SR.GetString(ResourceIdentifiers.SpellCheckerIgnoreFileNames);
            this.OptionExcludeFileNames.UseVisualStyleBackColor = true;
            // 
            // OptionExcludeInternetAddresses
            // 
            this.OptionExcludeInternetAddresses.AutoSize = true;
            this.OptionExcludeInternetAddresses.Location = new System.Drawing.Point(231, 71);
            this.OptionExcludeInternetAddresses.Name = "OptionExcludeInternetAddresses";
            this.OptionExcludeInternetAddresses.Size = new System.Drawing.Size(145, 17);
            this.OptionExcludeInternetAddresses.TabIndex = 6;
            this.OptionExcludeInternetAddresses.Text = SR.GetString(ResourceIdentifiers.SpellCheckerIgnoreInternetAddress);
            this.OptionExcludeInternetAddresses.UseVisualStyleBackColor = true;
            // 
            // OptionExcludeEmailAddress
            // 
            this.OptionExcludeEmailAddress.AutoSize = true;
            this.OptionExcludeEmailAddress.Location = new System.Drawing.Point(231, 94);
            this.OptionExcludeEmailAddress.Name = "OptionExcludeEmailAddress";
            this.OptionExcludeEmailAddress.Size = new System.Drawing.Size(128, 17);
            this.OptionExcludeEmailAddress.TabIndex = 7;
            this.OptionExcludeEmailAddress.Text = SR.GetString(ResourceIdentifiers.SpellCheckerIgnoreEmailAddress);
            this.OptionExcludeEmailAddress.UseVisualStyleBackColor = true;
            // 
            // Btn_Cancel
            // 
            this.Btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Btn_Cancel.Location = new System.Drawing.Point(345, 168);
            this.Btn_Cancel.Name = "Btn_Cancel";
            this.Btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.Btn_Cancel.TabIndex = 10;
            this.Btn_Cancel.Text = SR.GetString(ResourceIdentifiers.Cancel);
            this.Btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // Btn_Ok
            // 
            this.Btn_Ok.Location = new System.Drawing.Point(264, 168);
            this.Btn_Ok.Name = "Btn_Ok";
            this.Btn_Ok.Size = new System.Drawing.Size(75, 23);
            this.Btn_Ok.TabIndex = 9;
            this.Btn_Ok.Text = SR.GetString(ResourceIdentifiers.OK);
            this.Btn_Ok.UseVisualStyleBackColor = true;
            this.Btn_Ok.Click += new System.EventHandler(this.Btn_Ok_Click);
            // 
            // separator2
            // 
            this.separator2.Location = new System.Drawing.Point(10, 156);
            this.separator2.Name = "separator2";
            this.separator2.Size = new System.Drawing.Size(412, 3);
            this.separator2.TabIndex = 13;
            this.separator2.TabStop = false;
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.Btn_Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Btn_Cancel;
            this.ClientSize = new System.Drawing.Size(432, 199);
            this.Controls.Add(this.separator2);
            this.Controls.Add(this.Btn_Ok);
            this.Controls.Add(this.Btn_Cancel);
            this.Controls.Add(this.OptionExcludeHtmlTags);
            this.Controls.Add(this.OptionExcludeFileNames);
            this.Controls.Add(this.OptionExcludeInternetAddresses);
            this.Controls.Add(this.OptionExcludeEmailAddress);
            this.Controls.Add(this.OptionExcludeWordsInMixedCase);
            this.Controls.Add(this.OptionExcludeWordsWithNumbers);
            this.Controls.Add(this.OptionExcludeSpecialSymbols);
            this.Controls.Add(this.OptionExcludeWordsInUpperCase);
            this.Controls.Add(this.separator1);
            this.Controls.Add(this.lbl_SpellCheckerOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = SR.GetString(ResourceIdentifiers.SpellCheckerOptionsDialogCaption);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
