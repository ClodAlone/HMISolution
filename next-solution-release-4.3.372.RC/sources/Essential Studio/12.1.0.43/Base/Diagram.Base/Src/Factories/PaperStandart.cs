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

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Paper standard information.
    /// </summary>
    public sealed class PaperStandart
    {
        #region Public static properties
        /// <summary>
        /// Gets the allowed paper standards.
        /// </summary>
        /// <returns>The allowed paper standards.</returns>
        public static PaperStandart[] GetStandarts()
        {
            PaperStandart standart = new PaperStandart("Standard");
            Bitmap bmp = new Bitmap(1, 1);
            Graphics gfx = Graphics.FromImage(bmp);
            float dpi;
            dpi = gfx.DpiX;
            // Loose sizes
            standart.m_pageSizes = new PageSize[] 
            {
                new PageSize( "Letter: 8.5 in x 11 in", 8.5f, MeasureUnits.Inch, 11f, MeasureUnits.Inch ),
                new PageSize( "Legal: 8.5 in x 14 in", 8.5f, MeasureUnits.Inch, 14f, MeasureUnits.Inch ),
                new PageSize( "Folio: 8.5 in x 13 in", 8.5f, MeasureUnits.Inch, 13f, MeasureUnits.Inch ),
                new PageSize( "Tabloid: 11 in x 17 in", 11f, MeasureUnits.Inch, 17f, MeasureUnits.Inch )
            };

            PaperStandart metric = new PaperStandart("Metric (ISO)");
            
            // page sizes taken from print settings
            metric.m_pageSizes = new PageSize[]
            {
                new PageSize( "A5: 148 mm x 210 mm", (float)Math.Floor(148/25.4f*dpi), MeasureUnits.Pixel, (float)Math.Floor(210/25.4f*dpi), MeasureUnits.Pixel ),
                new PageSize( "A4: 210 mm x 297 mm", (float)Math.Floor(210/25.4f*dpi), MeasureUnits.Pixel, (float)Math.Floor(297/25.4f*dpi), MeasureUnits.Pixel ),
                new PageSize( "A3: 297 mm x 420 mm", (float)Math.Floor(297/25.4f*dpi), MeasureUnits.Pixel, (float)Math.Floor(420/25.4f*dpi), MeasureUnits.Pixel ),
                new PageSize( "A2: 420 mm x 594 mm", (float)Math.Floor(420/25.4f*dpi), MeasureUnits.Pixel, (float)Math.Floor(594/25.4f*dpi), MeasureUnits.Pixel ),
                new PageSize( "A1: 594 mm x 841 mm", (float)Math.Floor(594/25.4f*dpi), MeasureUnits.Pixel, (float)Math.Floor(841/25.4f*dpi), MeasureUnits.Pixel ),
                new PageSize( "A0: 841 mm x 1189 mm", (float)Math.Floor(841/25.4f*dpi), MeasureUnits.Pixel, (float)Math.Floor(1189/25.4f*dpi), MeasureUnits.Pixel )
            };

            PaperStandart engineering = new PaperStandart("ANSI Engineering");
            
            // ANSI paper sizes
            engineering.m_pageSizes = new PageSize[]
            {
                new PageSize( "A: 8.5 in x 11 in", 8.5f, MeasureUnits.Inch, 11, MeasureUnits.Inch ),
                new PageSize( "B: 11 in x 17 in", 11, MeasureUnits.Inch, 17, MeasureUnits.Inch ),
                new PageSize( "C: 17 in x 22 in", 17, MeasureUnits.Inch, 22, MeasureUnits.Inch),
                new PageSize( "D: 22 in x 34 in", 22, MeasureUnits.Inch, 34, MeasureUnits.Inch),
                new PageSize( "E: 34 in x 44 in", 34, MeasureUnits.Inch, 44, MeasureUnits.Inch )
            };

            PaperStandart architectural = new PaperStandart("ANSI Architectural");
            
            // Architectural sizes
            architectural.m_pageSizes = new PageSize[]
            {
                new PageSize( "9 in x 12 in", 9, MeasureUnits.Inch, 12, MeasureUnits.Inch ),
                new PageSize( "12 in x 18 in", 12, MeasureUnits.Inch, 18, MeasureUnits.Inch ),
                new PageSize( "18 in x 24 in", 18, MeasureUnits.Inch, 24, MeasureUnits.Inch ),
                new PageSize( "24 in x 36 in", 24, MeasureUnits.Inch, 36, MeasureUnits.Inch ),
                new PageSize( "30 in x 42 in", 30, MeasureUnits.Inch, 42, MeasureUnits.Inch ),
            };

            return new PaperStandart[] { standart, metric, engineering, architectural };
        }
        #endregion

        #region Class members
        private string m_strName;
        private PageSize[] m_pageSizes;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        /// <summary>
        /// Gets or sets the page sizes.
        /// </summary>
        /// <value>The page sizes.</value>
        public PageSize[] PageSizes
        {
            get
            {
                if (m_pageSizes == null)
                    m_pageSizes = new PageSize[] { };

                return m_pageSizes;
            }
            set
            {
                m_pageSizes = value;
            }
        }
        #endregion

        #region Class initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="PaperStandart"/> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        public PaperStandart(string displayName)
        {
            m_strName = displayName;
        }
        #endregion

        #region Class overrides methods
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.DisplayName;
        }
        #endregion
    }

    /// <summary>
    /// Paper standard scale information.
    /// </summary>
    public sealed class PaperScaleStandart
    {
        #region Public static properties
        /// <summary>
        /// Gets the allowed scale standards.
        /// </summary>
        /// <returns>The paper scale standards.</returns>
        public static PaperScaleStandart[] GetStandarts()
        {
            PaperScaleStandart architectural = new PaperScaleStandart("Architectural");
            architectural.PageScales = new PageScale[]
            {
                new PageScale( "3/32\" = 1'0\"", 36f / 32f, 1f ),
                new PageScale( "1/8\" = 1'0\"", 12f / 8f, 1f ),
                new PageScale( "3/16\" = 1'0\"", 36f / 16f, 1f ),
                new PageScale( "1/4\" = 1'0\"", 12f / 4f, 1f ),
                new PageScale( "3/8\" = 1'0\"", 36f / 8f, 1f ),
                new PageScale( "1/2\" = 1'0\"", 12f / 2f, 1f ),
                new PageScale( "3/4\" = 1'0\"", 36f / 4f, 1f ),
                new PageScale( "1/5\" = 1'0\"", 12f / 5f, 1f ),
                new PageScale( "3\" = 1'0\"", 36f, 1f ),
                new PageScale( "1' = 1'0\"", 1f, 1f )
            };

            PaperScaleStandart metric = new PaperScaleStandart("Metric");
            metric.PageScales = new PageScale[]
            {
                new PageScale( "1 : 1000", 1, 1000 ),
                new PageScale( "1 : 500", 1, 500 ),
                new PageScale( "1 : 200", 1, 200 ),
                new PageScale( "1 : 100", 1, 100 ),
                new PageScale( "1 : 50", 1, 50 ),
                new PageScale( "1 : 25", 1, 25 ),
                new PageScale( "1 : 20", 1, 20 ),
                new PageScale( "1 : 10", 1, 10 ),
                new PageScale( "1 : 5", 1, 5 ),
                new PageScale( "1 : 2.5", 1, 2.5f ),
                new PageScale( "1 : 2", 1, 2 ),
                new PageScale( "1 : 1", 1, 1 ),
                new PageScale( "10 : 1", 10, 1 ),
                new PageScale( "20 : 1", 20, 1 ),
                new PageScale( "50 : 1", 50, 1 )
            };

            return new PaperScaleStandart[] { architectural, metric };
        }
        #endregion

        #region Class members
        private PageScale[] m_pageScales;
        private string m_strDisplayName;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return m_strDisplayName; }
            set { m_strDisplayName = value; }
        }

        /// <summary>
        /// Gets or sets the page scales.
        /// </summary>
        /// <value>The page scales.</value>
        public PageScale[] PageScales
        {
            get { return m_pageScales; }
            set { m_pageScales = value; }
        }
        #endregion

        #region Class initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="PaperScaleStandart"/> class.
        /// </summary>
        /// <param name="name">The display name.</param>
        public PaperScaleStandart(string name)
        {
            m_strDisplayName = name;
        }
        #endregion

        #region Class overrides methods
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.DisplayName;
        }
        #endregion
    }
}
