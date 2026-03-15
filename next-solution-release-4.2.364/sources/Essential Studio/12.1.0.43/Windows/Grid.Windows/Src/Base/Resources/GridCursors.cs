//-------------------------------------------------------------------------------------------------
// <copyright file="GridCursors.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;

namespace Syncfusion.Windows.Forms.Grid
{ 
    /// <summary>
    /// Provides you with default cursors for the grid.
    /// </summary>
    public sealed class GridCursors
    {
        static string cursorNS = AssemblyInfo.RootNamespace + @".Base.Resources.";

        [ThreadStaticAttribute] static Cursor selectColumnCursor = null;
        [ThreadStaticAttribute] static Cursor selectRowCursor = null;
        [ThreadStaticAttribute] static Cursor selectRowRTLCursor = null;
        [ThreadStaticAttribute] static Cursor rowHeightCursor = null;
        [ThreadStaticAttribute] static Cursor columnWidthCursor = null;
        [ThreadStaticAttribute] static Cursor dragSelectionCursor = null;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridCursors()
            : base()
        {
        }

        static Cursor GetCursor(string cursorName)  
        {
            Cursor cursor = null;

            try
            {
                Type type = typeof(GridCursors);
                Stream stream = type.Module.Assembly.GetManifestResourceStream(cursorNS + cursorName);
                cursor = new Cursor(stream);
            }  
            catch (System.Exception exception)
            {
                MessageBoxAdv.Show(exception.Message);
                throw;
            }  

            return cursor;
        }

        /// <summary>
        /// Gets or sets cursor for resizing columns.
        /// </summary>
        public static Cursor ColumnWidthCursor
        {
            get
            {
                if (GridCursors.columnWidthCursor == null)
                {
                    GridCursors.columnWidthCursor = GetCursor(@"SFWIDTH.CUR");
                }

                return GridCursors.columnWidthCursor;
            }

            set
            {
                GridCursors.columnWidthCursor = value;
            }
        }

        /// <summary>
        /// Gets or sets cursor for resizing rows.
        /// </summary>
        public static Cursor RowHeightCursor
        {
            get
            {
                if (GridCursors.rowHeightCursor == null)
                {
                    GridCursors.rowHeightCursor = GetCursor(@"SFHEIGHT.CUR");
                }

                return GridCursors.rowHeightCursor;
            }

            set
            {
                GridCursors.rowHeightCursor = value;
            }
        }

        /// <summary>
        /// Gets or sets cursor for selecting columns.
        /// </summary>
        public static Cursor SelectColumnCursor
        {
            get
            {
                if (GridCursors.selectColumnCursor == null)
                {
                    GridCursors.selectColumnCursor = GetCursor(@"SFCOLUMN.CUR");
                }

                return GridCursors.selectColumnCursor;
            }

            set
            {
                GridCursors.selectColumnCursor = value;
            }
        }

        /// <summary>
        /// Gets or sets cursor for selecting rows.
        /// </summary>
        public static Cursor SelectRowCursor
        {
            get
            {
                if (GridCursors.selectRowCursor == null)
                {
                    GridCursors.selectRowCursor = GetCursor(@"SFROW.CUR");
                }

                return GridCursors.selectRowCursor;
            }

            set
            {
                GridCursors.selectRowCursor = value;
            }
        }

        /// <summary>
        /// Gets or sets cursor for selecting rows.
        /// </summary>
        public static Cursor SelectRowRTLCursor
        {
            get
            {
                if (GridCursors.selectRowRTLCursor == null)
                {
                    GridCursors.selectRowRTLCursor = GetCursor(@"SFROWRTL.CUR");
                }

                return GridCursors.selectRowRTLCursor;
            }

            set
            {
                GridCursors.selectRowRTLCursor = value;
            }
        }

        /// <summary>
        /// Gets or sets cursor for dragging a selection of columns or rows.
        /// </summary>
        public static Cursor DragSelectionCursor
        {
            get
            {
                if (GridCursors.dragSelectionCursor == null)
                {
                    GridCursors.dragSelectionCursor = GetCursor(@"SFSELDRG.CUR");
                }

                return GridCursors.dragSelectionCursor;
            }

            set
            {
                GridCursors.dragSelectionCursor = value;
            }
        }
    }
}
