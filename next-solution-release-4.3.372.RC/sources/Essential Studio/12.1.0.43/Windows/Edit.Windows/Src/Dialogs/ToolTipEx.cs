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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Edit.Forms.Popup
{
	/// <summary>
	/// Extended Tool Tip.
	/// </summary>
	public class ToolTipEx
		: Syncfusion.Windows.Forms.Edit.Forms.Popup.BasePopupForm
		, IDisposable
	{
		#region Constants
		/// <summary>
		/// Offset of the text from the borders.
		/// </summary>
		private const int DEF_TEXT_OFFSET = 3;
		/// <summary>
		/// Size of square clip for image.
		/// </summary>
		const int DEF_IMAGE_SIZE = 32;
		#endregion

		#region Fields
		/// <summary>
		/// Text of the tooltip.
		/// </summary>
		private string m_strText;
		/// <summary>
		/// Parent of the control.
		/// </summary>
		private Control m_parent;
		/// <summary>
		/// Timer, that has to wait for specified delay time.
		/// </summary>
		private Timer m_timer;
		/// <summary>
		/// Area, that contains object to be hinted with the current text.
		/// </summary>
		private Rectangle m_rectHintedRectangle = Rectangle.Empty;
		/// <summary>
		/// Temporary graphics object.
		/// </summary>
		private Graphics m_graphicsTemp;
		/// <summary>
		/// Mouse position saved and the last mouse move processing.
		/// </summary>
		private Point m_mouseLastPosition;
		/// <summary>
		/// Indicates whether XP style should be used.
		/// </summary>
		private bool m_bUseXPStyle = true;
        /// <summary>
        /// Indicates whether XP style Broder should be used.
        /// </summary>
        private bool m_bUseXPStyleBorder = true;
		/// <summary>
		/// Image associated with tooltip.
		/// </summary>
		private Image m_image;
		/// <summary>
		/// Indicates whether tooltip should be shown when mouse move is paused over parent's window.
		/// </summary>
		private bool m_bShowOnParentMouseStop = true;
        /// <summary>
        /// Stores MouseDown Hit Location to update tooltip
        /// </summary>
        private Point mousepoint = new Point();

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets delay in miliseconds before show.
		/// </summary>
		public int ShowDelay
		{
			get
			{
				return m_timer.Interval;
			}
			set
			{
				m_timer.Interval = value;
			}
		}
		/// <summary>
		/// Indicates whether XP style should be used.
		/// </summary>
		public bool UseXPStyle
		{
			get
			{
				return m_bUseXPStyle;
			}
			set
			{
				m_bUseXPStyle = value;
			}
		}
        /// <summary>
        /// Indicates whether XP style 3D border should be used.
        /// </summary>
        public bool UseXPStyleBorder
        {
            get
            {
                return m_bUseXPStyleBorder;
            }
            set
            {
                m_bUseXPStyleBorder = value;
            }
        }
		/// <summary>
		/// Gets or sets text of tooltip.
		/// </summary>
		public string ToolTipText
		{
			get
			{
				return m_strText;
			}
			set
			{
				m_strText = value;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Event, that is raised when text should be updated.
		/// </summary>
		public event UpdateTooltipEventHandler UpdateTooltip;
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Hides default constructor.
		/// </summary>
		private ToolTipEx()
			: base( null )
		{ }
		/// <summary>
		/// Creates new tootlip control and sets his parent.
		/// </summary>
		/// <param name="parent">Parent control of the tooltip.</param>
		public ToolTipEx( Control parent )
			: this( parent, true )
		{
		}
		/// <summary>
		/// Creates and initializes new ToolTipEx.
		/// </summary>
		/// <param name="parent">Parent of tooltip.</param>
		/// <param name="bShowOnMouseStop">Indicates whether tooltip should be shown when mouse move is paused over parent's window.</param>
		public ToolTipEx( Control parent, bool bShowOnMouseStop )
			: base( parent, true, 0.3f, true, false, true )
		{
			if( parent == null ) throw new ArgumentNullException( "parent" );

			m_parent = parent;

			ControlStyles styleTrue = ControlStyles.Selectable |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.UserMouse |
				ControlStyles.UserPaint;

			ControlStyles styleFalse = ControlStyles.CacheText | ControlStyles.Selectable;

			SetStyle( styleTrue, true );
			SetStyle( styleFalse, false );

			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.Manual;

			m_timer = new Timer();
			m_timer.Interval = 1000;
			m_timer.Stop();
			m_timer.Tick += new EventHandler( OnTimerTick );

			m_parent.MouseMove += new MouseEventHandler( OnParentMouseMove );
			m_parent.MouseDown += new MouseEventHandler( OnParentMouseDown );
			m_graphicsTemp = Graphics.FromImage( new Bitmap( 1, 1 ) );

			this.FormBorderStyle = FormBorderStyle.None;

			m_bShowOnParentMouseStop = bShowOnMouseStop;
		}
		/// <summary>
		/// Disposes all used resources.
		/// </summary>
		public new void Dispose()
		{
			if( m_timer != null )
			{
				m_timer.Stop();
				m_timer.Dispose();
				m_timer = null;
			}

			if( m_graphicsTemp != null )
			{
				m_graphicsTemp.Dispose();
				m_graphicsTemp = null;
			}

			m_parent.MouseMove -= new MouseEventHandler( OnParentMouseMove );
			m_parent.MouseDown -= new MouseEventHandler( OnParentMouseDown );

			m_parent = null;

			base.Dispose();
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Paints background of the tooltip.
		/// </summary>
		/// <param name="pevent">PaintEventArgs.</param>
		protected override void OnPaintBackground( PaintEventArgs pevent )
		{
			base.OnPaintBackground( pevent );

			Rectangle rect = ClientRectangle;
			rect.Height--;
			rect.Width--;

			BrushPaint.FillRectangle( pevent.Graphics, rect, m_backgroundBrush );
		}
		/// <summary>
		/// Paints tooltip itself.
		/// </summary>
		/// <param name="e">PaintEventArgs.</param>
		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			if (UseXPStyle)
			{
				if(m_bUseXPStyleBorder)
					GraphicsUtils.Draw3DBorder( e.Graphics, new Rectangle( 0, 0, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1 ) );
			}
			else
			{
				e.Graphics.DrawRectangle( m_borderPen, new Rectangle( 0, 0, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1 ) );
			}

			int imageOffset = ( null != m_image ) ? ( DEF_IMAGE_SIZE + DEF_TEXT_OFFSET ) : ( 0 );

			if( null != m_image )
			{
				e.Graphics.DrawImage( m_image, DEF_TEXT_OFFSET, DEF_TEXT_OFFSET, DEF_IMAGE_SIZE, DEF_IMAGE_SIZE );
			}

			StringFormat format = new StringFormat();
			int xOffset = DEF_TEXT_OFFSET + imageOffset;

			if (m_parent.RightToLeft == RightToLeft.Yes)
			{
				format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				xOffset = this.ClientRectangle.Right - xOffset;
			}

			e.Graphics.DrawString( m_strText, Font, new SolidBrush( SystemColors.InfoText ),xOffset, DEF_TEXT_OFFSET ,format);
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Shows tooltip.
		/// </summary>
		public void ShowToolTip()
		{
            this.Owner = null;
            //** To fix incident Id = 64202 ***/
            this.Owner = m_parent.TopLevelControl as Form;
            if (this.Owner == null)
            {
                return;
            }

            UpdateTooltipSize();
            UpdateToolTipLocation();
            if (!this.IsDisposed)
            {
                Show();
            }
            
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Updates text of the tooltip, shows it and stops timer.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTimerTick( object sender, EventArgs e )
		{
			if( m_bShowOnParentMouseStop )
			{
				if( m_parent != null && !m_parent.IsDisposed )
				{
					Point mousePosition = MousePosition;
					Point mousePositionClient = m_parent.PointToClient( mousePosition );

					if( mousePosition == m_mouseLastPosition )
					{
						m_timer.Stop();

						if( UpdateTooltip != null )
						{
							UpdateTooltipEventArgs args = new UpdateTooltipEventArgs();
							args.X = mousePositionClient.X;
							args.Y = mousePositionClient.Y;
							UpdateTooltip( this, args );

							m_strText = args.Text;
							m_image = args.Image;
							m_rectHintedRectangle = args.HintedArea;

							if( m_rectHintedRectangle == Rectangle.Empty )
							{
								m_rectHintedRectangle = new Rectangle( mousePositionClient.X, mousePositionClient.Y, 1, 1 );
							}
						}

						if( m_strText != string.Empty )
						{
							Point pointLocation = mousePosition;
							pointLocation.X += 2;
							pointLocation.Y += 2;
							this.Location = pointLocation;
							ShowToolTip();
							m_parent.Focus();
						}
					}
				}
			}
		}
		/// <summary>
		/// Processes mouse movement.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnParentMouseMove( object sender, MouseEventArgs e )
		{
			m_mouseLastPosition = Control.MousePosition;

            if (!Visible)
            {
                ResetTimer();
            }
            else
            {
                if (!m_rectHintedRectangle.Contains(e.X, e.Y))
                  {
                     ResetTimer();
                     ResetText();
                  }
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mousepoint.X = e.X;
                mousepoint.Y = e.Y;
            }

            if (mousepoint.Y != e.Y)
            {
                m_bShowOnParentMouseStop = true;
            }
		}
		/// <summary>
		/// Hides tooltip when mouse button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnParentMouseDown( object sender, MouseEventArgs e )
		{
			m_timer.Stop();
            Hide();
		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// Hides tooltip and resets timer.
		/// </summary>
		private void ResetTimer()
		{
			Hide();

			if( !m_timer.Enabled && ( Control.MouseButtons == MouseButtons.None ) )
			{
				m_timer.Start();
			}
		}
		/// <summary>
		/// Resets text and hinted area.
		/// </summary>
		private new void ResetText()
		{
			m_strText = string.Empty;
			m_rectHintedRectangle = Rectangle.Empty;
		}
		/// <summary>
		/// Updates size of the toolbar.
		/// </summary>
		private void UpdateTooltipSize()
		{
			SizeF size = m_graphicsTemp.MeasureString( m_strText, Font );

			if( null != m_image )
			{
				size.Width += DEF_TEXT_OFFSET * 2 + DEF_IMAGE_SIZE;
				size.Height = Math.Max( size.Height, DEF_IMAGE_SIZE );
			}

			this.Size = new Size( ( int )Math.Ceiling( size.Width ) + DEF_TEXT_OFFSET * 2, ( int )Math.Ceiling( size.Height ) + DEF_TEXT_OFFSET * 2 );
		}
		/// <summary>
		/// Updates the tooltip location for RTL.
		/// </summary>
		private void UpdateToolTipLocation()
		{
			if (m_parent.RightToLeft == RightToLeft.Yes)
			{
				this.Location = new Point(this.Location.X - this.ClientRectangle.Width, this.Location.Y + Cursor.Size.Height / 2);
			}
		}
		#endregion
	}
}
