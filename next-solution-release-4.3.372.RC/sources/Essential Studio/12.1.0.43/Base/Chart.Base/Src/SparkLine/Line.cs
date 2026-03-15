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
using System.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    public class Line
    {
        #region Member
        private float m_lineWidth = 1;

        private Color m_lineColor = ColorTranslator.FromHtml("#8494A7");

        #endregion

        #region Property

        /// <summary>
        ///  Gets or sets line color of the Line series.
        /// </summary>
        public Color LineColor
        {
            get
            {
                return m_lineColor;
            }
            set
            {
                if (m_lineColor != value)
                {
                    m_lineColor = value;
                }
            }
        }
        /// <summary>
        ///  Gets or sets width of the line.
        ///  Internally Only
        /// </summary>
        internal float LineWidth
        {
            get
            {
                return m_lineWidth;
            }
            set
            {
                if (m_lineWidth != value)
                {
                    m_lineWidth = value;

                }
            }
        }

        #endregion
       
    }
}
