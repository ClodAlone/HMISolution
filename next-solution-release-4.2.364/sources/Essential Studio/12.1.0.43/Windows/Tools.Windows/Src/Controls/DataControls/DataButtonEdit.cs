#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// CurrencyEdit class encapsulates a CurrencyTextBox control and
    /// adds the ability to drop down a popup calculator.
    /// </summary>
    [
    ToolboxItem(false),
    Syncfusion.Documentation.DocumentationExclude()
    ]
    public class DataButtonEdit : ButtonEdit
    {
        /// <summary>
        /// The left end button.
        /// </summary>
        private ButtonEditChildButton buttonLeftEnd;

        /// <summary>
        /// The left button.
        /// </summary>
        private ButtonEditChildButton buttonLeft;

        /// <summary>
        /// The right button.
        /// </summary>
        private ButtonEditChildButton buttonRight;

        /// <summary>
        /// The right end button.
        /// </summary>
        private ButtonEditChildButton buttonRightEnd;

        /// <summary>
        /// The datasource.
        /// </summary>
        private object dataSource;

        private BindingManagerBase dataManager;

        /// <summary>
        /// Initializes a new instance of the DataButtonEdit class.
        /// </summary>
        public DataButtonEdit()
        {
            this.buttonLeftEnd = new ButtonEditChildButton();
            this.buttonLeft = new ButtonEditChildButton();
            this.buttonRight = new ButtonEditChildButton();
            this.buttonRightEnd = new ButtonEditChildButton();
            this.SuspendLayout();

            this.Buttons.Add(this.buttonLeftEnd);
            this.Buttons.Add(this.buttonLeft);
            this.Buttons.Add(this.buttonRight);
            this.Buttons.Add(this.buttonRightEnd);
            this.Controls.AddRange(new System.Windows.Forms.Control[] { this.buttonRightEnd, this.buttonRight, this.buttonLeft, this.buttonLeftEnd });
            this.SelectionLength = 0;
            this.SelectionStart = 0;
            this.ShowTextBox = true;
            this.Size = new System.Drawing.Size(200, 22);
            this.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;

            // buttonLeftEnd
            this.buttonLeftEnd.BackColor = System.Drawing.SystemColors.Control;
            this.buttonLeftEnd.ButtonAlign = ButtonAlignment.Left;
            this.buttonLeftEnd.ButtonEditParent = this;
            this.buttonLeftEnd.ButtonType = ButtonTypes.LeftEnd;
            this.buttonLeftEnd.Name = "buttonLeftEnd";
            this.buttonLeftEnd.PreferredWidth = 16;
            this.buttonLeftEnd.TabIndex = 1;
            this.buttonLeftEnd.Click += new System.EventHandler(this.ButtonLeftEnd_Click);

            // buttonLeft
            this.buttonLeft.BackColor = System.Drawing.SystemColors.Control;
            this.buttonLeft.ButtonAlign = ButtonAlignment.Left;
            this.buttonLeft.ButtonEditParent = this;
            this.buttonLeft.ButtonType = ButtonTypes.Left;
            this.buttonLeft.Name = "buttonLeft";
            this.buttonLeft.PreferredWidth = 16;
            this.buttonLeft.TabIndex = 2;
            this.buttonLeft.Click += new System.EventHandler(this.ButtonLeft_Click);

            // buttonRight
            this.buttonRight.BackColor = System.Drawing.SystemColors.Control;
            this.buttonRight.ButtonAlign = ButtonAlignment.Right;
            this.buttonRight.ButtonEditParent = this;
            this.buttonRight.ButtonType = ButtonTypes.Right;
            this.buttonRight.Name = "buttonRight";
            this.buttonRight.PreferredWidth = 16;
            this.buttonRight.TabIndex = 3;
            this.buttonRight.Click += new System.EventHandler(this.ButtonRight_Click);
        
            // buttonRightEnd
            this.buttonRightEnd.BackColor = System.Drawing.SystemColors.Control;
            this.buttonRightEnd.ButtonAlign = ButtonAlignment.Right;
            this.buttonRightEnd.ButtonEditParent = this;
            this.buttonRightEnd.ButtonType = ButtonTypes.RightEnd;
            this.buttonRightEnd.Name = "buttonRightEnd";
            this.buttonRightEnd.PreferredWidth = 16;
            this.buttonRightEnd.TabIndex = 4;
            this.buttonRightEnd.Click += new System.EventHandler(this.ButtonRightEnd_Click);

            this.Controls.AddRange(new System.Windows.Forms.Control[] { this.buttonLeftEnd, this.buttonLeft, this.buttonRight, this.buttonRightEnd });  
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Sets the status of the data buttons depending on position.
        /// </summary>
        public void SetButtonsStatus()
        {
            if (this.DataManager != null)
            {
            }
        }

        private void ButtonLeftEnd_Click(object sender, System.EventArgs e)
        {
            if (this.DataManager != null)
                this.DataManager.Position = 0;
        }

        private void ButtonLeft_Click(object sender, System.EventArgs e)
        {
            if (this.DataManager != null)
                this.DataManager.Position -= 1;
        }

        private void ButtonRight_Click(object sender, System.EventArgs e)
        {
            if (this.DataManager != null)
                this.DataManager.Position += 1;
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="newPosition">The new position to move to.</param>
        public void SetPosition(int newPosition)
        {
            try
            {
                this.DataManager.Position = newPosition;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets or sets the data manager.
        /// </summary>
        public BindingManagerBase DataManager
        {
            get
            {
                if (this.dataManager != null)
                    return this.dataManager;

                if (this.ParentForm != null && this.DataSource != null)
                    this.dataManager = this.ParentForm.BindingContext[this.DataSource];
                return this.dataManager;
            }

            set
            {
                this.dataManager = value;
            }
        }

        private void ButtonRightEnd_Click(object sender, System.EventArgs e)
        {
            if (this.dataSource is IList)
            {
                if (this.DataManager != null)
                    this.DataManager.Position = ((IList)this.DataSource).Count;
            }
            else if (this.dataSource is IListSource)
            {
                IListSource listsource = this.dataSource as IListSource;
                IList list = listsource.GetList();

                if (this.DataManager != null)
                    this.DataManager.Position = ((ICollection)list).Count;
            }
        }

        /// <summary>
        /// Gets or sets the data source that the grid is displaying data for.
        /// </summary>
        [SRDescription(@"Indicates the source of data for the DataButtonEdit"),
        DefaultValue(null),
        SRCategory(@"Data"),
        RefreshProperties(RefreshProperties.Repaint),
        TypeConverter(@"System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        public object DataSource
        {
            get
            {
                return this.dataSource;
            }

            set
            {
                if (value != null && (value is IList || value is IListSource))
                {
                    this.dataSource = value;
                }
                return;
            }
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">Bool disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
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

        }
        #endregion
    }
}
