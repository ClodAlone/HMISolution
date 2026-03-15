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
    /// Class for PropertyBag dialog.
    /// </summary>
    public partial class PropertyBagDialog : Form
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
        public Dictionary<string,object> PropertyBag
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
        /// Initializes a new instance of the <see cref="PropertyBagDialog"/> class.
        /// </summary>
        public PropertyBagDialog()
        {
            InitializeComponent();
        }

        #region Events
        private void PropertyBagDialog_Load(object sender, EventArgs e)
        {
            propertyGrid.SelectedObject = new DictionaryPropertyGridAdapter(this.PropertyBag);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DefinePropertyDialog dialog = new DefinePropertyDialog();
            dialog.PropertyBag = this.propertyBag;

            dialog.ShowDialog();
            propertyGrid.Refresh();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            this.PropertyBag.Remove( propertyGrid.SelectedGridItem.Label);
            propertyGrid.Refresh();
        }
        #endregion
    }
}
