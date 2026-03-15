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
using System.Diagnostics;

using Syncfusion.Diagnostics;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Chart
{
    internal class ChartStyleInfoIdentity : StyleInfoIdentityBase
    {
        #region Members
        private ChartSeriesComposedStylesModel m_data;
        private int m_index;
        private IStyleInfo[] m_baseStyles = null;
        private bool m_isOffLine;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStyleInfoIdentity"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="offLine">if set to <c>true</c> style is in offline.</param>
        public ChartStyleInfoIdentity(ChartSeriesComposedStylesModel data, int index, bool offLine)
        {
            m_data = data;
            m_index = index;
            m_isOffLine = offLine;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns an array with base styles for the specified style object.
        /// </summary>
        /// <param name="thisStyleInfo">The style object.</param>
        /// <returns>
        /// An array of style objects that are base styles for the current style object.
        /// </returns>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            if (m_baseStyles == null)
            {
                m_baseStyles = m_data.GetBaseStyles(thisStyleInfo, m_index);
            }

            return m_baseStyles;
        }

        /// <summary>
        /// Occurs when a property in the <see cref="StyleInfoBase"/> has changed.
        /// </summary>
        /// <param name="style">The <see cref="StyleInfoBase"/> instance that has changed.</param>
        /// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            if (!m_isOffLine)
            {
                TraceUtil.TraceCurrentMethodInfo(sip.PropertyName, sip.FormatValue(style.GetValue(sip)));
                m_baseStyles = null;
                m_data.ChangeStyle(style as ChartStyleInfo, m_index);
            }
        }
        #endregion
    }
}