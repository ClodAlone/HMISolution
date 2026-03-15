#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Custom orthogonal line segment for orthogonal connector.
	/// </summary>
	[ Serializable ]
	public class OrthogonalLineSegment 
		: ConnectorLineSegment
	{
		#region Class members
		/// <summary>
		/// Indicate orientation of segment what set inside.
		/// </summary>
		private Orientation m_orientation;
		#endregion

		#region Class properties
		/// <summary>
		/// Orientation of segment
		/// </summary>
		public Orientation Orientation
		{
			get
			{
				return m_orientation;
			}
			set
			{
				if( value != m_orientation )
				{
					m_orientation = value;
				}
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Initializes a new instance of the <see cref="OrthogonalLineSegment"/> class.
		/// </summary>
		/// <param name="firstHandle">The first handle.</param>
		/// <param name="secondHandle">The second handle.</param>
		public OrthogonalLineSegment( IHandle firstHandle, IHandle secondHandle )
			: base( firstHandle, secondHandle )
		{
			if( firstHandle == null || secondHandle == null )
				throw new ArgumentNullException( "Segment handles can't be null." );

			m_firstHandle = firstHandle;
			m_secondHandle = secondHandle;

			m_orientation = Orientation.Horizontal;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OrthogonalLineSegment"/> class.
		/// </summary>
		/// <param name="firstHandle">The first handle.</param>
		/// <param name="secondHandle">The second handle.</param>
		/// <param name="orientation">The segment orientation.</param>
		public OrthogonalLineSegment( IHandle firstHandle, IHandle secondHandle, Orientation orientation )
			: this( firstHandle, secondHandle )
		{
			m_orientation = orientation;
		}

		#endregion

		#region Class overrides
		/// <summary>
		/// Move segment to offset.
		/// </summary>
		/// <param name="szOffset">Move offset.</param>
		public override void Move(SizeF szOffset)
		{
			// get offset to segment orientation
			szOffset = GetOrientationOffset( szOffset );

			// move segment handles
			base.Move( szOffset );
		}
		/// <summary>
		/// Synchronization segment to handle changes.
		/// </summary>
		/// <param name="handle">The handle.</param>
		public override void SyncSegmentHandle(IHandle handle )
		{ 
			// update first handle of segment
			if( handle != m_firstHandle && m_firstHandle != null )
			{
				UpdatePosition( m_firstHandle, m_secondHandle );
			}

			// update second handle of segment
			if( handle != m_secondHandle && m_secondHandle != null )
			{
				UpdatePosition( m_secondHandle, m_firstHandle );
			}
		}
		#endregion

		#region Class helper methods
		/// <summary>
		/// Gets the orientation offset.
		/// </summary>
		/// <param name="szOffset">The offset size.</param>
		/// <returns></returns>
		private SizeF GetOrientationOffset( SizeF szOffset )
		{
			SizeF szToReturn = szOffset;
			Orientation orientation = this.Orientation;

			if( orientation == Orientation.Horizontal )
				szToReturn.Width = 0;
			else 
				szToReturn.Height = 0;

			return szToReturn;
		}
		/// <summary>
		/// Update syncHandle position to lockHandle.
		/// </summary>
		/// <param name="syncHandle">The handle to synchronize.</param>
		/// <param name="lockHandle">The handle synchornize to.</param>
		private void UpdatePosition( IHandle syncHandle, IHandle lockHandle )
		{
			PointF ptSync = GetHandleLocation( syncHandle );
			PointF ptLock = GetHandleLocation( lockHandle );

			if( this.Orientation == Orientation.Vertical )
			{
				ptSync.X = ptLock.X;
			}
			else 
			{
				ptSync.Y = ptLock.Y;
			}

			SetHandleLocation( syncHandle, ptSync );
		}
		/// <summary>
		/// Get the handle location in container local coordinates.
		/// </summary>
		/// <param name="handle">The handle.</param>
		/// <returns>Handle location in container local coordinates.</returns>
		private PointF GetHandleLocation( IHandle handle )
		{
			// get location in local coordinates
			PointF ptPointToReturn = handle.Location;
			EndPoint endPoint = handle as EndPoint;

			// transform end point to local coordinates
			if( endPoint != null )
			{
				PathNode parent = endPoint.Container;
				
				if( parent != null )
				{
					PointF[] pts = new PointF[]{ ptPointToReturn };
					Matrix mtxTransfrom = parent.GetTransformations();
					parent.AppendFlipTransforms( mtxTransfrom );
					mtxTransfrom.Invert();

					// set new point to return
					mtxTransfrom.TransformPoints( pts );
					ptPointToReturn = pts[0];
				}
			}

			return ptPointToReturn;
		}
		/// <summary>
		/// Set the handle location in local coordinates.
		/// </summary>
		/// <param name="handle">The handle.</param>
		/// <param name="ptSet">The point in container local coordinates.</param>
		private void SetHandleLocation( IHandle handle, PointF ptSet )
		{
			EndPoint endPoint = handle as EndPoint;
			ControlPoint cntPoint = handle as ControlPoint;

			// transform end point to parent coordinates
			if( endPoint != null )
			{
				PathNode parent = endPoint.Container;
				
				// append container transformations
				if( parent != null )
				{
					PointF[] pts = new PointF[]{ ptSet };
					Matrix mtxTransfrom = parent.GetTransformations();
					parent.AppendFlipTransforms( mtxTransfrom );

					// set new location
					mtxTransfrom.TransformPoints( pts );
					endPoint.Location = pts[0];
				}
			}
			// set control point location in local coordinates
			else if( cntPoint != null )
			{
				cntPoint.Location = ptSet;
			}
		}
		#endregion
	}
}
