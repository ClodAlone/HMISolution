//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelInsertRangeOptions.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Holds additional information for the <see cref="GridModelRowColOperations.InsertRange(int,int)"/> command such as cell contents, row, and column
    /// sizes, hidden state, and covered cells state.
    /// </summary>
    public class GridModelInsertRangeOptions
    {
        /// <summary>
        /// A <see cref="GridStyleInfoStoreTable"/> with cell contents.
        /// </summary>
        internal GridStyleInfoStoreTable data = null;

        /// <summary>
        /// An array with row or column sizes.
        /// </summary>
        internal int/*float*/[] rowColSizes = null;

        /// <summary>
        /// An array with hidden state of rows or columns.
        /// </summary>
        internal bool[] rowColHide = null;

        /// <summary>
        /// A <see cref="GridRangeInfoList"/> with covered cells.
        /// </summary>
        internal GridRangeInfoList covered = null;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridModelInsertRangeOptions()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets <see cref="GridStyleInfoStoreTable"/> with cell contents.
        /// </summary>
        public GridStyleInfoStoreTable Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }

        /// <summary>
        /// Gets or sets an array with row or column sizes.
        /// </summary>
        public int[] RowColSizes
        {
            get
            {
                return rowColSizes;
            }

            set
            {
                rowColSizes = value;
            }
        }

        /// <summary>
        /// Gets or sets an array with hidden state of rows or columns.
        /// </summary>
        public bool[] RowColHide
        {
            get
            {
                return rowColHide;
            }

            set
            {
                rowColHide = value;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="GridRangeInfoList"/> with covered cells.
        /// </summary>
        public GridRangeInfoList Covered
        {
            get
            {
                return covered;
            }

            set
            {
                covered = value;
            }
        }
    }
}
