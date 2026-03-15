//-------------------------------------------------------------------------------------------------
// <copyright file="FieldDialogBox.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using Syncfusion.Windows.Forms.Grid;

    /// <summary>
    /// For internal use.
    /// </summary>
    public partial class FieldDialogBox : Form
    {
        /// <summary>
        /// For internal use.
        /// </summary>
        public int[] count;

        private int minimumWidth = 180;

        internal int MinimumWidth
        {
            get { return this.minimumWidth; }
        }

        /// <summary>
        /// Constructor for FieldDialogBox.
        /// </summary>
        public FieldDialogBox()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.InitializeComponent();
        }
    }
}
