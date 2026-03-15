#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Implements the ChartControl printing feature.
    /// </summary>
    [ToolboxItem(false)]
    public sealed class ChartPrintDocument : PrintDocument
    {
        #region Members
        private ChartControl m_chart = null;
        private ChartPrintColorMode m_colorMode = ChartPrintColorMode.CheckPrinter;
        private PrintAction? m_currectAction = null;
        private bool m_printToolBar = false;
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets or sets the color mode.
        /// </summary>
        /// <value>The color mode.</value>
        [DefaultValue(ChartPrintColorMode.CheckPrinter)]
        public ChartPrintColorMode ColorMode
        {
            get
            {
                return m_colorMode;
            }

            set
            {
                m_colorMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tool bar should be printed.
        /// </summary>
        /// <value><c>true</c> if tool bar should be printed; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool PrintToolBar
        {
            get { return m_printToolBar; }

            set { m_printToolBar = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPrintDocument"/> class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        internal ChartPrintDocument(ChartControl chart)
        {
            if (chart == null)
                throw new ArgumentNullException(chart.Localization.ChartNullException);

            m_chart = chart;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises the <see cref="E:System.Drawing.Printing.PrintDocument.BeginPrint"/> event. It is called after the <see cref="M:System.Drawing.Printing.PrintDocument.Print"/> method is called and before the first page of the document prints.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Drawing.Printing.PrintEventArgs"/> that contains the event data.</param>
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            m_currectAction = e.PrintAction;

            base.OnBeginPrint(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Drawing.Printing.PrintDocument.PrintPage"/> event. It is called before a page prints.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Drawing.Printing.PrintPageEventArgs"/> that contains the event data.</param>
        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            bool grayScale = m_colorMode == ChartPrintColorMode.GrayScale;
            bool toolBatVisibility = m_chart.ShowToolbar;

            if (!m_printToolBar)
            {
                m_chart.ShowToolbar = false;
            }

            if (m_currectAction.Value == PrintAction.PrintToPrinter
                && m_colorMode == ChartPrintColorMode.CheckPrinter)
            {
                grayScale = this.PrinterSettings.SupportsColor;
            }

            ////Here I will check whether the printer supports colors
            ////An experimental code (it doesn't work correctly, but I can't delete it :( - "backward compatibility" )
            if (!grayScale)
            {
                GraphicsContainer container = ChartControl.BeginTransform(e.Graphics);
                e.Graphics.ResetTransform();

                m_chart.Draw(e.Graphics, e.MarginBounds);

                ChartControl.EndTransform(e.Graphics, container);
            }
            else if (grayScale)
            {
                ChartStyleInfo[] tempStyles = new ChartStyleInfo[m_chart.Series.Count];
                Array ps = System.Enum.GetValues(typeof(PatternStyle));
                Array ds = System.Enum.GetValues(typeof(DashStyle));

                for (int i = 0; i < m_chart.Series.Count; i++)
                {
                    tempStyles[i] = new ChartStyleInfo();
                    tempStyles[i].CopyFrom(m_chart.Series[i].StylesImpl.Style);

                    m_chart.Series[i].Style.Interior = new BrushInfo((PatternStyle)ps.GetValue(i % ps.Length), Color.Black, Color.White);
                    m_chart.Series[i].Style.Border.MakeCopy(tempStyles[i], m_chart.Series[i].Style.Border.Sip);
                    m_chart.Series[i].Style.Border.Color = Color.Black;
                    m_chart.Series[i].Style.Border.DashStyle = (DashStyle)ds.GetValue(i % ds.Length);

                    if (m_chart.Series[i].Type == ChartSeriesType.Line || m_chart.Series[i].Type == ChartSeriesType.Spline || m_chart.Series[i].Type == ChartSeriesType.StepLine || m_chart.Series[i].Type == ChartSeriesType.RotatedSpline)
                    {
                        m_chart.Series[i].Style.Interior = new BrushInfo((PatternStyle)ps.GetValue(i % ps.Length), Color.White, Color.Black);

                        if (m_chart.Series3D || m_chart.ChartInterior.BackColor == Color.Black)
                        {
                            m_chart.Series[i].Style.Interior = new BrushInfo((PatternStyle)ps.GetValue(i % ps.Length), Color.Black, Color.White);
                            m_chart.Series[i].Style.Border.Color = Color.Black;

                        }                      
                    }
                }

                GraphicsContainer container = ChartControl.BeginTransform(e.Graphics);
                e.Graphics.ResetTransform();

                using (Image img = new Bitmap(e.MarginBounds.Width, e.MarginBounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(img))
                    {
                        IntPtr hdc = g.GetHdc();
                        Stream stream = new MemoryStream();
                        Metafile mf = new Metafile(stream, hdc);

                        m_chart.Draw(mf, img.Size);

                        DrawingUtils.DrawGrayedImage(e.Graphics, mf, e.MarginBounds, new Rectangle(Point.Empty, img.Size));

                        g.ReleaseHdc(hdc);
                        g.Dispose();
                        mf.Dispose();
                    }
                }

                ChartControl.EndTransform(e.Graphics, container);

                for (int i = 0; i < m_chart.Series.Count; i++)
                {
                    m_chart.Series[i].StylesImpl.Style.ResetInterior();
                    m_chart.Series[i].StylesImpl.Style.ResetBorder();
                    m_chart.Series[i].StylesImpl.Style.CopyFrom(tempStyles[i]);
                }
            }
            ////END A little experimental code

            if (!m_printToolBar)
            {
                m_chart.ShowToolbar = toolBatVisibility;
            }

            m_chart.Redraw(true);

            base.OnPrintPage(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Drawing.Printing.PrintDocument.EndPrint"/> event. It is called when the last page of the document has printed.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Drawing.Printing.PrintEventArgs"/> that contains the event data.</param>
        protected override void OnEndPrint(PrintEventArgs e)
        {
            m_currectAction = null;
            base.OnEndPrint(e);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (m_chart != null)
            {
                m_chart = null;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
