//-------------------------------------------------------------------------------------------------
// <copyright file="GridGroupingCursors.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
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

namespace Syncfusion.Windows.Forms.Grid.Grouping
{ 
    /// <summary>
    /// Provides you with default cursors for the grid.
    /// </summary>
    public sealed class GridGroupingCursors
    {
        static string cursorNS = AssemblyInfo.RootNamespace + @".Resources.";

        [ThreadStaticAttribute] static Cursor removeCursor = null;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridGroupingCursors()
        {
        }

        static Cursor GetCursor(string cursorName)  
        {
            Cursor cursor = null;

            try
            {
                Type type = typeof(AssemblyInfo);
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
        /// Cursor for removing columns.
        /// </summary>
        public static Cursor RemoveCursor
        {
            get
            {
                if (removeCursor == null)
                {
                    removeCursor = GetCursor("SFREMOVE.CUR");
                }

                return removeCursor;
            }

            set
            {
                removeCursor = value;
            }
        }
    }
}
