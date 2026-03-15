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
using Syncfusion.Drawing;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    public class Column
    {
        #region Member
        private float m_columnSpace = 1;

        private BrushInfo m_columnColor = new BrushInfo(ColorTranslator.FromHtml("#244062"));

        #endregion

        #region Property
        /// <summary>
        ///  Gets or sets column space of the each point.
        /// </summary>
        public float ColumnSpace
        {
            get
            {
                return m_columnSpace;
            }
            set
            {
                if (m_columnSpace != value)
                {
                    m_columnSpace = value;

                }
            }
        }
        /// <summary>
        ///  Gets or sets columncolor of the column series.
        /// </summary>
        public BrushInfo ColumnColor
        {
            get
            {
                return m_columnColor;
            }
            set
            {
                if (m_columnColor != value)
                {
                    m_columnColor = value;
                }
            }
        }

        #endregion
    }
}
