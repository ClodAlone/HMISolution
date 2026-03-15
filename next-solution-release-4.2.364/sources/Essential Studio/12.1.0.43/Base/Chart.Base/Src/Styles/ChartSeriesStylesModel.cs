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
using System.Collections;
using System.Diagnostics;

using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// The ChartSeriesStylesModel class.
    /// </summary>
    internal class ChartSeriesStylesModel : IChartSeriesStylesModel
    {
        #region Constants
        private const int c_seriesIndex = -1;
        #endregion

        #region Members
        private ChartStyleInfo m_seriesStyle;
        private IChartSeriesStylesHost m_host;
        private ChartSeriesComposedStylesModel m_volatileData;
        private Hashtable m_styles = new Hashtable();
        #endregion

        #region Events
        /// <summary>
        /// Occurs when model is changed
        /// </summary>
        public event ChartStyleChangedEventHandler Changed;
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets the series style information.
        /// </summary>
        /// <value></value>
        public ChartStyleInfo Style
        {
            get
            {
                return m_seriesStyle;
            }
        }

        /// <summary>
        /// Gets the ComposedStyles. Completely composed styles can be accessed using the interface returned by this property.
        /// Composed styles have all information initialized from base styles and any other styles along their
        /// inheritance hierarchy.
        /// </summary>
        /// <value></value>
        public IChartSeriesComposedStylesModel ComposedStyles
        {
            get
            {
                return m_volatileData;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSeriesStylesModel"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        public ChartSeriesStylesModel(IChartSeriesStylesHost host)
        {
            m_host = host;
            m_seriesStyle = new ChartStyleInfo();
            m_volatileData = new ChartSeriesComposedStylesModel(this);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the style information at the specified index. This is the actual style information and not composed style information.
        /// </summary>
        /// <param name="index">The index value of the point for which style information is needed.</param>
        /// <returns>
        /// Style information at the specified index.
        /// </returns>
        public ChartStyleInfo GetStyleAt(int index)
        {
            if (m_styles.ContainsKey(index))
            {
                return m_styles[index] as ChartStyleInfo;
            }

            return null;
        }

        /// <summary>
        /// Changes style information at the specified index.
        /// </summary>
        /// <param name="style">Style whose attributes are to be stored.</param>
        /// <param name="index">Index value where they need to be stored.</param>
        public void ChangeStyleAt(ChartStyleInfo style, int index)
        {
            ChartStyleInfo storedStyle = this.GetStyleAt(index);

            if (storedStyle == null)
            {
                m_styles[index] = new ChartStyleInfo(style);
            }
            else
            {
                storedStyle.ModifyStyle(style, StyleModifyType.Changes);
            }

            this.RaiseStyleChanged(index);
        }

        /// <summary>
        /// Changes series style information.
        /// </summary>
        /// <param name="style">Style whose attributes are to be stored in the series style.</param>
        public void ChangeStyle(ChartStyleInfo style)
        {
            ChartStyleInfo storedStyle = m_seriesStyle;

            if (storedStyle == null)
            {
                m_seriesStyle = new ChartStyleInfo(style);
            }
            else
            {
                storedStyle.ModifyStyle(style, StyleModifyType.Changes);
            }
        }

        /// <summary>
        /// Accesses base style information for the specified style.
        /// </summary>
        /// <param name="styleInfo">Style for which base style information is needed.</param>
        /// <param name="index">Index value where the style is stored.</param>
        /// <returns>Returns ChartStyleInfo array.</returns>
        public ChartStyleInfo[] GetBaseStyles(IStyleInfo styleInfo, int index)
        {
            ChartStyleInfo[] composedStyles = null;
            ChartStyleInfo chartStyleInfo = styleInfo as ChartStyleInfo;

            if (index == c_seriesIndex)
            {
                composedStyles = m_host.GetStylesMap().GetSubBaseStyles(chartStyleInfo, m_seriesStyle);
            }
            else
            {
                if (m_styles[index] == null)
                {
                    composedStyles = m_host.GetStylesMap().GetSubBaseStyles(chartStyleInfo, m_seriesStyle);
                }
                else
                {
                    ChartStyleInfo subStyleInfo = m_styles[index] as ChartStyleInfo;
                    composedStyles = m_host.GetStylesMap().GetSubBaseStyles(chartStyleInfo, new ChartStyleInfo[] { subStyleInfo, m_seriesStyle });
                }
            }

            return composedStyles;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises the style changed.
        /// </summary>
        /// <param name="index">The index.</param>
        protected virtual void RaiseStyleChanged(int index)
        {
            if (this.Changed != null)
            {
                this.Changed(this, new ChartStyleChangedEventArgs(ChartStyleChangedEventArgs.Type.Changed, index));
            }
        }
        #endregion
    }
}