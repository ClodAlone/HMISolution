namespace Microsoft.Data.ConnectionUI
{
	public partial class MySQLConnectionUIControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MySQLConnectionUIControl));
            this.serverLabel = new System.Windows.Forms.Label();
            this.serverTextBox = new System.Windows.Forms.TextBox();
            this.logonGroupBox = new System.Windows.Forms.GroupBox();
            this.loginTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.savePasswordCheckBox = new System.Windows.Forms.CheckBox();
            this.DatabaseLabel = new System.Windows.Forms.Label();
            this.DatabaseTextBox = new System.Windows.Forms.TextBox();
            this.logonGroupBox.SuspendLayout();
            this.loginTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverLabel
            // 
            resources.ApplyResources(this.serverLabel, "serverLabel");
            this.serverLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.serverLabel.Name = "serverLabel";
            // 
            // serverTextBox
            // 
            resources.ApplyResources(this.serverTextBox, "serverTextBox");
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.TextChanged += new System.EventHandler(this.SetServer);
            this.serverTextBox.Leave += new System.EventHandler(this.TrimControlText);
            // 
            // logonGroupBox
            // 
            resources.ApplyResources(this.logonGroupBox, "logonGroupBox");
            this.logonGroupBox.Controls.Add(this.loginTableLayoutPanel);
            this.logonGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.logonGroupBox.Name = "logonGroupBox";
            this.logonGroupBox.TabStop = false;
            // 
            // loginTableLayoutPanel
            // 
            resources.ApplyResources(this.loginTableLayoutPanel, "loginTableLayoutPanel");
            this.loginTableLayoutPanel.Controls.Add(this.userNameLabel, 0, 1);
            this.loginTableLayoutPanel.Controls.Add(this.userNameTextBox, 1, 1);
            this.loginTableLayoutPanel.Controls.Add(this.passwordLabel, 0, 2);
            this.loginTableLayoutPanel.Controls.Add(this.passwordTextBox, 1, 2);
            this.loginTableLayoutPanel.Controls.Add(this.savePasswordCheckBox, 1, 3);
            this.loginTableLayoutPanel.Controls.Add(this.DatabaseLabel, 0, 0);
            this.loginTableLayoutPanel.Controls.Add(this.DatabaseTextBox, 1, 0);
            this.loginTableLayoutPanel.Name = "loginTableLayoutPanel";
            // 
            // userNameLabel
            // 
            resources.ApplyResources(this.userNameLabel, "userNameLabel");
            this.userNameLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.userNameLabel.Name = "userNameLabel";
            // 
            // userNameTextBox
            // 
            resources.ApplyResources(this.userNameTextBox, "userNameTextBox");
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.TextChanged += new System.EventHandler(this.SetUserName);
            this.userNameTextBox.Leave += new System.EventHandler(this.TrimControlText);
            // 
            // passwordLabel
            // 
            resources.ApplyResources(this.passwordLabel, "passwordLabel");
            this.passwordLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.passwordLabel.Name = "passwordLabel";
            // 
            // passwordTextBox
            // 
            resources.ApplyResources(this.passwordTextBox, "passwordTextBox");
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.UseSystemPasswordChar = true;
            this.passwordTextBox.TextChanged += new System.EventHandler(this.SetPassword);
            // 
            // savePasswordCheckBox
            // 
            resources.ApplyResources(this.savePasswordCheckBox, "savePasswordCheckBox");
            this.savePasswordCheckBox.Name = "savePasswordCheckBox";
            this.savePasswordCheckBox.CheckedChanged += new System.EventHandler(this.SetSavePassword);
            // 
            // DatabaseLabel
            // 
            resources.ApplyResources(this.DatabaseLabel, "DatabaseLabel");
            this.DatabaseLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.DatabaseLabel.Name = "DatabaseLabel";
            // 
            // DatabaseTextBox
            // 
            resources.ApplyResources(this.DatabaseTextBox, "DatabaseTextBox");
            this.DatabaseTextBox.Name = "DatabaseTextBox";
            this.DatabaseTextBox.TextChanged += new System.EventHandler(this.SetDatabase);
            this.DatabaseTextBox.Leave += new System.EventHandler(this.TrimControlText);
            // 
            // MySQLConnectionUIControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logonGroupBox);
            this.Controls.Add(this.serverTextBox);
            this.Controls.Add(this.serverLabel);
            this.Name = "MySQLConnectionUIControl";
            this.logonGroupBox.ResumeLayout(false);
            this.logonGroupBox.PerformLayout();
            this.loginTableLayoutPanel.ResumeLayout(false);
            this.loginTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label serverLabel;
		private System.Windows.Forms.TextBox serverTextBox;
		private System.Windows.Forms.GroupBox logonGroupBox;
		private System.Windows.Forms.TableLayoutPanel loginTableLayoutPanel;
		private System.Windows.Forms.Label userNameLabel;
		private System.Windows.Forms.TextBox userNameTextBox;
		private System.Windows.Forms.Label passwordLabel;
		private System.Windows.Forms.TextBox passwordTextBox;
		private System.Windows.Forms.CheckBox savePasswordCheckBox;
        private System.Windows.Forms.Label DatabaseLabel;
        private System.Windows.Forms.TextBox DatabaseTextBox;
    }
}
