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
    internal class CustomDictionaryEditor : Form
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

        public CustomDictionaryEditor()
        {
            InitializeComponent();
        }

        #endregion

        #region Helper Methods
        public void ShowOptionsDialog(SpellChecker spellChecker, Form owner)
        {
            this.SpellChecker = spellChecker;

            LoadCustomDictionary(SpellChecker.CustomDictionaryPath);

            UpdateButtons();

            this.ShowDialog(owner);
        }

        private void HideDialog()
        {
            this.Owner.Activate();
            this.Hide();
            this.txt_Word.Clear();
        }

        private void UpdateButtons()
        {
            this.Btn_Add.Enabled = (this.txt_Word.Text != string.Empty) && !this.listBox_Words.Items.Contains(this.txt_Word.Text);
            this.Btn_DeleteAll.Enabled = this.Btn_Delete.Enabled = this.listBox_Words.Items.Count > 0;
        }

        private void LoadCustomDictionary()
        {
            this.LoadCustomDictionary(SpellChecker.CustomDictionaryPath);
        }

        private void LoadCustomDictionary(string customDictionaryPath)
        {
            ArrayList wordList = this.SpellChecker.GetDictList(customDictionaryPath);
            wordList.Sort();
            this.listBox_Words.DataSource = wordList;
            this.txt_Path.Text = customDictionaryPath;
        }

        #endregion

        #region Event Handlers

        private void Btn_New_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Dictionary Files(*.dic)| *.dic|Text Files (*.txt)|*.txt";
            dialog.FilterIndex = 1;
            dialog.DefaultExt = "dic";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SpellChecker.CustomDictionaryPath = dialog.FileName;
                LoadCustomDictionary(SpellChecker.CustomDictionaryPath);
            }
        }

        private void txt_Word_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            this.SpellChecker.WriteToDictionary(SpellChecker.CustomDictionaryPath,this.txt_Word.Text);
            this.txt_Word.Clear();
            LoadCustomDictionary();
            UpdateButtons();
        }

        private void Btn_Ok_Click(object sender, EventArgs e)
        {
            HideDialog();
        }

        private void Btn_DeleteAll_Click(object sender, EventArgs e)
        {
            this.SpellChecker.DeleteAllFromDictionary(SpellChecker.CustomDictionaryPath);
            LoadCustomDictionary();
            UpdateButtons();
        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            ArrayList wordList = this.listBox_Words.DataSource as ArrayList;
            if (wordList != null && wordList.Contains(this.listBox_Words.SelectedItem))
            {
                wordList.Remove(this.listBox_Words.SelectedItem);
                wordList.Sort();

                this.SpellChecker.WriteToDictionary(SpellChecker.CustomDictionaryPath, wordList);
                LoadCustomDictionary();
                UpdateButtons();
            }
        }

        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbl_Word = new System.Windows.Forms.Label();
            this.txt_Word = new System.Windows.Forms.TextBox();
            this.listBox_Words = new System.Windows.Forms.ListBox();
            this.lbl_Dictionary = new System.Windows.Forms.Label();
            this.Btn_Add = new System.Windows.Forms.Button();
            this.Btn_Delete = new System.Windows.Forms.Button();
            this.Btn_DeleteAll = new System.Windows.Forms.Button();
            this.Btn_Cancel = new System.Windows.Forms.Button();
            this.Btn_Ok = new System.Windows.Forms.Button();
            this.txt_Path = new System.Windows.Forms.TextBox();
            this.Btn_New = new System.Windows.Forms.Button();
            this.lbl_Path = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbl_Word
            // 
            this.lbl_Word.AutoSize = true;
            this.lbl_Word.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Word.Location = new System.Drawing.Point(8, 51);
            this.lbl_Word.Name = "lbl_Word";
            this.lbl_Word.Size = new System.Drawing.Size(47, 13);
            this.lbl_Word.TabIndex = 3;
            this.lbl_Word.Text = SR.GetString(ResourceIdentifiers.SpellCheckerLabelWords);
            // 
            // txt_Word
            // 
            this.txt_Word.Location = new System.Drawing.Point(11, 70);
            this.txt_Word.Name = "txt_Word";
            this.txt_Word.Size = new System.Drawing.Size(329, 20);
            this.txt_Word.TabIndex = 4;
            this.txt_Word.TextChanged += new System.EventHandler(this.txt_Word_TextChanged);
            // 
            // listBox_Words
            // 
            this.listBox_Words.FormattingEnabled = true;
            this.listBox_Words.Location = new System.Drawing.Point(11, 109);
            this.listBox_Words.Name = "listBox_Words";
            this.listBox_Words.Size = new System.Drawing.Size(329, 95);
            this.listBox_Words.TabIndex = 6;
            // 
            // lbl_Dictionary
            // 
            this.lbl_Dictionary.AutoSize = true;
            this.lbl_Dictionary.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Dictionary.Location = new System.Drawing.Point(8, 91);
            this.lbl_Dictionary.Name = "lbl_Dictionary";
            this.lbl_Dictionary.Size = new System.Drawing.Size(68, 13);
            this.lbl_Dictionary.TabIndex = 5;
            this.lbl_Dictionary.Text = SR.GetString(ResourceIdentifiers.SpellCheckerLabelDictionary);
            // 
            // Btn_Add
            // 
            this.Btn_Add.Location = new System.Drawing.Point(102, 210);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(75, 23);
            this.Btn_Add.TabIndex = 7;
            this.Btn_Add.Text = SR.GetString(ResourceIdentifiers.Add);
            this.Btn_Add.UseVisualStyleBackColor = true;
            this.Btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // Btn_Delete
            // 
            this.Btn_Delete.Location = new System.Drawing.Point(184, 210);
            this.Btn_Delete.Name = "Btn_Delete";
            this.Btn_Delete.Size = new System.Drawing.Size(75, 23);
            this.Btn_Delete.TabIndex = 8;
            this.Btn_Delete.Text = SR.GetString(ResourceIdentifiers.Delete);
            this.Btn_Delete.UseVisualStyleBackColor = true;
            this.Btn_Delete.Click += new System.EventHandler(this.Btn_Delete_Click);
            // 
            // Btn_DeleteAll
            // 
            this.Btn_DeleteAll.Location = new System.Drawing.Point(266, 210);
            this.Btn_DeleteAll.Name = "Btn_DeleteAll";
            this.Btn_DeleteAll.Size = new System.Drawing.Size(75, 23);
            this.Btn_DeleteAll.TabIndex = 9;
            this.Btn_DeleteAll.Text = SR.GetString(ResourceIdentifiers.DeleteAll);
            this.Btn_DeleteAll.UseVisualStyleBackColor = true;
            this.Btn_DeleteAll.Click += new System.EventHandler(this.Btn_DeleteAll_Click);
            // 
            // Btn_Cancel
            // 
            this.Btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Btn_Cancel.Location = new System.Drawing.Point(266, 244);
            this.Btn_Cancel.Name = "Btn_Cancel";
            this.Btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.Btn_Cancel.TabIndex = 11;
            this.Btn_Cancel.Text = SR.GetString(ResourceIdentifiers.Cancel);
            this.Btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // Btn_Ok
            // 
            this.Btn_Ok.Location = new System.Drawing.Point(183, 244);
            this.Btn_Ok.Name = "Btn_Ok";
            this.Btn_Ok.Size = new System.Drawing.Size(75, 23);
            this.Btn_Ok.TabIndex = 10;
            this.Btn_Ok.Text = SR.GetString(ResourceIdentifiers.OK);
            this.Btn_Ok.UseVisualStyleBackColor = true;
            this.Btn_Ok.Click += new System.EventHandler(this.Btn_Ok_Click);
            // 
            // txt_Path
            // 
            this.txt_Path.Location = new System.Drawing.Point(11, 29);
            this.txt_Path.Name = "txt_Path";
            this.txt_Path.ReadOnly = true;
            this.txt_Path.Size = new System.Drawing.Size(251, 20);
            this.txt_Path.TabIndex = 1;
            // 
            // Btn_New
            // 
            this.Btn_New.Location = new System.Drawing.Point(266, 27);
            this.Btn_New.Name = "Btn_New";
            this.Btn_New.Size = new System.Drawing.Size(75, 23);
            this.Btn_New.TabIndex = 2;
            this.Btn_New.Text = SR.GetString(ResourceIdentifiers.SpellCheckerButtonNew);
            this.Btn_New.UseVisualStyleBackColor = true;
            this.Btn_New.Click += new System.EventHandler(this.Btn_New_Click);
            // 
            // lbl_Path
            // 
            this.lbl_Path.AutoSize = true;
            this.lbl_Path.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Path.Location = new System.Drawing.Point(8, 9);
            this.lbl_Path.Name = "lbl_Path";
            this.lbl_Path.Size = new System.Drawing.Size(139, 13);
            this.lbl_Path.TabIndex = 0;
            this.lbl_Path.Text = SR.GetString(ResourceIdentifiers.SpellCheckerLabelDictionaryPath);
            // 
            // CustomDictionaryEditor
            // 
            this.AcceptButton = this.Btn_Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Btn_Cancel;
            this.ClientSize = new System.Drawing.Size(349, 277);
            this.Controls.Add(this.lbl_Path);
            this.Controls.Add(this.Btn_New);
            this.Controls.Add(this.txt_Path);
            this.Controls.Add(this.Btn_Cancel);
            this.Controls.Add(this.Btn_Ok);
            this.Controls.Add(this.Btn_DeleteAll);
            this.Controls.Add(this.Btn_Delete);
            this.Controls.Add(this.Btn_Add);
            this.Controls.Add(this.lbl_Dictionary);
            this.Controls.Add(this.listBox_Words);
            this.Controls.Add(this.txt_Word);
            this.Controls.Add(this.lbl_Word);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomDictionaryEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = SR.GetString(ResourceIdentifiers.SpellCheckerDictioanryEditorCaption);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

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

        #region Designer Variables
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lbl_Path;
        private System.Windows.Forms.Label lbl_Dictionary;
        private System.Windows.Forms.Label lbl_Word;
        private System.Windows.Forms.ListBox listBox_Words;
        private System.Windows.Forms.TextBox txt_Path;
        private System.Windows.Forms.TextBox txt_Word;
        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.Button Btn_Delete;
        private System.Windows.Forms.Button Btn_DeleteAll;
        private System.Windows.Forms.Button Btn_Cancel;
        private System.Windows.Forms.Button Btn_New;
        private System.Windows.Forms.Button Btn_Ok;
        #endregion
    }
}
