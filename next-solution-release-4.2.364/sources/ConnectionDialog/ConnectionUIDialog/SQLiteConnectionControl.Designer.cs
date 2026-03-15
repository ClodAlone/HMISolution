namespace Microsoft.Data.ConnectionUI
{
	public partial class SQLiteConnectionUIControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SQLiteConnectionUIControl));
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.DatabaseLabel = new System.Windows.Forms.Label();
            this.databaseFileTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.serverTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.databaseFileTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
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
            // DatabaseLabel
            // 
            resources.ApplyResources(this.DatabaseLabel, "DatabaseLabel");
            this.DatabaseLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.DatabaseLabel.Name = "DatabaseLabel";
            // 
            // databaseFileTableLayoutPanel
            // 
            resources.ApplyResources(this.databaseFileTableLayoutPanel, "databaseFileTableLayoutPanel");
            this.databaseFileTableLayoutPanel.Controls.Add(this.serverTextBox, 0, 0);
            this.databaseFileTableLayoutPanel.Controls.Add(this.button1, 1, 0);
            this.databaseFileTableLayoutPanel.Name = "databaseFileTableLayoutPanel";
            // 
            // serverTextBox
            // 
            resources.ApplyResources(this.serverTextBox, "serverTextBox");
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.TextChanged += new System.EventHandler(this.SetServer);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.Click += new System.EventHandler(this.Browse);
            // 
            // SQLiteConnectionUIControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.databaseFileTableLayoutPanel);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.DatabaseLabel);
            this.Name = "SQLiteConnectionUIControl";
            this.databaseFileTableLayoutPanel.ResumeLayout(false);
            this.databaseFileTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label passwordLabel;
		private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label DatabaseLabel;
        private System.Windows.Forms.TableLayoutPanel databaseFileTableLayoutPanel;
        private System.Windows.Forms.TextBox serverTextBox;
        private System.Windows.Forms.Button button1;
    }
}
