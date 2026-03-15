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

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Editor to modify page scale.
    /// </summary>
    public partial class PageScaleDialog : Form
    {
        /// <summary>
        /// Gets or sets the page scale.
        /// </summary>
        /// <value>The page scale.</value>
        public PageScale PageScale
        {
            get { return drawingScaleControl1.PageScale; }
            set { drawingScaleControl1.PageScale = value; }
        }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public PageSize PageSize
        {
            get { return drawingScaleControl1.PageSize; }
            set { drawingScaleControl1.PageSize = value; }
        }

        /// <summary>
        /// Gets or sets the measure units.
        /// </summary>
        /// <value>The measure units.</value>
        public MeasureUnits MeasureUnits
        {
            get { return drawingScaleControl1.MeasureUnits; }
            set { drawingScaleControl1.MeasureUnits = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageScaleDialog"/> class.
        /// </summary>
        public PageScaleDialog()
        {
            InitializeComponent();
        }
    }
}