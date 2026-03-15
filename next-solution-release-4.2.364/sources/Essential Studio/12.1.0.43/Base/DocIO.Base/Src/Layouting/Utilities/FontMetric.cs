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

#if !SILVERLIGHT && !WP

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Helper class, used for getting font ascent/descent
    /// </summary>
    internal class FontMetric
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private Graphics m_bmpG;
        /// <summary>
        /// 
        /// </summary>
        private Graphics m_g;
        /// <summary>
        /// 
        /// </summary>
        private Font m_font;
        /// <summary>
        /// 
        /// </summary>
        private UnitsConvertor m_convertor;
        #endregion

        #region properties
        /// <summary>
        /// Gets the ascent.
        /// </summary>
        /// <value>The ascent.</value>
        public double Ascent
        {
            get
            {
                int height = m_font.FontFamily.GetEmHeight(m_font.Style);
                int ascent = m_font.FontFamily.GetCellAscent(m_font.Style);

                double ascentInPoints = (double)(m_font.SizeInPoints * ascent) / (double)height;
                switch (m_g.PageUnit)
                {
                    case GraphicsUnit.Pixel:
                        //return m_metric.otmMacAscent;
                        return m_convertor.ConvertToPixels((double)ascentInPoints, PrintUnits.Point);
                    case GraphicsUnit.Point:
                        //return m_convertor.ConvertFromPixels(ascentInPoints, PrintUnits.Point);
                        return (double)ascentInPoints;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Gets the descent.
        /// </summary>
        /// <value>The descent.</value>
        public double Descent
        {
            get
            {
                int height = m_font.FontFamily.GetEmHeight(m_font.Style);
                int descent = m_font.FontFamily.GetCellDescent(m_font.Style);
                double descentInPoints = (double)(m_font.SizeInPoints * descent) / (double)height;
                switch (m_g.PageUnit)
                {
                    case GraphicsUnit.Pixel:
                        //return m_metric.otmMacDescent;
                        return m_convertor.ConvertToPixels((double)descentInPoints, PrintUnits.Point);
                    case GraphicsUnit.Point:
                        //return m_convertor.ConvertFromPixels( m_metric.otmMacDescent, PrintUnits.Point );
                        return (double)descentInPoints;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Gets the BMP graphics.
        /// </summary>
        /// <value>The BMP graphics.</value>
        protected Graphics BmpGraphics
        {
            get
            {
                if (m_bmpG == null)
                {
                    Bitmap bmp = new Bitmap(1, 1);
                    m_bmpG = Graphics.FromImage(bmp);
                }

                return m_bmpG;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FontMetric"/> class.
        /// </summary>
        public FontMetric()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontMetric"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public FontMetric(Font font)
        {
            UpdateMetricData(null, font);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontMetric"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="g">The g.</param>
        public FontMetric(Font font, Graphics g)
        {
            UpdateMetricData(g, font);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Updates the metric data.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="font">The font.</param>
        public void UpdateMetricData(Graphics g, Font font)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            m_font = font;
            UpdateMetricData(g);
        }

        /// <summary>
        /// Updates the metric data.
        /// </summary>
        /// <param name="g">The g.</param>
        public void UpdateMetricData(Graphics g)
        {
            m_g = (g == null) ? BmpGraphics : g;
            m_convertor = new UnitsConvertor(m_g);
            //ParseFontData();
        }
        #endregion
    }
}

#endif