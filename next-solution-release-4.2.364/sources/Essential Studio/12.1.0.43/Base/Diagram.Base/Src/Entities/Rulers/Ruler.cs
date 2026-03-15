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

#region File using derectives

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;
using FontStyle = Syncfusion.Windows.Forms.Diagram.FontStyle;


#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Ruler class.
    /// </summary>
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
        private float m_fMajorCalibratedLineUnits;
		#endregion
		
		#region Class initialize/finalize methods
		/// <summary>
		/// Creates instance of Ruler
		/// </summary>
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
            m_fMajorCalibratedLineUnits = GetMinimumCalibratedLineUnits(this.MeasureUnit);
			m_markupRenderer = CreateMarkupRenderer(this.MeasureUnit);
		}
		/// <summary>
		/// Creates Ruler as copy given
		/// </summary>
		/// <param name="src">The source instance.</param>
		public Ruler( Ruler src )
			: base( src )
		{
			m_szSize = src.m_szSize;
			m_clrBkgnd = src.m_clrBkgnd;
			m_clrMajorLines = src.m_clrMajorLines;
			m_clrMinorLines = src.m_clrMinorLines;
			m_clrHighlightColor = src.m_clrHighlightColor;
			m_clrMarker = src.m_clrMarker;
            m_fMajorCalibratedLineUnits = src.m_fMajorCalibratedLineUnits;
			m_markupRenderer = CreateMarkupRenderer(this.MeasureUnit);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Ruler"/> class.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		public Ruler( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
			m_szSize = ( Size ) info.GetValue( "size", typeof( Size ) );
			m_clrBkgnd = ( Color ) info.GetValue( "backgroundColor", typeof( Color ) );
			m_clrMajorLines = ( Color ) info.GetValue( "majorLinesColor", typeof( Color ) );
			m_clrMinorLines = ( Color ) info.GetValue( "minorLinesColor", typeof( Color ) );
			m_clrHighlightColor = ( Color ) info.GetValue( "highlightColor", typeof( Color ) );
			m_clrMarker = ( Color ) info.GetValue( "cursorMarkerColor", typeof( Color ) );
            m_fMajorCalibratedLineUnits = (float)info.GetValue("StepValue", typeof(float));
			m_markupRenderer = CreateMarkupRenderer(this.MeasureUnit);
		}
		#endregion
		
		#region Class properties
		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		/// <value>The location.</value>
		[ Browsable( false ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		public Point Location
		{
			get
			{
				return m_ptLocation;
			}
			set
			{
				if( m_ptLocation != value )
				{
					m_ptLocation = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the size.
		/// </summary>
		/// <value>The size.</value>
		[ Browsable( false ) ]
		public Size Size
		{
			get
			{
				return m_szSize;
			}
			set
			{
				if( m_szSize != value )
				{
					m_szSize = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the color of the background.
		/// </summary>
		/// <value>The color of the background.</value>
		[ Browsable( true ) ]
		[ Description( "Color used to fill ruler background." ) ]
		public Color BackgroundColor
		{
			get
			{
				return m_clrBkgnd;
			}
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
		/// <summary>
		/// Gets or sets the color of the major lines.
		/// </summary>
		/// <value>The color of the major lines.</value>
		[ Browsable( true ) ]
		[ Description( "Color used to draw major lines." ) ]
		public Color MajorLinesColor
		{
			get
			{
				return m_clrMajorLines;
			}
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
		/// <summary>
		/// Gets or sets the color of the minor lines.
		/// </summary>
		/// <value>The color of the minor lines.</value>
		[ Browsable( true ) ]
		[ Description( "Color used to draw minor lines." ) ]
		public Color MinorLinesColor
		{
			get
			{
				return m_clrMinorLines;
			}
			set
			{
				if( m_clrMinorLines != value )
				{
					m_clrMinorLines = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the color of the highlight.
		/// </summary>
		/// <value>The color of the highlight.</value>
		[ Browsable( true ) ]
		[ Description( "Color used to fill highlight area." ) ]
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
		/// <summary>
		/// Gets or sets the color of the marker.
		/// </summary>
		/// <value>The color of the marker.</value>
		[ Browsable( true ) ]
		[ Description( "Color used to draw cursor marker." ) ]
		public Color MarkerColor
		{
			get
			{
				return m_clrMarker;
			}
			set
			{
				if( m_clrMarker != value )
				{
					m_clrMarker = value;
				}
			}
		}
		/// <summary>
		/// Gets the text style.
		/// </summary>
		/// <value>The text style.</value>
		[ Browsable( true ) ]
		[ Description( "Determines the font used to draw the text." ) ]
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

        /// <summary>
        /// Gets the minimum calibrated line unit for ruler
        /// </summary>
        public float MinimumCalibratedLineUnits
        {
            get{ return GetMinimumCalibratedLineUnits(this.MeasureUnit);}
        }

        /// <summary>
        /// Gets or sets the major calibrated line unit for ruler
        /// </summary>
        public float MajorCalibratedLineUnits
        {
            get { return m_fMajorCalibratedLineUnits; }
            set
            {
                if (m_fMajorCalibratedLineUnits != value)
                {
                    if (value < MinimumCalibratedLineUnits)
                        m_fMajorCalibratedLineUnits = MinimumCalibratedLineUnits;
                    else
                        m_fMajorCalibratedLineUnits = value;
                }
            }
        }

		/// <summary>
		/// Gets or sets the marker position.
		/// </summary>
		/// <value>The marker position.</value>
		[ Browsable( false ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		public int MarkerPosition
		{
			get
			{
				return m_ptMarkerLocation;
			}
			set
			{
				m_ptMarkerLocation = value;
			}
		}
		/// <summary>
		/// Gets or sets the highlight area start.
		/// </summary>
		/// <value>The highlight area start.</value>
		[ Browsable( false ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		public int HighlightAreaStart
		{
			get
			{
				return m_nHighlightAreaStart;
			}
			set
			{
				m_nHighlightAreaStart = value;
			}
		}
		/// <summary>
		/// Gets or sets the width of the highlight area.
		/// </summary>
		/// <value>The width of the highlight area.</value>
		[ Browsable( false ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		public int HighlightAreaWidth
		{
			get
			{
				return m_nHighlightAreaWidth;
			}
			set
			{
				m_nHighlightAreaWidth = value;
			}
		}
		/// <summary>
		/// Gets or sets the measure unit.
		/// </summary>
		/// <value>The measure unit.</value>
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
                    m_fMajorCalibratedLineUnits = GetMinimumCalibratedLineUnits(this.MeasureUnit);

				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether current instance inherit container measure units.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if current instance inherit container measure units; otherwise, <c>false</c>.
		/// </value>
		[ Browsable( false ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		public override bool InheritContainerMeasureUnits
		{
			get
			{
				return base.InheritContainerMeasureUnits;
			}
			set
			{
				base.InheritContainerMeasureUnits = value;
			}
		}
		#endregion
		
		#region Class public methods
		/// <summary>
		/// Draws the specified graphics.
		/// </summary>
		/// <param name="gfx">Graphics to draw on.</param>
		/// <param name="view">The view.</param>
		public abstract void Draw( Graphics gfx, View view );
		#endregion
		
		#region Class overrides
		/// <summary>
		/// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData( info, context );
			
			info.AddValue( "size", m_szSize );
			info.AddValue( "backgroundColor", m_clrBkgnd );
			info.AddValue( "majorLinesColor", m_clrMajorLines );
			info.AddValue( "minorLinesColor", m_clrMinorLines );
			info.AddValue( "highlightColor", m_clrHighlightColor );
			info.AddValue( "cursorMarkerColor", m_clrMarker );
            info.AddValue( "StepValue", m_fMajorCalibratedLineUnits);
		}
		/// <summary>
		/// Updates the service references.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public override void UpdateServiceReferences( IServiceReferenceProvider provider )
		{
			base.UpdateServiceReferences( provider );
			
			if( provider == null )
			{
				m_propertyObserver = null;
			}
			else
			{
				m_propertyObserver = ( IPropertyObserver ) provider.ProvideServiceReference( typeof( IPropertyObserver ).TypeHandle );
			}
		}
		/// <summary>
		/// Create Markup Renderer for Diagram Measure Units
		/// </summary>
		/// <param name="measureUnits"></param>
		/// <returns></returns>
		protected RulerMarkupRenderer CreateMarkupRenderer (MeasureUnits measureUnits)
		{
			switch( measureUnits )
			{
				case MeasureUnits.Pixel:
					return new PixelMarkupRenderer( this );
				case MeasureUnits.Point:
					return new PointsMarkupRenderer( this );
				case MeasureUnits.Document:
					return new DocumentMarkupRenderer( this );
				case MeasureUnits.Display:
					return new DisplayMarkupRenderer( this );
				case MeasureUnits.SixteenthInch:
					return new SixteenthInchMarkupRenderer( this );
				case MeasureUnits.EighthInch:
					return new EightInchMarkupRenderer( this );
				case MeasureUnits.QuarterInch:
					return new QuarterInchMarkupRenderer( this );
				case MeasureUnits.HalfInch:
					return new HalfInchMarkupRenderer( this );
				case MeasureUnits.Inch:
					return new InchMarkupRenderer( this );
				case MeasureUnits.Foot:
					return new FeetMarkupRenderer( this );
				case MeasureUnits.Yard:
					return new YardMarkupRenderer( this );
				case MeasureUnits.Mile:
					return new MileMarkupRenderer( this );
				case MeasureUnits.Millimeter:
					return new MillimetersMarkupRenderer( this );
				case MeasureUnits.Centimeter:
					return new CentimetersMarkupRenderer( this );
				case MeasureUnits.Meter:
					return new MetersMarkupRenderer( this );
				case MeasureUnits.Kilometer:
					return new KilometerMarkupRenderers( this );
			}

			return new PixelMarkupRenderer(this) ;
		}

        /// <summary>
        /// Gets the minimum calibrated line unit
        /// </summary>
        /// <param name="measureUnits">Measurement unit</param>
        /// <returns>Minimum calibrated line unit</returns>
        protected float GetMinimumCalibratedLineUnits(MeasureUnits measureUnits)
        {
            switch (measureUnits)
            {
                case MeasureUnits.Pixel:
                    return 20;
                case MeasureUnits.Point:
                    return 20;
                case MeasureUnits.Document:
                    return 100;
                case MeasureUnits.Display:
                    return 20;
                case MeasureUnits.SixteenthInch:
                    return 4;
                case MeasureUnits.EighthInch:
                    return 2;
                case MeasureUnits.QuarterInch:
                    return 1;
                case MeasureUnits.HalfInch:
                    return 1;
                case MeasureUnits.Inch:
                    return 1;
                case MeasureUnits.Foot:
                    return 1;
                case MeasureUnits.Yard:
                    return 1;
                case MeasureUnits.Mile:
                    return 1;
                case MeasureUnits.Millimeter:
                    return 10;
                case MeasureUnits.Centimeter:
                    return 1;
                case MeasureUnits.Meter:
                    return 1;
                case MeasureUnits.Kilometer:
                    return 1;
            }
            return 20f;
        }

		#endregion       
	}
}
