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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Renderers
{
	#region ColorTableOffice2010
    /// <summary>
    /// Interface to create an Office 2010 color table.
    /// </summary>
	public interface IColorTableOffice2010Creator
	{
		ColorTableOffice2010 Create();
	}
	
	/// <summary>
    /// Color table for Office 2010 like scroll bars.
    /// </summary>
	public class ColorTableOffice2010:
		IColorTableOffice2010Creator
	{
		#region Constants
		/// <summary>
        /// Color references for Office2010.
        /// </summary>
		protected enum OFFICE2010COLOR
		{
			/// <summary></summary>
			scrollerGradientBegin = 0,
			/// <summary></summary>
			scrollerGradientEnd,
			/// <summary></summary>
			arrowButtonGradientBegin,
			/// <summary></summary>
			arrowButtonGradientEnd,
			/// <summary></summary>
			arrowButtonBorderDark,
			/// <summary></summary>
			arrowButtonBorderLight,
			/// <summary></summary>
			arrowGradientBegin,
			/// <summary></summary>
			arrowGradientEnd,
			/// <summary></summary>
			arrowGradientNormalBegin,
			/// <summary></summary>
			arrowGradientNormalEnd,
			/// <summary></summary>
			thumbLinesGradientBegin,
			/// <summary></summary>
			thumbLinesGradientEnd,
			/// <summary></summary>
			scrollerBorderBegin,
			/// <summary></summary>
			scrollerBorderEnd,
            /// <summary>
            /// 
            /// </summary>
            scrollerGripDark,
            /// <summary>
            /// 
            /// </summary>
            scrollerGripLight,
            /// <summary>
            /// 
            /// </summary>
            scrollerGripBackGround,

			[Browsable(false)]
			MAX_NORMAL_COLORS,

			/// <summary></summary>
			arrowButtonGradientSelectedBegin = MAX_NORMAL_COLORS,
			/// <summary></summary>
			arrowButtonGradientSelectedEnd,
			/// <summary></summary>
			arrowButtonGradientPressedBegin,
			/// <summary></summary>
			arrowButtonGradientPressedEnd,
			/// <summary></summary>
			arrowButtonBorderSelectedDark,
			/// <summary></summary>
			arrowButtonBorderSelectedLight,
			/// <summary></summary>
			arrowButtonBorderPressedDark,
			/// <summary></summary>
			arrowButtonBorderPressedLight,
			/// <summary></summary>
			thumbPressedBackgroundGradientBegin,
			/// <summary></summary>
			thumbPressedBackgroundGradientEnd,

			[Browsable( false )]
			MAX
		}
		#endregion

		#region Constructor
        /// <summary>
        /// Constructor for Office2010 color table which is used to register the color style of each theme.
        /// </summary>
		static ColorTableOffice2010()
		{
			ScrollBarCustomDrawStyles style;

			style = ScrollBarCustomDrawStyles.Office2010;
			RegisterColorTable( style, Office2010ColorScheme.Blue,		new ColorTableOffice2010Blue() );
			RegisterColorTable( style, Office2010ColorScheme.Silver,	new ColorTableOffice2010Silver() );			
			RegisterColorTable( style, Office2010ColorScheme.Black,		new ColorTableOffice2010Black() );
			RegisterColorTable( style, Office2010ColorScheme.Managed,	new ColorTableOffice2010Blue() );

			Office2007Colors.ManagedColorsApplied += new Office2007Colors.ManagedColorsAppliedEventHandler( ManagedColorsApplied );

			if( Office2007Colors.ManagedBaseColor != Color.Empty )
			{
				OnManagedColorApplied( Office2007Colors.ManagedBaseColor );
			}
		}
        /// <summary>
        /// Used internally
        /// </summary>
		internal ColorTableOffice2010()
		{
		}

		#endregion

		#region Properties
		/// <summary>
        /// Specifies the ScrollerGradientBegin color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ScrollerGradientBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.scrollerGradientBegin ];
			}
		}
		/// <summary>
        /// Specifies the ScrollerGradientEnd color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ScrollerGradientEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.scrollerGradientEnd ];
			}
		}
		/// <summary>
        /// Specifies the ArrowButtonGradientBegin color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowButtonGradientBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowButtonGradientBegin ];
			}
		}
		/// <summary>
        /// Specifies the ArrowButtonGradientEnd color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowButtonGradientEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowButtonGradientEnd ];
			}
		}
		/// <summary>
        /// Specifies the ArrowButtonGradientSelectedBegin color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowButtonGradientSelectedBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowButtonGradientSelectedBegin ];
			}
		}
		/// <summary>
        /// Specifies the ArrowButtonGradientSelectedEnd color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowButtonGradientSelectedEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowButtonGradientSelectedEnd ];
			}
		}
		/// <summary>
        /// Specifies the ArrowButtonGradientPressedBegin color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowButtonGradientPressedBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowButtonGradientPressedBegin ];
			}
		}
		/// <summary>
        /// Specifies the ArrowButtonGradientPressedEnd color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowButtonGradientPressedEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowButtonGradientPressedEnd ];
			}
		}
		/// <summary>
        /// Specifies the ArrowButtonBorderDark color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowButtonBorderDark
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowButtonBorderDark ];
			}
		}
		/// <summary>
        /// Specifies the ArrowButtonBorderLight color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowButtonBorderLight
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowButtonBorderLight ];
			}
		}
		/// <summary>
        /// Specifies the ArrowButtonBorderSelectedDark color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowButtonBorderSelectedDark
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowButtonBorderSelectedDark ];
			}
		}
		/// <summary>
        /// Specifies the ArrowButtonBorderSelectedLight color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowButtonBorderSelectedLight
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowButtonBorderSelectedLight ];
			}
		}
		/// <summary>
        /// Specifies the ArrowButtonBorderPressedDark color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowButtonBorderPressedDark
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowButtonBorderPressedDark ];
			}
		}
		/// <summary>
        /// Specifies the ArrowButtonBorderPressedLight color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowButtonBorderPressedLight
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowButtonBorderPressedLight ];
			}
		}
		/// <summary>
        /// Specifies the ArrowGradientBegin color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowGradientBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowGradientBegin ];
			}
		}
		/// <summary>
        /// Specifies the ArrowGradientEnd color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowGradientEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowGradientEnd ];
			}
		}
		/// <summary>
        /// Specifies the ArrowGradientNormalBegin color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowGradientNormalBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowGradientNormalBegin ];
			}
		}
		/// <summary>
        /// Specifies the ArrowGradientNormalEnd color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ArrowGradientNormalEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.arrowGradientNormalEnd ];
			}
		}
		/// <summary>
        /// Specifies the ThumbLinesGradientBegin color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ThumbLinesGradientBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.thumbLinesGradientBegin ];
			}
		}
		/// <summary>
        /// Specifies the ThumbLinesGradientEnd color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ThumbLinesGradientEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.thumbLinesGradientEnd ];
			}
		}
		/// <summary>
        /// Specifies the ThumbPressedBackgroundGradientBegin color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ThumbPressedBackgroundGradientBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.thumbPressedBackgroundGradientBegin ];
			}
		}
		/// <summary>
        /// Specifies the ThumbPressedBackgroundGradientEnd color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ThumbPressedBackgroundGradientEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.thumbPressedBackgroundGradientEnd ];
			}
		}
		/// <summary>
        /// Specifies the ScrollerBorderBegin color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ScrollerBorderBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.scrollerBorderBegin ];
			}
		}
		/// <summary>
        /// Specifies the ScrollerBorderEnd color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ScrollerBorderEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.scrollerBorderEnd ];
			}
		}
        /// <summary>
        /// Specifies the ScrollerGripDark color of the Office2010 scroll bars
        /// </summary>
		public virtual Color ScrollerGripDark
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.scrollerGripDark ];
			}
		}
        /// <summary>
        /// Specifies the ScrollerGripLight color of the Office2010 scroll bars
        /// </summary>
        public virtual Color ScrollerGripLight
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2010COLOR.scrollerGripLight ];
			}
		}
        /// <summary>
        /// Specifies the ScrollerGripBackGround color of the Office2010 scroll bars
        /// </summary>
        public virtual Color ScrollerGripBackGround
		{
			get
			{
                return OfficeColors[(int)OFFICE2010COLOR.scrollerGripBackGround];
			}
		}

		/// <summary>
        /// Specifies the color of the Office2010 scroll bars
        /// </summary>
		protected Color[ ] OfficeColors
		{
			get
			{
				if( m_office2010Colors == null )
				{
					m_office2010Colors = new Color[( int )OFFICE2010COLOR.MAX];
					InitColors( ref m_office2010Colors );
				}
				return m_office2010Colors;
			}
		}
		#endregion

		#region Methods
		
		/// <summary>Get the Alpha blended color. </summary>
		/// <param name="src">Source color</param>
		/// <param name="dest">destination color</param>
		/// <param name="alpha">alpha color</param>
		/// <returns>returns the alpha blended color</returns>
		public static Color GetAlphaBlendedColor( Color src, Color dest, int alpha )
		{
			int R = ( ( src.R * alpha ) + ( ( 0xff - alpha ) * dest.R ) ) / 0xff;
			int G = ( ( src.G * alpha ) + ( ( 0xff - alpha ) * dest.G ) ) / 0xff;
			int B = ( ( src.B * alpha ) + ( ( 0xff - alpha ) * dest.B ) ) / 0xff;
			int A = ( ( src.A * alpha ) + ( ( 0xff - alpha ) * dest.A ) ) / 0xff;

			return Color.FromArgb( A, R, G, B );
		}

		/// <summary>
		/// Retrieves color table according to style and scheme.
		/// </summary>
		/// <param name="style">Office2010 style.</param>
		/// <param name="scheme">Office2010 color scheme.</param>
		/// <returns>Color table.</returns>
		public static ColorTableOffice2010 GetColorTable( ScrollBarCustomDrawStyles style, Office2010ColorScheme scheme )
		{			
			ColorTableKey key = new ColorTableKey( style, scheme );
			ColorTableOffice2010 colorTable = (ColorTableOffice2010)s_colorTables[ key ];

			return colorTable;
		}

		/// <summary>
        /// Registers color table within internal collection.
		/// </summary>
		/// <param name="style">Office2010 style.</param>
		/// <param name="scheme">Office2010 scheme.</param>
		/// <param name="colorTable">Color table itself.</param>
		protected static void RegisterColorTable( ScrollBarCustomDrawStyles style, Office2010ColorScheme scheme, ColorTableOffice2010 colorTable )
		{
			ColorTableKey key = new ColorTableKey( style, scheme );

			s_colorTables[ key ] = colorTable;
		}

		/// <summary>
		/// Updates the styles of the scrollbars related to Office2010 colors.
		/// </summary>
		/// <param name="basicColor">base color.</param>
		internal void UpdateColors( ScrollBarCustomDrawStyles style, Color basicColor )
		{
			Color[] office2010Colors = this.OfficeColors;
			ColorTableOffice2010 silverColors = GetColorTable( style, Office2010ColorScheme.Silver );

			for( int i = 0; i < (int)OFFICE2010COLOR.MAX_NORMAL_COLORS; ++i )
			{
				office2010Colors[i] = Office2007Colors.MergeColors( silverColors.OfficeColors[i], basicColor );
			}
		}
       
		/// <summary>
		/// Updates the Office2010 scrollbar color schemes.
		/// </summary>
        /// <param name="style">Custom scrollbar draw style.</param>
		/// <param name="scheme">Office2010 color scheme.</param>
		internal void UpdateScheme( ScrollBarCustomDrawStyles style, Office2010ColorScheme scheme )
		{
			ColorTableOffice2010 colorTable = GetColorTable( style, scheme );
			ColorTableOffice2010 newColorTable = ((IColorTableOffice2010Creator)colorTable).Create();

			RegisterColorTable( style, scheme, newColorTable );
		}

		/// <summary>
		/// Applies colors for managed scheme.
		/// </summary>
        /// <param name="style">Custom scrollbar draw style.</param>
        /// <param name="baseColor">Base color for the managed theme.</param>
		public static void ApplyManagedColors( ScrollBarCustomDrawStyles style, Color baseColor )
		{
			ColorTableOffice2010 managedColors = GetColorTable( style, Office2010ColorScheme.Managed );

			managedColors.UpdateColors( style, baseColor );
		}

		/// <summary>
        ///  Applies scheme for managed theme.
		/// </summary>
		/// <param name="form">Container form.</param>
        /// <param name="style">Custom scrollbar draw style.</param>
        /// <param name="scheme">Office2010 color scheme.</param>
		public static void ApplyManagedScheme( Form form, ScrollBarCustomDrawStyles style, Office2010ColorScheme scheme )
		{
			ColorTableOffice2010 managedColors = GetColorTable( style, scheme );

			managedColors.UpdateScheme( style, scheme );

			form.Invalidate( true );
		}

        /// <summary>
        /// Applies managed colors
        /// </summary>
        /// <param name="args">base color <see cref="ManagedColorsAppliedEventArgs"/>.</param>
		private static void ManagedColorsApplied( Office2007Colors.ManagedColorsAppliedEventArgs args )
		{
			OnManagedColorApplied( args.BaseColor );
		}

		/// <summary>
		/// Called when <see cref="ManagedColorsAppliedEvent"/> is risen.
		/// </summary>
		/// <param name="baseColor">base color for managed theme.</param>
		protected static void OnManagedColorApplied( Color baseColor )
		{
			ApplyManagedColors( ScrollBarCustomDrawStyles.Office2010, baseColor );
		}

		#endregion

		#region Overrides
		/// <summary>
        /// Initialize scroll bar color fields.
        /// </summary>
		/// <param name="colors">Scroll bar color references.</param>
		protected virtual void InitColors( ref Color[ ] colors )
		{
			colors[ ( int )OFFICE2010COLOR.scrollerGradientBegin ] = Color.FromArgb( 165, 187, 211 );
			colors[ ( int )OFFICE2010COLOR.scrollerGradientEnd ] = Color.FromArgb( 185, 206, 230 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientBegin ] = Color.FromArgb( 185, 206, 230 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientEnd ] = Color.FromArgb( 145, 171, 201 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientSelectedBegin ] = Color.FromArgb( 193, 215, 240 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientSelectedEnd ] = Color.FromArgb( 163, 190, 219 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientPressedBegin ] = Color.FromArgb( 176, 199, 222 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientPressedEnd ] = Color.FromArgb( 137, 164, 193 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonBorderDark ] = Color.FromArgb( 88, 104, 145 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonBorderLight ] = Color.FromArgb( 225, 231, 239 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonBorderSelectedDark ] = Color.FromArgb( 92, 119, 150 );
            colors[ ( int )OFFICE2010COLOR.arrowButtonBorderSelectedLight ] = Color.FromArgb( 177, 202, 229 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonBorderPressedDark ] = Color.FromArgb( 92, 119, 150 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonBorderPressedLight ] = Color.FromArgb( 177, 202, 229 );
			colors[ ( int )OFFICE2010COLOR.arrowGradientBegin ] = Color.FromArgb( 110, 126, 166 );
			colors[ ( int )OFFICE2010COLOR.arrowGradientEnd ] = Color.FromArgb( 66, 75, 99 );
			colors[ ( int )OFFICE2010COLOR.arrowGradientNormalBegin ] = Color.FromArgb( 110, 126, 166 );
			colors[ ( int )OFFICE2010COLOR.arrowGradientNormalEnd ] = Color.FromArgb( 66, 75, 99 );
			colors[ ( int )OFFICE2010COLOR.thumbLinesGradientBegin ] = Color.FromArgb( 139, 142, 144 );
			colors[ ( int )OFFICE2010COLOR.thumbLinesGradientEnd ] = Color.FromArgb( 120, 126, 134 );
			colors[ ( int )OFFICE2010COLOR.thumbPressedBackgroundGradientBegin ] = Color.FromArgb( 94, 100, 117 );
			colors[ ( int )OFFICE2010COLOR.thumbPressedBackgroundGradientEnd ] = Color.FromArgb( 73, 81, 101 );
			colors[ ( int )OFFICE2010COLOR.scrollerBorderBegin ] = Color.FromArgb( 149, 176, 210 );
			colors[ ( int )OFFICE2010COLOR.scrollerBorderEnd ] = Color.FromArgb( 124, 155, 193 );
			
            colors[ ( int )OFFICE2010COLOR.scrollerGripDark ] = Color.FromArgb( 69, 93, 128 );
            colors[ ( int )OFFICE2010COLOR.scrollerGripLight ] = Color.FromArgb( 177, 201, 232 );
            colors[ ( int )OFFICE2010COLOR.scrollerGripBackGround ] = Color.FromArgb( 127, 163, 211 );
		}
		#endregion

		#region Fields
		/// <summary>
        /// Office 2010 color array.
        /// </summary>
		private Color[ ] m_office2010Colors;

		/// <summary>
		/// Registered color tables.
		/// </summary>
		private static IDictionary s_colorTables = new SortedList();
		#endregion

		#region ColorTableKey class
        /// <Internal/>
		internal class ColorTableKey:
			IComparable
		{
			private ScrollBarCustomDrawStyles m_style;
			private Office2010ColorScheme m_scheme;

            /// <summary>
            /// Constructor for color Table key.
            /// </summary>
            /// <param name="style">Custom scrollbar draw style.</param>
            /// <param name="scheme">Office2010 color scheme.</param>
            public ColorTableKey(ScrollBarCustomDrawStyles style, Office2010ColorScheme scheme)
			{
				m_style = style;
				m_scheme = scheme;
			}

			#region IComparable Members
            /// <summary>
            /// Compare the color table keys through <see cref="IComparable"/>.
            /// </summary>
            /// <param name="obj">Object of the ColorTableKey.</param>
            /// <returns>Returns 1 if color key is less. Otherwise returns 0.</returns>
			int IComparable.CompareTo( object obj )
			{
				ColorTableKey key = (ColorTableKey)obj;
				int result = -1;

				if( !this.Less( key ) )
				{
					result = key.Less( this ) ? 1 : 0;
				}

				return result;
			}

            /// <summary>
            /// Compares the color table keys.
            /// </summary>
            /// <param name="key">A color table key.</param>
            /// <returns>returns true if passed one is less. Otherwise return false</returns>
			private bool Less( ColorTableKey key )
			{
				bool bLess = true;

				if( m_style == key.m_style )
				{
					bLess = (m_scheme < key.m_scheme);
				}
				if( m_style > key.m_style )
				{
					bLess = false;
				}

				return bLess;
			}

			#endregion
		}

		#endregion

		#region IColorTableOffice2010Creator Members
        /// <summary>
        /// Interface to create a Office2010 color table.
        /// </summary>
        /// <returns>returns office2010 color table</returns>
		ColorTableOffice2010 IColorTableOffice2010Creator.Create()
		{
			return new ColorTableOffice2010();
		}

		#endregion
	}
	#endregion

	#region ColorTableOffice2010Blue
    /// <summary>
    /// Provides attributes and methods for Office2010 blue color table.
    /// </summary>
	public class ColorTableOffice2010Blue:
		ColorTableOffice2010,
		IColorTableOffice2010Creator
	{
		#region Constructor

        /// <summary>
        /// Used internally
        /// </summary>
        internal ColorTableOffice2010Blue()
		{
		}

		#endregion

		#region IColorTableOffice2010Creator Members

        /// <summary>
        /// Interface to create an office2010 blue color table.
        /// </summary>
        /// <returns>returns office2010 blue color table.</returns>
        ColorTableOffice2010 IColorTableOffice2010Creator.Create()
		{
			return new ColorTableOffice2010Blue();
		}

		#endregion
	}

	#endregion

	#region ColorTableOffice2010Silver
	/// <summary>
    /// Provides attributes and methods for Office2010 Silver color table.
    /// </summary>
	public class ColorTableOffice2010Silver:
		ColorTableOffice2010,
		IColorTableOffice2010Creator
	{
		#region Construction

        /// <summary>
        /// Used internally
        /// </summary>
        internal ColorTableOffice2010Silver()
		{
		}

		#endregion

		#region Overrides
        /// <summary>
        /// Initialize scroll bar color fields for silver color.
        /// </summary>
        /// <param name="colors">Scroll bar color references.</param>
        protected override void InitColors(ref Color[] colors)
		{
			base.InitColors( ref colors );

			colors[ ( int )OFFICE2010COLOR.scrollerGradientBegin ] = Color.FromArgb( 213, 218, 224 );
			colors[ ( int )OFFICE2010COLOR.scrollerGradientEnd ] = Color.FromArgb( 223, 228, 235 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientBegin ] = Color.FromArgb( 240, 244, 248 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientEnd ] = Color.FromArgb( 215, 219, 225 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientSelectedBegin ] = Color.FromArgb( 252, 253, 254 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientSelectedEnd ] = Color.FromArgb( 228, 232, 236 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientPressedBegin ] = Color.FromArgb( 235, 239, 243 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientPressedEnd ] = Color.FromArgb( 208, 212, 218 );
            colors[ ( int )OFFICE2010COLOR.arrowButtonBorderSelectedDark ] = Color.FromArgb( 150, 158, 168 );
            colors[ ( int )OFFICE2010COLOR.arrowButtonBorderSelectedLight ] = Color.FromArgb( 241, 243, 245 );
            colors[ ( int )OFFICE2010COLOR.arrowButtonBorderPressedDark ] = Color.FromArgb( 150, 158, 168 );
            colors[ ( int )OFFICE2010COLOR.arrowButtonBorderPressedLight ] = Color.FromArgb( 241, 243, 245 );
            colors[ ( int )OFFICE2010COLOR.arrowButtonBorderDark ] = Color.FromArgb( 108, 110, 113 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonBorderLight ] = Color.FromArgb( 239, 239, 240 );
			colors[ ( int )OFFICE2010COLOR.arrowGradientBegin ] = Color.FromArgb( 116, 117, 118 );
			colors[ ( int )OFFICE2010COLOR.arrowGradientEnd ] = Color.FromArgb( 69, 70, 71 );
			colors[ ( int )OFFICE2010COLOR.arrowGradientNormalBegin ] = Color.FromArgb( 116, 117, 118 );
			colors[ ( int )OFFICE2010COLOR.arrowGradientNormalEnd ] = Color.FromArgb( 69, 70, 71 );
			colors[ ( int )OFFICE2010COLOR.thumbLinesGradientBegin ] = Color.FromArgb( 125, 134, 146 );
			colors[ ( int )OFFICE2010COLOR.thumbLinesGradientEnd ] = Color.FromArgb( 113, 127, 146 );
			colors[ ( int )OFFICE2010COLOR.scrollerBorderBegin ] = Color.FromArgb( 157, 159, 166 );
			colors[ ( int )OFFICE2010COLOR.scrollerBorderEnd ] = Color.FromArgb( 153, 156, 162 );

            colors[(int)OFFICE2010COLOR.scrollerGripDark] = Color.FromArgb(114, 118, 122);
            colors[(int)OFFICE2010COLOR.scrollerGripLight] = Color.FromArgb(205, 209, 213);
            colors[(int)OFFICE2010COLOR.scrollerGripBackGround] = Color.FromArgb(183, 186, 194);
        }
		#endregion

		#region IColorTableOffice2010Creator Members


        /// <summary>
        /// Interface to create an office2010 Silver color table.
        /// </summary>
        /// <returns>returns office2010 silver color table</returns>
		ColorTableOffice2010 IColorTableOffice2010Creator.Create()
		{
			return new ColorTableOffice2010Silver();
		}

		#endregion
	}
	#endregion

	#region ColorTableOffice2010Black
    /// <summary>
    /// Provides attributes and methods for Office2010 Black color table.
    /// </summary>
    public class ColorTableOffice2010Black :
		ColorTableOffice2010,
		IColorTableOffice2010Creator
	{
		#region Constructor
        /// <summary>
        /// Used internally
        /// </summary>
        internal ColorTableOffice2010Black()
		{
		}

		#endregion

		#region Overrides
        /// <summary>
        /// Initialize scroll bar color fields for black color.
        /// </summary>
        /// <param name="colors">Scroll bar color references.</param>
        protected override void InitColors(ref Color[] colors)
		{
			base.InitColors( ref colors );

			colors[ ( int )OFFICE2010COLOR.scrollerGradientBegin ] = Color.FromArgb( 99, 99, 99 );
			colors[ ( int )OFFICE2010COLOR.scrollerGradientEnd ] = Color.FromArgb( 104, 104, 104 );
            colors[ ( int )OFFICE2010COLOR.arrowButtonGradientBegin] = Color.FromArgb( 115, 115, 115 );
            colors[ ( int )OFFICE2010COLOR.arrowButtonGradientEnd ] = Color.FromArgb( 87, 87, 87 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientSelectedBegin ] = Color.FromArgb( 127, 127, 127 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientSelectedEnd ] = Color.FromArgb( 98, 98, 98 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientPressedBegin ] = Color.FromArgb( 107, 107, 107 );
			colors[ ( int )OFFICE2010COLOR.arrowButtonGradientPressedEnd ] = Color.FromArgb( 78, 78, 78 );
            colors[ ( int )OFFICE2010COLOR.arrowButtonBorderSelectedDark ] = Color.FromArgb(36, 36, 36);
            colors[ ( int )OFFICE2010COLOR.arrowButtonBorderSelectedLight ] = Color.FromArgb(110, 110, 110);
            colors[ ( int )OFFICE2010COLOR.arrowButtonBorderPressedDark ] = Color.FromArgb(36, 36, 36);
            colors[ ( int )OFFICE2010COLOR.arrowButtonBorderPressedLight ] = Color.FromArgb(110, 110, 110);
            colors[ ( int )OFFICE2010COLOR.arrowButtonBorderDark ] = Color.FromArgb(42, 42, 42);
			colors[ ( int )OFFICE2010COLOR.arrowButtonBorderLight ] = Color.FromArgb( 103, 103, 103 );
			colors[ ( int )OFFICE2010COLOR.arrowGradientBegin ] = Color.FromArgb( 52, 52, 52 );
			colors[ ( int )OFFICE2010COLOR.arrowGradientEnd ] = Color.FromArgb( 31, 31, 31 );
			colors[ ( int )OFFICE2010COLOR.arrowGradientNormalBegin ] = Color.FromArgb( 52, 52, 52 );
			colors[ ( int )OFFICE2010COLOR.arrowGradientNormalEnd ] = Color.FromArgb( 31, 31, 31 );
			colors[ ( int )OFFICE2010COLOR.thumbLinesGradientBegin ] = Color.FromArgb( 157, 157, 157 );
			colors[ ( int )OFFICE2010COLOR.thumbLinesGradientEnd ] = Color.FromArgb( 147, 147, 147 );
			colors[ ( int )OFFICE2010COLOR.scrollerBorderBegin ] = Color.FromArgb( 45, 45, 45 );
			colors[ ( int )OFFICE2010COLOR.scrollerBorderEnd ] = Color.FromArgb( 54, 54, 54 );

            colors[(int)OFFICE2010COLOR.scrollerGripDark] = Color.FromArgb(37, 0, 37);
            colors[(int)OFFICE2010COLOR.scrollerGripLight] = Color.FromArgb(204, 204, 204);
            colors[(int)OFFICE2010COLOR.scrollerGripBackGround] = Color.FromArgb(112, 0, 112);
        }
		#endregion

		#region IColorTableOffice2010Creator Members
        /// <summary>
        /// Interface to create an office2010 Black color table.
        /// </summary>
        /// <returns>returns Office2010 black color table</returns>
        ColorTableOffice2010 IColorTableOffice2010Creator.Create()
		{
			return new ColorTableOffice2010Black();
		}

		#endregion
	}
	#endregion

}