//-------------------------------------------------------------------------------------------------
// <copyright file="GridDropDownEditPartControl.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

#define RICHTEXT

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the text box that is displayed in the user input field for 
    /// a combo box. 
    /// </summary>
    [ToolboxItem(false)]
#if RICHTEXT
    public class GridDropDownEditPartControl: GridTextBoxControl
#else
    public class GridDropDownOriginalTextBoxControl: GridOriginalTextBoxControl
#endif
    {
        /// <summary>
        /// Initializes a <see cref="GridDropDownEditPartControl"/> and associates it with a <see cref="GridTextBoxCellRenderer"/>.
        /// </summary>
        /// <param name="parent">The parent for this text box part.</param>
#if RICHTEXT
        public GridDropDownEditPartControl(GridTextBoxCellRenderer parent)
#else
        public GridDropDownOriginalTextBoxControl(GridTextBoxCellRenderer parent)
#endif
            : base(parent)
        {
        }
    }
}
