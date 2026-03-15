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
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{

	public class ProgressBarValueChangedEventArgs : EventArgs
	{
		private string text;
		private bool handled = false;
		/// <summary>
		/// Gets or sets a value indicating whether the Text is changed in ValueChanged event 
		/// </summary>
		public string Text
		{
			get { return text; }
			set { text = value; }
		}
		/// <summary>
		/// Gets or sets a value indicating whether the ValueChanged event was handled.
		/// </summary>
		public bool Handled
		{
			get { return handled; }
			set { handled = value; }
		}
	}

	public delegate void ProgressBarValueChangedEventHandler( object sender, ProgressBarValueChangedEventArgs e );


	/// <summary>
	/// ProgressBarAdv is an extension to the standard
	/// progress bar with many styles to choose from.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The ProgressBarAdv control has background, border and foreground styles.
	/// The background styles are <see cref="ProgressBarBackgroundStyles"/>.
	/// The border styles are <see cref="BorderStyle"/>.
	/// The	foreground styles are <see cref="ProgressBarStyles"/>.
	/// </para>
	/// <code lang="C#">
	/// this.progressBarEx1 = new ProgressBarAdv();
	/// this.progressBarEx1.BackGradientEndColor = System.Drawing.SystemColors.ControlLightLight;
	/// this.progressBarEx1.BackGradientStartColor = System.Drawing.SystemColors.ControlDark;
	/// this.progressBarEx1.BackgroundStyle = ProgressBarBackgroundStyles.VerticalGradient;
	/// this.progressBarEx1.BackSegments = false;
	/// this.progressBarEx1.Border3DStyle = System.Windows.Forms.Border3DStyle.RaisedOuter;
	/// this.progressBarEx1.FontColor = System.Drawing.SystemColors.HighlightText;
	/// this.progressBarEx1.Location = new System.Drawing.Point(240, 8);
	/// this.progressBarEx1.ProgressStyle = ProgressBarStyles.Tube;
	/// this.progressBarEx1.SegmentWidth = 20;
	/// this.progressBarEx1.Size = new System.Drawing.Size(400, 23);
	/// this.progressBarEx1.TextShadow = false;
	/// this.progressBarEx1.ThemesEnabled = false;
	/// this.progressBarEx1.TubeEndColor = System.Drawing.SystemColors.Control;
	/// this.progressBarEx1.TubeStartColor = System.Drawing.SystemColors.ControlDark;
	/// this.progressBarEx1.Value = 79;
	///</code>
	/// </remarks>
    [Designer(typeof(ProgressBarAdvDesigner), typeof(System.ComponentModel.Design.IDesigner))]
	[ToolboxItem( true )]
	[ToolboxBitmap( typeof( Syncfusion.Windows.Forms.PopupControlContainer ), "ToolboxIcons.ProgressBarEx.bmp" )]
	[Description( "Extended ProgressBar control with many styles to choose from." )]
	public class ProgressBarAdv : ThemedControl, ISupportInitialize
	{
		private System.ComponentModel.IContainer components;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private static Size CTRLSIZE = default(Size);

		/// <summary>
		/// Handle this event to set the text of the progressbar when the value changes.
		/// Make sure the TextStyle property is set to Custom.
		/// </summary>
		/// <remarks>
		/// To set the text of the progressbar, set the <see cref="ProgressBarValueChangedEventArgs.Text"/> and the <see cref="ProgressBarValueChangedEventArgs.Handled"/> to True.
		/// </remarks>
		[Description(" Handle this event to set the text of the progressbar when the value changes. Make sure the TextStyle property is set to Custom.")]
		public event ProgressBarValueChangedEventHandler ValueChanged;

		protected virtual void OnValueChanged( ProgressBarValueChangedEventArgs e )
		{
			if( ValueChanged != null )
			{
				ValueChanged( this, e );
			}
		}

		/// <summary>
		/// Handle this event to draw a custom waiting render.
		/// WaitingCustomRender must be set to True.
		/// </summary>
		[Description("Handle this event to draw a custom waiting render. WaitingCustomRender must be set to True.")]
		public event ProgressBarAdvDrawEventHandler DrawWaitingCustomRender;

		protected virtual void OnDrawWaitingCustomRender( ProgressBarAdvDrawEventArgs e )
		{
			if( DrawWaitingCustomRender != null )
			{
				DrawWaitingCustomRender( this, e );
			}
		}

		bool inInit = false;

		/// <summary>
		/// Starts initialization mode.
		/// </summary>
		public void BeginInit()
		{
			inInit = true;
		}

		/// <summary>
		/// Ends initialization mode and calls <see cref="RefreshBrushes"/>.
		/// </summary>
		public void EndInit()
		{
			inInit = false;
			RefreshBrushes();
		}

		/// <summary>
		/// Implementation of the <see cref="ISupportInitialize"/> interface.
		/// </summary>
		/// <param name="refreshBrushes">Set this to False if you want to refresh brushes later.</param>
		public void EndInit( bool refreshBrushes )
		{
			inInit = false;
			if( refreshBrushes )
				RefreshBrushes();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ProgressBarAdv()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new Syncfusion.Core.Licensing.LicensedComponent( typeof( ProgressBarAdv ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}

			// This call is required by the Windows.Forms Form Designer.
			SetStyle( ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint | WhidbeyCompatibleControlStyles.DoubleBuffer | ControlStyles.SupportsTransparentBackColor, true );

			InitializeComponent();
			waitingGradientRect = new Rectangle( 0, 0, Width, Height );

			if( XPThemes.IsThemedOS )
			{
				tc = new ThemedControlDrawing( ThemedControls.PROGRESS, this );
			}

			this.RefreshBrushes();
		}

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
				{
					components.Dispose();
				}

				if( this.tc != null )
				{
					this.tc.Dispose();
					this.tc = null;
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timer = new System.Windows.Forms.Timer( this.components );
			//
			// timer
			//
			this.timer.Interval = 10;
			this.timer.Tick += new System.EventHandler( this.timer_Tick );
			//
			// ProgressBarAdv
			//
			this.Size = new System.Drawing.Size( 400, 23 );
			this.Resize += new System.EventHandler( this.OnResize );
			this.Paint += new System.Windows.Forms.PaintEventHandler( this.OnBarPaint );

		}
		#endregion

		private System.Windows.Forms.Timer timer;

		private int offset
		{
			get
			{
				if( offs != 3 ) return offs;
				if( ( borderStyle == BorderStyle.Fixed3D ) ||
					( this.progressStyle == ProgressBarStyles.System ) ||
					this.BackgroundStyle == ProgressBarBackgroundStyles.System )
					return 3;
				else
					return 1;
			}
			set
			{
				offs = value;
			}

		}
		private int offs = 3;
		/// <summary>
		/// Paints the control.
		/// </summary>
		/// <param name="sender">Event sender.<see cref="object"/></param>
		/// <param name="e">Event data.<see cref="PaintEventArgs"/></param>
		private void OnBarPaint( object sender, System.Windows.Forms.PaintEventArgs e )
		{
			//base.OnPaint(e);
			Rectangle rect = ClientRectangle;//new Rectangle(/*Location*/new Point(0,0),ClientSize);
			rect.Width -= offset;
			rect.Height -= offset;
			rect.X += offset;
			rect.Y += offset;

			if( maximum != minimum )
			{
				if( progressOrientation == Orientation.Horizontal )
				{
					rect.Width = ( int )( ( ( double )ivalue - minimum ) * 100 / ( maximum - minimum ) * rect.Width / 100 );
				}
				else
				{
					rect.Height = ( int )( ( double )ivalue - minimum ) * 100 / ( maximum - minimum ) * rect.Height / 100;
				}
			}

			bool bIsMirrored = GetIsMirrored();
			bool bVertical = ( progressOrientation == Orientation.Vertical );

			Graphics g = e.Graphics;

			if( bVertical )
			{
				float dX = 0, dY = 0, dAngle;
				if( bIsMirrored )
				{
					dX = ClientSize.Width;
					dAngle = 90;
				}
				else
				{
					dY = ClientSize.Height;
					dAngle = -90;
				}

				g.TranslateTransform( dX, dY );
				g.RotateTransform( dAngle );

				int w = rect.Width;
				rect.Width = rect.Height;
				rect.Height = w;
			}

			Matrix mtrxBack = g.Transform;
			if( bIsMirrored )
			{
				Matrix mtrxMult = bVertical ?
					new Matrix( 1, 0, 0, -1, 0, ClientSize.Height ) :
					new Matrix( -1, 0, 0, 1, ClientSize.Width, 0 );
				mtrxMult.Multiply( mtrxBack );
				g.Transform = mtrxMult;
			}

			offset = 0;
			DrawBackground( e.Graphics, backSegments );
			offset = 3;

			DrawBorder( e.Graphics, GetClientRectangle() );

			DrawProgress( g, rect );

			if( bIsMirrored )
			{
				g.Transform = mtrxBack;
			}

			DrawText( g, this.ClientRectangle );
		}
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }

		private bool GetIsMirrored()
		{
			return RightToLeft.Yes == RightToLeft;
		}

		bool hasHandle = false;

		/// <override/>
		protected override void OnHandleCreated( EventArgs e )
		{
			hasHandle = true;
			base.OnHandleCreated( e );
		}

		/// <overload>
		/// Draws the contents of the progress bar at the specified rectangle. This method can only
		/// be called when the control has no handle attached or is not visible.
		/// </overload>
		/// <summary>
		/// Draws the contents of the progress bar at the specified rectangle. This method can only
		/// be called when the control has no handle attached or is not visible.
		/// </summary>
		/// <param name="g">The graphics context.</param>
		/// <param name="rect">The rectangle.</param>
		/// <remarks>
		/// Essential Grid calls this method to render the contents of the progress bar
		/// within a cell's area.
		/// <para/>
		/// No border will be drawn around the control.
		/// <para/>
		/// The method will throw an InvalidOperationException if the control has a handle
		/// and is visible.
		/// <para/>
		/// The method will reset the bounds and BorderStyle of this control.
		/// <note type="note">This method only supports horizontal progress bars. We will add support for static drawing of vertical progress bars in the future.</note>
		/// </remarks>
		public void Draw( Graphics g, Rectangle rect )
		{
			if( progressOrientation == Orientation.Vertical )
			{
				progressOrientation = Orientation.Horizontal;

				Rectangle destRect = rect;
				Rectangle rectangle = new Rectangle( 0, 0, destRect.Height, destRect.Width );  // flip Width and Height
				rectangle.Offset( -rectangle.X, -rectangle.Y );
				Bitmap bm = new Bitmap( rectangle.Width, rectangle.Height );
				Graphics bmg = Graphics.FromImage( bm );

				_Draw( bmg, rectangle );

				bmg.Dispose();

				bm.RotateFlip( RotateFlipType.Rotate270FlipNone );

				g.DrawImageUnscaled( bm, destRect );

				progressOrientation = Orientation.Vertical;
			}
			else
				_Draw( g, rect );
		}

		void _Draw( Graphics g, Rectangle rect )
		{
			inDraw = true;
			inDrawRectangle = rect;

			if( hasHandle && Visible )
				throw new InvalidOperationException( "This method can only be called when the control has no handle attached or is not visible." );

			Bounds = rect;
			BorderStyle = BorderStyle.None;
			RefreshBrushes();

			if( BackgroundImage != null )
			{
				Rectangle src = new Rectangle( 0, 0, Math.Min( BackgroundImage.Width, rect.Width ), Math.Min( BackgroundImage.Height, rect.Height ) );
				g.DrawImage( BackgroundImage, rect, src, GraphicsUnit.Pixel );
			}

			offset = 0;
			DrawBackground( g, backSegments );
			offset = 3;

			rect.Width -= 2 * offset;
			rect.Height -= 2 * offset;

			rect.Width = ( int )( ( ( double )ivalue - minimum ) * 100 / ( maximum - minimum ) * rect.Width / 100 );

			DrawProgress( g, rect );

      DrawText(g, inDrawRectangle);

			inDraw = false;
		}

		/// <summary>
		/// Draws the contents of the progress bar at the specified rectangle. This method can only
		/// be called when the control has no handle attached or is not visible.
		/// </summary>
		/// <param name="g">The graphics context.</param>
		/// <param name="clientRectangle">The rectangle.</param>
		/// <param name="drawRightToLeft">Specifies if the progressbar should be drawn right to left.</param>
		/// <remarks>
		/// Essential Grid calls this method to render the contents of the progress bar
		/// within a cell's area.
		/// <para/>
		/// No border will be drawn around the control.
		/// <para/>
		/// The method will throw an InvalidOperationException if the control has a handle
		/// and is visible.
		/// <para/>
		/// The method will reset bounds and BorderStyle of this control.
		/// <note type="note">This method only support horizontal progress bars. We will add support for static drawing of vertical progress bars in the future.</note>
		/// </remarks>
		public void Draw( Graphics g, Rectangle clientRectangle, bool drawRightToLeft )
		{
			if( progressOrientation == Orientation.Horizontal && drawRightToLeft )
			{
				RightToLeft = RightToLeft.Yes;
				System.Drawing.Drawing2D.Matrix mtrxBack = g.Transform;

				System.Drawing.Drawing2D.Matrix mtrxMult = mtrxBack.Clone();
				mtrxMult.Translate( -clientRectangle.Left, -clientRectangle.Y, System.Drawing.Drawing2D.MatrixOrder.Append );
				System.Drawing.Drawing2D.Matrix mtrxMult1 = new System.Drawing.Drawing2D.Matrix( -1, 0, 0, 1, clientRectangle.Width, 0 );
				mtrxMult1.Multiply( mtrxMult );
				mtrxMult1.Translate( clientRectangle.Left, clientRectangle.Y, System.Drawing.Drawing2D.MatrixOrder.Append );

				g.Transform = mtrxMult1;

				//Hide the text so it does not get flipped.
				text = false;
				this.Draw( g, clientRectangle );
				text = true;

				g.Transform = mtrxBack;

				clientRectangle.Width -= 2 * offset;
				clientRectangle.Height -= 2 * offset;

				inDraw = true;
				DrawText( g, clientRectangle );
				inDraw = false;

			}
			else
			{
				RightToLeft = RightToLeft.No;
				this.Draw( g, clientRectangle );
			}
		}

		bool inDraw = false;
		Rectangle inDrawRectangle = Rectangle.Empty;

		Rectangle GetClientRectangle()
		{
			return inDraw ? inDrawRectangle : ClientRectangle;
		}
		Size GetClientSize()
		{
			return inDraw ? inDrawRectangle.Size : ClientSize;
		}

		int GetLeft()
		{
			return inDraw ? inDrawRectangle.Left : 0;
		}

		int GetTop()
		{
			return inDraw ? inDrawRectangle.Top : 0;
		}

		private string GetText()
		{
            if (textStyle == ProgressBarTextStyles.Custom)
            {
                this.customtext = this.CustomText;
                //this.Refresh();
                return customtext;
            }
            else
                if (textStyle == ProgressBarTextStyles.Percentage)
                {
                    if (minimum == maximum) return "0%";

                    int calcValue = (int)((double)(ivalue - minimum) * 100 / (maximum - minimum));

                    return calcValue.ToString() + "%";
                }
                else
                {
                    return (ivalue).ToString() + "\\" + (maximum).ToString();
                }
		}
		private void DrawText( Graphics g, Rectangle rect )
		{
			StringFormat drawFormat = new StringFormat();

			if( !inDraw && ( textOrientation == Orientation.Vertical && !Vertical() ) || ( textOrientation == Orientation.Horizontal && Vertical() ) )
			{
				drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
			}

			if( text )
			{
				string txt = GetText();
				Size txtSize = g.MeasureString( txt, this.Font, 0, drawFormat ).ToSize();
				if( this.Vertical() )
                {
                    txtSize = new Size( txtSize.Height, txtSize.Width );
                }
                int x = rect.Left + ( rect.Width - txtSize.Width ) / 2;
				int y = rect.Top + ( rect.Height - txtSize.Height ) / 2;
				Point p = Vertical() ? new Point( y, x ) : new Point( x, y );

				if( textShadow )
				{
					g.DrawString( txt, this.Font, Brushes.Black, new Point( p.X + 1, p.Y + 1 ), drawFormat );
				}
                using (Brush brush = new SolidBrush( this.fontColor))
                    g.DrawString(txt, this.Font, brush, p, drawFormat);

				this.lastText = txt;
			}

			drawFormat.Dispose();
		}


		#region Variables

		private ThemedControlDrawing tc;
		private int minimum = 0;
		private int maximum = 100;
		private int ivalue = 50;
		private ProgressBarTextStyles textStyle = ProgressBarTextStyles.Percentage;
		private int segmentwidth = 12;
		private bool text = true;
		private bool textShadow = true;
		private ArrayList foreColors = new ArrayList();
		private ArrayList backColors = new ArrayList();
		private Border3DStyle border3DStyle = Border3DStyle.Sunken;
		private BorderStyle borderStyle = BorderStyle.Fixed3D;
        protected BorderStyle prev_borderStyle = BorderStyle.FixedSingle;
		private ButtonBorderStyle borderSingle = ButtonBorderStyle.Solid;
		private Image foregroundImage;
		private Color foregroundColor = Color.DarkCyan;
		private Color fontColor = Color.White;
		private Color borderColor = Color.Black;
        protected Color prev_borderColor = ColorTranslator.FromHtml("#939598");
        private Color foreStartColor = Color.Red;
        protected Color prev_foreStartColor = Color.FromArgb(22, 165, 220);
		private Color foreEndColor = Color.Lime;
        protected Color prev_foreEndColor = Color.FromArgb(22, 165, 220);
		private Color tubeStartColor = Color.Red;
		private Color tubeEndColor = Color.Black;
        private Color backStartColor = ColorTranslator.FromHtml("#D1D3D4");
        private Color backEndColor = ColorTranslator.FromHtml("#D1D3D4");
		private Color backTubeStartColor = Color.LightGray;
		private Color backTubeEndColor = Color.White;
		private Brush vbackGradBrush = null;
		private Brush backFillBrush = null;
		private Brush foreFillBrush = null;
		private Brush foreGradientBrush = null;
		private Brush foreWaitingGradientBrush = null;
		private Brush backGradientBrush = null;
		private Brush foreTubeBrush = null;
		private Brush backTubeBrush = null;

		private Rectangle waitingGradientRect = Rectangle.Empty;

		private ProgressBarBackgroundStyles backgroundStyle = ProgressBarBackgroundStyles.None;
		private ProgressBarBackgroundStyles backgroundFallbackStyle = ProgressBarBackgroundStyles.None;
		private ProgressBarStyles progressStyle = ProgressBarStyles.Constant;
		private ProgressBarStyles progressFallbackStyle = ProgressBarStyles.Constant;
		private Orientation progressOrientation;
		private Orientation textOrientation = Orientation.Horizontal;
		private bool stretchMultGrad = true;
		private bool stretchImage = true;
		private int step = 10;
		private bool foreSegments = true;
		private bool backSegments = false;
		private string lastText = "";
		private bool customWaitingRender = false;
		//		private bool optimizedSpeed = false;
		private TextAlignment textAlign = TextAlignment.Center;
        private string customtext;
        /// <summary>
        ///Scalig value
        /// </summary>
        private bool isScaling = false;
        /// <summary>
        ///scale factor value
        /// </summary>
        private float _scalefactor = 1f;
		#endregion

		#region Behavior properties

		//		/// <summary>
		//		/// Determines if the ProgressBar will refresh faster when the value is changed.
		//		/// </summary>
		//		/// <remarks>
		//		///  If True, the appearance of the text will suffer. Call ProgressBarExt.InvalidateBar() to refresh the ProgressBar.
		//		///  If False, the ProgressBar will refresh each time the value changes but speed will be lower.
		//		/// </remarks>
		//		/// <example>
		//		/// progressBarEx1.Minimum = 0;
		//		///	progressBarEx1.Maximum = 10000;
		//		///	for(int i=0;i<10000;i++)
		//		///	{
		//		///	progressBarEx1.Value = i;
		//		///	Application.DoEvents();
		//		///	}
		//		///	this.progressBarEx1.InvalidateBar();
		//		/// </example>
		//		[Category("Behavior")]
		//		[DefaultValue(false)]
		//		[Description("Determines if the ProgressBar will refresh faster when the value is changed.")]
		//		public bool OptimizedSpeed
		//		{
		//			get{return optimizedSpeed;}
		//			set{optimizedSpeed = value;}
		//		}

          
		/// <summary>
		/// Gets / sets the value between minimum and maximum.
		/// </summary>
		/// <remarks>
		/// This value represents the progress state of the ProgessBar. For default if it is set to 50, minimum=0 and maximum=100 ( 50% ).
		/// </remarks>
		[Category( "Behavior" )]
		[DefaultValue( 50 )]
		[Description( "The current value between the minimum and maximum values." )]
		public int Value
		{
			get { return ivalue; }
			set
			{
				if( value == ivalue ) return;

				if( value > maximum )
				{
					SetValue( maximum );
					InvalidateBar( true );
					UpdateBar();
					return;
				}
				if( value < minimum )
				{
					SetValue( minimum );
					InvalidateBar( true );
					UpdateBar();
					return;
				}
				if( ( textStyle == ProgressBarTextStyles.Percentage ) && text && GetText() != lastText/* && !optimizedSpeed*/)
				{
					if( value != ivalue )
					{
						SetValue( value );
						InvalidateBar( true );
						UpdateBar();
					}
				}
				else
				{
					if( textStyle != ProgressBarTextStyles.Percentage )
					{
						SetValue( value );
						InvalidateBar( true );
						UpdateBar();
					}
					else
					{

						if( ( ( int )( ( ( double )ivalue - minimum ) * 100 / ( maximum - minimum ) * Width / 100 ) )
							== ( ( int )( ( ( double )value - minimum ) * 100 / ( maximum - minimum ) * Width / 100 ) ) )
						{
							SetValue( value );
                            //if (text)
                            //{
                            //    DrawText(CreateGraphics(), new Rectangle(offset, offset, Width - 2 * offset, Height - 2 * offset));
                            //}
							if( ivalue == maximum )
							{
								InvalidateBar();
								UpdateBar();
							}
							return;
						}
						SetValue( value );
						InvalidateBar( true );
						UpdateBar();

					}
				}

			}

           
		}

		private void SetValue( int value )
		{
			ivalue = value;
			ProgressBarValueChangedEventArgs e = new ProgressBarValueChangedEventArgs();
			this.OnValueChanged( e );
			if( e.Handled )
				this.customtext = e.Text;
		}

		void InvalidateBar()
		{
			if( inInit )
				return;

			base.Invalidate();
		}

		void InvalidateBar( bool b )
		{
			if( inInit )
				return;

			base.Invalidate( b );
		}

		void UpdateBar()
		{
			if( inInit )
				return;

			base.Update();
		}

		/// <summary>
		/// Gets / sets the lower boundary for the value.
		/// </summary>
		/// <remarks>
		/// By default, its value is zero which means that the value of the ProgressBar cannot take values lower than zero.
		/// </remarks>
		[Category( "Behavior" )]
		[DefaultValue( 0 )]
		[Description( "The lower bound of the range of the ProgressBar." )]
		public int Minimum
		{
			get { return minimum; }
			set
			{
				if( value > ivalue )
				{
					if( value > maximum )
					{
						minimum = maximum = ivalue = value;
						InvalidateBar( true );
						return;
					}
					ivalue = value;
					InvalidateBar( true );
					return;
				}
				bool invalidate = false;
				if( minimum != value )
					invalidate = true;
				minimum = value;
				if( invalidate )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Gets / sets the upper boundary for the value.
		/// </summary>
		/// <remarks>
		/// By default, its value is 100 which means that the value of the ProgressBar cannot take values higher than 100.
		/// </remarks>
		[Category( "Behavior" )]
		[DefaultValue( 100 )]
		[Description( "The higher bound of the range of the ProgressBar." )]
		public int Maximum
		{
			get { return maximum; }
			set
			{
				if( value < ivalue )
				{
					if( value < minimum )
					{
						minimum = maximum = ivalue = value;
						InvalidateBar( true );
						return;
					}
					ivalue = value;
					maximum = value;
					InvalidateBar( true );
					return;
				}
				bool invalidate = false;
				if( maximum != value ) invalidate = true;
				maximum = value;
				if( invalidate )
					InvalidateBar( true );
			}
		}
		/// <summary>
		/// Gets / sets the value to increment when Increment() and Decrement() methods.
		/// </summary>
		/// <remarks>
		/// By default, its value is 10 which means that when Increment() is called, the value of the ProgressBar is incremented by 10.
		/// </remarks>
		[Category( "Behavior" )]
		[DefaultValue( 10 )]
		[Description( "The amount to increment the value of the ProgressBar when Increment() is called" )]
		public int Step
		{
			get { return step; }
			set { step = value; }
		}
		#endregion

		#region Foreground properties


		/// <summary>
		/// Indicates whether the waiting gradient will be replaced by another custom waiting render which is defaulted to segments.
		/// To customize it, handle the DrawCustomWaitingRender event.
		/// </summary>
		[Description( "Indicates whether the waiting gradient will be replaced by another custom waiting render which is defaulted to segments." )]
		[Category( "Foreground Gradient" )]
		public bool CustomWaitingRender
		{
			get
			{
				return customWaitingRender;
			}
			set
			{
				if( customWaitingRender != value )
				{
					customWaitingRender = value;
					this.InvalidateBar();
				}
			}
		}

		/// <summary>
		/// Gets / sets the width of the waiting gradient.
		/// </summary>
		[Description( "Determines the width of the waiting gradient." )]
		[Category( "Foreground Gradient" )]
		public int WaitingGradientWidth
		{
			get { return waitingGradientRect.Width; }
			set { waitingGradientRect.Width = value; RefreshBrushes(); }
		}

		/// <summary>
		/// Indicates whether the waiting gradient is enabled.
		/// </summary>
		[Description( "Determines if the waiting gradient is enabled." )]
		[Category( "Foreground Gradient" )]
		[DefaultValue( false )]
		public bool WaitingGradientEnabled
		{
			get { return timer.Enabled; }
			set { timer.Enabled = value; }
		}
		/// <summary>
		/// Gets / sets the interval of the waiting gradient.
		/// </summary>
		[Description( "Determines the interval of the waiting gradient." )]
		[Category( "Foreground Gradient" )]
		[DefaultValue( 10 )]
		public int WaitingGradientInterval
		{
			get { return timer.Interval; }
			set { timer.Interval = value; }
		}
		/// <summary>
		/// Indicates whether the foreground is segmented.
		/// </summary>
		/// <remarks>
		/// By default, its value is True which means that the foreground will be drawn segmented.
		/// </remarks>
		[Category( "Foreground Gradient" )]
		[DefaultValue( true )]
		[Description( "Determines if the foreground is segmented." )]
		public bool ForeSegments
		{
			get { return foreSegments; }
			set
			{
				bool invalidate = false;
				if( foreSegments != value ) invalidate = true;
				foreSegments = value;
				if( invalidate )
					InvalidateBar( true );
			}
		}


		/// <summary>
		/// Indicates whether the multiple gradient is compressed if the value is smaller than the maximum.
		/// </summary>
		/// <remarks>
		/// By default, its value is True which means that if the value is less than the maximum, the multiple gradient is compressed.
		/// </remarks>
		[Category( "Foreground Gradient" )]
		[DefaultValue( true )]
		[Description( "Determines if the multiple gradient will be stretched." )]
		public bool StretchMultGrad
		{
			get { return stretchMultGrad; }
			set
			{
				bool invalidate = false;
				if( stretchMultGrad != value && progressStyle == ProgressBarStyles.MultipleGradient ) invalidate = true;
				stretchMultGrad = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar( true );
			}
		}


		/// <summary>
		/// Gets / sets the colors of the foreground multiple gradient when ForegroundStyle is multiple gradient.
		/// </summary>
		/// <remarks>
		///	By default, its value is an empty color array. You can add colors to multiple gradients by modifying this property.
		/// </remarks>
		[Category( "Foreground Gradient" )]
		[Description( "The array of colors used in multiple gradients of the foreground." )]
		public Color[] MultipleColors
		{
			get
			{
				Color[] ca = new Color[ foreColors.Count ];
				for( int i = 0; i < foreColors.Count; i++ )
				{
					ca.SetValue( ( Color )foreColors[ i ], i );
				}
				return ca;
			}
			set
			{
				foreColors.Clear();
				for( int i = 0; i < value.Length; i++ )
				{
					foreColors.Add( value[ i ] );
				}
				RefreshBrushes();
				if( progressStyle == ProgressBarStyles.MultipleGradient )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Gets / sets the start color of the foreground gradient when ForegroundStyle is gradient.
		/// </summary>
		[Category( "Foreground Gradient" )]
		[Description( "The start color of the dual gradient of the foreground." )]
		public Color GradientStartColor
		{
			get { return foreStartColor; }
			set
			{
				bool invalidate = false;
				if( foreStartColor != value && progressStyle == ProgressBarStyles.Gradient ) invalidate = true;
				foreStartColor = value;
                prev_foreStartColor = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Gets / sets the end color of the foreground gradient when ForegroundStyle is gradient.
		/// </summary>
		[Category( "Foreground Gradient" )]
		[Description( "The end color of the dual gradient of the foreground." )]
		public Color GradientEndColor
		{
			get { return foreEndColor; }
			set
			{
				bool invalidate = false;
				if( foreEndColor != value && progressStyle == ProgressBarStyles.Gradient ) invalidate = true;
				foreEndColor = value;
                prev_foreEndColor = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar();
			}
		}
        /// <summary>
        ///  Serialize the gradient color
        /// </summary>
        protected bool ShouldSerializeGradientStartColor()
        {
            if (this.foreStartColor == Color.White)
                return false;
            return true;
        }

        /// <summary>
        /// Resets the gradient color
        /// </summary>
        public void ResetGradientStartColor()
        {
            this.foreStartColor = Color.FromArgb(94, 194, 25);
        }
        /// <summary>
        /// Serialize the gradient color
        /// </summary>
        protected bool ShouldSerializeGradientEndColor()
        {
            if (this.foreEndColor == Color.White)
                return false;
            return true;
        }

        /// <summary>
        ///  Resets the gradient color
        /// </summary>
        public void ResetGradientEndColor()
        {
            this.foreEndColor = Color.FromArgb(19, 158, 214);
        }
		/// <summary>
		/// Gets / sets the start color of the foreground tube when ForegroundStyle is Tube.
		/// </summary>
		[Category( "Foreground Gradient" )]
		[Description( "The middle color of the tube of the foreground." )]
		public Color TubeStartColor
		{
			get { return tubeStartColor; }
			set
			{
				bool invalidate = false;
				if( tubeStartColor != value && progressStyle == ProgressBarStyles.Tube ) invalidate = true;
				tubeStartColor = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar();
			}
		}

		/// <summary>
		/// Gets / sets the end color of the foreground tube when ForegroundStyle is Tube.
		/// </summary>
		[Category( "Foreground Gradient" )]
		[Description( "The outer color of the tube of the foreground." )]
		public Color TubeEndColor
		{
			get { return tubeEndColor; }
			set
			{
				bool invalidate = false;
				if( tubeEndColor != value && progressStyle == ProgressBarStyles.Tube ) invalidate = true;
				tubeEndColor = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar();
			}
		}

		#endregion

		#region Background properties

		/// <summary>
		/// Indicates whether the background is segmented.
		/// </summary>
		/// <remarks>
		/// By default, its value is False.
		/// </remarks>
		[Category( "Background Gradient" )]
		[DefaultValue( true )]
		[Description( "Determines if the background is segmented." )]
		public bool BackSegments
		{
			get { return backSegments; }
			set
			{
				bool invalidate = false;
				if( backSegments != value ) invalidate = true;
				backSegments = value;
				if( invalidate )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Gets / sets the colors of the background multiple gradients when BackgroundStyle is multiple gradient.
		/// </summary>
		/// <remarks>
		/// By default, its value is an empty array of colors.
		/// </remarks>
		[Category( "Background Gradient" )]
		[Description( "The array of colors used to draw the multiple gradient of the background." )]
		public Color[] BackMultipleColors
		{
			get
			{
				Color[] ca = new Color[ backColors.Count ];
				for( int i = 0; i < backColors.Count; i++ )
				{
					ca.SetValue( ( Color )backColors[ i ], i );
				}
				return ca;
			}
			set
			{
				backColors.Clear();
				for( int i = 0; i < value.Length; i++ )
				{
					backColors.Add( value[ i ] );
				}
				RefreshBrushes();
				if( backgroundStyle == ProgressBarBackgroundStyles.MultipleGradient )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Gets / sets the start color of the background gradient when BackgroundStyle is gradient or vertical gradient.
		/// </summary>
		[Category( "Background Gradient" )]
		[Description( "The start color of the dual gradient of the background." )]
		public Color BackGradientStartColor
		{
			get { return backStartColor; }
			set
			{
				bool invalidate = false;
				if( backStartColor != value && backgroundStyle == ProgressBarBackgroundStyles.Gradient ) invalidate = true;
				backStartColor = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar();
			}
		}

		/// <summary>
		/// Gets / sets the end color of the background gradient when BackgroundStyle is gradient or vertical gradient.
		/// </summary>
		[Category( "Background Gradient" )]
		[Description( "The end color of the dual gradient of the background." )]
		public Color BackGradientEndColor
		{
			get { return backEndColor; }
			set
			{
				bool invalidate = false;
				if( backEndColor != value && backgroundStyle == ProgressBarBackgroundStyles.Gradient ) invalidate = true;
				backEndColor = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar();
			}
		}

		/// <summary>
		/// Gets / sets the start color of the background tube when BackgroundStyle is Tube.
		/// </summary>
		[Category( "Background Gradient" )]
		[Description( "The middle color of the tube of the background." )]
		public Color BackTubeStartColor
		{
			get { return backTubeStartColor; }
			set
			{
				bool invalidate = false;
				if( backTubeStartColor != value && backgroundStyle == ProgressBarBackgroundStyles.Tube ) invalidate = true;
				backTubeStartColor = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar();
			}
		}

		/// <summary>
		/// Gets / sets the end color of the background tube when BackgroundStyle is Tube.
		/// </summary>
		[Category( "Background Gradient" )]
		[Description( "The outer color of the tube of the background." )]
		public Color BackTubeEndColor
		{
			get { return backTubeEndColor; }
			set
			{
				bool invalidate = false;
				if( backTubeEndColor != value && backgroundStyle == ProgressBarBackgroundStyles.Tube ) invalidate = true;
				backTubeEndColor = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar();
			}
		}

		#endregion

		#region Appearance properties
		/// <summary>
		/// Gets / sets the text alignment of the ProgressBarAdv.
		/// </summary>
		[Description( "Indicates the text alignment of the ProgressBarAdv." )]
		[Category( "Appearance" )]
		[DefaultValue( TextAlignment.Center )]
		public TextAlignment TextAlignment
		{
			get
			{
				return this.textAlign;
			}
			set
			{
				if( this.textAlign != value )
				{
					this.textAlign = value;
					InvalidateBar( true );
				}
			}
		}

		/// <summary>
		/// Indicates whether the foreground image will be stretched.
		/// </summary>
		/// <remarks>
		/// By default, its value is True.
		/// </remarks>
		[Category( "Appearance" )]
		[DefaultValue( true )]
		[Description( "Determines if the foreground image will be stretched." )]
		public bool StretchImage
		{
			get { return stretchImage; }
			set
			{
				bool invalidate = false;
				if( stretchImage != value && progressStyle == ProgressBarStyles.Image ) invalidate = true;
				stretchImage = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Gets / sets the image to draw on the foreground when ProgressStyle is Image.
		/// </summary>
		[Category( "Appearance" )]
		[DefaultValue( true )]
		[Description( "The image used to draw the foreground." )]
		public Image ForegroundImage
		{
			get { return foregroundImage; }
			set
			{
				bool invalidate = false;
				if( foregroundImage != value && progressStyle == ProgressBarStyles.Image ) invalidate = true;
				foregroundImage = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar();
			}
		}

		/// <summary>
		/// Gets / sets the width of the segments.
		/// </summary>
		/// <remarks>By default, its value is 12.</remarks>
		[Category( "Appearance" )]
		[Description( "The width of the segments." )]
		public int SegmentWidth
		{
			get { return segmentwidth; }
			set
			{
				bool invalidate = false;
				if( segmentwidth != value && foreSegments ) invalidate = true;
				segmentwidth = value;
				if( invalidate )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Gets / sets the color of the font.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "The color of the font used to draw the text of the ProgressBar." )]
		public Color FontColor
		{
			get { return fontColor; }
			set
			{
				bool invalidate = false;
				if( fontColor != value && text ) invalidate = true;
				fontColor = value;
				if( invalidate )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Gets / sets the color used to draw the foreground in segment mode and constant mode.
		/// </summary>
		[Description( "The color used to draw the foreground in segment mode and constant mode." )]
		public override Color ForeColor
		{
			get
			{
				return foregroundColor;
			}
			set
			{
				bool invalidate = false;
				if( foregroundColor != value && progressStyle == ProgressBarStyles.Constant ) invalidate = true;
				base.ForeColor = value;
				foregroundColor = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar( true );
			}

		}
		/// <summary>
		/// Indicates whether the text is visible.
		/// </summary>
		[Category( "Appearance" )]
		[DefaultValue( true )]
		[Description( "Determines if the text of the Progressbar is visible." )]
		public Boolean TextVisible
		{
			get { return text; }
			set
			{
				bool invalidate = false;
				if( text != value ) invalidate = true;
				text = value;
				if( invalidate )
					InvalidateBar( true );
			}
		}

		/// <summary>
		///Gets / sets the style of the text:
		///	-Percentage
		///	-Value (Ex:  70/150 )
		/// </summary>
		[Category( "Appearance" )]
		[DefaultValue( ProgressBarTextStyles.Percentage )]
		[Description( "Determines the style of the text." )]
		public ProgressBarTextStyles TextStyle
		{
			get { return textStyle; }
			set
			{
				bool invalidate = false;
				if( textStyle != value && text ) invalidate = true;
				textStyle = value;
				if( invalidate )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Gets / sets the orientation of the text.
		/// </summary>
		[Category( "Appearance" )]
		[DefaultValue( Orientation.Horizontal )]
		[Description( "Determines the orientation of the text." )]
		public Orientation TextOrientation
		{
			get
			{
				return textOrientation;
			}
			set
			{
				bool invalidate = false;
				if( textOrientation != value && text ) invalidate = true;
				textOrientation = value;
				if( invalidate )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Indicates whether the text shadow is visible.
		/// </summary>
		[Category( "Appearance" )]
		[DefaultValue( true )]
		[Description( "Determines if the text shadow is visible." )]
		public Boolean TextShadow
		{
			get { return textShadow; }
			set
			{
				textShadow = value;
				if( text )
					InvalidateBar( true );
			}
		}

        /// <summary>
        /// Gets or sets the custom text for the ProgressBarAdv.
        /// </summary>
        /// <value>The custom text.</value>
        /// <remarks>
        /// This value represents the CustomText of the ProgessBarAdv. The customText is returned only if we set the ProgressBarTextStyles.Custom 
        /// </remarks>
        [Category("Appearance")]
        [Description("Determines the Custom Text for setting the center Text for the ProgressBarAdv instead of percentage and values. This will be applied only when we set the ProgressBarTextStyles as Custom")]
        public string CustomText
        {
            get
            {
                return customtext;
            }

            set
            {
                customtext = value;
				this.Refresh();
            }
        }

		/// <summary>
		/// Gets / sets the style of the foreground:
		///  -Constant
		///	 -Gradient
		///  -Multiple gradient
		///  -Tube
		///  -Image
		///	 -System
		/// </summary>
		/// <remarks>
		/// By default, its value is constant.
		/// </remarks>
		[Category( "Appearance" )]
		[DefaultValue( ProgressBarStyles.Constant )]
		[Description( "Determines the foreground drawing style." )]
		public ProgressBarStyles ProgressStyle
		{
			get { return progressStyle; }
			set
			{
				bool invalidate = false;
				if( progressStyle != value ) invalidate = true;
				progressStyle = value;
                if (ProgressStyle == ProgressBarStyles.Metro)
                {
                    this.BorderStyle = prev_borderStyle;
                    foreStartColor = prev_foreStartColor;
                    foreEndColor = prev_foreEndColor;
                    borderColor = prev_borderColor;
                    BackgroundStyle = ProgressBarBackgroundStyles.Gradient;
                }
				RefreshBrushes();
				if( invalidate )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Gets / sets the style of the foreground when ProgressStyle is System and the system can not support themes.
		/// </summary>
		[Category( "Appearance" )]
		[DefaultValue( ProgressBarStyles.Constant )]
		[Description( "Determines the foreground drawing style if System is selected and the themes are not supported by the machine." )]
		public ProgressBarStyles ProgressFallbackStyle
		{
			get { return progressFallbackStyle; }
			set
			{
				if( progressFallbackStyle != value )
				{
					progressFallbackStyle = value;
					this.RefreshBrushes();
					this.InvalidateBar();
				}
			}
		}

		/// <summary>
		/// Gets / sets the style of the border when BorderStyle is Fixed3D.
		/// </summary>
		/// <remarks>
		/// By default, its value is Sunken.
		/// </remarks>
		[Category( "Appearance" )]
		[DefaultValue( Border3DStyle.Sunken )]
		[Description( "Determines the style of the 3Dborder." )]
		public Border3DStyle Border3DStyle
		{
			get { return border3DStyle; }
			set
			{
				bool invalidate = false;
				if( border3DStyle != value && borderStyle == BorderStyle.Fixed3D ) invalidate = true;
				border3DStyle = value;
				if( invalidate )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Gets / sets the style of the border. It can be None, 3D or 2D.
		/// </summary>
		/// <remarks>
		/// By default, its value is Fixed3D.
		/// </remarks>
        [Category("Appearance")]
        [DefaultValue(BorderStyle.Fixed3D)]
        [Description("Determines the style of the border.")]
        public BorderStyle BorderStyle
        {
            get { return borderStyle; }
            set
            {
                bool invalidate = false;
                if (borderStyle != value) invalidate = true;
                borderStyle = value;
                prev_borderStyle = value;
                RefreshBrushes();
                if (invalidate)
                    InvalidateBar(true);
            }
        }
		/// <summary>
		/// Serialize the borderstyle
		/// </summary>
        protected bool ShouldSerializeBorderStyle()
        {
            if (this.borderStyle == System.Windows.Forms.BorderStyle.FixedSingle)
                return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetBorderStyle()
        {
            this.borderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        }
		/// <summary>
		/// Gets / sets the style of the border when BorderStyles is FixedSingle.
		/// </summary>
		/// <remarks>
		/// By default, its value is Solid.
		/// </remarks>
		[Category( "Appearance" )]
		[DefaultValue( ButtonBorderStyle.Solid )]
		[Description( "Determines the style of the thin border." )]
		public ButtonBorderStyle BorderSingle
		{
			get { return borderSingle; }
			set
			{
				bool invalidate = false;
				if( borderSingle != value && borderStyle == BorderStyle.FixedSingle ) invalidate = true;
				borderSingle = value;
				if( invalidate )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Gets / sets the color of the border when BorderStyles is FixedSingle.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Determines the color of the thin border." )]
		public Color BorderColor
		{
			get { return borderColor; }
			set
			{
				bool invalidate = false;
				if( borderColor != value && borderStyle == BorderStyle.FixedSingle ) invalidate = true;
				borderColor = value;
                prev_borderColor = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar( true );
			}
		}
        /// <summary>
        /// Serializes the bordercolor
        /// </summary>
        protected bool ShouldSerializeBorderColor()
        {
            if (this.borderColor == Color.Black)
                return false;
            return true;
        }

        /// <summary>
        /// Resets the bordercolor
        /// </summary>
        public void ResetBorderColor()
        {
            this.borderColor = Color.FromArgb(19, 158, 214);
        }
		/// <summary>
		/// Gets / sets the style of the background. It can have the following values:
		/// -Image
		///	-Gradient
		/// -Vertical gradient
		/// -Tube
		///	-Multiple gradient
		///	-System
		///	-None
		/// </summary>
		/// <remarks>
		/// By default, its value is None.
		/// </remarks>
		[Category( "Appearance" )]
		[DefaultValue( ProgressBarBackgroundStyles.None )]
		[Description( "Determines the background style." )]
		public ProgressBarBackgroundStyles BackgroundStyle
		{
			get { return backgroundStyle; }
			set
			{
				bool invalidate = false;
				if( backgroundStyle != value ) invalidate = true;
				backgroundStyle = value;
				RefreshBrushes();
				if( invalidate )
					InvalidateBar( true );
			}
		}

		/// <summary>
		/// Gets / sets the style of the background when BackgroundStyle is set to System and the system cannot support themes.
		/// </summary>
		/// <remarks>
		/// By default, its value is None.
		/// </remarks>
		[Category( "Appearance" )]
		[DefaultValue( ProgressBarBackgroundStyles.None )]
		[Description( "Determines the background style when System mode is selected and the machine doesn't support Themes." )]
		public ProgressBarBackgroundStyles BackgroundFallbackStyle
		{
			get { return backgroundFallbackStyle; }
			set
			{
				backgroundFallbackStyle = value;
			}
		}

		/// <summary>
		/// Gets / sets the horizontal or vertical style of the progress bar.
		/// </summary>
		/// <remarks>
		/// By default, its value is Horizontal.
		/// </remarks>
		[Category( "Appearance" )]
		[DefaultValue( Orientation.Horizontal )]
		[Description( "Determines the ProgressBar orientation." )]
		public Orientation ProgressOrientation
		{
			get
			{
				return progressOrientation;
			}
			set
			{
				if( value != progressOrientation )
				{
					int w;
					w = Width;
					Width = Height;
					Height = w;
					progressOrientation = value;
					RefreshBrushes();
					InvalidateBar( true );
				}
			}
		}
		#endregion

		#region Drawing methods
		private void RefreshWaitingGradientBrush()
		{
			try
			{
				this.foreWaitingGradientBrush = new LinearGradientBrush(
					new Point( this.waitingGradientRect.X, 0 ),
					new Point( this.waitingGradientRect.X +/*(Vertical()?Height-2*offset:*/this.WaitingGradientWidth/*)*/, 0 ),
					this.foreStartColor,
					this.foreEndColor );

				ColorBlend cb = new ColorBlend( 3 );
				cb.Colors = new Color[] { foreStartColor, foreEndColor, foreStartColor };
				cb.Positions = new float[] { 0, ( float )0.5, 1 };
				( ( LinearGradientBrush )this.foreWaitingGradientBrush ).InterpolationColors = cb;
			}
			catch
			{
				this.foreWaitingGradientBrush = new SolidBrush( this.foreStartColor );
			}
		}
		/// <summary>
		/// Recreates the brushes used to draw the ProgressBar.
		/// </summary>
		public void RefreshBrushes()
		{
			if( inInit || this.Height <= 0 || this.Width <= 0 ) return;
			this.vbackGradBrush = new LinearGradientBrush(
				new Point( GetLeft(), GetTop() ),
				new Point( GetLeft(), GetTop() + ( Vertical() ? Width : Height ) ),
				backStartColor,
				backEndColor );

			this.backFillBrush = new SolidBrush( BackColor );

			this.foreFillBrush = new SolidBrush( this.foregroundColor );

			this.foreGradientBrush = new LinearGradientBrush(
				new Point( GetLeft(), GetTop() ),
				new Point( GetLeft() + ( Vertical() ? Height : Width ), GetTop() ),
				this.foreStartColor,
				this.foreEndColor );

			RefreshWaitingGradientBrush();

			this.backGradientBrush = new LinearGradientBrush(
				new Point( GetLeft(), GetTop() ),
				new Point( GetLeft() + ( Vertical() ? Width : 0 ), GetTop() + ( Vertical() ? 0 : Height ) ),
				this.backStartColor,
				this.backEndColor );

			this.foreTubeBrush = new LinearGradientBrush(
				new Point( GetLeft(), GetTop() ),
				new Point( GetLeft(), GetTop() + ( Vertical() ? Width : Height ) ),
				this.tubeStartColor,
				this.tubeEndColor );

			ColorBlend cb = new ColorBlend( 3 );
			cb.Colors = new Color[] { this.tubeEndColor, this.tubeStartColor, this.tubeEndColor };
			cb.Positions = new float[] { 0, ( float )0.5, 1 };

			( ( LinearGradientBrush )this.foreTubeBrush ).InterpolationColors = cb;

			this.backTubeBrush = new LinearGradientBrush(
				new Point( GetLeft(), GetTop() ),
				new Point( GetLeft(), GetTop() + ( Vertical() ? Width : Height ) ),
				this.backTubeStartColor,
				this.backTubeEndColor );

			ColorBlend cb2 = new ColorBlend( 3 );
			cb2.Colors = new Color[] { this.backTubeEndColor, this.backTubeStartColor, this.backTubeEndColor };
			cb2.Positions = new float[] { 0, ( float )0.5, 1 };

			( ( LinearGradientBrush )this.backTubeBrush ).InterpolationColors = cb2;
			/*
						Point[] fpoints;

							fpoints = new Point[]{
												 new Point(offset,offset),
												 new Point(offset+Width,offset),
												 new Point(offset+Width,offset+(Height-2*offset)/2),
												 new Point(offset+Width,offset+(Height-2*offset)),
												 new Point(offset,offset+Height-2*offset),
												 new Point(offset,offset+(Height-2*offset)/2)
											 };

						Point[] bpoints;

						bpoints = new Point[]{
												 new Point(offset,offset),
												 new Point(offset+Width,offset),
												 new Point(offset+Width,offset+(Height)/2),
												 new Point(offset+Width,offset+(Height)),
												 new Point(offset,offset+Height),
												 new Point(offset,offset+(Height)/2)
											 };

						GraphicsPath path = new GraphicsPath();
						path.AddLines(fpoints);
						foreTubeBrush = new PathGradientBrush(path);

						path.Reset();
						path.AddLines(bpoints);
						backTubeBrush = new PathGradientBrush(path);
						foreTubeBrush.CenterColor = tubeStartColor;
						Color[] colors = {
											 tubeEndColor,
											 tubeEndColor,
											 tubeStartColor,
											 tubeEndColor,
											 tubeEndColor,
											 tubeStartColor
										};
						foreTubeBrush.SurroundColors = colors;

						backTubeBrush.CenterColor = backTubeStartColor;
						Color[] colors2 = {
											 backTubeEndColor,
											 backTubeEndColor,
											 backTubeStartColor,
											 backTubeEndColor,
											 backTubeEndColor,
											 backTubeStartColor
										 };
						backTubeBrush.SurroundColors = colors2;
						*/
		}

		/// <summary>
		/// Draws the background of the ProgressBar.
		/// </summary>
		/// <param name="g">The <see cref="Graphics"/> object to draw on.</param>
		/// <param name="segmented">Indicates whether the background is segmented.</param>
		private void DrawBackground( Graphics g, bool segmented )
		{
			if(backgroundStyle == ProgressBarBackgroundStyles.System)
			{
				if(XPThemes.IsThemedOS && XPThemes.IsAppThemed && XPThemes.IsThemeActive && ThemesEnabled)
				{
					if(!Vertical())
					{
						if (GetIsMirrored())
						{
							// workaround for flipping ...
							Rectangle destRect = GetClientRectangle();
							Rectangle rectangle = destRect;
							rectangle.Offset(-rectangle.X, -rectangle.Y);
							Bitmap bm = new Bitmap(destRect.Width, destRect.Height);
							Graphics bmg = Graphics.FromImage(bm);
							tc.DrawThemeBackground(bmg,1,1,rectangle);
							bmg.Dispose();
							g.DrawImageUnscaled(bm, destRect);
						}
						else
						{
							tc.DrawThemeBackground(g,1,1,GetClientRectangle());
						}
					}
					else
					{
						g.ResetTransform();
						tc.DrawThemeBackground(g,2,1,GetClientRectangle());
						g.TranslateTransform(0,GetClientSize().Height);
						g.RotateTransform(-90);
					}
				}
				else
				{
					BackgroundStyle = BackgroundFallbackStyle;
				}
			}
			if(backgroundStyle == ProgressBarBackgroundStyles.Gradient)
			{
				DrawGradient(g,GetClientRectangle(),backSegments,false);
			}
			if(backgroundStyle == ProgressBarBackgroundStyles.VerticalGradient)
			{
				if(!segmented)
				{
					Rectangle rect = new Rectangle( GetLeft(),GetTop(),Vertical()?Height:Width,Vertical()?Width:Height );
					g.FillRectangle(this.vbackGradBrush,rect);
				}
				else
				{
                    using(Brush brush=new SolidBrush(Color.FromKnownColor(KnownColor.Control)))
                        g.FillRectangle(brush, GetLeft(), GetTop(), Vertical() ? Height : Width, Vertical() ? Width : Height);
					g.FillRegion(this.vbackGradBrush,GetSegmentRegion(new Rectangle(GetLeft()+offset,GetTop()+offset,Vertical()?Height:Width,Vertical()?Width:Height),true));
				}


			}
			if(backgroundStyle == ProgressBarBackgroundStyles.Tube)
			{
				DrawTube(g,new Rectangle(GetLeft(),GetTop(),Vertical()?Height:Width,Vertical()?Width:Height),backSegments,false);
			}
			if(backgroundStyle == ProgressBarBackgroundStyles.MultipleGradient)
			{
				DrawMultipleGradient(g,GetClientRectangle(),backColors,backSegments,false);
			}
			if(backgroundStyle == ProgressBarBackgroundStyles.None)
			{
				g.FillRectangle(backFillBrush,GetLeft(),GetTop(),Vertical()?Height:Width,Vertical()?Width:Height);
			}

		}
		/// <summary>
		/// Draws the foreground of the ProgressBar.
		/// </summary>
		/// <param name="g">The <see cref="Graphics"/> object to draw on.</param>
		/// <param name="rect">The <see cref="Rectangle"/> to draw in.</param>
		private void DrawProgress( Graphics g, Rectangle rect )
		{
			//			g.SetClip(rect);
			if( progressStyle == ProgressBarStyles.WaitingGradient )
				DrawWaitingGradient( g );
			if( ( rect.Width <= 0 ) || ( rect.Height <= 0 ) ) return;

			if( progressStyle == ProgressBarStyles.System )
			{
				if( XPThemes.IsThemedOS && XPThemes.IsAppThemed && XPThemes.IsThemeActive && ThemesEnabled )
				{
					DrawSystemSegments( g, rect );
				}
				else
				{
					ProgressStyle = ProgressFallbackStyle;
				}
			}
			if( progressStyle == ProgressBarStyles.Constant )
				DrawFillColor( g, rect, foreSegments, true );
			if( progressStyle == ProgressBarStyles.Gradient )
				DrawGradient( g, rect, foreSegments, true );
			if( progressStyle == ProgressBarStyles.Image )
				DrawImage( g, rect, foregroundImage );
			if( progressStyle == ProgressBarStyles.Tube )
				DrawTube( g, rect, foreSegments, true );
			if( progressStyle == ProgressBarStyles.MultipleGradient )
				DrawMultipleGradient( g, rect, foreColors, foreSegments, true );
			if (progressStyle == ProgressBarStyles.Metro)
			{
				DrawGradient(g, rect, false, true);
			}
			//			g.ResetClip();
		}

		/// <summary>
		/// Draws the border of the ProgressBar.
		/// </summary>
		/// <param name="g"><see cref="Graphics"/></param>
		/// <param name="rc">The <see cref="Rectangle"/> of the border.</param>
		private void DrawBorder( Graphics g, Rectangle rc )
		{
			if( borderStyle == BorderStyle.Fixed3D )
			{
				ControlPaint.DrawBorder3D( g, rc, border3DStyle );
			}
			if( borderStyle == BorderStyle.FixedSingle )
			{
				ControlPaint.DrawBorder( g, rc, borderColor, borderSingle );
			}
		}

		private void DrawFillColor( Graphics g, Rectangle rc, bool fore )
		{
			g.FillRectangle( fore ? foreFillBrush : backFillBrush, rc );
		}

		/// <summary>
		/// Draws the fill color of the ProgressBar when continuous is selected.
		/// </summary>
		private void DrawFillColor( Graphics g, Rectangle rc, bool segmented, bool fore )
		{
			if( !segmented )
			{
				g.FillRectangle( fore ? foreFillBrush : backFillBrush, new Rectangle( GetLeft() + offset, GetTop() + offset, rc.Width, rc.Height ) );
			}
			else
			{
				g.FillRegion( fore ? foreFillBrush : backFillBrush, GetSegmentRegion( rc, true ) );
			}
		}
		/// <summary>
		/// Draws segments in the given rectangle when system is selected.
		/// </summary>
		/// <param name="g"><see cref="Graphics"/></param>
		/// <param name="rc">The <see cref="Rectangle"/> to draw the segments in.</param>
		private void DrawSystemSegments( Graphics g, Rectangle rc )
		{
			int segwidth = 8;
			if( !Vertical() )
			{
				int Count = ( ( rc.Width ) / ( segwidth + 2 ) ) + ( ( rc.Width <= 0 ) ? 0 : 1 );

				Rectangle rect = new Rectangle( GetLeft() + offset, GetTop() + offset, Count * ( segwidth + 2 ), rc.Height - offset );
				if( rect.Width > ( Width - 2 * offset ) )
				{
					rect.Width = rc.Width;
				}

				if( GetIsMirrored() )
				{
					// workaround for flipping ...

					Rectangle destRect = rect;
					Rectangle rectangle = destRect;
					rectangle.Offset( -rectangle.X, -rectangle.Y );
					Bitmap bm = new Bitmap( destRect.Width, destRect.Height );
					Graphics bmg = Graphics.FromImage( bm );
					tc.DrawThemeBackground( bmg, 3, 1, rectangle );
					bmg.Dispose();
					g.DrawImageUnscaled( bm, destRect );
				}
				else
				{
					tc.DrawThemeBackground( g, 3, 1, rect );
				}
			}
			else
			{
				g.ResetTransform();

				int Count = ( ( rc.Width ) / ( segwidth + 2 ) ) + ( ( rc.Width <= 0 ) ? 0 : 1 );

				Rectangle rect = new Rectangle( GetLeft() + offset, GetTop() + Height - Count * ( segwidth + 2 ) - offset, rc.Height - offset, Count * ( segwidth + 2 ) );


				if( rect.Height > ( Height - 2 * offset ) )
				{
					Rectangle rect2 = new Rectangle( GetLeft() + offset, GetTop() + Height - ( Count - 1 ) * ( segwidth + 2 ) - offset, rc.Height - offset, ( Count - 1 ) * ( segwidth + 2 ) );
					tc.DrawThemeBackground( g, 4, 1, rect2 );
					int hl = ( Height - rect2.Height - offset * 2 );
					tc.DrawThemeBackground( g, 4, 1, new Rectangle( GetLeft() + offset, GetTop() + offset, rc.Height - offset, hl ) );
				}
				else
				{
					tc.DrawThemeBackground( g, 4, 1, rect );
				}
				g.TranslateTransform( 0, GetClientSize().Height );
				g.RotateTransform( -90 );
			}
		}

		/// <summary>
		/// Draws the dual gradient of the ProgressBar when gradient is selected.
		/// </summary>
		private void DrawGradient( Graphics g, Rectangle rc,bool segmented,bool fore )
		{
			if( !segmented )
			{
                g.FillRectangle(fore ? foreGradientBrush : backGradientBrush, new Rectangle(GetLeft() + offset, GetTop() + offset, rc.Width - offset, rc.Height - offset));
			}
			else
			{
				g.FillRegion( fore ? foreGradientBrush : backGradientBrush, GetSegmentRegion( rc, true ) );
			}
		}
		/// <summary>
		/// Draws the image of the ProgressBar when image is selected.
		/// </summary>
		/// <param name="g"><see cref="Graphics"/></param>
		/// <param name="rc">The <see cref="Rectangle"/> to draw the image in.</param>
		/// <param name="img">The <see cref="Image"/> used to draw in the rectangle.</param>
		private void DrawImage( Graphics g, Rectangle rc,Image img )
		{
			if( img == null ) return;
			Image image = img;
			int width = image.Width;
			int height = image.Height;

			if( stretchImage )
			{
				g.DrawImage( image, new Rectangle( GetLeft() + offset, GetTop() + offset, rc.Width, rc.Height ), 0, 0, width, height, GraphicsUnit.Pixel );
			}
			else
			{
				int Count = rc.Width / width;
				Rectangle destRect = new Rectangle( GetLeft() + offset, GetTop() + offset, width, rc.Height - offset );
				for( int i = 0; i < Count; i++ )
				{
					g.DrawImage( image, destRect, 0, 0, width, rc.Height, GraphicsUnit.Pixel );
					destRect.Offset( width, 0 );
				}
				destRect.Width = rc.Width - Count * width;
				g.DrawImage( image, destRect, 0, 0, rc.Width - Count * width, rc.Height, GraphicsUnit.Pixel );
			}
		}

		/// <summary>
		/// Draws the dual tube of the ProgressBar when tube is selected.
		/// </summary>
		private void DrawTube( Graphics g, Rectangle rc,bool segmented,bool fore )
		{
			Brush brush = fore ? foreTubeBrush : backTubeBrush;

			if( !segmented )
			{
				Rectangle rect = new Rectangle( GetLeft() + offset, GetTop() + offset, rc.Width, rc.Height );
				rect.Intersect( GetClipRect( true ) );
				g.FillRectangle( brush, rect );
			}
			else
			{
				g.FillRegion( brush, GetSegmentRegion( rc, true ) );
			}
		}

		/// <summary>
		/// Draws the multiple gradient of the ProgressBar when multiple gradient is selected.
		/// </summary>
		private void DrawMultipleGradient( Graphics g, Rectangle rc,ArrayList foreColors,bool segmented,bool fore )
		{
			if( foreColors.Count == 0 ) return;
			if( foreColors.Count == 1 )
            {
                using (Brush brush = new SolidBrush((Color)foreColors[0]))
                    g.FillRectangle(brush, rc);
				return;
			}

			Bitmap bmp;
			if( stretchMultGrad )
			{
				bmp = new Bitmap( rc.Width, rc.Height, g );
			}
			else
			{
				bmp = new Bitmap( Vertical() ? Height : Width - 2 * offset, rc.Height, g );
			}
			Graphics gr = Graphics.FromImage( bmp );

			LinearGradientBrush linGrBrush = new LinearGradientBrush(
				new Point( 0, offset ),
				new Point( bmp.Width, offset ),
				Color.Black,
				Color.Black );

			ColorBlend cb = new ColorBlend( foreColors.Count );
			for( int i = 0; i < foreColors.Count; i++ )
			{
				cb.Colors.SetValue( foreColors[ i ], i );
				cb.Positions.SetValue( ( float )i / ( foreColors.Count - 1 ), i );
			}

			linGrBrush.InterpolationColors = cb;

			if( !segmented )
			{
				gr.FillRectangle( linGrBrush, new Rectangle( GetLeft(), GetTop(), bmp.Width, rc.Height ) );
				g.DrawImage( bmp, new Rectangle( GetLeft() + offset, GetTop() + offset, rc.Width, rc.Height - offset ), 0, 0, rc.Width, rc.Height, GraphicsUnit.Pixel );
			}
			else
			{
				int Count = ( ( rc.Width ) / ( segmentwidth + 2 ) ) + ( ( rc.Width <= 0 ) ? 0 : 1 );
				gr.FillRegion( linGrBrush, GetSegmentRegion( rc, false ) );
				g.DrawImage( bmp, new Rectangle( GetLeft() + offset, GetTop() + offset, Count * ( segmentwidth + 2 ), rc.Height - offset ), 0, 0, Count * ( segmentwidth + 2 ), rc.Height, GraphicsUnit.Pixel );
			}
            linGrBrush.Dispose();
		}
		private void DrawWaitingGradient( Graphics g )
		{
			if( !Vertical() )
			{
				g.SetClip( new Rectangle( GetLeft() + offset, GetTop() + offset, Width - offset * 2, Height - offset * 2 ) );
			}
			else
			{
				g.SetClip( new Rectangle( GetLeft() + offset, GetTop() + offset, Height - 2 * offset, Width - 2 * offset ) );
			}
			Rectangle rc = new Rectangle( GetLeft() + this.waitingGradientRect.X,
				GetTop() + offset,
				this.waitingGradientRect.Width,
				( Vertical() ? Width : Height ) - 2 * offset );
			if( this.CustomWaitingRender )
			{
				ProgressBarAdvDrawEventArgs e = new ProgressBarAdvDrawEventArgs( g, rc );
				OnDrawWaitingCustomRender( e );
				if( !e.Handled )
				{
					Rectangle small = new Rectangle( rc.Left, rc.Top, this.segmentwidth, rc.Height );
					int count = ( int )( ( float )this.WaitingGradientWidth / ( ( float )this.segmentwidth + 2 ) + 1 );
					Region rgn = new Region();
					rgn.MakeEmpty();
					for( int i = 0; i < count; i++ )
					{
						rgn.Union( small );
						small.Offset( segmentwidth + 2, 0 );
					}

					switch( this.ProgressFallbackStyle )
					{
						case ProgressBarStyles.Constant: g.FillRegion( foreFillBrush, rgn ); break;
						case ProgressBarStyles.Gradient: g.FillRegion( foreGradientBrush, rgn ); break;
						case ProgressBarStyles.System:
							{

								small = new Rectangle( rc.Left, rc.Top, this.segmentwidth, rc.Height );
								for( int i = 0; i < count; i++ )
								{
									DrawWaitingSegment( g, small );
									small.Offset( segmentwidth + 2, 0 );
								}
								break;
							}
						case ProgressBarStyles.Tube: g.FillRegion( this.foreTubeBrush, rgn ); break;
						default:
							{
								small = new Rectangle( rc.Left, rc.Top, this.segmentwidth, rc.Height );
								for( int i = 0; i < count; i++ )
								{
									DrawWaitingSegment( g, small );
									small.Offset( segmentwidth + 2, 0 );
								}
								break;
							}
					}
                    rgn.Dispose();
				}
			}
			else
			{
				g.FillRectangle( this.foreWaitingGradientBrush, rc );
			}
			g.ResetClip();
		}

		private void DrawWaitingSegment( Graphics g, Rectangle small )
		{
			if( null != tc )
			{
				tc.DrawThemeBackground( g, 3, 1, small );
			}
			else
			{
				DrawFillColor( g, small, true );
			}
		}

		#endregion
		/// <summary>
		/// Returns the region of the segments if segment mode is selected.
		/// </summary>
		/// <param name="rc"><see cref="Rectangle"/>The rectangle in which the segments are situated in.</param>
		/// <param name="isoffset">Indicates the segments are offset.</param>
		/// <returns><see cref="Region"/>The region containing the segments.</returns>
		private Region GetSegmentRegion( Rectangle rc, bool isoffset )
		{
			Region rgn = new Region();
			rgn.MakeEmpty();

			int Count = ( ( rc.Width ) / ( segmentwidth + 2 ) ) + ( ( rc.Width <= 0 ) ? 0 : 1 );
			Rectangle Segrc = new Rectangle( GetLeft() + ( isoffset ? offset : 0 ), GetTop() + ( isoffset ? offset : 0 ), segmentwidth, rc.Height );

			for( int i = 0; i < Count; i++ )
			{
				rgn.Union( Segrc );
				Segrc.Offset( segmentwidth + 2, 0 );
			}

			Rectangle clipRectangle = GetClipRect( isoffset ); ;
			rgn.Intersect( clipRectangle );

			return rgn;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="isoffset"></param>
		/// <returns></returns>
		private Rectangle GetClipRect( bool isoffset )
		{
			Rectangle clipRectangle;
			if( !Vertical() )
			{
				clipRectangle = new Rectangle( GetLeft(), GetTop(), Width, Height );
			}
			else
			{
				clipRectangle = new Rectangle( GetLeft(), GetTop(), Height, Width );
			}
			if( isoffset )
			{
				clipRectangle.Offset( offset, offset );
				clipRectangle.Width -= 2 * offset;
				clipRectangle.Height -= 2 * offset;
			}
			return clipRectangle;
		}
		/// <summary>
		/// Indicates whether the ProgressBar is vertical.
		/// </summary>
		/// <returns>
		/// </returns>
		private bool Vertical()
		{
			if( progressOrientation == Orientation.Vertical )
			{
				return true;
			}
			return false;
		}
		private void OnResize( object sender, System.EventArgs e )
		{
			RefreshBrushes();
			InvalidateBar( true );
		}
		/// <summary>
		/// Increments the Value property with the Step value.
		/// </summary>
		/// <returns>The success or failure of the Increment. It fails if the incremented value is bigger than the maximum.</returns>
		public bool Increment()
		{
			int val = Value;
			Value += step;
			return ( Value == ( val + step ) );
		}

		/// <summary>
		/// Decrements the Value property with the Step value.
		/// </summary>
		/// <returns>The success or failure of the Increment. It fails if the incremented value is smaller than minimum.</returns>
		public bool Decrement()
		{
			int val = Value;
			Value -= step;
			return ( Value == ( val - step ) );
		}

		private void timer_Tick( object sender, System.EventArgs e )
		{
			this.waitingGradientRect.Offset( step, 0 );
			if( waitingGradientRect.X > ( Vertical() ? Height : Width ) ) waitingGradientRect.X = -waitingGradientRect.Width;
			this.RefreshWaitingGradientBrush();
			InvalidateBar();
		}
	}

	public delegate void ProgressBarAdvDrawEventHandler( object sender, ProgressBarAdvDrawEventArgs e );

	public class ProgressBarAdvDrawEventArgs : EventArgs
	{
		private Rectangle rect;
		private Graphics graphics;
		private bool handled = false;

		/// <summary>
		/// Gets or sets a value indicating whether the DrawWaitingCustomRender event was handled.
		/// </summary>
		public bool Handled
		{
			get
			{
				return handled;
			}
			set
			{
				handled = value;
			}
		}
		/// <summary>
		/// Gets the bounding rectangle.
		/// </summary>
		public Rectangle Rectangle
		{
			get
			{
				return rect;
			}
		}

		/// <summary>
		/// Gets the Graphics used to paint.
		/// </summary>
		public Graphics Graphics
		{
			get
			{
				return graphics;
			}
		}
		public ProgressBarAdvDrawEventArgs( Graphics g, Rectangle rect )
		{
			this.graphics = g;
			this.rect = rect;
		}
	}

	/// <summary>
	/// The ProgressBarEx type will soon be replaced with the ProgressBarAdv for consistency in
	/// control naming in our library.
	/// Please replace all occurrences of ProgressBarEx with ProgressBarAdv in your application.
	/// </summary>
	[Obsolete( "The ProgressBarEx type will soon be replaced with the ProgressBarAdv for consistency in naming in our library. Please replace all occurrences of ProgressBarEx with ProgressBarAdv in your app." ),
	ToolboxItem( false )]
	public class ProgressBarEx : ProgressBarAdv
	{
		public ProgressBarEx() : base() { }
	}
    /// <summary>
    /// ProgressBarAdv Designer
    /// </summary>
    public class ProgressBarAdvDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public ProgressBarAdvDesigner()
            : base()
        {
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        /// <summary>
        /// Gets a value indication the designer action
        /// </summary>
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == this.actionLists)
                {
                    this.actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    this.actionLists.Add(new ProgressBarAdvActionLists(this.Component));
                }

                return this.actionLists;
            }
        }

#endif
    }
}
