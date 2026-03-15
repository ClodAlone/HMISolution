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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.ComponentModel;
using Syncfusion.Windows.Forms.Design;

using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// An interface for hosting <see cref="InternalTab"/> objects and
	/// receiving clicks from these buttons.
	/// </summary>
	public interface IInternalTabParent
	{
		/// <summary>
		/// Returns Graphics object, font and delta between tabs.
		/// </summary>
		/// <param name="g">Graphics object.</param>
		/// <param name="font">Font.</param>
		/// <param name="delta">Delta between tabs.</param>
		void GetMeasureTools( out Graphics g, out Font font, out int delta );

		/// <summary>
		/// Returns <see cref="InternalTab"/>, brush, text color, font and delta between tabs.
		/// </summary>
		/// <param name="tab">Tab object.</param>
		/// <param name="fillBrush">Brush for drawing the tab background.</param>
		/// <param name="textColor">Text color.</param>
		/// <param name="font">Font used to draw text.</param>
		/// <param name="delta">Delta between tabs.</param>
		void GetDrawingTools( InternalTab tab, out Brush fillBrush, out Color textColor, out Font font, out int delta );

		/// <summary>
		/// Disposes any temporary drawing object.
		/// </summary>
		void DisposeMeasureTools();

		/// <summary>
		/// Returns the image list that these tabs get images from.
		/// </summary>
		ImageList ImageList { get; }

		/// <summary>
		/// Indicates the visual style of the tabBar.
		/// </summary>
		TabBarSplitterStyle Style { get; }
	}

	/// <summary>
	/// InternalTab draws tabs inside a <see cref="TabBar"/> in a <see cref="TabBarSplitterControl"/>.
	/// </summary>
	[
	ToolboxItem( false ),
	]
	public class InternalTab: InternalButton
	{
		private string label;
		int imageIndex = -1;
		/// <summary>
		/// Renderer used to draw the tab.
		/// </summary>
		internal TabsRendererBase m_renderer = null;

		/// <overload>
		/// Initializes a new <see cref="InternalTab"/>.
		/// </overload>
		/// <summary>
		/// Initializes a new <see cref="InternalTab"/>.
		/// </summary>
		public InternalTab()
		{
		}
        /// <summary>
        /// Gets or sets the visibility of the control.
        /// </summary>
        private bool visible = true;
        /// <summary>
        /// Gets or sets the visibility of the control.
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

		/// <summary>
		/// Initializes a new <see cref="InternalTab"/> with a label.
		/// </summary>
		/// <param name="label">The label to display in the tab.</param>
		public InternalTab( string label )
			: this( null, label )
		{
			this.label = label;
		}

		/// <summary>
		/// Initializes a new <see cref="InternalTab"/> with a cookie.
		/// </summary>
		/// <param name="cookie">The cookie associated with this tab.</param>
		public InternalTab( object cookie )
			: this( null, cookie )
		{
		}

		/// <summary>
		/// Initializes a new <see cref="InternalTab"/> with a cookie and a label.
		/// </summary>
		/// <param name="cookie">The cookie associated with this tab.</param>
		/// <param name="owner">The owner of this tab.</param>
		public InternalTab( object owner, object cookie )
			: base( owner, cookie )
		{
			if( cookie != null )
			{
				Control control = cookie as Control;
				if( control != null )
					this.label = control.Text;
				else
					this.label = cookie.ToString();
			}

			if( owner != null ) //&& registerWithOwner
				OnOwnerChanged();

			m_renderer = new Office2007TabsRenderer( this );
			if ((this.Owner as TabBarSplitterControl ).Style == TabBarSplitterStyle.Metro)
				m_renderer = new MetroTabsRenderer(this);
			else
				m_renderer = new Office2007TabsRenderer(this);
		}

		/// <override/>
		public override string ToString()
		{
			if( this.label.Length > 0 )
				return this.label;
			return this.cookie.ToString();
		}

		/// <override/>
		public override void OnOwnerChanged()
		{
			// TODO: Handling page registration in parent form
			//TabScrollBar parent = this.Owner as TabScrollBar;
			//Control control = Cookie as Control;
			//if (parent != null && control != null)
			//    parent.RegisterTab(control);
		}

		/// <summary>
		/// Gets / sets the label to display in the tab.
		/// </summary>
		public string Label
		{
			get
			{
				return this.label;
			}
			set
			{
				if( this.label != value )
				{
					this.label = value;
					this.Dirty = true;

					Control control = this.Owner as Control;
					if( control != null )
						control.PerformLayout();
				}
			}
		}

		/// <summary>
		/// Gets / sets the index of the image to display in this tab.
		/// </summary>
		[
		DefaultValue( -1 ),
		Category( "Appearance" ),
		TypeConverter( typeof( System.Windows.Forms.ImageIndexConverter ) ),
		Editor( typeof( ImageIndexEditor ), typeof( UITypeEditor ) )
		]
		public int ImageIndex
		{
			get
			{
				return imageIndex;
			}
			set
			{

				if( value < -1 )
					throw new ArgumentException( SR.GetString( "InvalidLowBoundArgumentEx", "imageIndex", value, -1 ) );
				imageIndex = value;
			}
		}

		/// <summary>
		/// Gets the region that contains the tab bounds.
		/// </summary>
		public Region GetTabRegion
		{
			get
			{
				if( m_renderer != null )
				{
					return m_renderer.GetTabRegion;
				}
				else
				{
					return new Region( this.Bounds );
				}
			}
		}

		/// <summary>
		/// Gets the renderer that renders the tab.
		/// </summary>
		internal TabsRendererBase Renderer
		{
			get
			{
				return m_renderer;
			}
		}

		/// <override/>
		public override Size GetPreferredSize( Size maxSize )
		{
			return Size;
		}

		/// <overload>
		/// Recalculates the best size for the button and resizes it.
		/// </overload>
		/// <summary>
		/// Recalculates the best size for the button and resizes it.
		/// </summary>
		/// <param name="g">Graphics object.</param>
		/// <param name="font">Font.</param>
		/// <param name="delta">Delta between tabs.</param>
		public void AdjustSize( Graphics g, Font font, int delta )
		{
			Debug.Assert( g != null && font != null );

			IInternalTabParent tabContainer = this.Owner as IInternalTabParent;
			if( tabContainer != null )
			{
				if( tabContainer.Style == TabBarSplitterStyle.Default )
				{
					Size textSize;
					//FR #1081
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					textSize = g.MeasureString(this.Label, font).ToSize();
#else
					if( this.Pushed )
						textSize = TextRenderer.MeasureText( this.Label, font );
					else
						textSize = TextRenderer.MeasureText( this.Label, new Font( font, FontStyle.Regular ) );
#endif

					IInternalTabParent container = this.Owner as IInternalTabParent;
					if( container.ImageList != null && imageIndex >= 0 && imageIndex < container.ImageList.Images.Count )
					{
						int imageWidth = container.ImageList.ImageSize.Width + 5;
						textSize.Width += imageWidth;
						textSize.Height = Math.Max( container.ImageList.ImageSize.Height, textSize.Height );
					}
					this.Size = new Size( textSize.Width+delta*2, textSize.Height+2 );
				}
				else
				{
					if( this.m_renderer != null )
					{
						this.Size = m_renderer.GetItemPreferredSize();
					}
				}
			}
		}

		/// <override/>
		public override void AdjustSize()
		{
			IInternalTabParent tabContainer = this.Owner as IInternalTabParent;
			if( tabContainer != null )
			{
				Graphics g;
				int delta;
				Font font;

				tabContainer.GetMeasureTools( out g, out font, out delta );

				try
				{
					AdjustSize( g, font, delta );
				}
				finally
				{
					tabContainer.DisposeMeasureTools();
				}
			}
		}

		/// <summary>
		/// Creates and initializes a bitmap for this tab. 
		/// </summary>
		/// <param name="size">The size of the bitmap.</param>
		/// <param name="flatLook">Indicates the flat look status.</param>
		/// <returns>The bitmap where the button is drawn into.</returns>
		/// <remarks>
		/// When you drag a tab, this function is called to create the dragging button image.
		/// </remarks>
		public virtual Bitmap CreateBitmap( System.Drawing.Size size, bool flatLook )
		{
			Bitmap bm = null;
			Graphics g = null;

			IInternalTabParent container = this.Owner as IInternalTabParent;
			if( container != null )
			{
				try
				{
					bm = new Bitmap( size.Width, size.Height );
					g = Graphics.FromImage( bm );

					if( this.Style == TabBarSplitterStyle.Default )
					{
						Brush fillBrush;
						Color textColor;
						Font  font;
						int delta;

						container.GetDrawingTools( this, out fillBrush, out textColor, out font, out delta );

						// Draw the tab.
						Rectangle bounds = new Rectangle( 0, 0, size.Width, size.Height );
						TabPaint.DrawTab( g, bounds, container.ImageList, this.ImageIndex, this.Label, fillBrush, textColor, font, this.Enabled, delta, true );
					}
					else
					{
						m_renderer.Bounds = new Rectangle( 0, 0, size.Width, size.Height );
						m_renderer.DrawTab( g );
					}
				}
				finally
				{
					container.DisposeMeasureTools();

					if( g != null )
						g.Dispose();
				}
			}

			return bm;
		}

		/// <override/>
		public override void Paint( System.Drawing.Graphics g, System.Drawing.Rectangle bounds, bool flatLook, System.Drawing.Rectangle barArea )
		{
			Debug.Assert( g != null, "Must pass valid graphics." );

			try
			{
				InitToolTip( Rectangle.Intersect( bounds, barArea ) );

				if( g.ClipBounds.IntersectsWith( bounds ) )
				{
					IInternalTabParent container = this.Owner as IInternalTabParent;
					if( container != null )
					{
						if( this.Style == TabBarSplitterStyle.Default )
						{
							Brush fillBrush;
							Color textColor;
							Font  font;
							int delta;

							container.GetDrawingTools( this, out fillBrush, out textColor, out font, out delta );

							// Draw the tab.
							TabPaint.DrawTab( g, bounds, container.ImageList, imageIndex, this.Label, fillBrush, textColor, font, this.Enabled, delta, this.Checked || bounds.Left == 0 );
						}
						else
						{
							m_renderer.Bounds = bounds;
							m_renderer.DrawTab( g );
						}
					}
				}
			}
			finally
			{
				this.Dirty = false;
			}
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( m_renderer != null )
				{
					m_renderer.Dispose();
					m_renderer = null;
				}
			}

			base.Dispose( disposing );
		}
	}
}
