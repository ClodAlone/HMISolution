#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using Syncfusion.XlsIO.Interfaces.PivotTables;
namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    /// <summary>
    /// Represents an item within a PivotTable field which uses a formula
    /// TODO: Need to create support for pivot item.
    /// </summary>
    public class PivotCalculatedItemImpl:IPivotCalculatedItem
    {
        #region Members
        /// <summary>
        /// Specifies the formula of the calculated item
        /// </summary>
        private string m_formula;
        /// <summary>
        /// Specifies the pivotField with which this calculated item is associated.
        /// </summary>
        private PivotCacheFieldImpl fieldImpl;
        /// <summary>
        /// Specifies the pivot field index with 
        /// which this calculated item is associated
        /// </summary>
        private int m_iFieldIndex;
        /// <summary>
        /// Represents the calculated field area
        /// </summary>
        private PivotArea m_pivotArea;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies the formula of the calculated item
        /// </summary>
        public string Formula
        {
            get
            {
                return m_formula;
            }
            set
            {
                m_formula = value;
            }
        }
        /// <summary>
        /// Represents the calculated field area
        /// </summary>
        public PivotArea PivotArea
        {
            get
            {
                return m_pivotArea;
            }
        }
        /// <summary>
        /// Represents the cache field 
        /// </summary>
        internal PivotCacheFieldImpl cacheField
        {
            get
            {
                return cacheField;
            }
            set
            {
                cacheField = value;
            }
        }
        internal int FieldIndex
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
         
        #endregion

        #region Initialization
        public PivotCalculatedItemImpl(PivotCacheFieldImpl cacheField)
        {
            this.fieldImpl = cacheField;
            m_pivotArea = new PivotArea(fieldImpl);
            m_iFieldIndex = -1;
        }

        #endregion

    }
}
