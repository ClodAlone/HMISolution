#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.GridCommon;
using System.Windows.Interop;

namespace Syncfusion.Windows.Controls.Cells
{

    /// <summary>
    /// Provides you with default cursors for the grid.
    /// </summary>
    public sealed class CellCursors
    {
        static string cursorNS = AssemblyInfo.RootNamespace + @".Resources.";

        [ThreadStaticAttribute]
        static Cursor selectColumnCursor = null;
        [ThreadStaticAttribute]
        static Cursor selectRowCursor = null;
        [ThreadStaticAttribute]
        static Cursor selectRowRTLCursor = null;
        [ThreadStaticAttribute]
        static Cursor rowHeightCursor = null;
        [ThreadStaticAttribute]
        static Cursor columnWidthCursor = null;
        [ThreadStaticAttribute]
        static Cursor dragSelectionCursor = null;
        [ThreadStaticAttribute]
        static Cursor resizeHiddenColumnCursor = null;
        [ThreadStaticAttribute]
        static Cursor resizeHiddenRowCursor = null;

        static Cursor GetCursor(string cursorName)
        {
            Cursor cursor = null;

            try
            {
                Type type = typeof(CellCursors);
                Stream stream = type.Module.Assembly.GetManifestResourceStream(cursorNS + cursorName);
                cursor = new Cursor(stream);
            }
            catch (System.Exception exception)
            {
                MessageBox.Show(exception.Message);
                throw exception;
            }

            return cursor;
        }

        /// <summary>
        /// Cursor for resizing columns.
        /// </summary>
        public static Cursor ResizeHiddenColumnCursor
        {
            get
            {
                if (CellCursors.resizeHiddenColumnCursor == null)
                {
                    if (!BrowserInteropHelper.IsBrowserHosted)
                    {
                        CellCursors.resizeHiddenColumnCursor = GetCursor(@"ResizeHiddenColumn.cur");
                    }
                    else
                    {
                        CellCursors.resizeHiddenColumnCursor = Cursors.SizeWE;
                    }
                }
                return CellCursors.resizeHiddenColumnCursor;
            }
            set
            {
                CellCursors.resizeHiddenColumnCursor = value;
            }
        }


        /// <summary>
        /// Cursor for resizing columns.
        /// </summary>
        public static Cursor ResizeHiddenRowCursor
        {
            get
            {
                if (CellCursors.resizeHiddenRowCursor == null)
                {
                    if (!BrowserInteropHelper.IsBrowserHosted)
                    {
                        CellCursors.resizeHiddenRowCursor = GetCursor(@"ResizeHiddenRow.cur");
                    }
                    else
                    {
                        CellCursors.resizeHiddenRowCursor = Cursors.ScrollNS;
                    }
                }
                return CellCursors.resizeHiddenRowCursor;
            }
            set
            {
                CellCursors.resizeHiddenRowCursor = value;
            }
        }
       

        /// <summary>
        /// Cursor for resizing columns.
        /// </summary>
        public static Cursor ResizeWidthCursor
        {
            get
            {
                if (CellCursors.columnWidthCursor == null)
                {
                    if (!BrowserInteropHelper.IsBrowserHosted)
                    {
                        CellCursors.columnWidthCursor = GetCursor(@"ResizeWidth.cur");
                    }
                    else
                    {
                        CellCursors.columnWidthCursor = Cursors.SizeWE;
                    }
                }
                return CellCursors.columnWidthCursor;
            }
            set
            {
                CellCursors.columnWidthCursor = value;
            }
        }

        /// <summary>
        /// Cursor for resizing rows.
        /// </summary>
        public static Cursor ResizeHeightCursor
        {
            get
            {
                if (CellCursors.rowHeightCursor == null)
                {
                    if (!BrowserInteropHelper.IsBrowserHosted)
                    {
                        CellCursors.rowHeightCursor = GetCursor(@"ResizeHeight.cur");
                    }
                    else
                    {
                        CellCursors.rowHeightCursor = Cursors.SizeNS;
                    }
                }
                return CellCursors.rowHeightCursor;
            }
            set
            {
                CellCursors.rowHeightCursor = value;
            }
        }

        /// <summary>
        /// Cursor for selecting columns.
        /// </summary>
        public static Cursor SelectColumnCursor
        {
            get
            {
                if (CellCursors.selectColumnCursor == null)
                {
                    if (!BrowserInteropHelper.IsBrowserHosted)
                    {
                        CellCursors.selectColumnCursor = GetCursor(@"SelectColumn.cur");
                    }
                    else
                    {
                        CellCursors.selectColumnCursor = Cursors.Arrow;
                    }
                }
                return CellCursors.selectColumnCursor;
            }
            set
            {
                CellCursors.selectColumnCursor = value;
            }
        }

        /// <summary>
        /// Cursor for selecting rows.
        /// </summary>
        public static Cursor SelectRowCursor
        {
            get
            {
                if (CellCursors.selectRowCursor == null)
                    CellCursors.selectRowCursor = GetCursor(@"SelectRow.cur");
                return CellCursors.selectRowCursor;
            }
            set
            {
                CellCursors.selectRowCursor = value;
            }
        }

        /// <summary>
        /// Cursor for selecting rows.
        /// </summary>
        public static Cursor SelectRowRTLCursor
        {
            get
            {
                if (CellCursors.selectRowRTLCursor == null)
                    CellCursors.selectRowRTLCursor = GetCursor(@"SelectRowRightToLeft.cur");
                return CellCursors.selectRowRTLCursor;
            }
            set
            {
                CellCursors.selectRowRTLCursor = value;
            }
        }

        /// <summary>
        /// Cursor for dragging a selection of columns or rows.
        /// </summary>
        public static Cursor DragSelectionCursor
        {
            get
            {
                if (CellCursors.dragSelectionCursor == null)
                    CellCursors.dragSelectionCursor = GetCursor(@"DragSelection.cur");
                return CellCursors.dragSelectionCursor;
            }
            set
            {
                CellCursors.dragSelectionCursor = value;
            }
        }

    }
}
