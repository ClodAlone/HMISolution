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

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Class that holds the FieldTreeDialogBox methods
    /// </summary>
    public partial class FieldTreeDialogBox : Form
    {
        /// <summary>
        /// For internal use.
        /// </summary>
        public int[] count;

        private int minimumWidth = 180;

        internal int MinimumWidth
        {
            get { return minimumWidth; }
        }

        /// <summary>
        /// Constructor for FieldTreeDialogBox.
        /// </summary>
        public FieldTreeDialogBox()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeComponent();
        }
    }
}
