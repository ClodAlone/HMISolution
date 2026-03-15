#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Implement base line node object with display distance between two endpoint.
    /// </summary>
    [Serializable]
    public class MeasureLine
        : Line,
          IDeserializationCallback
    {
        #region Class members
        private string m_strText;
        private MeasureUnits m_unitsMeasure;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureLine"/> class.
        /// </summary>
        public MeasureLine()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureLine"/> class.
        /// </summary>
        /// <param name="pts">The array of points.</param>
        public MeasureLine(PointF[] pts)
            : base(pts)
        {
            ReMeasureLine(pts[0], pts[1]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureLine"/> class.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        public MeasureLine(PointF ptStart, PointF ptEnd)
            : base(ptStart, ptEnd)
        {
            ReMeasureLine(ptStart, ptEnd);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureLine"/> class.
        /// </summary>
        /// <param name="src">The LineNode instance.</param>
        public MeasureLine(MeasureLine src)
            : base(src)
        {
            m_unitsMeasure = src.m_unitsMeasure;
            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureLine"/> class.
        /// </summary>
        /// <param name="info">The serialization Info.</param>
        /// <param name="context">The streaming context.</param>
        protected MeasureLine(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_unitsMeasure = (MeasureUnits)info.GetValue("measureUnits", typeof(MeasureUnits));

            UpdateBoundingRectangle();

            Bitmap bmp = new Bitmap(1, 1);

            using (Graphics gfxTemp = Graphics.FromImage(bmp))
            {
                SizeF szText = GetTextWidth(m_strText, gfxTemp);

                if (szText.Height > this.BoundingRect.Height)
                {
                    RectangleF rcTemp = this.BoundingRect;

                    rcTemp.Y -= szText.Height / 2;
                    rcTemp.Height = szText.Height / 2;

                    this.BoundingRect = rcTemp;
                }
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the measure units.
        /// </summary>
        /// <value>The measure units.</value>
        [Browsable(true)]
        [Category("Misc")]
        [Description("Logical unit of measurement.")]
        public MeasureUnits MeasureUnits
        {
            get 
            { 
                return m_unitsMeasure; 
            }
            set
            {
                if (m_unitsMeasure != value && OnPropertyChanging(this.FullContainerName, DPN.MeasureUnits, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.MeasureUnits);

                    m_unitsMeasure = value;

                    ReMeasureLine(this.HeadEndPoint.Location, this.TailEndPoint.Location);

                    OnPropertyChanged(this.FullContainerName, DPN.MeasureUnits);
                }
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Accumulates the refresh rect.
        /// </summary>
        /// <param name="rcRefresh">The refresh rectangle.</param>
        protected override void AccumulateRefreshRect(ref RectangleF rcRefresh)
        {
            base.AccumulateRefreshRect(ref rcRefresh);

            if (m_strText != null)
            {
                RectangleF rcText = RectangleF.Empty;

                using (Graphics gfx = Graphics.FromHwnd(IntPtr.Zero))
                {
                    rcText.Size = GetTextWidth(m_strText, gfx);
                }

                float fLineLength =
                    (float)Geometry.PointDistance(this.HeadEndPoint.Location, this.TailEndPoint.Location);

                rcText.X = fLineLength / 2 - rcText.Width / 2;
                rcText.Y = -rcText.Height / 2;

                if (rcRefresh.Size.IsEmpty)
                {
                    rcRefresh = rcText;
                }
                else
                {
                    rcRefresh = RectangleF.Union(rcRefresh, rcText);
                }
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The object.</returns>
        public override object Clone()
        {
            return new MeasureLine(this);
        }

        /// <summary>
        /// Draws the path to specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected override void DrawPath(Graphics gfx)
        {
            float fLineLength =
                (float)Geometry.PointDistance(this.HeadEndPoint.Location, this.TailEndPoint.Location);

            ReMeasureLine(this.HeadEndPoint.Location, this.TailEndPoint.Location);

            // get text size
            SizeF szText = GetTextWidth(m_strText, gfx);
            float fHeight = 8 * gfx.PageScale;

            if (szText.Width > fLineLength)
            {
                float fontSize = (MeasureUnitsConverter.Convert(MeasureUnitsConverter.FromPixelX(gfx.PageScale, MeasureUnits.Point) * (MeasureUnitsConverter.FromPixelX(fHeight, MeasureUnits.Point)), MeasureUnits.Point, MeasureUnits.Inch));
                if (fontSize > (1 / 72f))
                gfx.DrawString(m_strText, new Font("Times New Ronam", fHeight), Brushes.Black, 0, -szText.Height / 2);
            }
            else
            {
                using (Pen pen = this.LineStyle.CreatePen())
                {
                    float tailDecoratorWidth = (this.TailDecorator != null && this.TailDecorator.GraphicsPath != null)
                        ? this.TailDecorator.GraphicsPath.GetBounds().Width : 0;
                    float headDecoratorWidth = (this.HeadDecorator != null && this.HeadDecorator.GraphicsPath != null)
                        ? this.HeadDecorator.GraphicsPath.GetBounds().Width : 0;
                    float length = (fLineLength - szText.Width) / 2;

                    // draw left line
                    if (tailDecoratorWidth < length && tailDecoratorWidth + headDecoratorWidth < fLineLength - szText.Width / 2)
                    {
                        gfx.DrawLine(pen, new PointF(tailDecoratorWidth, 0), new PointF(length, 0));
                    }

                    // draw text
                    float fontSize = (MeasureUnitsConverter.Convert(MeasureUnitsConverter.FromPixelX(gfx.PageScale, MeasureUnits.Point) * (MeasureUnitsConverter.FromPixelX(fHeight, MeasureUnits.Point)), MeasureUnits.Point, MeasureUnits.Inch));
                    if (fontSize > (1 / 72f))
                    gfx.DrawString(m_strText, new Font("Times New Ronam", fHeight), Brushes.Black, length, -szText.Height / 2);

                    // draw right line
                    if ((fLineLength - szText.Width) / 2 > headDecoratorWidth && tailDecoratorWidth + headDecoratorWidth < fLineLength - szText.Width / 2)
                    {
                        gfx.DrawLine(pen, new PointF(fLineLength / 2 + szText.Width / 2, 0), new PointF(fLineLength - headDecoratorWidth, 0));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The serialization Info.</param>
        /// <param name="context">The streaming context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("measureUnits", m_unitsMeasure);
        }
        #endregion

        #region Class helper methods
        private void ReMeasureLine(PointF ptStart, PointF ptEnd)
        {
            float fLineLength =
                (float)Geometry.PointDistance(ptStart, ptEnd);

            float unitDependentfLineLength = MeasureUnitsConverter.FromPixelX(fLineLength, this.MeasureUnits);
            m_strText = unitDependentfLineLength.ToString();

            if (m_bIsVertexEditable)
                m_bIsVertexEditable = false;

            UpdateBoundingRectangle();

            Bitmap bmp = new Bitmap(1, 1);

            using (Graphics gfxTemp = Graphics.FromImage(bmp))
            {
                SizeF szText = GetTextWidth(m_strText, gfxTemp);

                if (szText.Height > this.BoundingRect.Height)
                {
                    RectangleF rcTemp = this.BoundingRect;

                    rcTemp.Y -= szText.Height / 2;
                    rcTemp.Height = szText.Height / 2;

                    this.BoundingRect = rcTemp;
                }
            }
        }
        private SizeF GetTextWidth(string strMeasuring, Graphics gfx)
        {
            float fHeight = 8 * gfx.PageScale;
            return gfx.MeasureString(strMeasuring, new Font("Times New Roman", fHeight));
        }
        #endregion

        #region IDeserializationCallback Members
        /// <summary>
        /// Runs when the entire object graph has been deserialized.
        /// </summary>
        /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
        public void OnDeserialization(object sender)
        {
            float fLineLength =
                (float)Geometry.PointDistance(this.HeadEndPoint.Location, this.TailEndPoint.Location);

            float unitDependentfLineLength = MeasureUnitsConverter.FromPixelX(fLineLength, this.MeasureUnits);
            m_strText = unitDependentfLineLength.ToString();
        }
        #endregion
    }
}
