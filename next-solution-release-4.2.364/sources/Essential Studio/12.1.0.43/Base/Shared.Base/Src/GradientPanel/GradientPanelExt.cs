#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;

using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Grid;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents panel that has border and collection of <see cref="Primitive"/>
	/// that can be drawn on border. Control perhaps has rounded or right corner.
	/// </summary>
	/// <remarks>
	/// <para>For setting rounded corner, assign <see cref="CornerRadius"/> property. 
	/// You can set space between the bounds of the control and border 
	/// using <see cref="BorderGap"/> property. Control has two states: collapse or expand.
	/// For change this state use <see cref="Collapsed"/> property.</para>
	/// <para>Collapse/Expand state can be used with animation. For using animation, you 
	/// must set <see cref="Animated"/> as true. For specifying speed animation 
	/// use <see cref="AnimationSpeed"/>. To specify delay for animation use
	/// <see cref="AnimationDelay"/>.</para>
	/// <para>Control can contain <see cref="Primitive"/>, see <see cref="Primitives"/>.</para>
	/// </remarks>
	[ 
	Designer( typeof( GradientPanelExtDesigner ), typeof(System.ComponentModel.Design.IDesigner) ) 
	]
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.GradientPanelExt.bmp")]
	[Description("Represents collapsible panel that has curved border and primitives collection.")]
	public class GradientPanelExt : GradientPanel, ISupportInitialize
	{
		#region Class Constants

		/// <summary>
		/// Gap between primitive and bounds control.
		/// </summary>
		private const int DEF_PRIMITIVE_CONTROL_GAP = 2;
		
		/// <summary>
		/// Minimum lenght for border where draw primitives.
		/// </summary>
		private const int DEF_MIN_LENGHT_BORDER = 20;

		/// <summary>
		/// Default corner radius.
		/// </summary>
		private const int DEF_CORNER_RADIUS = 20;

		/// <summary>
		/// Gap between border and bounds control.
		/// </summary>
		private const int DEF_BORDER_GAP = 12;

		/// <summary>
		/// Width for pent which draws light line for border.
		/// </summary>
		private const int DEF_WIDTH_PEN = 2;

		/// <summary>
		/// Gap between border and side of the primitive.
		/// </summary>
		private const int DEF_PRIMITIMVE_BORDER_GAP = 3;

		/// <summary>
		/// Offset for border.
		/// </summary>
		private const int DEF_BORDER_OFFSET = 1;

		/// <summary>
		/// Offset for border when control is collapse.
		/// </summary>
		private const int DEF_COLLAPSE_OFFSET = 3;

		/// <summary>
		/// Delay for timer.
		/// </summary>
		private const int DEF_ANIMATION_DELAY = 10;

		/// <summary>
		/// Animation speed.
		/// </summary>
		private const int DEF_ANIMATION_SPEED = 1;

		/// <summary>
		/// Angle degree 0.
		/// </summary>
		private const int DEF_ANGLE_0 = 0;

		/// <summary>
		/// Angle degree 45.
		/// </summary>
		private const int DEF_ANGLE_45 = 45;

		/// <summary>
		/// Angle degree 90;
		/// </summary>
		private const int DEF_ANGLE_90 = 90;

		/// <summary>
		/// Angle degree 180;
		/// </summary>
		private const int DEF_ANGLE_180 = 180;

		/// <summary>
		/// Angle degree 270;
		/// </summary>
		private const int DEF_ANGLE_270 = 270;

		#endregion

		#region Class Members

		/// <summary>
		/// Indicate that control is collapse or expande.
		/// </summary>
		private bool m_bCollapsed = false;
		
		/// <summary>
		/// Radius truncation of the corner.
		/// </summary>
		private int m_cornerRadius = DEF_CORNER_RADIUS;

		/// <summary>
		/// Space between the bound of the control and the border.
		/// </summary>
		private int m_borderGap = DEF_BORDER_GAP;

		/// <summary>
		/// Padding like space between the bounds of the control and the borders.
		/// </summary>
		protected Padding pBorderGap = new Padding(DEF_BORDER_GAP);
		/// <summary>
		/// Background color for control.
		/// </summary>
		private Color m_backColor = SystemColors.Control;

		/// <summary>
		/// Graphics path which represent client area.
		/// </summary>
		private GraphicsPath m_clienPath = null;

		/// <summary>
		/// Graphics path which represent control area.
		/// </summary>
		private GraphicsPath m_controlPath = null;

		/// <summary>
		/// Collection of the Primitives.
		/// </summary>
		private PrimitiveCollection m_arrPrimitive = null;

		/// <summary>
		/// Size of the control for expande state.
		/// </summary>
		private Size m_expandSize = Size.Empty;

		/// <summary>
		/// Location of the control for expande state.
		/// </summary>
		private Point m_expandLocation = Point.Empty;

		/// <summary>
		/// Alignment collapse control.
		/// </summary>
		private Alignment m_enCollapseAlignment = Alignment.Top;
		
		/// <summary>
		/// Last alignment of the control.
		/// </summary>
		private Alignment m_lastAlignment = Alignment.Top;

		/// <summary>
		/// Use for sets correctly location control regarding client path.
		/// </summary>
		private bool m_bPerformingCorrectionsLocation = false;

		/// <summary>
		/// A value indicating whether the control uses animation.
		/// </summary>
		private bool m_bAnimated = false;

		/// <summary>
		/// Collapsed size of the control.
		/// </summary>
		private int CollapseSize;

		/// <summary>
		/// Mimimal controls size.
		/// </summary>
		private Size m_minSize = Size.Empty;

		/// <summary>
		/// Rectangle which contains border.
		/// </summary>
		private Rectangle m_rectangleForBorder = Rectangle.Empty;

		/// <summary>
		/// Uses for animation.
		/// </summary>
		private AnimationHelper m_animator = null;

		/// <summary>
		/// Indicate that control in process of collapse.
		/// Uses by redraw.
		/// </summary>
		private bool m_bCollapsedPerforming = false;

		/// <summary>
		/// Delay for animation.
		/// </summary>
		private int m_animationDelay = DEF_ANIMATION_DELAY;

		/// <summary>
		/// Speed of animation.
		/// </summary>
		private int m_animationSpeed = DEF_ANIMATION_SPEED;

		/// <summary>
		/// Indicate that control need redraw.
		/// Uses by redraw before animation.
		/// </summary>
		private bool m_bIsRedrawCollapsedControl = false;

		private bool m_bInitializePerforming = false;

        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);


		#endregion 

		#region Class Properties

		/// <summary>
		/// Gets or sets delay for animation.
		/// </summary>
		[ 
		DefaultValue( DEF_ANIMATION_DELAY ),
		Description( "Delay for animation." ),
		Category( "Animation" )
		]
		
		public int AnimationDelay
		{
			get
			{
				return m_animationDelay;
			}
			set
			{
				if( value != m_animationDelay )
				{
					m_animationDelay = value;
				}
			}
		}

	
		/// <summary>
		/// Gets or sets speed of animation.
		/// </summary>
		[ 
		DefaultValue( DEF_ANIMATION_SPEED ),
		Description( "Speed of animation." ),
		Category( "Animation" )
		]
		public int AnimationSpeed
		{
			get
			{
				return m_animationSpeed;
			}
			set
			{
				if( value != m_animationSpeed )
				{
					m_animationSpeed = value;
				}
			}
		}


		/// <summary>
		/// Location expanded control. 
		/// When control expanded it is Empty.
		/// </summary>
		[ Browsable( false ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		[ DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ) ]
        [ DefaultValue(typeof(Point), "0, 0") ]
		public Point ExpandLocation
		{
			get
			{
				return m_expandLocation;
			}
			set
			{
				if( value != m_expandLocation )
				{
					m_expandLocation = value;
				}
			}
		}

		
		/// <summary>
		/// Size expanded control. 
		/// When control expanded it is Empty.
		/// </summary>
		[ Browsable(false) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		[ DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ) ]
        [ DefaultValue(typeof(Size), "0, 0") ]
		public Size ExpandSize
		{
			get
			{
				return m_expandSize;
			}
			set
			{
				if( value != m_expandSize )
				{
					m_expandSize = value;
				}
			}
		}

		
		/// <summary>
		/// Gets or sets a value indicating whether the control uses animation.
		/// </summary>
		[ 
		DefaultValue( false ),
		Description( "Indicating whether the control uses animation." ),
		Category( "Animation" )
		]
		public bool Animated
		{
			get
			{
				return m_bAnimated;
			}
			set
			{
				if( value != m_bAnimated )
				{
					m_bAnimated = value;
				}
			}
		}

		
		/// <summary>
		/// Gets or sets state of the control, collapsed or expanded.
		/// </summary>
		[ 
		DefaultValue( false ),
		Description( "Indicating whether the control is collapsed." ),
		Category( "Appearance" )
		]
		public bool Collapsed
		{
			get
			{
				return m_bCollapsed;
			}
			set
			{
				if( value != m_bCollapsed )
				{
					m_bCollapsed = value;
					OnCollapsedChanged();
				}
			}
		}


		/// <summary>
		/// Gets or sets alignment collapsed control.
		/// </summary>
		[ 
		DefaultValue( Alignment.Top ),
		Description( "Alignment for collapsed control." ),
		RefreshProperties( RefreshProperties.All ),
		Category( "Appearance" )
		]
		public Alignment CollapseAlignment
		{
			get
			{
				return m_enCollapseAlignment;
			}
			set
			{
				if( value != m_enCollapseAlignment )
				{
					m_lastAlignment = m_enCollapseAlignment;
					m_enCollapseAlignment = value;
					OnCollapseAlignmentChanged();
				}
			}
		}

	
		/// <summary>
		/// Gets or sets radius truncation corner of the control.
		/// </summary>
		[ 
		DefaultValue( DEF_CORNER_RADIUS ),
		Description( "Radius truncation corner of the control." ),
		RefreshProperties( RefreshProperties.All ),
		Category( "Appearance" )
		]
		public int CornerRadius
		{
			get
			{
				return m_cornerRadius;
			}
			set
			{
				if( value != m_cornerRadius )
				{
					if( this.DesignMode && this.IsHandleCreated )
					{
						int maxRadius = GetMaxRadius();

						if( value > maxRadius || value < 0 )
						{
							int actualValue = ( value > maxRadius ) ? maxRadius : 0;

							throw new ArgumentOutOfRangeException( "CornerRadius", actualValue, "Value must be more than -1 and less than " + maxRadius.ToString() );
						}
					}

					m_cornerRadius = value;
					OnCornerRadiusChanged();
				}
			}
		}
		/// <summary>
		/// Gets or sets border gap. 
		/// </summary>
		[
		DefaultValue(DEF_BORDER_GAP),
		Description("Border gap for each side of the control"),
		Category("Appearance")
		]
		public Padding Border
		{
			get
			{
				return pBorderGap;
			}
			set
			{
				if (CheckPBorderGap(value))
				{
					pBorderGap = value;
					OnBorderGapChanged();
				}
			}
		}

		
		/// <summary>
		/// Gets or sets border gap. 
		/// </summary>
		[ 
		DefaultValue( DEF_BORDER_GAP ),
		Description( "Border gap." ),
		Category( "Appearance" )
		]
		public int BorderGap
		{
			get
			{
				return m_borderGap;
			}
			set
			{
				if( value != m_borderGap )
				{
					if( this.DesignMode && !this.m_bInitializePerforming && this.IsHandleCreated )
					{
						int minBorderGap = GetMinBorderGap();
						int maxBorderGap = GetMaxBorderGap();

						if( value > maxBorderGap || value < minBorderGap )
						{
							int actualValue = ( value > maxBorderGap ) ? maxBorderGap : minBorderGap;

							throw new ArgumentOutOfRangeException( "BorderGap", actualValue, "Value must be more than " + minBorderGap.ToString() + " and less than " + maxBorderGap.ToString() );
						}
					}

					m_borderGap = value;
					this.Border = new Padding(value);
					OnBorderGapChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets graphics path which represent client area.
		/// </summary>
		private GraphicsPath ClientPath
		{
			get
			{
				if( m_clienPath == null )
				{
					m_clienPath = GetClientPath( this.CollapseAlignment, this.Collapsed );
				}

				return m_clienPath;
			}
			set
			{
				if( value != m_clienPath )
				{
					m_clienPath = value;
				}
			}
		}

		
		/// <summary>
		/// Gets or sets graphics path which represent control area.
		/// </summary>
		private GraphicsPath ControlPath
		{
			get
			{
				return m_controlPath;
			}
			set
			{
				if( value != m_controlPath )
				{
					m_controlPath = value;
				}
			}
		}

		
		/// <summary>
		/// Gets collection of the primitives.
		/// </summary>
		[ 
		Description( "Collection of the primitives." ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Content )
		]
		public PrimitiveCollection Primitives
		{
			get
			{
				return m_arrPrimitive;
			}
			set
			{
				if( value != m_arrPrimitive )
				{
					m_arrPrimitive = value;
										
					OnPrimitivesChanged();
				}
			}
		}

		
		/// <summary>
		/// Gets bounds for expanded control.
		/// </summary>
		private Rectangle RealRectangle
		{
			get
			{
				return new Rectangle( this.RealLocation, this.RealSize );
			}
		}
		

		/// <summary>
		/// Gets size for expanded control.
		/// </summary>
		private Size RealSize
		{
			get
			{
				Size size = Size.Empty;

				if( this.ExpandSize != Size.Empty )
				{
					size = this.ExpandSize;
				}
				else
				{
					size = this.Size;
				}

				return size;
			}
		}


		/// <summary>
		/// Get location for expanded control.
		/// </summary>
		private Point RealLocation
		{
			get
			{
				Point location = Point.Empty;

				if( this.ExpandLocation != Point.Empty )
				{
					location = this.ExpandLocation;
				}
				else
				{
					location = this.Location;
				}

				return location;
			}
		}
 
		
		[ Browsable( false ) ]
		[ EditorBrowsable(EditorBrowsableState.Never ) ]
		public new Border3DStyle Border3DStyle
		{
			get
			{
				return base.Border3DStyle;
			}
			set
			{
				base.Border3DStyle = value;
			}
		}

		[ Browsable( false ) ]
		[ EditorBrowsable(EditorBrowsableState.Never ) ]
		public new Border3DSide BorderSides
		{
			get
			{
				return base.BorderSides;
			}
			set
			{
				base.BorderSides = value;
			}
		}

		
		[ Browsable( false ) ]
		[ EditorBrowsable(EditorBrowsableState.Never ) ]
		public new ButtonBorderStyle BorderSingle
		{
			get
			{
				return base.BorderSingle;
			}
			set
			{
				base.BorderSingle = value;
			}
		}

		
		[ Browsable( false ) ]
		[ EditorBrowsable(EditorBrowsableState.Never ) ]
		public new bool ThemesEnabled
		{
			get
			{
				return base.ThemesEnabled;
			}
			set
			{
				base.ThemesEnabled = value;
			}
		}

		
		[ Browsable( false ) ]
		[ EditorBrowsable(EditorBrowsableState.Never ) ]
		public new bool IgnoreThemeBackground
		{
			get
			{
				return base.IgnoreThemeBackground;
			}
			set
			{
				base.IgnoreThemeBackground = value;
			}
		}


		#endregion

		#region Class Initialize/Finalize Method

		public GradientPanelExt()
		{
			InitializeControlStyle();
			InitializeControlColor();
			InitializePrimitives();
			InitializemAnimator();

			SetCorrectRadius( this.CollapseAlignment, this.Collapsed );
			SetCorrectBorderGap( this.CollapseAlignment, this.Collapsed );
			RefreshSizes();
			RefreshPath();
			RefreshPrimitives();
            CTRLSIZE = this.Size;
		}

		
		/// <summary>
		/// Initialize animation.
		/// </summary>
		private void InitializemAnimator()
		{
			this.m_animator = new AnimationHelper();
			this.m_animator.AnimationPositionChanged += new EventHandler( Animator_AnimationPositionChanged );
			this.m_animator.AnimationDone += new EventHandler( Animator_AnimationDone);
		}

		
		/// <summary>
		/// Initialize control's style.
		/// </summary>
		private void InitializeControlStyle()
		{
			this.SetStyle( ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
				ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true );
		}

		
		/// <summary>
		/// Initialize control's colors.
		/// </summary>
		private void InitializeControlColor()
		{
			base.BackColor = System.Drawing.Color.Transparent;
			this.BorderStyle = System.Windows.Forms.BorderStyle.None;
		}

		/// <summary>
		/// Initialize primitives.
		/// </summary>
		private void InitializePrimitives()
		{
			m_arrPrimitive = new PrimitiveCollection();
			this.Primitives.CollectionChanged += new CollectionChangeEventHandler( Primitives_CollectionChanged );
		}
		
		#endregion

        #region For Touch

        bool isScaling = false;

        bool _touchMode = false;
        /// <summary>
        ///Gets or Sets the touchmode
        /// </summary>
        [DefaultValue(false)]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        ///Applies the scaling
        /// </summary>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            foreach (Control ctrl in this.Controls)
            {
                PropertyInfo fi = ctrl.GetType().GetProperty("EnableTouchMode");//,
                fi.SetValue(ctrl, this.EnableTouchMode,null);
            }
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        ///Font Chnaged
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        #endregion

		#region Class Overrides

		protected override void SetBoundsCore(int x, int y, int width, int height, 
			BoundsSpecified specified)
		{
			Rectangle rect  = new Rectangle( x, y, width, height );

			Rectangle correctRect = Rectangle.Empty;

			// gets correct size of the control
			if( this.m_animator != null && !this.m_animator.AnimationOn )
			{
				correctRect = GetCorrectSize( rect, this.CollapseAlignment, this.Collapsed, specified );
			}
			else
			{
				correctRect = rect;
			}

			base.SetBoundsCore( correctRect.X, correctRect.Y, correctRect.Width, correctRect.Height, 
				specified );
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			Region saveClip = e.Graphics.Clip;
			using(Region region =new Region( this.ControlPath ))
				e.Graphics.Clip = region;
			 
			base.OnPaint( e );

			e.Graphics.Clip = saveClip;
			
			// draw control
			DrawControl( e.Graphics );

			// draw border within control
			DrawBorder( e.Graphics );
		}

		protected override void OnPaintBackground( PaintEventArgs pevent )
		{
			PaintTransparentBackground( pevent, this.ClientRectangle );

			Region saveClip = pevent.Graphics.Clip;
			using (Region region = new Region(this.ControlPath))
				pevent.Graphics.Clip = region;

			if ( ( this.BackgroundImage != null ) && !SystemInformation.HighContrast )
			{
				TextureBrush brush = null;
				switch (this.BackgroundImageLayout)
				{
					case ImageLayout.Tile:
						brush = new TextureBrush(this.BackgroundImage, WrapMode.Tile);
						break;
					default:
						brush = new TextureBrush(this.BackgroundImage, WrapMode.Clamp);
						break;
				}

				try
				{
					Matrix matrix = brush.Transform;

					if (this.BackgroundImageLayout == ImageLayout.Center)
					{
						matrix.Translate((float)(this.DisplayRectangle.X + (this.DisplayRectangle.Width - this.BackgroundImage.Width) / 2),
							(float)(this.DisplayRectangle.Y + (this.DisplayRectangle.Height - this.BackgroundImage.Height) / 2));
					}
					else
					{
						if (this.BackgroundImageLayout == ImageLayout.Stretch)
						{
							pevent.Graphics.DrawImage(this.BackgroundImage, this.DisplayRectangle);
							return;
						}
						else if (this.BackgroundImageLayout == ImageLayout.Zoom)
						{
							// the lesser value - either Width or Height will applied as the new size
							int newsize = this.DisplayRectangle.Width < this.DisplayRectangle.Height ? this.DisplayRectangle.Width : this.DisplayRectangle.Height;

							// image will be aligned to centre of the larger side
							int newX = newsize == this.DisplayRectangle.Width ? this.DisplayRectangle.X : this.DisplayRectangle.X + (this.DisplayRectangle.Width - newsize) / 2;
							int newY = newsize == this.DisplayRectangle.Height ? this.DisplayRectangle.Y : this.DisplayRectangle.Y + (this.DisplayRectangle.Height - newsize) / 2;

							pevent.Graphics.DrawImage(this.BackgroundImage, new Rectangle(newX,newY,newsize,newsize));
							return;
						}

						matrix.Translate((float)this.DisplayRectangle.X, (float)this.DisplayRectangle.Y);
					}
					brush.Transform = matrix;
					pevent.Graphics.FillRectangle( brush, ClientRectangle );
					return;
				}
				finally
				{
					brush.Dispose();
				}
			}

			pevent.Graphics.Clip = saveClip;
		}

		protected override void OnControlAdded( ControlEventArgs e )
		{
			base.OnControlAdded( e );
						
			e.Control.LocationChanged += new EventHandler( Control_LocationChanged );
			e.Control.SizeChanged += new EventHandler( Control_SizeChanged );
		}

		protected override void OnControlRemoved( ControlEventArgs e )
		{
			base.OnControlRemoved( e );

			e.Control.LocationChanged -= new EventHandler( Control_LocationChanged );
			e.Control.SizeChanged -= new EventHandler( Control_SizeChanged );
		}

		protected override void OnSizeChanged( EventArgs e )
		{
			base.OnSizeChanged( e );
            if (!EnableTouchMode && CTRLSIZE != this.Size)
            {
                CTRLSIZE = this.Size;
            }
			if( this.m_animator != null && !this.m_animator.AnimationOn )
			{
				RefreshSizes();
				RefreshPath();
				RefreshPrimitives();
				Invalidate();
			}
		}

		protected override void Dispose( bool disposing )
		{
			if( this.Primitives != null && this.Primitives.Count > 0 )
			{
				foreach( Primitive primitive in this.Primitives )
				{
					primitive.Dispose();
				}
			}

			base.Dispose (disposing);
		}

		#endregion

		#region Class Utility Method

		/// <summary>
		/// Draws transparent background.
		/// </summary>
		private void PaintTransparentBackground( PaintEventArgs e, Rectangle rectangle )
		{
			Graphics graphics = e.Graphics;
			Control parentControl = this.Parent;

			if( parentControl != null )
			{
                Form frm = parentControl as Form;
				NativeMethods.POINT location = new NativeMethods.POINT( 0, 0 );

                if (frm != null && frm.RightToLeft == RightToLeft.Yes && frm.RightToLeftLayout)
                    location = new NativeMethods.POINT(this.Right, 0);

				NativeMethods.MapWindowPoints( this.Handle, parentControl.Handle,
					ref location, 1 );

				rectangle.Offset( location.X, location.Y );
				PaintEventArgs args = new PaintEventArgs( graphics, rectangle );
				GraphicsState state = graphics.Save();

				try
				{
					#if  SyncfusionFramework4_0
					graphics.TranslateTransform( ( float ) - location.X, ( float ) - location.Y );
					#endif
					this.InvokePaintBackground( parentControl, args );

					graphics.Restore( state );
					state = graphics.Save();

					graphics.TranslateTransform( ( float ) - location.X, ( float ) - location.Y );
					this.InvokePaint( parentControl, args );
					return;
				}
				finally
				{
					graphics.Restore( state );
				}
			}
			graphics.FillRectangle( SystemBrushes.Control, rectangle );
		}


		internal void MakeDirty()
		{
			IComponentChangeService changeService = this.GetService( typeof( IComponentChangeService ) ) as IComponentChangeService;
			
			if( changeService != null )
			{
				changeService.OnComponentChanging( this, null );
				changeService.OnComponentChanged( this, null, null, null );
			}
		}


		/// <summary>
		/// Calculates count position of animation.
		/// </summary>
		private int GetCountAnimationPosition( Alignment alignment )
		{
			int countPosition = 0;

			switch( alignment )
			{
				case Alignment.Top :
				{
					if( this.m_animator.AnimationOn )
					{
						countPosition = ( this.Collapsed ) ? 
							( this.RealSize.Height - this.CollapseSize ) / this.AnimationSpeed 
							- this.m_animator.AnimationPosition :
							( this.ExpandSize.Height - this.Size.Height ) / this.AnimationSpeed;
					}
					else
					{
						countPosition = this.RealSize.Height - this.CollapseSize;
					}

					break;
				}
				case Alignment.Bottom :
				{
					if( this.m_animator.AnimationOn )
					{
						countPosition = ( this.Collapsed ) ? 
							( this.RealSize.Height - this.CollapseSize ) / this.AnimationSpeed 
							- this.m_animator.AnimationPosition :
							( this.ExpandSize.Height - this.Size.Height ) / this.AnimationSpeed;
					}
					else
					{
						countPosition = this.RealSize.Height - this.CollapseSize;
					}
					break;
				}
				case Alignment.Left :
				{
					if( this.m_animator.AnimationOn )
					{
						countPosition = ( this.Collapsed ) ? 
							( this.RealSize.Width - this.CollapseSize ) / this.AnimationSpeed 
							- this.m_animator.AnimationPosition :
							( this.ExpandSize.Width - this.Size.Width ) / this.AnimationSpeed;
					}
					else
					{
						countPosition = this.RealSize.Width - this.CollapseSize;
					}
					
					break;
				}
				case Alignment.Right :
				{
					if( this.m_animator.AnimationOn )
					{
						countPosition = ( this.Collapsed ) ? 
							( this.RealSize.Width - this.CollapseSize ) / this.AnimationSpeed 
							- this.m_animator.AnimationPosition :
							( this.ExpandSize.Width - this.Size.Width ) / this.AnimationSpeed;
					}
					else
					{
						countPosition = this.RealSize.Width - this.CollapseSize;
					}

					break;
				}
			}

			return countPosition;
		}


		/// <summary>
		/// Initiate animation.
		/// </summary>
		private void StartAnimation()
		{
			if( this.m_animator != null )
			{
				HidePrimitive( this.CollapseAlignment );
				HideControls();

				m_bIsRedrawCollapsedControl = true;

				int countPosition = 0;

				if( !this.m_animator.AnimationOn )
				{
					if( this.Collapsed )
					{
						this.ExpandSize = new Size( this.Size.Width, this.Size.Height );
						this.ExpandLocation = new Point( this.Location.X, this.Location.Y );
					}

					RefreshSizes();
					RefreshPath();
					Invalidate();
				}

				// calculates count position of animation
				countPosition = GetCountAnimationPosition( this.CollapseAlignment );
				

				if( countPosition != 0 && !this.m_animator.AnimationOn )
				{
					countPosition = countPosition / this.AnimationSpeed;
				}

				// start animation
				this.m_animator.StartAnimation( countPosition, this.Collapsed, this.AnimationDelay );
			}
		}


		/// <summary>
		/// Collapsing control.
		/// </summary>
		internal void Collapse()
		{
			this.Collapsed = true;

			m_bCollapsedPerforming = true;

			HideControls();
			HidePrimitive( this.CollapseAlignment );
		}


		/// <summary>
		/// Expanding control.
		/// </summary>
		internal void Expand()
		{
			this.Collapsed = false;

			m_bCollapsedPerforming = false;

			if( !this.m_animator.AnimationOn )
			{
				ShowControls();
				ShowPrimitive( this.CollapseAlignment );
			}
		}


		/// <summary>
		/// Check position added control regarding client path.
		/// </summary>
		/// <returns>True if control contains within client area, otherwise False. </returns>
		private bool CheckPositionControl( Control control )
		{
			bool isInside = false;

			if( control != null )
			{
				Point ltPoint = new Point( control.Location.X, control.Location.Y );
				Point lbPoint = new Point( control.Location.X, control.Location.Y + control.Size.Height );
				Point rtPoint = new Point( control.Location.X + control.Size.Width, control.Location.Y );
				Point rbPoint = new Point( control.Location.X + control.Size.Width, 
					control.Location.Y + control.Size.Height );

				if( this.ClientPath.IsVisible( ltPoint ) && this.ClientPath.IsVisible( lbPoint ) &&
					this.ClientPath.IsVisible( rtPoint ) && this.ClientPath.IsVisible( rbPoint ) )
				{
					isInside = true;
				}
			}

			return isInside;
		}


		/// <summary>
		/// Refresh control path and client path.
		/// </summary>
		private void RefreshPath()
		{
			this.ControlPath = GetControlPath( this.CollapseAlignment, this.Collapsed );
			this.ClientPath = GetClientPath( this.CollapseAlignment, this.Collapsed );
		}

	
		/// <summary>
		/// Refresh location for all primitives
		/// </summary>
		private void RefreshPrimitives()
		{
			if( !this.m_bInitializePerforming )
			{
				foreach( Primitive primitive in this.Primitives )
				{
					if( this.CollapseAlignment == primitive.Alignment )
					{
						primitive.Visible = true;
					}

					SetCorrectPrimitivePosition( primitive );
				
					primitive.Bounds = GetPrimitiveRectangle( primitive );
				}
			}
		}

		
		/// <summary>
		/// Refresh correctly position and size for all controls
		/// which contain in GradinetPanelExt.
		/// </summary>
		private void RefreshCorrectPosition()
		{
			if( !this.Collapsed )
			{
				foreach( Control control in this.Controls )
				{
					SetCorrectPosition( control );
				}
			}
		}

		
		/// <summary>
		/// Refresh work sizes.
		/// </summary>
		private void RefreshSizes()
		{
			this.CollapseSize = GetCollapseSize();
			this.m_minSize = GetMinSize();
			this.m_rectangleForBorder = GetRect( this.pBorderGap, this.CollapseAlignment, this.Collapsed );
		}
		

		/// <summary>
		/// Gets inscribed rectangle in rounded path.
		/// </summary>
		private Rectangle GetClientRectangle()
		{
			int offset = this.BorderGap;
			Rectangle rect = Rectangle.Empty;

			// location of the rectangle
			Point location = Point.Empty;

			// size of the rectangle
			Size size = Size.Empty;

			// radius corner
			int r = this.CornerRadius;

			// left top corner
			int alpha = DEF_ANGLE_180 + DEF_ANGLE_45;

			double radians = alpha * ( Math.PI / DEF_ANGLE_180 );
			double dx = r * Math.Cos( radians );
			double dy = r * Math.Sin( radians );

			int x = this.CornerRadius + this.Border.Left;
			int y = this.CornerRadius + this.Border.Top;

			// set location of the rectangle
			location = new Point( x + ( int )dx, y + ( int )dy );

			// right top corner
			alpha = DEF_ANGLE_45;

			radians = alpha * ( Math.PI / DEF_ANGLE_180 );
			dx = r * Math.Cos( radians );
			dy = r * Math.Sin( radians );

			x = this.ClientRectangle.Right - this.CornerRadius + ( int )dx - this.Border.Left - DEF_BORDER_OFFSET;
			y = this.ClientRectangle.Bottom - this.CornerRadius + ( int )dy - this.Border.Top - DEF_BORDER_OFFSET;

			// set size of the rectangle
			size = new Size( x - location.X, y - location.Y );

			rect = new Rectangle( location, size );

			return rect;
		}


		/// <summary>
		/// Gets rectangle which drawing control.
		/// </summary>
		private Rectangle GetRect( int offset, Alignment alignment, bool collapsed )
		{
			Rectangle rect = Rectangle.Empty;

			int rectX = offset;
			int rectY = offset;
			int rectWidth = this.RealSize.Width - 2 * offset;
			int rectHeight = rectHeight = this.RealSize.Height - 2 * offset;

			if( alignment == Alignment.Bottom )
			{
				rectY -= this.RealSize.Height - this.Size.Height;
			}

			if( alignment == Alignment.Right )
			{
				rectX -= this.RealSize.Width - this.Size.Width;
			}

			rect = new Rectangle( rectX, rectY, rectWidth - DEF_BORDER_OFFSET, 
				rectHeight - DEF_BORDER_OFFSET );

			return rect;
		}
		/// <summary>
		/// Gets rectangle which drawing control.
		/// </summary>
		private Rectangle GetRect(Padding pboderGap, Alignment alignment, bool collapsed)
		{
			Rectangle rect = Rectangle.Empty;

			int rectX = pboderGap.Left;
			int rectY = pboderGap.Top;
			int rectWidth = this.RealSize.Width - (pboderGap.Left + pboderGap.Right);
			int rectHeight = rectHeight = this.RealSize.Height - (pboderGap.Top + pboderGap.Bottom);

			if (alignment == Alignment.Bottom)
			{
				rectY -= this.RealSize.Height - this.Size.Height;
			}

			if (alignment == Alignment.Right)
			{
				rectX -= this.RealSize.Width - this.Size.Width;
			}

			rect = new Rectangle(rectX, rectY, rectWidth - DEF_BORDER_OFFSET,
				rectHeight - DEF_BORDER_OFFSET);

			return rect;
		}

		/// <summary>
		/// Gets rounded path.		
		/// </summary>
		private GraphicsPath GetPath( int offset, int positionOffset, Alignment alignment, bool collapsed )
		{
			GraphicsPath path = new GraphicsPath( FillMode.Winding );
			Rectangle rect = GetRect( offset, alignment, collapsed );

			if( alignment == Alignment.Bottom )
			{
				rect.Offset( positionOffset, 0 );
			}

			if( alignment == Alignment.Right )
			{
				rect.Offset( 0, positionOffset );
			}

			if( this.CornerRadius > 0 )
			{
				int xArc = 0;
				int yArc = 0;
				int sizeArc = 2 * this.CornerRadius;
				
				int collapsedSize = this.CollapseSize;

				// left top corner
				xArc = rect.X;
				yArc = rect.Y;
				path.AddArc( xArc, yArc, sizeArc, sizeArc, DEF_ANGLE_180, DEF_ANGLE_90 );

				// reight top corner
				xArc = rect.Right - sizeArc;
				yArc = rect.Y;
				path.AddArc( xArc, yArc, sizeArc, sizeArc, DEF_ANGLE_270, DEF_ANGLE_90 );

				// reight bottom corner
				xArc = rect.Right - sizeArc;
				yArc = rect.Bottom - sizeArc;
				path.AddArc( xArc, yArc, sizeArc, sizeArc, DEF_ANGLE_0, DEF_ANGLE_90 );

				// left bottom corner
				xArc = rect.X;
				yArc = rect.Bottom - sizeArc;
				path.AddArc( xArc, yArc, sizeArc, sizeArc, DEF_ANGLE_90, DEF_ANGLE_90 );
				
				// left top corner
				xArc = rect.X;
				yArc = rect.Y;
				path.AddArc( xArc, yArc, sizeArc, sizeArc, DEF_ANGLE_180, DEF_ANGLE_90 );
			}
			else
			{
				path.AddRectangle( rect );
			}

			return path;
		}


		/// <summary>
		/// Gets graphics path which represent control.
		/// </summary>
		private GraphicsPath GetControlPath( Alignment alignment, bool collapsed )
		{
			return GetPath( 0, 0, alignment, collapsed );
		}


		/// <summary>
		/// Gets graphics path which represent client area of the control.
		/// </summary>
		private GraphicsPath GetClientPath( Alignment alignment, bool collapsed )
		{
			return GetPath( this.BorderGap + DEF_BORDER_OFFSET, 0, alignment, collapsed );
		}

		
		/// <summary>
		/// Gets rectangle which primitive is drawing.
		/// </summary>
		internal Rectangle GetPrimitiveRectangle( Primitive primitive )
		{
			Rectangle rect = Rectangle.Empty;

			if( primitive != null )
			{
				int maxPositionX = this.ClientRectangle.Right - this.CornerRadius - primitive.Size.Width;
				int maxPositionY = this.ClientRectangle.Height - this.CornerRadius - primitive.Size.Height;

				Point startPoint = Point.Empty;
				Point location = Point.Empty;

				int offset = DEF_BORDER_OFFSET;
				int offsetY = primitive.Size.Height / 2 - offset;
				int offsetX = primitive.Size.Width / 2 - offset;

				switch( primitive.Alignment )
				{
					case Alignment.Top :
					{
						startPoint = GetStartPosition( Alignment.Top );
						location = new Point( startPoint.X + primitive.Position, startPoint.Y - offsetY );
						rect = new Rectangle( location, primitive.Size );

						break;
					}
					case Alignment.Bottom :
					{
						startPoint = GetStartPosition( Alignment.Bottom );
						location = new Point( startPoint.X + primitive.Position, startPoint.Y - offsetY );
						rect = new Rectangle( location, primitive.Size );

						break;
					}
					case Alignment.Left : 
					{
						startPoint = GetStartPosition( Alignment.Left );
						location = new Point( startPoint.X - offsetY, startPoint.Y + primitive.Position );
						rect = new Rectangle( location, new Size( primitive.Size.Height, 
							primitive.Size.Width ) );
						
						break;
					}
					case Alignment.Right :
					{
						startPoint = GetStartPosition( Alignment.Right );
						location = new Point( startPoint.X - offsetY, startPoint.Y + primitive.Position );
						rect = new Rectangle( location, new Size( primitive.Size.Height, primitive.Size.Width ) );

						break;
					}
				}
			}

			return rect;
		}

		
		/// <summary>
		/// Gets start point position primitive from given side.
		/// </summary>
		internal Point GetStartPosition( Alignment side )
		{
			Point startPoint = Point.Empty;
			int offset = 2 * DEF_BORDER_OFFSET;

			switch( side )
			{
				case Alignment.Top :
				{
					startPoint = new Point( this.ClientRectangle.Left + this.CornerRadius + this.Border.Left, 
						this.ClientRectangle.Top + this.Border.Top );
					break;
				}
				case Alignment.Bottom :
				{
					startPoint = new Point( this.ClientRectangle.X + this.CornerRadius + this.Border.Left, 
						this.ClientRectangle.Bottom - this.Border.Bottom - offset );
					break;
				}
				case Alignment.Left : 
				{
					startPoint = new Point( this.ClientRectangle.X + this.Border.Left, 
						this.ClientRectangle.Top + this.CornerRadius + this.Border.Top );
					break;
				}
				case Alignment.Right : 
				{
					startPoint = new Point( this.ClientRectangle.Right - this.Border.Right - offset, 
						this.ClientRectangle.Top + this.CornerRadius + this.Border.Top );
					break;
				}
			}

			return startPoint;
		}
		

		/// <summary>
		/// Gets end point position of the primitive from given side.
		/// </summary>
		internal Point GetEndPosition( Alignment side )
		{
			Point endPoint = Point.Empty;
			int offset = 2 * DEF_BORDER_OFFSET;

			switch( side )
			{
				case Alignment.Top :
				{
					endPoint = new Point( this.ClientRectangle.Right - this.CornerRadius - this.Border.Right,
						this.ClientRectangle.Top + this.Border.Top );
					break;
				}
				case Alignment.Bottom :
				{
					endPoint = new Point( this.ClientRectangle.Right - this.CornerRadius - this.Border.Right,
						this.ClientRectangle.Bottom - this.Border.Bottom - offset );
					break;
				}
				case Alignment.Left : 
				{
					endPoint = new Point( this.ClientRectangle.X + this.Border.Left, 
						this.ClientRectangle.Bottom - this.CornerRadius - this.Border.Bottom );
					break;
				}
				case Alignment.Right : 
				{
					endPoint = new Point( this.ClientRectangle.Right - this.Border.Right - offset, 
						this.ClientRectangle.Bottom - this.CornerRadius - this.Border.Bottom );
					break;
				}
			}

			return endPoint;
		}

		
		/// <summary>
		/// Gets size collapsed control.
		/// </summary>
		private int GetCollapseSize()
		{
			int collapseSize = 0;

			collapseSize = this.CornerRadius + this.BorderGap;

			int minCollapsedSize = 2 * this.BorderGap;

			if( minCollapsedSize > collapseSize )
			{
				collapseSize = minCollapsedSize;
			}

			collapseSize += DEF_COLLAPSE_OFFSET;

			return collapseSize;
		}


		/// <summary>
		/// Find maximum size of the primitive in the collection.
		/// </summary>
		private Size GetMaxPrimitivesSize()
		{
			Size maxSize = Size.Empty;

			foreach( Primitive primitive in this.Primitives )
			{
				if( primitive.Size.Height > maxSize.Height ||
					primitive.Size.Width > maxSize.Width )
				{
					maxSize = primitive.Size;
				} 
			}

			return maxSize;
		}
		/// <summary>
		/// Find maximum size of the primitive in the collection for a specific side.
		/// </summary>
		private Size GetMaxPrimitivesSize(Alignment alignment)
		{
			Size maxSize = Size.Empty;

			foreach (Primitive primitive in this.Primitives)
			{
				if ( primitive.Alignment ==alignment && (primitive.Size.Height > maxSize.Height || primitive.Size.Width > maxSize.Width))
				{
						maxSize = primitive.Size;
				}
			}
			return maxSize;
		}
		/// <summary>
		/// Gets minimum border gap.
		/// </summary>
		private int GetMinBorderGap()
		{
			int maxValue = GetMaxPrimitivesSize().Height;

			maxValue = maxValue / 2 + DEF_PRIMITIVE_CONTROL_GAP;

			return maxValue;
		}
		/// <summary>
		/// Gets minimum border gap for a specific side.
		/// </summary>
		private int GetMinBorderGap(Alignment alignment)
		{
			int maxValue = GetMaxPrimitivesSize(alignment).Height;

			maxValue = maxValue / 2 + DEF_PRIMITIVE_CONTROL_GAP;

			return maxValue;
		}
		
		/// <summary>
		/// Gets maximum border gap.
		/// </summary>
		private int GetMaxBorderGap()
		{
			int maxBorderGap = ( this.RealSize.Width > this.RealSize.Height ) ? 
				this.RealSize.Height : this.RealSize.Width;

			Size maxSizePrimitive = GetMaxPrimitivesSize();

			int maxSize = ( maxSizePrimitive.Height > maxSizePrimitive.Width ) ? 
				maxSizePrimitive.Height : maxSizePrimitive.Width;

			maxBorderGap -= 2 * this.CornerRadius;
			maxBorderGap -= maxSize;
			maxBorderGap = maxBorderGap / 2;

			int minBorderGap = GetMinBorderGap();
			maxBorderGap = ( maxBorderGap < minBorderGap ) ? minBorderGap : maxBorderGap;
			
			return maxBorderGap;
		}
        /// <summary>
        /// Gets maximum border gap for the specified side.
        /// </summary>
        private int GetMaxBorderGap(Alignment alignment)
        {
            int maxBorderGap = (this.RealSize.Width > this.RealSize.Height) ?
                this.RealSize.Height : this.RealSize.Width;

            Size maxSizePrimitive = GetMaxPrimitivesSize(alignment);

            int maxSize = (maxSizePrimitive.Height > maxSizePrimitive.Width) ?
                maxSizePrimitive.Height : maxSizePrimitive.Width;

            maxBorderGap -= 2 * this.CornerRadius;
            maxBorderGap -= maxSize;
            maxBorderGap = maxBorderGap / 2;
            
            int minBorderGap = GetMinBorderGap();
            maxBorderGap = (maxBorderGap < minBorderGap) ? minBorderGap : maxBorderGap;

            return maxBorderGap;
        }
        /// <summary>
        /// Ensures if the PBorderGap can be set
        /// </summary>
        private bool CheckPBorderGap(Padding value)
        {

            if (value.Left > GetMaxBorderGap(Alignment.Left) || value.Left < GetMinBorderGap(Alignment.Left))
                return false;
            else if (value.Right > GetMaxBorderGap(Alignment.Right) || value.Right < GetMinBorderGap(Alignment.Right))
                return false;
            else if (value.Top > GetMaxBorderGap(Alignment.Top) || value.Top < GetMinBorderGap(Alignment.Top))
                return false;
            else if (value.Bottom > GetMaxBorderGap(Alignment.Bottom) || value.Bottom < GetMinBorderGap(Alignment.Bottom))
                return false;

            return true;
        }
		
		/// <summary>
		/// Gets maximum radius.
		/// </summary>
		/// <returns></returns>
		private int GetMaxRadius()
		{
			Size expandSize = Size.Empty;

			int maxRadius = ( this.RealSize.Width > this.RealSize.Height ) ?
				this.RealSize.Height : this.RealSize.Width;
			
			Size maxPrimitiveSize = GetMaxPrimitivesSize();
			
			if( maxPrimitiveSize.Width + DEF_MIN_LENGHT_BORDER > maxRadius )
			{
				maxRadius = maxPrimitiveSize.Width + DEF_MIN_LENGHT_BORDER;
			}
			else
			{
				maxRadius -= ( maxPrimitiveSize.Width + DEF_MIN_LENGHT_BORDER );
			}

			maxRadius = maxRadius / 2;

			return maxRadius;
		}


		/// <summary>
		/// Gets minimum size of the control.
		/// </summary>
		private Size GetMinSize()
		{
			Size primitiveSize = GetMinSizeFromPrimitive();
			Size controlSize = GetMinSizeFromControl();

			int width = ( primitiveSize.Width < controlSize.Width ) ? primitiveSize.Width : 
				controlSize.Width;

			int height = ( primitiveSize.Height < controlSize.Height ) ? primitiveSize.Height :
				controlSize.Height;

			Size minSize = new Size( width, height );

			return minSize;
		}

		
		/// <summary>
		/// Gets minimum size of the control relative to Primitives.
		/// </summary>
		private Size GetMinSizeFromPrimitive()
		{
			Size size = Size.Empty;

			int xPosition = 0;
			int yPosition = 0;

			foreach( Primitive primitive in this.Primitives )
			{
				switch( primitive.Alignment )
				{
					case Alignment.Top :
					{
						if( xPosition < primitive.Bounds.Right )
						{
							xPosition = primitive.Bounds.Right;
						}

						break;
					}
					case Alignment.Bottom :
					{
						if( xPosition < primitive.Bounds.Right )
						{
							xPosition = primitive.Bounds.Right;
						}

						break;
					}
					case Alignment.Left :
					{
						if( yPosition < primitive.Bounds.Bottom )
						{
							yPosition = primitive.Bounds.Bottom;
						}

						break;
					}
					case Alignment.Right :
					{
						if( yPosition < primitive.Bounds.Bottom )
						{
							yPosition = primitive.Bounds.Bottom;
						}

						break;
					}
				}
			}

			int width = ( xPosition == 0 ) ? 2 * this.CornerRadius +this.Border.Horizontal+ DEF_MIN_LENGHT_BORDER :
				this.CornerRadius + this.Border.Left  + xPosition + 2 * DEF_PRIMITIMVE_BORDER_GAP;
			
			int height = ( yPosition == 0 ) ? 2 * this.CornerRadius + this.Border.Vertical + DEF_MIN_LENGHT_BORDER : 
				this.CornerRadius + this.Border.Top + yPosition + 2 * DEF_PRIMITIMVE_BORDER_GAP;

			size = new Size( width, height );

			return size;
		}


		/// <summary>
		/// Gets minimum size of the control relative to controls when contains control.
		/// </summary>
		private Size GetMinSizeFromControl()
		{
			Size size = Size.Empty;

			int xPosition = 0;
			int yPosition = 0;

			foreach( Control control in this.Controls )
			{
				if( !IsControlHostControl( control ) )
				{
					if( xPosition < control.Bounds.Right )
					{
						xPosition = control.Bounds.Right;
					}

					if( yPosition < control.Bounds.Bottom ) 
					{
						yPosition = control.Bounds.Bottom;
					}
				}
			}

			int width = ( xPosition == 0 ) ? 2 * this.CornerRadius + this.Border.Horizontal + DEF_MIN_LENGHT_BORDER :
				this.CornerRadius / 2 + this.Border.Left + xPosition;
			
			int heght = ( yPosition == 0 ) ? 2 * this.CornerRadius +this.Border.Vertical + DEF_MIN_LENGHT_BORDER : 
				this.CornerRadius / 2 + this.Border.Top + yPosition;
			
			size = new Size( width, heght );

			return size;
		}

		
		/// <summary>
		/// Gets lines which dont contained in border.
		/// </summary>
		private ArrayList GetNonBorderLine()
		{
			ArrayList lines = new ArrayList();
			
			Line darkLine = Line.Empty;
			Line lightLine = Line.Empty;

			Point p1 = Point.Empty;
			Point p2 = Point.Empty;

			int offset = DEF_PRIMITIMVE_BORDER_GAP;
			int offsetLine = DEF_BORDER_OFFSET;
			
			foreach( Primitive primitive in this.Primitives )
			{
				if( primitive.Visible && ( !this.Collapsed || 
					this.Collapsed && this.CollapseAlignment == primitive.Alignment ) )
				{
					switch( primitive.Alignment )
					{
						case Alignment.Top :
						{
							p1 = new Point( primitive.Bounds.X - offset, 
								primitive.Bounds.Y + primitive.Bounds.Size.Height / 2 - offsetLine );
							p2 = new Point( primitive.Bounds.Right + offset + offsetLine, 
								primitive.Bounds.Y + primitive.Bounds.Size.Height / 2 - offsetLine );
							darkLine = new Line( p1, p2 );

							p1 = new Point( primitive.Bounds.X - offset,
								primitive.Bounds.Y + primitive.Bounds.Size.Height / 2 );
							p2 = new Point( primitive.Bounds.Right + offset + offsetLine,
								primitive.Bounds.Y + primitive.Bounds.Size.Height / 2 );
							lightLine = new Line( p1, p2 );

							break;
						}
						case Alignment.Bottom :
						{
							p1 = new Point( primitive.Bounds.X - offset,
								primitive.Bounds.Y + primitive.Bounds.Size.Height / 2 - offsetLine );
							p2 = new Point( primitive.Bounds.Right + offset + offsetLine,
								primitive.Bounds.Y + primitive.Bounds.Size.Height / 2 - offsetLine );
							darkLine = new Line( p1, p2 );

							p1 = new Point( primitive.Bounds.X - offset, 
								primitive.Bounds.Y + primitive.Bounds.Size.Height / 2 );
							p2 = new Point( primitive.Bounds.Right + offset + offsetLine,
								primitive.Bounds.Y + primitive.Bounds.Size.Height / 2 );
							lightLine = new Line( p1, p2 );
							break;
						}
						case Alignment.Left :
						{
							p1 = new Point( primitive.Bounds.X + primitive.Bounds.Size.Width / 2 - offsetLine, 
								primitive.Bounds.Y - offset );
							p2 = new Point( primitive.Bounds.X + primitive.Bounds.Size.Width / 2 - offsetLine, 
								primitive.Bounds.Bottom + offset + offsetLine );
							darkLine = new Line( p1, p2 );

							p1 = new Point( primitive.Bounds.X + primitive.Bounds.Size.Width / 2, 
								primitive.Bounds.Y - offset );
							p2 = new Point( primitive.Bounds.X + primitive.Bounds.Size.Width / 2, 
								primitive.Bounds.Bottom + offset + offsetLine );
							lightLine = new Line( p1, p2 );

							break;
						}
						case Alignment.Right :
						{
							p1 = new Point( primitive.Bounds.X + primitive.Bounds.Size.Width / 2 - offsetLine, 
								primitive.Bounds.Y - offset );
							p2 = new Point( primitive.Bounds.X + primitive.Bounds.Size.Width / 2 - offsetLine, 
								primitive.Bounds.Bottom + offset + offsetLine );
							darkLine = new Line( p1, p2 );

							p1 = new Point( primitive.Bounds.X + primitive.Bounds.Size.Width / 2, 
								primitive.Bounds.Y - offset );
							p2 = new Point( primitive.Bounds.X + primitive.Bounds.Size.Width / 2, 
								primitive.Bounds.Bottom + offset + offsetLine );
							lightLine = new Line( p1, p2 );

							break;
						}
					}

					lines.Add( darkLine );
					lines.Add( lightLine );
				}
			}

			return lines;
		}

		
		/// <summary>
		/// Gets IDesignerHost interface.
		/// </summary>
		/// <returns></returns>
		internal IDesignerHost GetIDesignerHost()
		{
			IDesignerHost designerHost = ( IDesignerHost )this.GetService( typeof( IDesignerHost ) );

			return designerHost;
		}

		
		/// <summary>
		/// Gets region when dont draw.
		/// </summary>
		private Region GetNonDrawClip()
		{
			Region clip = new Region( this.ClientRectangle );

			ArrayList arrLines = GetNonBorderLine();

			Rectangle rect;

			foreach( Line line in arrLines )
			{
				rect = new Rectangle( line.Point1, line.GetSize() );
				clip.Exclude( rect );
			}

			return clip;
		}


		/// <summary>
		/// Draws control.
		/// </summary>
		private void DrawControl( Graphics g )
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;

			if( this.BackgroundImage == null )
			{
				if( BackgroundColor != BrushInfo.Empty )
				{
					// draws gradient background
					BrushPaint.FillPath( g, this.ControlPath, this.BackgroundColor );
				}
				else
				{
					// draws simple background
					Brush brush = new SolidBrush( this.BackColor );
					g.FillPath( brush, this.ControlPath );
					brush.Dispose();
				}
			}
		}

		
		/// <summary>
		/// Draws the border.
		/// </summary>
		private void DrawBorder( Graphics g )
		{
			Rectangle rect = m_rectangleForBorder;

			g.SmoothingMode = SmoothingMode.AntiAlias;
			
			// draws border
			if( this.CornerRadius > 0 )
			{
				// draws non-rectangle border
				DrawNonRectBorder( g, rect );
				
			}
			else
			{
				// draws rectangle border
				DrawRectBorder( g, rect );
			}

			// draw primitive
			foreach( Primitive primitive in this.Primitives )
			{
				if( this.Collapsed )
				{
					if( primitive.Alignment == this.CollapseAlignment )
					{
						primitive.Draw( g );
					}
				}
				else
				{
					primitive.Draw( g );
				}
			}
		}


		/// <summary>
		/// Draws non-rectangle border.
		/// </summary>
		private void DrawNonRectBorder( Graphics g, Rectangle rect )
		{
			g.Clip = GetNonDrawClip();

			if( this.CornerRadius > 0 )
			{
				DrawLightLines( g, rect );
				DrawDarkLines( g, rect );
				DrawLightArcs( g, rect );
				DrawDarkArcs( g, rect );

				// draw collapse lines
				if( this.m_animator != null && !this.m_animator.AnimationOn )
				{
					DrawCollapseLine( g, rect );
				}
			}
			using(Region region=new Region( this.ClientRectangle ))
				g.Clip = region;
		}

		
		/// <summary>
		/// Draws light lines for non-rectangle border.
		/// </summary>
		private void DrawLightLines( Graphics g, Rectangle rect )
		{
			Pen lightPen = GetLightPen();

			int x1Line = 0;
			int y1Line = 0;
			int x2Line = 0;
			int y2Line = 0;

			int borderOffset = DEF_BORDER_OFFSET;

			// top line
			x1Line = rect.Left + this.CornerRadius;
			y1Line = rect.Top + borderOffset ;
			x2Line = rect.Right - this.CornerRadius - borderOffset;
			y2Line = y1Line;

			g.DrawLine( lightPen, x1Line, y1Line, x2Line, y2Line );

			// left line
			x1Line = rect.Left + borderOffset;
			y1Line = rect.Top + this.CornerRadius;
			x2Line = x1Line;
			y2Line = rect.Bottom - this.CornerRadius;

			g.DrawLine( lightPen, x1Line, y1Line, x2Line, y2Line );

			// reight line
			x1Line = rect.Right;
			y1Line = rect.Top + this.CornerRadius;
			x2Line = x1Line;
			y2Line = rect.Bottom - this.CornerRadius;
				
			g.DrawLine( lightPen, x1Line, y1Line, x2Line, y2Line );
				
			// bottom line
			x1Line = rect.Left + this.CornerRadius;
			y1Line = rect.Bottom;
			x2Line = rect.Right - this.CornerRadius;
			y2Line = y1Line;
								
			g.DrawLine( lightPen, x1Line, y1Line, x2Line, y2Line );
		}


		/// <summary>
		/// Draws dark lines for non-rectangle border.
		/// </summary>
		private void DrawDarkLines( Graphics g, Rectangle rect )
		{
			Pen darkPen = GetDarkPen();

			int x1Line = 0;
			int y1Line = 0;
			int x2Line = 0;
			int y2Line = 0;

			int borderOffset = DEF_BORDER_OFFSET;

			// top dark line
			x1Line = rect.Left + this.CornerRadius;
			y1Line = rect.Top ;
			x2Line = rect.Right - this.CornerRadius - borderOffset;
			y2Line = y1Line;

			g.DrawLine( darkPen, x1Line, y1Line, x2Line, y2Line );
				

			// reight line
			x1Line = rect.Right - borderOffset;
			y1Line = rect.Top + this.CornerRadius;
			x2Line = x1Line;
			y2Line = rect.Bottom - this.CornerRadius;

			g.DrawLine( darkPen, x1Line, y1Line, x2Line, y2Line );

			// bottom line
			x1Line = rect.Left + this.CornerRadius;
			y1Line = rect.Bottom - borderOffset;
			x2Line = rect.Right - this.CornerRadius - borderOffset;
			y2Line = y1Line;

			g.DrawLine( darkPen, x1Line, y1Line, x2Line, y2Line );

			// left line
			x1Line = rect.Left;
			y1Line = rect.Top + this.CornerRadius;
			x2Line = x1Line;
			y2Line = rect.Bottom - this.CornerRadius - borderOffset;

			g.DrawLine( darkPen, x1Line, y1Line, x2Line, y2Line );
		}


		/// <summary>
		/// Draws light arcs for non-rectangle border.
		/// </summary>
		private void DrawLightArcs( Graphics g, Rectangle rect )
		{
			Pen lightPen = GetLightPen();

			int xArc = 0;
			int yArc = 0;
			int heightArc = 0;
			int widthArc = 0;
			Rectangle rectArc = Rectangle.Empty;
			int borderOffset = DEF_BORDER_OFFSET;

			// arc for left top corner
			xArc = rect.Left + borderOffset;
			yArc = rect.Top + borderOffset;
			heightArc = 2 * this.CornerRadius;
			widthArc = 2 * this.CornerRadius;
			rectArc = new Rectangle( xArc, yArc, widthArc, heightArc );

			g.DrawArc( lightPen, rectArc, DEF_ANGLE_180, DEF_ANGLE_90 );

			// arc for reight top corner
			xArc = rect.Right - 2 * this.CornerRadius - borderOffset;
			yArc = rect.Top + borderOffset;
			heightArc = 2 * this.CornerRadius;
			widthArc = 2 * this.CornerRadius;
			rectArc = new Rectangle( xArc, yArc, widthArc, heightArc );

			g.DrawArc( lightPen, rectArc, DEF_ANGLE_270, DEF_ANGLE_45 );
				
			// arc for left bottom corner
			xArc = rect.Left + borderOffset;
			yArc = rect.Bottom - 2 * this.CornerRadius - borderOffset;
			heightArc = 2 * this.CornerRadius;
			widthArc = 2 * this.CornerRadius;
			rectArc = new Rectangle( xArc, yArc, widthArc, heightArc );

			g.DrawArc( lightPen, rectArc, DEF_ANGLE_90 + DEF_ANGLE_45, DEF_ANGLE_45 );

			// arc for reight top corner
			xArc = rect.Right - 2 * this.CornerRadius;
			yArc = rect.Top;
			heightArc = 2 * this.CornerRadius;
			widthArc = 2 * this.CornerRadius;
			rectArc = new Rectangle( xArc, yArc, widthArc, heightArc );
				
			g.DrawArc( lightPen, rectArc, DEF_ANGLE_270 + DEF_ANGLE_45, DEF_ANGLE_45 );
				
			// reight bottom corner
			xArc = rect.Right - 2 * this.CornerRadius;
			yArc = rect.Bottom - 2 * this.CornerRadius;
			heightArc = 2 * this.CornerRadius;
			widthArc = 2 * this.CornerRadius;
			rectArc = new Rectangle( xArc, yArc, widthArc, heightArc );
			g.DrawArc( lightPen, rectArc, DEF_ANGLE_0, DEF_ANGLE_90 );
				
			// arc for left bottom corner
			xArc = rect.Left;
			yArc = rect.Bottom - 2 * this.CornerRadius;
			heightArc = 2 * this.CornerRadius;
			widthArc = 2 * this.CornerRadius;
			rectArc = new Rectangle( xArc, yArc, widthArc, heightArc );
				
			g.DrawArc( lightPen, rectArc, DEF_ANGLE_90, DEF_ANGLE_45 );
		}


		/// <summary>
		/// Draws dark arcs for non-rectangle border.
		/// </summary>
		private void DrawDarkArcs( Graphics g, Rectangle rect )
		{
			Pen darkPen = GetDarkPen();

			int xArc = 0;
			int yArc = 0;
			int heightArc = 0;
			int widthArc = 0;
			Rectangle rectArc = Rectangle.Empty;
			int borderOffset = DEF_BORDER_OFFSET;

			// left top corner
			xArc = rect.Left;
			yArc = rect.Top;
			heightArc = 2 * this.CornerRadius;
			widthArc = 2 * this.CornerRadius;
			rectArc = new Rectangle( xArc, yArc, widthArc, heightArc );

			g.DrawArc( darkPen, rectArc, DEF_ANGLE_180, DEF_ANGLE_90 );

			// reight top corner
			xArc = rect.Right - 2 * this.CornerRadius - borderOffset;
			yArc = rect.Top;
			heightArc = 2 * this.CornerRadius;
			widthArc = 2 * this.CornerRadius;
			rectArc = new Rectangle( xArc, yArc, widthArc, heightArc );

			g.DrawArc( darkPen, rectArc, DEF_ANGLE_270, DEF_ANGLE_90 );

			// reight bottom corner
			xArc = rect.Right - 2 * this.CornerRadius - borderOffset;
			yArc = rect.Bottom - 2 * this.CornerRadius - borderOffset;
			heightArc = 2 * this.CornerRadius;
			widthArc = 2 * this.CornerRadius;
			rectArc = new Rectangle( xArc, yArc, widthArc, heightArc );

			g.DrawArc( darkPen, rectArc, DEF_ANGLE_0, DEF_ANGLE_90 );

			// left bottom corner
			xArc = rect.Left;
			yArc = rect.Bottom - 2 * this.CornerRadius - borderOffset;
			heightArc = 2 * this.CornerRadius;
			widthArc = 2 * this.CornerRadius;
			rectArc = new Rectangle( xArc, yArc, widthArc, heightArc );

			g.DrawArc( darkPen, rectArc, DEF_ANGLE_90, DEF_ANGLE_90 );
		}
		
		
		/// <summary>
		/// Draws rectangle border.
		/// </summary>
		private void DrawRectBorder( Graphics g, Rectangle rect )
		{
			int offset = DEF_BORDER_OFFSET;

			Pen lightPen = GetLightPen();
			Pen darkPen = GetDarkPen();

			Rectangle lightRect = new Rectangle( rect.X + offset, rect.Y + offset,  
				rect.Width - offset, rect.Height - offset );

			// draw light rectangle
			g.DrawRectangle( lightPen, lightRect );

			// draw dark rectangle
			Rectangle darkRect = new Rectangle( rect.X, rect.Y,  rect.Width - offset, 
				rect.Height - offset );
			g.DrawRectangle( darkPen, darkRect );

			// draw collapse lines
			if( this.m_animator != null && !this.m_animator.AnimationOn )
			{
				DrawCollapseLine( g, rect );
			}
		}
		

		/// <summary>
		/// Draws lines for collapsed control.
		/// </summary>
		private void DrawCollapseLine( Graphics g, Rectangle rect )
		{
			if( this.Collapsed )
			{
				DrawCollapseLineLight( g, rect );
				DrawCollapseLineDark( g, rect );
			}
		}
		

		/// <summary>
		/// Draws light line for collapsed control.
		/// </summary>
		private void DrawCollapseLineLight( Graphics g, Rectangle rect )
		{
			int offset = DEF_BORDER_OFFSET;
			Pen lightPen = GetLightPen();

			switch( this.CollapseAlignment )
			{
				case Alignment.Top :
				{
					g.DrawLine( lightPen, rect.Left, this.CollapseSize - offset, 
						rect.Right, this.CollapseSize - offset );

					break;
				}
				case Alignment.Bottom : 
				{
					g.DrawLine( lightPen, rect.Left + offset, 
						this.ClientRectangle.Bottom - this.CollapseSize + offset, 
						rect.Right - offset, this.ClientRectangle.Bottom - this.CollapseSize + offset );

					break;
				}
				case Alignment.Left :
				{
					g.DrawLine( lightPen, this.CollapseSize - offset, rect.Top, 
						this.CollapseSize - offset, rect.Bottom );

					break;
				}
				case Alignment.Right :
				{
					g.DrawLine( lightPen, ClientRectangle.Right - this.CollapseSize + offset, 
						rect.Top + offset, ClientRectangle.Right - this.CollapseSize + offset, 
						rect.Bottom - offset );

					break;
				}
			}
		}
		

		/// <summary>
		/// Draws dark line for collapsed control.
		/// </summary>
		private void DrawCollapseLineDark( Graphics g, Rectangle rect )
		{
			int offset = DEF_BORDER_OFFSET;
			Pen darkPen = GetDarkPen();

			switch( this.CollapseAlignment )
			{
				case Alignment.Top :
				{
					g.DrawLine( darkPen, rect.Left, this.CollapseSize - 2 * offset, 
						rect.Right - offset, this.CollapseSize - 2 * offset );

					break;
				}
				case Alignment.Bottom : 
				{
					g.DrawLine( darkPen, rect.Left, this.ClientRectangle.Bottom - this.CollapseSize, 
						rect.Right - offset, this.ClientRectangle.Bottom - this.CollapseSize );

					break;
				}
				case Alignment.Left :
				{
					g.DrawLine( darkPen, this.CollapseSize - 2 * offset, rect.Top, 
						this.CollapseSize - 2 * offset, rect.Bottom - offset );

					break;
				}
				case Alignment.Right :
				{
					g.DrawLine( darkPen, ClientRectangle.Right - this.CollapseSize, rect.Top, 
						ClientRectangle.Right - this.CollapseSize, rect.Bottom - offset );

					break;
				}
			}
		}


		/// <summary>
		/// Changes size of the control.
		/// </summary>
		private Rectangle ChangeSizeControl( Rectangle controlBound )
		{
			if( controlBound != Rectangle.Empty )
			{
				Rectangle minRectangle = GetClientRectangle();

				//controlBound.Location = minRectangle.Location;

				// changes width of the control
				if( minRectangle.Size.Width < controlBound.Size.Width )
				{
					controlBound.Size = new Size( minRectangle.Size.Width, 
						controlBound.Size.Height );
					//controlBound.Location = minRectangle.Location;
				}

				// changes height of the control
				if( minRectangle.Size.Height < controlBound.Size.Height )
				{
					controlBound.Size = new Size( controlBound.Size.Width, 
						minRectangle.Size.Height );
					//controlBound.Location = minRectangle.Location;
				}
			}

			return controlBound;
		}

		
		/// <summary>
		/// Changes location of the control.
		/// </summary>
		private Rectangle ChangeLocationControl( Rectangle controlBounds )
		{
			Rectangle minRectangle = GetClientRectangle();

			int offset = DEF_BORDER_OFFSET;

			Point ltPoint = new Point( controlBounds.X, controlBounds.Y );
			Point lbPoint = new Point( controlBounds.X, controlBounds.Bottom );
			Point rtPoint = new Point( controlBounds.Right, controlBounds.Y );
			Point rbPoint = new Point( controlBounds.Right, controlBounds.Bottom );

			if( controlBounds.X < minRectangle.X )
			{
				controlBounds.X = minRectangle.X;

				if( this.CornerRadius == 0 )
				{
					controlBounds.X += DEF_BORDER_OFFSET;
				}
			}
			else if( controlBounds.Right > minRectangle.Right )
			{
				controlBounds.X = minRectangle.Right - controlBounds.Width;

				if( this.CornerRadius == 0 )
				{
					controlBounds.X -= DEF_COLLAPSE_OFFSET;
				}
			}

			// for top side
			if( ltPoint.Y < minRectangle.Top )
			{
				bool finished = false;

				while( !finished )
				{
					controlBounds.Y += offset;

					rtPoint = new Point( controlBounds.Right, controlBounds.Y );

					if( this.ClientPath.IsVisible( controlBounds.Location ) &&
						this.ClientPath.IsVisible( rtPoint ) ||
						ltPoint.Y > minRectangle.Top )
					{
						finished = true;
					}
				}
			}

			// for bottom side
			if( rbPoint.Y > minRectangle.Bottom )
			{
				bool finished = false;

				while( !finished && controlBounds.Y > 0 )
				{
					controlBounds.Y -= offset;

					lbPoint = new Point( controlBounds.X, controlBounds.Bottom );

					if( this.ClientPath.IsVisible( new Point( controlBounds.Right,
						controlBounds.Bottom ))
						&& this.ClientPath.IsVisible( lbPoint )  
						|| rbPoint.Y < minRectangle.Bottom )
					{
						finished = true;
					}
				}
			}


			// for left side
			if( ltPoint.X < minRectangle.Left )
			{
				bool finished = false;

				while( !finished )
				{
					controlBounds.X += offset;

					if( this.ClientPath.IsVisible( controlBounds.Location ) ||
						ltPoint.X > minRectangle.Left )
					{
						finished = true;
					}
				}
			}

			// for raight side
			if( rbPoint.X > minRectangle.Right )
			{
				bool finished = false;

				while( !finished && controlBounds.X > 0 )
				{
					controlBounds.X -= offset;

					if( this.ClientPath.IsVisible( new Point( controlBounds.Right, 
						controlBounds.Bottom ) ) 
						|| rbPoint.X < minRectangle.Right )
					{
						finished = true;
					}
				}
			}

			return controlBounds;
		}


		/// <summary>
		/// Sets correctly position of the control regarding client path.
		/// </summary>
		private void SetCorrectPosition( Control control )
		{	
			IDesignerHost designerHost = GetIDesignerHost();

			if( !this.m_bInitializePerforming && 
				designerHost != null && !designerHost.Loading &&
				control != null && !IsControlHostControl( control ) )
			{
				m_bPerformingCorrectionsLocation = true;

				Rectangle controlBounds = control.Bounds;
				Rectangle minRectangle = GetClientRectangle();

				if( minRectangle.Size.Width < controlBounds.Size.Width || 
					minRectangle.Size.Height < controlBounds.Size.Height )
				{
					// changes size of the control
					controlBounds = ChangeSizeControl( controlBounds );
				}
				else
				{
					// changes location of the control
					controlBounds = ChangeLocationControl( controlBounds );
				}

				control.Location = controlBounds.Location;
				control.Size = controlBounds.Size;

				m_bPerformingCorrectionsLocation = false;
			}
		}


		/// <summary>
		/// Determines whether the control is the HostControl.
		/// </summary>
		internal bool IsControlHostControl( Control control )
		{
			bool result = false;

			if( control != null && this.Primitives != null )
			{

				foreach( Primitive primitive in this.Primitives )
				{
					HostPrimitive hostPrimitive = primitive as HostPrimitive;
				
					if( hostPrimitive != null && hostPrimitive.HostControl == control )
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}


		/// <summary>
		/// Sets correctly corner radius.
		/// </summary>
		private void SetCorrectRadius( Alignment alignment, bool collapsed )
		{
			if( !this.m_bInitializePerforming && this.IsHandleCreated )
			{
				int maxRadius = GetMaxRadius();

				if( this.CornerRadius > maxRadius )
				{
					this.CornerRadius = maxRadius;
				}

				RefreshSizes();
				RefreshPrimitives();

				SetSizeControl( this.CollapseAlignment, this.Collapsed );
			}
		}


		/// <summary>
		/// Sets correctly border gap.
		/// </summary>
		internal void SetCorrectBorderGap( Alignment alignment, bool collapsed )
		{
			int minBorderGap = GetMinBorderGap(alignment);
			int maxBorderGap = GetMaxBorderGap(alignment);
                switch (alignment)
                {
                    case Alignment.Left:
                        if (this.Border.Top > maxBorderGap)
                            this.Border = new Padding(maxBorderGap);
                        else if (this.Border.Top < minBorderGap)
                            this.pBorderGap = new Padding(minBorderGap);

                        this.m_borderGap = this.pBorderGap.Left;
                        break;

                    case Alignment.Right:
                        if (this.Border.Right > maxBorderGap)
                            this.Border = new Padding(maxBorderGap);
                        else if (this.Border.Right < minBorderGap)
                            this.pBorderGap = new Padding(minBorderGap);

                        this.m_borderGap = this.pBorderGap.Right;
                        break;

                    case Alignment.Top:
                        if (this.Border.Top > maxBorderGap)
                            this.Border = new Padding(maxBorderGap);
                        else if (this.Border.Top < minBorderGap)
                            this.pBorderGap = new Padding(minBorderGap);

                        this.m_borderGap = this.pBorderGap.Top;
                        break;

                    case Alignment.Bottom:
                        if (this.Border.Bottom > maxBorderGap)
                            this.Border = new Padding(maxBorderGap);
                        else if (this.Border.Bottom < minBorderGap)
                            this.pBorderGap = new Padding(minBorderGap);

                        this.m_borderGap = this.pBorderGap.Bottom;
                        break;

                }
			SetSizeControl( this.CollapseAlignment, this.Collapsed );
		}

		
		/// <summary>
		/// Sets correctly position of the primitive.
		/// </summary>
		private void SetCorrectPrimitivePosition( Primitive primitive )
		{
			IDesignerHost designerHost = GetIDesignerHost();
			
			if( !this.m_animator.AnimationOn && primitive != null &&
				!(designerHost != null && designerHost.Loading) )
			{
				Point startPosition = GetStartPosition( primitive.Alignment );
				Point entPosition = GetEndPosition( primitive.Alignment );

				int position = 0;

				if( primitive.Alignment == Alignment.Top || primitive.Alignment == Alignment.Bottom )
				{
					position = entPosition.X - startPosition.X - primitive.Size.Width;
				}
				else
				{
					position = entPosition.Y - startPosition.Y - primitive.Size.Height;
				}

				if( position > 0 && position < primitive.Position )
				{
					primitive.Position = position;
				}
			}
		}

		
		/// <summary>
		/// Gets correct size of the control.
		/// </summary>
		private Rectangle GetCorrectSize( Rectangle rect, Alignment alignment, bool collapsed, 
			BoundsSpecified specified )
		{
			Rectangle correctRect = Rectangle.Empty;

			RefreshSizes();

			Size minSize = this.m_minSize;
			Size newSize = Size.Empty;

			if( !collapsed )
			{
				// control expanded.
				if( rect.Width < minSize.Width )
				{
					newSize.Width = minSize.Width;
				}
				else
				{
					newSize.Width = rect.Width;
				}

				if( rect.Height < minSize.Height )
				{
					newSize.Height = minSize.Height;
				}
				else
				{
					newSize.Height = rect.Height;
				}
			}
			else if( this.m_animator != null && !this.m_animator.AnimationOn )
			{
				// control collapsed
				if( m_bCollapsedPerforming && collapsed &&
					( specified == BoundsSpecified.All || specified == BoundsSpecified.Size ) )
				{
					// if control collapsed then don't changing it collapse height
					if( alignment == Alignment.Top || alignment == Alignment.Bottom ) 
					{
						rect.Height = this.Size.Height;
						
						if( this.Size.Width > this.m_minSize.Width )
						{
							rect.Y = this.Location.Y;
						}

						if( rect.Width < minSize.Width )
						{
							newSize = new Size( minSize.Width, rect.Height );
						}
						
						this.m_expandSize.Width = rect.Width;
					}
					else
					{
						rect.Width = this.Size.Width;
						rect.X = this.Location.X;

						if( rect.Height < minSize.Height )
						{
							newSize = new Size( rect.Width, minSize.Height );
						}

						this.m_expandSize.Height = rect.Height;
					}
				}
			}

			if( newSize == Size.Empty )
			{
				correctRect = rect;
			}
			else
			{
				correctRect = new Rectangle( rect.Location, newSize );
			}

			return correctRect;
		}


		/// <summary>
		/// Sets position of the control.
		/// </summary>
		private void SetPositionControl( Alignment alignment, bool collapsed )
		{
			if( collapsed )
			{
				// if control collapsed

				Point newLocation = Point.Empty;
				int collapseSize = this.CollapseSize;

				if( this.ExpandLocation == Point.Empty )
				{
					this.ExpandLocation = this.Location;
				}

				if( alignment == Alignment.Bottom )
				{
					newLocation = new Point( this.ExpandLocation.X, 
						this.ExpandLocation.Y + this.ExpandSize.Height - collapseSize );
				} 
				else if( alignment == Alignment.Right )
				{
					newLocation = new Point( this.ExpandLocation.X + this.ExpandSize.Width - collapseSize, 
						this.ExpandLocation.Y );
				}
				else
				{
					newLocation = this.ExpandLocation;
				}

				this.Location = newLocation;
			}
			else
			{
				// if control expanded

				if( this.ExpandLocation != Point.Empty )
				{
					this.Location = this.ExpandLocation;
					this.ExpandLocation = Point.Empty;
				}
			}
		}

		
		/// <summary>
		/// Hide primitives.
		/// </summary>
		private void HidePrimitive( Alignment alignment )
		{
			foreach( Primitive primitive in this.Primitives )
			{
				if( primitive.Alignment != alignment )
				{
					primitive.Visible = false;
				}
			}
		}

		
		/// <summary>
		/// Show primitives.
		/// </summary>
		private void ShowPrimitive( Alignment alignment )
		{
			foreach( Primitive primitive in this.Primitives )
			{
				primitive.Visible = true;
			}
		}
		
	
		/// <summary>
		/// Sets size of the control.
		/// </summary>
		private void SetSizeControl( Alignment alignment, bool collapsed )
		{
			int collapseSize = this.CollapseSize;

			if( collapsed )
			{
				// if control collapsed

				m_bCollapsedPerforming = false;

				if( this.ExpandSize == Size.Empty )
				{
					this.ExpandSize = this.Size;
				}

				if( alignment == Alignment.Top || alignment == Alignment.Bottom )
				{
					if( m_lastAlignment == Alignment.Left || m_lastAlignment == Alignment.Right )
					{
						Size newSize = new Size( this.ExpandSize.Width, collapseSize );
						this.Size = newSize;
					}
					else
					{
						Size newSize = new Size( this.Size.Width, collapseSize );
						this.Size = newSize;
					}
				}

				if( alignment == Alignment.Left || alignment == Alignment.Right )
				{
					if( m_lastAlignment == Alignment.Bottom || m_lastAlignment == Alignment.Top )
					{
						Size newSize = new Size( collapseSize, this.ExpandSize.Height );
						this.Size = newSize;
					}
					else
					{
						Size newSize = new Size( collapseSize, this.Size.Height );
						this.Size = newSize;
					}
				}

				m_bCollapsedPerforming = true;
			}
			else
			{
				if( this.ExpandSize != Size.Empty )
				{
					this.Size = this.ExpandSize;
					this.ExpandSize = Size.Empty;
				}
			}
		}

		
		/// <summary>
		/// Hide all controls which contains this control.
		/// </summary>
		private void HideControls()
		{
			foreach( Control control in this.Controls )
			{
				if( !IsControlHostControl( control ) )
				{
					control.Hide();
				}
			}
		}

		
		/// <summary>
		/// Show all controls which contains this control.
		/// </summary>
		private void ShowControls()
		{
			foreach( Control control in this.Controls )
			{
				control.Show();
			}
		}


		/// <summary>
		/// Gets bound of the control when doing animation.
		/// </summary>
		private Rectangle GetAnimationRect( Alignment alignment, bool collapse )
		{
			Rectangle rect = Rectangle.Empty;

			Size size = this.Size;
			Point location = this.Location;

			int offset = this.AnimationSpeed;

			switch( alignment )
			{
				case Alignment.Top :
				{
					size = ( collapse ) ? new Size( this.Size.Width, this.Size.Height - offset ) : 
						new Size( this.Size.Width, this.Size.Height + offset );

					break;
				}
				case Alignment.Bottom :
				{
					location = ( collapse ) ? new Point( this.Location.X, this.Location.Y + offset ) :
						new Point( this.Location.X, this.Location.Y - offset );
					size = ( collapse ) ? new Size( this.Size.Width, this.Size.Height - offset ) :
						new Size( this.Size.Width, this.Size.Height + offset );

					break;
				}
				case Alignment.Left :
				{
					size = ( collapse ) ? new Size( this.Size.Width - offset, this.Size.Height ) :
						new Size( this.Size.Width + offset, this.Size.Height );

					break;
				}
				case Alignment.Right :
				{
					location = ( collapse ) ? new Point( this.Location.X + offset, this.Location.Y ) :
						new Point( this.Location.X - offset, this.Location.Y );
					size = ( collapse ) ? new Size( this.Size.Width - offset, this.Size.Height ) :
						new Size( this.Size.Width + offset, this.Size.Height );

					break;
				}
			}

			rect = new Rectangle( location, size );

			return rect;
		}


		/// <summary>
		/// Gets rectangle for redwaw when doing animation.
		/// </summary>
		private Rectangle GetAnimationRedrawRect( Alignment alignment, bool collapse )
		{
			Rectangle rect = Rectangle.Empty;

			int offset = this.AnimationSpeed;

			switch( alignment )
			{
				case Alignment.Top :
				{
					rect = ( collapse ) ? new Rectangle( this.Location.X, 
						this.Location.Y + this.Size.Height, this.Size.Width, offset ) :
						new Rectangle( ClientRectangle.X, 
						this.ClientRectangle.Bottom - offset, this.Size.Width, offset );

					break;
				}
				case Alignment.Bottom :
				{
					rect = ( collapse ) ? new Rectangle( this.Location.X, this.Location.Y - offset , 
						this.Size.Width, offset ) : 
						new Rectangle( ClientRectangle.X, this.ClientRectangle.Y, 
						this.Size.Width, offset );

					break;
				}
				case Alignment.Left :
				{
					rect = ( collapse ) ? new Rectangle( this.Location.X + this.Size.Width, 
						this.Location.Y , offset, this.Size.Height ) :
						new Rectangle( this.ClientRectangle.Right - offset, 
						this.ClientRectangle.Y, offset, this.Size.Height );

					break;
				}
				case Alignment.Right :
				{
					rect = ( collapse ) ? new Rectangle( this.Location.X - offset, this.Location.Y, 
						offset, this.Size.Height ) :
						new Rectangle( ClientRectangle.X, this.ClientRectangle.Y, 
						offset, this.Size.Height );

					break;
				}
			}

			return rect;
		}


		/// <summary>
		/// Gets dark pen for border.
		/// </summary>
		private Pen GetDarkPen()
		{
			Pen darkPen = ( this.BackgroundColor != BrushInfo.Empty ) ? 
				new Pen( ControlPaint.Dark( this.BackgroundColor.BackColor, 0f ) ) :
				new Pen( ControlPaint.Dark( this.BackColor, 0f ) );

			return darkPen;
		}


		/// <summary>
		/// Gets light pen for border.
		/// </summary>
		private Pen GetLightPen()
		{
			Pen lightPen = ( this.BackgroundColor != BrushInfo.Empty ) ? 
				new Pen( ControlPaint.Light( this.BackgroundColor.BackColor, 1f ) ) :
				new Pen( ControlPaint.Light( this.BackColor, 1f ) );

			return lightPen;
		}


		/// <summary>
		/// Changes collapse state for each CollapsePrimitive from <see cref="Primitives"/>.
		/// </summary>
		private void ChangePrimitiveState( bool collapseState )
		{
			if( this.Primitives != null && this.Primitives.Count > 0 )
			{
				foreach( Primitive primitive in this.Primitives )
				{
					if( primitive is CollapsePrimitive )
					{
						CollapsePrimitive collapsePrimitive = primitive as CollapsePrimitive;
						collapsePrimitive.SetCollapseState( collapseState );
					}
				}
			}
		}
		

		#endregion

		#region Class Events

		/// <summary>
		/// Raise by <see cref="OnCornerRadiusChanged"/> method.
		/// </summary>
		[ 
		Description( "Event fired when the value of CornerRadius property is changed." ),
		Category( "Property Changed" )
		]
		public event EventHandler CornerRadiusChanged;

		/// <summary>
		/// Raise by <see cref="OnBorderGapChanged"/> method.
		/// </summary>
		[ 
		Description( "Event fired when the value of BorderGap property is changed." ),
		Category( "Property Changed" )
		]
		public event EventHandler BorderGapChanged;

		/// <summary>
		/// Raise by <see cref="OnCollapseAlignmentChanged"/> method.
		/// </summary>
		[ 
		Description( "Event fired when the value of CollapseAlignment property is changed." ),
		Category( "Property Changed" )
		]
		public event EventHandler CollapseAlignmentChanged;

		/// <summary>
		/// Raise by <see cref="OnCollapsedChanged"/> method.
		/// </summary>
		[ 
		Description( "Event fired when the value of Collapsed property is changed." ),
		Category( "Property Changed" )
		]
		public event EventHandler CollapsedChanged;

		/// <summary>
		/// Raise by <see cref="OnPrimitivesChanged"/> method.
		/// </summary>
		[ 
		Description( "Event fired when the value of Primitives property is changed." ),
		Category( "Property Changed" )
		]
		public event EventHandler PrimitivesChanged;


		#endregion

		#region Class Event Raisers

		private void RaiseCollapsedChanged()
		{
			if( this.CollapsedChanged != null )
			{
				this.CollapsedChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseCollapseAlignmentChanged()
		{
			if( this.CollapseAlignmentChanged != null )
			{
				this.CollapseAlignmentChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseCornerRadiusChanged()
		{
			if( this.CornerRadiusChanged != null )
			{
				this.CornerRadiusChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseBorederGapChanged()
		{
			if( this.BorderGapChanged != null )
			{
				this.BorderGapChanged( this, EventArgs.Empty );
			}
		}

		private void RaisePrimitivesChanged()
		{
			if( this.PrimitivesChanged != null )
			{
				this.PrimitivesChanged( this, EventArgs.Empty );
			}
		}

		protected virtual void OnPrimitivesChanged()
		{
			SetCorrectRadius( this.CollapseAlignment, this.Collapsed );
			SetCorrectBorderGap( this.CollapseAlignment, this.Collapsed );
			RefreshSizes();
			RefreshPath();
			RefreshPrimitives();

			RaisePrimitivesChanged();
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			Invalidate();
		}		

		protected virtual void OnCornerRadiusChanged()
		{
			if( !this.m_bInitializePerforming )
			{
				SetCorrectRadius( this.CollapseAlignment, this.Collapsed );
				SetCorrectBorderGap( this.CollapseAlignment, this.Collapsed );

				RefreshSizes();
				RefreshPath();
				RefreshPrimitives();
				RefreshCorrectPosition();

				Invalidate();
			}

			RaiseCornerRadiusChanged();
		}

		
		protected virtual void OnBorderGapChanged()
		{
			if( !this.m_bInitializePerforming )
			{
				SetCorrectBorderGap( this.CollapseAlignment, this.Collapsed );
				RefreshSizes();
				RefreshPath();
				RefreshPrimitives();
				RefreshCorrectPosition();
			
				Invalidate();
			}

			RaiseBorederGapChanged();
		}

		protected virtual void OnCollapseAlignmentChanged()
		{
			if( !this.m_bInitializePerforming )
			{
				if( this.Collapsed )
				{
					m_bCollapsedPerforming = false;
				}

				SetSizeControl( this.CollapseAlignment, this.Collapsed );
				SetPositionControl( this.CollapseAlignment, this.Collapsed );

				RefreshPath();
				RefreshPrimitives();
				RefreshCorrectPosition();
				RefreshSizes();

				Invalidate();

				if( this.Collapsed )
				{
					m_bCollapsedPerforming = true;
				}
			}

			RaiseCollapseAlignmentChanged();
		}

		protected virtual void OnCollapsedChanged()
		{
			if( !this.m_bInitializePerforming )
			{
				ChangePrimitiveState( this.Collapsed );

				if( this.Animated )
				{
					StartAnimation();
				}
				else
				{
					if( this.Collapsed )
					{
						HidePrimitive( this.CollapseAlignment );
						HideControls();
					}
					else
					{
						ShowPrimitive( this.CollapseAlignment );
						ShowControls();
					}

					SetSizeControl( this.CollapseAlignment, this.Collapsed );
					SetPositionControl( this.CollapseAlignment, this.Collapsed );

					RefreshSizes();
					RefreshPath();
					RefreshPrimitives();
					RefreshCorrectPosition();

					Invalidate();
				}
			}

			RaiseCollapsedChanged();
		}


		#endregion

		#region Class Event Handler

		private void Control_LocationChanged( object sender, EventArgs e )
		{
			if( !m_bPerformingCorrectionsLocation )
			{
				Control control = sender as Control;

				if( control != null )
				{
					if( !CheckPositionControl( control ) )
					{
						// sets correct location control which added
						SetCorrectPosition( control );
					}
				}
			}
		}

		private void Control_SizeChanged(object sender, EventArgs e)
		{
			Control control = sender as Control;

			if( control != null )
			{
				SetCorrectPosition( control );
			}
		}

		private void Animator_AnimationPositionChanged(object sender, EventArgs e)
		{
			if( this.m_animator.AnimationPosition > 0 )
			{
				Rectangle redrawRect = Rectangle.Empty;

				NativeMethodsHelper.SuspendRedrawWindow( this.Handle );
				
				// changes size end location of the control
				Rectangle animationRect = GetAnimationRect( this.CollapseAlignment, this.Collapsed );
				this.Location = animationRect.Location;
				this.Size = animationRect.Size;
				redrawRect = GetAnimationRedrawRect( this.CollapseAlignment, this.Collapsed );

				RefreshSizes();
				RefreshPath();
				RefreshPrimitives();

				NativeMethodsHelper.ResumeRedrawWindow( this.Handle );

				// redraw control
				if( m_bIsRedrawCollapsedControl )
				{
					this.Invalidate();
					this.m_bIsRedrawCollapsedControl = false;
				}
				
				if( this.Collapsed && this.Parent != null )
				{
					this.Parent.Invalidate( redrawRect );
				}
				else if( !this.Collapsed )
				{
					this.Invalidate( redrawRect, true );
				}
			}
			else
			{
				// stop animation
				this.m_animator.StopAnimation();
			}
		}

		private void Animator_AnimationDone(object sender, EventArgs e)
		{
			NativeMethodsHelper.SuspendRedrawWindow( this.Handle );

			SetSizeControl( this.CollapseAlignment, this.Collapsed );
			SetPositionControl( this.CollapseAlignment, this.Collapsed );

			RefreshPath();
			RefreshPrimitives();
			RefreshSizes();

			if( !this.Collapsed )
			{
				ShowControls();
				ShowPrimitive( this.CollapseAlignment );
			}

			NativeMethodsHelper.ResumeRedrawWindow( this.Handle );

			Invalidate( true );
		}

		private void Primitives_CollectionChanged( object sender, CollectionChangeEventArgs e )
		{
			Primitive primitive = e.Element as Primitive;

			switch( e.Action )
			{
				case CollectionChangeAction.Add : 
				{
					Primitive[] primitives = e.Element as Primitive[];

					if( primitive != null )
					{
						primitive.OwnerControl = this;
						primitive.SizeChanged += new EventHandler( Primitive_Property_Changed );
						primitive.AlignmentChanged += new EventHandler( Primitive_Property_Changed );
					}
					else if( primitives != null )
					{
						foreach( Primitive item in primitives )
						{
							item.OwnerControl = this;
							item.SizeChanged += new EventHandler( Primitive_Property_Changed );
							item.AlignmentChanged += new EventHandler( Primitive_Property_Changed );
						}
					}

					SetCorrectRadius( this.CollapseAlignment, this.Collapsed );
					SetCorrectBorderGap( this.CollapseAlignment, this.Collapsed );

					RefreshSizes();
					RefreshPath();
					RefreshPrimitives();

					this.Invalidate();

					break;
				}
				case CollectionChangeAction.Remove :
				{
					if( primitive != null )
					{
						primitive.OwnerControl = null;
					}

					break;
				}
			}
		}

		private void Primitive_Property_Changed( object sender, EventArgs e )
		{
			SetCorrectRadius( ((Primitive)sender).Alignment, this.Collapsed );
			SetCorrectBorderGap( ((Primitive)sender).Alignment, this.Collapsed );

			RefreshSizes();
			RefreshPath();
			RefreshPrimitives();

			this.Invalidate();
		}

		#endregion

		#region Support ISupportInitialize
		
		void ISupportInitialize.BeginInit()
		{
			m_bInitializePerforming = true;
		}

		void ISupportInitialize.EndInit()
		{
			m_bInitializePerforming = false;

			SetCorrectRadius( this.CollapseAlignment, this.Collapsed );
			SetCorrectBorderGap( this.CollapseAlignment, this.Collapsed );

			RefreshSizes();
			RefreshPath();
			RefreshPrimitives();
			RefreshCorrectPosition();

			Invalidate();
		}

		#endregion
	}

	
	/// <summary>
	/// Represent line.
	/// </summary>
	struct Line
	{
		#region Struct Members

		/// <summary>
		/// Start point of the line.
		/// </summary>
		private Point m_point1;

		/// <summary>
		/// End point of the line.
		/// </summary>
		private Point m_point2;

		/// <summary>
		/// Represents a null Line.
		/// </summary>
		public static Line Empty = new Line( Point.Empty, Point.Empty );

		#endregion

		#region Struct Properties

		/// <summary>
		/// Gets or sets start point of the line.
		/// </summary>
		public Point Point1
		{
			get
			{
				return m_point1;
			}
			set
			{
				if( value != m_point1 )
				{
					m_point1 = value;
				}
			}
		}

		
		/// <summary>
		/// Gets or set end point of the line.
		/// </summary>
		public Point Point2
		{
			get
			{
				return m_point2;
			}
			set
			{
				if( value != m_point2 )
				{
					m_point2 = value;
				}
			}
		}


		#endregion

		#region Struct Initialize

		public Line( Point point1, Point point2 )
		{
			m_point1 = point1;
			m_point2 = point2;
		}

		#endregion

		#region Struct Public Method

		/// <summary>
		/// Gets size of the line.
		/// </summary>
		public Size GetSize()
		{
			int x = this.Point2.X - this.Point1.X;
			
			if( x == 0 )
			{
				x = 1;
			}

			int y = this.Point2.Y - this.Point1.Y;

			if( y == 0 )
			{
				y = 1;
			}

			Size size = new Size( x, y );

			return size;
		}

		#endregion
	}
}
