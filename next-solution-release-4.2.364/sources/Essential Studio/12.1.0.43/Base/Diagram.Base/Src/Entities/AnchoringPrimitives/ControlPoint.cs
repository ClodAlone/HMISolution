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
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.ComponentModel;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Summary description for ControlPoint.
	/// </summary>
	[ Serializable ]
	public class ControlPoint
		: PropertyContainer
		, IHandle
	{
		#region Class members
		private PointF m_ptLocation;
		[ NonSerialized ]
		private PathNode m_container;
		private bool m_bCanMoveX;
		private bool m_bCanMoveY;
		private int m_nID;
		private HandleMoved m_handleMovedCallback;
        private bool m_bIsMoving = false;
        private bool m_bInUpdate = false;
		#endregion

		#region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPoint"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="ptLocation">The pt location.</param>
        /// <param name="nID">The n ID.</param>
		public ControlPoint( PathNode container, PointF ptLocation, int nID )
		{
			if( container == null )
				throw new ArgumentNullException( "container" );
			
			this.Container = container;
			m_ptLocation = ptLocation;
			m_bCanMoveX = true;
			m_bCanMoveY = true;
			m_nID = nID;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPoint"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
		public ControlPoint( ControlPoint src )
		{
			m_nID = src.m_nID;
			m_ptLocation = src.m_ptLocation;
			m_bCanMoveX = src.m_bCanMoveX;
			m_bCanMoveY = src.m_bCanMoveY;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlPoint"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
		protected ControlPoint( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
			m_ptLocation = ( PointF ) info.GetValue( "location", typeof( PointF ) );
			m_bCanMoveX = info.GetBoolean( "canMoveX" );
			m_bCanMoveY = info.GetBoolean( "canMoveY" );
			m_nID = info.GetInt32( "ID" );
		}
		#endregion

		#region Class properties
		/// <summary>
		/// Gets or sets the container.
		/// </summary>
		/// <value>The container.</value>
		public PathNode Container
		{
			get{ return m_container; }
			set
			{
				if( m_container != value )
				{
					m_container = value;

					if( m_container != null )
					{
						m_handleMovedCallback = ( HandleMoved )Delegate.CreateDelegate( typeof( HandleMoved ), m_container, "HandleMove" );
					}
					else
					{
						m_handleMovedCallback = null;
					}
				}
			}
		}
		#endregion
		
		#region IHandle Members
        /// <summary>
        /// Gets or sets a value indicating whether the handle is updating
        /// </summary>
        /// <value><c>true</c> if handle is updating; otherwise, <c>false</c>.</value>
        public bool InUpdate
        {
            get
            {
                return m_bInUpdate;
            }
            set
            {
                if (value != m_bInUpdate)
                    m_bInUpdate = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the handle is moving
        /// </summary>
        /// <value><c>true</c> if handle is moving; otherwise, <c>false</c>.</value>
        public bool IsMoving
        {
            get
            {
                return m_bIsMoving;
            }
            set
            {
                if (value != m_bIsMoving)
                    m_bIsMoving = value;
            }
        }

		/// <summary>
		/// Gets the ID.
		/// </summary>
		/// <value>The ID.</value>
		public int ID
		{
			get{ return m_nID; }
		}
		/// <summary>
		/// Gets or sets the location in local coordinates.
		/// </summary>
		/// <value>The location.</value>
		public PointF Location
		{
			get{ return m_ptLocation; }
			set
			{
				if( m_ptLocation != value && OnVertexChanging( value ) )
				{
					// make history entry
					RecordHandleMove();
					// pause history service
					if( this.HistoryService != null )
					{
						this.HistoryService.Pause();
					}

					SizeF szOffset = new SizeF( value.X - m_ptLocation.X, value.Y - m_ptLocation.Y );
					// assign new value
					m_ptLocation = value;

					// update container's data
					if( m_handleMovedCallback != null )
					{
						m_handleMovedCallback( this, szOffset );
					}

					// resume history service
					if( this.HistoryService != null )
					{
						this.HistoryService.Resume();
					}
                    if (this.Container != null && !this.Container.BoundsInfo.IsResizing)
                        // raise property changes event					
                        OnVertexChanged();
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether instance allow move by X axis.
		/// </summary>
		/// <value><c>true</c> if allow to move by X axis; otherwise, <c>false</c>.</value>
		[ Browsable( false ) ]
		[ DefaultValue( true ) ]
		public bool AllowMoveX
		{
			get { return m_bCanMoveX; }
			set { m_bCanMoveX = value; }
		}
		/// <summary>
		/// Gets or sets a value indicating whether instance allow move by Y axis.
		/// </summary>
		/// <value><c>true</c> if allow to move by Y axis; otherwise, <c>false</c>.</value>
		[ Browsable( false ) ]
		[ DefaultValue( true ) ]
		public bool AllowMoveY
		{
			get { return m_bCanMoveY; }
			set { m_bCanMoveY = value; }
		}
		/// <summary>
		/// Moves the specified offset.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <param name="offsetUnits">The offset units.</param>
		public void Move( SizeF offset, MeasureUnits offsetUnits )
		{
			if( m_container != null )
			{
				// convert to pixels
				offset = MeasureUnitsConverter.Convert( offset, offsetUnits, MeasureUnits.Pixel );
				
				// cals new position
				PointF ptPoint = ((IUnitIndependent)Container).GetPinPoint( MeasureUnits.Pixel );

				PointF[] pts = new PointF[]{ m_ptLocation };

				Matrix matrix = new Matrix();
				matrix.RotateAt( Geometry.ConvertToFullCircle( Container.RotationAngle ), ptPoint );
				matrix.TransformPoints( pts );
				
				pts[ 0 ].X += offset.Width;
				pts[ 0 ].Y += offset.Height;

				matrix.Invert();
				matrix.TransformPoints( pts );
				
				this.Location = pts[ 0 ];
			}
		}
		#endregion
		
		#region Class overrides
		/// <summary>
		/// Called when vertex position changing.
		/// </summary>
		/// <param name="newValue">The new value.</param>
		/// <returns></returns>
		protected bool OnVertexChanging( PointF newValue )
		{
		    bool bSuccess = true;
			PathNode pathNode = this.Container;

			if( pathNode != null )
			{
				int  nVertexIdx = Array.IndexOf( pathNode.GetControlPoints(), this );

				// if node contain end points increment index
				if( pathNode is  IEndPointContainer )
					nVertexIdx++;

			    bSuccess = pathNode.OnVertexChanging( VertexChangeType.Set, nVertexIdx, newValue );
			}

			return bSuccess;
		}
		/// <summary>
		/// Called when vertex position changed.
		/// </summary>
		protected void OnVertexChanged()
		{
			PathNode pathNode = this.Container;

			if( pathNode != null )
			{
				int  nVertexIdx = Array.IndexOf( pathNode.GetControlPoints(), this );

				// if node contain end points increment index
				if( pathNode is  IEndPointContainer )
					nVertexIdx++;

			    pathNode.OnVertexChanged( VertexChangeType.Set, nVertexIdx, this.Location );
			}
		}
		/// <summary>
		/// Gets the name of the property container.
		/// </summary>
		/// <returns></returns>
		protected override string GetPropertyContainerName()
		{
			return null;
		}
		/// <summary>
		/// Called when measure units property changing.
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		protected override void OnMeasureUnitsChanging( MeasureUnits from, MeasureUnits to )
		{
			base.OnMeasureUnitsChanging( from, to );
			
			m_ptLocation = MeasureUnitsConverter.Convert( m_ptLocation, from, to );
		}
		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new ControlPoint( this );
		}
		/// <summary>
		/// Gets the object data.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		protected override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData( info, context );
			
			info.AddValue( "location", m_ptLocation );
			info.AddValue( "canMoveX", m_bCanMoveX );
			info.AddValue( "canMoveY", m_bCanMoveY );
			info.AddValue( "ID", m_nID );
		}
		#endregion

		#region Class helper methods
		/// <summary>
		/// Record the handle move action.
		/// </summary>
		private void RecordHandleMove()
		{
			if( this.HistoryService != null )
			{
				this.HistoryService.RecordMoveHandle( this.Container );
			}
		}
		#endregion
	}
}
