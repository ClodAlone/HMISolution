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
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents primitive that can be displayed in <see cref="GradientPanelExt"/>.
	/// </summary>
	/// <remarks>
	/// <para>In order to display the Primitive, you must add it to the <see cref="Primitive"/>
	/// property of <see cref="GradientPanelExt"/>. This is normally achieved using simple drag-and-drop 
	/// during design-time.</para>
	/// <para>The Primitive class provides properties that enable you to configure the
	/// appearance, behavior abd layoutof a primitive. For selecting a primitive you must click on it.
	/// You can change <see cref="BorderColor"/>,<see cref="BackColor"/> 
	/// and <see cref="PrimitiveBorderStyle"/>. You can define <see cref="Position"/> 
	/// of the Primitive in <see cref="GradientPanel"/>, <see cref="Alignment"/>
	/// inside <see cref="GradientPanelExt"/>.</para>
	/// </remarks>
	[ToolboxItem(false)]
	public class Primitive : Component, ICloneable
	{
		#region Class Constants

		/// <summary>
		/// Color which uses for drawing border selected primitive.
		/// </summary>
		[ NonSerialized ]
		private readonly Color DEF_SELECTED_COLOR = Color.Yellow;

		/// <summary>
		/// Default size for primitive.
		/// </summary>
		[ NonSerialized ]
		protected static readonly Size c_primitiveSize = new Size( 20, 20 );

		/// <summary>
		/// Offset for select rectangle.
		/// </summary>
		private const int DEF_OFFSET = 1;

		/// <summary>
		/// Offset for redraw primitive.
		/// </summary>
		private const int DEF_BORDER_OFFSET = 3;

		/// <summary>
		/// Start position for primitive.
		/// </summary>
		private const int DEF_START_POSITION = 0;

		#endregion
		
		#region Class Members

		/// <summary>
		/// Size of the primitive.
		/// </summary>
		private Size m_size;

		/// <summary>
		/// Reprecent position of the primitive.
		/// </summary>
		private int m_position = 0;

		/// <summary>
		/// Border color for primitive.
		/// </summary>
		private Color m_clrBorder = Color.Black;

		/// <summary>
		/// Background color for primitive.
		/// </summary>
        private Color m_clrBackGround = Color.Transparent;

		/// <summary>
		/// Border style for primitive.
		/// </summary>
		private PrimitiveBorderStyle m_enPrimitiveBorderStyle = PrimitiveBorderStyle.Single;

		/// <summary>
		/// Control which contains this primitive.
		/// </summary>
		[ NonSerialized ]
		private GradientPanelExt m_ownerControl = null;

		/// <summary>
		/// Size and location of the primitive.
		/// </summary>
		private Rectangle m_bounds = Rectangle.Empty;

		/// <summary>
		/// Rectangle which drawing primitive.
		/// </summary>
		private Rectangle m_clientRect = Rectangle.Empty;

		/// <summary>
		/// Rectangle which drew primitive on previous position.
		/// Uses for redraw previous position of the primitives.
		/// </summary>
		private Rectangle m_previousBounds = Rectangle.Empty;

		/// <summary>
		/// Alignment primitive.
		/// </summary>
		private Alignment m_enAlignment = Alignment.Top;

		/// <summary>
		/// A value indicating whether the primitive is displayed.
		/// </summary>
		private bool m_bVisible = true;

		/// <summary>
		/// Indicate that primitive is selectes.
		/// </summary>
		private bool m_bSelected = false;

		#endregion

		#region Class Properties

		/// <summary>
		/// Gets or sets value indicate that primitive is selected.
		/// </summary>
		[ 
		DefaultValue( false ),
		Description( "Indicate that primitive is selected." ),
		Category( "Behavior" )
		]
		internal bool Selected
		{
			get
			{
				return m_bSelected;
			}
			set
			{
				if( value != m_bSelected )
				{
					m_bSelected = value;
					OnSelectedChanged();
				}
			}
		}

	
		/// <summary>
		/// Gets or sets a value indicating whether the primitive is displayed.
		/// </summary>
		[ 
		DefaultValue( true ),
		Description( "Indicating whether the primitive is displayed." ),
		Category( "Behavior" )
		]
		public bool Visible
		{
			get
			{
				return m_bVisible;
			}
			set
			{
				if( value != m_bVisible )
				{
					m_bVisible = value;
					OnVisibleChanged();
				}
			}
		}


		/// <summary>
		/// Gets or set alignment of the primitive.
		/// </summary>
		[ 
		DefaultValue( typeof( Alignment ), "Top" ),
		Description( "Alignment of the primitive." ),
		Category( "Layout" )
		]
		public Alignment Alignment
		{
			get
			{
				return m_enAlignment;
			}
			set
			{
				if( value != m_enAlignment )
				{
					m_enAlignment = value;
					OnAlignmentChanged();
				}
			}
		}

		
		/// <summary>
		/// Gets or sets size of the primitive.
		/// </summary>
		[ 
		Description( "Size of the primitive." ),
		Category( "Layout" )
		]
		public Size Size
		{
			get
			{
				return m_size;
			}
			set
			{
				if( value != m_size )
				{
					m_size = value;
					OnSizeChanged();
				}
			}
		}


		/// <summary>
		/// Gets or sets border color for primitive.
		/// </summary>
		[ 
		DefaultValue( typeof( Color ), "Black" ),
		Description( "Border color for primitive." ),
		Category( "Appearance" )
		]
		public Color BorderColor
		{
			get
			{
				return m_clrBorder;
			}
			set
			{
				if( value != m_clrBorder )
				{
					m_clrBorder = value;
					OnBorderColorChanged();
				}
			}
		}

		
		/// <summary>
		/// Gets or sets background color for primitive.
		/// </summary>
		[ 
		DefaultValue( typeof( Color ), "Transparent" ),
		Description( "Background color for primitive." ),
		Category( "Appearance" )
		]
		public Color BackColor
		{
			get
			{
				return m_clrBackGround;
			}
			set
			{
				if( value != m_clrBackGround )
				{
					m_clrBackGround = value;
					OnBackColorChanged();
				}
			}
		}


		/// <summary>
		/// Gets or sets border style for primitive.
		/// </summary>
		[ 
		DefaultValue( typeof( PrimitiveBorderStyle ), "Single" ),
		Description( "Border style for primitive." ),
		Category( "Appearance" )
		]
		public PrimitiveBorderStyle PrimitiveBorderStyle
		{
			get
			{
				return m_enPrimitiveBorderStyle;
			}
			set
			{
				if( value != m_enPrimitiveBorderStyle )
				{
					m_enPrimitiveBorderStyle = value;
					OnPrimitiveBorderStyleChanged();
				}
			}
		}

		
		/// <summary>
		/// Gets or sets position of the primitive.
		/// </summary>
		[ 
		DefaultValue( 0 ),
		Description( "Position of the primitive." ),
		Category( "Layout" ),
		RefreshProperties( RefreshProperties.All )
		]
		public int Position
		{
			get
			{
				return m_position;
			}
			set
			{
				if( value != m_position )
				{
					m_position = value;
					OnPositionChanged();
				}
			}
		}

		
		/// <summary>
		/// Gets control which contains this primitive.
		/// </summary>
		internal GradientPanelExt OwnerControl
		{
			get
			{
				return m_ownerControl;
			}
			set
			{
				if( value != m_ownerControl )
				{
					m_ownerControl = value;
					OnOwnerControlChanged();
				}
			}
		}


		/// <summary>
		/// Gets or sets the size and location of the primitive.
		/// </summary>
		internal Rectangle Bounds
		{
			get
			{
				return m_bounds;
			}
			set
			{
				if( value != m_bounds )
				{
					m_bounds = value;
					OnBoundsChanged();
				}
			}
		}


		/// <summary>
		/// Gets rectangle of drawing primitive.
		/// </summary>
		protected Rectangle ClientRect
		{
			get
			{
				if( m_clientRect == Rectangle.Empty )
				{
					m_clientRect = GetClientRect();
				}

				return m_clientRect;
			}
		}


		#endregion

		#region Class Initialize/Finalize Method

		public Primitive( Size size, int position, GradientPanelExt owner )
		{
			m_size = size;
			m_position = position;
			m_ownerControl = owner;
		}

		public Primitive( GradientPanelExt owner ) : this( c_primitiveSize, 0, owner )
		{}
		public Primitive() : this( c_primitiveSize, 0, null )
		{}

		public Primitive( Size size, GradientPanelExt owner ) : this( size, 0, owner )
		{}

		#endregion

		#region Class Public Method

		/// <summary>
		/// Draws primitive.
		/// </summary>
		public void Draw( Graphics g )
		{
			if( this.Visible )
			{
				// draws primitive's border.
				DrawBorder( g, this.PrimitiveBorderStyle );

				// draws primitive's background.
				DrawBackground( g );

				if( Selected )
				{
					// draws border for selected primitive in the designer.
					DrawSelectedBorder( g );
				}
			}
		}


		/// <summary>
		/// Redraws primitive.
		/// </summary>
		public void Invalidate()
		{
			if( this.OwnerControl != null )
			{
				int offset = DEF_OFFSET + DEF_BORDER_OFFSET;

				Rectangle rect = new Rectangle( this.Bounds.Location, this.Bounds.Size );
				rect.Inflate( offset, offset );

				// redraw primitive
				this.OwnerControl.Invalidate( rect );

				// redraw old position primitive
				if( m_previousBounds != Rectangle.Empty )
				{
					m_previousBounds.Inflate( offset, offset );
					this.OwnerControl.Invalidate( m_previousBounds );
					m_previousBounds = Rectangle.Empty;
				}
			}
		}

	
		/// <summary>
		/// Draws border for primitive.
		/// </summary>
		protected virtual void DrawBorder( Graphics g, PrimitiveBorderStyle style )
		{
			g.SmoothingMode = SmoothingMode.None;

			Rectangle rect = this.Bounds;
			Pen pen;

			switch( style )
			{
				case PrimitiveBorderStyle.Single : 
				{
					pen = new  Pen( this.BorderColor );
					break;
				}
				case PrimitiveBorderStyle.None :
				{
					pen = new  Pen( this.m_clrBackGround );
					break;
				}
				default : 
				{
					pen = new  Pen( SystemColors.Control );
					break;
				}
			}

			if( this.PrimitiveBorderStyle != PrimitiveBorderStyle.None )
			{
				// draws border
				g.DrawRectangle( pen, rect );
			}
		}


		/// <summary>
		/// Draws primitive.
		/// </summary>
		protected virtual void DrawBackground( Graphics g )
		{
			Rectangle backRect = this.ClientRect;

			Brush brush = new SolidBrush( this.BackColor );

			// fills rectangle of backcolor
			g.FillRectangle( brush, backRect);
			brush.Dispose();
		}


		/// <summary>
		/// Draws border for selected primitive in the designer.
		/// </summary>
		protected void DrawSelectedBorder( Graphics g )
		{
			Rectangle selectedRect = GetSelectedRect();

			Pen pen = new Pen( DEF_SELECTED_COLOR );
			g.DrawRectangle( pen, selectedRect );
			pen.Dispose();
		}


		#endregion

		#region Class Utility Method

		/// <summary>
		/// Gets rectangle for selected primitive.
		/// </summary>
		private Rectangle GetSelectedRect()
		{
			Rectangle rect = new Rectangle( this.Bounds.X - DEF_OFFSET, this.Bounds.Y - DEF_OFFSET, 
				this.Bounds.Size.Width + 2 * DEF_OFFSET, this.Bounds.Size.Height + 2 * DEF_OFFSET );

			return rect;
		}
		
	
		/// <summary>
		/// Gets rectangle which drawing primitive.
		/// </summary>
		private Rectangle GetClientRect()
		{
			int offset = DEF_OFFSET;

			Point location = new Point( this.Bounds.Location.X + offset,
				this.Bounds.Location.Y + offset );
			Size size = new Size( this.Bounds.Size.Width - offset,
				this.Bounds.Size.Height - offset );

			Rectangle backRect = new Rectangle( location, size );

			return backRect;
		}

	
		/// <summary>
		/// Gets center of the primitive.
		/// </summary>
		public Point GetCenter()
		{
			Point center = new Point( this.Bounds.X + this.Bounds.Size.Width / 2,
				this.Bounds.Y + this.Bounds.Height / 2 );

			return center;
		}

	
		/// <summary>
		/// Gets count position of the primitive.
		/// </summary>
		private int GetCountPosition( Alignment alignment )
		{
			int countPosition = 0;

			if( this.OwnerControl != null )
			{
				switch( alignment )
				{
					case Alignment.Top : 
					{
						countPosition = this.OwnerControl.Size.Width;
						break;
					}
					case Alignment.Bottom :
					{
						countPosition = this.OwnerControl.Size.Width;
						break;
					}
					case Alignment.Left : 
					{
						countPosition = this.OwnerControl.Size.Height;
						break;
					}
					case Alignment.Right :
					{
						countPosition = this.OwnerControl.Size.Height;
						
						break;
					}
				}

				countPosition -= 2 * this.OwnerControl.CornerRadius + this.OwnerControl.Border.Left;
			}

			return countPosition;
		}

		
		/// <summary>
		/// Gets correctly position of the primitive.
		/// </summary>
		private int GetCorrectPosition()
		{
			int correctPosition = this.Position;

			if( this.OwnerControl != null )
			{
				Point startPoint = this.OwnerControl.GetStartPosition( this.Alignment );
				Point endPoint = this.OwnerControl.GetEndPosition( this.Alignment );

				switch( this.Alignment )
				{
					case Alignment.Top :
					{
						if( correctPosition < DEF_START_POSITION )
						{
							correctPosition = DEF_START_POSITION;
						}
						else if( startPoint.X + correctPosition + this.Size.Width > endPoint.X )
						{
							correctPosition = GetCountPosition( Alignment.Top );
							correctPosition -= this.Size.Width + this.OwnerControl.CornerRadius;
						}

						break;
					}
					case Alignment.Bottom :
					{
						if( correctPosition < DEF_START_POSITION )
						{
							correctPosition = DEF_START_POSITION;
						}
						else if( startPoint.X + correctPosition + this.Size.Width > endPoint.X )
						{
							correctPosition = GetCountPosition( Alignment.Bottom );
							correctPosition -= this.Size.Width + this.OwnerControl.CornerRadius;
						}

						break;
					}
					case Alignment.Left :
					{
						if( correctPosition < DEF_START_POSITION )
						{
							correctPosition = DEF_START_POSITION;
						}
						else if( startPoint.Y + correctPosition + this.Size.Height > endPoint.Y )
						{
							correctPosition = GetCountPosition( Alignment.Left );
							correctPosition -= this.Size.Width + this.OwnerControl.CornerRadius;
						}

						break;
					}
					case Alignment.Right :
					{
						if( correctPosition < DEF_START_POSITION )
						{
							correctPosition = DEF_START_POSITION;
						}
						else if( startPoint.Y + correctPosition + this.Size.Height > endPoint.Y )
						{
							correctPosition = GetCountPosition( Alignment.Right );
							correctPosition -= this.Size.Width + this.OwnerControl.CornerRadius;
						}

						break;
					}
				}
			} 

			return correctPosition;
		}


		#endregion

		#region Class Overrides

		public override string ToString()
		{
			string name;

			if( this.Site != null )
			{
				name = this.Site.Name;
			}
			else
			{
				name = base.ToString();
			}

			return name;
		}


		#endregion

		#region Class Events

		/// <summary>
		/// Raise by <see cref="OnSelectedChanged"/> method.
		/// </summary>
		internal event EventHandler SelectedChanged;

		/// <summary>
		/// Raise by <see cref="OnSizeChanged"/> method.
		/// </summary>
		public event EventHandler SizeChanged;

		/// <summary>
		/// Raise by <see cref="OnBorderColorChanged"/> method.
		/// </summary>
		public event EventHandler BorderColorChanged;

		/// <summary>
		/// Raise by <see cref="OnBackColorChanged"/> method.
		/// </summary>
		public event EventHandler BackColorChanged;

		/// <summary>
		/// Raise by <see cref="OnPrimitiveBorderStyleChanged"/> method.
		/// </summary>
		public event EventHandler PrimitiveBorderStyleChanged;

		/// <summary>
		/// Raise by <see cref="OnPositionChanged"/> method.
		/// </summary>
		public event EventHandler PositionChanged;

		/// <summary>
		/// Raise by <see cref="OnPropertyChanged"/> method.
		/// </summary>
		public event EventHandler PropertyChanged;

		/// <summary>
		/// Raise by <see cref="OnBoundsChanged"/> method.
		/// </summary>
		public event EventHandler BoundsChanged;

		/// <summary>
		/// Raise by <see cref="OnAlignmentChanged"/> method.
		/// </summary>
		public event EventHandler AlignmentChanged;

		/// <summary>
		/// Raise by <see cref="OnVisibleChanged"/> method.
		/// </summary>
		public event EventHandler VisibleChanged;

		/// <summary>
		/// Raise by <see cref="OnMouseMove"/> method.
		/// </summary>
		public event MouseEventHandler MouseMove;

		/// <summary>
		/// Raise by <see cref="OnMouseDown"/> method.
		/// </summary>
		public event MouseEventHandler MouseDown;

		/// <summary>
		/// Raise by <see cref="OnOwnerControlChanged"/> method.
		/// </summary>
		public event EventHandler OwnerControlChanged;

		#endregion

		#region Class Raisers

		private void RaiseSelectedChanged()
		{
			if( this.SelectedChanged != null )
			{
				this.SelectedChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseVisibleChanged()
		{
			if( this.VisibleChanged != null )
			{
				this.VisibleChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseAlignmentChanged()
		{
			if( this.AlignmentChanged != null )
			{
				this.AlignmentChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseSizeChanged()
		{
			if( this.SizeChanged != null )
			{
				this.SizeChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseBorderColorChanged()
		{
			if( this.BorderColorChanged != null )
			{
				this.BorderColorChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseBackColorChanged()
		{
			if( this.BackColorChanged != null )
			{
				this.BackColorChanged( this, EventArgs.Empty );
			}
		}

		private void RaisePrimitiveBorderStyleChanged()
		{
			if( this.PrimitiveBorderStyleChanged != null )
			{
				this.PrimitiveBorderStyleChanged( this, EventArgs.Empty );
			}
		}

		private void RaisePositionChanged()
		{
			if( this.PositionChanged != null )
			{
				this.PositionChanged( this, EventArgs.Empty );
			}
		}

		private void RaisePropertyChanged()
		{
			if( this.PropertyChanged != null )
			{
				this.PropertyChanged( this, EventArgs.Empty );
			}
		}
		private void RaiseBoundsChanged()
		{
			if( this.BoundsChanged != null )
			{
				this.BoundsChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseMouseMove( MouseEventArgs args )
		{
			if( this.MouseMove != null )
			{
				this.MouseMove( this, args );
			}
		}

		private void RaiseMouseDown( MouseEventArgs args )
		{
			if( this.MouseDown != null )
			{
				this.MouseDown( this, args );
			}
		}

		private void RaiseOwnerControlChanged()
		{
			if( this.OwnerControlChanged != null )
			{
				this.OwnerControlChanged( this, EventArgs.Empty );
			}
		}

		
		protected virtual void OnOwnerControlChanged()
		{
			if( this.OwnerControl != null )
			{
				this.Bounds = this.OwnerControl.GetPrimitiveRectangle( this );
				this.OwnerControl.MouseMove += new MouseEventHandler( owner_MouseMove );
				this.OwnerControl.MouseDown += new MouseEventHandler( owner_MouseDown );
			}

			RaiseOwnerControlChanged();
		}

		protected virtual void OnSizeChanged()
		{
			if( this.OwnerControl != null )
			{
				this.Bounds = this.OwnerControl.GetPrimitiveRectangle( this );
			}

			Invalidate();

			RaiseSizeChanged();
		}

		protected virtual void OnBorderColorChanged()
		{
			Invalidate();

			RaiseBorderColorChanged();
		}

		protected virtual void OnBackColorChanged()
		{
			Invalidate();

			RaiseBackColorChanged();
		}

		protected virtual void OnPrimitiveBorderStyleChanged()
		{
			Invalidate();

			RaisePrimitiveBorderStyleChanged();
		}
		
		protected virtual void OnPositionChanged()
		{
			this.Position = GetCorrectPosition();

			if( this.OwnerControl != null )
			{
				m_previousBounds = new Rectangle( this.Bounds.Location, this.Bounds.Size );
				this.Bounds = this.OwnerControl.GetPrimitiveRectangle( this );
			}

			Invalidate();

			RaisePositionChanged();
		}
		
		protected virtual void OnBoundsChanged()
		{
			m_clientRect = GetClientRect();

			Invalidate();

			RaiseBoundsChanged();
		}

		protected virtual void OnAlignmentChanged()
		{
			Invalidate();

			RaiseAlignmentChanged();
		}

		protected virtual void OnVisibleChanged()
		{
			if( this.OwnerControl != null )
			{
				this.Bounds = this.OwnerControl.GetPrimitiveRectangle( this );
			}

			Invalidate();

			RaiseVisibleChanged();
		}

		protected virtual void OnMouseMove( MouseEventArgs args )
		{
			RaiseMouseMove( args );
		}

		protected virtual void OnMouseDown( MouseEventArgs args )
		{
			RaiseMouseDown( args );
		}

		protected virtual void OnSelectedChanged()
		{
			if( this.OwnerControl != null )
			{
				Rectangle rect = new Rectangle( this.Bounds.X - DEF_OFFSET, this.Bounds.Y - DEF_OFFSET, 
					this.Bounds.Size.Width + DEF_BORDER_OFFSET, this.Bounds.Size.Height + DEF_BORDER_OFFSET );

				this.OwnerControl.Invalidate( rect );
			}
			
			RaiseSelectedChanged();
		}


		#endregion

		#region Class Event Handler

		private void owner_MouseMove( object sender, MouseEventArgs e )
		{
			Point location = new Point( e.X, e.Y );

			if( this.Bounds.Contains( location ) )
			{
				OnMouseMove( e );
			}
		}

		private void owner_MouseDown( object sender, MouseEventArgs e )
		{
			Point location = new Point( e.X, e.Y );

			if( this.Bounds.Contains( location ) )
			{
				OnMouseDown( e );
			}
		}


		#endregion

		#region Supprot ICloneable

		public virtual object Clone()
		{
			Primitive clone = new Primitive( this.Size, this.Position, this.OwnerControl );
			clone.m_enAlignment = this.Alignment;
			clone.m_clrBackGround = this.BackColor;
			clone.m_clrBorder = this.BorderColor;
			clone.m_enPrimitiveBorderStyle = this.PrimitiveBorderStyle;
			clone.m_bounds = this.Bounds;
			clone.m_bSelected = this.Selected;
			clone.m_bVisible = this.Visible;
			clone.m_ownerControl = this.OwnerControl;

			return clone;
		}

		#endregion
	}


	/// <summary>
	/// Represents a collapse/expand primitive.
	/// </summary>
	[
	DesignTimeVisible( false ),
	ToolboxItem( false ) 
	]
	public class CollapsePrimitive : Primitive
	{
		#region Class Members

		/// <summary>
		/// Indicate that primitive is collapse.
		/// </summary>
		private bool m_bIsCollapse = false;

		/// <summary>
		/// Image for collapsed primitive.
		/// </summary>
		private Image m_collapseImage = null;

		/// <summary>
		/// Image for expanded primitive.
		/// </summary>
		private Image m_expandeImage = null;

		#endregion

		#region Class Properties

		/// <summary>
		/// Gets or sets image for collapsed primitive.
		/// </summary>
		[ 
		DefaultValue( null ),
		Description( "Image for collapsed primitive." ),
		Category( "Appearance" )
		]
		public Image CollapseImage
		{
			get
			{
				return m_collapseImage;
			}
			set
			{
				if( value != m_collapseImage )
				{
					m_collapseImage = value;
					OnCollapseImageChanged();
				}
			}
		}

	
		/// <summary>
		/// Gets or sets image for expanded primitive.
		/// </summary>
		[ 
		DefaultValue( null ),
		Description( "Image for expanded primitive." ),
		Category( "Appearance" )
		]
		public Image ExpandImage
		{
			get
			{
				return m_expandeImage;
			}
			set
			{
				if( value != m_expandeImage )
				{
					m_expandeImage = value;
					OnExpandImageChanged();
				}
			}
		}

	
		/// <summary>
		/// Gets or sets value which indicate that primitive is collapse.
		/// </summary>
		[ 
		DefaultValue( false ),
		Description( "Indicate that primitive is collapse." ),
		Category( "Behavior" )
		]
		public bool Collapsed
		{
			get
			{
				return m_bIsCollapse;
			}
			set
			{
				if( value != m_bIsCollapse )
				{
					m_bIsCollapse = value;
					OnCollapsedChanged();
				}
			}
		}


		#endregion

		#region Class Initialize/Finalize Method

		public CollapsePrimitive( Size size, int position, GradientPanelExt owner ) : base( size, position, owner )
		{}

		public CollapsePrimitive( GradientPanelExt owner ) : this( c_primitiveSize, 0, owner )
		{}

		public CollapsePrimitive() : this( c_primitiveSize, 0, null )
		{}

		#endregion

		#region Class Public Methods

		/// <summary>
		/// Sets collapse state. Don't raise <see cref="CollapsedChanged"/> event.
		/// </summary>
		public void SetCollapseState( bool collapse )
		{
			this.m_bIsCollapse = collapse;
		}

		#endregion

		#region Class Overrides

		/// <summary>
		/// Draws plus/minus primitive.
		/// </summary>
		protected override void DrawBackground(Graphics g)
		{
			base.DrawBackground( g );

			Rectangle rect = this.Bounds;

			g.SmoothingMode = SmoothingMode.None;

			Pen pen = new  Pen( this.BorderColor );

			int centerWidth = rect.Left + rect.Width / 2;
			int centerHeight = rect.Top + rect.Height / 2;
			int offset = 2;

			Point startPoint;
			Point endPoint;
			
			if( this.Collapsed )
			{
				if( this.CollapseImage == null )
				{
					// draws gorizontal line
					startPoint = new Point( rect.Left + offset, centerHeight );
					endPoint = new Point( rect.Right - offset, centerHeight );
					g.DrawLine( pen, startPoint, endPoint );

					// draws vertical line
					startPoint = new Point( centerWidth, rect.Top + offset );
					endPoint = new Point( centerWidth, rect.Bottom - offset );
					g.DrawLine( pen, startPoint, endPoint );
				}
				else
				{
					g.DrawImage( this.CollapseImage, this.ClientRect );
				}
			}
			else
			{
				if( this.ExpandImage == null )
				{
					// draws gorizontal line
					startPoint = new Point( rect.Left + offset, centerHeight );
					endPoint = new Point( rect.Right - offset, centerHeight );
					g.DrawLine( pen, startPoint, endPoint );
				}
				else
				{
					g.DrawImage( this.ExpandImage, this.ClientRect );
				}
			}
		}


		public override object Clone()
		{
			CollapsePrimitive clone = new CollapsePrimitive( this.Size, this.Position, this.OwnerControl );
			clone.Alignment = this.Alignment;
			clone.BackColor = this.BackColor;
			clone.BorderColor = this.BorderColor;
			clone.PrimitiveBorderStyle = this.PrimitiveBorderStyle;
			clone.Bounds = this.Bounds;
			clone.Selected = this.Selected;
			clone.Visible = this.Visible;
			clone.Collapsed = this.Collapsed;
			clone.CollapseImage = this.CollapseImage;
			clone.ExpandImage = this.ExpandImage;
			clone.OwnerControl = this.OwnerControl;

			return clone;
		}


		#endregion

		#region Class Events

		/// <summary>
		/// Raise by <see cref="OnCollapsedChanged"/> method.
		/// </summary>
		public event EventHandler CollapsedChanged;

		/// <summary>
		/// Raise by <see cref="OnCollapseImageChanged"/> method.
		/// </summary>
		public event EventHandler CollpaseImageChanged;

		/// <summary>
		/// Raise by <see cref="OnExpandImageChanged"/> method.
		/// </summary>
		public event EventHandler ExpandImageChanged;

		#endregion

		#region Class Raisers

		private void RaiseCollapseImageChanged()
		{
			if( this.CollapsedChanged != null )
			{
				this.CollpaseImageChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseExpandImageChanged()
		{
			if( this.ExpandImageChanged != null )
			{
				this.ExpandImageChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseCollapsedChanged()
		{
			if( this.CollapsedChanged != null )
			{
				this.CollapsedChanged( this, EventArgs.Empty );
			}
		}

		protected virtual void OnCollapseImageChanged()
		{
			if( this.OwnerControl != null )
			{
				this.OwnerControl.Invalidate( this.Bounds );
			}

			RaiseCollapseImageChanged();
		}

		protected virtual void OnExpandImageChanged()
		{
			if( this.OwnerControl != null )
			{
				this.OwnerControl.Invalidate( this.Bounds );
			}

			RaiseExpandImageChanged();
		}

		protected virtual void OnCollapsedChanged()
		{
			if( this.OwnerControl != null )
			{
				if( this.Collapsed )
				{
					this.OwnerControl.CollapseAlignment = this.Alignment;
					this.OwnerControl.Collapse();
				}
				else
				{
					this.OwnerControl.CollapseAlignment = this.Alignment;
					this.OwnerControl.Expand();
				}
			}

			RaiseCollapsedChanged();
		}


		protected override void OnMouseDown(MouseEventArgs args)
		{
			base.OnMouseDown( args );
			
			this.Collapsed = !this.Collapsed;
		}

		#endregion
	}

	
	/// <summary>
	/// Represents a text primitive.
	/// </summary>
	[ 
	DesignTimeVisible( false ),
	ToolboxItem( false ) 
	]
	public class TextPrimitive : Primitive
	{
		#region Class Constants

		/// <summary>
		/// Angle of rotation for text primitive.
		/// </summary>
		private const int DEF_ANGLE_ROTATE = 180;

		#endregion

		#region Class Members

		/// <summary>
		/// Displayed text.
		/// </summary>
		private string m_text = String.Empty;

		/// <summary>
		/// Font for displayed text.
		/// </summary>
		private Font m_textFont = new Font( "Arial", 8 );

		/// <summary>
		/// Color of the displayed text.
		/// </summary>
		private Color m_textColor = SystemColors.ControlText;

		#endregion

		#region Class Properties

		/// <summary>
		/// Gets or sets displayed text.
		/// </summary>
		[ 
		DefaultValue( "" ),
		Description( "Displayed text." ),
		Category( "Appearance" )
		]
		public string Text
		{
			get
			{
				return m_text;
			}
			set
			{
				if( value != m_text )
				{
					m_text= value;
					OnTextChanged();
				}
			}
		}


		/// <summary>
		/// Gets or sets font for displayed text.
		/// </summary>
		[ 
		Description( "Font for displayed text." ),
		Category( "Appearance" )
		]
		public Font TextFont
		{
			get
			{
				return m_textFont;
			}
			set
			{
				if( value != m_textFont )
				{
					m_textFont = value;
					OnTextFontChanged();
				}
			}
		}

		
		/// <summary>
		/// Gets or sets color of the displayed text.
		/// </summary>
		[ 
		Description( "Color of the displayed text." ),
		Category( "Appearance" )
		]		
		public Color TextColor
		{
			get
			{
				return m_textColor;
			}
			set
			{
				if( value != m_textColor )
				{
					m_textColor = value;
					OnTextColorChanged();
				}
			}
		}


		#endregion

		#region Class Initialize/Finalize Method

		public TextPrimitive( Size size, string text, GradientPanelExt owner ) : base( size, owner )
		{
			this.Text = text;
			this.BackColor = Color.Transparent;
			this.PrimitiveBorderStyle = PrimitiveBorderStyle.Single;
		}

		public TextPrimitive( string text, GradientPanelExt owner ) : this( c_primitiveSize, text, owner )
		{}

		public TextPrimitive( GradientPanelExt owner ) : this( c_primitiveSize, String.Empty, owner )
		{}

		public TextPrimitive() : this( c_primitiveSize, String.Empty, null )
		{}

		#endregion

		#region Class Overrides

		protected override void DrawBackground( Graphics g )
		{
			base.DrawBackground( g );

			Rectangle rect = this.ClientRect;

			Brush brush = new SolidBrush( this.TextColor );

			StringFormat textFormat = new StringFormat();
			textFormat.FormatFlags |= StringFormatFlags.NoWrap;
			textFormat.Trimming = StringTrimming.EllipsisCharacter;
			textFormat.Alignment = StringAlignment.Center;
			textFormat.LineAlignment = StringAlignment.Center;

			Matrix saveMatrix = null;
			
			if( this.Alignment == Alignment.Left )
			{
				// rotates text
				textFormat.FormatFlags |= StringFormatFlags.DirectionVertical;

				saveMatrix = g.Transform;
				Matrix transfoemMatrix = new Matrix( );

				transfoemMatrix.RotateAt( DEF_ANGLE_ROTATE, this.GetCenter() );
				
				g.Transform = transfoemMatrix;
			}

			if( this.Alignment == Alignment.Right )
			{
				textFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
			}
			

			g.DrawString( this.Text, this.TextFont, brush, rect, textFormat );
			
			if( saveMatrix != null )
			{
				g.Transform = saveMatrix;
			}
			brush.Dispose();
			textFormat.Dispose();
		}


		public override object Clone()
		{
			TextPrimitive clone = new TextPrimitive( this.Size, this.Text, this.OwnerControl );
			clone.Alignment = this.Alignment;
			clone.BackColor = this.BackColor;
			clone.BorderColor = this.BorderColor;
			clone.PrimitiveBorderStyle = this.PrimitiveBorderStyle;
			clone.Bounds = this.Bounds;
			clone.Selected = this.Selected;
			clone.Visible = this.Visible;
			clone.Position = this.Position;
			clone.TextColor = this.TextColor;
			clone.TextFont = this.TextFont;

			return clone;
		}


		#endregion

		#region Class Events

		/// <summary>
		/// Raise by <see cref="OnTextChanged"/> method.
		/// </summary>
		public event EventHandler TextChanged;

		/// <summary>
		/// Raise by <see cref="OnTextFontChanged"/> method.
		/// </summary>
		public event EventHandler TextFontChanged;

		/// <summary>
		/// Raise by <see cref="OnTextColorChanged"/> method.
		/// </summary>
		public event EventHandler TextColorChanged;

		#endregion

		#region Class Raisers

		private void RaiseTextChanged()
		{
			if( this.TextChanged != null )
			{
				this.TextChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseTextFontChanged()
		{
			if( this.TextFontChanged != null )
			{
				this.TextFontChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseTextColorChanged()
		{
			if( this.TextColorChanged != null )
			{
				this.TextColorChanged( this, EventArgs.Empty );
			}
		}

		
		protected virtual void OnTextChanged()
		{
			Invalidate();

			RaiseTextChanged();
		}

		protected virtual void OnTextFontChanged()
		{
			Invalidate();

			RaiseTextFontChanged();
		}

		protected virtual void OnTextColorChanged()
		{
			Invalidate();

			RaiseTextFontChanged();
		}


		#endregion
	}

	
	/// <summary>
	/// Represents an image primitive.
	/// </summary>
	[ 
	DesignTimeVisible( false ),
	ToolboxItem( false ) 
	]
	public class ImagePrimitive : Primitive
	{
		#region Class Members

		/// <summary>
		/// Displayed image.
		/// </summary>
		private Image m_image = null;

		/// <summary>
		/// Rotated image.
		/// </summary>
		private Image m_rotateImage = null;

		/// <summary>
		/// A value indicating whether the primitive is rotated.
		/// </summary>
		private bool m_bRotate = false;

		#endregion

		#region Class Properties

		/// <summary>
		/// Gets or set displayed image.
		/// </summary>
		[ 
		DefaultValue( null ),
		Description( "Displayed image." ),
		Category( "Appearance" )
		]
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
					OnImageChanged();
				}
			}
		}


		/// <summary>
		/// Gets or sets a value indicating whether the primitive is rotated.
		/// </summary>
		[ 
		DefaultValue( false ),
		Description( "Displayed text." ),
		Category( "Behavior" )
		]
		public bool Rotate
		{
			get
			{
				return m_bRotate;
			}
			set
			{
				if( value != m_bRotate )
				{
					m_bRotate = value;
					OnRotateChanged();
				}
			}
		}


		#endregion

		#region Class Initialize/Finalize Method

		public ImagePrimitive( Size size, Image image, GradientPanelExt owner ) : base( size, owner )
		{
			this.Image = image;
			this.PrimitiveBorderStyle = PrimitiveBorderStyle.None;
			this.BackColor = Color.Transparent;
		}

		public ImagePrimitive( Image image, GradientPanelExt owner ) : this( c_primitiveSize, image, owner )
		{}

		public ImagePrimitive( GradientPanelExt owner ) : this( c_primitiveSize, null, owner )
		{}

		public ImagePrimitive() : this( c_primitiveSize, null, null )
		{}


		#endregion

		#region Class Overrides

		protected override void DrawBackground( Graphics g )
		{
			base.DrawBackground( g );
			
			if( this.Image != null )
			{
				if( this.m_rotateImage == null )
				{
					g.DrawImage( this.Image, this.ClientRect );
				}
				else
				{
					g.DrawImage( this.m_rotateImage, this.ClientRect );
				}
			}
		}


		protected override void OnAlignmentChanged()
		{
			base.OnAlignmentChanged ();

			if( this.Rotate )
			{
				switch( this.Alignment )
				{
					case Alignment.Left :
					{
						this.m_rotateImage = ( Image )m_image.Clone();
						this.m_rotateImage.RotateFlip( RotateFlipType.Rotate270FlipNone );

						break;
					}
					case Alignment.Right :
					{
						this.m_rotateImage = ( Image )m_image.Clone();
						this.m_rotateImage.RotateFlip( RotateFlipType.Rotate90FlipNone );

						break;
					}
					default :
					{
						this.m_rotateImage = null;
						break;
					}
				}
			}
		}

		
		public override object Clone()
		{
			ImagePrimitive clone = new ImagePrimitive( this.Size, this.Image, this.OwnerControl );
			clone.Alignment = this.Alignment;
			clone.BackColor = this.BackColor;
			clone.BorderColor = this.BorderColor;
			clone.PrimitiveBorderStyle = this.PrimitiveBorderStyle;
			clone.Bounds = this.Bounds;
			clone.Selected = this.Selected;
			clone.Visible = this.Visible;
			clone.Position = this.Position;
			clone.Rotate = this.Rotate;

			return clone;
		}


		#endregion

		#region Class Events

		/// <summary>
		/// Raise by <see cref="OnImageChanged"/> method.
		/// </summary>
		public event EventHandler ImageChanged;

		/// <summary>
		/// Raise by <see cref="OnRotateChanged"/> method.
		/// </summary>
		public event EventHandler RotateChanged;

		#endregion

		#region Class Raisers

		private void RaiseImageChanged()
		{
			if( this.ImageChanged != null )
			{
				this.ImageChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseRotateChanged()
		{
			if( this.RotateChanged != null )
			{
				this.RotateChanged( this, EventArgs.Empty );
			}
		}

		protected virtual void OnImageChanged()
		{
			RaiseImageChanged();
		}

		protected virtual void OnRotateChanged()
		{
			RaiseRotateChanged();
		}

		#endregion
	}

	
	/// <summary>
	/// Represents a primitive which contain any control.
	/// </summary>
	[ 
	DesignTimeVisible( false ),
	ToolboxItem( false ) 
	]
	public class HostPrimitive : Primitive
	{
		#region Class Members

		/// <summary>
		/// Control which displaying.
		/// </summary>
		private Control m_hostControl = null;

		#endregion

		#region Class Properties
		
		/// <summary>
		/// Gets or set control which displaying inside the primitive.
		/// </summary>
		[
		Description( "The control which displaying inside the primitive." ),
		Category( "Behavior" ),
		DefaultValue( null )
		]
		public Control HostControl
		{
			get
			{
				return m_hostControl;
			}
			set
			{
				if( value != m_hostControl )
				{
					OnHostControlBeforeChanged();
					m_hostControl = value;
					OnHostControlChanged();
				}
			}
		}


		#endregion

		#region Class Initialize/Finalize Method

		public HostPrimitive( Control control, Size size, GradientPanelExt owner ) 
			: base( size, owner )
		{
			if( control != null )
			{
				this.HostControl = control;
				this.HostControl.Parent = this.OwnerControl;
			}

			this.PrimitiveBorderStyle = PrimitiveBorderStyle.None;
		}

		public HostPrimitive( Control control, GradientPanelExt owner ) : this( control, c_primitiveSize, owner )
		{}

		public HostPrimitive( GradientPanelExt owner ) : this( null, c_primitiveSize, owner )
		{}

		public HostPrimitive() : this( null, c_primitiveSize, null )
		{}

		#endregion

		#region Class Utility Methods

		/// <summary>
		/// Refresh location and size for host control.
		/// </summary>
		private void RefreshHostControlLoyaut()
		{
			if( this.OwnerControl != null && this.HostControl != null )
			{
				if( !this.OwnerControl.Controls.Contains( this.HostControl ) )
				{
					this.OwnerControl.Controls.Add( this.HostControl );
				}

				this.HostControl.Location = new Point( this.Bounds.Location.X, this.Bounds.Location.Y );
				this.HostControl.Size = new Size( this.Bounds.Size.Width, this.Bounds.Size.Height );
			}
		}

		#endregion

		#region Class Public Methods

		/// <summary>
		/// Determines if a primitive contains control.
		/// </summary>
		public bool IsContainControl( Control control )
		{
			bool result = false;

			if( control != null )
			{
				result = this.Bounds.IntersectsWith( control.Bounds );
			}

			return result;
		}


		#endregion

		#region Class Override

		protected override void OnSizeChanged()
		{
			base.OnSizeChanged ();

			RefreshHostControlLoyaut();
		}

		protected override void OnBoundsChanged()
		{
			base.OnBoundsChanged ();

			RefreshHostControlLoyaut();
		}


		public override object Clone()
		{
			HostPrimitive clone = new HostPrimitive( this.HostControl, this.Size, this.OwnerControl );
			clone.Alignment = this.Alignment;
			clone.BackColor = this.BackColor;
			clone.BorderColor = this.BorderColor;
			clone.PrimitiveBorderStyle = this.PrimitiveBorderStyle;
			clone.Bounds = this.Bounds;
			clone.Selected = this.Selected;
			clone.Visible = this.Visible;
			clone.Position = this.Position;
			clone.HostControl = this.HostControl;
			clone.Site = this.Site;

			return clone;
		}


		protected override void OnVisibleChanged()
		{
			base.OnVisibleChanged ();

			if( HostControl != null )
			{
				HostControl.Visible = ( this.Visible ) ? true : false;
			}
		}


		#endregion

		#region Class Events

		/// <summary>
		/// Raise by <see cref="OnHostControlChanged"/> method.
		/// </summary>
		public event EventHandler HostControlChanged;

		#endregion

		#region Class Raisers

		private void RaiseHostControlChanged()
		{
			if( this.HostControlChanged != null )
			{
				this.HostControlChanged( this, EventArgs.Empty );
			}
		}

		protected virtual void OnHostControlChanged()
		{
			RefreshHostControlLoyaut();

			RaiseHostControlChanged();
		}

		private void OnHostControlBeforeChanged()
		{
			if( this.HostControl != null && this.OwnerControl != null )
			{
				this.OwnerControl.Controls.Remove( this.HostControl );
			}
		}

		#endregion
	}

	
	/// <summary>
	/// Border style for Primitive.
	/// </summary>
	public enum PrimitiveBorderStyle
	{
		// single line
		Single,

		// no line
		None
	}

	
	/// <summary>
	/// Alignment primitive within control.
	/// </summary>
	public enum Alignment
	{
		Left,
		Right,
		Top,
		Bottom
	}

	/// <summary>
	/// Types of the Primitives.
	/// </summary>
	public enum PrimitiveTypes
	{
		Collapse = 0,
		Image = 1,
		Text = 2,
		Host = 3
	}
}
