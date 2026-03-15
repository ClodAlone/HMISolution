#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Connector that draws as a poly line.
    /// </summary>
    [Serializable]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class PolyLineConnector
        : LineConnector
    {
        #region Class members
        private bool m_bMoveConnectedSegments;
        private float m_fCurveRadius = 8;
        #endregion
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLineConnector"/> class.
        /// </summary>
        /// <param name="ptStart">The pt start.</param>
        /// <param name="ptEnd">The pt end.</param>
        /// <param name="measureUnits">The measure units.</param>
        public PolyLineConnector(PointF ptStart, PointF ptEnd, MeasureUnits measureUnits)
            : base(ptStart, ptEnd, measureUnits)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLineConnector"/> class.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        public PolyLineConnector(PointF ptStart, PointF ptEnd)
            : base(ptStart, ptEnd)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLineConnector"/> class.
        /// </summary>
        /// <param name="pts">The points collection.</param>
        public PolyLineConnector(PointF[] pts)
            : base(pts)
        {
            if (pts == null || pts.Length < 1)
                throw new ArgumentOutOfRangeException("PolyLineConnector");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLineConnector"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public PolyLineConnector(PolyLineConnector src)
            : base(src)
        {
            m_bMoveConnectedSegments = src.m_bMoveConnectedSegments;
            m_fCurveRadius = src.m_fCurveRadius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLineConnector"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected PolyLineConnector(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "moveconnectedsegments":
                        m_bMoveConnectedSegments = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "CurveRadius":
                        m_fCurveRadius = float.Parse(entry.Value.ToString());
                        break;
                }
            }
        }
        #endregion
        #region Public properties
        /// <summary>
        /// Gets or sets a value indicating whether connected segments are movable.
        /// </summary>       
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Indicates whether connected segments are movable.")]
        public bool MoveConnectedSegments
        {
            get
            {
                return m_bMoveConnectedSegments;
            }
            set
            {
                if (m_bMoveConnectedSegments != value)
                    m_bMoveConnectedSegments = value;
            }
        }

        /// <summary>
        /// Gets or sets the radius of the curve.
        /// </summary>       
        [Browsable(true)]
        [DefaultValue(8f)]
        [Description("Radius of the curve")]
        public float CurveRadius
        {
            get { return m_fCurveRadius; }
            set
            {
                if (m_fCurveRadius != value && OnPropertyChanging(this.FullContainerName, DPN.CurveRadius, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.CurveRadius);

                    // set new value
                    m_fCurveRadius = value;

                    base.SetPointsInternal(this.PathPoints);

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.CurveRadius);
                }
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Indicate that selection and resize handles will draw on diagram canvas.
        /// </summary>
        /// <returns>
        /// <c>True</c> if node can be resized, otherwise - <c>false</c>.
        /// </returns>
        public override bool ShowResizeHandles()
        {
            return true;
        }

        /// <summary>
        /// Updates the Pin position, pin offset and size from new bounds rectangle.
        /// </summary>
        /// <param name="rcBounds">The bounds rectangle.</param>
        protected override void UpdateBoundsInfo(RectangleF rcBounds)
        {
            UpdateNonLineBoundsInfo(rcBounds);
        }

        /// <summary>
        /// Creates node's path with given array of points.
        /// </summary>
        /// <param name="pts">Points to create path from.</param>
        /// <returns>Created GraphicsPath, otherwise null.</returns>
        protected override GraphicsPath CreateLogicalGraphicsPath(PointF[] pts)
        {
            GraphicsPath gpPath = new GraphicsPath();
            float fcurveRadius = this.CurveRadius;
            if (!this.EnableRoundedCorner)
                // assign new GraphicsPath
                // get polygon's bounding rectangle
                gpPath.AddLines(pts);
            else
            {
                PointF previousEndPoint = PointF.Empty;
                for (int i = 1; i < pts.Length; i++)
                {
                    PointF startPoint = pts[i - 1];
                    PointF endPoint = pts[i];
                    float fLineLength = (float)Geometry.PointDistance(startPoint, endPoint);
                    if (fLineLength < CurveRadius * 2)
                        fcurveRadius = fLineLength / 2;
                    using (Pen pen = this.LineStyle.CreatePen())
                    {
                        if (i > 1)
                        {
                            PointF cornerPoint = startPoint;
                            LengthenLine(endPoint, ref startPoint, -fcurveRadius);
                            PointF controlPoint1 = cornerPoint;
                            PointF controlPoint2 = cornerPoint;
                            LengthenLine(previousEndPoint, ref controlPoint1, -fcurveRadius / 2);
                            LengthenLine(startPoint, ref controlPoint2, -fcurveRadius / 2);
                            gpPath.AddBezier(previousEndPoint, controlPoint1, controlPoint2, startPoint);
                        }
                        if (i + 1 < pts.Length) // shorten end point of all but the last line segment.
                            LengthenLine(startPoint, ref endPoint, -fcurveRadius);

                        gpPath.AddLine(startPoint, endPoint);
                        previousEndPoint = endPoint;
                    }
                }
            }

            return gpPath;
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("moveconnectedsegments", m_bMoveConnectedSegments);
            info.AddValue("CurveRadius", m_fCurveRadius);
        }

        #endregion
    }
}
