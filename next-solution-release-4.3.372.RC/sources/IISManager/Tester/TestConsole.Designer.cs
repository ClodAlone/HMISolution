namespace Tester
{
    partial class TestConsole
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
            this.BtnOpen = new System.Windows.Forms.Button();
            this.cbWebsiteList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbNewSite = new System.Windows.Forms.TextBox();
            this.BtnNewSite = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbRootPath = new System.Windows.Forms.TextBox();
            this.BtnDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnOpen
            // 
            this.BtnOpen.Location = new System.Drawing.Point(213, 27);
            this.BtnOpen.Name = "BtnOpen";
            this.BtnOpen.Size = new System.Drawing.Size(107, 23);
            this.BtnOpen.TabIndex = 0;
            this.BtnOpen.Text = "OpenWebsite";
            this.BtnOpen.UseVisualStyleBackColor = true;
            this.BtnOpen.Click += new System.EventHandler(this.BtnOpen_Click);
            // 
            // cbWebsiteList
            // 
            this.cbWebsiteList.FormattingEnabled = true;
            this.cbWebsiteList.Location = new System.Drawing.Point(13, 27);
            this.cbWebsiteList.Name = "cbWebsiteList";
            this.cbWebsiteList.Size = new System.Drawing.Size(194, 20);
            this.cbWebsiteList.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "WebsiteName:";
            // 
            // tbNewSite
            // 
            this.tbNewSite.Location = new System.Drawing.Point(95, 96);
            this.tbNewSite.Name = "tbNewSite";
            this.tbNewSite.Size = new System.Drawing.Size(100, 21);
            this.tbNewSite.TabIndex = 3;
            // 
            // BtnNewSite
            // 
            this.BtnNewSite.Location = new System.Drawing.Point(245, 205);
            this.BtnNewSite.Name = "BtnNewSite";
            this.BtnNewSite.Size = new System.Drawing.Size(75, 23);
            this.BtnNewSite.TabIndex = 4;
            this.BtnNewSite.Text = "Create";
            this.BtnNewSite.UseVisualStyleBackColor = true;
            this.BtnNewSite.Click += new System.EventHandler(this.BtnNewSite_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "port:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(95, 131);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(100, 21);
            this.tbPort.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 172);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "Root Path:";
            // 
            // tbRootPath
            // 
            this.tbRootPath.Location = new System.Drawing.Point(95, 169);
            this.tbRootPath.Name = "tbRootPath";
            this.tbRootPath.Size = new System.Drawing.Size(330, 21);
            this.tbRootPath.TabIndex = 8;
            // 
            // BtnDelete
            // 
            this.BtnDelete.Location = new System.Drawing.Point(338, 27);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.Size = new System.Drawing.Size(103, 23);
            this.BtnDelete.TabIndex = 9;
            this.BtnDelete.Text = "DeleteWebsite";
            this.BtnDelete.UseVisualStyleBackColor = true;
            this.BtnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // TestConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 266);
            this.Controls.Add(this.BtnDelete);
            this.Controls.Add(this.tbRootPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BtnNewSite);
            this.Controls.Add(this.tbNewSite);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbWebsiteList);
            this.Controls.Add(this.BtnOpen);
            this.Name = "TestConsole";
            this.Text = "TestIISWebManager";
            this.Load += new System.EventHandler(this.TestConsole_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnOpen;
        private System.Windows.Forms.ComboBox cbWebsiteList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbNewSite;
        private System.Windows.Forms.Button BtnNewSite;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbRootPath;
        private System.Windows.Forms.Button BtnDelete;
    }
}

