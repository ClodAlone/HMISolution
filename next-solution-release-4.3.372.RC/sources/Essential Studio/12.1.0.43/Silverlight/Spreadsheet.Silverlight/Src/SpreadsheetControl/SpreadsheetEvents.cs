#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.XlsIO;
using System.Collections.Generic;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    

    public delegate void WorkSheetAddingEventHandler(object sender,WorkSheetAddingEventArgs args);

    public class WorkSheetAddingEventArgs : EventArgs
    {
        public WorkSheetAddingEventArgs()
            : base()
        {

        }

        public string SheetName
        {
            get;
            set;
        }

        public bool Cancel
        {
            get;
            set;
        }
    }

    public delegate void WorkSheetAddedEventHandler(object sender, WorkSheetAddedEventArgs args);

    public class WorkSheetAddedEventArgs : EventArgs
    {
        public WorkSheetAddedEventArgs()
        {

        }
        public string SheetName
        {
            get;
            set;
        }

        public ExcelProperties ExcelProperties
        {
            get;
            set;
        }
    }

    public delegate void WorkbookLoadedEventHandler(object sender, WorkbookLoadedEventArgs args);

    public class WorkbookLoadedEventArgs : EventArgs
    {
        public WorkbookLoadedEventArgs()
            : base()
        {

        }


        public WorkbookLoadedEventArgs(IWorkbook Workbook, List<SpreadsheetGrid> GridCollection)
            : base()
        {
            this.Workbook = Workbook;
            this.GridCollection = GridCollection;
        }

        /// <summary>
        /// Loaded Workbook
        /// </summary>
        public IWorkbook Workbook
        {
            get;
            internal set;
        }

        /// <summary>
        /// Spreadsheet Grid Collection
        /// </summary>
        public List<SpreadsheetGrid> GridCollection
        {
            get;
            internal set;
        }
    }
}
