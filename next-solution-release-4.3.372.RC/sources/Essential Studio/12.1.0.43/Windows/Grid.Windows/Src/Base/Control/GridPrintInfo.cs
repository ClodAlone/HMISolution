//-------------------------------------------------------------------------------------------------
// <copyright file="GridPrintInfo.cs" company="syncfusion">
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
    /// Holds temporary information related to printing. This class will change in future versions.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridPrintInfo
    {
        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int m_nPrintTopRow = 0;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int m_nPrintLeftCol = 0;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public ArrayList m_awPageFirstCol = null;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public ArrayList m_awPageFirstRow = null;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int m_nCurrentPageColIndex = 0;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int m_nCurrentPageRowIndex = 0;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public GridCountRecordsBehavior m_nPrintHandleRecordCount = GridCountRecordsBehavior.CountPrint;

        /// <summary>
        /// OnGridPrint will check m_bPrintPaintMsg and redraw the whole grid later.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool m_bPrintPaintMsg = false;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool m_bPrintCurSelOnly = false;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridPrintInfo()
            : base()
        {
        }
    }
}
