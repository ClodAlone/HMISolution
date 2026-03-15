#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram.Controls
{
    /// <summary>
    /// Form used to prompt the user for the name of new symbol palettes.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controls.PaletteGroupBar"/>
    /// </remarks>
    public class PaletteAddDlg
        : Form
    {
        #region Class members
        private TextBox paletteNameTextBox;
        private Button btnOK;
        private Button btnCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteAddDlg"/> class.
        /// </summary>
        public PaletteAddDlg()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaletteAddDlg));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.paletteNameTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.Click += new System.EventHandler(this.OK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            // 
            // PaletteNameTextBox
            // 
            resources.ApplyResources(this.paletteNameTextBox, "PaletteNameTextBox");
            this.paletteNameTextBox.Name = "PaletteNameTextBox";
            // 
            // PaletteAddDlg
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.paletteNameTextBox);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PaletteAddDlg";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.PaletteAddDlg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #endregion

        #region Class properties
        /// <summary>
        /// Gets the name for the new palette entered by the user.
        /// </summary>
        public string PaletteName
        {
            get
            {
                return paletteNameTextBox.Text;
            }
        }

        #endregion

        #region Event handlers
        private void OK_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
        private void PaletteAddDlg_Load(object sender, System.EventArgs e)
        {
            this.paletteNameTextBox.Focus();
        }
        #endregion
    }
}
