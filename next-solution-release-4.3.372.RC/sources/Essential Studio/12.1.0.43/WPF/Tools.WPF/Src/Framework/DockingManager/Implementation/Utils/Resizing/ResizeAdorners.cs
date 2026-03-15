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
using System.Diagnostics;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
	public class LeftTopResizeAdorner
		: ResizingPartBase
	{
		public LeftTopResizeAdorner( ToolWindow window )
			: base( window )
		{
			this.Cursor = Cursors.SizeNWSE;
		}

		protected override Rect AdornerMoved( Vector movement )
		{
			Rect bounds = Window.Bounds;

			if( movement.X >= bounds.Width )
				movement.X = bounds.Width - 1;

			if( movement.Y >= bounds.Height )
				movement.Y = bounds.Height - 1;

			bounds.X += movement.X;
			bounds.Width -= movement.X;
			bounds.Y += movement.Y;
			bounds.Height -= movement.Y;

			//Debug.WriteLine( movement );

			return bounds;
		}

		/// <summary>
		/// Arranges inner control to the full size.
		/// </summary>
		/// <param name="finalSize"></param>
		/// <returns></returns>
		protected override Size ArrangeOverride( Size finalSize )
		{
			InnerControl.Arrange( new Rect( 0, 0, 20, 20 ) );
			return finalSize;
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="constraint"></param>
		/// <returns></returns>
		protected override Size MeasureOverride( Size constraint )
		{
			base.MeasureOverride( new Size( 20, 20 ) );

			return constraint;
		}

	}
	public class RightTopResizeAdorner
		: ResizingPartBase
	{
		public RightTopResizeAdorner( ToolWindow window )
			: base( window )
		{
			this.Cursor = Cursors.SizeNESW;
		}

		protected override Rect AdornerMoved( Vector movement )
		{
			Rect bounds = Window.Bounds;

			if( bounds.Width + movement.X <= 0  )
				movement.X = -bounds.Width + 1;

			if( movement.Y >= bounds.Height )
				movement.Y = bounds.Height - 1;

			bounds.Y += movement.Y;
			bounds.Width += movement.X;
			bounds.Height -= movement.Y;

			//Debug.WriteLine( movement );

			return bounds;
		}

		/// <summary>
		/// Arranges inner control to the full size.
		/// </summary>
		/// <param name="finalSize"></param>
		/// <returns></returns>
		protected override Size ArrangeOverride( Size finalSize )
		{
			InnerControl.Arrange( new Rect( finalSize.Width - 20, 0, 20, 20 ) );
			return finalSize;
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="constraint"></param>
		/// <returns></returns>
		protected override Size MeasureOverride( Size constraint )
		{
			base.MeasureOverride( new Size( 20, 20 ) );

			return constraint;
		}

	}
	public class RightBottomResizeAdorner
		: ResizingPartBase
	{
		public RightBottomResizeAdorner( ToolWindow window )
			: base( window )
		{
			this.Cursor = Cursors.SizeNWSE;
		}

		protected override Rect AdornerMoved( Vector movement )
		{
			Rect bounds = Window.Bounds;

			if( bounds.Width + movement.X <= 0 )
				movement.X = -bounds.Width + 1;

			if( bounds.Height + movement.Y <= 0 )
				movement.Y = -bounds.Height + 1;

			bounds.Width += movement.X;
			bounds.Height += movement.Y;

			//Debug.WriteLine( movement );

			return bounds;
		}

		/// <summary>
		/// Arranges inner control to the full size.
		/// </summary>
		/// <param name="finalSize"></param>
		/// <returns></returns>
		protected override Size ArrangeOverride( Size finalSize )
		{
			InnerControl.Arrange( new Rect( finalSize.Width - 20, finalSize.Height - 20, 20, 20 ) );
			return finalSize;
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="constraint"></param>
		/// <returns></returns>
		protected override Size MeasureOverride( Size constraint )
		{
			base.MeasureOverride( new Size( 20, 20 ) );

			return constraint;
		}

	}
	public class LeftBottomResizeAdorner
		: ResizingPartBase
	{
		public LeftBottomResizeAdorner( ToolWindow window )
			: base( window )
		{
			this.Cursor = Cursors.SizeNESW;
		}

		protected override Rect AdornerMoved( Vector movement )
		{
			Rect bounds = Window.Bounds;

			if( bounds.Height + movement.Y <= 0 )
				movement.Y = -bounds.Height + 1;

			if( movement.X >= bounds.Width )
				movement.X = bounds.Width - 1;

			bounds.X += movement.X;
			bounds.Width -= movement.X;
			bounds.Height += movement.Y;

			//Debug.WriteLine( movement );

			return bounds;
		}

		/// <summary>
		/// Arranges inner control to the full size.
		/// </summary>
		/// <param name="finalSize"></param>
		/// <returns></returns>
		protected override Size ArrangeOverride( Size finalSize )
		{
			InnerControl.Arrange( new Rect( 0, finalSize.Height - 20, 20, 20 ) );
			return finalSize;
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="constraint"></param>
		/// <returns></returns>
		protected override Size MeasureOverride( Size constraint )
		{
			base.MeasureOverride( new Size( 20, 20 ) );

			return constraint;
		}

	}
}
