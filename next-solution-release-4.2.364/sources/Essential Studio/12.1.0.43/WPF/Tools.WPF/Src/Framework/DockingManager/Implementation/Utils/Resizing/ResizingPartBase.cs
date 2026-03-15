#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools;
using Syncfusion.Windows.Shared;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Controls
{
	/// <summary>
	/// Class that is used as a base class for all parts of the resizing adorner.
	/// </summary>
	public abstract class ResizingPartBase
		: TemplatedAdornerBase
	{
		#region Class Private Members
		/// <summary>
		/// Specifies whether the adorner is currently moved.
		/// </summary>
		private bool m_bMoving;
		/// <summary>
		/// Specifies point of the mouse click.
		/// </summary>
		private Point m_pointClick;
		/// <summary>
		/// Specifies toolwindow the adorners are added to.
		/// </summary>
		private ToolWindow m_window;
		/// <summary>
		/// Specifies initial window bounds (before resizing).
		/// </summary>
		private Rect m_boundsInitial;
		/// <summary>
		/// Specifies window bounds that should be set on resize commit.
		/// </summary>
		private Rect m_boundsCurrent;
		/// <summary>
		/// Window that is shown while resizing.
		/// </summary>
		private ResizingWindowFrame m_frame;
		#endregion

		#region Class Dependency Properties
		/// <summary>
		/// Attached dependency property that indicates whether mouse is over the adorned element.
		/// </summary>
		public static readonly DependencyProperty IsMouseOverContentProperty =
			DependencyProperty.RegisterAttached( "IsMouseOverContent", typeof( bool ), typeof( ResizingPartBase ), new UIPropertyMetadata( false ) );
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets adorned window.
		/// </summary>
		protected ToolWindow Window
		{
			get
			{
				return m_window;
			}
		}
		/// <summary>
		/// Gets initial window bounds. (Current resizing is not taken into account)
		/// </summary>
		protected Rect InitialBounds
		{
			get
			{
				return m_boundsInitial;
			}
		}
		/// <summary>
		/// Gets current window bounds that will be set on resizing commit.
		/// </summary>
		protected Rect CurrentBounds
		{
			get
			{
				return m_boundsCurrent;
			}
		}
		#endregion

		#region Class Initialization
		/// <summary>
		/// Creates and initializaes new instance of the adorner. Content of the specified window is used as adorned element.
		/// </summary>
		/// <param name="window">Popup to adorn.</param>
		public ResizingPartBase( ToolWindow window )
			: base( window.ContentAdornable )
		{
			m_window = window;
			//InnerControl.
			BindingUtils.SetBinding( InnerControl, window.Content, IsMouseOverContentProperty, UIElement.IsMouseOverProperty );

		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Processes mouse clicks on the adorner.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown( MouseButtonEventArgs e )
		{
			base.OnMouseDown( e );

			if( e.ChangedButton == MouseButton.Left )
			{
				m_bMoving = true;
				m_pointClick = e.GetPosition( AdornedElement );
				m_boundsInitial = Window.Bounds;

                ShowFrame();

				this.CaptureMouse();
			}
		}
		/// <summary>
		/// Processes mouse movement, resized frame if adorner is currently dragged.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			if( m_bMoving )
			{
				Point point = e.GetPosition( AdornedElement );
				Vector vector = new Vector( point.X - m_pointClick.X, point.Y - m_pointClick.Y );

				Rect rect = AdornerMoved( vector );

				m_boundsCurrent = rect;
				m_frame.SetPosition( rect.Location, rect.Size );
			}

		}
		/// <summary>
		/// Processes mouse button releasing. Window resizing is committed if left mouse button has been released.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp( MouseButtonEventArgs e )
		{
			base.OnMouseUp( e );

			if( m_bMoving )
			{
				m_bMoving = false;
				this.ReleaseMouseCapture();

				if( e.ChangedButton == MouseButton.Left )
					Commit();
				else
					Cancel();
			}
		}
		/// <summary>
		/// Processe
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLostMouseCapture( System.Windows.Input.MouseEventArgs e )
		{
			base.OnLostMouseCapture( e );

			if( m_bMoving )
			{
				m_bMoving = false;
				this.ReleaseMouseCapture();
				Cancel();
			}
        }
        #endregion

		#region Class Virtuals
		/// <summary>
		/// Reacts to the adorner's movement.
		/// </summary>
		/// <param name="movementSize">Movement distance in x-y directions.</param>
		protected abstract Rect AdornerMoved( Vector movement );
		/// <summary>
		/// Commits changes that where done by the adorner's movement.
		/// </summary>
		protected virtual void Commit()
		{
			HideFrame();
			Window.Bounds = CurrentBounds;
		}
		/// <summary>
		/// Cancels changes that where done by the adorner's movement.
		/// </summary>
		protected virtual void Cancel()
		{
			HideFrame();
			Window.Bounds = InitialBounds;
		}
		#endregion

		#region Class Helper Methods
		/// <summary>
		/// Hides resizing frame.
		/// </summary>
		protected void HideFrame()
		{
			if( m_frame == null )
				return;

			m_frame.Hide();
			m_frame = null;
		}
		/// <summary>
		/// Creates and shows resizing frame.
		/// </summary>
		protected void ShowFrame()
		{
			HideFrame();

			m_frame = new ResizingWindowFrame();
			Point location = m_window.Location;
			Size size = m_window.Size;

			m_frame.Show( location, size );
		}
		#endregion

		#region Class Static Methods
		/// <summary>
		/// Gets IsMouseOverContent dependency property's value for the specified object.
		/// </summary>
		/// <param name="obj">Object to get the value for.</param>
		/// <returns>IsMouseOverContent dependency property's value.</returns>
		public static bool GetIsMouseOverContent( DependencyObject obj )
		{
			return (bool)obj.GetValue( IsMouseOverContentProperty );
		}
		/// <summary>
		/// Sets IsMouseOverContent dependency property's value for the specified object.
		/// </summary>
		/// <param name="obj">Specifies the object, the value is set for.</param>
		/// <param name="value">The value to be set.</param>
		public static void SetIsMouseOverContent( DependencyObject obj, bool value )
		{
			obj.SetValue( IsMouseOverContentProperty, value );
		}
		#endregion
	}
}
