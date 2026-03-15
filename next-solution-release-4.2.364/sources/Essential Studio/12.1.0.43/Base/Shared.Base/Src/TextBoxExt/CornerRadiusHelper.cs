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
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Utils
{
	/// <summary>
	/// Helper class for handling corner radius of control region.
	/// </summary>
	public sealed class CornerRadiusHelper:
		IDisposable
	{
		#region Fields

		private int _cornerRadius = 0;
		private Control _owner;

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="CornerRadiusHelper"/> class.
		/// </summary>
		/// <param name="owner">The owner control.</param>
		public CornerRadiusHelper( Control owner )
		{
			_owner = owner;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets corner radius.
		/// </summary>
		/// <remarks>Radius has to be not less than zero or half of minimum dimension (width or height) of control.
		/// If radius is zero, region has rectangular appearance.
		/// </remarks>
		public int CornerRadius
		{
			get
			{
				return _cornerRadius;
			}
			set
			{
				if( value != _cornerRadius )
				{
					if( _owner != null )
					{
						int minRadius;

						if( IsCornerRadiusValid( value, out minRadius ) )
						{
							_cornerRadius = value;
							_owner.MinimumSize = GetMinimalSize( _owner.MinimumSize );

							UpdateRegion();
						}
						else
						{
							throw new ArgumentException(
								string.Format( "Invalid corner raduis value - radius can't be less than zero and greater than {0}.", minRadius ) );
						}
					}
					else
					{
						_cornerRadius = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets the graphics path for region border.
		/// </summary>
		public GraphicsPath GetPath( RectangleF bounds )
		{
			int r = this.CornerRadius;
			GraphicsPath path = null;

			if( r > 0 )
			{
				float d = 2*r;
				float w = bounds.Width;
				float h = bounds.Height;

				RectangleF corner = new RectangleF( 0, 0, d, d );

				path = new GraphicsPath();

				// Top left
				path.AddArc( corner, 180, 90 );
				// Top
				path.AddLine( r, 0, w-r, 0 );
				// Top right
				corner.Offset( w - corner.Width, 0 );
				path.AddArc( corner, 270, 90 );
				// Right
				path.AddLine( w, r, w, h-r );
				// Bottom right						
				corner.Offset( 0, h - corner.Height );
				path.AddArc( corner, 0, 90 );
				// Bottom
				path.AddLine( w-r+1, h, r-1, h );
				// Bottom left
				corner.Offset( corner.Width - w, 0 );
				path.AddArc( corner, 90, 90 );
				// Left
				path.AddLine( 0, h-r, 0, r );

				Matrix m = new Matrix();

				m.Translate( bounds.X, bounds.Y );
				path.Transform( m );
			}

			return path;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Updates and sets control's region.
		/// </summary>
		public void UpdateRegion()
		{
			if( _owner != null )
			{
				_owner.Region = GetRegion();

				if( _owner.IsHandleCreated )
				{
					NativeMethodsHelper.RedrawWindow( _owner.Handle,
						NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_UPDATENOW );
				}
			}
		}

		/// <summary>
		/// Retrieves the minimal size of the owner control.
		/// </summary>
		/// <param name="currentMinimalSize">Current minimal size.</param>
		public Size GetMinimalSize( Size currentMinimalSize )
		{
			int minWidth = currentMinimalSize.Width;
			int minHeight = currentMinimalSize.Height;

			if( this.CornerRadius > 0 )
			{
				minWidth = Math.Max( 2*( this.CornerRadius + 2 ), minWidth );
				minHeight = Math.Max( 2*this.CornerRadius, minHeight );
			}

			return new Size( minWidth, minHeight );
		}

		#endregion

		#region Implementation

		private bool IsCornerRadiusValid( int radius, out int minRadius )
		{
			int minDim = Math.Min( _owner.Width, _owner.Height );
			minRadius = minDim / 2;

			return ( radius >= 0 ) && ( 2*radius <= minDim );
		}

		private Region GetRegion()
		{
			int r = this.CornerRadius;
			int w = _owner.Width;
			int h = _owner.Height;
			Region region = null;// new Region( GetPath( new Rectangle( Point.Empty, _owner.Size ) ) );

			if( r > 0 )
			{
				//IntPtr hRoundRgn = CreateRoundedRegion( r, w+1, h+1 );
				//region = Region.FromHrgn( hRoundRgn );
				//NativeMethods.DeleteObject( hRoundRgn );

				using( GraphicsPath	p = GetPath( new Rectangle( 0, 0, _owner.Size.Width, _owner.Size.Height ) ) )
				{
					region = new Region( p );
					p.Reverse();

					using( Region rgnReverse = new Region(p) )
					{
						region.Union( rgnReverse );
					}
				}
			}
			else
			{
				region = new Region( new Rectangle( 0, 0, _owner.Width, _owner.Height ) );
			}

			return region;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			_owner = null;
		}

		#endregion
	}
}
