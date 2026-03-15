//-------------------------------------------------------------------------------------------------
// <copyright file="GridFormatCellDialog.Designer.cs" company="Syncfusion">
// Copyright (c) Syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Defines excel-like Format Cells Dialog that allows the user to format the cells dynamically.
    /// There are options to customized cell font, font color, font size, font style, font effects,
    /// background, alignment, cell merge, text formats.
    /// </summary>
    partial class GridFormatCellDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.colorPickerButton1 = new Syncfusion.Windows.Forms.ColorPickerButton();
            this.chkStrikeout = new System.Windows.Forms.CheckBox();
            this.chkUnderline = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.lstFontSize = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFSize = new System.Windows.Forms.TextBox();
            this.lstFontType = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFType = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFFace = new System.Windows.Forms.TextBox();
            this.lstFontFace = new System.Windows.Forms.ListBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.lblIfText = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.lstbxType = new System.Windows.Forms.ListBox();
            this.lstbxFormat = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.clrPanel = new Syncfusion.Windows.Forms.Tools.GradientPanel();
            this.cmbGradient = new System.Windows.Forms.ComboBox();
            this.lblGradient = new System.Windows.Forms.Label();
            this.cmbPattern = new System.Windows.Forms.ComboBox();
            this.lblPattern = new System.Windows.Forms.Label();
            this.clrFore = new Syncfusion.Windows.Forms.ColorPickerButton();
            this.clrBack = new Syncfusion.Windows.Forms.ColorPickerButton();
            this.cmbBgStyle = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.chkRightToLeft = new System.Windows.Forms.CheckBox();
            this.chkMerge = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.upDownIndent = new System.Windows.Forms.NumericUpDown();
            this.upDownOrient = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.chkWrapText = new System.Windows.Forms.CheckBox();
            this.cmbVrtcl = new System.Windows.Forms.ComboBox();
            this.cmbHzntl = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clrPanel)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownIndent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownOrient)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(483, 258);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.colorPickerButton1);
            this.tabPage1.Controls.Add(this.chkStrikeout);
            this.tabPage1.Controls.Add(this.chkUnderline);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.lstFontSize);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.txtFSize);
            this.tabPage1.Controls.Add(this.lstFontType);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.txtFType);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.txtFFace);
            this.tabPage1.Controls.Add(this.lstFontFace);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(475, 232);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Font";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(231, 203);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Color:";
            // 
            // colorPickerButton1
            // 
            this.colorPickerButton1.ColorUISize = new System.Drawing.Size(208, 230);
            this.colorPickerButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colorPickerButton1.Location = new System.Drawing.Point(271, 200);
            this.colorPickerButton1.Name = "colorPickerButton1";
            this.colorPickerButton1.SelectedAsBackcolor = true;
            this.colorPickerButton1.SelectedColorGroup = Syncfusion.Windows.Forms.ColorUISelectedGroup.None;
            this.colorPickerButton1.Size = new System.Drawing.Size(120, 19);
            this.colorPickerButton1.TabIndex = 15;
            this.colorPickerButton1.UseVisualStyleBackColor = false;
            // 
            // chkStrikeout
            // 
            this.chkStrikeout.AutoSize = true;
            this.chkStrikeout.Location = new System.Drawing.Point(124, 202);
            this.chkStrikeout.Name = "chkStrikeout";
            this.chkStrikeout.Size = new System.Drawing.Size(68, 17);
            this.chkStrikeout.TabIndex = 14;
            this.chkStrikeout.Text = "Strikeout";
            this.chkStrikeout.UseVisualStyleBackColor = true;
            // 
            // chkUnderline
            // 
            this.chkUnderline.AutoSize = true;
            this.chkUnderline.Location = new System.Drawing.Point(18, 202);
            this.chkUnderline.Name = "chkUnderline";
            this.chkUnderline.Size = new System.Drawing.Size(71, 17);
            this.chkUnderline.TabIndex = 13;
            this.chkUnderline.Text = "Underline";
            this.chkUnderline.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Silver;
            this.panel2.Location = new System.Drawing.Point(87, 181);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(371, 1);
            this.panel2.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 175);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Font Effects";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkGray;
            this.panel1.Location = new System.Drawing.Point(47, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(409, 1);
            this.panel1.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Font";
            // 
            // lstFontSize
            // 
            this.lstFontSize.FormattingEnabled = true;
            this.lstFontSize.Items.AddRange(new object[] {
            "6",
            "7",
            "8",
            "9",
            "10",
            "10.5",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "18",
            "20",
            "22",
            "24",
            "26",
            "28",
            "32",
            "36",
            "40",
            "44",
            "48",
            "54",
            "60",
            "72",
            "80",
            "88",
            "96"});
            this.lstFontSize.Location = new System.Drawing.Point(333, 65);
            this.lstFontSize.Name = "lstFontSize";
            this.lstFontSize.Size = new System.Drawing.Size(120, 95);
            this.lstFontSize.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(330, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Size";
            // 
            // txtFSize
            // 
            this.txtFSize.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.txtFSize.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtFSize.Location = new System.Drawing.Point(333, 45);
            this.txtFSize.Name = "txtFSize";
            this.txtFSize.Size = new System.Drawing.Size(120, 20);
            this.txtFSize.TabIndex = 6;
            // 
            // lstFontType
            // 
            this.lstFontType.FormattingEnabled = true;
            this.lstFontType.Items.AddRange(new object[] {
            "Regular",
            "Bold",
            "Italic",
            "Bold Italic"});
            this.lstFontType.Location = new System.Drawing.Point(191, 65);
            this.lstFontType.Name = "lstFontType";
            this.lstFontType.Size = new System.Drawing.Size(123, 95);
            this.lstFontType.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(187, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Style";
            // 
            // txtFType
            // 
            this.txtFType.AutoCompleteCustomSource.AddRange(new string[] {
            "Regular",
            "Bold",
            "Italic",
            "Bold Italic"});
            this.txtFType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.txtFType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtFType.Location = new System.Drawing.Point(191, 45);
            this.txtFType.Name = "txtFType";
            this.txtFType.Size = new System.Drawing.Size(123, 20);
            this.txtFType.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Face";
            // 
            // txtFFace
            // 
            this.txtFFace.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.txtFFace.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtFFace.Location = new System.Drawing.Point(17, 45);
            this.txtFFace.Name = "txtFFace";
            this.txtFFace.Size = new System.Drawing.Size(154, 20);
            this.txtFFace.TabIndex = 1;
            // 
            // lstFontFace
            // 
            this.lstFontFace.FormattingEnabled = true;
            this.lstFontFace.Location = new System.Drawing.Point(17, 65);
            this.lstFontFace.Name = "lstFontFace";
            this.lstFontFace.Size = new System.Drawing.Size(154, 95);
            this.lstFontFace.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.lblIfText);
            this.tabPage5.Controls.Add(this.label26);
            this.tabPage5.Controls.Add(this.label25);
            this.tabPage5.Controls.Add(this.lstbxType);
            this.tabPage5.Controls.Add(this.lstbxFormat);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(475, 232);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Number";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // lblIfText
            // 
            this.lblIfText.AutoSize = true;
            this.lblIfText.Location = new System.Drawing.Point(205, 23);
            this.lblIfText.Name = "lblIfText";
            this.lblIfText.Size = new System.Drawing.Size(0, 13);
            this.lblIfText.TabIndex = 5;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(17, 23);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(47, 13);
            this.label26.TabIndex = 4;
            this.label26.Text = "Formats:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(164, 23);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(34, 13);
            this.label25.TabIndex = 2;
            this.label25.Text = "Type:";
            // 
            // lstbxType
            // 
            this.lstbxType.FormattingEnabled = true;
            this.lstbxType.Location = new System.Drawing.Point(167, 39);
            this.lstbxType.Name = "lstbxType";
            this.lstbxType.Size = new System.Drawing.Size(121, 95);
            this.lstbxType.TabIndex = 1;
            // 
            // lstbxFormat
            // 
            this.lstbxFormat.FormattingEnabled = true;
            this.lstbxFormat.Items.AddRange(new object[] {
            "Number",
            "Currency",
            "Percentage",
            "Scientific",
            "Date",
            "Time",
            "Text"});
            this.lstbxFormat.Location = new System.Drawing.Point(20, 40);
            this.lstbxFormat.Name = "lstbxFormat";
            this.lstbxFormat.Size = new System.Drawing.Size(115, 160);
            this.lstbxFormat.TabIndex = 0;
            this.lstbxFormat.SelectedIndexChanged += new System.EventHandler(this.lstbxFormat_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.clrPanel);
            this.tabPage2.Controls.Add(this.cmbGradient);
            this.tabPage2.Controls.Add(this.lblGradient);
            this.tabPage2.Controls.Add(this.cmbPattern);
            this.tabPage2.Controls.Add(this.lblPattern);
            this.tabPage2.Controls.Add(this.clrFore);
            this.tabPage2.Controls.Add(this.clrBack);
            this.tabPage2.Controls.Add(this.cmbBgStyle);
            this.tabPage2.Controls.Add(this.label24);
            this.tabPage2.Controls.Add(this.label23);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(475, 232);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Background";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // clrPanel
            // 
            this.clrPanel.BorderColor = System.Drawing.Color.DarkGray;
            this.clrPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.clrPanel.Location = new System.Drawing.Point(248, 29);
            this.clrPanel.Name = "clrPanel";
            this.clrPanel.Size = new System.Drawing.Size(195, 80);
            this.clrPanel.TabIndex = 12;
            // 
            // cmbGradient
            // 
            this.cmbGradient.Enabled = false;
            this.cmbGradient.FormattingEnabled = true;
            this.cmbGradient.Location = new System.Drawing.Point(100, 66);
            this.cmbGradient.Name = "cmbGradient";
            this.cmbGradient.Size = new System.Drawing.Size(121, 21);
            this.cmbGradient.TabIndex = 11;
            // 
            // lblGradient
            // 
            this.lblGradient.AutoSize = true;
            this.lblGradient.Enabled = false;
            this.lblGradient.Location = new System.Drawing.Point(7, 66);
            this.lblGradient.Name = "lblGradient";
            this.lblGradient.Size = new System.Drawing.Size(73, 13);
            this.lblGradient.TabIndex = 10;
            this.lblGradient.Text = "GradientStyle:";
            // 
            // cmbPattern
            // 
            this.cmbPattern.Enabled = false;
            this.cmbPattern.FormattingEnabled = true;
            this.cmbPattern.Location = new System.Drawing.Point(100, 187);
            this.cmbPattern.Name = "cmbPattern";
            this.cmbPattern.Size = new System.Drawing.Size(121, 21);
            this.cmbPattern.TabIndex = 9;
            // 
            // lblPattern
            // 
            this.lblPattern.AutoSize = true;
            this.lblPattern.Enabled = false;
            this.lblPattern.Location = new System.Drawing.Point(8, 190);
            this.lblPattern.Name = "lblPattern";
            this.lblPattern.Size = new System.Drawing.Size(67, 13);
            this.lblPattern.TabIndex = 8;
            this.lblPattern.Text = "PatternStyle:";
            // 
            // clrFore
            // 
            this.clrFore.ColorUISize = new System.Drawing.Size(208, 230);
            this.clrFore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clrFore.Location = new System.Drawing.Point(100, 146);
            this.clrFore.Name = "clrFore";
            this.clrFore.SelectedAsBackcolor = true;
            this.clrFore.SelectedColorGroup = Syncfusion.Windows.Forms.ColorUISelectedGroup.None;
            this.clrFore.Size = new System.Drawing.Size(121, 23);
            this.clrFore.TabIndex = 7;
            this.clrFore.UseVisualStyleBackColor = true;
            // 
            // clrBack
            // 
            this.clrBack.ColorUISize = new System.Drawing.Size(208, 230);
            this.clrBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clrBack.Location = new System.Drawing.Point(100, 106);
            this.clrBack.Name = "clrBack";
            this.clrBack.SelectedAsBackcolor = true;
            this.clrBack.SelectedColorGroup = Syncfusion.Windows.Forms.ColorUISelectedGroup.None;
            this.clrBack.Size = new System.Drawing.Size(121, 23);
            this.clrBack.TabIndex = 6;
            this.clrBack.UseVisualStyleBackColor = true;
            // 
            // cmbBgStyle
            // 
            this.cmbBgStyle.FormattingEnabled = true;
            this.cmbBgStyle.Items.AddRange(new object[] {
            "Solid",
            "Gradient",
            "Pattern"});
            this.cmbBgStyle.Location = new System.Drawing.Point(100, 26);
            this.cmbBgStyle.Name = "cmbBgStyle";
            this.cmbBgStyle.Size = new System.Drawing.Size(121, 21);
            this.cmbBgStyle.TabIndex = 5;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(10, 151);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(54, 13);
            this.label24.TabIndex = 4;
            this.label24.Text = "Forecolor:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(10, 111);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(58, 13);
            this.label23.TabIndex = 3;
            this.label23.Text = "Backcolor:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(8, 29);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(33, 13);
            this.label16.TabIndex = 2;
            this.label16.Text = "Style:";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chkRightToLeft);
            this.tabPage3.Controls.Add(this.chkMerge);
            this.tabPage3.Controls.Add(this.label18);
            this.tabPage3.Controls.Add(this.upDownIndent);
            this.tabPage3.Controls.Add(this.upDownOrient);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Controls.Add(this.chkWrapText);
            this.tabPage3.Controls.Add(this.cmbVrtcl);
            this.tabPage3.Controls.Add(this.cmbHzntl);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.panel4);
            this.tabPage3.Controls.Add(this.panel3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(475, 232);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Alignment";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // chkRightToLeft
            // 
            this.chkRightToLeft.AutoSize = true;
            this.chkRightToLeft.Location = new System.Drawing.Point(179, 167);
            this.chkRightToLeft.Name = "chkRightToLeft";
            this.chkRightToLeft.Size = new System.Drawing.Size(80, 17);
            this.chkRightToLeft.TabIndex = 16;
            this.chkRightToLeft.Text = "Right-to-left";
            this.chkRightToLeft.UseVisualStyleBackColor = true;
            // 
            // chkMerge
            // 
            this.chkMerge.AutoSize = true;
            this.chkMerge.Location = new System.Drawing.Point(26, 167);
            this.chkMerge.Name = "chkMerge";
            this.chkMerge.Size = new System.Drawing.Size(78, 17);
            this.chkMerge.TabIndex = 15;
            this.chkMerge.Text = "MergeCells";
            this.chkMerge.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(292, 38);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(37, 13);
            this.label18.TabIndex = 14;
            this.label18.Text = "Indent";
            // 
            // upDownIndent
            // 
            this.upDownIndent.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.upDownIndent.Location = new System.Drawing.Point(295, 55);
            this.upDownIndent.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.upDownIndent.Name = "upDownIndent";
            this.upDownIndent.Size = new System.Drawing.Size(62, 20);
            this.upDownIndent.TabIndex = 13;
            // 
            // upDownOrient
            // 
            this.upDownOrient.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.upDownOrient.Location = new System.Drawing.Point(87, 123);
            this.upDownOrient.Maximum = new decimal(new int[] {
            355,
            0,
            0,
            0});
            this.upDownOrient.Name = "upDownOrient";
            this.upDownOrient.Size = new System.Drawing.Size(57, 20);
            this.upDownOrient.TabIndex = 11;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(23, 126);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Orientation:";
            // 
            // chkWrapText
            // 
            this.chkWrapText.AutoSize = true;
            this.chkWrapText.Location = new System.Drawing.Point(179, 124);
            this.chkWrapText.Name = "chkWrapText";
            this.chkWrapText.Size = new System.Drawing.Size(73, 17);
            this.chkWrapText.TabIndex = 8;
            this.chkWrapText.Text = "WrapText";
            this.chkWrapText.UseVisualStyleBackColor = true;
            // 
            // cmbVrtcl
            // 
            this.cmbVrtcl.FormattingEnabled = true;
            this.cmbVrtcl.Items.AddRange(new object[] {
            "Top",
            "Middle",
            "Bottom"});
            this.cmbVrtcl.Location = new System.Drawing.Point(179, 58);
            this.cmbVrtcl.Name = "cmbVrtcl";
            this.cmbVrtcl.Size = new System.Drawing.Size(80, 21);
            this.cmbVrtcl.TabIndex = 7;
            // 
            // cmbHzntl
            // 
            this.cmbHzntl.FormattingEnabled = true;
            this.cmbHzntl.Items.AddRange(new object[] {
            "Left",
            "Center",
            "Right"});
            this.cmbHzntl.Location = new System.Drawing.Point(23, 58);
            this.cmbHzntl.Name = "cmbHzntl";
            this.cmbHzntl.Size = new System.Drawing.Size(91, 21);
            this.cmbHzntl.TabIndex = 6;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(176, 41);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(42, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Vertical";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 41);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Horizontal";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 99);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(64, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Text Control";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Text Alignment";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.DarkGray;
            this.panel4.Location = new System.Drawing.Point(78, 106);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(350, 1);
            this.panel4.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.DarkGray;
            this.panel3.Location = new System.Drawing.Point(100, 26);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(330, 1);
            this.panel3.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(304, 261);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(394, 261);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // GridFormatCellDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 292);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GridFormatCellDialog";
            this.Text = "FormatCellDialog";
            this.Load += new System.EventHandler(this.GridFormatCellDialog_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clrPanel)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownIndent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownOrient)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListBox lstFontType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFFace;
        private System.Windows.Forms.ListBox lstFontFace;
        private System.Windows.Forms.ListBox lstFontSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFSize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkStrikeout;
        private System.Windows.Forms.CheckBox chkUnderline;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private Syncfusion.Windows.Forms.ColorPickerButton colorPickerButton1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.NumericUpDown upDownOrient;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkWrapText;
        private System.Windows.Forms.ComboBox cmbVrtcl;
        private System.Windows.Forms.ComboBox cmbHzntl;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown upDownIndent;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label16;
        private Syncfusion.Windows.Forms.ColorPickerButton clrFore;
        private Syncfusion.Windows.Forms.ColorPickerButton clrBack;
        private System.Windows.Forms.ComboBox cmbBgStyle;
        private System.Windows.Forms.ComboBox cmbPattern;
        private System.Windows.Forms.Label lblPattern;
        private System.Windows.Forms.ComboBox cmbGradient;
        private System.Windows.Forms.Label lblGradient;
        private Syncfusion.Windows.Forms.Tools.GradientPanel clrPanel;
        private System.Windows.Forms.CheckBox chkMerge;
        private System.Windows.Forms.CheckBox chkRightToLeft;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.ListBox lstbxType;
        private System.Windows.Forms.ListBox lstbxFormat;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label lblIfText;
    }
}