#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.ComponentModel;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Lists the types of chart region.
    /// </summary>
    public enum ChartRegionType
    {
        /// <summary>
        /// The region is a series point.
        /// </summary>
        SeriesPoint,

        /// <summary>
        /// The region is a axes.
        /// </summary>
        Axis,

        /// <summary>
        /// The region is a horizontal axis grouping label.
        /// </summary>
        HorAxisLabel,

        /// <summary>
        /// The region is a vertical axis grouping label.
        /// </summary>
        VerAxisLabel,

        /// <summary>
        /// The region is a unknown element.
        /// </summary>
        Unknown,

        /// <summary>
        /// Anything else not covered.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        ChartCustom = Unknown
    }

    /// <summary>
    /// Represents the data of <see cref="ChartRegion"/>
    /// </summary>
    public sealed class ChartRegionData : ICloneable
    {
        #region Members       
        private int m_pointIndex = -1;
        private int m_seriesIndex = -1;
        private int m_axisIndex = -1;
        private string m_toolTip;
        private string m_description;
        private ChartRegionType m_type = ChartRegionType.Unknown;
        #endregion

        #region Properties
        /// <summary>
        /// Indicates whether the region represents a chart point.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is chart point; otherwise, <c>false</c>.
        /// </value>
        public bool IsChartPoint
        {
            get
            {
                return m_type == ChartRegionType.SeriesPoint;
            }
        }
        
        /// <summary>
        /// Gets the index of the point.
        /// </summary>
        /// <value>The index of the point.</value>
        public int PointIndex
        {
            get
            {
                return m_pointIndex;
            }
        }

        /// <summary>
        /// Gets the index of the series.
        /// </summary>
        /// <value>The index of the series.</value>
        public int SeriesIndex
        {
            get
            {
                return m_seriesIndex;
            }
        }
        /// <summary>
        /// Gets the index of the Axis.
        /// </summary>
        /// <value>The index of the Axis.</value>
        public int AxisIndex
        {
            get
            {
                return m_axisIndex;
            }
        }

        /// <summary>
        /// Gets or sets the ToolTip for this region.
        /// </summary>
        /// <value>The tool tip.</value>
        public string ToolTip
        {
            get
            {
                return m_toolTip;
            }

            set
            {
                m_toolTip = value;
            }
        }
        
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                return m_description;
            }
        }
        
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public ChartRegionType Type
        {
            get
            {
                return m_type;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRegionData"/> class.
        /// </summary>
        /// <param name="seriesIndex">Index of the series.</param>
        /// <param name="pointIndex">Index of the point.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="description">The description.</param>
        public ChartRegionData(int seriesIndex, int pointIndex, string toolTip, string description)
        {
            m_type = ChartRegionType.SeriesPoint;
            m_seriesIndex = seriesIndex;
            m_pointIndex = pointIndex;
            m_toolTip = toolTip;
            m_description = description;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRegionData"/> class.
        /// </summary>
        /// <param name="seriesIndex">Index of the series.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="description">The description.</param>
        public ChartRegionData(int seriesIndex, string toolTip, string description)
        {
            m_type = ChartRegionType.SeriesPoint;
            m_seriesIndex = seriesIndex;
            m_toolTip = toolTip;
            m_description = description;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRegionData"/> class.
        /// </summary>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="description">The description.</param>
        public ChartRegionData(string toolTip, string description)
        {
            m_type = ChartRegionType.ChartCustom;
            m_toolTip = toolTip;
            m_description = description;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRegionData"/> class.
        /// </summary>
        /// <param name="regionType">Type of the region.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="description">The description.</param>
        public ChartRegionData(ChartRegionType regionType, string toolTip, string description)
        {
            m_toolTip = toolTip;
            m_description = description;
            m_type = regionType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRegionData"/> class.
        /// </summary>
        /// <param name="regionType">Type of the region.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="index">Axis index.</param>
        /// <param name="description">The description.</param>
        public ChartRegionData(ChartRegionType regionType, string toolTip,int index, string description)
        {
            m_toolTip = toolTip;
            m_axisIndex = index;
            m_description = description;
            m_type = regionType;
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Gets the chart region.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <returns></returns>
        public ChartRegion GetChartRegion(Region region)
        {
            return new ChartRegion(region, this);
        }

        /// <summary>
        /// Clones a data.
        /// </summary>
        /// <returns>Return ChartRegion Data.</returns>
        public ChartRegionData Clone()
        {
            ChartRegionData result = new ChartRegionData(m_type, m_toolTip, m_description);

            result.m_pointIndex = m_pointIndex;
            result.m_seriesIndex = m_seriesIndex;

            return result;
        }

        /// <summary>
        /// Implementation of interface ICloneable.
        /// </summary>
        /// <returns> Returns ChartRegiondata clone. </returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }
        #endregion
    }
}
