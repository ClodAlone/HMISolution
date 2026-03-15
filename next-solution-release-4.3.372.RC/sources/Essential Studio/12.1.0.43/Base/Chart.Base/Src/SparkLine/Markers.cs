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
   public class Markers
    {
        bool m_showHighPoint = false;
        bool m_showLowPoint = false;
        bool m_showStartPoint = false;
        bool m_showEndPoint = false;
        bool m_showMarker = false;
        bool m_showNegativePoint = false;

        private BrushInfo m_markerColor = new BrushInfo(ColorTranslator.FromHtml("#244062"));
        private BrushInfo m_highPointColor = new BrushInfo(ColorTranslator.FromHtml("#4F81BD"));
        private BrushInfo m_lowPointColor = new BrushInfo(ColorTranslator.FromHtml("#4F81BD"));
        private BrushInfo m_startPointColor = new BrushInfo(ColorTranslator.FromHtml("#95B3D7"));
        private BrushInfo m_endPointColor = new BrushInfo(ColorTranslator.FromHtml("#95B3D7"));
        private BrushInfo m_negativePointColor = new BrushInfo(ColorTranslator.FromHtml("#C0504D"));

        /// <summary>
        /// Gets or sets a value indicating whether the High point marker are shown.
        /// </summary>
        public bool ShowHighPoint
        {
            get
            {
                return m_showHighPoint;
            }
            set
            {
                if (m_showHighPoint != value)
                {
                   m_showHighPoint = value;

                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the Low point marker are shown.
        /// </summary>
        public bool ShowLowPoint
        {
            get
            {
                return m_showLowPoint;
            }
            set
            {
                if (m_showLowPoint != value)
                {
                    m_showLowPoint = value;

                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the Start point marker are shown.
        /// </summary>
        public bool ShowStartPoint
        {
            get
            {
                return m_showStartPoint;
            }
            set
            {
                if (m_showStartPoint != value)
                {
                    m_showStartPoint = value;

                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the End point marker are shown.
        /// </summary>
        public bool ShowEndPoint
        {
            get
            {
                return m_showEndPoint;
            }
            set
            {
                if (m_showEndPoint != value)
                {
                    m_showEndPoint = value;

                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the marker are shown for all line points.
        /// </summary>
        public bool ShowMarker
        {
            get
            {
                return m_showMarker;
            }
            set
            {
                if (m_showMarker != value)
                {
                    m_showMarker = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the Negative point marker are shown.
        /// </summary>
        public bool ShowNegativePoint
        {
            get
            {
                return m_showNegativePoint;
            }
            set
            {
                if (m_showNegativePoint != value)
                {
                    m_showNegativePoint = value;
                }
            }
        }
        /// <summary>
        ///  Gets or sets Markercolor for all points.
        /// </summary>
        public BrushInfo MarkerColor
        {
            get
            {
                return m_markerColor;
            }
            set
            {
                if (m_markerColor != value)
                {
                    m_markerColor = value;
                }
            }
        }
        /// <summary>
        ///  Gets or sets Markercolor of the High point.
        /// </summary>
        public BrushInfo HighPointColor
        {
            get
            {
                return m_highPointColor;
            }
            set
            {
                if (m_highPointColor != value)
                {
                    m_highPointColor = value;
                }
            }
        }
        /// <summary>
        ///  Gets or sets Markercolor of the Low point.
        /// </summary>
        public BrushInfo LowPointColor
        {
            get
            {
                return m_lowPointColor;
            }
            set
            {
                if (m_lowPointColor != value)
                {
                    m_lowPointColor = value;
                }
            }
        }
        /// <summary>
        ///  Gets or sets Markercolor of the Start point.
        /// </summary>
        public BrushInfo StartPointColor
        {
            get
            {
                return m_startPointColor;
            }
            set
            {
                if (m_startPointColor != value)
                {
                    m_startPointColor = value;
                }
            }
        }
        /// <summary>
        ///  Gets or sets Markercolor of the End point.
        /// </summary>
        public BrushInfo EndPointColor
        {
            get
            {
                return m_endPointColor;
            }
            set
            {
                if (m_endPointColor != value)
                {
                    m_endPointColor = value;
                }
            }
        }
        /// <summary>
        ///  Gets or sets Markercolor of the Negative point.
        /// </summary>
        public BrushInfo NegativePointColor
        {
            get
            {
                return m_negativePointColor;
            }
            set
            {
                if (m_negativePointColor != value)
                {
                    m_negativePointColor = value;
                }
            }
        }


    
    }
}
