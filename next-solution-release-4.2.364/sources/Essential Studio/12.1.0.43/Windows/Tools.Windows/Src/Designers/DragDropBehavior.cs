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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

#region file using directives
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;
#endregion

namespace Syncfusion.Windows.Forms.Tools.Design
{
	/// <summary>
	/// This class provides extended design-time behavior for mouse processing
	/// and allows user to handle it at design time in VS2005. 
	/// </summary>
	internal class DragDropBehavior: Behavior
	{
		#region Class Events
		public event DragEventHandler DragDrop;
		public event DragEventHandler DragOver;
		public event EventHandler DragLeave;
		public event EventHandler MouseUp;
		public event EventHandler MouseDown;
		public event EventHandler MouseMove;
		#endregion

		#region Class overrides
		public override bool OnMouseDown( Glyph g, MouseButtons button, Point mouseLoc )
		{
			if( MouseDown != null )
			{
				MouseDown( this, EventArgs.Empty );
			}

			return base.OnMouseDown( g, button, mouseLoc );
		}

		public override bool OnMouseUp( Glyph g, MouseButtons button )
		{
			if( MouseUp != null )
			{
				MouseUp( this, EventArgs.Empty );
			}

			return base.OnMouseUp( g, button );
		}

		public override bool OnMouseMove( Glyph g, MouseButtons button, Point mouseLoc )
		{
			if( MouseMove != null )
			{
				MouseMove( this, EventArgs.Empty );
			}

			return base.OnMouseMove( g, button, mouseLoc );
		}

		public override void OnDragDrop( Glyph g, DragEventArgs e  )
		{
			base.OnDragDrop( g, e );

			if( DragDrop != null )
			{
				DragDrop( this, e );
			}
		}

		public override void OnDragOver( Glyph g, DragEventArgs e )
		{
			base.OnDragOver( g, e );

			if( DragOver != null )
			{
				DragOver( this, e );
			}            
		}

		public override void OnDragLeave( Glyph g, EventArgs e )
		{
			base.OnDragLeave( g, e );

			if( DragLeave != null )
			{
				DragLeave( this, e );
			}            
		}

		#endregion


	}

	internal class DragDropGlyph: Glyph
	{        
		public DragDropGlyph( DragDropBehavior b ):base( ( Behavior) b )
		{

		}

		public override Cursor GetHitTest( Point p )
		{
			return Cursors.Default;
		}

		public override void Paint( PaintEventArgs pe )
		{
            // Do nothing here
		}
	}	
}

#endif