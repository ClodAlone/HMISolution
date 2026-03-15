#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility.Selection;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// FindDialog form is used for text searching in the HTMLUIControl.
    /// </summary>
    [ToolboxItem(false)]
    public class FindDialog : System.Windows.Forms.Form
    {
        #region Class constants
        /// <summary>
        /// Default error message.
        /// </summary>
        private const string DEF_ERR_MSG = "Cannot find ";

        /// <summary>
        /// Quote symbol.
        /// </summary>
        private const char DEF_QUOTE = '"';
        #endregion
        #region Form controls
        private System.Windows.Forms.Label lblWhat;
        private System.Windows.Forms.TextBox findTextBox;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkCase;
        private System.Windows.Forms.GroupBox grpDirection;
        private System.Windows.Forms.RadioButton rbtnUp;
        private System.Windows.Forms.RadioButton rbtnDown;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        #endregion

        #region Class members
        /// <summary>
        /// Last searched text in the control.
        /// </summary>
        private string m_lastSearchedText = string.Empty;

        /// <summary>
        /// Index of last found text in the control.
        /// </summary>
        private int m_lastFoundIndex;

        /// <summary>
        /// Message when text is not found.
        /// </summary>
        private string m_errorMsg = DEF_ERR_MSG;

        /// <summary>
        /// Indicates whether error message must be shown.
        /// </summary>
        private bool m_bShowErrMsg = true;

        /// <summary>
        /// Indicates the HTMLUI control that uses this dialog.
        /// </summary>
        private HTMLUIControl m_ownerHTMLUIControl = null;

        /// <summary>
        /// Text to be searched with the help of the FindDialog.
        /// </summary>
        private string m_searchText = String.Empty;

        #endregion

        #region Class properties

        /// <summary>
        /// Gets the last searched text in the control.
        /// </summary>
        public string LastSearchedText
        {
            get
            {
                return m_lastSearchedText;
            }
        }

        /// <summary>
        /// Gets the index of the last found text in the control.
        /// </summary>
        public int LastFoundIndex
        {
            get
            {
                return m_lastFoundIndex;
            }
        }

        /// <summary>
        /// Gets a value indicating whether forward search is used.
        /// </summary>
        public bool IsForward
        {
            get
            {
                return rbtnDown.Checked;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the case of symbol is significant.
        /// </summary>
        public bool MatchCase
        {
            get
            {
                return chkCase.Checked;
            }
        }

        /// <summary>
        /// Gets or sets the message when text is not found.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return m_errorMsg;
            }
            set
            {
                if (m_errorMsg != value)
                {
                    m_errorMsg = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether error message must be shown.
        /// </summary>
        public bool ShowErrorMessage
        {
            get
            {
                return m_bShowErrMsg;
            }
            set
            {
                if (m_bShowErrMsg != value)
                {
                    m_bShowErrMsg = value;
                }
            }
        }
        #endregion

        #region Class events

        /// <summary>
        /// Raised when 'Find' button is clicked.
        /// </summary>
        public event FindTextEventHandler FindText;
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes a new instance of the FindDialog class
        /// </summary>
        public FindDialog()
        {
            //// Required for Windows Form Designer support

            InitializeComponent();

            CheckEnableState();
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">A bool value indicates whether to dispose or not</param>
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
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblWhat = new System.Windows.Forms.Label();
            this.findTextBox = new System.Windows.Forms.TextBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkCase = new System.Windows.Forms.CheckBox();
            this.grpDirection = new System.Windows.Forms.GroupBox();
            this.rbtnUp = new System.Windows.Forms.RadioButton();
            this.rbtnDown = new System.Windows.Forms.RadioButton();
            this.grpDirection.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblWhat
            // 
            this.lblWhat.Location = new System.Drawing.Point(10, 12);
            this.lblWhat.Name = "lblWhat";
            this.lblWhat.Size = new System.Drawing.Size(56, 16);
            this.lblWhat.TabIndex = 0;
            this.lblWhat.Text = "Fi&nd what:";
            // 
            // findTextBox
            // 
            this.findTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
              | System.Windows.Forms.AnchorStyles.Right)));
            this.findTextBox.Location = new System.Drawing.Point(72, 10);
            this.findTextBox.Name = "findTextBox";
            this.findTextBox.Size = new System.Drawing.Size(192, 20);
            this.findTextBox.TabIndex = 1;
            this.findTextBox.Text = String.Empty;
            this.findTextBox.TextChanged += new System.EventHandler(this.FindTextBox_TextChanged);
            // 
            // btnFind
            // 
            this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFind.Location = new System.Drawing.Point(272, 9);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(76, 22);
            this.btnFind.TabIndex = 4;
            this.btnFind.Text = "&Find Next";
            this.btnFind.Click += new System.EventHandler(this.BtnFind_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(272, 39);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 22);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // chkCase
            // 
            this.chkCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkCase.Location = new System.Drawing.Point(10, 69);
            this.chkCase.Name = "chkCase";
            this.chkCase.Size = new System.Drawing.Size(94, 24);
            this.chkCase.TabIndex = 2;
            this.chkCase.Text = "Match &case";
            // 
            // grpDirection
            // 
            this.grpDirection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDirection.Controls.Add(this.rbtnUp);
            this.grpDirection.Controls.Add(this.rbtnDown);
            this.grpDirection.Location = new System.Drawing.Point(156, 56);
            this.grpDirection.Name = "grpDirection";
            this.grpDirection.Size = new System.Drawing.Size(108, 40);
            this.grpDirection.TabIndex = 3;
            this.grpDirection.TabStop = false;
            this.grpDirection.Text = "Direction";
            // 
            // rbtnUp
            // 
            this.rbtnUp.Location = new System.Drawing.Point(8, 16);
            this.rbtnUp.Name = "rbtnUp";
            this.rbtnUp.Size = new System.Drawing.Size(42, 18);
            this.rbtnUp.TabIndex = 0;
            this.rbtnUp.Text = "&Up";
            // 
            // rbtnDown
            // 
            this.rbtnDown.Checked = true;
            this.rbtnDown.Location = new System.Drawing.Point(52, 16);
            this.rbtnDown.Name = "rbtnDown";
            this.rbtnDown.Size = new System.Drawing.Size(54, 16);
            this.rbtnDown.TabIndex = 0;
            this.rbtnDown.TabStop = true;
            this.rbtnDown.Text = "&Down";
            // 
            // FindDialog
            // 
            this.AcceptButton = this.btnFind;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(354, 104);
            this.ControlBox = false;
            this.Controls.Add(this.grpDirection);
            this.Controls.Add(this.chkCase);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.findTextBox);
            this.Controls.Add(this.lblWhat);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "FindDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Find";
            this.grpDirection.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Opens the dialog for text searching.
        /// </summary>
        /// <param name="control">A Control instance</param>
        public void OpenDialog(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            this.m_ownerHTMLUIControl = control as HTMLUIControl;

            SetState();

            this.Owner = control.FindForm();
            this.Location = GetLocation(control);
            this.Size = new Size(360, 128);

            Show();

            m_lastFoundIndex = 0;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Raises the Find event.
        /// </summary>
        /// <param name="args">Event data.</param>
        protected virtual void OnFindText(FindTextEventArgs args)
        {
            RaiseOnFindText(args);
        }

        /// <summary>
        /// Overloaded. Raised when visibility is changed.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Visible)
            {
                findTextBox.Select();
            }
        }
        #endregion

        #region Class event raisers
        /// <summary>
        /// Raises the Find event.
        /// </summary>
        /// <param name="args">Event data.</param>
        private void RaiseOnFindText(FindTextEventArgs args)
        {
            if (FindText != null)
            {
                FindText(this, args);
            }
        }
        #endregion

        #region Class event handlers
        /// <summary>
        /// Raised when text in text box is changed.
        /// </summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="e">Event data.</param>
        private void FindTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckEnableState();
        }

        /// <summary>
        /// Raised when 'Find' button is clicked.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event data.</param>
        private void BtnFind_Click(object sender, EventArgs e)
        {
            m_lastSearchedText = findTextBox.Text;

            if (findTextBox.Text.Length > 0)
            {
                FindTextEventArgs args = new FindTextEventArgs(this.LastSearchedText, this.LastFoundIndex, rbtnDown.Checked, chkCase.Checked);

                OnFindText(args);

                // Assign index of found text.
                if (args.StartIndex >= 0)
                {
                    m_lastFoundIndex = args.StartIndex;
                }
                else if (this.ShowErrorMessage)
                {
                    MessageBox.Show(this.ErrorMessage + DEF_QUOTE +
                      this.LastSearchedText + DEF_QUOTE);
                }
            }
        }

        /// <summary>
        /// Raises when Close button is clicked.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event data.</param>
        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            this.Visible = false;
        }
        #endregion

        #region Class utility methods

        /// <summary>
        /// Checks the 'Enable' state of the text box.
        /// </summary>
        private void CheckEnableState()
        {
            btnFind.Enabled = findTextBox.Text.Length > 0;
        }

        /// <summary>
        /// Sets the text to be searched in the text field.
        /// </summary>
        /// <param name="text">Text to be searched.</param>
        private void SetSearchedText(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            findTextBox.Text = text;
            findTextBox.SelectAll();
        }

        /// <summary>
        /// Sets the state of all controls in the dialog.
        /// </summary>
        private void SetState()
        {
            if (m_ownerHTMLUIControl.SelectedText.Length > 0)
            {
                m_searchText = m_ownerHTMLUIControl.SelectedText;
            }
            else
            {
                m_searchText = this.LastSearchedText;
            }

            SetSearchedText(m_searchText);

            m_lastFoundIndex = 0;
            chkCase.Checked = false;
            rbtnDown.Checked = true;
        }

        /// <summary>
        /// Calculates a new location for the dialog.
        /// </summary>
        /// <param name="parent">Parent control for dialog.</param>
        /// <returns>New location of the dialog.</returns>
        private Point GetLocation(Control parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            Rectangle controlBound = new Rectangle(
              parent.PointToScreen(parent.Location),
              parent.Size);

            Point controlCenter = new Point(controlBound.Left + controlBound.Width / 2, controlBound.Top + controlBound.Height / 2);

            Point location = new Point(controlCenter.X - this.Width / 2, controlCenter.Y - this.Height / 2);

            return location;
        }
        #endregion
    }
}
