#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#region File Using Directives
using System;
#endregion

namespace Syncfusion.XlsIO
{
    /// <summary>
    /// Represents a Sparkline.The Sparkline object is a member of the Sparklines Collection.
    /// The Sparklines collection contains all the sparkline objects in a worksheet.    
    /// </summary>
    public class Sparkline : ISparkline
    {
        #region Fields
        private IRange m_dataRange;
        private IRange m_referenceRange;
        #endregion

        #region Properties
        /// <summary>
        ///Represents the data range of the sparkline.
        /// </summary>
        /// <value>The data range.</value>
        /// <exception cref="ArgumentOutOfRange">
        /// if the value.Rows.Length is not equal to 1.
        /// </exception>
        public IRange DataRange
        {
            get
            {
                return m_dataRange;
            }
            set
            {
                if ( value.LastRow - value.Row == 1 && value.LastColumn - value.Column == 1 )
                    throw new ArgumentOutOfRangeException("DataRange", "The range should not exceed single row.");

                m_dataRange = value;
            }
        }
        /// <summary>
        /// Represents the reference range of the sparkline.
        /// </summary>
        /// <value>The reference range.</value>
        /// <exception cref="ArgumentOutOfRange">
        /// if the value.Rows.length and value.Columns.Length is not equal to 1;
        /// </exception>
        public IRange ReferenceRange
        {
            get
            {
                return m_referenceRange;
            }
            set
            {
                if (!(value.Rows.Length == 1 && value.Columns.Length == 1))
                    throw new ArgumentOutOfRangeException("ReferenceRange", "Location reference is not valid because the cells are not all in the same column or row.");

                m_referenceRange = value;
            }
        }
        /// <summary>
        /// Gets the column index of a sparkline.
        /// </summary>
        /// <value>The column index.</value>
        public int Column
        {
            get
            {
                return m_dataRange.Column;
            }
        }
        /// <summary>
        /// Gets the row index of a sparkline.
        /// </summary>
        /// <value>The row index.</value>
        public int Row
        {
            get
            {
                return m_dataRange.Row;
            }
        }
        #endregion
        
    }
}
