#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
//  Author: Jeff Boenig
//
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
	public abstract class Ruler
		: PropertyContainer
	{
		#region Class members
		/// <summary>
		/// Ruler location ( in pixels ).
		/// </summary>
		private Point m_ptLocation;
		/// <summary>
		/// Ruler size ( in pixels ).
		/// </summary>
		private Size m_szSize;
		/// <summary>
		/// Color used to fill background.
		/// </summary>
		private Color m_clrBkgnd;
		/// <summary>
		/// Color used to draw major lines.
		/// </summary>
		private Color m_clrMajorLines;
		/// <summary>
		/// Color used to draw minor lines.
		/// </summary>
		private Color m_clrMinorLines;
		/// <summary>
		/// Color used to draw highlight.
		/// </summary>
		private Color m_clrHighlightColor = Color.Empty;
		/// <summary>
		/// Color used to draw marker ( current cursor position ).
		/// </summary>
		private Color m_clrMarker;
		/// <summary>
		/// Text style.
		/// </summary>
		private FontStyle m_styleFont;
		/// <summary>
		/// Defines current marker location( in viewer coordinates ).
		/// </summary>
		private int m_ptMarkerLocation;
		/// <summary>
		/// Defines current highlight area position.
		/// </summary>
		private int m_nHighlightAreaStart;
		/// <summary>
		/// Defines current highlight area size.
		/// </summary>
		private int m_nHighlightAreaWidth;
		/// <summary>
		/// Renderer used to render markup.
		/// </summary>
		internal RulerMarkupRenderer m_markupRenderer;

		#endregion
		
		#region Class initialize/finalize methods
		public Ruler()
		{
			m_szSize = Size.Empty;
			m_ptLocation = Point.Empty;
			m_clrBkgnd = Color.WhiteSmoke;
			m_clrMajorLines = Color.Black;
			m_clrMinorLines = Color.Black;
			//m_clrHighlightColor = Color.LightGray;
			HighlightColor = Color.LightGray;
			m_clrMarker = Color.Red;
			m_markupRenderer = CreateMarkupRenderer(this.MeasureUnit);
		}
		public Ruler( Ruler src )
			: base( src )
		{
			m_szSize = src.m_szSize;
			m_clrBkgnd = src.m_clrBkgnd;
			m_clrMajorLines = src.m_clrMajorLines;
			m_clrMinorLines = src.m_clrMinorLines;
			m_clrHighlightColor = src.m_clrHighlightColor;
			m_clrMarker = src.m_clrMarker;
			m_markupRenderer = CreateMarkupRenderer(this.MeasureUnit);
		}
		public Ruler( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
			m_szSize = ( Size ) info.GetValue( "size", typeof( Size ) );
			m_clrBkgnd = ( Color ) info.GetValue( "backgroundColor", typeof( Color ) );
			m_clrMajorLines = ( Color ) info.GetValue( "majorLinesColor", typeof( Color ) );
			m_clrMinorLines = ( Color ) info.GetValue( "minorLinesColor", typeof( Color ) );
			m_clrHighlightColor = ( Color ) info.GetValue( "highlightColor", typeof( Color ) );
			m_clrMarker = ( Color ) info.GetValue( "cursorMarkerColor", typeof( Color ) );
			m_markupRenderer = CreateMarkupRenderer(this.MeasureUnit);
		}
		#endregion
		
		#region Class properties
		[ Browsable( false ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		public Point Location
		{
			get{ return m_ptLocation; }
			set
			{
				if( m_ptLocation != value )
				{
					m_ptLocation = value;
				}
			}
		}
		[ Browsable( false ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		public Size Size
		{
			get{ return m_szSize; }
			set
			{
				if( m_szSize != value )
				{
					m_szSize = value;
				}
			}
		}
		[ Browsable( true ) ]
		[ Description( "Color used to fill ruler background." ) ]
		[ DefaultValue( typeof( Color ), "Color.LightSkyBlue" ) ]
		public Color BackgroundColor
		{
			get{ return m_clrBkgnd; }
			set
			{
				if( m_clrBkgnd != value && OnPropertyChanging( DPN.BackgroundColor, value ) )
				{
					// assign new value
					m_clrBkgnd = value;
					// raise PropertyChanged
					OnPropertyChanged( DPN.BackgroundColor );
				}
			}
		}
		[ Browsable( true ) ]
		[ Description( "Color used to draw major lines." ) ]
		[ DefaultValue( typeof( Color ), "Color.Black" ) ]
		public Color MajorLinesColor
		{
			get{ return m_clrMajorLines; }
			set
			{
				if( m_clrMajorLines != value && OnPropertyChanging( DPN.MajorLinesColor, value ) )
				{
					// assign new value
					m_clrMajorLines = value;
					// raise PropertyChanged
					OnPropertyChanged( DPN.MajorLinesColor );
				}
			}
		}
		[ Browsable( true ) ]
		[ Description( "Color used to draw minor lines." ) ]
		[ DefaultValue( typeof( Color ), "Color.Black" ) ]
		public Color MinorLinesColor
		{
			get{ return m_clrMinorLines; }
			set
			{
				if( m_clrMinorLines != value )
				{
					m_clrMinorLines = value;
				}
			}
		}
		[ Browsable( true ) ]
		[ Description( "Color used to fill highlight area." ) ]
//		[ DefaultValue( typeof( Color ), "Color.Gray" ) ]
		public Color HighlightColor
		{
			get{ return m_clrHighlightColor; }
			set
			{
				if( m_clrHighlightColor != value )
				{
					//m_clrHighlightColor = value;
					m_clrHighlightColor = Color.FromArgb(150, value);
				}
			}
		}
		[ Browsable( true ) ]
		[ Description( "Color used to draw cursor marker." ) ]
		[ DefaultValue( typeof( Color ), "Color.Red" ) ]
		public Color MarkerColor
		{
			get{ return m_clrMarker; }
			set
			{
				if( m_clrMarker != value )
				{
					m_clrMarker = value;
				}
			}
		}
		[ Browsable( true ) ]
		public FontStyle TextStyle
		{
			get
			{
				if( m_styleFont == null )
				{
					m_styleFont = new FontStyle();
				}
				
				 return m_styleFont;
			}
		}
		[ Browsable( false ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		public int MarkerPosition
		{
			get{ return m_ptMarkerLocation; }
			set
			{ m_ptMarkerLocation = value; }
		}
		[ Browsable( false ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		public int HighlightAreaStart
		{
			get{ return m_nHighlightAreaStart; }
			set{ m_nHighlightAreaStart = value; }
		}
		[ Browsable( false ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		public int HighlightAreaWidth
		{
			get{ return m_nHighlightAreaWidth; }
			set{ m_nHighlightAreaWidth = value; }
		}
		[ Browsable( false ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		public override MeasureUnits MeasureUnit
		{
			get
			{
				return base.MeasureUnit;
			}
			set
			{
				if( base.MeasureUnit != value )
				{
					base.MeasureUnit = value;

					// TODO : create appropriate markuprnderer
					m_markupRenderer = CreateMarkupRenderer( this.MeasureUnit );

				}
			}
		}

		[ Browsable( false ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		public override bool InheritContainerMeasureUnits
		{
			get{ return base.InheritContainerMeasureUnits; }
			set{ base.InheritContainerMeasureUnits = value; }
		}
		#endregion
		
		#region Class public methods
		public abstract void Draw( Graphics gfx );
		#endregion
		
		#region Class overrides
		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData( info, context );
			
			info.AddValue( "size", m_szSize );
			info.AddValue( "backgroundColor", m_clrBkgnd );
			info.AddValue( "majorLinesColor", m_clrMajorLines );
			info.AddValue( "minorLinesColor", m_clrMinorLines );
			info.AddValue( "highlightColor", m_clrHighlightColor );
			info.AddValue( "cursorMarkerColor", m_clrMarker );
		}
		public override void UpdateServiceReferences( IServiceReferenceProvider provider )
		{
			base.UpdateServiceReferences( provider );
			
			if( provider == null )
			{
				m_propertyObserver = null;
			}
			else
			{
				m_propertyObserver = ( IPropertyObserver ) provider.ProvideServiceReference( typeof( IPropertyObserver ) );
			}
		}


		/// <summary>
		/// Create Markup Renderer for Diagram Measure Units
		/// </summary>
		/// <param name="measureUnits"></param>
		/// <returns></returns>
		private RulerMarkupRenderer CreateMarkupRenderer (MeasureUnits measureUnits)
		{

			if ( measureUnits == MeasureUnits.Millimeter || measureUnits == MeasureUnits.Centimeter || measureUnits == MeasureUnits.Meter || measureUnits == MeasureUnits.Kilometer )
			{
				return new CentimetersMarkupRenderer(this);
			}

			if ( measureUnits == MeasureUnits.Inch || measureUnits == MeasureUnits.SixteenthInch || measureUnits == MeasureUnits.QuarterInch || measureUnits == MeasureUnits.HalfInch || measureUnits == MeasureUnits.EighthInch || measureUnits == MeasureUnits.Foot || measureUnits == MeasureUnits.Yard /*|| measureUnits == MeasureUnits.Mile*/ )
			{
				return new InchMarkupRenderer(this);
			}

			if ( measureUnits == MeasureUnits.Pixel )
			{
				return new PixelMarkupRenderer(this) ;
			}

			if ( measureUnits == MeasureUnits.Display )
			{
				return new DisplayMarkupRenderer(this) ;
			}

			if ( measureUnits == MeasureUnits.Point )
			{
				return new PointsMarkupRenderer(this) ;
			}

			if ( measureUnits == MeasureUnits.Document )
			{
				return new DocumentMarkupRenderer(this) ;
			}

			if ( measureUnits == MeasureUnits.Mile )
			{
				return new MileMarkupRenderer(this);
			}



			return new PixelMarkupRenderer(this) ;
		}

		#endregion
	}
	
	public class HorizontalRuler
		: Ruler
	{
		#region Class initialize/finalize methods
		public HorizontalRuler()
			: base()
		{}
		public HorizontalRuler( HorizontalRuler src )
			: base( src )
		{}
		public HorizontalRuler( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{}
		#endregion
		
		#region Class overrides
		public override void Draw( Graphics gfx )
		{
			float realPageScale = gfx.PageScale;

			GraphicsState save = gfx.Save();
			gfx.PageScale = 1f;
			//this.Size = new Size(this.Size.Width , (int)(this.Size.Height / gfx.PageScale));

			// fill background
			using( Brush brushBkgnd = new SolidBrush( this.BackgroundColor ))
			{
				gfx.FillRectangle( brushBkgnd, this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height/* / gfx.PageScale*/ );
			}

//			gfx.PageScale = 1f;

			m_markupRenderer.RenderMarkup(gfx, realPageScale);

//			gfx.Restore( gState );
			

			// Draw current tool work area
			if( this.HighlightAreaWidth > 0 )
			{
				using( Brush brushHighlight = new SolidBrush( this.HighlightColor ) )
				{
					System.Drawing.Rectangle rectHighlight = System.Drawing.Rectangle.Empty;
					rectHighlight.Location = new Point( this.HighlightAreaStart, this.Location.Y );
					rectHighlight.Size = new Size( this.HighlightAreaWidth, /*(int)(*/this.Size.Height /*/gfx.PageScale)*/ );
					
					gfx.FillRectangle( brushHighlight, rectHighlight );
				}
			}
			
			//draw markup

			if( this.Location.X < this.MarkerPosition && this.MarkerPosition < ( this.Location.X + this.Size.Width ) )
			{
				using( Pen pen = new Pen( this.MarkerColor, 0f ) )
				{
					gfx.DrawLine( pen, this.MarkerPosition, 0, this.MarkerPosition, /*(int)(*/this.Size.Height /*/gfx.PageScale)*/ );
				}
			}
			

			// outline border
			using( Pen pen = new Pen( Color.Black, 0f ) )
			{
				gfx.DrawRectangle( pen, this.Location.X, this.Location.Y, this.Size.Width - 1, /*(int)(*/this.Size.Height/* /gfx.PageScale)*/ - 1 );
			}

			gfx.Restore(save);
		}
		public override object Clone()
		{
			return new HorizontalRuler( this );
		}
		protected override string GetPropertyContainerName()
		{
			return DPN.HorizontalRuler;
		}
		#endregion
	}

	public class VerticalRuler
		: Ruler
	{
		#region Class initialize/finalize methods
		public VerticalRuler()
			: base()
		{}
		public VerticalRuler( VerticalRuler src )
			: base( src )
		{}
		public VerticalRuler( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{}
		#endregion
		
		#region Class overrides
		public override void Draw( Graphics gfx )
		{
			float realPageScale = gfx.PageScale;

			GraphicsState save = gfx.Save();
			gfx.PageScale = 1f;
			// fill background
			using( Brush brushBkgnd = new SolidBrush( this.BackgroundColor ) )
			{
				gfx.FillRectangle( brushBkgnd, this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height );
			}

			m_markupRenderer.RenderMarkup( gfx, 1 );

			// Draw current tool work area
			if( this.HighlightAreaWidth > 0 )
			{
				using( Brush brushHighlight = new SolidBrush( this.HighlightColor ) )
				{
					System.Drawing.Rectangle rectHighlight = System.Drawing.Rectangle.Empty;
					rectHighlight.Location = new Point( this.Location.X, this.HighlightAreaStart );
					rectHighlight.Size = new Size( this.Size.Width, this.HighlightAreaWidth );
					
					gfx.FillRectangle( brushHighlight, rectHighlight );
				}
			}
			
			if( this.Location.Y < this.MarkerPosition && this.MarkerPosition < ( this.Location.Y + this.Size.Height ) )
			{
				// Draw cursor marker
				using( Pen pen = new Pen( this.MarkerColor, 0f ) )
				{
					gfx.DrawLine( pen, 0, this.MarkerPosition, this.Size.Width, this.MarkerPosition );
				}
			}
			
			// outline border
			using( Pen pen = new Pen( Color.Black, 0f ) )
			{
				gfx.DrawRectangle( pen, this.Location.X, this.Location.Y, this.Size.Width - 1, this.Size.Height - 1 );
			}

			gfx.Restore(save);
		}
		public override object Clone()
		{
			return new VerticalRuler( this );
		}
		protected override string GetPropertyContainerName()
		{
			return DPN.VerticalRuler;
		}
		#endregion
	}
	
	
	public abstract class RulerMarkupRenderer
	{
		#region Class members
		private Ruler m_ruler;
		private PointF m_nDocumentOrigin;
		private float m_fZoomLevel;
		#endregion

		#region Class initialize/finalize methods
		public RulerMarkupRenderer( Ruler ruler )
		{
			if( ruler == null )
			{
				throw new ArgumentNullException( "ruler" );
			}
			
			m_ruler = ruler;
			m_fZoomLevel = 1.0f;
		}
		#endregion
		
		#region Class properties

		public float ZoomLevel
		{
			get{ return m_fZoomLevel; }
		}

		public PointF DocumentOrigin
		{
			get{ return m_nDocumentOrigin; }
			set{ m_nDocumentOrigin = value; }
		}
		#endregion
		
		#region Class public methods
		public abstract void RenderMarkup( Graphics gfx, float realPageScale );
		#endregion
	}
	
	/// <summary>
	/// Markup in Pixels
	/// </summary>
	public class PixelMarkupRenderer
		: RulerMarkupRenderer
	{
		Ruler m_ruler = null;

		#region Class initialize/finalize methods
		public PixelMarkupRenderer( Ruler ruler )
			: base( ruler )
		{
			m_ruler = ruler;
		}
		#endregion
		
		#region Class overrides
 		public override void RenderMarkup( Graphics gfx, float realPageScale )
		{
			//float fWidth

			gfx.FillRectangle(new SolidBrush( Color.FromArgb(253, 255, 232) ), m_ruler.Location.X, m_ruler.Location.Y, m_ruler.Size.Width, m_ruler.Size.Height/* * gfx.PageScale */);

			// if Horizontal Ruler .....
			if ( m_ruler.Size.Width > m_ruler.Size.Height )
			{
				DrawHorizontalRuler( gfx, DocumentOrigin.X, realPageScale );
			}
			else
			{
				DrawVerticalRulers( gfx, DocumentOrigin.Y, realPageScale );
			}
		}


		#endregion

		#region Helper Methods

		/// <summary>
		/// Draw Horizontal Ruler
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawHorizontalRuler( Graphics gfx, float horizontalOrigin, float realPageScale )
		{

			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int nRulerHeight = m_ruler.Size.Height;

			horizontalOrigin *= realPageScale;

//			// draw Minor lines
//			int incStep = (int)(10 * realPageScale);
//			for (int i = m_ruler.Location.X - (int)horizontalOrigin ; i <= m_ruler.Size.Width + 20; i += incStep/*10*/ )
//			{
//				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, /*m_ruler.Size.Height*/nRulerHeight / 2 - 2 );	
//			}



			// draw Minor lines
			float incStep = 10 * realPageScale;
			for (float i = m_ruler.Location.X - horizontalOrigin ; i <= m_ruler.Size.Width + 20; i += incStep/*10*/ )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, /*m_ruler.Size.Height*/nRulerHeight / 2 - 2 );	
			}

//			// draw  lines
//			incStep = (int)(50 * realPageScale);
//			for (int i = m_ruler.Location.X - (int)horizontalOrigin; i <= m_ruler.Size.Width + 20; i += incStep/*50*/)
//			{
//				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, /*m_ruler.Size.Height*/nRulerHeight / 2 );	
//			}

			// draw Major lines
			incStep = 100 * realPageScale;
			//int incStep2 = (int)(100 * realPageScale);
			for (float i = m_ruler.Location.X - /*(int)*/horizontalOrigin; i <= m_ruler.Size.Width /*- (int)horizontalOrigin*/; i += incStep/*100*/ )
			{
//				float fontSize = m_ruler.Size.Height/2 - 3;

				float fontSize = nRulerHeight/2 - 3;

				Font font = new Font(FontFamily.GenericSansSerif, fontSize );

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), i , 0, i , nRulerHeight - 5 /* * gfx.PageScale*/);	
				gfx.DrawString((Math.Round((i - 20 + horizontalOrigin) / realPageScale)).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), i + 1, /*m_ruler.Size.Height*/nRulerHeight - MeasureUnitsConverter.ToPixelY( fontSize, MeasureUnits.Point ) - 3/* * gfx.PageScale */);

			}			
		}


		/// <summary>
		/// Draw Vertical Ruler
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawVerticalRulers( Graphics gfx, float verticalOrigin, float realPageScale  )
		{
			// TODO: Implement drawing
			
			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int nRulerWidth = m_ruler.Size.Width;

			verticalOrigin *= realPageScale;

			// draw Minor lines
			float incStep = 10 * realPageScale;
			for (float i = m_ruler.Location.Y - verticalOrigin; i <= m_ruler.Size.Height + 20; i +=incStep )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, nRulerWidth / 2 - 2, i );	
			}

//			// draw  lines
//			for (int i = m_ruler.Location.Y - (int)verticalOrigin; i <= m_ruler.Size.Height + 20; i +=50 )
//			{
//				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, nRulerWidth / 2, i );	
//			}

			// draw Major lines
			incStep = 100 * realPageScale;
			for (float i = m_ruler.Location.Y - verticalOrigin; i <= m_ruler.Size.Height; i +=incStep )
			{

				float fontSize = nRulerWidth/2 - 3;
				Font font = new Font(FontFamily.GenericSansSerif, fontSize);

				StringFormat drawFormat = new StringFormat();
				drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), 0, i, nRulerWidth - 5, i );	
				gfx.DrawString((Math.Round((i - 20 + verticalOrigin)/ realPageScale)).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), nRulerWidth - MeasureUnitsConverter.ToPixelX( fontSize, MeasureUnits.Point ) - 4 , i + 1, drawFormat ); 

			}

		}

		#endregion
	}


	/// <summary>
	/// Markup in Inches
	/// </summary>
	public class InchMarkupRenderer
		: RulerMarkupRenderer
	{
		Ruler m_ruler = null;

		#region Class initialize/finalize methods
		public InchMarkupRenderer( Ruler ruler )
			: base( ruler )
		{
			m_ruler = ruler;
		}
		#endregion
		
		#region Class overrides
		public override void RenderMarkup( Graphics gfx, float realPageScale )
		{
			//float fWidth

			gfx.FillRectangle(new SolidBrush( Color.FromArgb(253, 255, 232) ), m_ruler.Location.X, m_ruler.Location.Y, m_ruler.Size.Width, m_ruler.Size.Height);



			// if Horizontal Ruler .....
			if ( m_ruler.Size.Width > m_ruler.Size.Height )
			{
				DrawHorizontalRuler( gfx, DocumentOrigin.X, realPageScale );
			}
			else
			{
				DrawVerticalRulers( gfx, DocumentOrigin.Y, realPageScale );
			}

		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Draw Horizontal Rullers
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawHorizontalRuler( Graphics gfx, float horizontalOrigin, float realPageScale )
		{
			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			horizontalOrigin *= realPageScale;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(1, MeasureUnits.Inch);

			// draw Minor lines
			//float incStep = koef/16/* * realPageScale*/;
			float incStep = 0;

			if ( realPageScale * 100 != 25 && ((realPageScale * 100) % 50 != 0 || (realPageScale * 100) % 150 == 0))
			{
				incStep = koef/16 * realPageScale;
			}
			else
			{
				incStep = koef/16 /** realPageScale*/;
			}

			for (float i = m_ruler.Location.X  - horizontalOrigin; i <= m_ruler.Size.Width + 20; i +=/*koef/16*/incStep )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 - 2 );	
			}

			// draw  lines
			//incStep = koef/2/* * realPageScale*/;
			
			if ( realPageScale * 100 != 25 && ((realPageScale * 100) % 50 != 0 || (realPageScale * 100) % 150 == 0))
			{
				incStep = koef/2 * realPageScale;
			}
			else
			{
				incStep = koef/2 /** realPageScale*/;
			}

			for (float i = m_ruler.Location.X  - horizontalOrigin; i <= m_ruler.Size.Width + 20; i +=/*koef/2*/incStep )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 );	
			}


			float tempScale = realPageScale; // for test only
			if ((realPageScale * 100) % 25 != 0 )
			{
				// TODO:
				if ( realPageScale > 2 )
				{
					tempScale = realPageScale % 1 + 1;
				}
			}

			if ( /*realPageScale*/tempScale * 100 != 25 && ((/*realPageScale*/tempScale * 100) % 50 != 0 || (/*realPageScale*/tempScale * 100) % 150 == 0))
			{
				incStep = koef * /*realPageScale*/tempScale;
			}
			else
			{
				incStep = koef /** realPageScale*/;
			}
			

//			float tempScale = realPageScale; // for test only
//			if ((realPageScale * 100) % 25 != 0 )
//			{
//				// TODO:
//				if ( realPageScale > 2 )
//				{
//					tempScale = realPageScale % 1;
//				}
//			}

			for (float i = m_ruler.Location.X - horizontalOrigin ; i <= m_ruler.Size.Width; i += /*koef*/incStep )
			{

				float fontSize = m_ruler.Size.Height/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height - 5);	
				gfx.DrawString(Math.Round(((i - 20 + horizontalOrigin)/koef/ /*tempScale*/realPageScale), 1).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), i + 1, m_ruler.Size.Height - MeasureUnitsConverter.ToPixelY( fontSize, MeasureUnits.Point ) - 2 );

			}			
		}


		/// <summary>
		/// Draw Vertical Ruler
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawVerticalRulers( Graphics gfx, float verticalOrigin, float realPageScale )
		{
			// TODO: Implement drawing

			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			verticalOrigin *= realPageScale;

			int koef = ( int ) MeasureUnitsConverter.ToPixelY(1, MeasureUnits.Inch);

			// draw Minor lines
			//float incStep = koef/16 * realPageScale;

			float incStep = 0;

			if ( realPageScale * 100 != 25 && ((realPageScale * 100) % 50 != 0 || (realPageScale * 100) % 150 == 0))
			{
				incStep = koef/16 * realPageScale;
			}
			else
			{
				incStep = koef/16 /** realPageScale*/;
			}

			for (float i = m_ruler.Location.Y - verticalOrigin; i <= m_ruler.Size.Height + 20; i += incStep )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2 - 2, i );	
			}

			// draw  lines
			//incStep = koef/2 * realPageScale;

			if ( realPageScale * 100 != 25 && ((realPageScale * 100) % 50 != 0 || (realPageScale * 100) % 150 == 0))
			{
				incStep = koef/2 * realPageScale;
			}
			else
			{
				incStep = koef/2 /** realPageScale*/;
			}

			for (float i = m_ruler.Location.Y - verticalOrigin; i <= m_ruler.Size.Height + 20; i += incStep )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2, i );	
			}

			// draw Major lines
			//incStep = koef * realPageScale;

			if ( realPageScale * 100 != 25 && ((realPageScale * 100) % 50 != 0 || (realPageScale * 100) % 150 == 0))
			{
				incStep = koef * realPageScale;
			}
			else
			{
				incStep = koef /** realPageScale*/;
			}

			for (float i = m_ruler.Location.Y - verticalOrigin; i <= m_ruler.Size.Height; i += incStep )
			{

				float fontSize = m_ruler.Size.Width/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				StringFormat drawFormat = new StringFormat();
				drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), 0, i, m_ruler.Size.Width - 5, i );	
				gfx.DrawString(Math.Round(((i - 20 + verticalOrigin)/koef / realPageScale), 1).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), m_ruler.Size.Width - MeasureUnitsConverter.ToPixelX( fontSize, MeasureUnits.Point ) - 2, i + 1, drawFormat ); 

			}

		}

		#endregion
	}



	/// <summary>
	/// Markup in Inches
	/// </summary>
	public class MileMarkupRenderer
		: RulerMarkupRenderer
	{
		Ruler m_ruler = null;

		#region Class initialize/finalize methods
		public MileMarkupRenderer( Ruler ruler )
			: base( ruler )
		{
			m_ruler = ruler;
		}
		#endregion
		
		#region Class overrides
		public override void RenderMarkup( Graphics gfx, float realPageScale )
		{
			//float fWidth

			gfx.FillRectangle(new SolidBrush( Color.FromArgb(253, 255, 232) ), m_ruler.Location.X, m_ruler.Location.Y, m_ruler.Size.Width, m_ruler.Size.Height);



			// if Horizontal Ruler .....
			if ( m_ruler.Size.Width > m_ruler.Size.Height )
			{
				DrawHorizontalRuler( gfx, DocumentOrigin.X );
			}
			else
			{
				DrawVerticalRulers( gfx );
			}

		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Draw Horizontal Rullers
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawHorizontalRuler( Graphics gfx, float horizontalOrigin )
		{
			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(0.00001f, MeasureUnits.Mile);

			// draw Minor lines
			for (int i = m_ruler.Location.X - (int)horizontalOrigin; i <= m_ruler.Size.Width + 20; i +=koef/16 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 - 2 );	
			}

			// draw  lines
			for (int i = m_ruler.Location.X - (int)horizontalOrigin; i <= m_ruler.Size.Width + 20; i +=koef/2 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 );	
			}


			float temp = koef * 1000000f;
			for (int i = m_ruler.Location.X - (int)horizontalOrigin; i <= m_ruler.Size.Width; i += koef )
			{

				float fontSize = m_ruler.Size.Height/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				float test = MeasureUnitsConverter.FromPixelX(6000000, MeasureUnits.Mile);

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height - 5);	
				float f = ((i - 20 + horizontalOrigin) /temp);
				string str = string.Format("{0:#0.000000}", f);
				gfx.DrawString(str, font, new SolidBrush( m_ruler.MajorLinesColor ), i + 1, m_ruler.Size.Height - MeasureUnitsConverter.ToPixelY( fontSize, MeasureUnits.Point ) - 2 );

			}			
		}


		/// <summary>
		/// Draw Vertical Ruler
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawVerticalRulers( Graphics gfx )
		{
			// TODO: Implement drawing

			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelY(1, MeasureUnits.Inch);

			// draw Minor lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef/16 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2 - 2, i );	
			}

			// draw  lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef/2 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2, i );	
			}

			// draw Major lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height; i += koef )
			{

				float fontSize = m_ruler.Size.Width/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				StringFormat drawFormat = new StringFormat();
				drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), 0, i, m_ruler.Size.Width - 5, i );	
				gfx.DrawString(((i - 20)/koef).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), m_ruler.Size.Width - MeasureUnitsConverter.ToPixelX( fontSize, MeasureUnits.Point ) - 2, i + 1, drawFormat ); 

			}

		}

		#endregion
	}


	/// <summary>
	/// Markup in Centimeters
	/// </summary>
	public class CentimetersMarkupRenderer
		: RulerMarkupRenderer
	{
		Ruler m_ruler = null;

		#region Class initialize/finalize methods
		public CentimetersMarkupRenderer( Ruler ruler )
			: base( ruler )
		{
			m_ruler = ruler;
		}
		#endregion
		
		#region Class overrides
		public override void RenderMarkup( Graphics gfx, float realPageScale )
		{
			//float fWidth

			gfx.FillRectangle(new SolidBrush( Color.FromArgb(253, 255, 232) ), m_ruler.Location.X, m_ruler.Location.Y, m_ruler.Size.Width, m_ruler.Size.Height);



			// if Horizontal Ruler .....
			if ( m_ruler.Size.Width > m_ruler.Size.Height )
			{
				DrawHorizontalRuler( gfx );
			}
			else
			{
				DrawVerticalRulers( gfx );
			}

		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Draw Horizontal Rullers
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawHorizontalRuler( Graphics gfx )
		{
			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(0.1F, MeasureUnits.Centimeter);

			// draw Minor lines
			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i +=koef )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 - 2 );
			}

			// draw  lines
			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i +=koef*5 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 );	
			}


			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i += koef*10 )
			{

				float fontSize = m_ruler.Size.Height/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height - 5);	
				gfx.DrawString(((i - 20)/koef/10).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), i + 1, m_ruler.Size.Height - MeasureUnitsConverter.ToPixelY( fontSize, MeasureUnits.Point ) - 2 );

			}			
		}


		/// <summary>
		/// Draw Vertical Ruler
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawVerticalRulers( Graphics gfx )
		{
			// TODO: Implement drawing

			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(0.1F, MeasureUnits.Centimeter);

			// draw Minor lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2 - 2, i );	
			}

			// draw  lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef * 5 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2, i );	
			}

			// draw Major lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height; i += koef * 10)
			{

				float fontSize = m_ruler.Size.Width/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				StringFormat drawFormat = new StringFormat();
				drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), 0, i, m_ruler.Size.Width - 5, i );	
				gfx.DrawString(((i - 20)/koef/10).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), m_ruler.Size.Width - MeasureUnitsConverter.ToPixelX( fontSize, MeasureUnits.Point ) - 2, i + 1, drawFormat ); 

			}

		}

		#endregion
	}


	/// <summary>
	/// Markup in Millimeters
	/// </summary>
	public class MillimetersMarkupRenderer
		: RulerMarkupRenderer
	{
		Ruler m_ruler = null;

		#region Class initialize/finalize methods
		public MillimetersMarkupRenderer( Ruler ruler )
			: base( ruler )
		{
			m_ruler = ruler;
		}
		#endregion
		
		#region Class overrides
		public override void RenderMarkup( Graphics gfx, float realPageScale )
		{
			//float fWidth

			gfx.FillRectangle(new SolidBrush( Color.FromArgb(253, 255, 232) ), m_ruler.Location.X, m_ruler.Location.Y, m_ruler.Size.Width, m_ruler.Size.Height);



			// if Horizontal Ruler .....
			if ( m_ruler.Size.Width > m_ruler.Size.Height )
			{
				DrawHorizontalRuler( gfx );
			}
			else
			{
				DrawVerticalRulers( gfx );
			}

		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Draw Horizontal Rullers
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawHorizontalRuler( Graphics gfx )
		{
			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(0.1F, MeasureUnits.Centimeter);

			// draw Minor lines
			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i +=koef )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 - 2 );
			}

			// draw  lines
			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i +=koef*5 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 );	
			}


			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i += koef*10 )
			{

				float fontSize = m_ruler.Size.Height/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height - 5);	
				gfx.DrawString(((i - 20)/koef).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), i + 1, m_ruler.Size.Height - MeasureUnitsConverter.ToPixelY( fontSize, MeasureUnits.Point ) - 2 );

			}			
		}


		/// <summary>
		/// Draw Vertical Ruler
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawVerticalRulers( Graphics gfx )
		{
			// TODO: Implement drawing

			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(0.1F, MeasureUnits.Centimeter);

			// draw Minor lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2 - 2, i );	
			}

			// draw  lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef * 5 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2, i );	
			}

			// draw Major lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height; i += koef * 10)
			{

				float fontSize = m_ruler.Size.Width/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				StringFormat drawFormat = new StringFormat();
				drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), 0, i, m_ruler.Size.Width - 5, i );	
				gfx.DrawString(((i - 20)/koef).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), m_ruler.Size.Width - MeasureUnitsConverter.ToPixelX( fontSize, MeasureUnits.Point ) - 2, i + 1, drawFormat ); 

			}

		}

		#endregion
	}


	/// <summary>
	/// Markup in Meters
	/// </summary>
	public class MetersMarkupRenderer
		: RulerMarkupRenderer
	{
		Ruler m_ruler = null;

		#region Class initialize/finalize methods
		public MetersMarkupRenderer( Ruler ruler )
			: base( ruler )
		{
			m_ruler = ruler;
		}
		#endregion
		
		#region Class overrides
		public override void RenderMarkup( Graphics gfx, float realPageScale )
		{
			//float fWidth

			gfx.FillRectangle(new SolidBrush( Color.FromArgb(253, 255, 232) ), m_ruler.Location.X, m_ruler.Location.Y, m_ruler.Size.Width, m_ruler.Size.Height);



			// if Horizontal Ruler .....
			if ( m_ruler.Size.Width > m_ruler.Size.Height )
			{
				DrawHorizontalRuler( gfx );
			}
			else
			{
				DrawVerticalRulers( gfx );
			}

		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Draw Horizontal Rullers
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawHorizontalRuler( Graphics gfx )
		{
			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(0.1F, MeasureUnits.Centimeter);

			// draw Minor lines
			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i +=koef )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 - 2 );
			}

			// draw  lines
			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i +=koef*5 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 );	
			}

			int counter = 0;

			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i += koef*10 )
			{
				float fontSize = m_ruler.Size.Height/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height - 5);
				if ( counter == 10 || counter == 0 )
				{
                    gfx.DrawString((((float)i - 20)/koef/1000).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), i + 1, m_ruler.Size.Height - MeasureUnitsConverter.ToPixelY( fontSize, MeasureUnits.Point ) - 2 );
					counter = 0;
				}
				counter++;

			}			
		}


		/// <summary>
		/// Draw Vertical Ruler
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawVerticalRulers( Graphics gfx )
		{
			// TODO: Implement drawing

			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(0.1F, MeasureUnits.Centimeter);

			// draw Minor lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2 - 2, i );	
			}

			// draw  lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef * 5 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2, i );	
			}

			// draw Major lines
			int counter = 0;

			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height; i += koef * 10)
			{

				float fontSize = m_ruler.Size.Width/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				StringFormat drawFormat = new StringFormat();
				drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), 0, i, m_ruler.Size.Width - 5, i );	

				if ( counter == 10 || counter == 0 )
				{
					gfx.DrawString((((float)i - 20)/koef/1000).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), m_ruler.Size.Width - MeasureUnitsConverter.ToPixelX( fontSize, MeasureUnits.Point ) - 2, i + 1, drawFormat ); 
					counter = 0;
				}
				counter++;

			}

		}

		#endregion
	}

	/// <summary>
	/// Markup in Points
	/// </summary>
	public class PointsMarkupRenderer
		: RulerMarkupRenderer
	{
		Ruler m_ruler = null;

		#region Class initialize/finalize methods
		public PointsMarkupRenderer( Ruler ruler )
			: base( ruler )
		{
			m_ruler = ruler;
		}
		#endregion
		
		#region Class overrides
		public override void RenderMarkup( Graphics gfx, float realPageScale )
		{
			//float fWidth

			gfx.FillRectangle(new SolidBrush( Color.FromArgb(253, 255, 232) ), m_ruler.Location.X, m_ruler.Location.Y, m_ruler.Size.Width, m_ruler.Size.Height);



			// if Horizontal Ruler .....
			if ( m_ruler.Size.Width > m_ruler.Size.Height )
			{
				DrawHorizontalRuler( gfx );
			}
			else
			{
				DrawVerticalRulers( gfx );
			}

		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Draw Horizontal Rullers
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawHorizontalRuler( Graphics gfx )
		{
			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(1, MeasureUnits.Point);

			// draw Minor lines
			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i +=koef * 5 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 - 2 );	
			}

			// draw  lines
			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i +=koef * 25 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 );	
			}


			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width; i += koef * 50 )
			{

				float fontSize = m_ruler.Size.Height/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height - 5);	
				gfx.DrawString(((i - 20)/koef).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), i + 1, m_ruler.Size.Height - MeasureUnitsConverter.ToPixelY( fontSize, MeasureUnits.Point ) - 2 );

			}			
		}


		/// <summary>
		/// Draw Vertical Ruler
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawVerticalRulers( Graphics gfx )
		{
			// TODO: Implement drawing

			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(1, MeasureUnits.Point);

			// draw Minor lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef * 5 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2 - 2, i );	
			}

			// draw  lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef * 25 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2, i );	
			}

			// draw Major lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height; i += koef * 50 )
			{

				float fontSize = m_ruler.Size.Width/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				StringFormat drawFormat = new StringFormat();
				drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), 0, i, m_ruler.Size.Width - 5, i );	
				gfx.DrawString(((i - 20)/koef).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), m_ruler.Size.Width - MeasureUnitsConverter.ToPixelX( fontSize, MeasureUnits.Point ) - 2, i + 1, drawFormat ); 

			}

		}

		#endregion
	}


	/// <summary>
	/// Markup in Document
	/// </summary>
	public class DocumentMarkupRenderer
		: RulerMarkupRenderer
	{
		Ruler m_ruler = null;

		#region Class initialize/finalize methods
		public DocumentMarkupRenderer( Ruler ruler )
			: base( ruler )
		{
			m_ruler = ruler;
		}
		#endregion
		
		#region Class overrides
		public override void RenderMarkup( Graphics gfx, float realPageScale )
		{
			//float fWidth

			gfx.FillRectangle(new SolidBrush( Color.FromArgb(253, 255, 232) ), m_ruler.Location.X, m_ruler.Location.Y, m_ruler.Size.Width, m_ruler.Size.Height);



			// if Horizontal Ruler .....
			if ( m_ruler.Size.Width > m_ruler.Size.Height )
			{
				DrawHorizontalRuler( gfx );
			}
			else
			{
				DrawVerticalRulers( gfx );
			}

		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Draw Horizontal Rullers
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawHorizontalRuler( Graphics gfx )
		{
			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(1, MeasureUnits.Document);

			// draw Minor lines
			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i +=koef * 5 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 - 2 );	
			}

			// draw  lines
			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i +=koef * 25 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 );	
			}


			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width; i += koef * 50 )
			{

				float fontSize = m_ruler.Size.Height/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height - 5);	
				gfx.DrawString(((i - 20)/koef).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), i + 1, m_ruler.Size.Height - MeasureUnitsConverter.ToPixelY( fontSize, MeasureUnits.Point ) - 2 );

			}			
		}


		/// <summary>
		/// Draw Vertical Ruler
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawVerticalRulers( Graphics gfx )
		{
			// TODO: Implement drawing

			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(1, MeasureUnits.Point);

			// draw Minor lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef * 5 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2 - 2, i );	
			}

			// draw  lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef * 25 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2, i );	
			}

			// draw Major lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height; i += koef * 50 )
			{

				float fontSize = m_ruler.Size.Width/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				StringFormat drawFormat = new StringFormat();
				drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), 0, i, m_ruler.Size.Width - 5, i );	
				gfx.DrawString(((i - 20)/koef).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), m_ruler.Size.Width - MeasureUnitsConverter.ToPixelX( fontSize, MeasureUnits.Point ) - 2, i + 1, drawFormat ); 

			}

		}

		#endregion
	}
	/// <summary>
	/// Markup in Display
	/// </summary>
	public class DisplayMarkupRenderer
		: RulerMarkupRenderer
	{
		Ruler m_ruler = null;

		#region Class initialize/finalize methods
		public DisplayMarkupRenderer( Ruler ruler )
			: base( ruler )
		{
			m_ruler = ruler;
		}
		#endregion
		
		#region Class overrides
		public override void RenderMarkup( Graphics gfx, float realPageScale )
		{
			//float fWidth

			gfx.FillRectangle(new SolidBrush( Color.FromArgb(253, 255, 232) ), m_ruler.Location.X, m_ruler.Location.Y, m_ruler.Size.Width, m_ruler.Size.Height);



			// if Horizontal Ruler .....
			if ( m_ruler.Size.Width > m_ruler.Size.Height )
			{
				DrawHorizontalRuler( gfx );
			}
			else
			{
				DrawVerticalRulers( gfx );
			}

		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Draw Horizontal Rullers
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawHorizontalRuler( Graphics gfx )
		{
			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(1, MeasureUnits.Display);

			// draw Minor lines
			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i +=koef * 5 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 - 2 );	
			}

			// draw  lines
			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width + 20; i +=koef * 25 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height / 2 );	
			}


			for (int i = m_ruler.Location.X; i <= m_ruler.Size.Width; i += koef * 50 )
			{

				float fontSize = m_ruler.Size.Height/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), i, 0, i, m_ruler.Size.Height - 5);	
				gfx.DrawString(((i - 20)/koef).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), i + 1, m_ruler.Size.Height - MeasureUnitsConverter.ToPixelY( fontSize, MeasureUnits.Point ) - 2 );

			}			
		}


		/// <summary>
		/// Draw Vertical Ruler
		/// </summary>
		/// <param name="gfx"></param>
		private void DrawVerticalRulers( Graphics gfx )
		{
			// TODO: Implement drawing

			gfx.SmoothingMode = SmoothingMode.HighSpeed;

			int koef = ( int ) MeasureUnitsConverter.ToPixelX(1, MeasureUnits.Display);

			// draw Minor lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef * 5 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2 - 2, i );	
			}

			// draw  lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height + 20; i += koef * 25 )
			{
				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MinorLinesColor ), 1 ), 0, i, m_ruler.Size.Width / 2, i );	
			}

			// draw Major lines
			for (int i = m_ruler.Location.Y; i <= m_ruler.Size.Height; i += koef * 50 )
			{

				float fontSize = m_ruler.Size.Width/2 - 1;
				Font font = new Font(FontFamily.GenericSerif, fontSize);

				StringFormat drawFormat = new StringFormat();
				drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

				gfx.DrawLine(new Pen(new SolidBrush( m_ruler.MajorLinesColor ), 1 ), 0, i, m_ruler.Size.Width - 5, i );	
				gfx.DrawString(((i - 20)/koef).ToString(), font, new SolidBrush( m_ruler.MajorLinesColor ), m_ruler.Size.Width - MeasureUnitsConverter.ToPixelX( fontSize, MeasureUnits.Point ) - 2, i + 1, drawFormat ); 

			}

		}

		#endregion
	}
}
