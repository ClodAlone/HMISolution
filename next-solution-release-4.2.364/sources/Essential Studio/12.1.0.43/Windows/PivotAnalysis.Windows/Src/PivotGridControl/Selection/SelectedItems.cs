#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    /// <summary>
    /// This class defines the IEnumerable collection of SelectedItem in Grid
    /// </summary>
    public class SelectedItems : List<SelectedItem>
    {
        #region [ Initialize/Finalize ]
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedItems"/> class.
        /// </summary>
        public SelectedItems()
        {

        }

        #endregion
    }

    /// <summary>
    /// This class represents the Column, Row and Value of SelectedItem
    /// </summary>
    public class SelectedItem
    {
        #region [ Initialize/Finalize ]
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedItem"/> class.
        /// </summary>
        public SelectedItem()
        {
            this.RowList = new List<string>();
            this.ColumnList = new List<string>();
        }

        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>The column.</value>
        public string Column
        {
            get
            {
                string column = string.Empty;
                foreach (var item in this.ColumnList)
                {
                    if (column != string.Empty)
                        column += " - ";
                    column += item;
                }
                return column;
            }
        }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>The row.</value>
        public string Row
        {
            get
            {
                string row = string.Empty;
                foreach (var item in this.RowList)
                {
                    if (row != string.Empty)
                        row += " - ";
                    row += item;
                }
                return row;
            }
        }

        /// <summary>
        /// Gets or sets the list of column of Selected Item.
        /// </summary>
        /// <value>The column.</value>
        public List<string> ColumnList { get; internal set; }

        /// <summary>
        /// Gets or sets the list of row of Selected Item.
        /// </summary>
        /// <value>The row.</value>
        public List<string> RowList { get; internal set; }

        /// <summary>
        /// Gets or sets the value of Selected Item.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; internal set; }

        /// <summary>
        /// Gets or sets the formatted value.
        /// </summary>
        /// <value>The formatted value.</value>
        public string FormattedValue { get; internal set; }

        #endregion

        #region [ Overrides ]
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Column : " + this.Column + " ; " + "Row : " + this.Row + " ; " + "Value : " + this.Value;
        }

        #endregion
    }
}
