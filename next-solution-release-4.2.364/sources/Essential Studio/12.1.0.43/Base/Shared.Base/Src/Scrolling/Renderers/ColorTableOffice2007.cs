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
	#region ColorTableOffice2007

	public interface IColorTableOffice2007Creator
	{
		ColorTableOffice2007 Create();
	}
	
	/// <summary></summary>
	public class ColorTableOffice2007:
		IColorTableOffice2007Creator
	{
		#region Constants
		/// <summary></summary>
		protected enum OFFICE2007COLOR
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

		#region Construction

		static ColorTableOffice2007()
		{
			ScrollBarCustomDrawStyles style;

			style = ScrollBarCustomDrawStyles.Office2007;
			RegisterColorTable( style, Office2007ColorScheme.Blue,		new ColorTableOffice2007Blue() );
			RegisterColorTable( style, Office2007ColorScheme.Silver,	new ColorTableOffice2007Silver() );			
			RegisterColorTable( style, Office2007ColorScheme.Black,		new ColorTableOffice2007Black() );
			RegisterColorTable( style, Office2007ColorScheme.Managed,	new ColorTableOffice2007Blue() );

			style = ScrollBarCustomDrawStyles.Office2007Generic;
			RegisterColorTable( style, Office2007ColorScheme.Blue,		new ColorTableGenericOffice2007Blue() );
			RegisterColorTable( style, Office2007ColorScheme.Silver,	new ColorTableGenericOffice2007Silver() );			
			RegisterColorTable( style, Office2007ColorScheme.Black,		new ColorTableGenericOffice2007Black() );
			RegisterColorTable( style, Office2007ColorScheme.Managed,	new ColorTableGenericOffice2007Blue() );

			Office2007Colors.ManagedColorsApplied += new Office2007Colors.ManagedColorsAppliedEventHandler( ManagedColorsApplied );

			if( Office2007Colors.ManagedBaseColor != Color.Empty )
			{
				OnManagedColorApplied( Office2007Colors.ManagedBaseColor );
			}
		}

		internal ColorTableOffice2007()
		{
		}

		#endregion

		#region Properties
		/// <summary></summary>
		public virtual Color ScrollerGradientBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.scrollerGradientBegin ];
			}
		}
		/// <summary></summary>
		public virtual Color ScrollerGradientEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.scrollerGradientEnd ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowButtonGradientBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowButtonGradientBegin ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowButtonGradientEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowButtonGradientEnd ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowButtonGradientSelectedBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedBegin ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowButtonGradientSelectedEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedEnd ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowButtonGradientPressedBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedBegin ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowButtonGradientPressedEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedEnd ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowButtonBorderDark
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowButtonBorderDark ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowButtonBorderLight
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowButtonBorderLight ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowButtonBorderSelectedDark
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowButtonBorderSelectedDark ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowButtonBorderSelectedLight
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowButtonBorderSelectedLight ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowButtonBorderPressedDark
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowButtonBorderPressedDark ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowButtonBorderPressedLight
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowButtonBorderPressedLight ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowGradientBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowGradientBegin ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowGradientEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowGradientEnd ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowGradientNormalBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowGradientNormalBegin ];
			}
		}
		/// <summary></summary>
		public virtual Color ArrowGradientNormalEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.arrowGradientNormalEnd ];
			}
		}
		/// <summary></summary>
		public virtual Color ThumbLinesGradientBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.thumbLinesGradientBegin ];
			}
		}
		/// <summary></summary>
		public virtual Color ThumbLinesGradientEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.thumbLinesGradientEnd ];
			}
		}
		/// <summary></summary>
		public virtual Color ThumbPressedBackgroundGradientBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.thumbPressedBackgroundGradientBegin ];
			}
		}
		/// <summary></summary>
		public virtual Color ThumbPressedBackgroundGradientEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.thumbPressedBackgroundGradientEnd ];
			}
		}
		/// <summary></summary>
		public virtual Color ScrollerBorderBegin
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.scrollerBorderBegin ];
			}
		}
		/// <summary></summary>
		public virtual Color ScrollerBorderEnd
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.scrollerBorderEnd ];
			}
		}
        /// <summary></summary>
		public virtual Color ScrollerGripDark
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.scrollerGripDark ];
			}
		}
        /// <summary></summary>
        public virtual Color ScrollerGripLight
		{
			get
			{
				return OfficeColors[ ( int )OFFICE2007COLOR.scrollerGripLight ];
			}
		}
        /// <summary></summary>
        public virtual Color ScrollerGripBackGround
		{
			get
			{
                return OfficeColors[(int)OFFICE2007COLOR.scrollerGripBackGround];
			}
		}

		/// <summary></summary>
		protected Color[ ] OfficeColors
		{
			get
			{
				if( m_office2007Colors == null )
				{
					m_office2007Colors = new Color[( int )OFFICE2007COLOR.MAX];
					InitColors( ref m_office2007Colors );
				}
				return m_office2007Colors;
			}
		}
		#endregion

		#region Methods
		
		/// <summary></summary>
		/// <param name="src"></param>
		/// <param name="dest"></param>
		/// <param name="alpha"></param>
		/// <returns></returns>
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
		/// <param name="style">Office2007 style.</param>
		/// <param name="scheme">Office2007 color scheme.</param>
		/// <returns>Color table.</returns>
		public static ColorTableOffice2007 GetColorTable( ScrollBarCustomDrawStyles style, Office2007ColorScheme scheme )
		{			
			ColorTableKey key = new ColorTableKey( style, scheme );
			ColorTableOffice2007 colorTable = (ColorTableOffice2007)s_colorTables[ key ];

			return colorTable;
		}

		/// <summary>
        /// Registers color table within internal collection.
		/// </summary>
		/// <param name="style">Office2007 style.</param>
		/// <param name="scheme">Office2007 scheme.</param>
		/// <param name="colorTable">Color table itself.</param>
		protected static void RegisterColorTable( ScrollBarCustomDrawStyles style, Office2007ColorScheme scheme, ColorTableOffice2007 colorTable )
		{
			ColorTableKey key = new ColorTableKey( style, scheme );

			s_colorTables[ key ] = colorTable;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="basicColor"></param>
		internal void UpdateColors( ScrollBarCustomDrawStyles style, Color basicColor )
		{
			Color[] office2007Colors = this.OfficeColors;
			ColorTableOffice2007 silverColors = GetColorTable( style, Office2007ColorScheme.Silver );

			for( int i = 0; i < (int)OFFICE2007COLOR.MAX_NORMAL_COLORS; ++i )
			{
				office2007Colors[i] = Office2007Colors.MergeColors( silverColors.OfficeColors[i], basicColor );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="scheme"></param>
		internal void UpdateScheme( ScrollBarCustomDrawStyles style, Office2007ColorScheme scheme )
		{
			ColorTableOffice2007 colorTable = GetColorTable( style, scheme );
			ColorTableOffice2007 newColorTable = ((IColorTableOffice2007Creator)colorTable).Create();

			RegisterColorTable( style, scheme, newColorTable );
		}

		/// <summary>
		/// Applies colors for managed scheme.
		/// </summary>
		/// <param name="form">Container form.</param>
		/// <param name="baseColor">Base color for the managed theme.</param>
		public static void ApplyManagedColors( ScrollBarCustomDrawStyles style, Color baseColor )
		{
			ColorTableOffice2007 managedColors = GetColorTable( style, Office2007ColorScheme.Managed );

			managedColors.UpdateColors( style, baseColor );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="form"></param>
		/// <param name="scheme"></param>
		public static void ApplyManagedScheme( Form form, ScrollBarCustomDrawStyles style, Office2007ColorScheme scheme )
		{
			ColorTableOffice2007 managedColors = GetColorTable( style, scheme );

			managedColors.UpdateScheme( style, scheme );

			form.Invalidate( true );
		}

		private static void ManagedColorsApplied( Office2007Colors.ManagedColorsAppliedEventArgs args )
		{
			OnManagedColorApplied( args.BaseColor );
		}

		/// <summary>
		/// Called when <see cref="ManagedColorsAppliedEvent"/> is risen.
		/// </summary>
		/// <param name="form"></param>
		/// <param name="baseColor"></param>
		protected static void OnManagedColorApplied( Color baseColor )
		{
			ApplyManagedColors( ScrollBarCustomDrawStyles.Office2007, baseColor );
			ApplyManagedColors( ScrollBarCustomDrawStyles.Office2007Generic, baseColor );
		}

		#endregion

		#region Overrides
		/// <summary></summary>
		/// <param name="colors"/>
		protected virtual void InitColors( ref Color[ ] colors )
		{
			colors[ ( int )OFFICE2007COLOR.scrollerGradientBegin ] = Color.FromArgb( 149, 176, 210 );
			colors[ ( int )OFFICE2007COLOR.scrollerGradientEnd ] = Color.FromArgb( 115, 149, 192 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientBegin ] = Color.FromArgb( 229, 234, 238 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientEnd ] = Color.FromArgb( 201, 212, 225 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedBegin ] = Color.FromArgb( 216, 232, 249 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedEnd ] = Color.FromArgb( 176, 209, 242 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedBegin ] = Color.FromArgb( 180, 209, 247 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedEnd ] = Color.FromArgb( 121, 173, 241 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderDark ] = Color.FromArgb( 88, 104, 145 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderLight ] = Color.FromArgb( 225, 231, 239 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderSelectedDark ] = Color.FromArgb( 60, 110, 176 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderSelectedLight ] = Color.FromArgb( 253, 253, 255 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderPressedDark ] = Color.FromArgb( 23, 73, 138 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderPressedLight ] = Color.FromArgb( 194, 211, 231 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientBegin ] = Color.FromArgb( 110, 126, 166 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientEnd ] = Color.FromArgb( 66, 75, 99 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientNormalBegin ] = Color.FromArgb( 110, 126, 166 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientNormalEnd ] = Color.FromArgb( 66, 75, 99 );
			colors[ ( int )OFFICE2007COLOR.thumbLinesGradientBegin ] = Color.FromArgb( 139, 142, 144 );
			colors[ ( int )OFFICE2007COLOR.thumbLinesGradientEnd ] = Color.FromArgb( 120, 126, 134 );
			colors[ ( int )OFFICE2007COLOR.thumbPressedBackgroundGradientBegin ] = Color.FromArgb( 94, 100, 117 );
			colors[ ( int )OFFICE2007COLOR.thumbPressedBackgroundGradientEnd ] = Color.FromArgb( 73, 81, 101 );
			colors[ ( int )OFFICE2007COLOR.scrollerBorderBegin ] = Color.FromArgb( 149, 176, 210 );
			colors[ ( int )OFFICE2007COLOR.scrollerBorderEnd ] = Color.FromArgb( 124, 155, 193 );
			
            colors[ ( int )OFFICE2007COLOR.scrollerGripDark ] = Color.FromArgb( 69, 93, 128 );
            colors[ ( int )OFFICE2007COLOR.scrollerGripLight ] = Color.FromArgb( 177, 201, 232 );
            colors[ ( int )OFFICE2007COLOR.scrollerGripBackGround ] = Color.FromArgb( 127, 163, 211 );
		}
		#endregion

		#region Fields
		/// <summary></summary>
		private Color[ ] m_office2007Colors;

		/// <summary>
		/// Registered color tables.
		/// </summary>
		private static IDictionary s_colorTables = new SortedList();
		#endregion

		#region *** ColorTableKey class

		internal class ColorTableKey:
			IComparable
		{
			private ScrollBarCustomDrawStyles m_style;
			private Office2007ColorScheme m_scheme;

			public ColorTableKey( ScrollBarCustomDrawStyles style, Office2007ColorScheme scheme )
			{
				m_style = style;
				m_scheme = scheme;
			}

			#region IComparable Members

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

		#region IColorTableOffice2007Creator Members

		ColorTableOffice2007 IColorTableOffice2007Creator.Create()
		{
			return new ColorTableOffice2007();
		}

		#endregion
	}
	#endregion

	#region ColorTableOffice2007Blue

	public class ColorTableOffice2007Blue:
		ColorTableOffice2007,
		IColorTableOffice2007Creator
	{
		#region Construction

		internal ColorTableOffice2007Blue()
		{
		}

		#endregion

		#region IColorTableOffice2007Creator Members

		ColorTableOffice2007 IColorTableOffice2007Creator.Create()
		{
			return new ColorTableOffice2007Blue();
		}

		#endregion
	}

	#endregion

	#region ColorTableOffice2007Silver
	/// <summary></summary>
	public class ColorTableOffice2007Silver:
		ColorTableOffice2007,
		IColorTableOffice2007Creator
	{
		#region Construction

		internal ColorTableOffice2007Silver()
		{
		}

		#endregion

		#region Overrides
		/// <summary></summary>
		/// <param name="colors"/>
		protected override void InitColors( ref Color[ ] colors )
		{
			base.InitColors( ref colors );

			colors[ ( int )OFFICE2007COLOR.scrollerGradientBegin ] = Color.FromArgb( 180, 184, 192 );
			colors[ ( int )OFFICE2007COLOR.scrollerGradientEnd ] = Color.FromArgb( 180, 184, 192 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientBegin ] = Color.FromArgb( 249, 249, 249 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientEnd ] = Color.FromArgb( 193, 193, 197 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedBegin ] = Color.FromArgb( 225, 237, 250 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedEnd ] = Color.FromArgb( 165, 211, 240 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedBegin ] = Color.FromArgb( 199, 231, 248 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedEnd ] = Color.FromArgb( 110, 201, 242 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderDark ] = Color.FromArgb( 108, 110, 113 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderLight ] = Color.FromArgb( 239, 239, 240 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientBegin ] = Color.FromArgb( 125, 132, 142 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientEnd ] = Color.FromArgb( 75, 79, 85 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientNormalBegin ] = Color.FromArgb( 125, 132, 142 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientNormalEnd ] = Color.FromArgb( 75, 79, 85 );
			colors[ ( int )OFFICE2007COLOR.thumbLinesGradientBegin ] = Color.FromArgb( 125, 134, 146 );
			colors[ ( int )OFFICE2007COLOR.thumbLinesGradientEnd ] = Color.FromArgb( 113, 127, 146 );
			colors[ ( int )OFFICE2007COLOR.scrollerBorderBegin ] = Color.FromArgb( 157, 159, 166 );
			colors[ ( int )OFFICE2007COLOR.scrollerBorderEnd ] = Color.FromArgb( 153, 156, 162 );

            colors[(int)OFFICE2007COLOR.scrollerGripDark] = Color.FromArgb(114, 118, 122);
            colors[(int)OFFICE2007COLOR.scrollerGripLight] = Color.FromArgb(205, 209, 213);
            colors[(int)OFFICE2007COLOR.scrollerGripBackGround] = Color.FromArgb(183, 186, 194);
        }
		#endregion

		#region IColorTableOffice2007Creator Members

		ColorTableOffice2007 IColorTableOffice2007Creator.Create()
		{
			return new ColorTableOffice2007Silver();
		}

		#endregion
	}
	#endregion

	#region ColorTableOffice2007Black
	/// <summary></summary>
	public class ColorTableOffice2007Black:
		ColorTableOffice2007,
		IColorTableOffice2007Creator
	{
		#region Construction

		internal ColorTableOffice2007Black()
		{
		}

		#endregion

		#region Overrides
		/// <summary></summary>
		/// <param name="colors"/>
		protected override void InitColors( ref Color[ ] colors )
		{
			base.InitColors( ref colors );

			colors[ ( int )OFFICE2007COLOR.scrollerGradientBegin ] = Color.FromArgb( 61, 61, 61 );
			colors[ ( int )OFFICE2007COLOR.scrollerGradientEnd ] = Color.FromArgb( 84, 84, 84 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientBegin ] = Color.FromArgb( 249, 249, 249 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientEnd ] = Color.FromArgb( 193, 193, 197 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedBegin ] = Color.FromArgb( 225, 237, 250 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedEnd ] = Color.FromArgb( 165, 211, 240 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedBegin ] = Color.FromArgb( 199, 231, 248 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedEnd ] = Color.FromArgb( 110, 201, 242 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderDark ] = Color.FromArgb( 52, 52, 52 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderLight ] = Color.FromArgb( 237, 237, 237 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientBegin ] = Color.FromArgb( 131, 131, 131 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientEnd ] = Color.FromArgb( 78, 78, 78 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientNormalBegin ] = Color.FromArgb( 202, 202, 202 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientNormalEnd ] = Color.FromArgb( 121, 121, 121 );
			colors[ ( int )OFFICE2007COLOR.thumbLinesGradientBegin ] = Color.FromArgb( 125, 134, 146 );
			colors[ ( int )OFFICE2007COLOR.thumbLinesGradientEnd ] = Color.FromArgb( 113, 127, 146 );
			colors[ ( int )OFFICE2007COLOR.scrollerBorderBegin ] = Color.FromArgb( 45, 45, 45 );
			colors[ ( int )OFFICE2007COLOR.scrollerBorderEnd ] = Color.FromArgb( 54, 54, 54 );

            colors[(int)OFFICE2007COLOR.scrollerGripDark] = Color.FromArgb(37, 37, 37);
            colors[(int)OFFICE2007COLOR.scrollerGripLight] = Color.FromArgb(204, 204, 204);
            colors[(int)OFFICE2007COLOR.scrollerGripBackGround] = Color.FromArgb(112, 112, 112);
        }
		#endregion

		#region IColorTableOffice2007Creator Members

		ColorTableOffice2007 IColorTableOffice2007Creator.Create()
		{
			return new ColorTableOffice2007Black();
		}

		#endregion
	}
	#endregion

    #region ColorTableGenericOffice2007Blue
    /// <summary></summary>
	public class ColorTableGenericOffice2007Blue:
		ColorTableOffice2007,
		IColorTableOffice2007Creator
	{
		#region Construction

		internal ColorTableGenericOffice2007Blue()
		{
		}

		#endregion

		#region Overrides
		/// <summary></summary>
		/// <param name="colors"/>
		protected override void InitColors( ref Color[ ] colors )
		{
			base.InitColors( ref colors );

			colors[ ( int )OFFICE2007COLOR.scrollerGradientBegin ] = SystemColors.Window;
			colors[ ( int )OFFICE2007COLOR.scrollerGradientEnd ] = SystemColors.Window;
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientBegin ] = Color.FromArgb( 229, 234, 238 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientEnd ] = Color.FromArgb( 201, 212, 225 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedBegin ] = Color.FromArgb( 216, 232, 249 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedEnd ] = Color.FromArgb( 176, 209, 242 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedBegin ] = Color.FromArgb( 180, 209, 247 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedEnd ] = Color.FromArgb( 121, 173, 241 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderDark ] = Color.FromArgb( 96, 111, 148 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderLight ] = Color.FromArgb( 225, 231, 239 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderSelectedDark ] = Color.FromArgb( 60, 110, 176 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderSelectedLight ] = Color.FromArgb( 253, 253, 255 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderPressedDark ] = Color.FromArgb( 23, 73, 138 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderPressedLight ] = Color.FromArgb( 194, 211, 231 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientBegin ] = Color.FromArgb( 110, 126, 166 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientEnd ] = Color.FromArgb( 66, 75, 99 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientNormalBegin ] = Color.FromArgb( 110, 126, 166 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientNormalEnd ] = Color.FromArgb( 66, 75, 99 );
            colors[ ( int )OFFICE2007COLOR.thumbLinesGradientBegin ] = Color.FromArgb(139, 142, 144);
			colors[ ( int )OFFICE2007COLOR.thumbLinesGradientEnd ] = Color.FromArgb( 120, 126, 134 );
			colors[ ( int )OFFICE2007COLOR.thumbPressedBackgroundGradientBegin ] = Color.FromArgb( 209, 209, 209 );
			colors[ ( int )OFFICE2007COLOR.thumbPressedBackgroundGradientEnd ] = Color.FromArgb( 217, 217, 217 );
			colors[ ( int )OFFICE2007COLOR.scrollerBorderBegin ] = SystemColors.Window;
			colors[ ( int )OFFICE2007COLOR.scrollerBorderEnd ] = SystemColors.Window;

            colors[(int)OFFICE2007COLOR.scrollerGripDark] = SystemColors.Window;
            colors[(int)OFFICE2007COLOR.scrollerGripLight] = SystemColors.Window;
            colors[(int)OFFICE2007COLOR.scrollerGripBackGround] = SystemColors.Window;
		}
		#endregion

		#region IColorTableOffice2007Creator Members

		ColorTableOffice2007 IColorTableOffice2007Creator.Create()
		{
			return new ColorTableGenericOffice2007Blue();
		}

		#endregion
	}
	#endregion

    #region ColorTableGenericOffice2007Silver
    /// <summary></summary>
    public class ColorTableGenericOffice2007Silver:
		ColorTableOffice2007,
		IColorTableOffice2007Creator
	{
		#region Construction

		internal ColorTableGenericOffice2007Silver()
		{
		}

		#endregion

		#region Overrides
		/// <summary></summary>
		/// <param name="colors"/>
		protected override void InitColors( ref Color[ ] colors )
		{
			base.InitColors( ref colors );

			colors[ ( int )OFFICE2007COLOR.scrollerGradientBegin ] = SystemColors.Window;
			colors[ ( int )OFFICE2007COLOR.scrollerGradientEnd ] = SystemColors.Window;
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientBegin ] = Color.FromArgb( 249, 249, 249 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientEnd ] = Color.FromArgb( 193, 193, 197 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedBegin ] = Color.FromArgb( 225, 237, 250 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedEnd ] = Color.FromArgb( 165, 211, 240 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedBegin ] = Color.FromArgb( 199, 231, 248 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedEnd ] = Color.FromArgb( 110, 201, 242 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderDark ] = Color.FromArgb( 157, 157, 157 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderLight ] = Color.FromArgb( 239, 239, 240 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientBegin ] = Color.FromArgb( 110, 126, 166 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientEnd ] = Color.FromArgb( 66, 75, 99 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientNormalBegin ] = Color.FromArgb( 110, 126, 166 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientNormalEnd ] = Color.FromArgb( 66, 75, 99 );
			colors[ ( int )OFFICE2007COLOR.thumbLinesGradientBegin ] = Color.FromArgb( 125, 134, 146 );
			colors[ ( int )OFFICE2007COLOR.thumbLinesGradientEnd ] = Color.FromArgb( 113, 127, 146 );
            colors[ ( int )OFFICE2007COLOR.thumbPressedBackgroundGradientBegin ] = Color.FromArgb( 210, 210, 210 );
			colors[ ( int )OFFICE2007COLOR.thumbPressedBackgroundGradientEnd ] = Color.FromArgb( 214, 214, 214 );
			colors[ ( int )OFFICE2007COLOR.scrollerBorderBegin ] = SystemColors.Window;
			colors[ ( int )OFFICE2007COLOR.scrollerBorderEnd ] = SystemColors.Window;

            colors[(int)OFFICE2007COLOR.scrollerGripDark] = SystemColors.Window;
            colors[(int)OFFICE2007COLOR.scrollerGripLight] = SystemColors.Window;
            colors[(int)OFFICE2007COLOR.scrollerGripBackGround] = SystemColors.Window;
        }
		#endregion

		#region IColorTableOffice2007Creator Members

		ColorTableOffice2007 IColorTableOffice2007Creator.Create()
		{
			return new ColorTableGenericOffice2007Silver();
		}

		#endregion
	}
	#endregion

    #region ColorTableGenericOffice2007Black
    /// <summary></summary>
    public class ColorTableGenericOffice2007Black:
		ColorTableOffice2007,
		IColorTableOffice2007Creator
	{
		#region Construction

		internal ColorTableGenericOffice2007Black()
		{
		}

		#endregion

		#region Overrides
		/// <summary></summary>
		/// <param name="colors"/>
		protected override void InitColors( ref Color[ ] colors )
		{
			base.InitColors( ref colors );

			colors[ ( int )OFFICE2007COLOR.scrollerGradientBegin ] = SystemColors.Window;
			colors[ ( int )OFFICE2007COLOR.scrollerGradientEnd ] = SystemColors.Window;
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientBegin ] = Color.FromArgb( 249, 249, 249 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientEnd ] = Color.FromArgb( 193, 193, 197 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedBegin ] = Color.FromArgb( 225, 237, 250 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientSelectedEnd ] = Color.FromArgb( 165, 211, 240 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedBegin ] = Color.FromArgb( 199, 231, 248 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonGradientPressedEnd ] = Color.FromArgb( 110, 201, 242 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderDark ] = Color.FromArgb( 157, 157, 157 );
			colors[ ( int )OFFICE2007COLOR.arrowButtonBorderLight ] = Color.FromArgb( 239, 239, 240 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientBegin ] = Color.FromArgb( 110, 126, 166 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientEnd ] = Color.FromArgb( 66, 75, 99 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientNormalBegin ] = Color.FromArgb( 110, 126, 166 );
			colors[ ( int )OFFICE2007COLOR.arrowGradientNormalEnd ] = Color.FromArgb( 66, 75, 99 );
			colors[ ( int )OFFICE2007COLOR.thumbLinesGradientBegin ] = Color.FromArgb( 125, 134, 146 );
			colors[ ( int )OFFICE2007COLOR.thumbLinesGradientEnd ] = Color.FromArgb( 113, 127, 146 );
            colors[ ( int )OFFICE2007COLOR.thumbPressedBackgroundGradientBegin ] = Color.FromArgb( 210, 210, 210 );
			colors[ ( int )OFFICE2007COLOR.thumbPressedBackgroundGradientEnd ] = Color.FromArgb( 214, 214, 214 );
			colors[ ( int )OFFICE2007COLOR.scrollerBorderBegin ] = SystemColors.Window;
			colors[ ( int )OFFICE2007COLOR.scrollerBorderEnd ] = SystemColors.Window;

            colors[(int)OFFICE2007COLOR.scrollerGripDark] = SystemColors.Window;
            colors[(int)OFFICE2007COLOR.scrollerGripLight] = SystemColors.Window;
            colors[(int)OFFICE2007COLOR.scrollerGripBackGround] = SystemColors.Window;
        }
		#endregion

		#region IColorTableOffice2007Creator Members

		ColorTableOffice2007 IColorTableOffice2007Creator.Create()
		{
			return new ColorTableGenericOffice2007Black();
		}

		#endregion
	}
	#endregion
}