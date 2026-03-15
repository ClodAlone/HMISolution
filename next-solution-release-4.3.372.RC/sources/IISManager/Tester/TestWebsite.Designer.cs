namespace Tester
{
    partial class TestWebsite
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.BtnChgName = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.btnChgPort = new System.Windows.Forms.Button();
            this.btnChgPath = new System.Windows.Forms.Button();
            this.WebDirs = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Website Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Main Path:";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(102, 22);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(109, 21);
            this.tbName.TabIndex = 2;
            // 
            // tbPath
            // 
            this.tbPath.Location = new System.Drawing.Point(102, 99);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(243, 21);
            this.tbPath.TabIndex = 3;
            // 
            // BtnChgName
            // 
            this.BtnChgName.Location = new System.Drawing.Point(242, 20);
            this.BtnChgName.Name = "BtnChgName";
            this.BtnChgName.Size = new System.Drawing.Size(75, 23);
            this.BtnChgName.TabIndex = 4;
            this.BtnChgName.Text = "Change Name";
            this.BtnChgName.UseVisualStyleBackColor = true;
            this.BtnChgName.Click += new System.EventHandler(this.BtnChgName_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Port:";
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(102, 63);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(109, 21);
            this.tbPort.TabIndex = 6;
            // 
            // btnChgPort
            // 
            this.btnChgPort.Location = new System.Drawing.Point(242, 61);
            this.btnChgPort.Name = "btnChgPort";
            this.btnChgPort.Size = new System.Drawing.Size(75, 23);
            this.btnChgPort.TabIndex = 7;
            this.btnChgPort.Text = "Change Name";
            this.btnChgPort.UseVisualStyleBackColor = true;
            this.btnChgPort.Click += new System.EventHandler(this.btnChgPort_Click);
            // 
            // btnChgPath
            // 
            this.btnChgPath.Location = new System.Drawing.Point(351, 97);
            this.btnChgPath.Name = "btnChgPath";
            this.btnChgPath.Size = new System.Drawing.Size(75, 23);
            this.btnChgPath.TabIndex = 8;
            this.btnChgPath.Text = "Change Name";
            this.btnChgPath.UseVisualStyleBackColor = true;
            this.btnChgPath.Click += new System.EventHandler(this.btnChgPath_Click);
            // 
            // WebDirs
            // 
            this.WebDirs.Location = new System.Drawing.Point(23, 139);
            this.WebDirs.Name = "WebDirs";
            this.WebDirs.Size = new System.Drawing.Size(403, 155);
            this.WebDirs.TabIndex = 9;
            // 
            // TestWebsite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 306);
            this.Controls.Add(this.WebDirs);
            this.Controls.Add(this.btnChgPath);
            this.Controls.Add(this.btnChgPort);
            this.Controls.Add(this.tbPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BtnChgName);
            this.Controls.Add(this.tbPath);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "TestWebsite";
            this.Text = "TestWebsite";
            this.Load += new System.EventHandler(this.TestWebsite_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Button BtnChgName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.Button btnChgPort;
        private System.Windows.Forms.Button btnChgPath;
        private System.Windows.Forms.TreeView WebDirs;
    }
}