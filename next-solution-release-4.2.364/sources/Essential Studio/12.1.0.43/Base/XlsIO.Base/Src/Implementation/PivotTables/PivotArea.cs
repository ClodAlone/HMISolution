#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.XlsIO.Implementation.Collections;
namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    public class PivotArea
    {
        #region Members
        /// <summary>
        /// The region of the PivotTable to which this rule applies.
        /// </summary>
        private PivotAxisTypes m_axis;
        /// <summary>
        /// Flag indicating whether any indexes refer to fields or items in the Pivot cache and not the
        ///view.
        /// </summary>
        private bool m_iIsCacheIndex;
        /// <summary>
        /// Flag indicating whether any indexes refer to fields or items in the Pivot cache and not the
        ///view.
        /// </summary>
        private bool m_bIsSubtotal;
        /// <summary>
        /// Flag indicating whether only the data values (in the data area of the view) for an item
        ///selection are selected and does not include the item labels.
        /// </summary>
        private bool m_bIsDataOnly;
        /// <summary>
        /// Index of the field that this selection rule refers to.
        /// </summary>
        private int m_iFieldIndex;
        /// <summary>
        /// Position of the field within the axis to which this rule applies.
        /// </summary>
        private int m_iFieldPosition;
        /// <summary>
        /// Flag indicating whether the column grand total is included.
        /// </summary>
        private bool m_bHasColumnGrand;
        /// <summary>
        /// Flag indicating whether the row grand total is included.
        /// </summary>
        private bool m_bHasRowGrand;
        /// <summary>
        /// Flag indicating whether only the item labels for an item selection are selected and does
            ///not include the data values (in the data area of the view).
        /// </summary>
        private bool m_bIsLableOnly;
        /// <summary>
        /// A Reference that specifies a subset of the selection area. Points are relative to the top
        ///left of the selection area.
        /// </summary>
        private IRange m_range;
        /// <summary>
        /// Flag indicating whether the rule refers to an area that is in outline mode.
        /// </summary>
        private bool m_bIsOutline;
        /// <summary>
        /// Indicates the type of selection rule.
        /// </summary>
        private PivotAreaType m_areaType;
        /// <summary>
        /// Pivot Area references
        /// </summary>
        private PivotAreaReferences m_references;
        #endregion

        #region Properties
        /// <summary>
        /// The region of the PivotTable to which this rule applies.
        /// </summary>
        public PivotAxisTypes Axis
        {
            get
            {
                return m_axis;
            }
            set
            {
                m_axis = value;
            }
        }
        /// <summary>
        /// Flag indicating whether any indexes refer to fields or items in the Pivot cache and not the
        ///view.
        /// </summary>
        public bool IsCacheIndex
        {
            get
            {
                return m_iIsCacheIndex;
            }
            set
            {
                m_iIsCacheIndex = value;
            }
        }
        /// <summary>
        /// Flag indicating whether any indexes refer to fields or items in the Pivot cache and not the
        ///view.
        /// </summary>
        public bool IsSubtotal
        {
            get
            {
                return m_bIsSubtotal;
            }
            set
            {
                m_bIsSubtotal = value;
            }
        }

        /// <summary>
        /// Flag indicating whether only the data values (in the data area of the view) for an item
        ///selection are selected and does not include the item labels.
        /// </summary>
        public bool IsDataOnly
        {
            get
            {
                return m_bIsDataOnly;
            }
            set
            {
                m_bIsDataOnly = value;
            }
        }

        /// <summary>
        /// Index of the field that this selection rule refers to.
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
        /// Position of the field within the axis to which this rule applies.
        /// </summary>
        public int FieldPosition
        {
            get
            {
                return m_iFieldPosition;
            }
            set
            {
                m_iFieldPosition = value;
            }
        }
        /// <summary>
        /// Flag indicating whether the column grand total is included.
        /// </summary>
        public bool HasColumnGrand
        {
            get
            {
                return m_bHasColumnGrand;
            }
            set
            {
                m_bHasColumnGrand = value;
            }
        }
        /// <summary>
        /// Flag indicating whether the row grand total is included.
        /// </summary>
        public bool HasRowGrand
        {
            get
            {
                return m_bHasRowGrand;
            }
            set
            {
                m_bHasRowGrand = value;
            }
        }
        /// <summary>
        /// Flag indicating whether only the item labels for an item selection are selected and does
        ///not include the data values (in the data area of the view).
        /// </summary>
        public bool IsLableOnly
        {
            get
            {
                return m_bIsLableOnly;
            }
            set
            {
                m_bIsLableOnly = value;
            }
        }
        /// <summary>
        /// A Reference that specifies a subset of the selection area. Points are relative to the top
        ///left of the selection area.
        /// </summary>
        public IRange Range
        {
            get
            {
                return m_range;
            }
            set
            {
                m_range = value;
            }
        }

        /// <summary>
        /// Flag indicating whether the rule refers to an area that is in outline mode.
        /// </summary>
        public bool IsOutline
        {
            get
            {
                return m_bIsOutline;
            }
            set
            {
                m_bIsOutline = value;
            }
        }
        /// <summary>
        /// Indicates the type of selection rule.
        /// </summary>
        public PivotAreaType AreaType
        {
            get
            {
                return m_areaType;
            }
            set
            {
                m_areaType = value;
            }
        }
        /// <summary>
        /// Pivot Area references
        /// </summary>
        internal PivotAreaReferences References
        {
            get
            {
                return m_references;
            }
        }
        /// <summary>
        /// First Index of the Reference
        /// </summary>
        internal int FirstIndexReference
        {
            get
            {
                if (m_references != null)
                    return m_references[0].FirstIndex;
                return -1;
            }
        }
        #endregion

        #region Initialization
        public PivotArea(PivotCacheFieldImpl cacheField)
        {
            m_references = new PivotAreaReferences();
            Axis = PivotAxisTypes.None;
            AreaType = PivotAreaType.None;
        }
        #endregion
    }
}
