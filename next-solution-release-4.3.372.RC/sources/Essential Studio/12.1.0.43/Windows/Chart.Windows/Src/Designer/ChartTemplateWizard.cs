#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Chart.Design;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// This dialog is used for preview the <see cref="ChartTemplate"/>.
    /// </summary>
    public class ChartTemplateWizard : Office2007Form
    {
        #region Constants
        private const string c_defaultSerieName = "Default";
        private const int c_defaultPointsCount = 8;
        private const int c_defaultFirstValueMax = 100;
        private const int c_defaultSecondValueMax = 100;
        private const int c_defaultThirdValueMax = 100;
        private const int c_defaultFourthValueMax = 100;
        private const string c_templateMask = "*.xml";
        private const string c_templatesPath = @"Resources\ChartTemplates";
        #endregion

        #region Members
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListBox lsbxTemplates;
        private Syncfusion.Windows.Forms.Chart.ImageButton bttnCancel;
        private Syncfusion.Windows.Forms.Chart.ImageButton bttnOk;
        private System.Windows.Forms.TextBox edFolder;
        private Syncfusion.Windows.Forms.Chart.ImageButton bttnBrowse;
        private System.Windows.Forms.Label labelTempalesFolder;
        private System.Windows.Forms.Label labelTemplates;
        private System.Windows.Forms.Label labelPreview;
        private Syncfusion.Windows.Forms.Chart.ChartControl chartControl;
        private Label labelErrorMessageTitle;
        private Label labelErrorMessage;
        private Label labelErrorMessageText;
        private Panel panelError;
        private FolderBrowser folderBrowser;

        private ChartTemplate m_resultTamplate = null;
        private Random m_random = new Random();
        private string m_templateDirectory = String.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the instance of <see cref="ChartTemplate"/>.
        /// </summary>
        public ChartTemplate Template
        {
            get
            {
                return m_resultTamplate;
            }

            set
            {
                if (m_resultTamplate != value)
                {
                    m_resultTamplate = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the template directory.
        /// </summary>
        /// <value>The template directory.</value>
        public string TemplateDirectory
        {
            get { return folderBrowser.SelectLocation; }
            set { folderBrowser.SelectLocation = value; }
        }
        #endregion

        #region Initialize
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartTemplateWizard));
            this.chartControl = new Syncfusion.Windows.Forms.Chart.ChartControl();
            this.lsbxTemplates = new System.Windows.Forms.ListBox();
            this.bttnCancel = new Syncfusion.Windows.Forms.Chart.ImageButton();
            this.bttnOk = new Syncfusion.Windows.Forms.Chart.ImageButton();
            this.edFolder = new System.Windows.Forms.TextBox();
            this.bttnBrowse = new Syncfusion.Windows.Forms.Chart.ImageButton();
            this.labelTempalesFolder = new System.Windows.Forms.Label();
            this.labelTemplates = new System.Windows.Forms.Label();
            this.labelPreview = new System.Windows.Forms.Label();
            this.folderBrowser = new Syncfusion.Windows.Forms.FolderBrowser(this.components);
            this.panelError = new System.Windows.Forms.Panel();
            this.labelErrorMessage = new System.Windows.Forms.Label();
            this.labelErrorMessageTitle = new System.Windows.Forms.Label();
            this.labelErrorMessageText = new System.Windows.Forms.Label();
            this.panelError.SuspendLayout();
            this.SuspendLayout();
            // 
            // chartControl
            // 
            // 
            // 
            // 
            this.chartControl.Legend.Font = new System.Drawing.Font("Verdana", 10F);
            this.chartControl.Legend.Location = new System.Drawing.Point(272, 129);
            this.chartControl.Location = new System.Drawing.Point(212, 138);
            this.chartControl.Name = "chartControl";
            this.chartControl.Size = new System.Drawing.Size(371, 255);
            this.chartControl.TabIndex = 0;
            this.chartControl.Text = "chartControl";
            // 
            // 
            // 
            this.chartControl.Title.Name = "Default";
            this.chartControl.Title.Text = "chartControl";
            this.chartControl.Titles.Add(this.chartControl.Title);
            this.chartControl.ToolBar.ButtonBackColor = System.Drawing.Color.Transparent;
            this.chartControl.ToolBar.ButtonSize = new System.Drawing.Size(22, 22);
            this.chartControl.ToolBar.Spacing = 0;
            // 
            // lsbxTemplates
            // 
            this.lsbxTemplates.BackColor = System.Drawing.Color.White;
            this.lsbxTemplates.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lsbxTemplates.DisplayMember = "Name";
            this.lsbxTemplates.ForeColor = System.Drawing.Color.Black;
            this.lsbxTemplates.HorizontalScrollbar = true;
            this.lsbxTemplates.Location = new System.Drawing.Point(31, 145);
            this.lsbxTemplates.Name = "lsbxTemplates";
            this.lsbxTemplates.Size = new System.Drawing.Size(168, 273);
            this.lsbxTemplates.TabIndex = 1;
            this.lsbxTemplates.SelectedIndexChanged += new System.EventHandler(this.lsbxTemplates_SelectedIndexChanged);
            // 
            // bttnCancel
            // 
            this.bttnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bttnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bttnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bttnCancel.ForeColor = System.Drawing.Color.Black;
            this.bttnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bttnCancel.Location = new System.Drawing.Point(493, 400);
            this.bttnCancel.Name = "bttnCancel";
            this.bttnCancel.Size = new System.Drawing.Size(84, 24);
            this.bttnCancel.TabIndex = 2;
            this.bttnCancel.Text = "Cancel";
            // 
            // bttnOk
            // 
            this.bttnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bttnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bttnOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bttnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bttnOk.Location = new System.Drawing.Point(402, 400);
            this.bttnOk.Name = "bttnOk";
            this.bttnOk.Size = new System.Drawing.Size(84, 24);
            this.bttnOk.TabIndex = 3;
            this.bttnOk.Text = "Ok";
            // 
            // edFolder
            // 
            this.edFolder.BackColor = System.Drawing.Color.White;
            this.edFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.edFolder.Location = new System.Drawing.Point(142, 100);
            this.edFolder.Name = "edFolder";
            this.edFolder.Size = new System.Drawing.Size(352, 20);
            this.edFolder.TabIndex = 4;
            this.edFolder.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edFolder_KeyDown);
            // 
            // bttnBrowse
            // 
            this.bttnBrowse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bttnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bttnBrowse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bttnBrowse.Location = new System.Drawing.Point(500, 100);
            this.bttnBrowse.Name = "bttnBrowse";
            this.bttnBrowse.Size = new System.Drawing.Size(76, 20);
            this.bttnBrowse.TabIndex = 5;
            this.bttnBrowse.Text = "Browse";
            this.bttnBrowse.Click += new System.EventHandler(this.bttnBrowse_Click);
            // 
            // labelTempalesFolder
            // 
            this.labelTempalesFolder.BackColor = System.Drawing.Color.Transparent;
            this.labelTempalesFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelTempalesFolder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(73)))), ((int)(((byte)(141)))));
            this.labelTempalesFolder.Location = new System.Drawing.Point(22, 103);
            this.labelTempalesFolder.Name = "labelTempalesFolder";
            this.labelTempalesFolder.Size = new System.Drawing.Size(114, 18);
            this.labelTempalesFolder.TabIndex = 6;
            this.labelTempalesFolder.Text = "Template location";
            // 
            // labelTemplates
            // 
            this.labelTemplates.BackColor = System.Drawing.Color.Transparent;
            this.labelTemplates.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelTemplates.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(73)))), ((int)(((byte)(141)))));
            this.labelTemplates.Location = new System.Drawing.Point(22, 123);
            this.labelTemplates.Name = "labelTemplates";
            this.labelTemplates.Size = new System.Drawing.Size(100, 14);
            this.labelTemplates.TabIndex = 7;
            this.labelTemplates.Text = "Templates";
            // 
            // labelPreview
            // 
            this.labelPreview.BackColor = System.Drawing.Color.Transparent;
            this.labelPreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelPreview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(73)))), ((int)(((byte)(141)))));
            this.labelPreview.Location = new System.Drawing.Point(210, 124);
            this.labelPreview.Name = "labelPreview";
            this.labelPreview.Size = new System.Drawing.Size(100, 14);
            this.labelPreview.TabIndex = 8;
            this.labelPreview.Text = "Preview";
            // 
            // folderBrowser
            // 
            this.folderBrowser.StartLocation = Syncfusion.Windows.Forms.FolderBrowserFolder.Desktop;
            this.folderBrowser.Style = Syncfusion.Windows.Forms.FolderBrowserStyles.RestrictToFilesystem;
            // 
            // panelError
            // 
            this.panelError.BackColor = System.Drawing.Color.White;
            this.panelError.Controls.Add(this.labelErrorMessage);
            this.panelError.Controls.Add(this.labelErrorMessageTitle);
            this.panelError.Controls.Add(this.labelErrorMessageText);
            this.panelError.Location = new System.Drawing.Point(212, 138);
            this.panelError.Name = "panelError";
            this.panelError.Size = new System.Drawing.Size(371, 256);
            this.panelError.TabIndex = 9;
            // 
            // labelErrorMessage
            // 
            this.labelErrorMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelErrorMessage.Font = new System.Drawing.Font("Verdana", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelErrorMessage.Location = new System.Drawing.Point(0, 102);
            this.labelErrorMessage.Name = "labelErrorMessage";
            this.labelErrorMessage.Size = new System.Drawing.Size(371, 20);
            this.labelErrorMessage.TabIndex = 0;
            this.labelErrorMessage.Text = "    You are trying to load invalid Chart Template     ";
            this.labelErrorMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelErrorMessageTitle
            // 
            this.labelErrorMessageTitle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelErrorMessageTitle.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelErrorMessageTitle.Location = new System.Drawing.Point(0, 122);
            this.labelErrorMessageTitle.Name = "labelErrorMessageTitle";
            this.labelErrorMessageTitle.Size = new System.Drawing.Size(371, 14);
            this.labelErrorMessageTitle.TabIndex = 1;
            this.labelErrorMessageTitle.Text = "Error message :";
            // 
            // labelErrorMessageText
            // 
            this.labelErrorMessageText.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelErrorMessageText.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelErrorMessageText.Location = new System.Drawing.Point(0, 136);
            this.labelErrorMessageText.Name = "labelErrorMessageText";
            this.labelErrorMessageText.Size = new System.Drawing.Size(371, 120);
            this.labelErrorMessageText.TabIndex = 2;
            // 
            // ChartTemplateWizard
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(597, 449);
            this.Controls.Add(this.edFolder);
            this.Controls.Add(this.lsbxTemplates);
            this.Controls.Add(this.labelPreview);
            this.Controls.Add(this.labelTemplates);
            this.Controls.Add(this.labelTempalesFolder);
            this.Controls.Add(this.bttnBrowse);
            this.Controls.Add(this.bttnOk);
            this.Controls.Add(this.bttnCancel);
            this.Controls.Add(this.chartControl);
            this.Controls.Add(this.panelError);
            this.MaximizeBox = false;
            this.Name = "ChartTemplateWizard";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chart Load Template Wizard";
            this.Load += new System.EventHandler(this.ChartTemplateWizard_Load);
            this.panelError.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartTemplateWizard"/> class.
        /// </summary>
        public ChartTemplateWizard()
        {
            InitializeComponent();
            UserInitializeComponent();
        }

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

        #region Implementation
        /// <summary>
        /// Users the initialize component.
        /// </summary>
        private void UserInitializeComponent()
        {
            this.BackgroundImage = ChartWizardResources.LoadTemplateBackImage;
            this.Icon = ChartWizardResources.WizardIcon;

            bttnCancel.NormalImage = ChartWizardResources.ButtonAlternativeNormal;
            bttnOk.NormalImage = ChartWizardResources.ButtonAlternativeNormal;
            bttnBrowse.NormalImage = ChartWizardResources.ButtonAlternativeNormal;

            bttnCancel.SelectedImage = ChartWizardResources.ButtonAlternativeSelected;
            bttnOk.SelectedImage = ChartWizardResources.ButtonAlternativeSelected;
            bttnBrowse.SelectedImage = ChartWizardResources.ButtonAlternativeSelected;

            chartControl.BringToFront();

            if (m_templateDirectory == String.Empty)
            {
                string templatesPath = ChartControlDesigner.InstallPath + c_templatesPath;

                if (Directory.Exists(templatesPath))
                {
                    folderBrowser.SelectLocation = templatesPath;
                }
                else
                {
                    folderBrowser.SelectLocation = Directory.GetCurrentDirectory();
                }
            }
            else
            {
                folderBrowser.SelectLocation = m_templateDirectory;
            }
        }

        /// <summary>
        /// Adds the all files from the current directory by the mask (*.xml).
        /// </summary>
        private void RefreshTamplatesList()
        {
            string[] files = Directory.GetFiles(edFolder.Text, c_templateMask);

            lsbxTemplates.Items.Clear();

            for (int i = 0; i < files.Length; i++)
            {
                lsbxTemplates.Items.Add(new FileInfo(files[i]));
            }
        }

        /// <summary>
        /// Handles the Load event of the ChartTemplateWizard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ChartTemplateWizard_Load(object sender, System.EventArgs e)
        {
            ChartSeries series = new ChartSeries(c_defaultSerieName);
            series.Text = c_defaultSerieName;

            for (int i = 0; i < c_defaultPointsCount; i++)
            {
                series.Points.Add(i, m_random.Next(c_defaultFirstValueMax), m_random.Next(c_defaultSecondValueMax), m_random.Next(c_defaultThirdValueMax), m_random.Next(c_defaultFourthValueMax) );
            }

            chartControl.Series.Add(series);

            edFolder.Text = folderBrowser.SelectLocation;
            RefreshTamplatesList();
        }

        /// <summary>
        /// Called when the Browse button is cliked. This method displays the <see cref="FolderBrowser"/> dialog.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void bttnBrowse_Click(object sender, System.EventArgs e)
        {
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                edFolder.Text = folderBrowser.DirectoryPath;
                RefreshTamplatesList();
            }
        }

        /// <summary>
        /// Called when the template is selected from the list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void lsbxTemplates_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            FileInfo file = lsbxTemplates.SelectedItem as FileInfo;

            if (file != null)
            {
                m_resultTamplate = new ChartTemplate(typeof(ChartControl));

                try
                {
                    m_resultTamplate.Load(file.FullName);
                    m_resultTamplate.Apply(chartControl);
                    chartControl.Visible = true;
                }
                catch (Exception ex)
                {
                    labelErrorMessageText.Text = ex.Message;
                    chartControl.Visible = false;
                    m_resultTamplate = null;
                }
            }
        }

        /// <summary>
        /// Called when a key is pressed while the control has focus. 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void edFolder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                try
                {
                    RefreshTamplatesList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// The ProcessKeyPreview method.
        /// </summary>
        /// <param name="m">A <see cref="T:System.Windows.Forms.Message"/>, passed by reference, that represents the window message to process.</param>
        /// <returns>
        /// true if the message was processed by the control; otherwise, false.
        /// </returns>
        protected override bool ProcessKeyPreview(ref Message m)
        {
            if (m.WParam.ToInt32() == (int)Keys.Escape)
            {
                this.Close();
            }

            return base.ProcessKeyPreview(ref m);
        }

        /// <summary>
        /// Processes a command key.
        /// </summary>
        /// <param name="msg">A <see cref="T:System.Windows.Forms.Message"></see>, passed by reference, that represents the Win32 message to process.</param>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"></see> values that represents the key to process.</param>
        /// <returns>
        /// true if the keystroke was processed and consumed by the control; otherwise, false to allow further processing.
        /// </returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion
    }
}