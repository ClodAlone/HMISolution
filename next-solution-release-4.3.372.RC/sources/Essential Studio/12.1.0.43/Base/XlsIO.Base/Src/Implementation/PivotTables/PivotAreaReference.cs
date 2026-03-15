#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    /// <summary>
    /// Represents a set of selected fields and selected items within those fields
    /// </summary>
    internal class PivotAreaReference
    {
        #region Members
        /// <summary>
        /// Represents the Subtotal type
        /// </summary>
        private PivotSubtotalTypes m_subtotal;
        /// <summary>
        /// Specifies a boolean value that indicates whether the item is referred to by position rather
        ///than item index.
        /// </summary>
        private bool m_bIsReferByPosition;
        /// <summary>
        /// Specifies the number of item indexes in the collection of indexes
        /// </summary>
        private int m_iCount;
        /// <summary>
        /// Specifies the index of the field to which this filter refers. A value of -2 indicates the 'data'
        ///field.
        /// </summary>
        private int m_iFieldIndex;
        /// <summary>
        /// Specifies a boolean value that indicates whether the item is referred to by a relative
        ///reference rather than an absolute reference
        /// </summary>
        private bool m_bIsRelativeReference;
        /// <summary>
        /// Specifies a boolean value that indicates whether this field has selection. This attribute is
        ///used when the PivotTable is in Outline view.
        /// </summary>
        private bool m_bIsSelected;
        /// <summary>
        /// Represents the indexes of the selected fileds
        /// </summary>
        private List<int> m_iIndexes;
        #endregion

        #region Properties
        /// <summary>
        /// Represents the Subtotal type
        /// </summary>
        public PivotSubtotalTypes Subtotal
        {
            get
            {
                return m_subtotal;
            }
            set
            {
                m_subtotal = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether the item is referred to by position rather
        ///than item index.
        /// </summary>
        public bool IsReferByPosition
        {
            get
            {
                return m_bIsReferByPosition;
            }
            set
            {
                m_bIsReferByPosition = value;
            }
        }
        /// <summary>
        /// Specifies the number of item indexes in the collection of indexes
        /// </summary>
        public int Count
        {
            get
            {
                return m_iCount;
            }
            set
            {
                m_iCount = value;
            }
        }
        /// <summary>
        /// Specifies the index of the field to which this filter refers. A value of -2 indicates the 'data'
        ///field.
        /// </summary>
        public int FieldIndex
        {
            get
            {
                return m_iFieldIndex;
            }
            set
            {
                m_iFieldIndex = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether the item is referred to by a relative
        ///reference rather than an absolute reference
        /// </summary>
        public bool IsRelativeReference
        {
            get
            {
                return m_bIsRelativeReference;
            }
            set
            {
                m_bIsRelativeReference = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether this field has selection. This attribute is
        ///used when the PivotTable is in Outline view.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return m_bIsSelected;
            }
            set
            {
                m_bIsSelected = value;
            }
        }
        /// <summary>
        /// Represents the indexes of the selected fileds
        /// </summary>
        public List<int> Indexes
        {
            get
            {
                if (m_iIndexes == null)
                    m_iIndexes = new List<int>();
                return m_iIndexes;
            }
        }
        /// <summary>
        /// First index in the indexes
        /// </summary>
        public int FirstIndex
        {
            get
            {
                if (m_iIndexes == null)
                    return -1;
                return m_iIndexes[0];
            }
        }
        #endregion

        #region Initialization
        public PivotAreaReference()
        {
            Subtotal = PivotSubtotalTypes.Default;
        }
        #endregion

    }
}
