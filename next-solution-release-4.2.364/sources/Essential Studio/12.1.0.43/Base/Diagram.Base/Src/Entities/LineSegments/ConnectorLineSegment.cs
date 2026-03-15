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
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Connector line segment.
    /// </summary>
	[ Serializable ]
	public class ConnectorLineSegment
		: LineSegment,
		 IConnectorLineSegment
	{
		#region Class members
        /// <summary>
        /// Collection of bridges.
        /// </summary>
		private ArrayList m_bridges;

        /// <summary>
        /// 
        /// </summary>
		protected IHandle m_firstHandle;

        /// <summary>
        /// 
        /// </summary>
		protected IHandle m_secondHandle;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets the bridges.
		/// </summary>
		/// <value>The bridges.</value>
		public ArrayList Bridges
		{
			get
			{
				if( m_bridges == null )
				{
					m_bridges = new ArrayList();
				}
				
				return m_bridges;
			}
		}
		/// <summary>
		/// First point in the line segment.
		/// </summary>
		/// <value></value>
		public override PointF Point1
		{
			get
			{
				return GetHandleLocation( m_firstHandle );
			}
			set
			{
				// value can't be set to handle
			}
		}
		/// <summary>
		/// Second point in the line segment.
		/// </summary>
		/// <value></value>
		public override PointF Point2
		{
			get
			{
				return GetHandleLocation( m_secondHandle );
			}
			set
			{
				// value can't be set to handle
			}
		}
		#endregion

		#region Class initialize/finalize methods
		/// <summary>
		/// Constructs a line segment given two points.
		/// </summary>
		/// <param name="firstHandle">First point in line segment.</param>
		/// <param name="secondHandle">Second point in line segment.</param>
		public ConnectorLineSegment( IHandle firstHandle, IHandle secondHandle )
			: base( GetHandleLocation( firstHandle ), GetHandleLocation(secondHandle ) )
		{
			m_firstHandle = firstHandle;
			m_secondHandle = secondHandle;
		}
		/// <summary>
		/// Constructs a line segment given two points.
		/// </summary>
		/// <param name="firstHandle">First point in line segment.</param>
		/// <param name="secondHandle">Second point in line segment.</param>
		/// <param name="segmentIndex">Line segment index.</param>
		public ConnectorLineSegment( IHandle firstHandle, IHandle secondHandle , int segmentIndex )
			: base( GetHandleLocation( firstHandle), GetHandleLocation(secondHandle ), segmentIndex )
		{
			m_firstHandle = firstHandle;
			m_secondHandle = secondHandle;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectorLineSegment"/> class.
		/// </summary>
		/// <param name="src">The SRC.</param>
		public ConnectorLineSegment( ConnectorLineSegment src )
			: base( src )
		{}
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectorLineSegment"/> class.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		protected ConnectorLineSegment( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{}
		#endregion
		
		#region Class public methods
		/// <summary>
		/// Calculate first point location.
		/// </summary>
		/// <param name="mtx">Parent transformation. If equals null return Point1 property.</param>
		/// <returns>First point location.</returns>
		public PointF GetFirstPointLocation( Matrix mtx )
		{
			if( mtx == null )
			{
				return Point1;
			}
			else
			{
				return GetHandleLocation( m_firstHandle, mtx );
			}

		}
		/// <summary>
		/// Calculate second point location.
		/// </summary>
		/// <param name="mtx">Parent transformation. If equals null return Point2 property.</param>
		/// <returns>Second point location.</returns>
		public PointF GetSecondPointLocation( Matrix mtx )
		{
			if( mtx == null )
			{
				return Point2;
			}
			else
			{
				return GetHandleLocation( m_secondHandle, mtx );
			}

		}
		#endregion

		#region IConnectorLineSegment
		/// <summary>
		/// Move segment to offset.
		/// </summary>
		/// <param name="szOffset">Move offset.</param>
		public virtual void Move( SizeF szOffset )
		{
			MeasureUnits units = MeasureUnits.Pixel;

			// move first handle of segment
			if( CanMove() )
			{
				m_firstHandle.Move( szOffset, units );
				m_secondHandle.Move( szOffset, units );
			}
		}
		/// <summary>
		/// Synchronization segment to handle changes.
		/// </summary>
		/// <param name="handle">The handle.</param>
		public virtual void SyncSegmentHandle( IHandle handle)
		{}
		#endregion

		#region Class helper methods
		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new ConnectorLineSegment( this );
		}
		/// <summary>
		/// Determines whether segment can move.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if segment can move; otherwise, <c>false</c>.
		/// </returns>
		protected bool CanMove()
		{
			bool bSuccess = ( m_firstHandle != null && m_secondHandle != null );
			EndPoint endPoint;

			if( bSuccess )
			{
				endPoint = m_firstHandle as EndPoint;

				// check first handle
				if( endPoint != null && endPoint.Port != null )
					bSuccess = false;
			}
			
			if( bSuccess )
			{
				endPoint = m_secondHandle as EndPoint;
			
				// check second handle
				if( endPoint != null && endPoint.Port != null )
					bSuccess = false;
			}

			return bSuccess;
		}
		#endregion

		#region Class static methods
		/// <summary>
		/// Gets the handle location in local coordinates.
		/// </summary>
		/// <param name="handle">The handle.</param>
		/// <returns></returns>
		private static PointF GetHandleLocation( IHandle handle )
		{
			
			if( handle == null )
				return PointF.Empty;

			PointF ptLocation = handle.Location;

			EndPoint endPoint = handle as EndPoint;

			// if handle is control point then handle
			// location need convert from local to model coordinates
			if( endPoint != null && endPoint.Container != null )
			{
				Matrix mtxTransformation = endPoint.Container.GetTransformations();
				endPoint.Container.AppendFlipTransforms(mtxTransformation);
				return GetHandleLocation( handle, mtxTransformation );
			}

			return ptLocation;
		}
		/// <summary>
		/// Gets the handle location in local coordinates.
		/// </summary>
		/// <param name="handle">The handle.</param>
		/// <param name="mtx"></param>
		/// <returns></returns>
		private static PointF GetHandleLocation( IHandle handle, Matrix mtx )
		{
			if( handle == null )
				return PointF.Empty;

			PointF ptLocation = handle.Location;
			EndPoint endPoint = handle as EndPoint;

			// if handle is control point then handle
			// location need convert from local to model coordinates
			if( endPoint != null && endPoint.Container != null )
			{
				PointF[] pts = new PointF[]{ ptLocation };

				mtx.Invert();
				mtx.TransformPoints( pts );
				mtx.Invert();

				ptLocation = pts[ 0 ];
			}

			return ptLocation;
		}
		#endregion
	}
}
