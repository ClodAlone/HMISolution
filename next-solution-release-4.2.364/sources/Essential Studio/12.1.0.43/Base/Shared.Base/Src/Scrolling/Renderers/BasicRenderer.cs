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

#region file using directives
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Renderers
{
	/// <summary></summary>
	public class BasicRenderer
		: IRenderer
	{
		#region class members
		/// <summary></summary>
		protected ScrollBarCustomDraw m_parent;
		/// <summary>
		/// Indicates whether scroll is vertical or horizontal
		/// </summary>
		protected bool m_isVerticalScroll = false;
		#endregion

		#region class properties
		/// <summary></summary>
		protected ScrollBarCustomDraw Parent
		{
			get
			{
				return m_parent;
			}
		}
		#endregion

		#region class initialize\finalize methods
		/// <summary></summary>
		/// <param name="isVerticalScrollBar"/>
		protected internal BasicRenderer( bool isVerticalScrollBar )
		{
			m_isVerticalScroll = isVerticalScrollBar;
		}

		/// <summary></summary>
		/// <param name="parent"/>
		public BasicRenderer( ScrollBarCustomDraw parent )
		{
			m_parent = parent;

			if( parent is HScrollBarCustomDraw )
			{
				IsVerticalScrollBar = false;
			}
			else if( parent is VScrollBarCustomDraw )
			{
				IsVerticalScrollBar = true;
			}
		}
		#endregion

		#region class implements IRenderer
		/// <summary>
		/// Indicates whether scrollBar is vertical or horizontal.
		/// </summary>
		public virtual bool IsVerticalScrollBar
		{
			get
			{
				return m_isVerticalScroll;
			}
			set
			{
				if( m_isVerticalScroll != value )
				{
					m_isVerticalScroll = value;
				}
			}
		}

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="bounds"></param>
		/// <param name="state"/>
		public virtual void DrawBackground( Graphics g, Rectangle bounds, ButtonState state )
		{
			if( null == g )
				throw new ArgumentNullException( "g" );

			if( bounds.Width > 0 && bounds.Height > 0 )
			{
				Color backColor = SystemColors.ControlLight;

				using( Brush bgBrush = new SolidBrush( backColor ) )
				{
					g.FillRectangle( bgBrush, bounds );
				}

				if( state == ButtonState.Pushed )
				{
					Color clrHighlight = SystemColors.Highlight;
					Color clrSelect = Color.FromArgb( 200, clrHighlight );

					using( Brush bgBrush = new SolidBrush( clrSelect ) )
					{
						g.FillRectangle( bgBrush, bounds );
					}
				}
			}
		}

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="bounds"></param>
		/// <param name="type"></param>
		/// <param name="state"></param>
		public virtual void DrawArrowButton( Graphics g, Rectangle bounds, ScrollButton type, ButtonState state )
		{
			if( null == g )
				throw new ArgumentNullException( "g" );

			if( bounds.Height > 0 && bounds.Width > 0 )
			{
				ControlPaint.DrawScrollButton( g, bounds, type, state );
			}
		}

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="bounds"></param>
		/// <param name="state"></param>
		public virtual void DrawThumb( Graphics g, Rectangle bounds, ButtonState state )
		{
			if( null == g )
				throw new ArgumentNullException( "g" );

			if( bounds.Width > 0 && bounds.Height > 0 )
			{
				if( state == ButtonState.Inactive )
				{
					DrawBackground( g, bounds, state );
				}
				else
				{
					ControlPaint.DrawButton( g, bounds, state );
				}
			}
		}
		#endregion

		#region Nested classes
		/// <summary></summary>
		protected class Bitmaps : Hashtable
		{
			/// <summary></summary>
			/// <param name="capacity"/>
			public Bitmaps( int capacity )
				: base( capacity )
			{
			}

			/// <summary></summary>
			public override object this[ object key ]
			{
				get
				{
					return base[ key ];
				}
				set
				{
					IDisposable iDispose = base[ key ] as IDisposable;

					if( iDispose != null )
					{
						iDispose.Dispose();
					}

					base[ key ] = value;
				}
			}
		}
		#endregion
	}
}