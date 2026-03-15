#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;

#if SILVERLIGHT
using Syncfusion.OlapSilverlight.Data;
namespace Syncfusion.OlapSilverlight.Engine
#else
using Syncfusion.Olap.Common;
using System.Web.UI;
using Syncfusion.Olap.Data;
namespace Syncfusion.Olap.Engine
#endif
{  

    /// <summary>
    /// Descriptor of pivot cell.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]

    public class PivotCellDescriptor 
        : ICloneable<PivotCellDescriptor>
#else
    public class PivotCellDescriptor
#endif
    {
        #region class members
        private string m_cellValue = string.Empty;
        private string m_value = string.Empty;
        private GridRangeInfo m_range = GridRangeInfo.EmptyRange();
        private PivotCellDescriptor m_spanCell = null;
        private GridRangeInfo m_modelLocation = null;
        private PivotCellDescriptorType m_cellType;
        private int m_cellIndex = -1;
        private int m_RowIndex = -1;
        private ExpandableState m_expandableState;
        private string m_uniqueName = string.Empty;
        private long m_level = -1;
        private bool m_IsEmpty = false;
        private PivotValueCellData m_CellData = new PivotValueCellData();
        private int _pivotRowIndex = -1;
      
        private object m_tag;
#if !SILVERLIGHT
        [NonSerialized]
#endif
        private List<PivotCellDescriptor> m_Parent;
        private bool m_bHasChildren;
        private string m_className = string.Empty;
        private List<string> m_cellExTypes = new List<string>();
        private KpiTypeEnum m_kpiType = KpiTypeEnum.Kpi_None;
        private string m_kpiGraphicsStyle = string.Empty;

        #endregion

        #region class properties

        /// <summary>
        /// Gets or sets the Pivot Engine.
        /// </summary>
        /// <value>The Engine.</value>
        public PivotEngine Engine { get; set; }

        /// <summary>
        /// Gets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public double DoubleValue
        {
            get
            {
                if (this.Value != null)
                {
                    double val = 0.0;
                    if (double.TryParse(this.Value.ToString(), out val))
                    {
                        return val;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// KPI graphical rendering style (for status and trend cell types).
        /// </summary>
        public string KpiGraphicsStyle
        {
            get
            {
                return m_kpiGraphicsStyle;
            }
            set
            {
                if (m_kpiGraphicsStyle != value)
                {
                    m_kpiGraphicsStyle = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent cell values.
        /// </summary>
        /// <value>The parent cell values.</value>
        internal List<string> ParentCellValues
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cell data.
        /// </summary>
        /// <value>The cell data.</value>
        public PivotValueCellData CellData
        {
            get
            {
                if (this.Engine != null)
                {
                    m_CellData = this.Engine.GetCellData(this);
                }
                return m_CellData ?? new PivotValueCellData();
            }
            internal set
            {
                if (m_CellData != value)
                {
                    m_CellData = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the cell object.
        /// </summary>
        /// <value>The cell object.</value>
        public object CellObject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return m_IsEmpty;
            }
            set
            {
                m_IsEmpty = value;
            }
        }

        /// <summary>
        /// Gets/sets KPI type of this cell.
        /// </summary>
        public KpiTypeEnum KpiType
        {
            get
            {
                return m_kpiType;
            }
            set
            {
                if (m_kpiType != value)
                {
                    m_kpiType = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets class name for the cell.
        /// </summary>
        public string ClassName
        {
            get
            {
                return m_className;
            }
            set
            {
                if (m_className != value)
                {
                    m_className = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has children.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has children; otherwise, <c>false</c>.
        /// </value>
        public bool HasChildren
        {
            get
            {
                return m_bHasChildren;
            }
            set
            {
                if (m_bHasChildren != value)
                {
                    m_bHasChildren = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the unique.
        /// </summary>
        /// <value>The name of the unique.</value>
        public string UniqueName
        {
            get
            {
                return m_uniqueName;
            }
            set
            {
                if (m_uniqueName != value)
                {
                    m_uniqueName = value;
                }
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets a value indicating whether this instance is last level.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is last level; otherwise, <c>false</c>.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        [DefaultValue(typeof(bool),"false")]
        public bool IsLastLevel
        {
            get;
            set;
        }


#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the parent cell descriptors.
        /// </summary>
        /// <value>The parent cell descriptors.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif       
        internal List<PivotCellDescriptor> ParentCellDescriptors
        {
            get
            {
                if (m_Parent == null)
                {
                    return m_Parent = new List<PivotCellDescriptor>();
                }
                else
                    return m_Parent;
            }
            set
            {
                if (m_Parent != value)
                {
                    m_Parent = value;
                }
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public object Tag
        {
            get
            {
                return m_tag;
            }
            set
            {
                if (m_tag != value)
                {
                    m_tag = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        public long Level
        {
            get
            {
                return m_level;
            }
            set
            {
                if (m_level != value)
                {
                    m_level = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the model location.
        /// </summary>
        /// <value>The model location.</value>
        internal GridRangeInfo ModelLocation
        {
            get
            {
                return m_modelLocation;
            }
            set
            {
                if (m_modelLocation != value)
                {
                    m_modelLocation = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets main cell in the span.
        /// </summary>
        public PivotCellDescriptor SpanCell
        {
            get
            {
                return m_spanCell;
            }
            set
            {
                if (m_spanCell != value)
                {
                    m_spanCell = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets if the cell is expanded.
        /// </summary>
        public ExpandableState ExpandableState
        {
            get
            {
                return m_expandableState;
            }
            set
            {
                if (m_expandableState != value)
                {
                    m_expandableState = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets range occupied by this cell.
        /// </summary>
        /// <remarks>
        /// Location data is ignored and assumed to be this cell location.
        /// </remarks>
        [DefaultValue(typeof(GridRangeInfo), "")]
        public GridRangeInfo Range
        {
            get
            {
                return m_range;
            }
            set
            {
                if (m_range != value)
                {
                    m_range = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets row index of this cell in a column.
        /// </summary>
        [DefaultValue(typeof(int), "-1")]
        internal int RowIndex
        {
            get
            {
                return m_RowIndex;
            }
            set
            {
                if (m_RowIndex != value)
                {
                    m_RowIndex = value;
                }
            }
        }

        [DefaultValue(typeof(int), "-1")]
        public int PivotRowIndex
        {
            get { return _pivotRowIndex; }
            set { _pivotRowIndex = value; }
        }

        /// <summary>
        /// Gets/sets index of this cell in a column.
        /// </summary>
        [DefaultValue(typeof(int), "-1")]
        public int CellIndex
        {
            get
            {
                return m_cellIndex;
            }
            set
            {
                if (m_cellIndex != value)
                {
                    m_cellIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets extended type of cell descriptor.
        /// </summary>
        /// <remarks>
        /// Extended type of any cell can be changed without any consequences to grouping logic.
        /// </remarks>
#if !SILVERLIGHT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        PersistenceMode(PersistenceMode.Attribute), Browsable(true), NotifyParentProperty(true)]
#endif
        public List<string> CellExTypes
        {
            get
            {
                return m_cellExTypes;
            }
            set
            {
                if (m_cellExTypes != value)
                {
                    m_cellExTypes = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets type of cell descriptor.
        /// </summary>
        /// <remarks>
        /// Types of row or coulmn header cells should not be changed.
        /// </remarks>
        [DefaultValue(typeof(PivotCellDescriptorType), "Any")]
#if !SILVERLIGHT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [PersistenceMode(PersistenceMode.Attribute), Browsable(true), NotifyParentProperty(true)]
#endif
        public PivotCellDescriptorType CellType
        {
            get
            {
                return m_cellType;
            }
            set
            {
                if (m_cellType != value)
                {
                    m_cellType = value;
                }
            }
        }

        /// <summary>
        /// value of current cell.
        /// </summary>
        [DefaultValue("")]
        public string CellValue
        {
            get
            {
                return m_cellValue;
            }
            set
            {
                if (m_cellValue != value)
                {
                    m_cellValue = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public string FormatString { get; set; }

        /// <summary>
        /// Unformatted value of current cell.
        /// </summary>
        [DefaultValue("")]
        public string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                if (m_value != value)
                {
                    m_value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the cell caption.
        /// </summary>
        /// <value>The cell caption.</value>
        [DefaultValue("")]
        public string CellCaption { get; set; }
        #endregion

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.CellValue + " " + this.CellType.ToString() + " " + this.UniqueName;
        }

        #region ICloneable Members
        /// <summary>
        /// Returns cloned cell descriptor.
        /// </summary>
        /// <returns>Cell descriptor.</returns>
        public PivotCellDescriptor Clone()
        {
            PivotCellDescriptor descriptor = new PivotCellDescriptor();
            descriptor.CellValue = this.CellValue;
            descriptor.Value = this.Value;
            descriptor.Range = this.Range.Clone() as GridRangeInfo;
            descriptor.SpanCell = this.SpanCell;
            descriptor.ModelLocation = this.ModelLocation;
            descriptor.CellType = this.CellType;
            descriptor.CellIndex = this.CellIndex;
            descriptor.UniqueName = this.UniqueName;
            descriptor.Level = this.Level;
            descriptor.ExpandableState = this.ExpandableState;
            descriptor.HasChildren = this.HasChildren;
            descriptor.Tag = this.Tag;
            return descriptor;
        }
        #endregion
               
    }
}
