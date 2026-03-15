using System.Threading;

namespace AutoUpdaterDotNET
{
    partial class UpdateForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateForm));
            this.labelUpdate = new DevExpress.XtraEditors.LabelControl();
            this.labelDescription = new DevExpress.XtraEditors.LabelControl();
            this.labelReleaseNotes = new DevExpress.XtraEditors.LabelControl();
            this.tabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.buttonSkip = new DevExpress.XtraEditors.SimpleButton();
            this.buttonRemindLater = new DevExpress.XtraEditors.SimpleButton();
            this.buttonUpdate = new DevExpress.XtraEditors.SimpleButton();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tabControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelUpdate
            // 
            this.labelUpdate.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelUpdate.Appearance.Font")));
            this.labelUpdate.Appearance.Options.UseFont = true;
            resources.ApplyResources(this.labelUpdate, "labelUpdate");
            this.labelUpdate.Name = "labelUpdate";
            // 
            // labelDescription
            // 
            this.labelDescription.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelDescription.Appearance.Font")));
            this.labelDescription.Appearance.Options.UseFont = true;
            resources.ApplyResources(this.labelDescription, "labelDescription");
            this.labelDescription.Name = "labelDescription";
            // 
            // labelReleaseNotes
            // 
            this.labelReleaseNotes.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelReleaseNotes.Appearance.Font")));
            this.labelReleaseNotes.Appearance.Options.UseFont = true;
            resources.ApplyResources(this.labelReleaseNotes, "labelReleaseNotes");
            this.labelReleaseNotes.Name = "labelReleaseNotes";
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            // 
            // buttonSkip
            // 
            resources.ApplyResources(this.buttonSkip, "buttonSkip");
            this.buttonSkip.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.buttonSkip.ImageOptions.Image = global::AutoUpdaterDotNET.Properties.Resources.hand_point;
            this.buttonSkip.Name = "buttonSkip";
            this.buttonSkip.Click += new System.EventHandler(this.ButtonSkipClick);
            // 
            // buttonRemindLater
            // 
            resources.ApplyResources(this.buttonRemindLater, "buttonRemindLater");
            this.buttonRemindLater.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonRemindLater.ImageOptions.Image = global::AutoUpdaterDotNET.Properties.Resources.clock_play;
            this.buttonRemindLater.Name = "buttonRemindLater";
            this.buttonRemindLater.Click += new System.EventHandler(this.ButtonRemindLaterClick);
            // 
            // buttonUpdate
            // 
            resources.ApplyResources(this.buttonUpdate, "buttonUpdate");
            this.buttonUpdate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonUpdate.ImageOptions.Image = global::AutoUpdaterDotNET.Properties.Resources.download;
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Click += new System.EventHandler(this.ButtonUpdateClick);
            // 
            // UpdateForm
            // 
            this.AcceptButton = this.buttonUpdate;
            this.Appearance.Options.UseFont = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonRemindLater;
            this.Controls.Add(this.buttonUpdate);
            this.Controls.Add(this.buttonRemindLater);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonSkip);
            this.Controls.Add(this.labelReleaseNotes);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.labelUpdate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IconOptions.Image = global::AutoUpdaterDotNET.Properties.Resources.update;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateForm";
            this.Load += new System.EventHandler(this.UpdateFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.tabControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.LabelControl labelUpdate;
        private DevExpress.XtraEditors.LabelControl labelDescription;
        private DevExpress.XtraEditors.LabelControl labelReleaseNotes;
        private DevExpress.XtraTab.XtraTabControl tabControl1;
        private DevExpress.XtraEditors.SimpleButton buttonSkip;
        private DevExpress.XtraEditors.SimpleButton buttonRemindLater;
        private DevExpress.XtraEditors.SimpleButton buttonUpdate;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
    }
}