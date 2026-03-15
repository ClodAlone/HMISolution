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
	/// The bases connector endPoint handle class that implement
	/// moving, protection and serialization. Used by <see cref="Syncfusion.Windows.Forms.Diagram.ConnectorBase"/> 
	/// and <see cref="Syncfusion.Windows.Forms.Diagram.Line"/> that implemented
	/// <see cref="Syncfusion.Windows.Forms.Diagram.IEndPointContainer"/> interface.
	/// </summary>
	/// <remarks>
	/// Used as base class for <see cref="Syncfusion.Windows.Forms.Diagram.HeadEndPoint"/> 
	/// and <see cref="Syncfusion.Windows.Forms.Diagram.TailEndPoint"/> classes.
	/// </remarks>
	[ Serializable ]
	public abstract class EndPoint
		: PropertyContainer
		,IHandle
	{
		#region Class members
		private PointF m_ptLocation;
		[ NonSerialized ]
		private PathNode m_container;
		private ConnectionPoint m_port;
		private bool m_bCanMoveX;
		private bool m_bCanMoveY;
		private HandleMoved m_handleMovedCallback;
        private bool m_bIsMoving = false;
        private bool m_bInUpdate = false;
		/// <summary>
		/// Call container MegreControlPoints and UpdatePathNodeData methods.
		/// </summary>
		private UpdateCallback m_mergePointsCallback;
		#endregion

		#region Class initialize/finalize methods
		/// <summary>
		/// Initializes a new instance of the <see cref="EndPoint"/> class.
		/// </summary>
		/// <param name="container">The <see cref="Syncfusion.Windows.Forms.Diagram.PathNode"/> container.</param>
		/// <param name="ptLocation">The handle location in parent coordinates.</param>
		public EndPoint( PathNode container, PointF ptLocation )
		{
			if( container == null )
				throw new ArgumentNullException( "container" );
			
			this.Container = container;
			m_bCanMoveX = true;
			m_bCanMoveY = true;
			m_ptLocation = ptLocation;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="EndPoint"/> class.
		/// </summary>
		/// <param name="src">The source instance.</param>
		public EndPoint( EndPoint src )
		{
			m_ptLocation = src.m_ptLocation;
			m_bCanMoveX = src.m_bCanMoveX;
			m_bCanMoveY = src.m_bCanMoveY;
            m_port = src.m_port;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="EndPoint"/> class.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		protected EndPoint( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
			m_ptLocation = ( PointF ) info.GetValue( "location", typeof( PointF ) );
			m_port = ( ConnectionPoint ) info.GetValue( "port", typeof( ConnectionPoint ) );
			m_bCanMoveX = info.GetBoolean( "canMoveX" );
			m_bCanMoveY = info.GetBoolean( "canMoveY" );
		}
		#endregion

		#region Class properties
		/// <summary>
		/// Gets or sets the end point container.
		/// </summary>
		/// <value>The PathNode container.</value>
		[ Browsable( false ) ]
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
						Delegate delegMerge = ( UpdateCallback )Delegate.CreateDelegate( typeof( UpdateCallback ), m_container, "MergeControlPoints" );
						Delegate delegUpdate = ( UpdateCallback )Delegate.CreateDelegate( typeof( UpdateCallback ), m_container, "UpdatePathNodeData" );

						m_mergePointsCallback = ( UpdateCallback )Delegate.Combine( delegMerge, delegUpdate );
					}
					else
					{
						m_handleMovedCallback = null;
						m_mergePointsCallback = null;
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets the port which end point is connected. 
		/// Return null if end point isn't connected.
		/// </summary>
		/// <value>The port which end point is connected.</value>
		[ Browsable( false ) ]
		public ConnectionPoint Port
		{
			get{ return m_port; }
			set
			{
				if( m_port != value )
				{
					Model model = ( this.Container != null )  ? this.Container.Root : null;

					if (model != null)
						model.BeginUpdate();

					m_port = value;

					if ( model != null )
					{
						model.LinkManager.SynchronizeEndPoint( this );
						model.EndUpdate();
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
		/// Gets the EndPoint ID.
		/// </summary>
		/// <value>The ID.</value>
		/// <remarks>Always return 0.</remarks>
		[ Browsable( false ) ]
		public int ID
		{
			get{ return 0; }
		}
		/// <summary>
		/// Gets or sets the endpoint location in parent coordinates.
		/// </summary>
		/// <value>The location in parent coordinates.</value>
		[ Browsable( true ) ]
		[ Description( "EndPoint location." ) ]
		[ TypeConverter( typeof( SizeFConverter ) ) ]
		public PointF Location
		{
			get { return m_ptLocation; }
			set
			{
				// new location can be set only
				// if endpoint is not connected with any port
				// otherwise it will synchronize its location through SyncPositionWithPort
//				if( this.Port == null )
					SetLocation( value );
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether handle can move by X axis.
		/// </summary>
		/// <value><c>true</c> if handle can move by X axis; otherwise, <c>false</c>.</value>
		[ Browsable( true ) ]
		[ DefaultValue( true ) ]
		[ Description( "Indicates whether moving along X axis is allowed." ) ]
		public bool AllowMoveX
		{
			get{ return m_bCanMoveX; }
			set
			{
				if( m_bCanMoveX != value && OnPropertyChanging( DPN.AllowMoveX, value ) )
				{
					RecordPropertyChanged( DPN.AllowMoveX );
					
					m_bCanMoveX = value;
					
					OnPropertyChanged( DPN.AllowMoveX );
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether can move by Y axis.
		/// </summary>
		/// <value><c>true</c> if handle can move by Y axis; otherwise, <c>false</c>.</value>
		[ Browsable( true ) ]
		[ DefaultValue( true ) ]
		[ Description( "Indicates whether moving along Y axis is allowed." ) ]
		public bool AllowMoveY
		{
			get{ return m_bCanMoveY; }
			set
			{
				if( m_bCanMoveY != value && OnPropertyChanging( DPN.AllowMoveY, value ) )
				{
					RecordPropertyChanged( DPN.AllowMoveY );
					
					m_bCanMoveY = value;
					
					OnPropertyChanged( DPN.AllowMoveY );
				}
			}
		}
		/// <summary>
		/// Move end point to the specified offset.
		/// </summary>
		/// <param name="offset">The move offset.</param>
		/// <param name="offsetUnits">The offset units.</param>
		public void Move( SizeF offset, MeasureUnits offsetUnits )
		{
			// convert to pixels
			offset = MeasureUnitsConverter.Convert( offset, offsetUnits, MeasureUnits.Pixel );

			if( m_container != null )
			{
				// Append flips
				if( this.Container.FlipX )
					offset.Width = -offset.Width;

				if( this.Container.FlipY )
					offset.Height = -offset.Height;
			}
			
			this.Location = new PointF( m_ptLocation.X + offset.Width, m_ptLocation.Y + offset.Height );
		}
		#endregion

		#region Class overrides
		/// <summary>
		/// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
		protected override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData( info, context );
			
			info.AddValue( "location", m_ptLocation );
			info.AddValue( "port", m_port );
			info.AddValue( "canMoveX", m_bCanMoveX );
			info.AddValue( "canMoveY", m_bCanMoveY );
		}
		#endregion
		
		#region Public Methods
		/// <summary>
		/// Get the heading to this end point taken from container.
		/// </summary>
		/// <returns>Current heading value.</returns>
		public CompassHeading GetHeading()
		{
			// set heading by default
			CompassHeading heading = CompassHeading.None;
			ConnectorBase connector = this.Container as ConnectorBase;

			// get heading value from container node
			if( connector != null )
			{
				heading = ( this is HeadEndPoint ) ? connector.HeadingHead : connector.HeadingTail;
			}

			// retun result value
			return heading;
		}
		/// <summary>
		/// Synchronizes EndPoint position with connected ConnectionPoint.
		/// </summary>
		[Obsolete("This method is obsolete. Please use Model.LinkManager.SynchronizeEndPoint(EndPoint endpoint).")]
		public void SyncPositionWithPort()
		{
			if (this.Container != null )
			{
				Model model = this.Container.Root;

				if( model != null )
					model.LinkManager.SynchronizeEndPoint( this );
			}
		}
		#endregion
		
		#region Helper Methods
		/// <summary>
		/// Called when vertex position changing.
		/// </summary>
		/// <param name="newValue">The new position value.</param>
		/// <returns></returns>
        protected bool OnVertexChanging( PointF newValue )
        {
            bool bSuccess = true;
            PathNode pathNode = this.Container;

            if( pathNode != null )
            {
                int nVertexIdx = Geometry.GetEndPointIndex( this, pathNode.PointCount, 0 );

                bSuccess =
                    pathNode.OnVertexChanging( VertexChangeType.Set, nVertexIdx, newValue );
            }

            return bSuccess;
        }
		/// <summary>
		/// Called when vertex changed.
		/// </summary>
	    protected void OnVertexChanged()
        {
            PathNode pathNode = this.Container;

            if( pathNode != null )
            {
                int nVertexIdx = Geometry.GetEndPointIndex( this, pathNode.PointCount, 0 );

                pathNode.OnVertexChanged( VertexChangeType.Set, nVertexIdx, this.Location );
            }
        }
	    /// <summary>
		/// Record the handle move action to history.
		/// </summary>
		private void RecordHandleMove()
		{
			if( this.HistoryService != null )
			{
				this.HistoryService.RecordMoveHandle( this.Container );
			}
		}
		/// <summary>
		/// Sets the new location value.
		/// </summary>
		/// <param name="value">The new value.</param>
		protected void SetLocation( PointF value )
		{
			// check if node can be rotated
			if (m_container != null && !m_container.CanMoveHandle( this, value ) )
				return;

			if( m_ptLocation != value && OnVertexChanging( value ) )
			{
				RecordHandleMove();
					
				if( this.HistoryService != null )
					this.HistoryService.Pause();

				SizeF szOffset = new SizeF( value.X - m_ptLocation.X, value.Y - m_ptLocation.Y );
				// assign new value
				m_ptLocation = value;

				// update container's data
				if( m_handleMovedCallback != null )
				{
					m_handleMovedCallback( this, szOffset );
				}

				if( this.HistoryService != null )
					this.HistoryService.Resume();

                if (this.Container != null && !this.Container.BoundsInfo.IsResizing)
                    OnVertexChanged();
			}
		}
		#endregion
	}
}
