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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// The Chart Region represents a region on the client area of the Chart control that has a ToolTip.
    /// It can be a point region. A point region is a region that represents the visual appearance of a point.
    /// For example, the Rectangle of a Column point. When the ChartRegion represents a chart point, IsChartPoint is set to True.
    /// </summary>
    public class ChartRegion
    {
        #region Members
        private ChartRegionData m_data = null;
        private Region m_region;
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
                return m_data.IsChartPoint;
            }
        }

        /// <summary>
        /// Indicates whether the region represents a chart point.
        /// </summary>
        /// <value>The type.</value>
        public ChartRegionType Type
        {
            get
            {
                return m_data.Type;
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
                return m_data.PointIndex;
            }
        }

        /// <summary>
        /// Gets or sets the actual region that has the ToolTip.
        /// </summary>
        /// <value>The region.</value>
        public Region Region
        {
            get
            {
                return m_region;
            }

            set
            {
                m_region = value;
            }
        }

        /// <summary>
        /// Returns the index value of the series that has this region.
        /// </summary>
        /// <value>The index of the series.</value>
        public int SeriesIndex
        {
            get
            {
                return m_data.SeriesIndex;
            }
        }

        /// <summary>
        /// Returns the index value of the axis that has this region.
        /// </summary>
        /// <value>The index of the axis.</value>
        public int AxisIndex
        {
            get
            {
                return m_data.AxisIndex;
            }
        }


        /// <summary>
        /// Gets or sets the ToolTip of the region.
        /// </summary>
        /// <value>The tool tip.</value>
        public string ToolTip
        {
            get
            {
                return m_data.ToolTip;
            }

            set
            {
                m_data.ToolTip = value;
            }
        }

        /// <summary>
        /// Returns the description for this region.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                return m_data.Description;
            }
        }
        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRegion"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="seriesIndex">Index of the series.</param>
        /// <param name="pointIndex">Index of the point.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="description">The description.</param>
        public ChartRegion(Region region, int seriesIndex, int pointIndex, string toolTip, string description)
        {
            m_region = region;
            m_data = new ChartRegionData(seriesIndex, pointIndex, toolTip, description);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRegion"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="seriesIndex">Index of the series.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="description">The description.</param>
        public ChartRegion(Region region, int seriesIndex, string toolTip, string description)
        {
            m_region = region;
            m_data = new ChartRegionData(seriesIndex, toolTip, description);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRegion"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="description">The description.</param>
        public ChartRegion(Region region, string toolTip, string description)
        {
            m_region = region;
            m_data = new ChartRegionData(toolTip, description);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRegion"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="regionType">Type of the region.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="description">The description.</param>
        public ChartRegion(Region region, ChartRegionType regionType, string toolTip, string description)
        {
            m_region = region;
            m_data = new ChartRegionData(regionType, toolTip, description);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRegion"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="regionType">Type of the region.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="index">Axis index.</param>
        /// <param name="description">The description.</param>
        public ChartRegion(Region region, ChartRegionType regionType, string toolTip, int index, string description)
        {
            m_region = region;
            m_data = new ChartRegionData(regionType, toolTip, index, description);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRegion"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="data">The data.</param>
        public ChartRegion(Region region, ChartRegionData data)
        {
            m_region = region;
            m_data = data;
        }
        ~ChartRegion()
        {
            if (m_region != null)
            {
                m_region.Dispose();
                m_region = null;
            }
            if (m_data != null)
                m_data = null;
            
        }
        #endregion
    }
}