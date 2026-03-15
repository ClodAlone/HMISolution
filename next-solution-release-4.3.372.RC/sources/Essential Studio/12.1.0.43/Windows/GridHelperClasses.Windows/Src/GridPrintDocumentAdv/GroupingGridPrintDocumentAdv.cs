//-------------------------------------------------------------------------------------------------
// <copyright file="GroupingGridPrintDocumentAdv.cs" company="Syncfusion">
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
    using System.ComponentModel;
    using System.Collections;
    using System.Diagnostics;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Printing;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;

    using Syncfusion.Diagnostics;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Windows.Forms.Grid.Grouping;
    using Syncfusion.ComponentModel;

    /// <summary>
    /// PrintDocument class for printing GridGroupingControl
    /// </summary>
    public class GroupingGridPrintDocumentAdv : GridPrintDocumentAdv
    {
        GroupingGridPrintDocumentAdv(GridControlBase grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Constructor for GroupingGridPrintDocumentAdv.
        /// </summary>
        /// <param name="grid">The grouping grid.</param>
        public GroupingGridPrintDocumentAdv(GridGroupingControl grid) : this(grid.TableControl)
        {
            int fixedRows = 0;

            if (grid.TopLevelGroupOptions.ShowCaption)
            {
                fixedRows++;
            }

            if (grid.TopLevelGroupOptions.ShowColumnHeaders)
            {
                fixedRows++;
            }

            if (grid.TopLevelGroupOptions.ShowAddNewRecordBeforeDetails)
            {
                fixedRows++;
            }

            this.fixedRowIndex = fixedRows;
        }
    }
}
