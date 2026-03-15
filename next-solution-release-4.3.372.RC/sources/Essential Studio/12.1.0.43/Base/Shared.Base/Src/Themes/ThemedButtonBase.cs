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
using System.Data;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Design;
using System.ComponentModel.Design;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Draws a themed button.
	/// </summary>
	public class ThemedButtonBase : ThemedControl
	{
		#region Constants

		/// <summary>
		/// Value for reduce ClientRectangle.
		/// </summary>
		private const int DEF_REDUCE_SIZE_IMAGE = 2;

		/// <summary>
		/// Value for shift image rectangle.
		/// </summary>
		private const int DEF_SHIFT_LOCATION_IMAGE = 1;

		#endregion

		#region Fields
		private ButtonState buttonState = ButtonState.Flat;
		private ButtonState defaultButtonState = ButtonState.Flat;
		private CheckState checkState = CheckState.Unchecked;
		protected bool mouseOver = false;
		protected ControlDrawing cd = new ControlDrawing();
		private ContentAlignment textAlign = ContentAlignment.MiddleCenter;
		private bool drawText = false;
		private Syncfusion.Windows.Forms.VisualStyle style = Syncfusion.Windows.Forms.VisualStyle.Default;
        private Office2007Theme m_office2007Theme = Office2007Theme.Blue;
        protected Office2007Colors office2007ColorTable = null;
        private Office2010Theme m_office2010Theme = Office2010Theme.Blue;
        protected Office2010Colors office2010ColorTable = null;
        private Color flatColor = SystemColors.ControlDark;
		/// <summary>
		/// Image for draw foreground.
		/// </summary>
		private Image m_image = null;
		/// <summary>
		/// Indicate wether the image for button 
		/// is stretched or shrunk to fit the size of the button.
		/// </summary>
		private bool m_stretchImage = false;
		/// <summary>
		/// Indicates whether mouse positionin is over control.
		/// </summary>
		private bool m_bIsMouseOver = false;
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets image for draw foreground.
		/// </summary>
		public Image Image
		{
			get
			{
				return m_image;
			}
			set
			{
				if( value != m_image )
				{
					m_image = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates whether mouse positionin is over control.
		/// </summary>
		private bool IsMouseOver
		{
			get
			{
				return m_bIsMouseOver;
			}
			set
			{
				if( value != m_bIsMouseOver )
				{
					m_bIsMouseOver = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicate wether the image for button 
		/// is stretched or shrunk to fit the size of the button.
		/// </summary>
		public bool StretchImage
		{
			get
			{
				return m_stretchImage;
			}
			set
			{
				if( value != m_stretchImage )
				{
					m_stretchImage = value;
					OnStretchImageChanged();
				}
			}
		}

		#endregion

		#region Events 

		public event EventHandler CheckStateChanged;

		/// <summary>
		/// Occurs when <see cref="Image"/> is changed.
		/// </summary>
		public event EventHandler ImageChanged;

		/// <summary>
		/// Occurs when <see cref="StretchImage"/> is changed.
		/// </summary>
		public event EventHandler StretchImageChanged;

		#endregion

		private void RaiseImageChanged()
		{
			if( ImageChanged != null )
			{
				ImageChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseStretchImageChanged()
		{
			if( StretchImageChanged != null )
			{
				StretchImageChanged( this, EventArgs.Empty );
			}
		}

		protected virtual void OnImageChanged()
		{
			Invalidate();

			RaiseImageChanged();
		}

		protected virtual void OnStretchImageChanged()
		{
			Invalidate();

			RaiseStretchImageChanged();
		}

		/// <override/>
		protected void OnCheckStateChanged(EventArgs e)
		{
			if (CheckStateChanged != null)
			{
				try
				{
					CheckStateChanged(this, e);
				}
				catch (Exception ex)
				{
					TraceUtil.TraceExceptionCatched(ex);
					if (!ExceptionManager.RaiseExceptionCatched(null, ex))
						throw;
				}
			}
		}

		/// <summary>
		/// Gets / sets the flatcolor of the themed button.
		/// </summary>
        public Color FlatColor
		{
			get
			{
				return flatColor;
			}
			set
			{
				if(flatColor!=value)
				{
					flatColor = value;
					Invalidate();
				}
			}
		}
		/// <summary>
		/// Gets or sets the visual style of the themed button.
		/// </summary>
		[TypeConverter( typeof( DefaultVisualStyleEnumFilter ) )]
        public Syncfusion.Windows.Forms.VisualStyle Style
		{
			get
			{
				return style;
			}
			set
			{
				if(style != value)
				{
					style = value;

                    if (style == VisualStyle.Office2007)
                    {
                        this.office2007ColorTable = Office2007Colors.GetColorTable(m_office2007Theme);
                    }
                    else if (style == VisualStyle.Office2010)
                    {
                        this.office2010ColorTable = Office2010Colors.GetColorTable(m_office2010Theme);
                    }

					this.Invalidate();
				}
			}
		}

        /// <summary>
        /// Indicates the Office2007 theme used for drawing the control.
        /// </summary>
        [
        Description("Gets / sets a value indicating the Office2007 theme used for drawing the control."),
        Category("Appearance"),
        DefaultValue(Office2007Theme.Blue)
        ]
        public Office2007Theme Office2007Theme
        {
            get { return m_office2007Theme; }
            set
            {
                if (m_office2007Theme != value)
                {
                    m_office2007Theme = value;
                    
                    if (this.Style == VisualStyle.Office2007)
                    {
                        this.office2007ColorTable = Office2007Colors.GetColorTable(m_office2007Theme);

                        this.Invalidate();
                    }
                }
            }
        }
        /// <summary>
        /// Indicates the Office2010 theme used for drawing the control.
        /// </summary>
        [
        Description("Gets / sets a value indicating the Office2010 theme used for drawing the control."),
        Category("Appearance"),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010Theme
        {
            get { return m_office2010Theme; }
            set
            {
                if (m_office2010Theme != value)
                {
                    m_office2010Theme = value;
                    
                    if (this.Style == VisualStyle.Office2010)
                    {
                        this.office2010ColorTable = Office2010Colors.GetColorTable(m_office2010Theme);
                    
                    	this.Invalidate();
                    }
                }
            }
        }

		/// <summary>
		/// Indicates whether to draw the button text.
		/// </summary>
		public bool DrawText
		{
			get{return drawText;}
			set{if(drawText!=value){drawText = value;Invalidate();}}
		}

		/// <summary>
		/// Gets / sets the text alignment.
		/// </summary>
		public ContentAlignment TextAlign
		{
			get{return textAlign;}
			set{if(textAlign!=value){textAlign = value;Invalidate();}}
		}
//		public string Text
//		{
//			get{return text;}
//			set{ if(text!=value){ text = value;Invalidate();}}
//		}

		/// <summary>
		/// Gets / sets the checked state.
		/// </summary>
		public CheckState CheckState
		{
			get{return checkState;}
			set{if(checkState!=value){checkState = value;OnCheckStateChanged(EventArgs.Empty); Invalidate();}}
		}

		/// <summary>
		/// Gets / sets the default button state.
		/// </summary>
		public ButtonState DefaultButtonState
		{
			get{return defaultButtonState;}
			set{defaultButtonState = value;buttonState = value;}
		}

		/// <summary>
		/// Initializes a new object.
		/// </summary>
		public ThemedButtonBase()
		{
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint|WhidbeyCompatibleControlStyles.DoubleBuffer|ControlStyles.SupportsTransparentBackColor, true);
		}

		private void ThemedButtonBase_MouseEnter(object sender, System.EventArgs e)
		{
			buttonState = ButtonState.Normal;
			mouseOver = true;
			Invalidate(true);
		}

		private void ThemedButtonBase_MouseLeave(object sender, System.EventArgs e)
		{
			buttonState = defaultButtonState;
			mouseOver = false;
			Invalidate(true);
		}

		private void ThemedButtonBase_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (this.TabStop)
			{
				this.Focus();
			}

			if (e.Button == MouseButtons.Left)
			{
				buttonState = ButtonState.Pushed;
				switch (checkState)
				{
					case CheckState.Checked: checkState = CheckState.Unchecked; break;
					case CheckState.Unchecked: checkState = CheckState.Checked; break;
					case CheckState.Indeterminate: checkState = CheckState.Unchecked; break;
				}
			
				OnCheckStateChanged(EventArgs.Empty);
				
				Invalidate(true);
			}
		}

		private void ThemedButtonBase_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				buttonState = ButtonState.Normal;
				Invalidate(true);
			}
		}

		private void InitializeComponent()
		{
			// 
			// ThemedButtonBase.
			// 
			this.EnabledChanged += new System.EventHandler(this.ThemedButtonBase_EnabledChanged);
			this.SizeChanged += new System.EventHandler(this.ThemedButtonBase_SizeChanged);
			this.MouseEnter += new System.EventHandler(this.ThemedButtonBase_MouseEnter);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ThemedButtonBase_MouseUp);
			this.MouseLeave += new System.EventHandler(this.ThemedButtonBase_MouseLeave);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ThemedButtonBase_MouseDown);

		}

		#region Overrides

		protected override void OnMouseMove(MouseEventArgs e)
		{
			Point pt = this.PointToClient( Control.MousePosition );
			IsMouseOver = this.ClientRectangle.Contains( pt );

			base.OnMouseMove (e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				buttonState = ButtonState.Pushed;
				Invalidate(this.ClientRectangle);
				Refresh();
			}

			base.OnMouseDown(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (!Enabled) buttonState = ButtonState.Inactive;

			if (this.Image == null)
			{
				if (this.Style == Syncfusion.Windows.Forms.VisualStyle.Default)
				{
					if (this.ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsAppThemed && XPThemes.IsThemeActive)
						DrawThemedControl(e.Graphics, buttonState, checkState);
					else
						DrawNotThemedControl(e.Graphics, buttonState, checkState);
				}
				else
				{
					DrawStyledControl(e.Graphics, buttonState, checkState);
				}
			}
			else
			{
				DrawImage(e.Graphics, buttonState, checkState);
			}

			DrawControlText(e.Graphics);

			base.OnPaint(e);
		}

		protected virtual void DrawImage( Graphics g, ButtonState buttonState, CheckState checkState )
		{
			Rectangle rectImage;

			if( this.StretchImage )
			{
				// stretch or shrink image rectangle to the ClientRectangle
				Size stretchSize = new Size( this.ClientSize.Width - DEF_REDUCE_SIZE_IMAGE, this.ClientSize.Height - DEF_REDUCE_SIZE_IMAGE );
				Point location = new Point( this.ClientRectangle.X + DEF_SHIFT_LOCATION_IMAGE, this.ClientRectangle.Y + DEF_SHIFT_LOCATION_IMAGE );

				rectImage = new Rectangle( location, stretchSize );
			}
			else
			{
				// align image rectangle to center of ClientRectangle
				int x = ( Image.Width - ClientSize.Width ) / 2;
				int y = ( Image.Height - ClientSize.Height ) / 2;
					
				rectImage = new Rectangle( ClientRectangle.X - x, ClientRectangle.Y - y, this.Image.Width, this.Image.Height );
			}

			if( buttonState == ButtonState.Inactive )
			{
				// draw disabled button
				if( this.StretchImage )
				{
					Rectangle stretchRectangle = new Rectangle( ClientRectangle.X , ClientRectangle.Y , this.Image.Width, this.Image.Height );
					DrawingUtils.DrawGrayedImage( g, this.Image, rectImage, stretchRectangle, 0.5f );
				}
				else
				{
					DrawingUtils.DrawGrayedImage( g, this.Image, rectImage.Left, rectImage.Top, 0.5f );
				}
			}
			else
			{
				if( buttonState == ButtonState.Normal 
					|| ( buttonState == ButtonState.Pushed && !IsMouseOver ) )
				{
					DrawingUtils.DrawShadow( g, this.Image, rectImage.Left + 1, rectImage.Top + 1 );
					rectImage.Offset( - DEF_SHIFT_LOCATION_IMAGE, - DEF_SHIFT_LOCATION_IMAGE ); 
					g.DrawImage( this.Image, rectImage );
				}
				else
				{
					g.DrawImage( this.Image, rectImage );
				}
			}
		}

		protected override void OnParentBackColorChanged( EventArgs e )
		{
			Invalidate();
			base.OnParentBackColorChanged ( e );
		}
		/// <summary>
		/// Draws the text.
		/// </summary>
		/// <param name="g">The graphics object.</param>
		protected virtual void DrawControlText(Graphics g)
		{
			if(drawText) cd.DrawAlignedText(g,Text,this.Font,this.ForeColor,ClientRectangle,textAlign);
		}

		/// <summary>
		/// Draws the button themed.
		/// </summary>
		/// <param name="g">Graphics object.</param>
		/// <param name="buttonState">The button state.</param>
		/// <param name="checkState">The checked state.</param>
		protected virtual void DrawThemedControl(Graphics g,ButtonState buttonState,CheckState checkState)
		{

		}

		/// <summary>
		/// Draws the button without themes.
		/// </summary>
		/// <param name="g">Graphics object.</param>
		/// <param name="buttonState">The button state.</param>
		/// <param name="checkState">The checked state.</param>
		protected virtual void DrawNotThemedControl(Graphics g,ButtonState buttonState,CheckState checkState)
		{

		}

		/// <summary>
		/// Draws the styled button without themes.
		/// </summary>
		/// <param name="g">Graphics object.</param>
		/// <param name="buttonState">The button state.</param>
		/// <param name="checkState">The checked state.</param>
		protected virtual void DrawStyledControl(Graphics g,ButtonState buttonState,CheckState checkState)
		{

		}
		#endregion

		private void ThemedButtonBase_SizeChanged(object sender, System.EventArgs e)
		{
			Invalidate();
		}

		private void ThemedButtonBase_EnabledChanged(object sender, System.EventArgs e)
		{
			buttonState = Enabled ? this.defaultButtonState : ButtonState.Inactive;

			Invalidate();
		}
	}
}
