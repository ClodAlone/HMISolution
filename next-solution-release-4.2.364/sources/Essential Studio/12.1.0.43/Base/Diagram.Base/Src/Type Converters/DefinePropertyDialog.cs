#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Class for defining new property dialog
    /// </summary>
    public partial class DefinePropertyDialog : Form
    {
        #region members
        /// <summary>
        /// Dynamic property data
        /// </summary>
        private Dictionary<string, object> propertyBag = null;        
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the dynamic property data
        /// </summary>
        public Dictionary<string, object> PropertyBag
        {
            get
            {
                if (propertyBag == null)
                    propertyBag = new Dictionary<string, object>();
                return propertyBag;
            }
            set
            {
                propertyBag = value;
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinePropertyDialog"/> class.
        /// </summary>
        public DefinePropertyDialog()
        {
            InitializeComponent();
        }

        #region Events
        private void DefinePropertyDialog_Load(object sender, EventArgs e)
        {
            comboType.SelectedIndex = 0;
            txtName.Focus();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            AddProperty();
        }        
        #endregion

        #region private methods
        /// <summary>
        /// Adds the new property to the dynamic property collection data.
        /// </summary>
        private void AddProperty()
        {
            if (txtName.Text == string.Empty)
            {
                ShowError("Property name must not be left blank");
                txtName.Focus();                
                return;
            }
            if (txtValue.Text == string.Empty)
            {
                ShowError(" Value must not be left blank");                
                return;
            }
            switch (comboType.SelectedItem.ToString())
            {
                case "String":
                    this.PropertyBag.Add(txtName.Text, txtValue.Text);
                    this.DialogResult = DialogResult.OK;
                    break;
                case "Integer":
                    int iResult;
                    if (int.TryParse(txtValue.Text, out iResult))
                    {
                        this.PropertyBag.Add(txtName.Text, iResult);
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        ShowError("Value must be of type integer");
                    }
                    break;
                case "Single":
                    float fResult;
                    if (float.TryParse(txtValue.Text, out fResult))
                    {
                        this.PropertyBag.Add(txtName.Text, fResult);
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        ShowError("Value must be of type single.");
                    }
                    break;
                case "Double":
                    double dResult;
                    if (double.TryParse(txtValue.Text, out dResult))
                    {
                        this.PropertyBag.Add(txtName.Text, dResult);
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        ShowError("Value must be of type double");
                    }
                    break;
                case "Boolean":
                    bool bResult;
                    if (bool.TryParse(txtValue.Text, out bResult))
                    {
                        this.PropertyBag.Add(txtName.Text, bResult);
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        ShowError("Value must be of type boolean.");
                    }
                    break;
            }
        }

        /// <summary>
        /// Throws error when wrong value entered for a property
        /// </summary>
        /// <param name="errorMsg">Error message</param>
        private void ShowError(string errorMsg)
        {
            MessageBox.Show(errorMsg);
            txtValue.Focus();
            this.DialogResult = DialogResult.None;
        }
        
        #endregion
    }
}
