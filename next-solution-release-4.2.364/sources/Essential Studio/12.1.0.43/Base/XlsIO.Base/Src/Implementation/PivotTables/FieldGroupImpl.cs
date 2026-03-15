#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    /// <summary>
    /// Describes single pivot field group in the pivot table
    /// </summary>
    public class FieldGroupImpl
    {
        #region Members
        /// <summary>
        /// Represents the PivotCacheFieldImpl Object 
        /// </summary>
        private PivotCacheFieldImpl m_pivotCacheField;
        /// <summary>
        /// Represents the range grouping properties.
        /// </summary>
        private FieldRangeGroup m_rangeGroup;
        /// <summary>
        /// Represents the discrete range grouping properties.
        /// </summary>
        private FieldDiscreteRangeGroup m_discreteGroup;
        /// <summary>
        /// Represents wheather the fieldGroup contains the date time
        /// </summary>
        private bool m_bHasDateTime;
        /// <summary>
        /// Represents wheather the fieldGroup contains the number
        /// </summary>
        private bool m_bHasNumber;

        /// <summary>
        /// Specifies the parent index of this field
        /// </summary>
        private int m_iParentFieldIndex;
        #endregion

        #region Properties
        /// <summary>
        /// Represents the index of the parent field.
        /// </summary>
        public int ParentFieldIndex
        {
            get
            {
                return m_iParentFieldIndex;
            }
            internal set
            {
                m_iParentFieldIndex = value;
            }
        }
        /// <summary>
        /// Represents the index of the PivotCacheField.
        /// </summary>
        public int PivotCacheFieldIndex
        {
            get
            {
                return m_pivotCacheField.Index;
            }
           
        }
        /// <summary>
        /// Specifies the base of this field
        /// </summary>
        public PivotCacheFieldImpl PivotCacheField
        {
            get
            {
                return m_pivotCacheField;
            }
        }
        /// <summary>
        /// Represents the collection of range grouping properties.
        /// </summary>
        internal FieldRangeGroup RangeGroup
        {
            get
            {
                if (m_rangeGroup == null)
                    m_rangeGroup = new FieldRangeGroup();
                return m_rangeGroup;
            }
        }
        /// <summary>
        /// Specifies the start date for pivot field grouping 
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return RangeGroup.m_startDate;
            }
            set
            {
                AutoStartRange = false;
                RangeGroup.m_startDate = value;
                HasDateTime = true;
            }
        }
        /// <summary>
        /// Specifies the end date for pivot field grouping
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return RangeGroup.m_endDate;
            }
            set
            {
                RangeGroup.m_endDate = value;
                HasDateTime = true;
            }
        }
        /// <summary>
        /// Specifies the order of the first pivot field item in the pivot field group.
        /// </summary>
        public double StartNumber
        {
            get
            {
                return RangeGroup.m_dStartNumber;
            }
            set
            {
                RangeGroup.m_dStartNumber = value;
                m_bHasNumber = true;
            }
        }
        /// <summary>
        /// Specifies the order of the last pivot field item in the pivot field group.
        /// </summary>
        public double EndNumber
        {
            get
            {
                return RangeGroup.m_dEndNumber;
            }
            set
            {
                RangeGroup.m_dEndNumber = value;
                m_bHasNumber = true;
            }
        }
        /// <summary>
        /// Specifies the pivot field group type for the pivot field group
        /// </summary>
        public PivotFieldGroupType GroupBy
        {
            get
            {
                return RangeGroup.m_groupBy;
            }
            set
            {
                RangeGroup.m_groupBy = value;
            }
        }
        /// <summary>
        /// Specifies the grouping interval for pivot field group.
        /// </summary>
        public double GroupInterval
        {
            get
            {

                return RangeGroup.m_dGroupInterval;
            }
            set
            {
                RangeGroup.m_dGroupInterval = value;
            }
        }

        internal bool HasGroupInterval
        {
            get
            {
                return RangeGroup.m_hasGroupInterval;
            }
            set
            {
                RangeGroup.m_hasGroupInterval = value;
            }
        }
        /// <summary>
        /// Represents the collection of pivot range group names in a field group.
        /// </summary>
        public List<string> PivotRangeGroupNames
        {
            get
            {
                return RangeGroup.GroupItems;
            }
        }
        /// <summary>
        /// Represents the collection of pivot discrete group names in a field group.
        /// </summary>
        public List<string> PivotDiscreteGroupNames
        {
            get
            {
                if (m_discreteGroup == null)
                    m_discreteGroup = new FieldDiscreteRangeGroup();
                return m_discreteGroup.GroupItems;
            }
        }
        /// <summary>
        /// Represents the collection of items indexes in a field group.
        /// </summary>
        public byte[] DiscreteGroupIndexes
        {
            get
            {
                return m_discreteGroup.Indexes;
            }
            set
            {
                m_discreteGroup.Indexes = value;
            }

        }
        /// <summary>
        /// Returns the whether the field group is a Dircrete group.
        /// </summary>
        public bool IsDiscrete
        {
            get
            {
               return m_discreteGroup != null;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether the application uses the source data to
        ///set the ending range value.
        /// </summary>
        public bool AutoStartRange
        {
            get
            {
                return RangeGroup.m_bAutoStartRange;
            }
            set
            {
                RangeGroup.m_bAutoStartRange = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether we use source data to set the beginning
        /// range value.
        /// </summary>
        public bool AutoEndRange
        {
            get
            {
                return RangeGroup.m_bAutoEndRange;
            }
            set
            {
                RangeGroup.m_bAutoEndRange = value;
            }
        }
        /// <summary>
        /// Represents wheather the fieldGroup contains the date time
        /// </summary>
        public bool HasDateTime
        {
            get
            {
                return m_bHasDateTime;
            }
            set
            {
                m_bHasDateTime = value;
            }

        }
        /// <summary>
        /// Represents wheather the fieldGroup contains the Number
        /// </summary>
        public bool HasNumber
        {
            get
            {
                return m_bHasNumber;
            }
            set
            {
                m_bHasNumber = value;
            }

        }
        #endregion

        #region Initaialization
        public FieldGroupImpl(PivotCacheFieldImpl baseField,int parentFieldIndex)
        {
            if (baseField == null)
                throw new ArgumentNullException("base Field");

            m_pivotCacheField = baseField;
            ParentFieldIndex = parentFieldIndex;
            this.EndDate = new DateTime();
            this.StartDate = new DateTime();
            this.AutoEndRange = true;
            this.AutoStartRange = true;
            this.EndNumber = -1;
            this.StartNumber = -1;
            HasDateTime = false;
            HasNumber = false;
            GroupBy = PivotFieldGroupType.None;
        }
        public FieldGroupImpl(PivotCacheFieldImpl baseField)
            : this(baseField,-1)
        {
        }
        #endregion

        #region Methods
        public void FillRangeGroup(string[] values)
        {
            PivotRangeGroupNames.AddRange(values);
        }
        public void FillDiscreteGroup(int []indexes,string []groupNames)
        {
            PivotDiscreteGroupNames.AddRange(groupNames);
            DiscreteGroupIndexes=new byte[indexes.Length];
            for (int i = 0; i < indexes.Length; i++)
                DiscreteGroupIndexes[i] = (byte)indexes[i];
        }
        #endregion

        #region Internal Classes

        /// <summary>
        /// Represents the Grouping of Field with the range 
        /// </summary>
        internal class FieldRangeGroup
        {
            #region Members
            /// <summary>
            /// Represents the collection of items in a field group.
            /// </summary>
            internal List<string> m_groupItems;
            /// <summary>
            /// Specifies a boolean value that indicates whether the application uses the source data to
            ///set the ending range value.
            /// </summary>
            internal bool m_bAutoStartRange;
            /// <summary>
            /// Specifies a boolean value that indicates whether we use source data to set the beginning
            /// range value.
            /// </summary>
            internal bool m_bAutoEndRange;
            /// <summary>
            /// Specifies the ending value for date grouping if autoEnd is false.
            /// </summary>
            internal DateTime m_endDate;
            /// <summary>
            /// Specifies the ending value for numeric grouping if autoEnd is false.
            /// </summary>
            internal double m_dEndNumber;
            /// <summary>
            /// Specifies the grouping the field item by
            /// </summary>
            internal PivotFieldGroupType m_groupBy;
            /// <summary>
            /// Specifies the grouping interval for numeric range grouping. Specifies the number of days
            ///to group by in date range grouping.
            /// </summary>
            internal double m_dGroupInterval;
            /// <summary>
            /// Specifies the starting value for date grouping if autoStart is false.
            /// </summary>
            internal DateTime m_startDate;
            /// <summary>
            /// Specifies the starting value for numeric grouping if autoStart is false.
            /// </summary>
            internal double m_dStartNumber;
            internal bool m_hasGroupInterval = false ;
            #endregion

            #region Properties
            /// <summary>
            /// Represents the collection of items in a field group.
            /// </summary>
            public List<string> GroupItems
            {
                get
                {
                    if (m_groupItems == null)
                        m_groupItems = new List<string>();
                    return m_groupItems;
                }
            }
            /// <summary>
            /// Specifies a boolean value that indicates whether the application uses the source data to
            ///set the ending range value.
            /// </summary>
            public bool AutoStartRange
            {
                get
                {
                    return m_bAutoEndRange;
                }
                set
                {
                    m_bAutoEndRange = value;
                }
            }
            /// <summary>
            /// Specifies a boolean value that indicates whether we use source data to set the beginning
            /// range value.
            /// </summary>
            public bool AutoEndRange
            {
                get
                {
                    return m_bAutoEndRange;
                }
                set
                {
                    m_bAutoEndRange = value;
                }
            }
            /// <summary>
            /// Specifies the ending value for date grouping if autoEnd is false.
            /// </summary>
            public DateTime EndDate
            {
                get
                {
                    return m_endDate;
                }
                set
                {
                    m_endDate = value;
                }
            }
            /// <summary>
            /// Specifies the ending value for numeric grouping if autoEnd is false.
            /// </summary>
            public double EndNumber
            {
                get
                {
                    return m_dEndNumber;
                }
                set
                {
                    m_dEndNumber = value;
                }
            }
            /// <summary>
            /// Specifies the grouping the field item by
            /// </summary>
            public PivotFieldGroupType GroupBy
            {
                get
                {
                    return m_groupBy;
                }
                set
                {
                    m_groupBy = value;
                }
            }

            /// <summary>
            /// Specifies the grouping interval for numeric range grouping. Specifies the number of days
            ///to group by in date range grouping.
            /// </summary>
            public double GroupInterval
            {
                get
                {
                    return m_dGroupInterval;
                }
                set
                {
                    m_dGroupInterval = value;
                }
            }
            /// <summary>
            /// Specifies the starting value for date grouping if autoStart is false.
            /// </summary>
            public DateTime StartDate
            {
                get
                {
                    return m_startDate;
                }
                set
                {
                    m_startDate = value;
                }
            }
            /// <summary>
            /// Specifies the starting value for numeric grouping if autoStart is false.
            /// </summary>
            public double StartNumber
            {
                get
                {
                    return m_dStartNumber;
                }
                set
                {
                    m_dStartNumber = value;
                }
            }
             #endregion
        }
        class FieldDiscreteRangeGroup
        {
            #region Members
            /// <summary>
            /// Represents the collection of items in a field group.
            /// </summary>
            internal List<string> m_groupItems;
            /// <summary>
            /// Represents the collection of item indexes
            /// </summary>
            private byte[] m_indexes;
            #endregion

            #region Properties
            /// <summary>
            /// Represents the collection of items in a field group.
            /// </summary>
            public List<string> GroupItems
            {
                get
                {
                    if (m_groupItems == null)
                        m_groupItems = new List<string>();
                    return m_groupItems;
                }
            }
            /// <summary>
            /// Represents the collection of item indexes
            /// </summary>
            public byte[] Indexes
            {
                get
                {
                    return m_indexes;
                }
                set
                {
                    m_indexes = value;
                }
            }
            #endregion
        }
        #endregion
    }
}
