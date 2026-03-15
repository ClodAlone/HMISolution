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

#if SILVERLIGHT
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Common;
namespace Syncfusion.OlapSilverlight.Engine
#else
using Syncfusion.Olap.Data;
using Syncfusion.Olap.Common;
namespace Syncfusion.Olap.Engine
#endif
{
    /// <summary>
    /// This class holds a list of header captions.
    /// </summary>
    public class HeaderCaptionCollection : List<string>
    {
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string value = string.Empty;
            foreach (var item in this)
            {
                if (value != string.Empty)
                    value += " - ";
                value += item;
            }
            return value;
        }
    }

    /// <summary>
    /// Represents the list of  <see cref="CellHeaderInfo"/> class.
    /// </summary>
    public class HeaderInfoCollection : List<CellHeaderInfo>
    {
      
    }

    /// <summary>
    /// Represents the cell header information.
    /// </summary>
    public class CellHeaderInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        /// <value>The unique name as string.</value>
        public string UniqueName { get; set; }
        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        /// <value>The member.</value>
        public Member Member { get; set; }
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Name: {0}, UniqueName: {1}", Name, UniqueName);
        }
    }
    /// <summary>
    /// Represents the pivot value cell data information.
    /// </summary>
    public class PivotValueCellData
    {
        #region members
        private string m_measure = string.Empty;
        private string m_value = string.Empty;
        private HeaderInfoCollection m_rowInfo;
        private HeaderInfoCollection m_colInfo;
        private HeaderCaptionCollection m_rows = null;
        private HeaderCaptionCollection m_columns = null;
        private CellHeaderInfo m_measureInfo = null;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                if( m_value != value )
                {
                    m_value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public HeaderCaptionCollection Rows
        {
            get
            {
                return m_rows;
            }
            set
            {
                if( m_rows != value )
                {
                    m_rows = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public HeaderCaptionCollection Columns
        {
            get
            {
                return m_columns;
            }
            set
            {
                if( m_columns != value )
                {
                    m_columns = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the measure.
        /// </summary>
        /// <value>The measure.</value>
        public string Measure
        {
            get
            {
                return m_measure;
            }
            set
            {
                if( m_measure != value )
                {
                    m_measure = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the row info.
        /// </summary>
        /// <value>The row info.</value>
        public HeaderInfoCollection RowInfo
        {
            get { return m_rowInfo; }
            set
            {
                if (m_rowInfo != value)
                {
                    m_rowInfo = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the column info.
        /// </summary>
        /// <value>The column info.</value>
        public HeaderInfoCollection ColumnInfo
        {
            get { return m_colInfo; }
            set
            {
                if (m_colInfo != value)
                {
                    m_colInfo = value;
                }
            }
        }

        
        /// <summary>
        /// Gets or Sets the measure info
        /// </summary>
        public CellHeaderInfo MeasureInfo
        {
            get { return m_measureInfo; }
            set { m_measureInfo = value; }
        }

        
        #endregion

        #region initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PivotValueCellData"/> class.
        /// </summary>
        public PivotValueCellData()
        {
            m_rows = new HeaderCaptionCollection();
            m_columns = new HeaderCaptionCollection();
            m_rowInfo = new HeaderInfoCollection();
            m_colInfo = new HeaderInfoCollection();
        }
        //public PivotValueCellData(string measureName, string valueName, List<string> rowsName, List<string> columnsName)
        //{
        //    this.Measure = measureName;
        //    this.Value = valueName;
        //    this.Rows = rowsName;
        //    this.Columns = columnsName;
        //}
        #endregion
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    /// <summary>
    /// Representation of list of <see cref="SerializableDictionary<TKey, TValue>"/> class.
    /// </summary>
    public class HeaderPositionsInfo : List<SerializableDictionary<string, List<PositionInfo>>>
    {
        /// <summary>
        /// Get the Hierarchy based Header's Position Collection
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Dictionary<string, string> GetHierarchyStrings(SerializableDictionary<string, List<PositionInfo>> positionInfos)
        {
            var pDict = new Dictionary<string, string>();
            PositionInfo pInfo = null;
            var pInfos = positionInfos.Select(pi => new { pi.Key, pi.Value }).ToList();
            for (int i = 0; i < pInfos.Count; i++)
            {
                pDict.Add(pInfos[i].Key, 
                    (pInfo = pInfos[i].Value.Last()) != null ? 
                    i == pInfos.Count - 1 ? string.Format("{0}, {0}.Children", pInfo.UniqueName) : pInfo.UniqueName : "");
            }
            return pDict;
        }

        /// <summary>
        /// Gets the hierarchy based positions.
        /// </summary>
        /// <param name="positionInfos">The position information.</param>
        /// <returns></returns>
        public static SerializableDictionary<string, List<PositionInfo>> GetHierarchyBasedPositions(List<PositionInfo> positionInfos)
        {
            if (positionInfos == null) return null;

            var info = positionInfos.GroupBy(p => p.HierarchyUniqueName).Select(p => new { Key = p.Key, Value = p.ToList() });
            var dict = new SerializableDictionary<string, List<PositionInfo>>();

            foreach (var i in info)
                dict.Add(i.Key, i.Value);

            return dict;
        }
    }

#if !SILVERLIGHT
    [Serializable]
#endif

    /// <summary>
    /// Represents the position information.
    /// </summary>
    public class PositionInfo
    {
        /// <summary>
        /// Gets or sets the name of the unique.
        /// </summary>
        /// <value>The name of the unique.</value>
        public string UniqueName { get; set; }
        /// <summary>
        /// Gets or sets the name of the hierarchy unique.
        /// </summary>
        /// <value>The name of the hierarchy unique.</value>
        public string HierarchyUniqueName { get; set; }
    }
}
