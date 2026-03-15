#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Represents the Cell information.
    /// </summary>
    [DataContract]
    public class Cell
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        public Cell()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="formattedValue">The formatted value.</param>
        public Cell(object value, string formattedValue, string formattedString)
        {
            this.Value = value;
            this.FormattedValue = formattedValue;
            this.FormatString = formattedString;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the formatted value.
        /// </summary>
        /// <value>The formatted value.</value>
        [Description("Gets a formatted cell value."), DefaultValue("")]
        [DataMember]
        public string FormattedValue { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Description("Gets a cell value."), DefaultValue((string)null)]
        [DataMember]
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        [DataMember]
        public string FormatString { get; set; }
        #endregion
    }
}
