//-------------------------------------------------------------------------------------------------
// <copyright file="Cell.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// In Microsoft SQL Server 2005 Analysis Services (SSAS), a cell represents the unique logical 
    /// intersection of one position from every axis in the cellset. Because every logical intersection 
    /// in a cellset may or may not have a corresponding record in a fact table, not every cell in a 
    /// cellset contains data.
    /// </summary>
    /// <remarks>
    /// A cell is created through GetCell method in AdomdProvider class by passing the index values,
    /// and the cell values are pumped into Pivotcelldescriptor of PivotEngine.
    /// </remarks>
    [Serializable]
    public class Cell
    {
        #region Private Properties
        private PropertyCollection _Properties;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="formattedValue">The formatted value.</param>
        /// <param name="formattedString">The formatted string.</param>
        public Cell(object value, string formattedValue, string formattedString)
        {
            this.Value = value;
            this.FormattedValue = formattedValue;
            this.FormatString = formattedString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        public Cell()
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the formatted value.
        /// </summary>
        /// <value>The formatted value.</value>
        [Description("Gets a formatted cell value."), DefaultValue("")]
        public string FormattedValue { get; set; }

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public string FormatString { get; set; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [DefaultValue((string)null), Description("Gets properties collection.")]
        public PropertyCollection Properties
        {
            get
            {
                if (_Properties == null)
                {
                    _Properties = new PropertyCollection();
                }

                return _Properties;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Description("Gets a cell value."), DefaultValue((string)null)]
        public object Value { get; set; }
        #endregion
    }
}
