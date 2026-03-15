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

using System;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit.Implementation
{
	/// <summary>
	/// Cursor manager. Manages translation of the 
	/// cursor coordinates between different coordinate systems.
	/// </summary>
	public class CursorManager
		: ICursorManager
		, ICursorVirtualCoordinates
		, ICursorPhysicalCoordinates
		, ICursorGraphicalCoordinates
		, IDisposable
	{
		#region Class Members
		/// <summary>
		/// Visibility of the cursor.
		/// Note: Cursor will be visible only when owner control has focus.
		/// </summary>
		private bool m_bVisible;
		/// <summary>
		/// Control, that is owner of the cursor.
		/// </summary>
		private Control m_Owner;
		/// <summary>
		/// Carret object.
		/// Is available only when control is focused.
		/// </summary>
		private GdiCaret m_caret;
		/// <summary>
		/// Carret size.
		/// </summary>
		private Size m_size = new Size( -1, 0 );
		/// <summary>
		/// Carret location on the screen.
		/// </summary>
		private Point m_location;
		/// <summary>
		/// Current virtual point of the cursor.
		/// </summary>
		private Point m_CurrentVirtualPoint = new Point( 1, 1 );
		/// <summary>
		/// Current physical point of the cursor.
		/// </summary>
		private IParsePoint m_CurrentPhysicalPoint;
		/// <summary>
		/// Converter of the positions.
		/// </summary>
		private IPositionConverter m_converter;
		/// <summary>
		/// Specifies whether virtual space mode is enabled.
		/// </summary>
		private bool m_bVirtualSpaceMode;
		/// <summary>
		/// Virtual coordinates, specified on last coordinates update.
		/// </summary>
		private Point m_pointVirtualOnLastUpdate;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets value that specifies whether virtual space mode is enabled.
		/// </summary>
		public bool VirtualSpaceMode
		{
			get
			{
				return m_bVirtualSpaceMode;
			}
			set
			{
				if( value != m_bVirtualSpaceMode )
				{
					m_bVirtualSpaceMode = value;

					if( value )
						Update();
				}
			}
		}
		/// <summary>
		/// Gets GdiCaret.
		/// </summary>
		internal GdiCaret Caret
		{
			get
			{
				return m_caret;
			}
		}
		/// <summary>
		/// Gets or sets visibility of the cursor.
		/// </summary>
		public bool Visible
		{
			get
			{
				return m_bVisible;
			}
			set
			{
				if( m_bVisible != value )
				{
					m_bVisible = value;

					Update();
				}
			}
		}
		/// <summary>
		/// Gets control, that is the owner of the cursor and control's it's visibility.
		/// </summary>
		public Control Owner
		{
			get
			{
				return m_Owner;
			}
		}
		/// <summary>
		/// Gets virtual coordinates of the cursor.
		/// </summary>
		public ICursorVirtualCoordinates CursorVirtualCoordinates
		{
			get
			{
				return this as ICursorVirtualCoordinates;
			}
		}
		/// <summary>
		/// Gets physical coordinates of the cursor. 
		/// </summary>
		public ICursorPhysicalCoordinates CursorPhysicalCoordinates
		{
			get
			{
				return this as ICursorPhysicalCoordinates;
			}
		}
		/// <summary>
		/// Gets graphical coordinates of the cursor.
		/// </summary>
		public ICursorGraphicalCoordinates CursorGraphicalCoordinates
		{
			get
			{
				return this as ICursorGraphicalCoordinates;
			}
		}
		/// <summary>
		/// Gets or sets left-top point of the cursor in client coordinates.
		/// </summary>
		Point ICursorGraphicalCoordinates.LeftTop
		{
			get
			{
				return m_location;
			}
			set
			{
				if( m_location != value )
				{
					Point point = m_converter.GraphicalToVirtual( value );

					if( point != Point.Empty )
					{
						RaiseBeforeCoordinatesChanged( point );

						bool bLineChanged = ( m_CurrentVirtualPoint.Y != point.Y );
						int oldLine = m_CurrentVirtualPoint.Y;

						m_CurrentVirtualPoint = point;
						Update();
						RaiseCoordinatesChangedEvent();

						if( bLineChanged )
						{
							RaiseLineChangedEvent( oldLine, point.Y );
						}
					}
				}
			}
		}
		/// <summary>
		/// Gets size of the cursor.
		/// </summary>
		Size ICursorGraphicalCoordinates.Size
		{
			get
			{
				Size size = m_size;

				if( size.Width == -1 )
					size.Width = 2;

				return size;
			}
			set
			{
				if( value != m_size )
				{
					m_size = value;

					Update();
				}
			}
		}
		/// <summary>
		/// Gets rectangle, occupied by cursor.
		/// </summary>
		Rectangle ICursorGraphicalCoordinates.Rectangle
		{
			get
			{
				return new Rectangle( m_location, m_size );
			}
		}
		/// <summary>
		/// Gets or sets cursor's virtual line index.
		/// </summary>
		int ICursorVirtualCoordinates.Line
		{
			get
			{
				return m_CurrentVirtualPoint.Y;
			}
			set
			{
				if( m_CurrentVirtualPoint.Y != value )
				{
					RaiseBeforeCoordinatesChanged( new Point( m_CurrentVirtualPoint.X, value ) );
					int oldLine = m_CurrentVirtualPoint.Y;
					m_CurrentVirtualPoint.Y = value;

					Update();

					RaiseCoordinatesChangedEvent();
					RaiseLineChangedEvent( oldLine, value );
				}
			}
		}
		/// <summary>
		/// Gets or sets cursor's virtual column index.
		/// </summary>
		int ICursorVirtualCoordinates.Column
		{
			get
			{
				return m_CurrentVirtualPoint.X;
			}
			set
			{
				if( m_CurrentVirtualPoint.X != value )
				{
					RaiseBeforeCoordinatesChanged( new Point( value, m_CurrentVirtualPoint.Y ) );

					m_CurrentVirtualPoint.X = value;

					Update();

					RaiseCoordinatesChangedEvent();
				}
			}
		}
		/// <summary>
		/// Gets or sets cursor's virtual position.
		/// </summary>
		Point ICursorVirtualCoordinates.Position
		{
			get
			{
				return m_CurrentVirtualPoint;
			}
			set
			{
				if( m_CurrentVirtualPoint != value )
				{
					RaiseBeforeCoordinatesChanged( value );

					bool bLineChanged = ( m_CurrentVirtualPoint.Y != value.Y );
					int oldLine = m_CurrentVirtualPoint.Y;

					m_CurrentVirtualPoint = value;
					Update();
					RaiseCoordinatesChangedEvent();

					if( bLineChanged )
					{
						RaiseLineChangedEvent( oldLine, value.Y );
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets cursor's physical line index.
		/// </summary>
		int ICursorPhysicalCoordinates.Line
		{
			get
			{
				if( m_CurrentPhysicalPoint == null )
					m_CurrentPhysicalPoint = m_converter.VirtualToPhysical( m_CurrentVirtualPoint );

				return m_CurrentPhysicalPoint.Line;
			}
		}
		/// <summary>
		/// Gets or sets cursor's physical column index.
		/// </summary>
		int ICursorPhysicalCoordinates.Column
		{
			get
			{
				if( m_CurrentPhysicalPoint == null )
					m_CurrentPhysicalPoint = m_converter.VirtualToPhysical( m_CurrentVirtualPoint );

				return m_CurrentPhysicalPoint.Position;
			}
		}
		/// <summary>
		/// Gets or sets cursor's physical position.
		/// </summary>
		IParsePoint ICursorPhysicalCoordinates.Position
		{
			get
			{
				if( m_CurrentPhysicalPoint == null )
					m_CurrentPhysicalPoint = m_converter.VirtualToPhysical( m_CurrentVirtualPoint );

				return m_CurrentPhysicalPoint;
			}
			set
			{
				if( m_CurrentPhysicalPoint == null )
					m_CurrentPhysicalPoint = m_converter.VirtualToPhysical( m_CurrentVirtualPoint );

				if( m_CurrentPhysicalPoint != value )
				{
					Point p = m_converter.PhysicalToVirtual( value );

					RaiseBeforeCoordinatesChanged( p );

					( ( ICursorVirtualCoordinates )this ).Position = p;
				}
			}
		}
		/// <summary>
		/// Gets converter of the positions.
		/// </summary>
		public IPositionConverter PositionConverter
		{
			get
			{
				return m_converter;
			}
		}
		#endregion

		#region Class Events
		/// <summary>
		/// Event, raised when position of the cursor was changed.
		/// </summary>
		public event EventHandler CoordinatesChanged;
		/// <summary>
		/// Raised before coordinates are about to change.
		/// </summary>
		public event CoordinatesChangeEventHandler BeforeCoordinatesChange;
		/// <summary>
		/// Raised when line of the cursor was changed.
		/// </summary>
		public event ValueChangedEventHandler LineChanged;
		#endregion

		#region Class Initialization & Finalization
		/// <summary>
		/// Creates and initalizes new instance of the class.
		/// </summary>
		/// <param name="owner">Owner of the cursor.</param>
		/// <param name="converter">Instance of position converter.</param>
		public CursorManager( Control owner, IPositionConverter converter )
		{
			if( owner == null )
				throw new ArgumentNullException( "owner" );

			if( converter == null )
				throw new ArgumentNullException( "converter" );

			m_Owner = owner;
			m_converter = converter;

			m_Owner.GotFocus += new EventHandler( m_Owner_GotFocus );
			m_Owner.LostFocus += new EventHandler( m_Owner_LostFocus );
			m_Owner.Resize += new EventHandler( m_Owner_Resize );
		}
		/// <summary>
		/// Cursor's manager finalizer.
		/// </summary>
		~CursorManager()
		{
			Dispose();
		}
		/// <summary>
		/// Disposes current carret and detaches event handlers from control.
		/// </summary>
		public virtual void Dispose()
		{
			if( m_Owner == null )
				return;

			DestroyCaret();

			m_Owner.GotFocus -= new EventHandler( m_Owner_GotFocus );
			m_Owner.LostFocus -= new EventHandler( m_Owner_LostFocus );
			m_Owner.Resize -= new EventHandler( m_Owner_Resize );
			GC.SuppressFinalize( this );
			m_Owner = null;
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Updates cursor's parameters.
		/// </summary>
		/// <remarks>
		/// All updates of coordinates are based on virtual position.
		/// </remarks>
		public void Update()
		{
			m_pointVirtualOnLastUpdate = m_CurrentVirtualPoint;
			Point newVirtual = m_converter.CorrectVirtual( m_CurrentVirtualPoint, VirtualSpaceMode );

			if( m_CurrentVirtualPoint != newVirtual )
			{
				( ( ICursorVirtualCoordinates )this ).Position = newVirtual;
			}

			RectangleF rect = m_converter.VirtualToGraphical( m_CurrentVirtualPoint );
			m_CurrentPhysicalPoint = null;

			m_location = Point.Round( rect.Location );
			m_location.X += 2;

			UpdateGraphicalCorinateToRTL();

			if( rect.Height != m_size.Height ||
				( m_size.Width != -1 && m_size.Width != rect.Width ) )
			{
				m_size.Height = ( int )rect.Height;

				if( m_size.Width != -1 )
					m_size.Width = ( int )rect.Width;

				DestroyCaret();
			}

			if( ( m_caret == null ) && m_Owner.Focused )
				CreateCaret();
			else if( ( m_caret != null ) && !m_Owner.Focused )
				DestroyCaret();

			if( m_caret != null )
			{
				UpdatePosition();
				UpdateVisibility();
			}
		}

        private void UpdateGraphicalCorinateToRTL()
        {
            StreamEditControl edit = this.Owner as StreamEditControl;
            if (edit != null && edit.RightToLeft == RightToLeft.Yes)
            {
                m_location = new Point(edit.Width - edit.ScrollOffsetRight - edit.ScrollOffsetLeft - this.CursorGraphicalCoordinates.Rectangle.Width - m_location.X - 2, m_location.Y);
            }
        }
		#endregion

		#region Class Helper Methods
		/// <summary>
		/// Raises CoordinatesChanged event.
		/// </summary>
		protected void RaiseCoordinatesChangedEvent()
		{
			if( CoordinatesChanged != null )
				CoordinatesChanged( this, EventArgs.Empty );
		}
		/// <summary>
		/// Raises LineChanged event.
		/// </summary>
		protected void RaiseLineChangedEvent( int oldLine, int newLine )
		{
			if( LineChanged != null )
				LineChanged( this, new ValueChangedEventArgs( oldLine, newLine ) );
		}

		/// <summary>
		/// Raises BeforeCoordinatesChanged event.
		/// </summary>
		protected void RaiseBeforeCoordinatesChanged( Point newPoint )
		{
			if( BeforeCoordinatesChange != null )
				BeforeCoordinatesChange( this, new CoordinatesChangeEventArgs( newPoint ) );
		}
		/// <summary>
		/// Updates position of the cursor.
		/// </summary>
		protected virtual void UpdatePosition()
		{
			m_caret.Position = CursorGraphicalCoordinates.LeftTop;
		}
		/// <summary>
		/// Updates visibility of the cursor.
		/// </summary>
		protected virtual void UpdateVisibility()
		{
			m_caret.Visible = m_bVisible;
		}
		/// <summary>
		/// Creates caret.
		/// </summary>
		protected virtual void CreateCaret()
		{
			if( m_caret != null || !m_Owner.Focused ) return;

			Size size = CursorGraphicalCoordinates.Size;

			if( size.Width == -1 )
				size.Width = 2;

			m_caret = new GdiCaret( m_Owner, size );
			UpdatePosition();
			UpdateVisibility();
		}
		/// <summary>
		/// Destroys caret.
		/// </summary>
		protected virtual void DestroyCaret()
		{
			if( m_caret == null ) return;

			m_caret.Dispose();
			m_caret = null;
		}
		#endregion

		#region Class Event Handlers
		/// <summary>
		/// Updates visibility of cursor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_Owner_Resize( object sender, EventArgs e )
		{
			Update();
		}
		/// <summary>
		/// Updates visibility of cursor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_Owner_GotFocus( object sender, EventArgs e )
		{
			CreateCaret();
		}
		/// <summary>
		/// Updates visibility of cursor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_Owner_LostFocus( object sender, EventArgs e )
		{
			DestroyCaret();
		}
		#endregion
	}
}