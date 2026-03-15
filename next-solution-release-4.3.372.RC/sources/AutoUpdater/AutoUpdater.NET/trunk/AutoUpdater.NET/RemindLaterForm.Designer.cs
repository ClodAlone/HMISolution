namespace AutoUpdaterDotNET
{
    partial class RemindLaterForm
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemindLaterForm));
            this.labelTitle = new DevExpress.XtraEditors.LabelControl();
            this.pictureBoxIcon = new DevExpress.XtraEditors.PictureEdit();
            this.labelDescription = new DevExpress.XtraEditors.LabelControl();
            this.radioButtonYes = new DevExpress.XtraEditors.CheckButton();
            this.radioButtonNo = new DevExpress.XtraEditors.CheckButton();
            this.comboBoxRemindLater = new DevExpress.XtraEditors.ComboBoxEdit();
            this.buttonOK = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel = new DevExpress.Utils.Layout.TablePanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxRemindLater.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableLayoutPanel)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelTitle.Appearance.Font")));
            this.labelTitle.Appearance.Options.UseFont = true;
            this.tableLayoutPanel.SetColumn(this.labelTitle, 1);
            this.tableLayoutPanel.SetColumnSpan(this.labelTitle, 2);
            resources.ApplyResources(this.labelTitle, "labelTitle");
            this.labelTitle.Name = "labelTitle";
            this.tableLayoutPanel.SetRow(this.labelTitle, 0);
            // 
            // pictureBoxIcon
            // 
            this.tableLayoutPanel.SetColumn(this.pictureBoxIcon, 0);
            resources.ApplyResources(this.pictureBoxIcon, "pictureBoxIcon");
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Properties.ReadOnly = true;
            this.tableLayoutPanel.SetRow(this.pictureBoxIcon, 0);
            this.tableLayoutPanel.SetRowSpan(this.pictureBoxIcon, 2);
            // 
            // labelDescription
            // 
            this.labelDescription.Appearance.Options.UseTextOptions = true;
            this.labelDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            resources.ApplyResources(this.labelDescription, "labelDescription");
            this.tableLayoutPanel.SetColumn(this.labelDescription, 2);
            this.tableLayoutPanel.SetColumnSpan(this.labelDescription, 2);
            this.labelDescription.Name = "labelDescription";
            this.tableLayoutPanel.SetRow(this.labelDescription, 3);
            // 
            // radioButtonYes
            // 
            resources.ApplyResources(this.radioButtonYes, "radioButtonYes");
            this.radioButtonYes.Checked = true;
            this.tableLayoutPanel.SetColumn(this.radioButtonYes, 1);
            this.radioButtonYes.Name = "radioButtonYes";
            this.tableLayoutPanel.SetRow(this.radioButtonYes, 2);
            this.radioButtonYes.CheckedChanged += new System.EventHandler(this.RadioButtonYesCheckedChanged);
            // 
            // radioButtonNo
            // 
            this.radioButtonNo.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("radioButtonNo.Appearance.Font")));
            this.radioButtonNo.Appearance.Options.UseFont = true;
            resources.ApplyResources(this.radioButtonNo, "radioButtonNo");
            this.tableLayoutPanel.SetColumn(this.radioButtonNo, 1);
            this.tableLayoutPanel.SetColumnSpan(this.radioButtonNo, 2);
            this.radioButtonNo.Name = "radioButtonNo";
            this.tableLayoutPanel.SetRow(this.radioButtonNo, 3);
            // 
            // comboBoxRemindLater
            // 
            this.tableLayoutPanel.SetColumn(this.comboBoxRemindLater, 2);
            resources.ApplyResources(this.comboBoxRemindLater, "comboBoxRemindLater");
            this.comboBoxRemindLater.Name = "comboBoxRemindLater";
            this.comboBoxRemindLater.Properties.Items.AddRange(new object[] {
            resources.GetString("comboBoxRemindLater.Properties.Items"),
            resources.GetString("comboBoxRemindLater.Properties.Items1"),
            resources.GetString("comboBoxRemindLater.Properties.Items2"),
            resources.GetString("comboBoxRemindLater.Properties.Items3"),
            resources.GetString("comboBoxRemindLater.Properties.Items4"),
            resources.GetString("comboBoxRemindLater.Properties.Items5"),
            resources.GetString("comboBoxRemindLater.Properties.Items6")});
            this.tableLayoutPanel.SetRow(this.comboBoxRemindLater, 2);
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.ButtonOkClick);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Controls.Add(this.pictureBoxIcon);
            this.tableLayoutPanel.Controls.Add(this.labelTitle);
            this.tableLayoutPanel.Controls.Add(this.radioButtonNo);
            this.tableLayoutPanel.Controls.Add(this.comboBoxRemindLater);
            this.tableLayoutPanel.Controls.Add(this.radioButtonYes);
            this.tableLayoutPanel.Controls.Add(this.labelDescription);
            this.tableLayoutPanel.Controls.Add(this.buttonOK);
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // RemindLaterForm
            // 
            this.Appearance.Options.UseFont = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IconOptions.ShowIcon = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RemindLaterForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.RemindLaterFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxRemindLater.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableLayoutPanel)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelTitle;
        private DevExpress.XtraEditors.PictureEdit pictureBoxIcon;
        private DevExpress.XtraEditors.LabelControl labelDescription;
        private DevExpress.XtraEditors.CheckButton radioButtonYes;
        private DevExpress.XtraEditors.CheckButton radioButtonNo;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxRemindLater;
        private DevExpress.XtraEditors.SimpleButton buttonOK;
        private DevExpress.Utils.Layout.TablePanel tableLayoutPanel;
    }
}