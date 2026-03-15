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
using System.Collections;
using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Text
{
    internal class SpellCheckerDialog : Form
    {
        #region Fields
        private SpellChecker spellChecker = null;
        private ISpellEditor editor = null;
        private bool TextUpdatePending = false;
        private bool IgnoreTextChange = false;
        Hashtable ChangeAllCollection = new Hashtable();
        Hashtable IgnoreAllCollection = new Hashtable();
        Hashtable IgnoreOnceCollection = new Hashtable();
        #endregion

        #region Properties
        public ISpellEditor Editor
        {
            get { return editor; }
            set { editor = value; }
        }
        public SpellChecker SpellChecker
        {
            get { return spellChecker; }
            set { spellChecker = value; }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Cannot create default instance of SpellCheckerDialog.
        /// </summary>
        private SpellCheckerDialog()
        { }

        public SpellCheckerDialog(SpellChecker spellChecker, ISpellEditor editor)
        {
            this.SpellChecker = spellChecker;
            this.Editor = editor;
            
            InitializeComponent();
        }

        #endregion

        #region Overriden Methods

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            ClearHash();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ProcessMisspelledWords();
        }

        #endregion

        #region Helper Methods

        private void UpdateSpellCheckerButtons()
        {
            bool enable = this.SpellChecker.MisspelledWords.Count > 0;

            if (this.Editor.Text != this.RTxt_Editor.Text)
            {
                TextUpdatePending = true;
                this.Btn_IgnoreOnce.Text = SR.GetString(ResourceIdentifiers.Update);
                this.Btn_Undo.Enabled = true;
            }
            else
            {
                this.Btn_IgnoreOnce.Text = SR.GetString(ResourceIdentifiers.IgnoreOnce);
                this.Btn_Undo.Enabled = false;
                TextUpdatePending = false;
            }

            this.Btn_IgnoreAll.Enabled = enable && !TextUpdatePending;
            this.Btn_AddToDicktionary.Enabled = enable && !TextUpdatePending && this.Editor.CurrentWord!= string.Empty;
            this.Btn_Change.Enabled = enable && !TextUpdatePending && this.LBox_Suggestions.Items.Count > 0;
            this.Btn_ChangeAll.Enabled = enable && !TextUpdatePending && this.LBox_Suggestions.Items.Count > 0;
        }

        public void ShowSpellCheckerDialog(Form owner)
        {
            this.SpellChecker.SpellCheck(this.Editor.Text);
            this.ShowDialog(owner);
        }

        private void ProcessMisspelledWords()
        {
            if (this.SpellChecker.MisspelledWords.Count > 0)
            {
                string misspelledWord = SpellChecker.MisspelledWords[0] as string;

                if (IgnoreAllCollection.Contains(misspelledWord))
                {
                    SpellChecker.MisspelledWords.RemoveAt(0);
                    ProcessMisspelledWords();
                }
                else if (ChangeAllCollection.Contains(misspelledWord))
                {
                    int startIndex = this.editor.Text.IndexOf(misspelledWord);
                    this.Editor.SelectText(startIndex, misspelledWord.Length);
                    this.Editor.CurrentWord = ChangeAllCollection[misspelledWord] as string;
                    SpellChecker.MisspelledWords.RemoveAt(0);
                    ProcessMisspelledWords();
                }
                else
                {
                    int searchIndex = 0;
                    if (IgnoreOnceCollection.Contains(misspelledWord))
                        searchIndex = (int)IgnoreOnceCollection[misspelledWord];

                    int startIndex = this.editor.Text.IndexOf(misspelledWord, searchIndex);

                    this.LBox_Suggestions.DataSource = this.SpellChecker.Suggest(misspelledWord);

                    UpdateAndHiglightTextInRichTextBox(startIndex, misspelledWord.Length);
                }

                UpdateSpellCheckerButtons();
            }
            else
            {
                HideSpellCheckerDialog();
            }
        }

        private void ClearHash()
        {
            this.ChangeAllCollection.Clear();
            this.IgnoreAllCollection.Clear();
            this.IgnoreOnceCollection.Clear();

            this.LBox_Suggestions.DataSource = null;
        }

        private void HideSpellCheckerDialog()
        {
            ClearHash();

            MessageBoxAdv.Show(this, SR.GetString(ResourceIdentifiers.SpellCheckCompletedAlert), SR.GetString(ResourceIdentifiers.SpellCheckerDialogCaption), MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (this.Owner != null)
                this.Owner.Activate();

            this.Hide();

        }

        private void UpdateAndHiglightTextInRichTextBox(int startIndex,int length)
        {
            this.IgnoreTextChange = true;
            this.RTxt_Editor.Clear();
            this.RTxt_Editor.Text = this.Editor.Text;
            
            this.RTxt_Editor.Select(startIndex, length);
            this.RTxt_Editor.ScrollToCaret();
            this.Editor.SelectText(startIndex, length);
            
            this.RTxt_Editor.SelectionColor = Color.Red;
            this.RTxt_Editor.SelectionFont = new Font(this.Font.FontFamily, 8, FontStyle.Bold);
            this.IgnoreTextChange = false;
        }

        #endregion

        #region Event Handlers

        private void Btn_Undo_Click(object sender, EventArgs e)
        {
            this.RTxt_Editor.Text = this.Editor.Text;
        }

        private void Btn_Options_Click(object sender, EventArgs e)
        {
            this.SpellChecker.ShowOptionsDialog(this);
        }

        private void Btn_CustomDictionary_Click(object sender, EventArgs e)
        {
            this.spellChecker.ShowCustomDictionaryEditor(this);
        }

        private void Btn_Change_Click(object sender, EventArgs e)
        {
            this.Editor.CurrentWord = this.LBox_Suggestions.SelectedItem.ToString();
            
            this.RTxt_Editor.Text = this.Editor.Text;
            
            SpellChecker.MisspelledWords.RemoveAt(0);

            ProcessMisspelledWords();
        }

        private void Btn_IgnoreOnce_Click(object sender, EventArgs e)
        {
            if (TextUpdatePending)
            {
                this.Editor.Text = this.RTxt_Editor.Text;
                ClearHash();
                this.SpellChecker.SpellCheck(this.Editor.Text);
            }
            else
            {
                if (!this.IgnoreOnceCollection.Contains(Editor.CurrentWord))
                {
                    //Stores the index of the current misspelled word end.
                    this.IgnoreOnceCollection[Editor.CurrentWord] = Editor.Text.IndexOf(Editor.CurrentWord) + Editor.CurrentWord.Length;
                }
                else
                {
                    int PrevSearchIndex = (int)this.IgnoreOnceCollection[Editor.CurrentWord];

                    int newSearchIndex = this.Editor.Text.IndexOf(Editor.CurrentWord, PrevSearchIndex);

                    this.IgnoreOnceCollection[Editor.CurrentWord] = newSearchIndex + Editor.CurrentWord.Length;
                }

                SpellChecker.MisspelledWords.RemoveAt(0);
            }
            ProcessMisspelledWords();
        }

        private void Btn_IgnoreAll_Click(object sender, EventArgs e)
        {
            if(!this.IgnoreAllCollection.Contains(this.Editor.CurrentWord))
                this.IgnoreAllCollection[this.Editor.CurrentWord] = this.Editor.CurrentWord;

            this.SpellChecker.MisspelledWords.RemoveAt(0);

            ProcessMisspelledWords();
        }

        private void Btn_AddToDicktionary_Click(object sender, EventArgs e)
        {
            SpellChecker.WriteToDictionary(SpellChecker.CustomDictionaryPath,this.Editor.CurrentWord.ToLower());

            if (!this.IgnoreAllCollection.Contains(this.Editor.CurrentWord))
                this.IgnoreAllCollection[this.Editor.CurrentWord] = this.Editor.CurrentWord;

            this.SpellChecker.MisspelledWords.RemoveAt(0);

            ProcessMisspelledWords();
        }

        private void Btn_ChangeAll_Click(object sender, EventArgs e)
        {
            string key = this.Editor.CurrentWord;
            string value = this.LBox_Suggestions.SelectedItem.ToString();

            if (!(String.IsNullOrEmpty(key) && String.IsNullOrEmpty(value)
                && this.ChangeAllCollection.Contains(key)))
                this.ChangeAllCollection[this.Editor.CurrentWord] = value;

            this.Editor.CurrentWord = this.LBox_Suggestions.SelectedItem.ToString();

            this.RTxt_Editor.Text = this.Editor.Text;

            SpellChecker.MisspelledWords.RemoveAt(0);

            ProcessMisspelledWords();
        }

        private void RTxt_Editor_TextChanged(object sender, EventArgs e)
        {
            if(!IgnoreTextChange)
                UpdateSpellCheckerButtons();
        }

        #endregion

        #region Designer Variables
        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.RichTextBox RTxt_Editor;
        private System.Windows.Forms.Label Lbl_NotInDictionary;
        private System.Windows.Forms.ListBox LBox_Suggestions;
        private System.Windows.Forms.Label Lbl_Suggestions;
        private System.Windows.Forms.Button Btn_Options;
        private System.Windows.Forms.Button Btn_Undo;
        private System.Windows.Forms.Button Btn_IgnoreOnce;
        private System.Windows.Forms.Button Btn_IgnoreAll;
        private System.Windows.Forms.Button Btn_Change;
        private System.Windows.Forms.Button Btn_AddToDicktionary;
        private System.Windows.Forms.Button Btn_Cancel;
        private System.Windows.Forms.Button Btn_ChangeAll;
        private System.Windows.Forms.Button Btn_CustomDictionary;
        
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
            this.RTxt_Editor = new System.Windows.Forms.RichTextBox();
            this.Lbl_NotInDictionary = new System.Windows.Forms.Label();
            this.LBox_Suggestions = new System.Windows.Forms.ListBox();
            this.Lbl_Suggestions = new System.Windows.Forms.Label();
            this.Btn_Options = new System.Windows.Forms.Button();
            this.Btn_Undo = new System.Windows.Forms.Button();
            this.Btn_IgnoreOnce = new System.Windows.Forms.Button();
            this.Btn_IgnoreAll = new System.Windows.Forms.Button();
            this.Btn_Change = new System.Windows.Forms.Button();
            this.Btn_AddToDicktionary = new System.Windows.Forms.Button();
            this.Btn_Cancel = new System.Windows.Forms.Button();
            this.Btn_ChangeAll = new System.Windows.Forms.Button();
            this.Btn_CustomDictionary = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RTxt_Editor
            // 
            this.RTxt_Editor.Location = new System.Drawing.Point(6, 25);
            this.RTxt_Editor.Name = "RTxt_Editor";
            this.RTxt_Editor.Size = new System.Drawing.Size(340, 82);
            this.RTxt_Editor.TabIndex = 1;
            this.RTxt_Editor.Text = "";
            this.RTxt_Editor.TextChanged += new System.EventHandler(this.RTxt_Editor_TextChanged);
            // 
            // Lbl_NotInDictionary
            // 
            this.Lbl_NotInDictionary.AutoSize = true;
            this.Lbl_NotInDictionary.Location = new System.Drawing.Point(3, 9);
            this.Lbl_NotInDictionary.Name = "Lbl_NotInDictionary";
            this.Lbl_NotInDictionary.Size = new System.Drawing.Size(86, 13);
            this.Lbl_NotInDictionary.TabIndex = 0;
            this.Lbl_NotInDictionary.Text = SR.GetString(ResourceIdentifiers.SpellCheckerLabelNotInDictionary);
            // 
            // LBox_Suggestions
            // 
            this.LBox_Suggestions.FormattingEnabled = true;
            this.LBox_Suggestions.Location = new System.Drawing.Point(6, 125);
            this.LBox_Suggestions.Name = "LBox_Suggestions";
            this.LBox_Suggestions.Size = new System.Drawing.Size(340, 82);
            this.LBox_Suggestions.TabIndex = 6;
            // 
            // Lbl_Suggestions
            // 
            this.Lbl_Suggestions.AutoSize = true;
            this.Lbl_Suggestions.Location = new System.Drawing.Point(3, 108);
            this.Lbl_Suggestions.Name = "Lbl_Suggestions";
            this.Lbl_Suggestions.Size = new System.Drawing.Size(71, 13);
            this.Lbl_Suggestions.TabIndex = 5;
            this.Lbl_Suggestions.Text = SR.GetString(ResourceIdentifiers.SpellCheckerLabelSuggestions);
            // 
            // Btn_Options
            // 
            this.Btn_Options.Location = new System.Drawing.Point(7, 221);
            this.Btn_Options.Name = "Btn_Options";
            this.Btn_Options.Size = new System.Drawing.Size(75, 23);
            this.Btn_Options.TabIndex = 9;
            this.Btn_Options.Text = SR.GetString(ResourceIdentifiers.Options);
            this.Btn_Options.UseVisualStyleBackColor = true;
            this.Btn_Options.Click += new System.EventHandler(this.Btn_Options_Click);
            // 
            // Btn_Undo
            // 
            this.Btn_Undo.Location = new System.Drawing.Point(91, 221);
            this.Btn_Undo.Name = "Btn_Undo";
            this.Btn_Undo.Size = new System.Drawing.Size(75, 23);
            this.Btn_Undo.TabIndex = 10;
            this.Btn_Undo.Text = SR.GetString(ResourceIdentifiers.Undo);
            this.Btn_Undo.UseVisualStyleBackColor = true;
            this.Btn_Undo.Click += new System.EventHandler(this.Btn_Undo_Click);
            // 
            // Btn_IgnoreOnce
            // 
            this.Btn_IgnoreOnce.Location = new System.Drawing.Point(352, 25);
            this.Btn_IgnoreOnce.Name = "Btn_IgnoreOnce";
            this.Btn_IgnoreOnce.Size = new System.Drawing.Size(102, 23);
            this.Btn_IgnoreOnce.TabIndex = 2;
            this.Btn_IgnoreOnce.Text = SR.GetString(ResourceIdentifiers.IgnoreOnce);
            this.Btn_IgnoreOnce.UseVisualStyleBackColor = true;
            this.Btn_IgnoreOnce.Click += new System.EventHandler(this.Btn_IgnoreOnce_Click);
            // 
            // Btn_IgnoreAll
            // 
            this.Btn_IgnoreAll.Location = new System.Drawing.Point(352, 55);
            this.Btn_IgnoreAll.Name = "Btn_IgnoreAll";
            this.Btn_IgnoreAll.Size = new System.Drawing.Size(102, 23);
            this.Btn_IgnoreAll.TabIndex = 3;
            this.Btn_IgnoreAll.Text = SR.GetString(ResourceIdentifiers.IgnoreAll);
            this.Btn_IgnoreAll.UseVisualStyleBackColor = true;
            this.Btn_IgnoreAll.Click += new System.EventHandler(this.Btn_IgnoreAll_Click);
            // 
            // Btn_Change
            // 
            this.Btn_Change.Location = new System.Drawing.Point(352, 124);
            this.Btn_Change.Name = "Btn_Change";
            this.Btn_Change.Size = new System.Drawing.Size(102, 23);
            this.Btn_Change.TabIndex = 7;
            this.Btn_Change.Text = SR.GetString(ResourceIdentifiers.Change);
            this.Btn_Change.UseVisualStyleBackColor = true;
            this.Btn_Change.Click += new System.EventHandler(this.Btn_Change_Click);
            // 
            // Btn_AddToDicktionary
            // 
            this.Btn_AddToDicktionary.Location = new System.Drawing.Point(352, 84);
            this.Btn_AddToDicktionary.Name = "Btn_AddToDicktionary";
            this.Btn_AddToDicktionary.Size = new System.Drawing.Size(102, 23);
            this.Btn_AddToDicktionary.TabIndex = 4;
            this.Btn_AddToDicktionary.Text = SR.GetString(ResourceIdentifiers.SpellCheckerButtonAddToDictionary);
            this.Btn_AddToDicktionary.UseVisualStyleBackColor = true;
            this.Btn_AddToDicktionary.Click += new System.EventHandler(this.Btn_AddToDicktionary_Click);
            // 
            // Btn_Cancel
            // 
            this.Btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Btn_Cancel.Location = new System.Drawing.Point(351, 221);
            this.Btn_Cancel.Name = "Btn_Cancel";
            this.Btn_Cancel.Size = new System.Drawing.Size(103, 23);
            this.Btn_Cancel.TabIndex = 12;
            this.Btn_Cancel.Text = SR.GetString(ResourceIdentifiers.Cancel);
            this.Btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // Btn_ChangeAll
            // 
            this.Btn_ChangeAll.Location = new System.Drawing.Point(352, 153);
            this.Btn_ChangeAll.Name = "Btn_ChangeAll";
            this.Btn_ChangeAll.Size = new System.Drawing.Size(102, 23);
            this.Btn_ChangeAll.TabIndex = 8;
            this.Btn_ChangeAll.Text = SR.GetString(ResourceIdentifiers.ChangeAll);
            this.Btn_ChangeAll.UseVisualStyleBackColor = true;
            this.Btn_ChangeAll.Click += new System.EventHandler(this.Btn_ChangeAll_Click);
            // 
            // Btn_CustomDictionary
            // 
            this.Btn_CustomDictionary.Location = new System.Drawing.Point(173, 221);
            this.Btn_CustomDictionary.Name = "Btn_CustomDictionary";
            this.Btn_CustomDictionary.Size = new System.Drawing.Size(105, 23);
            this.Btn_CustomDictionary.TabIndex = 11;
            this.Btn_CustomDictionary.Text = SR.GetString(ResourceIdentifiers.SpellCheckerButtonCustomDictionary,this);
            this.Btn_CustomDictionary.UseVisualStyleBackColor = true;
            this.Btn_CustomDictionary.Click += new System.EventHandler(this.Btn_CustomDictionary_Click);
            // 
            // SpellCheckerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.Btn_Cancel;
            this.ClientSize = new System.Drawing.Size(458, 253);
            this.Controls.Add(this.Btn_CustomDictionary);
            this.Controls.Add(this.Btn_Cancel);
            this.Controls.Add(this.Btn_ChangeAll);
            this.Controls.Add(this.Btn_Change);
            this.Controls.Add(this.Btn_AddToDicktionary);
            this.Controls.Add(this.Btn_IgnoreAll);
            this.Controls.Add(this.Btn_IgnoreOnce);
            this.Controls.Add(this.Btn_Undo);
            this.Controls.Add(this.Btn_Options);
            this.Controls.Add(this.Lbl_Suggestions);
            this.Controls.Add(this.LBox_Suggestions);
            this.Controls.Add(this.Lbl_NotInDictionary);
            this.Controls.Add(this.RTxt_Editor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SpellCheckerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = SR.GetString(ResourceIdentifiers.SpellCheckerDialogCaption);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

    }
}
