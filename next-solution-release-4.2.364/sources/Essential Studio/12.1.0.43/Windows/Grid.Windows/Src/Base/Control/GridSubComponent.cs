//-------------------------------------------------------------------------------------------------
// <copyright file="GridSubComponent.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Security;
using System.Security.Permissions;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// A base class for objects that are associated with a <see cref="GridControlBase"/>
    /// </summary>
    public class GridSubComponent : NonFinalizeDisposable
    {
        GridControlBase grid;

        /// <summary>
        /// Initializes a <see cref="GridSubComponent"/> and associates it with a grid.
        /// </summary>
        /// <param name="grid">The grid control this object is associated with.</param>
        protected GridSubComponent(GridControlBase grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Gets the grid control this object is associated with.
        /// </summary>
        public GridControlBase Grid
        {
            get
            {
                return grid;
            }
        }
    }
}