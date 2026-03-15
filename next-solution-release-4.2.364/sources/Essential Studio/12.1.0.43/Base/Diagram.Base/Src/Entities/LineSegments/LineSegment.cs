#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
//  Author: Jeff Boenig
//
#endregion

using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Encapsulates two points that comprise a line segment.
	/// </summary>
	[Serializable]
	public class LineSegment
		: ISerializable
		, ICloneable
	{
		#region Class members
        /// <summary>
        /// 
        /// </summary>
		protected PointF m_pt1;
        /// <summary>
        /// 
        /// </summary>
		protected PointF m_pt2;
		/// <summary>
		/// The segment index.
		/// </summary>
		public int nSegmentIndex;
		#endregion

		#region Class initialize/finalize methods
		/// <summary>
		/// Constructs a line segment given two points.
		/// </summary>
		/// <param name="ptStartPoint">First point in line segment.</param>
		/// <param name="ptEndPoint">Second point in line segment.</param>
		public LineSegment( PointF ptStartPoint, PointF ptEndPoint )
		{
			m_pt1 = ptStartPoint;
			m_pt2 = ptEndPoint;
			nSegmentIndex = 0;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="LineSegment"/> class.
		/// </summary>
		/// <param name="ptStartPoint">The given point.</param>
		/// <param name="ptEndPoint">The given point.</param>
		/// <param name="orientation">The orientation.</param>
		public LineSegment(PointF ptStartPoint, PointF ptEndPoint, Orientation orientation )
		{
			m_pt1 = ptStartPoint;
			m_pt2 = ptEndPoint;
			nSegmentIndex = 0;
		}
		/// <summary>
		/// Constructs a line segment given two points.
		/// </summary>
		/// <param name="ptStartPoint">First point in line segment.</param>
		/// <param name="ptEndPoint">Second point in line segment.</param>
		/// <param name="segmentIndex">Segment index.</param>
		public LineSegment(PointF ptStartPoint, PointF ptEndPoint, int segmentIndex)
		{
			m_pt1 = ptStartPoint;
			m_pt2 = ptEndPoint;
			nSegmentIndex = segmentIndex;
		}
		/// <summary>
		/// Constructs a line segment from an array of lines and a segment index.
		/// </summary>
		/// <param name="pts">Points that make up the polyline.</param>
		/// <param name="segIdx">Zero-based offset of segment.</param>
		public LineSegment(PointF[] pts, int segIdx)
		{
			int pt1Idx = segIdx;
			int pt2Idx = segIdx + 1;
			if (pt1Idx < 0 || pt2Idx > (pts.Length - 1))
			{
				throw new ArgumentOutOfRangeException( "segIdx", segIdx,
				                                       "Segment index is out of bounds for the given set of points." );
			}

			m_pt1 = pts[pt1Idx];
			m_pt2 = pts[pt2Idx];
			nSegmentIndex = segIdx;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="LineSegment"/> class.
		/// </summary>
		/// <param name="src">The source instance.</param>
		public LineSegment( LineSegment src )
		{
			m_pt1 = src.m_pt1;
			m_pt2 = src.m_pt2;
			nSegmentIndex = src.nSegmentIndex;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="LineSegment"/> class.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		protected LineSegment( SerializationInfo info, StreamingContext context )
		{
		}
		#endregion
		
		#region Class properties
		/// <summary>
		/// First point in the line segment.
		/// </summary>
		public virtual PointF Point1
		{
			get{ return m_pt1; }
			set{ m_pt1 = value;	}
		}
		/// <summary>
		/// Second point in the line segment.
		/// </summary>
		public virtual PointF Point2
		{
			get{ return m_pt2; }
			set{ m_pt2 = value;	}
		}
		/// <summary>
		/// Gets the midpoint of the linesegment.
		/// </summary>
		public PointF MidPoint
		{
			get
			{
				return new PointF((this.Point1.X + this.Point2.X) / 2, (this.Point1.Y + this.Point2.Y) / 2);
			}
		}
		#endregion
		
		#region ISerializable Members
		/// <summary>
		/// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
		void ISerializable.GetObjectData( SerializationInfo info, StreamingContext context )
		{
			GetObjectData( info, context );
		}
		#endregion
		
		#region ICloneable Members
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public virtual object Clone()
		{
			return new LineSegment( this );
		}
		#endregion

		#region Class public methods
		/// <summary>
		/// Gets the orthogonal intersect.
		/// </summary>
		/// <param name="lineSeg">The line segment.</param>
		/// <param name="ptIntersect">The intersect point.</param>
		/// <returns></returns>
		public bool GetOrthogonalIntersect(LineSegment lineSeg, out PointF ptIntersect)
		{
			return Geometry.GetOrthogonalIntersect(this.Point1, this.Point2, lineSeg.Point1, lineSeg.Point2, out ptIntersect);
		}
		#endregion
		
		#region Class utility methods
		/// <summary>
		/// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
		protected virtual void GetObjectData( SerializationInfo info, StreamingContext context )
		{
		}
		#endregion
	}
}
